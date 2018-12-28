using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Data;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Xml;

namespace ACI_TMS
{
    public class Global : System.Web.HttpApplication
    {
        public const string AUDIT_DATA = "audit";
        public const string GRP_NAME_COLUMN = "groupName";
        public const string TBL_NAME_COLUMN = "tableName";
        public const string TBL_QUERY_COLUMN = "tableSQL";

        private string auditFilePath;

        protected void Application_Start(object sender, EventArgs e)
        {
            auditFilePath = Server.MapPath("~/" + ConfigurationManager.AppSettings["auditFile"].ToString());
            loadAuditConfig(null, null);
            watchAuditConfig();
        }

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        private void watchAuditConfig()
        {
            string dir = Path.GetDirectoryName(auditFilePath);
            string filename = Path.GetFileName(auditFilePath);

            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @dir;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = filename;
            watcher.Changed += new FileSystemEventHandler(loadAuditConfig);
            watcher.EnableRaisingEvents = true;
        }

        private void loadAuditConfig(object source, FileSystemEventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(auditFilePath);

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn(GRP_NAME_COLUMN, typeof(string)));
            dt.Columns.Add(new DataColumn(TBL_NAME_COLUMN, typeof(string)));
            dt.Columns.Add(new DataColumn(TBL_QUERY_COLUMN, typeof(string)));

            foreach (XmlNode grp in doc.FirstChild.ChildNodes)
            {
                string grpName = grp.Attributes["name"].InnerText;
                foreach (XmlNode tbl in grp.ChildNodes)
                {
                    DataRow dr = dt.NewRow();
                    dr[GRP_NAME_COLUMN] = grpName;
                    dr[TBL_NAME_COLUMN] = tbl.Attributes["name"].InnerText;
                    dr[TBL_QUERY_COLUMN] = tbl.InnerText;
                    dt.Rows.Add(dr);
                }
            }

            Application[AUDIT_DATA] = dt;
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
         
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}