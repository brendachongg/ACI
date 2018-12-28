using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;
using System.Drawing;
using System.Globalization;

namespace ACI_TMS
{
    public partial class applicant_programme_payment : BasePage
    {
        public const string PAGE_NAME = "applicant-programme-payment.aspx";

        public const string APPLICANT_QUERY = "a";
        public const string TRAINEE_QUERY = "tr";
        public const string PREV_QUERY = "f";
        public const string TYPE_QUERY = "t";

        private const string PAYMENT_DATA = "payment";
        private const string SUBSIDY_DATA = "subsidy";
        private const string AMOUNT_FORMAT_STR = "#,##0.00";
        private const string CAN_EDIT_PAYMENT = "canEdit";

        private decimal amtPaid = 0;
        private Finance_Management fm = new Finance_Management();
        private string exemptedModules = "";

        public applicant_programme_payment()
            : base(PAGE_NAME, AccessRight_Constance.PAYMT_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[PREV_QUERY] != null && Request.QueryString[PREV_QUERY] == "A")
                {
                    if (Request.QueryString[APPLICANT_QUERY] != null && Request.QueryString[APPLICANT_QUERY] != "")
                        hfRtn.Value = applicant_details.PAGE_NAME + "?a=" + Request.QueryString[APPLICANT_QUERY];
                    else
                        hfRtn.Value = applicant.PAGE_NAME;
                }else hfRtn.Value = payment_management.PAGE_NAME;

                PrevPage = hfRtn.Value;

                if ((Request.QueryString[APPLICANT_QUERY] == null || Request.QueryString[APPLICANT_QUERY] == "") && (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == ""))
                {
                    redirectToErrorPg("Missing applicant/trainee information.");
                    return;
                }

                if (Request.QueryString[TYPE_QUERY] == null || Request.QueryString[TYPE_QUERY] == "" || HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == PaymentType.BOTH.ToString())
                {
                    lbHeader.Text = "Combined";
                    hfType.Value = PaymentType.BOTH.ToString();
                    tbGST.Attributes.Add("style", "padding-right:60px");
                }
                else if (HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == PaymentType.REG.ToString())
                {
                    lbHeader.Text = "Registration";
                    hfType.Value = PaymentType.REG.ToString();
                }
                else
                {
                    hfType.Value = PaymentType.PROG.ToString();
                    lbHeader.Text = "Programme"; 
                    tbGST.Attributes.Add("style", "padding-right:60px");
                }

                if (Request.QueryString[APPLICANT_QUERY] != null && Request.QueryString[APPLICANT_QUERY]!="")
                    lbApplicantId.Text = HttpUtility.UrlDecode(Request.QueryString[APPLICANT_QUERY]);
                else lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);

                DataTable dtTypes = lbApplicantId.Text == "" ? fm.getTraineeClassPaymentTypes(lbTraineeId.Text) : fm.getApplnClassPaymentTypes(lbApplicantId.Text);
                if (dtTypes == null)
                {
                    redirectToErrorPg("Error retrieving payment details.");
                    return;
                }
                if (dtTypes.Rows.Count > 0)
                {
                    //check if previous payment record is of the same type, if not, do not allow to proceed
                    if (hfType.Value == PaymentType.BOTH.ToString() && dtTypes.Rows[0][0].ToString() != PaymentType.BOTH.ToString())
                    {
                        redirectToErrorPg("Payment has already been split, unable to combined.");  
                        return;
                    }
                    else if (hfType.Value != PaymentType.BOTH.ToString() && dtTypes.Rows[0][0].ToString() == PaymentType.BOTH.ToString())
                    {
                        redirectToErrorPg("Payment has already been combined, unable to split.");
                        return;
                    }
                }

