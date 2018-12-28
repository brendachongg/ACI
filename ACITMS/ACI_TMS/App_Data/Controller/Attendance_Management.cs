using DataLayer;
using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace LogicLayer
{
    public class Attendance_Management
    {
        private DB_Attendance dbAttend = new DB_Attendance();
        private const float MAX_ABSENCE_RATE = 25;

        public int getNoOfAbsentee()
        {
            return dbAttend.getNoOfAbsentee();
        }

        public bool isOwnSession(int userId, int sessionId)
        {
            return dbAttend.isOwnSession(userId, sessionId, -1);
        }

        public bool isOwnBatchModule(int userId, int batchModuleId)
        {
            return dbAttend.isOwnSession(userId, -1, batchModuleId);
        }

        public Tuple<DataTable, DataTable> getSessionDetails(int batchModuleId)
        {
            DataTable dt = dbAttend.getSessionsTrainees(batchModuleId);
            if (dt != null)
            {
                Cryptography decryp = new Cryptography();
                dt.Columns.Add(new DataColumn("idNumberMasked", typeof(string)));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
                    //mask id number
                    dr["idNumberMasked"] = dr["idNumber"].ToString().Substring(0, 1) + "XXXX" + dr["idNumber"].ToString().Substring(Math.Max(0, dr["idNumber"].ToString().Length - 4));
                }
            }

            return new Tuple<DataTable,DataTable>(dbAttend.getSessionDetails(batchModuleId), dt);
        }

        public Tuple<DataTable, DataTable> getInsertedSessionDetails(int batchModuleId)
        {
            DataTable dt = dbAttend.getInsertedTrainees(batchModuleId);
            if (dt != null)
            {
                Cryptography decryp = new Cryptography();
                dt.Columns.Add(new DataColumn("idNumberMasked", typeof(string)));
                foreach (DataRow dr in dt.Rows)
                {
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
                    //mask id number
                    dr["idNumberMasked"] = dr["idNumber"].ToString().Substring(0, 1) + "XXXX" + dr["idNumber"].ToString().Substring(Math.Max(0, dr["idNumber"].ToString().Length - 4));
                }
            }

            return new Tuple<DataTable,DataTable>(dbAttend.getSessionDetails(batchModuleId), dt);
        }

        public DataTable getInsertedSessionTrainees(int batchModuleId)
        {
            DataTable dt = dbAttend.getInsertedSessionTrainees(batchModuleId);
            if (dt != null)
            {
                Cryptography decryp = new Cryptography();
                foreach (DataRow dr in dt.Rows)
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public Tuple<bool, string> updateMakeup(string traineeId, int sessionId, bool isAbsValid, string absReason, int makeupBatchModuleId, int makeupSessionId, int userId)
        {
            if (makeupBatchModuleId != -1)
            {
                //check if the selected makeup session clash with any of the trainee existing enroll programme or makeup or repeat
                DataTable dtClash = (new DB_Assessment()).getClashSession(traineeId, "select 1 from batchModule_session s where s.sessionId=@sid and s.sessionDate=t.sessionDate and s.sessionPeriod=t.sessionPeriod",
                    new SqlParameter[] { new SqlParameter("@sid", makeupSessionId) });

                if (dtClash == null) return new Tuple<bool, string>(false, "Error checking trainee's details.");
                if (dtClash.Rows.Count > 0)
                {
                    string errMsg = "Selected session clashes with the existing session(s) that trainee has been enrolled for:";
                    string type;
                    foreach (DataRow dr in dtClash.Rows)
                    {
                        if (dr["type"].ToString() == "MAIN") type = "Enrolled session for module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";
                        else if (dr["type"].ToString() == "MAKEUP") type = "Make-up session of module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";
                        else type = "Re-assessment session of module " + dr["moduleCode"].ToString() + ", class " + dr["batchCode"].ToString() + " on ";

                        errMsg += "<li>" + type + " " + dr["sessionDateDisp"].ToString() + " " + dr["sessionPeriodDisp"].ToString() + "</li>";
                    }

                    return new Tuple<bool, string>(false, errMsg);
                }

                //check if exceed the makeup batch capacity
                int bCapacity = (new DB_Batch_Session()).getBatchCapacityByBatchModule(makeupBatchModuleId);
                if (bCapacity == -1) return new Tuple<bool, string>(false, "Error updating make-up.");
                int currCapacity = dbAttend.getSessionEnrollment(makeupBatchModuleId, makeupSessionId);
                if (currCapacity == -1) return new Tuple<bool, string>(false, "Error updating make-up.");
                if (currCapacity + 1 > bCapacity) return new Tuple<bool, string>(false, "Exceeded class capacity.");
            }
            
            if (dbAttend.updateMakeup(traineeId, sessionId, isAbsValid, absReason, makeupBatchModuleId, makeupSessionId, userId))
                return new Tuple<bool, string>(true, "Make-up updated successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating make-up.");
        }

        public Tuple<DataTable, DataTable> getAbsenceDetails(string traineeId, int sessionId)
        {
            //get the absence details
            DataTable dtAbs = dbAttend.getTraineeAbsence(traineeId, sessionId, false);
            if (dtAbs == null || dtAbs.Rows.Count == 0) return null;

            DataTable dtMk = null;
            if (dtAbs.Rows[0]["insertedSessionId"] != DBNull.Value)
            {
                dtMk = dbAttend.getTraineeAbsence(traineeId, sessionId, true);
                if (dtMk == null || dtMk.Rows.Count == 0) return null;
            }

            dtAbs.Columns.Add(new DataColumn("hasPayment", typeof(bool)));
            dtAbs.Rows[0]["hasPayment"] = dbAttend.hasPaymentRef(traineeId, (int)dtAbs.Rows[0]["absentId"]);

            return new Tuple<DataTable, DataTable>(dtAbs, dtMk);
        }
        
        public DataTable searchAbsentees(string searchType, string searchValue)
        {
            string condition = null;
            SqlParameter p = null;

            if (searchType == "T")
            {
                condition = "where UPPER(t.fullName) like @search or UPPER(a.traineeId) like @search ";
                p = new SqlParameter("@search", "%" + searchValue.ToUpper() + "%");
            }
            else if (searchType == "SD")
            {
                condition = "where s.sessionDate = @search ";
                p = new SqlParameter("@search", DateTime.ParseExact(searchValue, "dd MMM yyyy", CultureInfo.InvariantCulture));
            }
            else if (searchType == "AVA")
            {
                condition = "where a.insertedSessionId is null ";
            }

            DataTable dt = dbAttend.searchAbsentees(condition, p);

            return dt;
        }

        public bool isTraineeMeetModuleAttendance(string traineeId, int batchModuleId)
        {
            DataTable dtAbs = dbAttend.getTraineeModuleAbsentSession(traineeId, batchModuleId);
            int totalSessions = (new DB_Bundle()).getBundleModuleNoOfSessions(batchModuleId);

            return ((float)dtAbs.Rows.Count / (float)totalSessions) * 100 <= MAX_ABSENCE_RATE;
        }

        public DataTable getAttendanceSessions(string criteria, string value, int userId, bool viewAll)
        {
            string condition = "";
            List<SqlParameter> p = new List<SqlParameter>();
            if (criteria == "M")
            {
                condition = "(UPPER(m.moduleCode) like @m or UPPER(m.moduleTitle) like @m)";
                p.Add(new SqlParameter("@m", "%" + value.ToUpper() + "%"));
            }
            else if (criteria == "P")
            {
                condition = "(UPPER(p.programmeCode) like @p or UPPER(p.programmeTitle) like @p)";
                p.Add(new SqlParameter("@p", "%" + value.ToUpper() + "%"));
            }
            else if (criteria == "D")
            {
                condition = "s.sessionDate=@dt";
                p.Add(new SqlParameter("@dt", DateTime.ParseExact(value, "dd MMM yyyy", CultureInfo.InvariantCulture)));
            }

            if (!viewAll)
            {
                condition += " and (bm.trainerUserId1=@uid or bm.trainerUserId2=@uid or bm.assessorUserId=@uid)";
                p.Add(new SqlParameter("@uid", userId));
            }

            //get the sessions based on search criteria
            DataTable dt = dbAttend.getSessionList(condition, p.ToArray());

            if (dt == null) return dt;

            dt.Columns.Add(new DataColumn("hasInserted", typeof(bool)));
            if (dt.Rows.Count == 0) return dt;
            
            List<int> ids = new List<int>();
            foreach (DataRow dr in dt.Rows) ids.Add((int)dr["sessionId"]);

            DataTable dtInsertedAbs = dbAttend.getInsertedAbsentTrainees(ids.ToArray());
            DataTable dtInsertedSitIn = dbAttend.getInsertedSitInTrainees(ids.ToArray());
            DataTable dtInsertedReassessment = dbAttend.getInsertedReassessmentTrainees(ids.ToArray());
            if (dtInsertedAbs == null || dtInsertedSitIn == null || dtInsertedReassessment == null) return null; 
            
            //for each session found, check if there is any inserted attendees
            foreach (DataRow dr in dt.Rows)
            {
                if ((dtInsertedAbs.Rows.Count == 0) && dtInsertedSitIn.Rows.Count == 0 && dtInsertedReassessment.Rows.Count == 0) 
                    dr["hasInserted"] = false;
                else
                {
                    dr["hasInserted"] = dtInsertedAbs.Select("sessionId=" + dr["sessionId"].ToString()).Length > 0
                        || dtInsertedSitIn.Select("sessionId=" + dr["sessionId"].ToString()).Length > 0
                        || dtInsertedReassessment.Select("sessionId=" + dr["sessionId"].ToString()).Length > 0 ? true : false;
                }
            }

            return dt;
        }

        public DataTable getSessionTrainees(int sessionId)
        {
            return dbAttend.getSessionTrainees(sessionId);
        }

        public DataTable getSessionAbsentees(int sessionId)
        {
            DataTable dt1 = dbAttend.getExistingEnrolledAbsentees(sessionId, new string[] { }, false);
            DataTable dt2 = dbAttend.getExistingInsertedAbsentees(sessionId);
            if (dt2 == null) return null;

            dt1.Merge(dt2);

            return dt1;
        }

        public DataTable getSessionInsertedTrainees(int sessionId)
        {
            DataTable dt1 = dbAttend.getInsertedAbsentTrainees(new int[] { sessionId });
            if (dt1 == null) return null;

            DataTable dt2 = dbAttend.getInsertedSitInTrainees(new int[] { sessionId });
            if (dt2 == null) return null;

            DataTable dt3 = dbAttend.getInsertedReassessmentTrainees(new int[] { sessionId });
            if (dt3 == null) return null;

            //merge the 3 tables together
            dt1.Merge(dt2);
            dt1.Merge(dt3);

            return dt1.Rows.Count==0? dt1 : dt1.Select("", "fullName").CopyToDataTable();
        }

        public Tuple<int, string> updateAttendance(int sessionId, int batchModuleId, string[] absentees, string[] insertedAbsentees, string[] insertedTrainees, int userId, bool overwrite)
        {
            //check if updating any absentees where makeup class is already set, if yes prompt the user should overwrite
            if (!overwrite)
            {   
                DataTable dt = dbAttend.getExistingEnrolledAbsentees(sessionId, absentees, true);
                if (dt == null) return new Tuple<int, string>(-1, "Error saving attendance.");
                if (dt.Rows.Count > 0)
                {
                    string msg = "";
                    foreach (DataRow dr in dt.Rows)
                    {
                        msg += "<li>" + dr["fullName"].ToString() + " (" + dr["traineeId"].ToString() + ")</li>";
                    }
                    return new Tuple<int, string>(1, msg);
                }
            }

            if (dbAttend.updateAttendance(sessionId, batchModuleId, absentees, insertedAbsentees, insertedTrainees, userId))
                return new Tuple<int, string>(0, "Attendance saved successfully.");
            else
                return new Tuple<int, string>(-1, "Error saving attendance.");
        }
    }
}