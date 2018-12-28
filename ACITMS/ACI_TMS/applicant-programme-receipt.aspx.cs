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
    public partial class applicant_programme_receipt : BasePage
    {
        public const string PAGE_NAME = "applicant-programme-receipt.aspx";

        public const string APPLICANT_QUERY = "a";
        public const string TRAINEE_QUERY = "tr";
        public const string TYPE_QUERY = "t";
        public const string PAYMENT_QUERY = "p";

        private const string AMOUNT_FORMAT_STR = "#,##0.00";

        private Finance_Management fm = new Finance_Management();

        private decimal nett = 0;

        public applicant_programme_receipt()
            : base(PAGE_NAME, AccessRight_Constance.PAYMT_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[PAYMENT_QUERY] == null || Request.QueryString[PAYMENT_QUERY] == "")
                {
                    redirectToErrorPg("Missing receipt information.");
                    return;
                }

                if ((Request.QueryString[APPLICANT_QUERY] == null || Request.QueryString[APPLICANT_QUERY] == "") && (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == ""))
                {
                    redirectToErrorPg("Missing applicant/trainee information.");
                    return;
                }

                if (Request.QueryString[TYPE_QUERY] == null || Request.QueryString[TYPE_QUERY] == "" || HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == PaymentType.BOTH.ToString())
                    hfType.Value = PaymentType.BOTH.ToString();
                else if (HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == PaymentType.REG.ToString())
                    hfType.Value = PaymentType.REG.ToString();
                else
                    hfType.Value = PaymentType.PROG.ToString();

                if (Request.QueryString[APPLICANT_QUERY] != null && Request.QueryString[APPLICANT_QUERY] != "")
                    lbApplicantId.Text = HttpUtility.UrlDecode(Request.QueryString[APPLICANT_QUERY]);
                else lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);

                PrevPage = applicant_programme_payment.PAGE_NAME + "?" + (lbApplicantId.Text == "" ? applicant_programme_payment.TRAINEE_QUERY + "=" + Request.QueryString[TRAINEE_QUERY] 
                    : applicant_programme_payment.APPLICANT_QUERY + "=" + Request.QueryString[APPLICANT_QUERY]) + "&" + applicant_programme_payment.TYPE_QUERY + "=" + HttpUtility.UrlEncode(hfType.Value);

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
                        redirectToErrorPg("Payment has already been split, unable to show combined receipt.");
                        return;
                    }
                    else if (hfType.Value != PaymentType.BOTH.ToString() && dtTypes.Rows[0][0].ToString() == PaymentType.BOTH.ToString())
                    {
                        redirectToErrorPg("Payment has already been combined, unable to show split receipt.");
                        return;
                    }
                }

                lbGSTReg.Text = ConfigurationManager.AppSettings["GSTRegNum"].ToString();
                loadApplicantOrTraineeDetails();

                if (hfType.Value == PaymentType.REG.ToString())
                {
                    trProg.Visible = false;
                    //rpModules.Visible = false;
                    trSubsidy.Visible = false;
                }
                else
                {
                    if (hfType.Value == PaymentType.PROG.ToString()) trReg.Visible = false;
                    //loadBatchModules();
                }

                loadPaymentDetail(int.Parse(Request.QueryString[PAYMENT_QUERY]));
            }
            else
            {
                PrevPage = applicant_programme_payment.PAGE_NAME + "?" + (lbApplicantId.Text == "" ? applicant_programme_payment.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text)
                    : applicant_programme_payment.APPLICANT_QUERY + "=" + HttpUtility.UrlEncode(lbApplicantId.Text))
                    + "&" + applicant_programme_payment.TYPE_QUERY + "=" + HttpUtility.UrlEncode(hfType.Value);
            }
        }

        private void loadPaymentDetail(int paymentId)
        {
            DataTable dt = fm.getClassPaymentDetail(paymentId);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving payment detail.");
                return;
            }

            DataRow dr = dt.Rows[0];
            lbPaymentDate.Text = dr["paymentDate"].ToString();
            lbReceiptNum.Text = dr["receiptNumber"].ToString();
            lbPaymentMode.Text = dr["paymentModeDisp"].ToString();
            lbAmtPaid.Text = "S$" + ((decimal)dr["paymentAmount"]).ToString(AMOUNT_FORMAT_STR);
            lbTotal.Text = "S$" + nett.ToString(AMOUNT_FORMAT_STR);

            if (nett == (decimal)dr["paymentAmount"])
                lbPartialPayment.Visible = false;
            else lbPartialPayment.Visible = true;

            if (dr["paymentRemarks"] != DBNull.Value) lbPaymentRemarks.Text = "(" + dr["paymentRemarks"].ToString() + ")";
            // display paymentAmt in words
            lbDisplayWords.Text = ConvertToWords(dr["paymentAmount"].ToString());


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
            lbProgTitle.Text = dr["programmeTitle"].ToString();
            lbProgCseCode.Text = dr["courseCode"].ToString();
            lbBatchProjCode.Text = dr["projectCode"].ToString();
            lbBatchCode.Text = dr["batchCode"].ToString();
            lbBatchStart.Text = dr["programmeStartDate"].ToString();
            lbBatchEnd.Text = dr["programmeCompletionDate"].ToString();
            hfBundleId.Value = dr["bundleId"].ToString();
            lbApplicantName.Text = dr["fullName"].ToString();

            if (dr["selfSponsored"].ToString() == Sponsorship.COMP.ToString())
            {
                pCompany.Visible = true;
                lbCompany.Text = dr["companyName"].ToString();
            }

            if (hfType.Value != PaymentType.REG.ToString())
            {
                lbProgFee.Text = ((decimal)dr["programmePayableAmount"]).ToString(AMOUNT_FORMAT_STR);
                nett = (decimal)dr["programmePayableAmount"];

                if (dr["subsidyId"] != DBNull.Value)
                {
                    lbSubsidy.Text = dr["subsidyScheme"].ToString();
                    lbSubsidyAmt.Text = "(" + ((decimal)dr["subsidyAmt"]).ToString(AMOUNT_FORMAT_STR) + ")";
                    nett -= (decimal)dr["subsidyAmt"];
                }
                else trSubsidy.Visible = false;

                lbGST.Text = "S$" + ((decimal)dr["GSTPayableAmount"]).ToString(AMOUNT_FORMAT_STR);
                nett += (decimal)dr["GSTPayableAmount"];
            }
            if (hfType.Value != PaymentType.PROG.ToString())
            {
                if (dr["registrationFee"] != DBNull.Value) 
                { 
                    lbRegFee.Text = ((decimal)dr["registrationFee"]).ToString(AMOUNT_FORMAT_STR);
                    nett += (decimal)dr["registrationFee"];
                }
                else lbRegFee.Text = "N/A";
            }

            if (hfType.Value == PaymentType.REG.ToString())
            {
                //if payment is registration, GST is calculation
                lbGST.Text = "S$" + (Math.Round((decimal)dr["registrationFee"] * General_Constance.GST_RATE, 2)).ToString(AMOUNT_FORMAT_STR);
                nett += Math.Round((decimal)dr["registrationFee"] * General_Constance.GST_RATE, 2);
            }
            
        }

        //private void loadBatchModules()
        //{
        //    DataTable dt = (new Bundle_Management()).getBundleModule(int.Parse(hfBundleId.Value));
        //    if (dt == null || dt.Rows.Count == 0)
        //    {
        //        redirectToErrorPg("Error retrieving programme's modules.");
        //        return;
        //    }
        //    rpModules.DataSource = dt;
        //    rpModules.DataBind();
        //}

        // methods to convert numbers into words
        private static String ConvertToWords(String numb)
        {
            String val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            String endStr = "Only";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (Convert.ToInt32(points) >= 0)
                    {
                        if (Convert.ToDouble(wholeNo) >= 1)
                        {
                            if (Convert.ToDouble(points) > 0)
                            {
                                andStr = "Dollars and ";// just to separate whole numbers from points/cents  
                            }

                            else
                            {
                                andStr = "Dollars";
                            }
                        }


                        if (Convert.ToDouble(points) > 0)
                        {
                            endStr = "Cents " + endStr;//Cents  
                        }

                        pointStr = ConvertDecimals(points);
                    }
                }
                val = String.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }
        private static String ConvertWholeNumber(String Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX
                bool isDone = false;//test if already translated
                double dblAmt = (Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping
                    String place = "";//digit grouping name:hundres,thousand,etc...
                    switch (numDigits)
                    {
                        case 1://ones' range

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range
                            pos = (numDigits % 3) + 1;
                            place = " Hundred ";
                            break;
                        case 4://thousands' range
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Thousand ";
                            break;
                        case 7://millions' range
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Million ";
                            break;
                        case 10://Billions's range
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billion ";
                            break;
                        //add extra case options for anything above Billion...
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros
                        //if (beginsZero) word = " and " + word.Trim();
                    }
                    //ignore digit grouping names
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }

        private static String tens(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = null;
            switch (_Number)
            {
                case 10:
                    name = "Ten";
                    break;
                case 11:
                    name = "Eleven";
                    break;
                case 12:
                    name = "Twelve";
                    break;
                case 13:
                    name = "Thirteen";
                    break;
                case 14:
                    name = "Fourteen";
                    break;
                case 15:
                    name = "Fifteen";
                    break;
                case 16:
                    name = "Sixteen";
                    break;
                case 17:
                    name = "Seventeen";
                    break;
                case 18:
                    name = "Eighteen";
                    break;
                case 19:
                    name = "Nineteen";
                    break;
                case 20:
                    name = "Twenty";
                    break;
                case 30:
                    name = "Thirty";
                    break;
                case 40:
                    name = "Forty";
                    break;
                case 50:
                    name = "Fifty";
                    break;
                case 60:
                    name = "Sixty";
                    break;
                case 70:
                    name = "Seventy";
                    break;
                case 80:
                    name = "Eighty";
                    break;
                case 90:
                    name = "Ninety";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }
        private static String ones(String Number)
        {
            int _Number = Convert.ToInt32(Number);
            String name = "";
            switch (_Number)
            {

                case 1:
                    name = "One";
                    break;
                case 2:
                    name = "Two";
                    break;
                case 3:
                    name = "Three";
                    break;
                case 4:
                    name = "Four";
                    break;
                case 5:
                    name = "Five";
                    break;
                case 6:
                    name = "Six";
                    break;
                case 7:
                    name = "Seven";
                    break;
                case 8:
                    name = "Eight";
                    break;
                case 9:
                    name = "Nine";
                    break;
            }
            return name;
        }

        private static String ConvertDecimals(String number)
        {
            String cd = "", digit = "", num = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();

                if (i == 0 && !digit.Equals("0"))
                {
                    //engOne = ones(digit);
                    cd = ones(digit);

                    //  engOne = "Zero";
                }
                else if (i > 0 && digit.Equals("0"))
                {
                    if (cd == "One")
                    {
                        cd = "Ten";
                    }
                    else if (cd == "Two")
                    {
                        cd = "Twenty";
                    }
                    else if (cd == "Three")
                    {
                        cd = "Thirty";
                    }
                    else if (cd == "Four")
                    {
                        cd = "Forty";
                    }
                    else if (cd == "Five")
                    {
                        cd = "Fifty";
                    }
                    else if (cd == "Six")
                    {
                        cd = "Sixty";
                    }
                    else if (cd == "Seven")
                    {
                        cd = "Seventy";
                    }
                    else if (cd == "Eight")
                    {
                        cd = "Eighty";
                    }
                    else if (cd == "Nine")
                    {
                        cd = "Ninety";
                    }
                }
                else if (i > 0 && !digit.Equals("0"))
                {

                    num = cd;

                    cd = cd + " " + ones(digit);

                    if (cd == "One One")
                    {
                        cd = "Eleven";
                    }
                    else if (cd == "One Two")
                    {
                        cd = "Twelve";
                    }
                    else if (cd == "One Three")
                    {
                        cd = "Thirteen";
                    }
                    else if (cd == "One Four")
                    {
                        cd = "Fourteen";
                    }
                    else if (cd == "One Five")
                    {
                        cd = "Fifteen";
                    }
                    else if (cd == "One Six")
                    {
                        cd = "Sixteen";
                    }
                    else if (cd == "One Seven")
                    {
                        cd = "Seventeen";
                    }
                    else if (cd == "One Eight")
                    {
                        cd = "Eighteen";
                    }
                    else if (cd == "One Nine")
                    {
                        cd = "Nineteen";
                    }
                    else
                    {
                        if (num == "Two")
                        {
                            cd = "Twenty " + ones(digit);
                        }

                        else if (num == "Three")
                        {
                            cd = "Thirty " + ones(digit);
                        }

                        else if (num == "Four")
                        {
                            cd = "Forty " + ones(digit);
                        }

                        else if (num == "Five")
                        {
                            cd = "Fifty " + ones(digit);
                        }

                        else if (num == "Six")
                        {
                            cd = "Sixty " + ones(digit);
                        }

                        else if (num == "Seven")
                        {
                            cd = "Seventy " + ones(digit);
                        }
                        else if (num == "Eight")
                        {
                            cd = "Eighty " + ones(digit);
                        }
                        else if (num == "Nine")
                        {
                            cd = "Ninety " + ones(digit);
                        }
                    }

                }

            }
            return cd;
        }


        //private static String ConvertDecimals(String number)
        //{
        //    String cd = "", digit = "", engOne = "";
        //    for (int i = 0; i < number.Length; i++)
        //    {
        //        digit = number[i].ToString();

        //        if (digit.Equals("0"))
        //        {
        //            //  engOne = "Zero";
        //        }
        //        else
        //        {
        //            engOne = ones(digit);
        //        }

        //        cd += " " + engOne;


        //    }
        //    return cd;
        //}
    }
}