                bool canEdit = checkAccessRights(AccessRight_Constance.PAYMT_EDIT);
                bool canAdd = checkAccessRights(AccessRight_Constance.PAYMT_NEW);
                ViewState[CAN_EDIT_PAYMENT] = canEdit;
                panelNewPayment.Visible = panelNewPaymentLegend.Visible = canAdd;
                //if cannot add new payment and cannot edit, then do not show the save payment button
                if (!canEdit && !canAdd) 
                { 
                    btnProcessPayment.Visible = false;
                    lbtnSetRegFee.Visible = false;
                    tbRegFee.ReadOnly = true;
                }

                loadApplicantOrTraineeDetails();
                loadPaymentModes();

                if (hfType.Value == PaymentType.REG.ToString())
                {
                    trProg.Visible = false;
                    rpModules.Visible = false;
                    trSubsidy.Visible = false;

                    //for payment of registration fees, GST cannot be changed
                    lbtnSetGST.Visible = false;
                    lbtnCalGST.Visible = false;
                    tbGST.ReadOnly = true;

                    calculateNetTotal();
                }
                else
                {
                    if (hfType.Value == PaymentType.PROG.ToString()) trReg.Visible = false;
                    loadBatchModules();
                }

                loadPaymentDetails();
                tbPaymentDt.Text = tbVoidDt.Text = DateTime.Now.ToString("dd MMM yyyy");
            }
            else
            {
                PrevPage = hfRtn.Value;
                //need to have a hidden field to store gst also because during payment for registration fee only where gst cannot be change,
                //the calculated gst by the javascript is not captured by code behind as the text field is marked readonly
                tbGST.Text = hfGST.Value;
                panelSuccess.Visible = false;
                panelError.Visible = false;

                //need to call this method on every post back because .net will not keep track of changes done to labels thru javascript 
                //(ie changing of registration fees that result in changing the outstanding amt which is label)
                calculateNetTotal();
            }
        }

        private void loadSubsidy()
        {
            DataTable dt = fm.getAvailableSubsidy(int.Parse(hfProgrammeId.Value));

            ViewState[SUBSIDY_DATA] = dt;
            ddlSubsidy.DataSource = dt;
            ddlSubsidy.DataBind();
            ddlSubsidy.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadPaymentModes()
        {
            DataTable dt = fm.getPaymentModes();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving payment modes.");
                return;
            }

            if (hfType.Value == PaymentType.REG.ToString()) //only left nets and chqeue payment for registration fee
                ddlPaymentMode.DataSource = dt.Select("codeValue in ('" + PaymentMode.CHEQ.ToString() + "', '" 
                    + PaymentMode.NETS.ToString() + "', '" + PaymentMode.AXS.ToString() + "')").CopyToDataTable();
            else ddlPaymentMode.DataSource = dt;
            ddlPaymentMode.DataBind();
            ddlPaymentMode.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadApplicantOrTraineeDetails()
        {
            DataTable dt = lbApplicantId.Text == "" ? (new Trainee_Management()).getTraineeDetailsForPayment(lbTraineeId.Text) : (new Applicant_Management()).getApplicantDetailsForPayment(lbApplicantId.Text);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving applicant's or trainee's details.");
                return;
            }

            DataRow dr = dt.Rows[0];
            lbApplName.Text = dr["fullName"].ToString();
            //encrypted
            hfNric.Value = dr["idNumber"].ToString();
            lbApplDate.Text = ((DateTime)dr["applicationDate"]).ToString("dd MMM yyyy hh:mm tt");
            lbProgTitle.Text = dr["programmeTitle"].ToString();
            lbProgCseCode.Text = dr["courseCode"].ToString();
            lbBatchProjCode.Text = dr["projectCode"].ToString();
            lbBatchCode.Text = dr["batchCode"].ToString();
            hfBundleId.Value = dr["bundleId"].ToString();
            hfBatchId.Value = dr["programmeBatchId"].ToString();
            hfProgrammeId.Value = dr["programmeId"].ToString();

            if (dr["selfSponsored"] != DBNull.Value && dr["selfSponsored"].ToString() == Sponsorship.SELF.ToString()) lbSelfSponsored.Text = "(Self-sponsored)";

            if (lbApplicantId.Text != "") exemptedModules = dr["applicantExemModule"] == DBNull.Value ? "" : dr["applicantExemModule"].ToString();

            if (hfType.Value == PaymentType.REG.ToString())
            {
                if (dr["registrationFee"] != DBNull.Value) tbRegFee.Text = ((decimal)dr["registrationFee"]).ToString(AMOUNT_FORMAT_STR);
                else
                {
                    decimal regFee = fm.getCurrentRegFee();
                    tbRegFee.Text = regFee == -1 ? "" : regFee.ToString(AMOUNT_FORMAT_STR);
                }

                tbGST.Text = hfGST.Value = Math.Round(decimal.Parse(tbRegFee.Text == "" ? "0" : tbRegFee.Text) * General_Constance.GST_RATE, 2).ToString(AMOUNT_FORMAT_STR);
            }
            else if (hfType.Value == PaymentType.PROG.ToString())
            {
                decimal progFee = (decimal)dr["programmePayableAmount"];
                decimal subAmt = 0;

                lbProgFee.Text = progFee.ToString(AMOUNT_FORMAT_STR);
                loadSubsidy();
                if (dr["subsidyId"] != DBNull.Value)
                {
                    ddlSubsidy.SelectedValue = dr["subsidyId"].ToString();
                    hfSubsidyAmt.Value = dr["subsidyAmt"].ToString();
                    subAmt = (decimal)dr["subsidyAmt"];
                    lbSubsidyAmt.Text = "(" + subAmt.ToString(AMOUNT_FORMAT_STR) + ")";
                }

                if (dr["GSTPayableAmount"] == DBNull.Value)
                {
                    //calculate
                    decimal gst = Math.Round((progFee - subAmt) * General_Constance.GST_RATE, 2);
                    tbGST.Text = hfGST.Value = gst.ToString(AMOUNT_FORMAT_STR);
                }
                else tbGST.Text = hfGST.Value = ((decimal)dr["GSTPayableAmount"]).ToString(AMOUNT_FORMAT_STR);
            }
            else
            {
                //combined payment
                decimal progFee = (decimal)dr["programmePayableAmount"];
                decimal subAmt = 0, regFee = 0;

                if (dr["registrationFee"] != DBNull.Value)
                {
                    regFee = (decimal)dr["registrationFee"];
                    tbRegFee.Text = regFee.ToString(AMOUNT_FORMAT_STR);
                }
                else
                {
                    regFee = fm.getCurrentRegFee();
                    tbRegFee.Text = regFee == -1 ? "" : regFee.ToString(AMOUNT_FORMAT_STR);
                }

                lbProgFee.Text = progFee.ToString(AMOUNT_FORMAT_STR);
                loadSubsidy();
                if (dr["subsidyId"] != DBNull.Value)
                {
                    ddlSubsidy.SelectedValue = dr["subsidyId"].ToString();
                    hfSubsidyAmt.Value = dr["subsidyAmt"].ToString();
                    subAmt = (decimal)dr["subsidyAmt"];
                    lbSubsidyAmt.Text = "(" + subAmt.ToString(AMOUNT_FORMAT_STR) + ")";
                }

                if (dr["GSTPayableAmount"] == DBNull.Value)
                {
                    //calculate
                    decimal gst = Math.Round((progFee - subAmt + regFee) * General_Constance.GST_RATE, 2);
                    tbGST.Text = hfGST.Value = gst.ToString(AMOUNT_FORMAT_STR);
                }
                else tbGST.Text = hfGST.Value = ((decimal)dr["GSTPayableAmount"]).ToString(AMOUNT_FORMAT_STR);
            }            
        }

        private void loadBatchModules()
        {
            DataTable dt = (new Bundle_Management()).getBundleModule(int.Parse(hfBundleId.Value), true);
            if (dt == null || dt.Rows.Count==0)
            {
                redirectToErrorPg("Error retrieving programme's modules.");
                return;
            }

            dt.Columns.Add(new DataColumn("isExempted", typeof(string)));
            if (exemptedModules != "")
            {
                string[] exem = exemptedModules.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (DataRow dr in dt.Rows)
                {
                    if (exem.Contains(dr["moduleId"].ToString()))
                        dr["isExempted"] = "<span style='color:red;'>[Exempted]&nbsp;</span>";
                }
            }
            else if (lbTraineeId.Text != "")
            {
                DataTable dtExem = (new Trainee_Management()).getTraineeExemMod(lbTraineeId.Text);
                if (dtExem == null)
                {
                    redirectToErrorPg("Error retrieving trainee's exempted modules.");
                    return;
                }

                foreach (DataRow dr in dt.Rows)
                {
                    if (dtExem.Select("moduleId=" + dr["moduleId"].ToString()).Length > 0)
                        dr["isExempted"] = "<span style='color:red;'>[Exempted]&nbsp;</span>";
                }
            }

            rpModules.DataSource = dt;
            rpModules.DataBind();
        }

        private void loadPaymentDetails()
        {
            DataTable dt = lbApplicantId.Text == "" ? fm.getAllTraineeClassPaymentDetails(lbTraineeId.Text, hfType.Value == PaymentType.REG.ToString() ? PaymentType.REG :
                    hfType.Value == PaymentType.PROG.ToString() ? PaymentType.PROG : PaymentType.BOTH) 
                : fm.getAllApplnClassPaymentDetails(lbApplicantId.Text,
                    hfType.Value == PaymentType.REG.ToString() ? PaymentType.REG:
                    hfType.Value == PaymentType.PROG.ToString() ? PaymentType.PROG : PaymentType.BOTH);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving payment details.");
                return;
            }

            //for payments loaded from db, indicate no changes
            dt.Columns.Add(new DataColumn("isChanged", typeof(bool)));
            foreach (DataRow dr in dt.Rows) dr["isChanged"] = false;

            populatePayments(dt);
        }

        private void populatePayments(DataTable dt)
        {
            ViewState[PAYMENT_DATA] = dt;

            amtPaid = 0;
            gvPayment.Columns[0].Visible = true;
            gvPayment.DataSource = dt;
            gvPayment.DataBind();
            gvPayment.Columns[0].Visible = false;
            hfAmtPaid.Value = amtPaid.ToString();
            lbAmtPaid.Text = "S$" + amtPaid.ToString(AMOUNT_FORMAT_STR);

            calculateNetTotal();
        }

        protected void gvPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            if ((e.Row.RowState & DataControlRowState.Edit) > 0) return;

            if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.VOID.ToString())
            {
                //payment status is void, show void details and hide clear and void payment btn
                string details = "<div class='text-left'><b>Date: </b>" + ((DateTime)((DataRowView)e.Row.DataItem)["voidDate"]).ToString("dd MMM yyyy")
                    + "<br/><b>By: </b>" + ((DataRowView)e.Row.DataItem)["voidByName"].ToString()
                    + "<br/><b>Reason: </b>" + ((DataRowView)e.Row.DataItem)["voidReason"].ToString()
                    + "</div>";
                ((Label)e.Row.FindControl("lbVoidDetails")).Attributes.Add("title", details);

                e.Row.FindControl("lbtnEdit").Visible = false;
                e.Row.FindControl("lbSpace1").Visible = false;
                e.Row.FindControl("lbtnClearPayment").Visible = false;
                e.Row.FindControl("lbtnVoidPayment").Visible = false;
            }
            else if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
            {
                //payment status is paid, hide void details and clear payment
                e.Row.FindControl("lbVoidDetails").Visible = false;
                e.Row.FindControl("lbtnClearPayment").Visible = false;
                e.Row.FindControl("lbSpace").Visible = false;

                amtPaid += (decimal)((DataRowView)e.Row.DataItem)["paymentAmount"];
            }
            else 
            {
                //payment status is pending, hide void details and show clear and void payment btn
                e.Row.FindControl("lbVoidDetails").Visible = false;

                if (((DataRowView)e.Row.DataItem)["paymentMode"].ToString() == PaymentMode.CHEQ.ToString())
                    ((Label)e.Row.FindControl("lbtnClearPayment")).Attributes["onClick"] = "showClearChqDialog(" + e.Row.RowIndex + ");";
                else
                    ((Label)e.Row.FindControl("lbtnClearPayment")).Attributes["onClick"] = "showClearDialog(" + e.Row.RowIndex + ");";
            }

            //only rows without payment id (haven save to db) can be deleted
            if (((DataRowView)e.Row.DataItem)["paymentId"] == DBNull.Value)
            {
                ((Label)e.Row.FindControl("lbtnVoidPayment")).Attributes["onClick"] = "showVoidDelDialog(" + e.Row.RowIndex + ");"; 
            }
            else
            {
                if ((bool)(ViewState[CAN_EDIT_PAYMENT]))
                {
                    //if have payment id, can only void payment
                    ((Label)e.Row.FindControl("lbtnVoidPayment")).Attributes["onClick"] = "showVoidDialog2(" + e.Row.RowIndex + ");";
                }
                else
                {
                    //no right to edit payment that has already been saved to database
                    e.Row.FindControl("lbVoidDetails").Visible = false;
                    e.Row.FindControl("lbtnClearPayment").Visible = false;
                    e.Row.FindControl("lbSpace").Visible = false;
                }
            }

            //if there are changes in payment highlight the row
            if ((bool)((DataRowView)e.Row.DataItem)["isChanged"]) e.Row.BackColor = System.Drawing.Color.Khaki;

            //determine if should show receipt
            if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.PAID.ToString() && ((DataRowView)e.Row.DataItem)["receiptNumber"] != DBNull.Value)
            {
                ((Label)e.Row.FindControl("lbtnReceipt")).Attributes["onClick"] = "showReceipt('" + lbApplicantId.Text + "', '" + lbTraineeId.Text + "', '"
                    + hfType.Value + "', " + ((DataRowView)e.Row.DataItem)["paymentId"].ToString() + ");";
                e.Row.FindControl("lbSpace").Visible = true;
            }
            else e.Row.FindControl("lbtnReceipt").Visible = false;
        }

        protected void btnClearPayment_Click(object sender, EventArgs e)
        {
            tbPaymentDt.Text = DateTime.Now.ToString("dd MMM yyyy");
            ddlPaymentMode.SelectedIndex = 0;
            tbPaymentRef.Text = "";
            tbPaymentAmt.Text = "";
            tbRemarks.Text = "";
        }

        protected void btnAddPayment_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[PAYMENT_DATA] as DataTable;

            DataRow dr = dt.NewRow();
            dr["paymentMode"] = ddlPaymentMode.SelectedValue;
            dr["paymentModeDisp"] = ddlPaymentMode.SelectedItem.Text;
            dr["referenceNumber"] = tbPaymentRef.Text;
            dr["paymentDate"] = tbPaymentDt.Text;
            dr["paymentAmount"] = decimal.Parse(tbPaymentAmt.Text);
            if (tbRemarks.Text != "") dr["paymentRemarks"] = tbRemarks.Text;
            dr["isChanged"] = true;
            //if payment by nets or axs, immediately approve, else pending clearence
            if (ddlPaymentMode.SelectedValue == PaymentMode.NETS.ToString() || ddlPaymentMode.SelectedValue == PaymentMode.AXS.ToString())
            {
                dr["paymentStatus"] = PaymentStatus.PAID.ToString();
                dr["paymentStatusDisp"] = "Paid";
            }
            else
            {
                dr["paymentStatus"] = PaymentStatus.PEND.ToString();
                dr["paymentStatusDisp"] = "Pending clearence";
            }

            dr["paymentType"] = hfType.Value;

            dt.Rows.Add(dr);

            populatePayments(dt);

            btnClearPayment_Click(null, null);
        }

        protected void ddlSubsidy_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlSubsidy.SelectedValue == "")
            {
                lbSubsidyAmt.Text = "(0.00)";
                hfSubsidyAmt.Value = "0";
            }
            else
            {
                DataTable dt = ViewState[SUBSIDY_DATA] as DataTable;
                DataRow dr = dt.Select("subsidyId=" + ddlSubsidy.SelectedValue)[0];
                decimal amt;
                if (dr["subsidyType"].ToString() == SubsidyType.AMT.ToString())
                {
                    if (decimal.Parse(lbProgFee.Text) < (decimal)dr["subsidyValue"]) amt = decimal.Parse(lbProgFee.Text);
                    else amt = (decimal)dr["subsidyValue"];
                }
                else
                {
                    //percentage
                    amt = Math.Round(decimal.Parse(lbProgFee.Text) * (decimal)dr["subsidyValue"], 2);
                }
                lbSubsidyAmt.Text = "(" + amt.ToString(AMOUNT_FORMAT_STR) + ")";
                hfSubsidyAmt.Value = amt.ToString();
            }

            calculateNetTotal();
        }

        private void calculateNetTotal()
        {
            decimal total = decimal.Parse(tbGST.Text);
            if (trReg.Visible) total += decimal.Parse(tbRegFee.Text);
            if (trProg.Visible) total += decimal.Parse(lbProgFee.Text);
            if (trSubsidy.Visible) total -= decimal.Parse(hfSubsidyAmt.Value);
            lbNetTotalValue.Text = total.ToString(AMOUNT_FORMAT_STR);

            decimal outstanding = total - decimal.Parse(hfAmtPaid.Value);
            hfAmtOutstanding.Value = outstanding.ToString();
            lbAmtOutstanding.Text = "S$" + outstanding.ToString(AMOUNT_FORMAT_STR);
        }

        protected void btnPaidPayment_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[PAYMENT_DATA] as DataTable;
            DataRow dr = dt.Rows[int.Parse(hfSelPayment.Value)];

            dr["paymentStatus"] = PaymentStatus.PAID.ToString();
            dr["paymentStatusDisp"] = "Paid";
            dr["isChanged"] = true;

            populatePayments(dt);
        }

        protected void btnClearChq_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[PAYMENT_DATA] as DataTable;
            DataRow dr = dt.Rows[int.Parse(hfSelPayment.Value)];

            dr["paymentStatus"] = PaymentStatus.PAID.ToString();
            dr["paymentStatusDisp"] = "Paid";
            dr["bankInDate"] = DateTime.ParseExact(tbBankDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            dr["isChanged"] = true;

            populatePayments(dt);
        }

        protected void btnDelPayment_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[PAYMENT_DATA] as DataTable;
            DataRow dr = dt.Rows[int.Parse(hfSelPayment.Value)];

            dt.Rows.Remove(dr);

            populatePayments(dt);
        }

        protected void btnVoidPayment_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[PAYMENT_DATA] as DataTable;
            DataRow dr = dt.Rows[int.Parse(hfSelPayment.Value)];

            dr["voidDate"] = DateTime.ParseExact(tbVoidDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            dr["voidBy"] = LoginID;
            dr["voidByName"] = LoginName;
            dr["voidReason"] = tbVoidReason.Text;
            dr["paymentStatus"] = PaymentStatus.VOID.ToString();
            dr["paymentStatusDisp"] = "Void";
            dr["bankInDate"] = DBNull.Value;
            dr["isChanged"] = true;

            tbVoidDt.Text = DateTime.Now.ToString("dd MMM yyyy");
            tbVoidReason.Text = "";

            populatePayments(dt);
        }

        protected void btnProcessPayment_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = lbApplicantId.Text == "" ? fm.updateTraineeClassPayment(lbTraineeId.Text, int.Parse(hfBatchId.Value), hfNric.Value, trReg.Visible ? decimal.Parse(tbRegFee.Text) : -1,
                trProg.Visible ? decimal.Parse(lbProgFee.Text) : -1, ddlSubsidy.SelectedValue == "" ? -1 : int.Parse(ddlSubsidy.SelectedValue), ddlSubsidy.SelectedValue == "" ? -1 : decimal.Parse(hfSubsidyAmt.Value),
                decimal.Parse(tbGST.Text), ViewState[PAYMENT_DATA] as DataTable, LoginID) :
                fm.updateApplnClassPayment(lbApplicantId.Text, int.Parse(hfBatchId.Value), hfNric.Value, trReg.Visible ? decimal.Parse(tbRegFee.Text) : -1,
                trProg.Visible ? decimal.Parse(lbProgFee.Text) : -1, ddlSubsidy.SelectedValue == "" ? -1 : int.Parse(ddlSubsidy.SelectedValue), ddlSubsidy.SelectedValue == "" ? -1 : decimal.Parse(hfSubsidyAmt.Value), 
                decimal.Parse(tbGST.Text), ViewState[PAYMENT_DATA] as DataTable, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                loadPaymentDetails();    
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void btnShowPayment_Click(object sender, EventArgs e)
        {
            panelPayment.Visible = true;
            panelShowPayment.Visible = false;
        }

        protected void btnRevert_Click(object sender, EventArgs e)
        {
            loadPaymentDetails();
        }

        protected void gvPayment_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvPayment.EditIndex = e.NewEditIndex;
            gvPayment.DataSource = ViewState[PAYMENT_DATA] as DataTable;
            gvPayment.DataBind();
        }

        protected void gvPayment_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvPayment.EditIndex = -1;
            gvPayment.DataSource = ViewState[PAYMENT_DATA] as DataTable;
            gvPayment.DataBind();
        }

        protected void gvPayment_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int selectedRow = e.RowIndex;
            GridViewRow row = (GridViewRow)gvPayment.Rows[selectedRow];

            TextBox tbRemarks = row.FindControl("tbEditRemarks") as TextBox;
            DataRow dr = ((DataTable)ViewState[PAYMENT_DATA]).Rows[selectedRow];
            if (!dr["referenceNumber"].ToString().Equals(((TextBox)row.FindControl("tbEditRef")).Text) ||
                (dr["paymentRemarks"] == DBNull.Value && tbRemarks.Text != "") || (dr["paymentRemarks"] != DBNull.Value && tbRemarks.Text == "")
                || (!dr["paymentRemarks"].ToString().Equals(tbRemarks.Text)))
            {
                dr["isChanged"] = true;
                dr["referenceNumber"] = ((TextBox)row.FindControl("tbEditRef")).Text;
                dr["paymentRemarks"] = tbRemarks.Text == "" ? (object)DBNull.Value : tbRemarks.Text;
            }

            gvPayment.EditIndex = -1;
            gvPayment.DataSource = ViewState[PAYMENT_DATA] as DataTable;
            gvPayment.DataBind();             
        }

        protected void lbtnCalGST_Click(object sender, EventArgs e)
        {

        }

        protected void btnUpdateApplSusbidy_Click(object sender, EventArgs e)
        {
            fm.updateApplSubsidy(lbApplicantId.Text, trReg.Visible ? decimal.Parse(tbRegFee.Text) : -1,
              trProg.Visible ? decimal.Parse(lbProgFee.Text) : -1, ddlSubsidy.SelectedValue == "" ? -1 : int.Parse(ddlSubsidy.SelectedValue), ddlSubsidy.SelectedValue == "" ? -1 : decimal.Parse(hfSubsidyAmt.Value),
              decimal.Parse(tbGST.Text), LoginID);
        }
    }
}