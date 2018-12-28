using DataLayer;
using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ACI_TMS
{
    public class AuditTrail_Management
    {
        private DB_AuditTrail dbAudit = new DB_AuditTrail();

        public DataTable previewAuditTrail(string[] tables, DateTime dtStart, DateTime dtEnd, string action)
        {
            string condition = "";
            List<SqlParameter> pList=new List<SqlParameter>();
            if (dtStart != DateTime.MinValue && dtEnd != DateTime.MaxValue)
            {
                if (dtStart != DateTime.MinValue)
                {
                    condition += "t.createdOn >= @dtStart ";
                    pList.Add(new SqlParameter("@dtStart", dtStart));
                }

                if (dtEnd != DateTime.MaxValue)
                {
                    condition += (condition == "" ? "" : "and ") + "t.createdOn <= @dtEnd ";
                    pList.Add(new SqlParameter("@dtEnd", dtEnd));
                }
            }

            if (action.ToUpper() != "ALL")
            {
                condition += (condition == "" ? "" : "and ") + "t.action=@a ";
                pList.Add(new SqlParameter("@a", action.ToUpper()));
            }

            if (condition == "") condition = "1=1";

            SqlParameter[] p = pList.ToArray();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("table", typeof(string)));
            dt.Columns.Add(new DataColumn("records", typeof(int)));

            foreach (string t in tables)
            {
                DataRow dr = dt.NewRow();
                dr["table"] = t;
                dr["records"]=dbAudit.getNoOfRecords(t, condition, p);
                dt.Rows.Add(dr);
            }

            return dt;
        }

        //instead of getting the whole list of tables to get records from, get individually so to prevent large amount of data stored in memory
        public DataTable getAuditTrail(string tableSQL, DateTime dtStart, DateTime dtEnd, string action)
        {
            string condition = "";
            List<SqlParameter> pList = new List<SqlParameter>();
            if (dtStart != DateTime.MinValue && dtEnd != DateTime.MaxValue)
            {
                if (dtStart != DateTime.MinValue)
                {
                    condition += "t.createdOn >= @dtStart ";
                    pList.Add(new SqlParameter("@dtStart", dtStart));
                }

                if (dtEnd != DateTime.MaxValue)
                {
                    condition += (condition == "" ? "" : "and ") + "t.createdOn <= @dtEnd ";
                    pList.Add(new SqlParameter("@dtEnd", dtEnd));
                }
            }

            if (action.ToUpper() != "ALL")
            {
                condition += (condition == "" ? "" : "and ") + "t.action=@a ";
                pList.Add(new SqlParameter("@a", action.ToUpper()));
            }

            if (condition == "") condition = "1=1";

            return dbAudit.getLogRecords(tableSQL, condition, pList.ToArray());
        }
    }
}