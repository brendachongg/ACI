using DataLayer;
using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Excel = Microsoft.Office.Interop.Excel;

namespace ACI_TMS
{
    public partial class temp : System.Web.UI.Page
    {
        protected Cryptography a = new Cryptography();

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            Cryptography c = new Cryptography();
            Label1.Text = c.encryptInfo(TextBox1.Text);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            Cryptography c = new Cryptography();
            Label1.Text = c.decryptInfo(TextBox1.Text);
        }

        protected void Enroll_Click(object sender, EventArgs e)
        {
            Tuple<int, string, string> status = (new Trainee_Management()).enrollApplicant(TextBox1.Text, 1, true);
            Label1.Text = status.Item3 + " (" + status.Item2 + ")";
            
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genACIMonthlySummary(2016, 2018);
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genACIMonthlyDetails(
            //    DateTime.ParseExact("2017-01-01", "yyyy-MM-dd", CultureInfo.InvariantCulture), DateTime.ParseExact("2017-12-31", "yyyy-MM-dd", CultureInfo.InvariantCulture));
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genCseFeeDrawnDown(7, 2017, "15% off");
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genFullQualQuarter(8, 2017);
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genWTSDisbursement(2017, "15% off");
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genSFCDisbursement(2017, "15% off");
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genALLSDetails(2017);
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genQPODetails(DateTime.ParseExact("2017-04-17", "yyyy-MM-dd", CultureInfo.InvariantCulture));
            //Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genCourseFeeCollection("ACT", DateTime.MinValue, DateTime.MaxValue);
            Tuple<bool, string> status = (new Report_Management(Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString()), 1)).genCourseFeeReceived(2017);
            Label1.Text = status.Item2;
        }

        protected void btnAddUser_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new ACI_Staff_User()).addStaff(tbName.Text, tbEmail.Text, "password", tbId.Text, Salutation.Mr, false, true, true, 
                StaffEmploymentType.FT, "12345678", null, "NA", "123456", 1);

            Label1.Text = status.Item2;
        }

    }
}