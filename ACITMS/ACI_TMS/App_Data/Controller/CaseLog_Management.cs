using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Configuration;

namespace LogicLayer
{
    public class CaseLog_Management
    {
        private DB_Caselog dbCaseLog = new DB_Caselog();

        //Get Case Log and search function values
        public DataTable searchCaseLog(CaseLogStatus status, string caseLogCat, string searchKey, string searchValue)
        {
            string condition = null;
            List<SqlParameter> param = new List<SqlParameter>();
            if (status == CaseLogStatus.UA || status == CaseLogStatus.NEW)
            {
                condition += "where cl.status = @status ";
                param.Add(new SqlParameter("@status", CaseLogStatus.NEW.ToString()));

                if (status == CaseLogStatus.UA)
                {
                    condition += "and GetDate() - cl.createdOn >= 3 ";
                }
                else if (status == CaseLogStatus.NEW)
                {
                    condition += "and GetDate() - cl.createdOn < 3 ";
                }
            }
            else if (status == CaseLogStatus.RS || status == CaseLogStatus.C)
            {
                condition += "where cl.status = @status ";
                param.Add(new SqlParameter("@status", status.ToString()));
            }

            condition += "and cl.caseLogCategory = @cat ";
            param.Add(new SqlParameter("@cat", caseLogCat));

            if (searchKey == "D")
            {
                condition += "and cast (cl.createdOn as date) = @search ";
                param.Add(new SqlParameter("@search", searchValue));
            }
            else if (searchKey == "RB")
            {
                condition += "and UPPER(au.userName) like @search ";
                param.Add(new SqlParameter("@search", "%" + searchValue.ToUpper() + "%"));
            }

            DataTable dt = dbCaseLog.searchCaseLog(condition, param.ToArray());

            return dt;
        }

        //Get CaseLog Creation DDL 
        public DataTable getCaseLogCategory()
        {
            return dbCaseLog.getCaseLogCategory();
        }

        //Create Case Log with Auto Generated ID
        public Tuple<bool, string> createCaseLog(CaseLogCategory caseLogCategory, DateTime incidentDate, string subject, string message, byte[] attachment, string attachmentName, string attachmentType, int userId)
        {
            if (dbCaseLog.createCaseLog(caseLogCategory, incidentDate, subject, message, attachment, attachmentName, attachmentType, userId))
            {
                if (ConfigurationManager.AppSettings["CaseLogEmail"].ToString() == "1")
                {
                    string[] lstTo = ConfigurationManager.AppSettings["CaseLogWebMaster"].ToString().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                    (new Email_Handler()).SendEmail("tms@aci.edu.sg", lstTo, null, null, "New case log in ACI TMS: " + subject.Replace("\r", "").Replace("\n", "")
                        , "<b>" + (new DB_Users()).getUserName(userId) + "</b> created a new case log with the following message:<br/><br/><i>" + message + "</i>" +
                        (attachment == null ? "" : "<br/><br/>You can view the attachment in TMS.") + "<br/><br/>Computer generated message. Do not reply.");
                }

                return new Tuple<bool, string>(true, "Case Log created successfully.");
            }
            else
            {
                return new Tuple<bool, string>(false, "Error creating Case Log");
            }
        }

        //Case Log View Get Details and Follow Up Records
        public Tuple<DataTable, DataTable> getCaseLogAndFollowUp(string caseLogId)
        {
            DataTable dt = dbCaseLog.getCaseLogDetails(caseLogId);
            DataTable dtbl = dbCaseLog.getFollowUpByCaseLogId(caseLogId);
            return new Tuple<DataTable,DataTable>(dt, dtbl);
        }

        //Case Log View Get Details by Case Log Id
        public DataTable getCaseLogDetails(string caseLogId)
        {
            return dbCaseLog.getCaseLogDetails(caseLogId);
        }


        //Get Follow Up Records by Case Log Id
        public DataTable getFollowUpByCaseLogId(string caseLogId)
        {
            DataTable dtCaseLog = dbCaseLog.getFollowUpByCaseLogId(caseLogId);

            return dtCaseLog;
        }

