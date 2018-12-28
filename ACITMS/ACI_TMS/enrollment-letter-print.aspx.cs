using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class enrollment_letter_print : BasePage
    {
        public const string PAGE_NAME = "enrollment-letter-print.aspx";

        public const string TRAINEE_QUERY = "t";

        public enrollment_letter_print()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_EDIT)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "")
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                Tuple<string, string> letter = (new Trainee_Management()).getEnrollmentLetter(HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]));
                if (letter == null)
                {
                    redirectToErrorPg("Error retrieving enrollment letter.");
                    return;
                }
                else if (letter.Item2 == "")
                {
                    redirectToErrorPg("Enrollment letter has not been setup for the class.");
                    return;
                }

                divContent.InnerHtml = letter.Item2;
            }
        }
        
    }
}