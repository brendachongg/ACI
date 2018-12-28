using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;
using System.IO;
using GeneralLayer;

namespace DataLayer
{
    public class DB_Caselog
    {
        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();

        //Try Catch Error and Create New Case Log
        //public bool createErrorCaseLog(CaseLogCategory caseLogCategory, DateTime incidentDate, string subject, string message, int userId)
        //{
        //    try
        //    {
        //        string sqlStatement = @"INSERT INTO case_log (caseLogId, caseLogCategory, incidentDate, subject, message, status, createdBy) "
        //        + "VALUES ((select format(getdate(), 'yyyyMM')+right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int, "
        //        + "case when isnumeric(right(caseLogId, 4))= 0 then '0' else right(caseLogId, 4) end)),0)+1)), 4) as id "
        //        + "from case_log where left(caseLogId, 6)=format(getdate(), 'yyyyMM')), @caseLogCategory, @incidentDate, @subject, @message, @status, @createdBy)";

        //        SqlCommand cmd = new SqlCommand(sqlStatement);
        //        cmd.Parameters.AddWithValue("@caseLogCategory", caseLogCategory.ToString());
        //        cmd.Parameters.AddWithValue("@incidentDate", incidentDate);
        //        cmd.Parameters.AddWithValue("@subject", subject);
        //        cmd.Parameters.AddWithValue("@message", message);
        //        cmd.Parameters.AddWithValue("@status", CaseLogStatus.NEW.ToString());
        //        cmd.Parameters.AddWithValue("@createdBy", userId);


        //        bool success = dbConnection.executeNonQuery(cmd);

        //        return success;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Handler lh = new Log_Handler();
        //        lh.WriteLog(ex, "DB_CaseLog.cs", "createErrorCaseLog()", ex.Message, -1, false);

        //        return false;
        //    }
        //}

        //Retrieve case logs and search functions
        public DataTable searchCaseLog(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT cl.caseLogId, cl.caseLogCategory, cl.subject, cl.createdBy, cl.createdOn, cl.createdOn as submittedOn, au.userName, cr.codeValueDisplay as LGCAT "
                 + "FROM case_log cl LEFT OUTER JOIN aci_user au ON cl.createdBy = au.userId "
                 + "INNER JOIN code_reference cr ON cl.caseLogCategory = cr.codeValue AND cr.codeType = 'LGCAT' "
                 + (condition == null ? "" : condition) 
                 + "ORDER BY cl.createdOn DESC, cl.caseLogId DESC";

                if (p != null) cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "searchCaseLog()", ex.Message, -1);