        //Get Follow Up Attachment by Follow Up Id
        public DataTable getFollowUpAttachment(int followUpId)
        {
            DataTable dtCaseLog = dbCaseLog.getFollowUpAttachment(followUpId);

            return dtCaseLog;
        }

        //Update Case Log Status to Resolving and HandledBy
        public Tuple<bool, string> updateCaseLog(string caseLogId, CaseLogStatus status, int userId)
        {

            if (dbCaseLog.updateCaseLog(caseLogId, status, userId))
            {
                if (ConfigurationManager.AppSettings["CaseLogEmail"].ToString() == "1")
                {
                    DataTable dt = dbCaseLog.getCaseLogDetails(caseLogId);
                    if (dt == null || dt.Rows.Count == 0) return new Tuple<bool, string>(true, "Case Log updated successfully.");
                    DataTable dtUsr = (new ACI_Staff_User()).getUser((int)dt.Rows[0]["createdBy"]);
                    if (dtUsr == null || dtUsr.Rows.Count == 0) return new Tuple<bool, string>(true, "Case Log updated successfully.");

                    (new Email_Handler()).SendEmail("tms@aci.edu.sg", dtUsr.Rows[0]["userEmail"].ToString(), null, null, "Status update to case log " + caseLogId + " " + dt.Rows[0]["subject"].ToString().Replace("\r", "").Replace("\n", "")
                        , "<b>" + (new DB_Users()).getUserName(userId) + "</b> has changed the case log status to " + (status == CaseLogStatus.C ? "closed" : "reopened") + ".<br/><br/>Computer generated message. Do not reply.");
                }

                return new Tuple<bool, string>(true, "Case Log updated successfully.");
            }
            else
            {
                return new Tuple<bool, string>(false, "Error updating Case Log");
            }
        }

        //Create new followup based on caseLogId
        public Tuple<bool, string> createFollowUp(string caseLogId, string message, byte[] attachment, string attachmentName, string attachmentType, int userId)
        {

            if (dbCaseLog.createFollowUp(caseLogId, message, attachment, attachmentName, attachmentType, userId))
            {
                if (dbCaseLog.updateCaseLog(caseLogId, CaseLogStatus.RS, userId))
                {
                    if (ConfigurationManager.AppSettings["CaseLogEmail"].ToString() == "1")
                    {
                        DataTable dt = dbCaseLog.getCaseLogDetails(caseLogId);
                        DataTable dtEmail = dbCaseLog.getCaseLogUsersEmail(caseLogId);
                        if (dt == null || dt.Rows.Count == 0 || dtEmail == null || dtEmail.Rows.Count == 0) return new Tuple<bool, string>(true, "Case Log Follow Up created successfully.");

                        List<string> emails = new List<string>();
                        Cryptography decrypt = new Cryptography();
                        foreach (DataRow dr in dtEmail.Rows) { if ((int)dr["userId"] != userId) emails.Add(decrypt.decryptInfo(dr["userEmail"].ToString())); }

                        if (emails.Count > 0)
                        {
                        (new Email_Handler()).SendEmail("tms@aci.edu.sg", emails.ToArray(), null, null, "Reply to case log " + caseLogId + " " + dt.Rows[0]["subject"].ToString().Replace("\r", "").Replace("\n", "")
                            , "<b>" + (new DB_Users()).getUserName(userId) + "</b> has replied the following to the case log:<br/><br/><i>" + message + "</i>" +
                                (attachment == null ? "" : "<br/><br/>You can view the attachment in TMS.") + "<br/><br/>Computer generated message. Do not reply.");
                        }
                    }
                    
                return new Tuple<bool, string>(true, "Case Log Follow Up created successfully.");
                }
                else return new Tuple<bool, string>(true, "Error updating case log status.");
            }
            else
            {
                return new Tuple<bool, string>(false, "Error creating Case Log Follow Up");
            }
        }

        public Tuple<bool, string> deleteCaseLogs(string[] caseLogIds, int userId)
        {
            if (dbCaseLog.deleteCaseLogs(caseLogIds, userId))
            {
                return new Tuple<bool, string>(true, "Case log(s) deleted successfully.");
            }
            else
            {
                return new Tuple<bool,string>(false, "Error deleting selected case log(s).");
            }
        }
    }
}