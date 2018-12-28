using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using System.IO;
using System.IO.Compression;
using System.Web.UI.WebControls;
using System.Configuration;

namespace ACI_TMS
{
    public partial class audit_trail_export : BasePage
    {
        public const string PAGE_NAME = "audit-trail-export-new.aspx";

        public const string DATA_QUERY = "d";
        public const string STARTDATE_QUERY = "s";
        public const string ENDDATE_QUERY = "e";
        public const string ACTION_QUERY = "a";

        private string dataType = "", action = "";
        private DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MaxValue;

        public audit_trail_export()
            : base(PAGE_NAME, AccessRight_Constance.AUDIT_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[DATA_QUERY] == null || Request.QueryString[DATA_QUERY] == "" || Request.QueryString[ACTION_QUERY] == null || Request.QueryString[ACTION_QUERY] == "")
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                dataType = Request.QueryString[DATA_QUERY];
                action = Request.QueryString[ACTION_QUERY].ToUpper();

                if (action != "ALL" && action != "CREATE" && action != "UPDATE" && action != "DELETE" && action != "VIEW")
                {
                    redirectToErrorPg("Invalid information.");
                    return;
                }

                if (Request.QueryString[STARTDATE_QUERY] != null && Request.QueryString[STARTDATE_QUERY] != ""
                    && !DateTime.TryParseExact(Request.QueryString[STARTDATE_QUERY], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStart))
                {
                    redirectToErrorPg("Invalid date.");
                    return;
                }

                if (Request.QueryString[ENDDATE_QUERY] != null && Request.QueryString[ENDDATE_QUERY] != ""
                    && !DateTime.TryParseExact(Request.QueryString[ENDDATE_QUERY], "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out dtStart))
                {
                    redirectToErrorPg("Invalid date.");
                    return;
                }

                Tuple<string[], string[]> tblInfo = getTableInfo();
                if (tblInfo == null) return;
                if (tblInfo.Item1.Length == 0)
                {
                    redirectToErrorPg("Invalid information.");
                    return;
                }

                generateLogFiles(tblInfo);
            }
        }

        private Tuple<string[], string[]> getTableInfo()
        {
            DataTable dt = Application[Global.AUDIT_DATA] as DataTable;
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving audit configuration.");
                return null;
            }

            List<string> tables = new List<string>();
            List<string> sql = new List<string>();
            foreach (DataRow dr in dt.Select(Global.GRP_NAME_COLUMN + "='" + dataType + "'"))
            {
                tables.Add(dr[Global.TBL_NAME_COLUMN].ToString());
                sql.Add(dr[Global.TBL_QUERY_COLUMN].ToString());
            }

            return new Tuple<string[], string[]>(tables.ToArray(), sql.ToArray());
        }

        private void generateLogFiles(Tuple<string[], string[]> tblInfo)
        {
            string pathName = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString());
            string folderName = "AuditLog_" + LoginID + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            try
            {
                if (Directory.Exists(pathName + folderName))
                {
                    redirectToErrorPg("Duplicate folder. Unable to generate log files.");
                    return;
                }

                DirectoryInfo di = Directory.CreateDirectory(pathName + folderName);

                AuditTrail_Management atm = new AuditTrail_Management();

                for (int i = 0; i < tblInfo.Item1.Length; i++)
                {
                    DataTable dt = atm.getAuditTrail(tblInfo.Item2[i], dtStart, dtEnd, action);
                    if (dt == null)
                    {
                        redirectToErrorPg("Error retrieving log records.");
                        return;
                    }

                    using (StreamWriter sw = File.CreateText(di.FullName + @"\" + tblInfo.Item1[i] + ".csv"))
                    {
                        //display columns
                        int col = 1;
                        foreach (DataColumn dc in dt.Columns)
                        {
                            sw.Write((col == 1 ? "" : ",") + "\"" + dc.ColumnName + "\"");
                            col++;
                        }
                        sw.Write(Environment.NewLine);

                        //write log records
                        foreach (DataRow dr in dt.Rows)
                        {
                            for (col = 0; col < dt.Columns.Count; col++)
                                sw.Write((col == 0 ? "" : ",") + "\"" + dr[col].ToString().Replace("\"", "\"\"") + "\"");

                            sw.Write(Environment.NewLine);
                        }
                    }	
                }

                //create the zip file
                ZipFile.CreateFromDirectory(di.FullName, pathName + @"\" + folderName + ".zip");
                //delete the directory
                di.Delete(true);

                //Transmit the file to client machine and start download
                Response.ClearContent();
                Response.Clear();
                Response.ContentType = "text/plain";
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=AuditLog.zip;");
                Response.TransmitFile(pathName + @"\" + folderName + ".zip");

                //use the following instead of response.end to prevent thread abort exception error
                HttpContext.Current.Response.Flush(); // Sends all currently buffered output to the client.
                HttpContext.Current.Response.SuppressContent = true;  // Gets or sets a value indicating whether to send HTTP content to the client.
                HttpContext.Current.ApplicationInstance.CompleteRequest(); // Causes ASP.NET to bypass all events and filtering in the HTTP pipeline chain of execution and directly execute the EndRequest event.

                File.Delete(pathName + @"\" + folderName + ".zip");
            }
            catch (Exception ex)
            {
                log("generateLogFiles()", "Error generating log files", ex);
                redirectToErrorPg("Error exporting log records.");
            }
        }
    }
}