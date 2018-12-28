using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;
using System.Configuration;

namespace ACI_TMS
{
    public partial class aci_staff_login : BasePage
    {
        public const string PAGE_NAME = "aci-staff-login.aspx";
        ACI_Staff_User staffAccount = new ACI_Staff_User();

        public aci_staff_login()
            : base(PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            lbUnauthorizedMessage.Visible = false;
        }

        protected void btnSignIn_Click(object sender, EventArgs e)
        {
            signIn(tbLoginId.Text, tbPassword.Text);
        }

        //Signing in with staff account.
        private void signIn(string loginId, string staffPassword)
        {

            string webServiceLogin = ConfigurationManager.AppSettings["WebServiceLogin"].ToString();
            string adjunctLogin = ConfigurationManager.AppSettings["AdjunctWSLogin"].ToString();

            if (webServiceLogin == General_Constance.STATUS_YES)
            {
                string userEmpType = staffAccount.getACIUserEmpType(loginId);
                if (userEmpType == GeneralLayer.StaffEmploymentType.FT.ToString() || userEmpType == GeneralLayer.StaffEmploymentType.ADJ.ToString())
                {

                    if (userEmpType == GeneralLayer.StaffEmploymentType.ADJ.ToString() && adjunctLogin == General_Constance.STATUS_NO)
                    {
                        Tuple<Boolean, DataTable> accountTuple = staffAccount.validatesStaffAccountAuthorization(loginId, staffPassword);

                        //if account authorization is true, search for user roles.
                        if (accountTuple.Item1 == true)
                        {
                            lbUnauthorizedMessage.Visible = false;

                            Response.Redirect(dashboard.PAGE_NAME);
                        }
                        else
                        {
                            lbUnauthorizedMessage.Visible = true;

                        }
                    }
                    NYPStaffLogin.WSStaffLogin ws = new NYPStaffLogin.WSStaffLogin();
                    int result = ws.valLogin(loginId, staffPassword);
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog("", "aci-staff-login.aspx", "aci-staff-login", "LoginID :" + loginId + " Return Value: " + result, -1, false);

                    if (result == 0)
                    {
                        Tuple<Boolean, DataTable> accountTuple = staffAccount.validatesStaffAccountAuthorizationByLoginId(loginId);
                        if (accountTuple.Item1 == true)
                        {
                            lbUnauthorizedMessage.Visible = false;

                            Response.Redirect(dashboard.PAGE_NAME);
                        }
                        else
                        {
                            lbUnauthorizedMessage.Visible = true;

                        }
                    }

                    else
                        lbUnauthorizedMessage.Visible = true;
                }
                else
                {
                    Tuple<Boolean, DataTable> accountTuple = staffAccount.validatesStaffAccountAuthorization(loginId, staffPassword);

                    //if account authorization is true, search for user roles.
                    if (accountTuple.Item1 == true)
                    {
                        lbUnauthorizedMessage.Visible = false;

                        Response.Redirect(dashboard.PAGE_NAME);
                    }
                    else
                    {
                        lbUnauthorizedMessage.Visible = true;

                    }
                }
            }
            else
            {

                Tuple<Boolean, DataTable> accountTuple = staffAccount.validatesStaffAccountAuthorization(loginId, staffPassword);

                //if account authorization is true, search for user roles.
                if (accountTuple.Item1 == true)
                {
                    lbUnauthorizedMessage.Visible = false;

                    Response.Redirect(dashboard.PAGE_NAME);
                }
                else
                {
                    lbUnauthorizedMessage.Visible = true;

                }
            }


            //Sign in method using NYP staff account. Coming soon.


            //Authorization of account in ACI TMS.




            //Message box for testing purpose.
            //ClientScript.RegisterStartupScript(this.GetType(), "myalert", "alert('" + accountTuple.Item1 + "');", true);
        }
    }
}