using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;

namespace DataLayer
{
    public class DB_AuditTrail
    {
        private Database_Connection dbConnection = new Database_Connection();

        public int getNoOfRecords(string table, string condition, SqlParameter[] p){
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT count(*) from " + table + " t where " + condition;
                if (p != null && p.Length > 0) cmd.Parameters.AddRange(p);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_AuditTrail.cs", "getNoOfRecords()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getLogRecords(string tableSQL, string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = tableSQL + "  where " + condition;
                if (p != null && p.Length > 0) cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_AuditTrail.cs", "getLogRecords()", ex.Message, -1);

                return null;
            }
        }
    }
}