                return null;
            }
        }

        //case-log-creation.aspx
        //Retrieve CaseLog DDL 
        public DataTable getCaseLogCategory()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = @"SELECT codeValue, codeValueDisplay FROM code_reference WHERE codeType = 'LGCAT'";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "getCaseLogCategory()", ex.Message, -1);

                return null;
            }
        }

        //Create Case Log with Auto Generated ID
        public bool createCaseLog(CaseLogCategory caseLogCategory, DateTime incidentDate, string subject, string message, byte[] attachment, string attachmentName, string attachmentType, int userId)
        {
            try
            {
                string sqlStatement = @"INSERT INTO case_log (caseLogId, caseLogCategory, incidentDate, subject, message, attachment, attachmentName, attachmentType, status, createdBy) 
                    select format(getdate(), 'yyyyMM')+right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int, case when isnumeric(right(caseLogId, 4))= 0 then '0' else right(caseLogId, 4) end)),0)+1)), 4) as id, 
                    @caseLogCategory, @incidentDate, @subject, @message, " + (attachment == null ? "null" : "@attachment") + @", @attachmentName, @attachmentType, @status, @createdBy 
                    from case_log where left(caseLogId, 6)=format(getdate(), 'yyyyMM')";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@caseLogCategory", caseLogCategory.ToString());
                cmd.Parameters.AddWithValue("@incidentDate", incidentDate);
                cmd.Parameters.AddWithValue("@subject", subject);
                cmd.Parameters.AddWithValue("@message", message);
                if (attachment != null) cmd.Parameters.AddWithValue("@attachment", attachment);
                cmd.Parameters.AddWithValue("@attachmentName", attachmentName == null ? (object)DBNull.Value : attachmentName);
                cmd.Parameters.AddWithValue("@attachmentType", attachmentType == null ? (object)DBNull.Value : attachmentType);
                cmd.Parameters.AddWithValue("@status", CaseLogStatus.NEW.ToString());
                cmd.Parameters.AddWithValue("@createdBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "createCaseLog()", ex.Message, -1, false);

                return false;
            }
        }

        // case-log-view.aspx
        //View details page get Case Log Details by Case Log Id
        public DataTable getCaseLogDetails(string caseLogId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT cl.caseLogId, cl.incidentDate, convert(nvarchar, cl.incidentDate, 106) as dateOfIncident, convert(char(5), cl.incidentDate, 108) as incidentTime, cl.subject, cl.message, cl.attachment, cl.attachmentName, cl.attachmentType, cl.status, cl.handledBy, cl.createdBy, cl.createdOn, au.userName, cr1.codeValueDisplay as LGCAT, cr2.codeValueDisplay as LGSTAT "
                    + "FROM case_log cl left outer join aci_user au on cl.createdBy = au.userId "
                    + "inner join code_reference cr1 on cl.caseLogCategory = cr1.codeValue and cr1.codeType = 'LGCAT' "
                    + "inner join code_reference cr2 on cl.status = cr2.codeValue and cr2.codeType = 'LGSTAT' where cl.caseLogId = @clid";

                cmd.Parameters.AddWithValue("@clId", caseLogId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "getCaseLogDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getCaseLogUsersEmail(string caseLogId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select distinct userId, userEmail from aci_user where userId in 
                        (select createdBy from case_log where caseLogId=@clid union select createdBy from case_log_followup where caseLogId=@clid)";

                cmd.Parameters.AddWithValue("@clid", caseLogId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "getCaseLogUsersEmail()", ex.Message, -1);

                return null;
            }
        }

        //Get Follow Up Records based on Case Log Id
        public DataTable getFollowUpByCaseLogId(string caseLogId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT clf.followUpId, clf.message, clf.attachment, clf.attachmentName, clf.attachmentType, convert(nvarchar, clf.createdOn, 106) as repliedOn, au.userName "
                    + "FROM case_log_followup clf inner join case_log cl on clf.caseLogId = cl.caseLogId "
                    + "left outer join aci_user au on clf.createdBy = au.userId "
                    + "WHERE clf.caseLogId = @clfId "
                    + "ORDER BY clf.createdOn DESC";

                cmd.Parameters.AddWithValue("@clfId", caseLogId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "getFollowUpByCaseLogId()", ex.Message, -1);

                return null;
            }
        }

        //Get Follow Up Attachment based on Follow Up Log Id
        public DataTable getFollowUpAttachment(int followUpId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT attachment, attachmentName, attachmentType "
                    + "FROM case_log_followup "
                    + "WHERE followUpId = @Id";

                cmd.Parameters.AddWithValue("@Id", followUpId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "getFollowUpAttachment()", ex.Message, -1);

                return null;
            }
        }

        //Update Case Log Status
        public bool updateCaseLog(string caseLogId, CaseLogStatus status, int userId)
        {
            try
            {
                string sqlStatement = "UPDATE case_log SET status = @status, handledBy = @handledBy, lastModifiedBy = @lastModifiedBy WHERE caseLogId = @caseLogId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@status", status.ToString());
                cmd.Parameters.AddWithValue("@handledBy", userId);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("@caseLogId", caseLogId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }

            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "updateCaseLog()", ex.Message, -1);

                return false;
            }
        }

        //Create new followup records for Case Logs.
        public bool createFollowUp(string caseLogId, string message, byte[] attachment, string attachmentName, string attachmentType, int userId)
        {
            try
            {
                string sqlStatement = @"INSERT INTO case_log_followup (caseLogId, message, attachment, attachmentName, attachmentType, createdBy)
                                      VALUES (@caseLogId, @message, " + (attachment == null ? "null" : "@attachment") + @", @attachmentName, @attachmentType, @createdBy)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@caseLogId", caseLogId);
                cmd.Parameters.AddWithValue("@message", message);
                if (attachment != null) cmd.Parameters.AddWithValue("@attachment", attachment);
                cmd.Parameters.AddWithValue("@attachmentName", attachmentName == null ? (object)DBNull.Value : attachmentName);
                cmd.Parameters.AddWithValue("@attachmentType", attachmentType == null ? (object)DBNull.Value : attachmentType);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_CaseLog.cs", "createFollowUp()", ex.Message, -1);

                return false;
            }
        }

        public bool deleteCaseLogs(string[] ids, int userId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;

                try
                {
                    string tmp = "";
                    for (int i = 0; i < ids.Length; i++)
                    {
                        tmp += "@id" + i + ",";
                        cmd.Parameters.AddWithValue("@id" + i, ids[i]);
                    }
                    tmp = tmp.Substring(0, tmp.Length - 1);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                    set CONTEXT_INFO @usrid;
                                delete from case_log_followup where caseLogId in (" + tmp + @");
                                delete from case_log where caseLogId in (" + tmp + @");";

                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_CaseLog.cs", "deleteCaseLogs()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
    }
}