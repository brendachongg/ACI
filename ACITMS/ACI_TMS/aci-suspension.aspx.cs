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
using System.Text.RegularExpressions;

namespace ACI_TMS
{
    public partial class aci_suspension : BasePage
    {
        public const string PAGE_NAME = "aci-suspension.aspx";

         public aci_suspension()
            : base(PAGE_NAME, AccessRight_Constance.SUSPEND_NEW, aci_suspended_list.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    panelError.Visible = false;
                    panelSuccess.Visible = false;
                    
                }

            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error accessing aci suspension creation.");
            }
        }


        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnAddSuspend_Click(object sender, EventArgs e)
        {
            string idNumber = tbNRICValue.Text;
            string fullName = tbFullNameValue.Text;
            DateTime startDate = Convert.ToDateTime(tbStartDateValue.Text);
            DateTime endDate = Convert.ToDateTime(tbEndDateValue.Text);
            string remarks = tbRemarksValue.Text;
            string type = ddlSuspensionType.SelectedValue;
            string errorMsg = "";
            Blacklist_Management bm = new Blacklist_Management();

            int failCounter = 0;

          

                if (bm.searchBlackListRecordByNRICSuspensionDate(idNumber, startDate, endDate))
                {
                    failCounter++;
                    errorMsg += "Duplicate NRIC with overlapping period.";
                }
                //check nric
            //    if (idNumber.Length > 0 && idNumber.Length == 9)
            //    {
            //        char fLetter = ' ';
            //        char lLetter = ' ';
            //        string seven_digits_nric = "";

            //        if (idNumber[0].ToString() == "S" || idNumber[0].ToString() == "T" || idNumber[0].ToString() == "G" || idNumber[0].ToString() == "F")
            //        {
            //            fLetter = idNumber[0];
            //        }

            //        if (idNumber[0].ToString().All(Char.IsLetter))
            //        {
            //            lLetter = idNumber[8];
            //        }

            //        if (idNumber.Substring(1, 7).All(Char.IsDigit))
            //        {
            //            seven_digits_nric = idNumber.Substring(1, 7);
            //        }

            //        if (fLetter != ' ' && lLetter != ' ' && seven_digits_nric != "")
            //        {
            //            char last_char_check = generateCheckCode(fLetter, seven_digits_nric);

            //            if (lLetter != last_char_check)
            //            {
            //                failCounter++;
            //                errorMsg += "Invalid NRIC" + "<br>";
            //            }
            //        }
            //        else
            //        {
            //            failCounter++;
            //            errorMsg += "Invalid NRIC" + "<br>";
            //        }

            //    }
            //    else
            //    {
            //        failCounter++;
            //        errorMsg += "Invalid NRIC" + "<br>";
            //    }
            

            //if (fullName.Trim().Equals(""))
            //{
            //    failCounter++;
            //    errorMsg += "Please enter Full Name. <br>";
            //}



            //if (remarks.Trim().Equals(""))
            //{
            //    failCounter++;
            //    errorMsg += "Please enter Remarks. <br>";
            //}

            //if(startDate > endDate){
            //    failCounter++;
            //    errorMsg += "Suspension Start Date cannot be later than Suspension End Date. <br>";

            //}


            //if (bm.searchBlacklistedRecordByNRIC(idNumber).Equals(General_Constance.STATUS_YES))
            //{
            //    failCounter++;
            //    errorMsg += "Duplicate NRIC. <br>";
            //}
          
            if (failCounter == 0)
            {
                int userId = LoginID;
                
                bool success = bm.addSuspension(idNumber, fullName, startDate, endDate, type, remarks, userId);

                if (success)
                {
                    panelSuccess.Visible = true;
                    panelError.Visible = false;
                    lblSuccess.Text = "Record successfully entered.";
                    lblErrorMsg.Text = "";
                }                
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = true;
                lblErrorMsg.Text = errorMsg;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbNRICValue.Text = "";
            tbFullNameValue.Text = "";
            tbStartDateValue.Text = "";
            tbEndDateValue.Text = "";
            tbRemarksValue.Text = "";

            lbSuccessMsg.Visible = false;
            lblErrorMsg.Text = "";
            panelError.Visible = false;
            panelSuccess.Visible = false;
        }

        private char generateCheckCode(char letter, string num)
        {

            // Extract the digits
            int digit7 = Convert.ToInt32(num.Substring(6, 1));
            int digit6 = Convert.ToInt32(num.Substring(5, 1));
            int digit5 = Convert.ToInt32(num.Substring(4, 1));
            int digit4 = Convert.ToInt32(num.Substring(3, 1));
            int digit3 = Convert.ToInt32(num.Substring(2, 1));
            int digit2 = Convert.ToInt32(num.Substring(1, 1));
            int digit1 = Convert.ToInt32(num.Substring(0, 1));

            int step1 = 0;

            if (letter == 'T' || letter == 'G')
            {
                step1 = digit1 * 2 + digit2 * 7 + digit3 * 6 + digit4 * 5 +
                       digit5 * 4 + digit6 * 3 + digit7 * 2 + 4;
            }
            else
            {
                step1 = digit1 * 2 + digit2 * 7 + digit3 * 6 + digit4 * 5 +
                       digit5 * 4 + digit6 * 3 + digit7 * 2;
            }


            int step2 = step1 % 11;
            int step3 = 11 - step2;
            char code = ' ';

            if (letter == 'S' || letter == 'T')
            {


                switch (step3)
                {
                    case 1: code = 'A'; break;
                    case 2: code = 'B'; break;
                    case 3: code = 'C'; break;
                    case 4: code = 'D'; break;
                    case 5: code = 'E'; break;
                    case 6: code = 'F'; break;
                    case 7: code = 'G'; break;
                    case 8: code = 'H'; break;
                    case 9: code = 'I'; break;
                    case 10: code = 'Z'; break;
                    case 11: code = 'J'; break;
                }


                return code;

            }
            else
            {

                switch (step3)
                {
                    case 1: code = 'K'; break;
                    case 2: code = 'L'; break;
                    case 3: code = 'M'; break;
                    case 4: code = 'N'; break;
                    case 5: code = 'P'; break;
                    case 6: code = 'Q'; break;
                    case 7: code = 'R'; break;
                    case 8: code = 'T'; break;
                    case 9: code = 'U'; break;
                    case 10: code = 'W'; break;
                    case 11: code = 'X'; break;
                }

                return code;

            }// end generateCheckCode 

        }

    }
}