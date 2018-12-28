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
using System.Configuration;

namespace ACI_TMS
{
    public partial class absentee_payment_receipt : BasePage
    {
        public const string PAGE_NAME = "absentee-payment-receipt.aspx";

        public const string PAYMENT_QUERY = "p";

        private const string AMOUNT_FORMAT_STR = "#,##0.00";

        public absentee_payment_receipt()
            : base(PAGE_NAME, AccessRight_Constance.PAYMT_VIEW)
        {

        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[PAYMENT_QUERY] == null || Request.QueryString[PAYMENT_QUERY] == "")
                {
                    redirectToErrorPg("Missing payment informtion.");
                    return;
                }

                int paymentId = int.Parse(HttpUtility.UrlDecode(Request.QueryString[PAYMENT_QUERY]));

                lbGSTReg.Text = ConfigurationManager.AppSettings["GSTRegNum"].ToString();

                loadTraineeNProgrammeDetails(paymentId);
                loadPaymentDetails(paymentId);
            }
        }

        private void loadPaymentDetails(int paymentId)
        {
            Tuple<DataTable, DataTable> details = (new Finance_Management()).getMakeupPaymentDetails(paymentId);
            if (details.Item1 == null || details.Item2 == null || details.Item1.Rows.Count == 0 || details.Item2.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving payment details.");
                return;
            }

            DataRow drPayment = details.Item1.Rows[0];

            lbReceiptNum.Text = drPayment["receiptNumber"].ToString();
            lbPaymentMode.Text = drPayment["paymentModeDisp"].ToString();
            lbPaymentDate.Text = drPayment["paymentDateDisp"].ToString();
            lbPaymentRemarks.Text = drPayment["paymentRemarks"].ToString();
            lbAmtPaid.Text = ((decimal)drPayment["paymentAmount"]).ToString(AMOUNT_FORMAT_STR);

            decimal fee = Math.Round(((decimal)drPayment["paymentAmount"]) * 100 / 107, 2);
            decimal gst = ((decimal)drPayment["paymentAmount"]) - fee;
            lbAmt.Text = "S$" + fee.ToString(AMOUNT_FORMAT_STR);
            lbGST.Text = "S$" + gst.ToString(AMOUNT_FORMAT_STR);
            lbTotal.Text = ((decimal)drPayment["paymentAmount"]).ToString(AMOUNT_FORMAT_STR);

            rpModules.DataSource = details.Item2;
            rpModules.DataBind();
        }

        private void loadTraineeNProgrammeDetails(int paymentId)
        {
            DataTable dt = (new Trainee_Management()).getTraineeDetailsByPayment(paymentId);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving trainee details.");
                return;
            }

            lbProgTitle.Text = dt.Rows[0]["programmeTitle"].ToString();
            lbProgCseCode.Text = dt.Rows[0]["courseCode"].ToString();
            lbBatchProjCode.Text = dt.Rows[0]["projectCode"].ToString();
            lbBatchStart.Text = dt.Rows[0]["programmeStartDateDisp"].ToString();
            lbBatchEnd.Text = dt.Rows[0]["programmeCompletionDateDisp"].ToString();
            lbBatchCode.Text = dt.Rows[0]["batchCode"].ToString();

            lbTraineeName.Text = dt.Rows[0]["fullName"].ToString();
            lbTraineeId.Text = dt.Rows[0]["traineeId"].ToString();
        }
    }
}