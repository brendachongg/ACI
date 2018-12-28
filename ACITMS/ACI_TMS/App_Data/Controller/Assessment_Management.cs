using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace LogicLayer
{
    public class Assessment_Management
    {
        private DB_Assessment dbAssess = new DB_Assessment();


        public bool isOwnBatchModule(int userId, int batchModuleId)
        {
            return dbAssess.isOwnBatchModule(userId, batchModuleId);
        }

        public int getNoOfUnprocessedSOA()
        {
            return dbAssess.getNoOfUnprocessedSOA();
        }

        public Tuple<bool, string> generateSOA(List<Tuple<string, int>> lstTrainees, int contactUserId, string certCode, string path, int userId)
        {
            DataTable dt = (new DB_Users()).getUser(contactUserId);
            if (dt == null || dt.Rows.Count == 0) return new Tuple<bool, string>(false, "Error retrieving contact user details.");

            Cryptography crypt = new Cryptography();
            string contactInfo = "\"" + dt.Rows[0]["userName"].ToString().Replace("\"", "\"\"") + "\",\"" + dt.Rows[0]["salutation"].ToString() + "\",\"" + dt.Rows[0]["contactNumber1"].ToString() + "\","
                + "\"" + (dt.Rows[0]["contactNumber2"] == DBNull.Value ? "" : dt.Rows[0]["contactNumber2"].ToString()) + "\",\"" + crypt.decryptInfo(dt.Rows[0]["userEmail"].ToString()) + "\","
                + "\"\",\"\",\"\",\"\",\"\",\"\",\"" + ConfigurationManager.AppSettings["SOAVersion"].ToString() + "\"" + Environment.NewLine;

            dt = dbAssess.getSOADetails(lstTrainees);
            if (dt == null) return new Tuple<bool, string>(false, "Error retrieving trainees details.");

            string fileName = userId + "_SOA_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".csv";
            string filePath = path + fileName;

            try
            {
                if (File.Exists(filePath)) File.Delete(filePath);

                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(contactInfo);

                    foreach (DataRow dr in dt.Rows)
                    {
                        dr[1] = crypt.decryptInfo(dr[1].ToString());    //id number
                        //dr[7] = crypt.decryptInfo(dr[7].ToString());    //contact 1
                        if (dr[7].ToString() != "") dr[7] = crypt.decryptInfo(dr[7].ToString()); //contact 1
                        if (dr[8].ToString() != "") dr[8] = crypt.decryptInfo(dr[8].ToString()); //contact 2
                        if (dr[9].ToString() != "") dr[9] = crypt.decryptInfo(dr[9].ToString());    //email
                        dr[23] = crypt.decryptInfo(dr[23].ToString());  //trainer id number
                        dr[24] = crypt.decryptInfo(dr[24].ToString());  //assessor id number

                        sw.Write("\"" + dr[0].ToString() + "\"");
                        for (int i = 1; i < dt.Columns.Count; i++) 
                        {
                            //TODO: temp measure to insert the cert code provided by ASP, should be retrieved from database
                            if (i == 19) sw.Write(",\"" + certCode + "\"");
                            else sw.Write(",\"" + dr[i].ToString().Replace("\"", "\"\"") + "\"");
                        }

                        sw.Write(Environment.NewLine);
                    }

                    sw.Flush();
                }

                if (!dbAssess.updateSOAStatus(lstTrainees, SOAStatus.PROC, "processSOADate", userId))
                {
                    File.Delete(filePath);
                    return new Tuple<bool, string>(false, "Error updating SOA status.");
                }
                else return new Tuple<bool, string>(true, fileName);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Assessment_Management.cs", "generateSOA()", ex.Message, userId);

                if (File.Exists(filePath)) File.Delete(filePath);

                return new Tuple<bool, string>(false, "Error generating SOA file.");
            }
        }

        public Tuple<bool, string> receiveSOA(List<Tuple<string, int>> lstTrainees, int userId)
        {
            if (dbAssess.updateSOAStatus(lstTrainees, SOAStatus.RSOA, "receivedSOADate", userId))
            {
                return new Tuple<bool, string>(true, "SOA received status saved successfully.");
            }
            else
            {
                return new Tuple<bool, string>(false, "Error saving SOA received status.");
            }
        }

        public DataTable searchSOATrainee(string type, string value1, string value2, SOAStatus[] status)
        {
            string condition = "";
            List<SqlParameter> p = new List<SqlParameter>();
            Cryptography crypt = new Cryptography();

            if (type == "T")
            {
                condition = "UPPER(m.traineeId) like @sv or UPPER(t.fullName) like @sv or t.idNumber=@sv1";
                p.Add(new SqlParameter("@sv1", crypt.encryptInfo(value1.ToUpper())));
            }
            else if (type == "M") condition = "UPPER(mod.moduleCode) like @sv or UPPER(mod.moduleTitle) like @sv";
            else if (type == "CM")
            {
                condition = "(UPPER(b.batchCode) like @sv or UPPER(b.projectCode) like @sv) and (UPPER(mod.moduleCode) like @sv1 or UPPER(mod.moduleTitle) like @sv1)";
                p.Add(new SqlParameter("@sv1", "%" + value2.ToUpper() + "%"));
            }

            p.Add(new SqlParameter("@sv", "%" + value1.ToUpper() + "%"));

            DataTable dt = dbAssess.searchSOATrainee(condition, p.ToArray(), status);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows) dr["idNumber"] = crypt.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public Tuple<bool, string> updateRepeatMod(string traineeId, int batchModuleId, int repeatProgrammeBatchId, int repeatModuleId, int userId)
        {
            //check if the selected repeat module clash with any of the trainee existing enroll programme or makeup or repeat
            DataTable dtClash = dbAssess.getClashSession(traineeId, "select 1 from batchModule_session s inner join batch_module bm on bm.batchModuleId=s.batchModuleId and bm.programmeBatchId=@bid and bm.moduleId=@mid "
                + "where s.sessionDate=t.sessionDate and s.sessionPeriod=t.sessionPeriod", new SqlParameter[] { new SqlParameter("@bid", repeatProgrammeBatchId), new SqlParameter("@mid", repeatModuleId) });

            if (dtClash == null) return new Tuple<bool, string>(false, "Error checking trainee's details.");
            if (dtClash.Rows.Count > 0)
            {
                string errMsg = "Selected module's session clashes with the existing session(s) that trainee has been enrolled for:";
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

            //check if exceed the all the batch session capacity
            DB_Batch_Session dbBatch = new DB_Batch_Session();
            int repeatBatchModuleId = dbBatch.getBatchModuleId(repeatProgrammeBatchId, repeatModuleId);
            int bCapacity = dbBatch.getBatchCapacityByBatchModule(repeatBatchModuleId);
            if (bCapacity == -1) return new Tuple<bool, string>(false, "Error updating repeat module details.");
            int currCapacity = (new DB_Attendance()).getMaxBatchSessionsEnrollment(repeatProgrammeBatchId);
            if (currCapacity == -1) return new Tuple<bool, string>(false, "Error updating repeat module detail.");
            if (currCapacity + 1 > bCapacity) return new Tuple<bool, string>(false, "Exceeded selected class capacity.");

            if (dbAssess.updateRepeatMod(traineeId, batchModuleId, repeatProgrammeBatchId, repeatModuleId, userId))
                return new Tuple<bool, string>(true, "Repeat module details updated succesfully.");
            else
                return new Tuple<bool, string>(false, "Error updating repeat module details.");
        }

        public Tuple<bool, string> updateReasessment(string traineeId, int batchModuleId, int sessionId, int userId)
        {
            //check if the selected reassessment session clash with any of the trainee existing enroll programme or makeup or repeat
            DataTable dtClash = dbAssess.getClashSession(traineeId, "select 1 from batchModule_session s where s.sessionId=@sid and s.sessionDate=t.sessionDate and s.sessionPeriod=t.sessionPeriod",
                new SqlParameter[] { new SqlParameter("@sid", sessionId) });

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

            //check if exceed the session capacity
            int bCapacity = (new DB_Batch_Session()).getBatchCapacityBySession(sessionId);
            if (bCapacity == -1) return new Tuple<bool, string>(false, "Error updating re-assessment.");
            int currCapacity = (new DB_Attendance()).getSessionEnrollment(sessionId);
            if (currCapacity == -1) return new Tuple<bool, string>(false, "Error re-assessment.");
            if (currCapacity + 1 > bCapacity) return new Tuple<bool, string>(false, "Exceeded session capacity.");

            if (dbAssess.updateReasessment(traineeId, batchModuleId, sessionId, userId))
                return new Tuple<bool, string>(true, "Re-assessment updated succesfully.");
            else
                return new Tuple<bool, string>(false, "Error updating re-assessment.");
        }

        public bool removeReassessment(string traineeId, int batchModuleId, int userId)
        {
            return dbAssess.removeReassessment(traineeId, batchModuleId, userId);
        }

        public bool removeRepeatMod(string traineeId, int batchModuleId, int userId)
        {
            return dbAssess.removeRepeatMod(traineeId, batchModuleId, userId);
        }

        public Tuple<DataTable, DataTable> getReassessmentDetails(string traineeId, int batchModuleId)
        {
            DataTable dt = dbAssess.getReassessmentDetails(traineeId, batchModuleId);
            if (dt == null || dt.Rows.Count == 0) return null;

            DataTable dtRe = null;
            if (dt.Rows[0]["finalAssessmentSessionId"] != DBNull.Value)
            {
                dtRe = (new DB_Batch_Session()).getSessionDetails(int.Parse(dt.Rows[0]["finalAssessmentSessionId"].ToString()));
                if (dtRe == null || dtRe.Rows.Count == 0) return null;
            }

            return new Tuple<DataTable, DataTable>(dt, dtRe);
        }

        public Tuple<DataTable, DataTable> getRepeatModDetails(string traineeId, int batchModuleId)
        {
            DataTable dt = dbAssess.getRepeatModDetails(traineeId, batchModuleId);
            if (dt == null || dt.Rows.Count == 0) return null;

            DataTable dtRe = null;
            if (dt.Rows[0]["reTakeBatchModuleId"] != DBNull.Value)
            {
                dtRe = (new DB_Batch_Session()).getBatchModuleInfo(int.Parse(dt.Rows[0]["reTakeBatchModuleId"].ToString()));
                if (dtRe == null || dtRe.Rows.Count == 0) return null;
            }

            return new Tuple<DataTable, DataTable>(dt, dtRe);
        }

        public DataTable searchTrainee(string search, int userId, bool searchAll)
        {
            DataTable dt = dbAssess.searchTraineeInfo(search, userId, searchAll);
            if (dt != null)
            {
                Cryptography decryp = new Cryptography();
                foreach (DataRow dr in dt.Rows)
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public DataTable searchClass(string search, int userId, bool searchAll)
        {
            DataTable dt = dbAssess.searchClassInfo(search, userId, searchAll);
            //if (dt != null)
            //{
            //    Cryptography decryp = new Cryptography();
            //    foreach (DataRow dr in dt.Rows)
            //        dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
            //}

            return dt;

        }

        public DataTable searchModule(string search, int userId, bool searchAll)
        {
            return dbAssess.searchBatchModuleInfo(search, userId, searchAll);
        }

        public DataTable searchRepeatModTrainees(string search, string value)
        {
            string condition = null;
            SqlParameter p = null;

            if (search != null)
            {
                if (search == "T")
                {
                    condition = "UPPER(tm.traineeId) like @t or UPPER(t.fullName) like @t";
                    p = new SqlParameter("@t", "%" + value.ToUpper() + "%");
                }
                else if (search == "M")
                {
                    condition = "UPPER(m.moduleCode) like @m or UPPER(m.moduleTitle) like @m";
                    p = new SqlParameter("@m", "%" + value.ToUpper() + "%");
                }
            }

            DataTable dt = dbAssess.searchRepeatModTrainees(condition, p);
            if (dt != null)
            {
                Cryptography decryp = new Cryptography();
                foreach (DataRow dr in dt.Rows)
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public DataTable searchReassessmentTrainees(string search, string value)
        {
            string condition = null;
            SqlParameter p = null;

            if (search != null)
            {
                if (search == "T")
                {
                    condition = "UPPER(tm.traineeId) like @t or UPPER(t.fullName) like @t";
                    p = new SqlParameter("@t", "%" + value.ToUpper() + "%");
                }
                else if (search == "M")
                {
                    condition = "UPPER(m.moduleCode) like @m or UPPER(m.moduleTitle) like @m";
                    p = new SqlParameter("@m", "%" + value.ToUpper() + "%");
                }
            }

            DataTable dt = dbAssess.searchReassessmentTrainees(condition, p);
            if (dt != null) { 
                Cryptography decryp = new Cryptography();
                foreach (DataRow dr in dt.Rows)
                    dr["idNumber"] = decryp.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public DataTable getTraineeModuleInfo(string traineeId, int batchModuleId)
        {
            return dbAssess.getTraineeModuleInfo(traineeId, batchModuleId);
        }

        public DataTable getTraineeModuleAssessment(string traineeId, int batchModuleId)
        {
            DataTable dt = dbAssess.getTraineeModuleAssessment(traineeId, batchModuleId);
            //if (dt != null && dt.Rows.Count > 0)
            //{
            //    ACI_Staff_User s = new ACI_Staff_User();
            //    if (dt.Rows[0]["firstAssessorName"] != DBNull.Value) dt.Rows[0]["firstAssessorName"] = s.decryptionString(dt.Rows[0]["firstAssessorName"].ToString());
            //    if (dt.Rows[0]["finalAssessorName"] != DBNull.Value) dt.Rows[0]["finalAssessorName"] = s.decryptionString(dt.Rows[0]["finalAssessorName"].ToString());
            //}

            return dt;
        }

        public DataTable getBatchModuleTraineesAssessment(int batchModuleId)
        {
            DataTable dt = dbAssess.getBatchModuleTraineesAssessment(batchModuleId);
            if (dt == null) return null;

            ACI_Staff_User s = new ACI_Staff_User();

            DataTable dtAssessment = new DataTable();
            dtAssessment.Columns.Add(new DataColumn("traineeId", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("fullName", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("assessmentDate", typeof(DateTime)));
            dtAssessment.Columns.Add(new DataColumn("assessmentDateDisp", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("assessorId", typeof(int)));
            dtAssessment.Columns.Add(new DataColumn("assessorName", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("moduleResult", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("moduleResultDisp", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("attempt", typeof(int)));

            DataRow dr1st, dr2nd;
            foreach (DataRow dr in dt.Rows)
            {
                dr1st = dtAssessment.NewRow();
                dr2nd = dtAssessment.NewRow();

                dr1st["traineeId"] = dr2nd["traineeId"] = dr["traineeId"];
                dr1st["fullName"] = dr2nd["fullName"] = dr["fullName"];
                dr1st["attempt"] = 1;
                dr2nd["attempt"] = 2;

                if (dr["firstAssessmentDate"] != DBNull.Value)
                {
                    dr1st["assessmentDate"] = dr["firstAssessmentDate"];
                    dr1st["assessmentDateDisp"] = dr["firstAssessmentDateDisp"];
                    dr1st["assessorId"] = dr["firstAssessorId"];
                    dr1st["assessorName"] = dr["firstAssessorName"].ToString();

                    if (dr["reAssessment"].ToString() == "N" && dr["moduleResult"].ToString() == ModuleResult.C.ToString()) 
                    {
                        dr1st["moduleResult"] = ModuleResult.C.ToString();
                        dr1st["moduleResultDisp"] = "Competent"; 
                    }
                    if (dr["reAssessment"].ToString() == "Y")
                    {
                        dr1st["moduleResult"] = ModuleResult.NYC.ToString();
                        dr1st["moduleResultDisp"] = "Not Yet Competent";
                    }
                }

                if (dr["finalAssessmentDate"] != DBNull.Value)
                {
                    dr2nd["assessmentDate"] = dr["finalAssessmentDate"];
                    dr2nd["assessmentDateDisp"] = dr["finalAssessmentDateDisp"];
                    dr2nd["assessorId"] = dr["finalAssessorId"];
                    dr2nd["assessorName"] = dr["finalAssessorName"].ToString();

                    if (dr["moduleResult"].ToString() == ModuleResult.C.ToString())
                    {
                        dr2nd["moduleResult"] = ModuleResult.C.ToString();
                        dr2nd["moduleResultDisp"] = "Competent";
                    }
                    else
                    {
                        dr2nd["moduleResult"] = ModuleResult.NYC.ToString();
                        dr2nd["moduleResultDisp"] = "Not Yet Competent";
                    }
                }

                dtAssessment.Rows.Add(dr1st);
                dtAssessment.Rows.Add(dr2nd);
            }

            return dtAssessment;
        }

        public Tuple<bool, string> updateBatchModuleAssessment(int batchModuleId, DataTable dtAssessment, int userId)
        {
            dtAssessment.Columns.Add(new DataColumn("moduleResult", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("assessmentCompleted", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("reAssessment", typeof(string)));
            dtAssessment.Columns.Add(new DataColumn("reTakeModule", typeof(string)));

            foreach (DataRow dr in dtAssessment.Rows)
            {
                if (dr["moduleResult1"] == DBNull.Value) continue;

                if (dr["moduleResult1"].ToString() == ModuleResult.C.ToString())
                {
                    dr["moduleResult"] = ModuleResult.C.ToString();
                    dr["assessmentCompleted"] = "Y";
                    dr["reAssessment"] = "N";
                    dr["reTakeModule"] = "N";
                }
                else
                {
                    if (dr["moduleResult2"] != DBNull.Value)
                    {
                        if (dr["moduleResult2"].ToString() == ModuleResult.C.ToString())
                        {
                            dr["moduleResult"] = ModuleResult.C.ToString();
                            dr["assessmentCompleted"] = "Y";
                            dr["reAssessment"] = "Y";
                            dr["reTakeModule"] = "N";
                        }
                        else
                        {
                            dr["moduleResult"] = ModuleResult.NYC.ToString();
                            dr["assessmentCompleted"] = "N";
                            dr["reAssessment"] = "Y";
                            dr["reTakeModule"] = "Y";
                        }
                    }
                    else dr["reAssessment"] = "Y";
                }
            }

            if (dbAssess.updateBatchModuleAssessment(batchModuleId, dtAssessment, userId))
                return new Tuple<bool, string>(true, "Assessment updated successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating assessment.");
        }

        public Tuple<bool, string> updateTraineeAssessment(string traineeId, int batchModuleId, DateTime dt1stDt, DateTime dt2ndDt, int assessorId1, int assessorId2, 
            string result1, string result2, int userId)
        {
            if(!(new Attendance_Management()).isTraineeMeetModuleAttendance(traineeId, batchModuleId)){
                return new Tuple<bool, string>(false, "Trainee has not meet attendance, unable to update assessment results.");
            }

            string moduleResult = null, assessmentCompleted = null, reAssessment = null, reTakeModule = null;

            if (result1 == ModuleResult.C.ToString())
            {
                moduleResult = ModuleResult.C.ToString();
                assessmentCompleted = "Y";
                reAssessment = "N";
                reTakeModule = "N";
            }
            else if (result1 == ModuleResult.NYC.ToString())
            {
                if (result2 != null)
                {
                    if (result2 == ModuleResult.C.ToString())
                    {
                        moduleResult = ModuleResult.C.ToString();
                        assessmentCompleted = "Y";
                        reAssessment = "Y";
                        reTakeModule = "N";
                    }
                    else
                    {
                        moduleResult = ModuleResult.NYC.ToString();
                        assessmentCompleted = "N";
                        reAssessment = "Y";
                        reTakeModule = "Y";
                    }
                }
                else
                    reAssessment = "Y";
            }

            if(dbAssess.updateTraineeModuleAssessment(traineeId, batchModuleId, moduleResult, assessmentCompleted, dt1stDt, assessorId1, reAssessment, reTakeModule, dt2ndDt, assessorId2, userId))
                return new Tuple<bool, string>(true, "Trainee assessment results updated successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating trainee assessment results.");
        }
    }
}
