using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;
using System.Configuration;
using System.IO;

namespace ACI_TMS
{
    public partial class report_export : BasePage
    {
        public const string PAGE_NAME = "report-export.aspx";
        public const string FILE_QUERY = "fn";

        public report_export()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.REPORT_ACIMTHLY, AccessRight_Constance.REPORT_SSGKPI, AccessRight_Constance.REPORT_FEEGRANT, AccessRight_Constance.REPORT_FULLQUALQT, AccessRight_Constance.REPORT_WTSDISB
            , AccessRight_Constance.REPORT_SFCDISB, AccessRight_Constance.REPORT_ALLS, AccessRight_Constance.REPORT_QPO, AccessRight_Constance.REPORT_FEECOLLECT, AccessRight_Constance.REPORT_FEETALLY, AccessRight_Constance.REPORT_NETS
            , AccessRight_Constance.REPORT_TRAINEE})
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[FILE_QUERY] == null || Request.QueryString[FILE_QUERY] == "")
                {
                    redirectToErrorPg("Missing parameters.");
                    return;
                }

                string path = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString());
                string filename = HttpUtility.UrlDecode(Request.QueryString[FILE_QUERY]);

                if (!File.Exists(path + filename))
                {
                    redirectToErrorPg("File does not exist.");
                    return;
                }

                //Transmit the file to client machine and start download
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" + filename + ";");
                Response.TransmitFile(path + filename);

                //use the following instead of response.end to prevent thread abort exception error
                HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                File.Delete(path + filename); 
            }
        }
    }
}