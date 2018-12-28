using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class report_management : BasePage
    {
        public const string PAGE_NAME = "report-management.aspx";

        public report_management()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.REPORT_ACIMTHLY, AccessRight_Constance.REPORT_SSGKPI, AccessRight_Constance.REPORT_FEEGRANT, AccessRight_Constance.REPORT_FULLQUALQT, AccessRight_Constance.REPORT_WTSDISB
            , AccessRight_Constance.REPORT_SFCDISB, AccessRight_Constance.REPORT_ALLS, AccessRight_Constance.REPORT_QPO, AccessRight_Constance.REPORT_FEECOLLECT, AccessRight_Constance.REPORT_FEETALLY, AccessRight_Constance.REPORT_NETS
            , AccessRight_Constance.REPORT_TRAINEE})
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnGen.UniqueID;

            panelError.Visible = false;

            if (!IsPostBack)
            {
                if (checkAccessRights(AccessRight_Constance.REPORT_ACIMTHLY)) ddlRepCat.Items.Add(new ListItem("ACI Internal Monthly Report", "1"));
                if (checkAccessRights(AccessRight_Constance.REPORT_SSGKPI)) ddlRepCat.Items.Add(new ListItem("SSG KPIs Achievement Reports", "2"));
                if (checkAccessRights(AccessRight_Constance.REPORT_FEEGRANT)) ddlRepCat.Items.Add(new ListItem("Course Fee Grant Draw Down Report", "3"));
                if (checkAccessRights(AccessRight_Constance.REPORT_FULLQUALQT)) ddlRepCat.Items.Add(new ListItem("FN Full Qual Quarterly Report", "4"));
                if (checkAccessRights(AccessRight_Constance.REPORT_WTSDISB)) ddlRepCat.Items.Add(new ListItem("WTS Disbursement Report", "5"));
                if (checkAccessRights(AccessRight_Constance.REPORT_SFCDISB)) ddlRepCat.Items.Add(new ListItem("SFC Disbursement Report", "6"));
                if (checkAccessRights(AccessRight_Constance.REPORT_ALLS)) ddlRepCat.Items.Add(new ListItem("ALLS Reports", "7"));
                if (checkAccessRights(AccessRight_Constance.REPORT_QPO)) ddlRepCat.Items.Add(new ListItem("QPO Reports", "8"));
                if (checkAccessRights(AccessRight_Constance.REPORT_FEECOLLECT)) ddlRepCat.Items.Add(new ListItem("Course Fee Collection Lists", "9"));
                if (checkAccessRights(AccessRight_Constance.REPORT_FEETALLY)) ddlRepCat.Items.Add(new ListItem("Tally Course Fee", "10"));
                if (checkAccessRights(AccessRight_Constance.REPORT_NETS)) ddlRepCat.Items.Add(new ListItem("Payment Settlement Report", "11"));
                if (checkAccessRights(AccessRight_Constance.REPORT_TRAINEE)) ddlRepCat.Items.Add(new ListItem("Trainee Particulars Report", "12"));
            }
        }

        protected void ddlRepCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlRep.Items.Clear();
            btnGen.Visible = true;
            if (ddlRepCat.SelectedValue == "")
            {
                ddlRep.Enabled = false;
                hideClearAllFilter();
                gv.Visible = false;
                lblMsg.Visible = false;
                return;
            }

            int choice = int.Parse(ddlRepCat.SelectedValue);
            switch (choice)
            {
                case 1:
                case 7:
                case 8:
                    ddlRep.Items.Add(new ListItem("Summary", "S"));
                    ddlRep.Items.Add(new ListItem("Details", "D"));
                    ddlRep.SelectedIndex = 0;
                    ddlRep.Enabled = true;
                    break;
                case 2:
                case 3:
                case 4:
                case 5:
                case 6:
                case 9:
                case 10:
                case 12:
                    ddlRep.Items.Add(new ListItem("N/A", ""));
                    ddlRep.SelectedIndex = 0;
                    ddlRep.Enabled = false;
                    break;
                case 11:
                    break;
                //case 12:
                //    //TODO: implement reports
                //    lblError.Text = "Report not yet available.";
                //    panelError.Visible = true;
                //    btnGen.Visible = false;
                //    break;
            }

            ddlRep_SelectedIndexChanged(null, null);
        }

        protected void ddlRep_SelectedIndexChanged(object sender, EventArgs e)
        {
            hideClearAllFilter();
            gv.Visible = false;
            lblMsg.Visible = false;

            int choice = int.Parse(ddlRepCat.SelectedValue);
            switch (choice)
            {
                case 1:
                    if (ddlRep.SelectedValue == "S") divFYRange.Visible = true;
                    else if (ddlRep.SelectedValue == "D") divDtRange.Visible = true;
                    break;
                case 2:
                    divFYRange.Visible = true;
                    break;
                case 3:
                    divMthYrSub.Visible = true;
                    break;
                case 4:
                    divMthYr.Visible = true;
                    break;
                case 5:
                case 6:
                    divYrSub.Visible = true;
                    break;
                case 7:
                    lbYr.Text = "";
                    divYr.Visible = true;
                    break;
                case 8:
                    if (ddlRep.SelectedValue == "S") divDtRange.Visible = true;
                    else if (ddlRep.SelectedValue == "D")
                    {
                        lbDt.Text = "Academic Year Start Date";
                        divDt.Visible = true;
                    }
                    break;
                case 9:
                    divDtDdl.Visible = true;
                    lbDtDdl.Text = "Programme Level";
                    DataTable dt = (new Programme_Management()).getProgrammeLevel();
                    if (dt == null)
                    {
                        redirectToErrorPg("Error retreiving programme levels.");
                        return;
                    }
                    ddlDtDdl.DataValueField = "codeValue";
                    ddlDtDdl.DataTextField = "codeValueDisplay";
                    ddlDtDdl.DataSource = dt;
                    ddlDtDdl.DataBind();
                    ddlDtDdl.Items.Insert(0, new ListItem("--Select--", ""));

                    rfvDtDdlStart.Enabled = false;
                    rfvDtDdlEnd.Enabled = false;
                    break;
                case 10:
                    divYr.Visible = true;
                    lbYr.Text = "Financial ";
                    break;
                case 11:
                    divDtPaymentMode.Visible = true;
                    lbDt1.Text = "Date Of Settlement";
                    lbSettlementMode.Text = "Mode Of Settlement";
                    DataTable dt1 = (new Report_Management()).getSettlementMode();


                    if (dt1 == null)
                    {
                        redirectToErrorPg("Error retreiving payment mode.");
                        return;
                    }

                    ddlSettlementMode.DataValueField = "codeValue";
                    ddlSettlementMode.DataTextField = "codeValueDisplay";
                    ddlSettlementMode.DataSource = dt1;
                    ddlSettlementMode.DataBind();
                    ddlSettlementMode.Items.Insert(0, new ListItem("--Select--", ""));

                    break;
                case 12:
                    divDtRange.Visible = true;
                    divTrainee.Visible = true;
                    break;
            }
        }

        private void hideClearAllFilter()
        {
            divDt.Visible = false;
            divDtDdl.Visible = false;
            divDtRange.Visible = false;
            divFYRange.Visible = false;
            divMthYr.Visible = false;
            divMthYrSub.Visible = false;
            divYr.Visible = false;
            divYrSub.Visible = false;
            divTrainee.Visible = false;
            divDtPaymentMode.Visible = false;

            rfvDtDdlStart.Enabled = true;
            rfvDtDdlEnd.Enabled = true;

            tbFYRangeStart.Text = tbFYRangeEnd.Text = tbDtRangeStart.Text = tbDtRangeEnd.Text = tbMthYrSubYr.Text = tbMthYrSubSub.Text = tbYrSubYr.Text =
                tbYrSubSub.Text = tbMthYrYr.Text = tbYr.Text = tbDt.Text = tbDtDdlStart.Text = tbDtDdlEnd.Text = tbClassCode.Text = "";
            ddlMthYrSubMth.SelectedIndex = ddlMthYrMth.SelectedIndex = 0;
            ddlDtDdl.Items.Clear();
        }

        protected void btnGen_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = null;
            DataTable dt = null;
            Report_Management rm = new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), LoginID);
            string displayFormat = null;

            int choice = int.Parse(ddlRepCat.SelectedValue);
            switch (choice)
            {
                case 1:
                    if (ddlRep.SelectedValue == "S") status = rm.genACIMonthlySummary(int.Parse(tbFYRangeStart.Text), int.Parse(tbFYRangeEnd.Text));
                    else if (ddlRep.SelectedValue == "D") status = rm.genACIMonthlyDetails(DateTime.ParseExact(tbDtRangeStart.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        DateTime.ParseExact(tbDtRangeEnd.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                    displayFormat = "FILE";
                    break;
                case 2:
                    dt = rm.genSSGKPIAchievement(int.Parse(tbFYRangeStart.Text), int.Parse(tbFYRangeEnd.Text));
                    displayFormat = "GV";
                    lblMsg.Text = "*For full qualification and WSQ short course, HC is based on trainee who is competent is all modules of the class. For non WSQ short course, HC is based on enrolled trainees.";
                    lblMsg.Visible = true;
                    break;
                case 3:
                    status = rm.genCseFeeDrawnDown(int.Parse(ddlMthYrSubMth.SelectedValue), int.Parse(tbMthYrSubYr.Text), tbMthYrSubSub.Text);
                    displayFormat = "FILE";
                    break;
                case 4:
                    status = rm.genFullQualQuarter(int.Parse(ddlMthYrMth.SelectedValue), int.Parse(tbMthYrYr.Text));
                    displayFormat = "FILE";
                    break;
                case 5:
                    status = rm.genWTSDisbursement(int.Parse(tbYrSubYr.Text), tbYrSubSub.Text);
                    displayFormat = "FILE";
                    break;
                case 6:
                    status = rm.genSFCDisbursement(int.Parse(tbYrSubYr.Text), tbYrSubSub.Text);
                    displayFormat = "FILE";
                    break;
                case 7:
                    if (ddlRep.SelectedValue == "S")
                    {
                        dt = rm.genALLSSummary(int.Parse(tbYr.Text));
                        displayFormat = "GV";
                    }
                    else if (ddlRep.SelectedValue == "D")
                    {
                        status = rm.genALLSDetails(int.Parse(tbYr.Text));
                        displayFormat = "FILE";
                    }
                    break;
                case 8:
                    if (ddlRep.SelectedValue == "S")
                    {
                        dt = rm.genQPOSummary(DateTime.ParseExact(tbDtRangeStart.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), DateTime.ParseExact(tbDtRangeEnd.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                        displayFormat = "GV";
                    }
                    else if (ddlRep.SelectedValue == "D")
                    {
                        status = rm.genQPODetails(DateTime.ParseExact(tbDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                        displayFormat = "FILE";
                    }
                    break;
                case 9:
                    status=rm.genCourseFeeCollection(ddlDtDdl.SelectedValue, tbDtDdlStart.Text == "" ? DateTime.MinValue : DateTime.ParseExact(tbDtDdlStart.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        tbDtDdlEnd.Text == "" ? DateTime.MaxValue : DateTime.ParseExact(tbDtDdlEnd.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                    displayFormat = "FILE";
                    break;
                case 10:
                    status=rm.genCourseFeeReceived(int.Parse(tbYr.Text));
                    displayFormat = "FILE";
                    break;

                case 11:
                    status = rm.getNetsSettlement(DateTime.ParseExact(tbDt1.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), ddlSettlementMode.SelectedValue); 
                    displayFormat = "FILE";
                    break;
                case 12:
                    if (!cbTrParticulars.Checked && !cbTrModResult.Checked)
                    {
                        lblError.Text = "At least 1 type of data must be selected.";
                        panelError.Visible = true;
                        return;
                    }
                    status = rm.genTraineeReport(tbDtRangeStart.Text == "" ? DateTime.MinValue : DateTime.ParseExact(tbDtRangeStart.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        tbDtRangeEnd.Text == "" ? DateTime.MaxValue : DateTime.ParseExact(tbDtRangeEnd.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        cbTrParticulars.Checked, cbTrModResult.Checked, tbClassCode.Text);
                    displayFormat = "FILE";
                    break;
            }

            if (displayFormat == "FILE")
            {
                if (status.Item1)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "export", "window.open('" + report_export.PAGE_NAME + "?" + report_export.FILE_QUERY + "=" + HttpUtility.UrlEncode(status.Item2)
                       + "', '_blank', 'menubar=no,location=no');", true);
                }
                else
                {
                    lblError.Text = status.Item2;
                    panelError.Visible = true;
                }
            }
            else if (displayFormat == "GV")
            {
                if (dt == null)
                {
                    redirectToErrorPg("Error retrieving data.");
                    return;
                }

                gv.Visible = true;
                gv.DataSource = dt;
                gv.DataBind();
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbFYRangeStart.Text = tbFYRangeEnd.Text = tbDtRangeStart.Text = tbDtRangeEnd.Text = tbMthYrSubYr.Text = tbMthYrSubSub.Text = tbYrSubYr.Text =
                tbYrSubSub.Text = tbMthYrYr.Text = tbYr.Text = tbDt.Text = tbDtDdlStart.Text = tbDtDdlEnd.Text = "";
            ddlMthYrSubMth.SelectedIndex = ddlMthYrMth.SelectedIndex = 0;

            if (ddlDtDdl.Items.Count > 0) ddlDtDdl.SelectedIndex = 0;
        }


    }
}