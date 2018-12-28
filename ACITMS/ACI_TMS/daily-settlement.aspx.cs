using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Data;
using GeneralLayer;
using LogicLayer;

namespace ACI_TMS
{
    public partial class daily_payment : BasePage
    {
        public const string PAGE_NAME = "daily-settlement.aspx";
        public daily_payment()
            : base(PAGE_NAME, AccessRight_Constance.DAILY_PAYMENT)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!checkAccessRights(AccessRight_Constance.DAILY_PAYMENT))
                {
                    redirectToErrorPg("You have not access right to this module");
                }



            }

        }

        private void populatePayment()
        {
            //string dtToday = "09 Mar 2018";
            DateTime dtTdy = DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

            Report_Management rptMgm = new Report_Management();

            DataSet ds = new DataSet();

            DataTable dt = new DataTable("Payment");
            dt.Columns.Add(new DataColumn("totalSubsidy", typeof(decimal)));
            dt.Columns.Add(new DataColumn("totalPaymentFee", typeof(decimal)));
            dt.Columns.Add(new DataColumn("paymentMode", typeof(string)));
            dt.Columns.Add(new DataColumn("paymentModeDisplay", typeof(string)));


            DataTable dtPaymentModes = rptMgm.getSettlementMode();
            DataTable allPayment = new DataTable();

            foreach (DataRow r in dtPaymentModes.Rows)
            {

                string paymentMode = r["codeValue"].ToString();
                string paymentModeDisplay = r["codeValueDisplay"].ToString();



                if (paymentMode == PaymentMode.NETS.ToString() || paymentMode == PaymentMode.CHEQ.ToString() || paymentMode == PaymentMode.AXS.ToString() || paymentMode == PaymentMode.PSEA.ToString())
                {
                    DataTable dtPayment = rptMgm.getNetSettlement(dtTdy, paymentMode);

                    if (dtPayment != null)
                    {
                        dtPayment.Columns.Add(new DataColumn("idNumberMasked", typeof(string)));
                        foreach (DataRow dr in dtPayment.Rows)
                        {
                            dr["idNumber"] = (new Cryptography()).decryptInfo(dr["idNumber"].ToString());
                            //dr["regGst"] = Convert.ToDecimal(dr["reggst"]).ToString("#.##");
                            dr["idNumberMasked"] = dr["idNumber"].ToString().Substring(0, 1) + "XXXX" + dr["idNumber"].ToString().Substring(Math.Max(0, dr["idNumber"].ToString().Length - 4));
                        }


                        allPayment.Merge(dtPayment);

                    }

                }
            }

            //GridViewHelper helper = new GridViewHelper(this.gvDailyPayments);
            //helper.RegisterGroup("paymentMode", true, true);
            //helper.GroupHeader += new GroupEvent(helper_GroupHeader);

            gvDailyPayments.DataSource = allPayment;
            gvDailyPayments.DataBind();

        }

        private void helper_GroupHeader(string groupName, object[] values, GridViewRow row)
        {
            if (groupName == "paymentMode")
            {
                row.Attributes.CssStyle.Value = "background-color: #b1a066";
            }


        }

        private void helper_Bug(string groupName, object[] values, GridViewRow row)
        {
            if (groupName == null) return;

            row.Attributes.CssStyle.Value = "background-color: #fff7e7";
            row.Cells[0].HorizontalAlign = HorizontalAlign.Right;
            row.Cells[0].Text = "Total For [" + values[0] + "]: ";
        }


        protected void btnVerify_Click(object sender, EventArgs e)
        {

            List<DailySettlementRecords> lstSettlementRecords = new List<DailySettlementRecords>();

            foreach (GridViewRow row in gvDailyPayments.Rows)
            {

                if (row.RowType == DataControlRowType.DataRow)
                {
                    string paymentMode = ((Label)row.FindControl("lblPaymentMode")).Text.ToString();
                    if (paymentMode != "")
                    {
                        string name = ((Label)row.FindControl("lblName")).Text.ToString();
                        string nric = ((Label)row.FindControl("lblNRIC")).Text.ToString();
                        DateTime startDate = DateTime.Parse(((Label)row.FindControl("lblProgStartDate")).Text.ToString());
                        DateTime endDate = DateTime.Parse(((Label)row.FindControl("lblProgEndDate")).Text.ToString());
                        string progname = ((Label)row.FindControl("lblProgrammeName")).Text.ToString();
                        string projcode = ((Label)row.FindControl("lblProjectCode")).Text.ToString();
                        string coursecode = ((Label)row.FindControl("lblCourseCode")).Text.ToString();
                        decimal adminFeesWOGst = Decimal.Parse(((Label)row.FindControl("lblAdminFeesWOGST")).Text.ToString());
                        decimal adminFeesGst = Decimal.Parse(((Label)row.FindControl("lblAdminFeesGST")).Text.ToString());
                        decimal adminFeesWGst = Decimal.Parse(((Label)row.FindControl("lblAdminFeesWGST")).Text.ToString());
                        decimal courseFeesWOGst = Decimal.Parse(((TextBox)row.FindControl("tbCourseFeesWOGST")).Text.ToString());
                        decimal courseFeesGST = Decimal.Parse(((Label)row.FindControl("lblCourseFeesGST")).Text.ToString());
                        decimal courseFeesWGst = Decimal.Parse(((TextBox)row.FindControl("tbCourseFeesWGST")).Text.ToString());
                        decimal lessScheme = Decimal.Parse(((TextBox)row.FindControl("tbScheme")).Text.ToString());
                        decimal totalCourseFees = Decimal.Parse(((Label)row.FindControl("lblTotalCourseFees")).Text.ToString());
                        decimal totalFeeCollected = Decimal.Parse(((Label)row.FindControl("lblTotalFeesCollected")).Text.ToString());
                        string remarks = ((TextBox)row.FindControl("tbRemarks")).Text.ToString();

                        DailySettlementRecords dsr = new DailySettlementRecords();

                        dsr.paymentMode = paymentMode;
                        dsr.applicantname = name;
                        dsr.applicantnric = nric;
                        dsr.progStartDate = startDate;
                        dsr.progEndDate = endDate;
                        dsr.programmeName = progname;
                        dsr.projectCode = projcode;
                        dsr.courseCode = coursecode;
                        dsr.adminFeesGst = adminFeesGst;
                        dsr.adminFeesWGst = adminFeesWGst;
                        dsr.adminFeesWOGst = adminFeesWOGst;
                        dsr.courseFeesWGst = courseFeesWGst;
                        dsr.courseFeesWOGst = courseFeesWOGst;
                        dsr.courseFessGst = courseFeesGST;
                        dsr.lessScheme = lessScheme;
                        dsr.totalCourseFees = totalCourseFees;
                        dsr.totalFeesCollected = totalFeeCollected;
                        dsr.remarks = remarks;

                        lstSettlementRecords.Add(dsr);
                    }

                }

            }

            List<DailySettlementDetails> lstDailySettlementDetails = new List<DailySettlementDetails>();

            var grpPaymentList = lstSettlementRecords
                .GroupBy(u => u.paymentMode)
                .Select(grp => grp.ToList())
                .ToList();

            foreach (var eachGrp in grpPaymentList)
            {

                decimal totalFeesCollected = eachGrp.Sum(x => x.totalFeesCollected);
                decimal lessSubsidyCollected = eachGrp.Sum(y => y.lessScheme);
                string paymentMode = eachGrp.FirstOrDefault().paymentMode.ToString();

                DailySettlementDetails dsd = new DailySettlementDetails();
                dsd.feesCollected = totalFeesCollected;
                dsd.lessSubsidy = lessSubsidyCollected;
                dsd.paymentMode = paymentMode;

                lstDailySettlementDetails.Add(dsd);
            }



            GridViewHelper helper = new GridViewHelper(this.gvConfirm);
            helper.RegisterGroup("paymentMode", true, true);
            helper.RegisterSummary("adminFeesGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("courseFeesWOGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("lessScheme", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("totalCourseFees", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("adminFeesWOGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("adminFeesWGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("courseFeesWGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("courseFessGst", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("totalCourseFees", SummaryOperation.Sum, "paymentMode");
            helper.RegisterSummary("totalFeesCollected", SummaryOperation.Sum, "paymentMode");

            //helper.RegisterSummary("adminFeesGst", SummaryOperation.Sum);
            //helper.RegisterSummary("courseFeesWOGst", SummaryOperation.Sum);
            //helper.RegisterSummary("lessScheme", SummaryOperation.Sum);
            //helper.RegisterSummary("totalCourseFees", SummaryOperation.Sum);
            //helper.RegisterSummary("adminFeesWOGst", SummaryOperation.Sum);
            //helper.RegisterSummary("adminFeesWGst", SummaryOperation.Sum);
            //helper.RegisterSummary("courseFeesWGst", SummaryOperation.Sum);
            //helper.RegisterSummary("courseFessGst", SummaryOperation.Sum);
            //helper.RegisterSummary("totalCourseFees", SummaryOperation.Sum);
            //helper.RegisterSummary("totalFeesCollected", SummaryOperation.Sum);

            helper.GroupHeader += new GroupEvent(helper_GroupHeader);
            helper.GroupSummary += new GroupEvent(helper_Bug);


            gvConfirm.DataSource = lstSettlementRecords;
            gvConfirm.DataBind();

            ViewState["lstSettlementRecords"] = lstSettlementRecords;
            ViewState["lstDailySettlementDetails"] = lstDailySettlementDetails;

        }



        protected void btnGet_Click(object sender, EventArgs e)
        {
            populatePayment();
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            
            List<DailySettlementRecords> lstSettlementRecords = ViewState["lstSettlementRecords"] as List<DailySettlementRecords>;
            List<DailySettlementDetails> lstDailySettlementDetails = ViewState["lstDailySettlementDetails"] as List<DailySettlementDetails>;

            DateTime dtSettlement = DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

            Daily_Settlement_Management dsm = new Daily_Settlement_Management();
            dsm.insertSettlementRecords(LoginID, dtSettlement, lstSettlementRecords, lstDailySettlementDetails);

    


        }
    }
}