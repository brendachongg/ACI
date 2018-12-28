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
    public partial class absentee_payment : BasePage
    {
        public const string PAGE_NAME = "absentee-payment.aspx";

        public const string TRAINEE_QUERY = "t";
        public const string NAME_QUERY = "n";

        private const string AMOUNT_FORMAT_STR = "#,##0.00";
        private const string CAN_EDIT_PAYMENT = "canEdit";
        private const string CAN_ADD_PAYMENT = "canAdd";

        private int rowCnt = 0;

        public absentee_payment()
            : base(PAGE_NAME, AccessRight_Constance.PAYMT_VIEW, payment_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadPaymentModes();
                tbVoidDt.Text = tbPaymentDt.Text = DateTime.Now.ToString("dd MMM yyyy");

                bool canEdit = checkAccessRights(AccessRight_Constance.PAYMT_EDIT);
                bool canAdd = checkAccessRights(AccessRight_Constance.PAYMT_NEW);
                ViewState[CAN_EDIT_PAYMENT] = canEdit;
                ViewState[CAN_ADD_PAYMENT] = canAdd;

                if (!canAdd) gvSessions.Columns[3].Visible = false;

                if(Request.QueryString[TRAINEE_QUERY]!=null && Request.QueryString[TRAINEE_QUERY]!="" &&
                    Request.QueryString[NAME_QUERY] != null && Request.QueryString[NAME_QUERY] != "")
                {
                    selectTrainee(HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]), HttpUtility.UrlDecode(Request.QueryString[NAME_QUERY]));
                }
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }

            traineesearch.selectTrainee += new trainee_search.SelectTrainee(selectTrainee);
        }

        private void loadPaymentModes()
        {
            DataTable dt = (new Finance_Management()).getPaymentModes();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving payment modes.");
                return;
            }

            //only allow nets, cheq and axs for payment
            ddlPaymentMode.DataSource = dt.Select("codeValue in ('" + PaymentMode.CHEQ.ToString() + "', '"
                + PaymentMode.NETS.ToString() + "', '" + PaymentMode.AXS.ToString() + "')").CopyToDataTable();

            ddlPaymentMode.DataBind();
            ddlPaymentMode.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void selectTrainee(string id, string name)
        {
            tbTraineeId.Text = id;
            lbTraineeName.Text = name;

            DataTable dt = (new Trainee_Management()).getTraineeProgrammeInfo(id);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving trainee details.");
                return;
            }

            lbBatchCode.Text = dt.Rows[0]["batchCode"].ToString();
            lbProgCode.Text = dt.Rows[0]["programmeCode"].ToString();
            lbProgTitle.Text = dt.Rows[0]["programmeTitle"].ToString();
            hfBatchId.Value = dt.Rows[0]["programmeBatchId"].ToString();

            loadTraineeAbsentNPayment();
        }

        private void loadTraineeAbsentNPayment()
        {
            DataTable dt = (new Finance_Management()).getTraineeAbsentPaymentDetails(tbTraineeId.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving absence details.");
                return;
            }

            gvSessions.Columns[0].Visible = true;
            gvSessions.Columns[1].Visible = true;
            gvSessions.Columns[2].Visible = true;
            gvSessions.DataSource = dt;
            gvSessions.DataBind();
            gvSessions.Columns[0].Visible = false;
            gvSessions.Columns[1].Visible = false;
            gvSessions.Columns[2].Visible = false;

            //only show the make payment button is no of unpaid session is more than 0
            if((bool)ViewState[CAN_ADD_PAYMENT]) panelMakePayment.Visible = rowCnt > 0;
        }

        protected void gvSessions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (((DataRowView)e.Row.DataItem)["paymentId"] == DBNull.Value)
            {
                e.Row.FindControl("lbtnClearPayment").Visible = false;
                e.Row.FindControl("lbtnReceipt").Visible = false;
                e.Row.FindControl("lbSpace1").Visible = false;
                e.Row.FindControl("lbSpace2").Visible = false;
                e.Row.FindControl("lbtnHistoryPayment").Visible = false;
                e.Row.FindControl("lbtnVoidPayment").Visible = false;

                e.Row.FindControl("lbPaymentDetails").Visible = false;
                rowCnt++;
            }
            else
            {
                if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
                {
                    e.Row.FindControl("lbtnClearPayment").Visible = false;
                    ((CheckBox)e.Row.FindControl("cb")).Enabled = false;
                }
                else if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.PEND.ToString())
                {
                    e.Row.FindControl("lbtnReceipt").Visible = false;
                    ((CheckBox)e.Row.FindControl("cb")).Enabled = false;
                }
                else if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.VOID.ToString())
                {
                    e.Row.FindControl("lbtnClearPayment").Visible = false;
                    e.Row.FindControl("lbtnReceipt").Visible = false;
                    e.Row.FindControl("lbSpace1").Visible = false;
                    e.Row.FindControl("lbSpace2").Visible = false;
                    e.Row.FindControl("lbtnVoidPayment").Visible = false;
                    rowCnt++;
                }

                if ((int)((DataRowView)e.Row.DataItem)["noOfPayments"] == 1)
                {
                    e.Row.FindControl("lbSpace2").Visible = false;
                    e.Row.FindControl("lbtnHistoryPayment").Visible = false;
                }

                //show the current payment details in tooltip 
                string details = "<div class='text-left'>"
                    + "<b>Payment Date: </b> " + ((DataRowView)e.Row.DataItem)["paymentDateDisp"].ToString() + "<br/>"
                    + "<b>Amount: </b> S$" + ((decimal)((DataRowView)e.Row.DataItem)["paymentAmount"]).ToString(AMOUNT_FORMAT_STR) + "<br/>"
                    + "<b>Mode: </b> " + ((DataRowView)e.Row.DataItem)["paymentModeDisp"].ToString() + "<br/>"
                    + "<b>Ref Num: </b>" + ((DataRowView)e.Row.DataItem)["referenceNumber"].ToString() + "<br/>"
                    + "<b>Remarks: </b>" + ((DataRowView)e.Row.DataItem)["paymentRemarks"].ToString() + "<br/>";

                if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.VOID.ToString())
                {
                    details += "<hr/>"
                        + "<b>Void Date: </b>" + ((DataRowView)e.Row.DataItem)["voidDateDisp"].ToString() + "<br/>"
                        + "<b>Void By: </b>" + ((DataRowView)e.Row.DataItem)["voidByName"].ToString() + "<br/>"
                        + "<b>Void Reason: </b>" + ((DataRowView)e.Row.DataItem)["voidReason"].ToString();
                }

                details += "</div>";
                ((Label)e.Row.FindControl("lbPaymentDetails")).Attributes.Add("title", details);

                if (e.Row.FindControl("lbtnReceipt").Visible) ((Label)e.Row.FindControl("lbtnReceipt")).Attributes["onClick"] = "showReceipt(" + ((DataRowView)e.Row.DataItem)["paymentId"].ToString() + ");";

                if ((bool)ViewState[CAN_EDIT_PAYMENT])
                {
                    if (e.Row.FindControl("lbtnClearPayment").Visible)
                    {
                        if (((DataRowView)e.Row.DataItem)["paymentMode"].ToString() == PaymentMode.CHEQ.ToString())
                            ((Label)e.Row.FindControl("lbtnClearPayment")).Attributes["onClick"] = "showClearChqDialog(" + ((DataRowView)e.Row.DataItem)["paymentId"].ToString() + ");";
                        else
                            ((Label)e.Row.FindControl("lbtnClearPayment")).Attributes["onClick"] = "showClearDialog(" + ((DataRowView)e.Row.DataItem)["paymentId"].ToString() + ");";    
                    }
                    if (e.Row.FindControl("lbtnVoidPayment").Visible) ((Label)e.Row.FindControl("lbtnVoidPayment")).Attributes["onClick"] = "showVoidDialog(" + ((DataRowView)e.Row.DataItem)["paymentId"].ToString() + ");";
                }
                else
                {
                    e.Row.FindControl("lbtnClearPayment").Visible = false;
                    e.Row.FindControl("lbtnVoidPayment").Visible = false;

                    if (!e.Row.FindControl("lbtnReceipt").Visible) e.Row.FindControl("lbSpace1").Visible = false;
                    if (!e.Row.FindControl("lbtnHistoryPayment").Visible) e.Row.FindControl("lbSpace2").Visible = false;
                }
                
                if (e.Row.FindControl("lbtnHistoryPayment").Visible) ((Label)e.Row.FindControl("lbtnHistoryPayment")).Attributes["onClick"] = "showPaymentHistory(" + ((DataRowView)e.Row.DataItem)["absentId"].ToString() + ");";
            }
        }

        protected void btnMakePayment_Click(object sender, EventArgs e)
        {
            int cnt = 0;
            foreach (GridViewRow gvr in gvSessions.Rows) {
                if (((CheckBox)gvr.FindControl("cb")).Checked)
                    cnt++;
            }

            if (cnt == 0)
            {
                lblError.Text = "Must select at least 1 session.";
                panelError.Visible = true;
                return;
            }

            panelPayment.Visible = true;
        }

        protected void btnVoidPayment_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).voidMakeupPayment(int.Parse(hfSelPayment.Value), DateTime.ParseExact(tbVoidDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                tbVoidReason.Text, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                tbVoidDt.Text = DateTime.Now.ToString("dd MMM yyyy");
                tbVoidReason.Text = "";

                loadTraineeAbsentNPayment();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;

                //Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "$('#diagVoidPayment').modal('show');", true);
            }
        }

        protected void btnPaidPayment_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).clearMakeupPayment(int.Parse(hfSelPayment.Value), LoginID,
                tbBankDt.Text == "" ? DateTime.MaxValue : DateTime.ParseExact(tbBankDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                tbBankDt.Text = "";

                Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showReceipt(" + hfSelPayment.Value + ");", true);

                loadTraineeAbsentNPayment();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void btnProcessPayment_Click(object sender, EventArgs e)
        {
            List<int> absId = new List<int>();
            foreach (GridViewRow gvr in gvSessions.Rows)
            {
                if (((CheckBox)gvr.FindControl("cb")).Checked)
                    absId.Add(int.Parse(gvr.Cells[0].Text));
            }

            if (absId.Count == 0)
            {
                lblError.Text = "Must select at least 1 session.";
                panelError.Visible = true;
                return;
            }

            Tuple<int, string> status = (new Finance_Management()).addMakeupPayment(tbTraineeId.Text, int.Parse(hfBatchId.Value), absId.ToArray(),
                DateTime.ParseExact(tbPaymentDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), (PaymentMode) Enum.Parse(typeof(PaymentMode), ddlPaymentMode.SelectedValue, true),
                tbPaymentRef.Text, decimal.Parse(tbPaymentAmt.Text), tbRemarks.Text == "" ? null : tbRemarks.Text, LoginID);

            if (status.Item1==-1)
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
            else
            {
                lblSuccess.Text = status.Item1==0 ? status.Item2 : "Payment saved successfully.";
                panelSuccess.Visible = true;

                if (status.Item1 == 1) Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showReceipt(" + status.Item2 + ");", true);

                loadTraineeAbsentNPayment();
                tbPaymentAmt.Text = "";
                tbPaymentDt.Text = DateTime.Now.ToString("dd MMM yyyy");
                tbPaymentRef.Text = "";
                ddlPaymentMode.SelectedIndex = 0;
                tbRemarks.Text = "";
                panelPayment.Visible = false;
            }
        }

        protected void lbtnViewPaymentHistory_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Finance_Management()).getMakeupPaymentHistory(int.Parse(hfSelSession.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving payment history.");
                return;
            }

            gvPayment.DataSource = dt;
            gvPayment.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "$('#diagPaymentHist').modal('show');", true);
        }

        protected void gvPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (((DataRowView)e.Row.DataItem)["paymentStatus"].ToString() == PaymentStatus.VOID.ToString())
            {
                string details = "<div class='text-left'>"
                         + "<b>Void Date: </b>" + ((DataRowView)e.Row.DataItem)["voidDateDisp"].ToString() + "<br/>"
                         + "<b>Void By: </b>" + ((DataRowView)e.Row.DataItem)["voidByName"].ToString() + "<br/>"
                         + "<b>Void Reason: </b>" + ((DataRowView)e.Row.DataItem)["voidReason"].ToString() + "</div>";

                ((Label)e.Row.FindControl("lbVoidDetails")).Attributes.Add("title", details);
            }
            else e.Row.FindControl("lbVoidDetails").Visible = false;
        }

    }
}