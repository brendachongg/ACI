﻿using GeneralLayer;
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
    public class DB_Attendance
    {
        private Database_Connection dbConnection = new Database_Connection();

        public bool isOwnSession(int userId, int sessionId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from batch_module bm inner join batchModule_session s on bm.batchModuleId=s.batchModuleId where "
                    + "(bm.trainerUserId1=@uid or bm.trainerUserId2=@uid or assessorUserId=@uid) and "
                    + (sessionId == -1 ? "bm.batchModuleId" : "s.sessionId") + "=@id";

                cmd.Parameters.AddWithValue("@id", sessionId == -1 ? batchModuleId : sessionId);
                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.executeScalarInt(cmd) == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "isOwnSession()", ex.Message, -1);

                return false;
            }
        }

        public int getNoOfAbsentee()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                // cmd.CommandText = "select count(*) from trainee_absence_record where defunct='N' and insertedSessionId is null";

                cmd.CommandText = @"select count(*) from trainee_absence_record tar left join batch_module bm on tar.batchModuleId = bm.batchModuleId left join programme_batch pb on                           pb.programmeBatchId = bm.programmeBatchId left join programme_structure ps on pb.programmeId = ps.programmeId 
                                    where tar.defunct='N' and tar.insertedSessionId is null and ps.programmeType ='" + GeneralLayer.ProgrammeType.FQ.ToString() + "' or ps.programmeType ='" + GeneralLayer.ProgrammeType.SCWSQ.ToString() + "'";

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getNoOfAbsentee()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getSessionsTrainees(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT t.fullName, t.idNumber "
                + "FROM [trainee_module] as tm INNER JOIN [trainee] as t "
                + "ON tm.traineeId = t.traineeId and tm.batchModuleId = @batchModuleId and tm.defunct='N' and tm.sitInModule='N' and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + "') "
                + "order by t.fullName";

                cmd.Parameters.AddWithValue("batchModuleId", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionsTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSessionDetails(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT bms.sessionDate, format(bms.sessionDate, 'dd MMM yy') as sessionDateDisp,  bms.sessionPeriod, c.codeOrder, pb.projectCode, "
                + "convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDate, convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDate, "
                + "ms.moduleTitle, ps.courseCode, bm.trainerUserId1, pb.batchCode, v.venueLocation, ms.moduleTrainingHour, ms.moduleCode, u.userName as trainerUserName1, "
                + "bm.assessorUserId, u2.userName as assessorUserName, c.codeValueDisplay as sessionPeriodDisp "
                + "FROM [batch_module] as bm "
                + "INNER JOIN [batchModule_session] as bms "
                + "ON bm.batchModuleId = bms.batchModuleId and bm.batchModuleId=@batchModuleId and bms.defunct='N' "
                + "INNER JOIN [programme_batch] as pb "
                + "ON bm.programmeBatchId = pb.programmeBatchId "
                + "INNER JOIN [programme_structure] as ps "
                + "ON pb.programmeId = ps.programmeId "
                + "INNER JOIN [module_structure] as ms "
                + "ON bm.moduleId = ms.moduleId "
                + "INNER JOIN [venue] as v "
                + "ON bms.venueId = v.venueId "
                + "inner join code_reference c on c.codeValue=bms.sessionPeriod and c.codeType='PERIOD' "
                + "left outer join aci_user u on u.userId=bm.trainerUserId1 "
                + "left outer join aci_user u2 on u2.userId=bm.assessorUserId "
                + "order by bms.sessionDate, c.codeOrder";

                cmd.Parameters.AddWithValue("batchModuleId", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInsertedTrainees(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //sit in trainees
                //cmd.CommandText = "SELECT distinct t.fullName, t.idNumber, 'SI' as type "
                //+ "FROM [trainee_module] as tm INNER JOIN [trainee] as t "
                //+ "ON tm.traineeId = t.traineeId and tm.defunct='N' and tm.batchModuleId = @batchModuleId and tm.sitInModule='Y' "
                //+ "union "
                ////reassessment trainees
                //+ "SELECT distinct t.fullName, t.idNumber, 'RA' as type "
                //+ "FROM [trainee_module] as tm INNER JOIN [trainee] as t "
                //+ "ON tm.traineeId = t.traineeId and tm.defunct='N' and "
                //+ " tm.finalAssessmentSessionId in (select sessionId from batchModule_session where defunct='N' and batchModuleId=@batchModuleId) "
                //+ "union "
                ////make up trainees
                //+ "select distinct t.fullName, t.idNumber, 'MU' as type "
                //+ "from trainee_absence_record a INNER JOIN [trainee] as t "
                //+ "ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId "
                //+ "union "
                ////trainees who has already attended the makeup
                //+ "select distinct t.fullName, t.idNumber, 'MUA' as type "
                //+ "from trainee_absence_removed a INNER JOIN [trainee] as t "
                //+ "ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId "
                //+ "order by 1, 2";

                cmd.CommandText = @"SELECT distinct pb.batchCode, pb.projectcode, ps.courseCode, t.fullName, t.idNumber, 'SI' as type FROM [trainee_module] as tm INNER JOIN [trainee] as t ON tm.traineeId = t.traineeId and tm.defunct='N' and tm.batchModuleId = @batchModuleId and tm.sitInModule='Y' inner join programme_batch pb on tm.programmeBatchId = pb.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId 
                  union 
                  SELECT distinct pb.batchCode, pb.projectcode, ps.courseCode, t.fullName, t.idNumber, 'RA' as type FROM [trainee_module] as tm INNER JOIN [trainee] as t ON tm.traineeId = t.traineeId and tm.defunct='N' and  tm.finalAssessmentSessionId in (select sessionId from batchModule_session where defunct='N' and batchModuleId=@batchModuleId) inner join programme_batch pb on tm.programmeBatchId = pb.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId 
                 union 
                 select distinct pb.batchCode, pb.projectcode, ps.courseCode, t.fullName, t.idNumber, 'MU' as type from trainee_absence_record a INNER JOIN [trainee] as t ON a.traineeId = t.traineeId 
                 and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId inner join trainee_module tm on t.traineeId = tm.traineeId inner join programme_batch pb on 
                tm.programmeBatchId= pb.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId 
                 union 
                  select distinct pb.batchCode, pb.projectcode, ps.courseCode, t.fullName, t.idNumber, 'MUA' as type from trainee_absence_removed a INNER JOIN [trainee] as t ON a.traineeId = 
                  t.traineeId and a.defunct='N' and a.insertedbatchModuleId= @batchModuleId inner join trainee_module tm on t.traineeId = tm.traineeId inner join programme_batch pb 
                  on tm.programmeBatchId = pb.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId  order by 1, 2";
                cmd.Parameters.AddWithValue("batchModuleId", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getInsertedTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInsertedSessionTrainees(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT t.idNumber, bms.sessionDate, bms.sessionPeriod "
                + "FROM [trainee_module] as tm "
                + "INNER JOIN [trainee] as t "
                + "ON tm.traineeId = t.traineeId "
                + "INNER JOIN [batch_module] as bm "
                + "ON tm.batchModuleId = bm.batchModuleId "
                + "INNER JOIN [batchModule_session] as bms "
                + "ON bm.batchModuleId = bms.batchModuleId "
                    //sit in trainees
                + "WHERE tm.defunct='N' and bms.defunct='N' and ((bm.batchModuleId = @batchModuleId and tm.sitInModule='Y') "
                    //reassessment trainees
                + "OR (tm.finalAssessmentSessionId in (select sessionId from batchModule_session where defunct='N' and batchModuleId=@batchModuleId))) "
                + "union "
                    //make up trainees
                + "select t.idNumber, bms.sessionDate, bms.sessionPeriod "
                + "from trainee_absence_record a "
                + "INNER JOIN [trainee] as t "
                + "ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId "
                + "INNER JOIN [batchModule_session] as bms "
                + "ON a.insertedSessionId = bms.sessionId and bms.defunct='N' "
                + "union "
                    //trainees who has already attended the makeup
                + "select t.idNumber, bms.sessionDate, bms.sessionPeriod "
                + "from trainee_absence_removed a "
                + "INNER JOIN [trainee] as t "
                + "ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId "
                + "INNER JOIN [batchModule_session] as bms "
                + "ON a.insertedSessionId = bms.sessionId and bms.defunct='N' "
                + "order by 2";

                //                cmd.CommandText = @"SELECT pb.projectCode,ps.courseCode,pb.batchCode,t.idNumber, bms.sessionDate, bms.sessionPeriod FROM [trainee_module] as tm INNER JOIN [trainee] as t ON tm.traineeId = t.traineeId INNER JOIN [batch_module] as bm ON tm.batchModuleId = bm.batchModuleId INNER JOIN [batchModule_session] as bms ON bm.batchModuleId = bms.batchModuleId inner join programme_batch pb on pb.programmeBatchId = tm.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId WHERE tm.defunct='N' and bms.defunct='N' and ((bm.batchModuleId = @batchModuleId and tm.sitInModule='Y') OR (tm.finalAssessmentSessionId in (select sessionId from batchModule_session where defunct='N' and batchModuleId=@batchModuleId))) 
                //                  union 
                //                  select pb.projectCode,ps.courseCode,pb.batchCode, t.idNumber, bms.sessionDate, bms.sessionPeriod 
                //                  from trainee_absence_record a INNER JOIN [trainee] as t ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId INNER JOIN [batchModule_session] as bms ON a.insertedSessionId = bms.sessionId and bms.defunct='N' inner join trainee_module tm on tm.traineeId = t.traineeId inner join programme_batch pb on pb.programmeBatchId = tm.programmeBatchId  inner join programme_structure ps on pb.programmeId = ps.programmeId  
                //                  union 
                //                  select pb.projectCode,ps.courseCode, pb.batchCode,t.idNumber, bms.sessionDate, bms.sessionPeriod from trainee_absence_removed a INNER JOIN [trainee] as t 
                //                 ON a.traineeId = t.traineeId and a.defunct='N' and a.insertedbatchModuleId=@batchModuleId INNER JOIN [batchModule_session] as bms ON a.insertedSessionId = bms.sessionId and bms.defunct='N' inner join trainee_module tm on t.traineeId = tm.traineeId inner join programme_batch pb 
                //                on pb.programmeBatchId = tm.programmeBatchId inner join programme_structure ps on pb.programmeId = ps.programmeId 
                //                order by 2";

                cmd.Parameters.AddWithValue("batchModuleId", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getInsertedSessionTrainees()", ex.Message, -1);

                return null;
            }
        }

        public int getMaxBatchSessionsEnrollment(int programmeBatchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @";with allBatchSessionCTE as ( 
	                                    --find out all the sessions of the batch
	                                    select s.sessionId, s.batchModuleId from batchModule_session s inner join batch_module bm on s.batchModuleId=bm.batchModuleId and s.defunct='N' and bm.defunct='N' 
	                                    inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId and pb.programmeBatchId=@pbid
                                    )
                                    select isnull(max(noOfTrainees), 0) from (
	                                    --count the no of trainees enrolled in each session of the class
	                                    select sessionId, count(traineeId) as noOfTrainees from (
		                                    --find the trainees enrolled in each session of the class (including make up and reassessment)
		                                    select b.sessionId, tm.traineeId
		                                    from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"' and tm.defunct='N'
		                                    inner join allBatchSessionCTE b on (tm.batchModuleId=b.batchModuleId and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + @"')) 
			                                    or tm.finalAssessmentSessionId=b.sessionId
		                                    union
		                                    select bs.sessionId, a.traineeId from trainee_absence_record a inner join allBatchSessionCTE bs on bs.sessionId=a.insertedSessionId and a.defunct='N'
	                                    ) t group by sessionId
                                    ) t";

                cmd.Parameters.AddWithValue("@pbid", programmeBatchId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getMaxBatchSessionsEnrollment()", ex.Message, -1);

                return -1;
            }
        }

        public int getSessionEnrollment(int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //get the normal enrolled trainees or added for reassessment
                cmd.CommandText = "select traineeId from trainee_module where defunct='N' and ((batchModuleId=(select batchModuleId from batchModule_session where sessionId=@sid) and "
                    + "(moduleResult is null or moduleResult<>'" + ModuleResult.EXEM.ToString() + "')) or finalAssessmentSessionId=@sid) union all "
                    //get the trainees who have been added for makeup
                    + "select traineeId from trainee_absence_record where insertedSessionId=@sid and defunct='N' ";

                cmd.Parameters.AddWithValue("@sid", sessionId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt.Rows.Count;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionEnrollment()", ex.Message, -1);

                return -1;
            }
        }

        public int getSessionEnrollment(int batchModuleId, int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //get the normal enrolled trainees or added for reassessment
                cmd.CommandText = "select traineeId from trainee_module where defunct='N' and ((batchModuleId=@bmid and (moduleResult is null or moduleResult<>'" + ModuleResult.EXEM.ToString() + "')) or finalAssessmentSessionId=@sid) union all "
                    //get the trainees who have been added for makeup
                    + "select traineeId from trainee_absence_record where insertedSessionId=@sid and defunct='N' ";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@sid", sessionId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt.Rows.Count;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionEnrollment()", ex.Message, -1);

                return -1;
            }
        }

        public Tuple<DateTime, string> getMinAbsenceSessionDate(int makeupSessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select min(convert(nvarchar, s.sessionDate, 112) + (case when s.sessionPeriod='" + DayPeriod.AM.ToString() + "' then '1' when s.sessionPeriod='" + DayPeriod.PM.ToString() + "' then '2' else '3' end)) "
                    + "from (select sessionId from trainee_absence_record where insertedSessionId=@sid and defunct='N' "
                    + "union select sessionId from trainee_absence_removed where insertedSessionId=@sid and defunct='N') a inner join batchModule_session s on "
                    + "s.sessionId=a.sessionId";

                cmd.Parameters.AddWithValue("@sid", makeupSessionId);

                DataTable dt = dbConnection.getDataTable(cmd);
                if (dt.Rows[0][0] == DBNull.Value) return new Tuple<DateTime, string>(DateTime.MinValue, null);

                DateTime d = DateTime.ParseExact(dt.Rows[0][0].ToString().Substring(0, dt.Rows[0][0].ToString().Length - 1), "yyyyMMdd", CultureInfo.InvariantCulture);
                string period = dt.Rows[0][0].ToString().Substring(dt.Rows[0][0].ToString().Length - 2);
                if (period == "1") period = DayPeriod.AM.ToString();
                else if (period == "2") period = DayPeriod.PM.ToString();
                else period = DayPeriod.EVE.ToString();

                return new Tuple<DateTime, string>(d, period);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getMinAbsentSessionDate()", ex.Message, -1);

                return null;
            }
        }

        public bool updateMakeup(string traineeId, int sessionId, bool isAbsValid, string absReason, int makeupBatchModuleId, int makeupSessionId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_absence_record set isAbsentValid=@absValid, AbsentRemarks=@remarks "
                    + (makeupBatchModuleId != -1 && makeupSessionId != -1 ? ", insertedbatchModuleId=@bmid, insertedSessionId=@msid" : "")
                    + ", lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and sessionId=@sid";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@sid", sessionId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@absValid", isAbsValid ? "Y" : "N");
                cmd.Parameters.AddWithValue("@remarks", absReason == null ? (object)DBNull.Value : absReason);

                if (makeupBatchModuleId != -1 && makeupSessionId != -1)
                {
                    cmd.Parameters.AddWithValue("@bmid", makeupBatchModuleId);
                    cmd.Parameters.AddWithValue("@msid", makeupSessionId);
                }

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "updateMakeup()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getTraineeAbsence(string traineeId, int sessionId, bool isMakeup)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //absence session fields
                cmd.CommandText = "select a.absentId, a.traineeId, a.sessionId, a.insertedSessionId, t.fullName, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.sessionPeriod, c1.codeValueDisplay as sessionPeriodDisp,  "
                    + "s.venueId, v.venueLocation, bm.batchModuleId, bm.programmeBatchId, bm.moduleId, m.moduleCode, m.moduleTitle, a.isAbsentValid, a.AbsentRemarks, left(pb.batchCode, len(pb.batchCode)-len(c2.codeValue)) as batchCode, "
                    + "c2.codeValue as batchType, c2.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, "
                    + "pb.programmeRegEndDate, convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, "
                    + "pb.programmeCompletionDate, convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c3.codeValueDisplay as classModeDisp, "
                    + "pb.programmeId, p.programmeCode, p.programmeTitle, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeLevel, c5.codeValueDisplay as programmeLevelDisp, "
                    + "p.programmeVersion, p.programmeType, c6.codeValueDisplay as programmeTypeDisp "

                    //absence session tables
                    + "from trainee_absence_record a inner join trainee t on a.traineeId=t.traineeId and a.traineeId=@tid and a.sessionId=@sid "
                    + "inner join batchModule_session s on s.sessionId=a." + (isMakeup ? "insertedSessionId" : "sessionId") + " "
                    + "inner join code_reference c1 on c1.codeValue=s.sessionPeriod and c1.codeType='PERIOD' inner join Venue v on v.venueId=s.venueId "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join module_structure m on m.moduleId=bm.moduleId "
                    + "inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId "

                    + "inner join code_reference c2 on c2.codeValue=right(pb.batchCode, 5) and c2.codeType='CLTYPE' "
                    + "inner join code_reference c3 on c3.codeValue=pb.classMode and c3.codeType='CLMODE' inner join programme_structure p on p.programmeId=pb.programmeId "
                    + "inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' inner join code_reference c5 on c5.codeValue=p.programmeLevel and c5.codeType='PGLVL' "
                    + "inner join code_reference c6 on c6.codeValue=p.programmeType and c6.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@sid", sessionId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getTraineeAbsence()", ex.Message, -1);

                return null;
            }
        }

        public bool hasPaymentRef(string traineeId, int absentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from payment_history ph inner join payment_details pd on ph.paymentId=pd.paymentId "
                    //payment must be cleared
                    + "where ph.traineeId=@tid and paymentType='" + PaymentType.MAKEUP.ToString() + "' and pd.recordRefId=@sid and ph.paymentStatus in ('" + PaymentStatus.PAID.ToString() + "', '" + PaymentStatus.WAIVED.ToString() + "')";
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@sid", absentId);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "hasPaymentRef()", ex.Message, -1);

                return false;
            }
        }

        public DataTable searchAbsentees(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select a.traineeId, a.batchModuleId, a.sessionId, t.fullName, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, "
                    + "s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, p.batchCode "
                    + "from trainee_absence_record a inner join trainee t on a.traineeId=t.traineeId and a.defunct='N' "
                    + "inner join batchModule_session s on s.sessionId=a.sessionId and a.batchModuleId=s.batchModuleId "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join programme_batch p on  p.programmeBatchId=bm.programmeBatchId "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + (condition == null ? "" : condition) + " order by t.fullName, s.sessionDate, c.codeOrder";

                if (p != null) cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "searchAbsentees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSessionTrainees(int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.sessionId, m.traineeId, t.fullName "
                    + "from trainee_module m inner join trainee t on m.traineeId=t.traineeId and m.defunct='N' and (m.moduleResult is null or m.moduleResult<>'" + ModuleResult.EXEM.ToString() + "') "
                    + "inner join batchModule_session s on s.batchModuleId=m.batchModuleId "
                    + "where m.sitInModule='N' and s.sessionId=@sid order by t.FullName";
                cmd.Parameters.AddWithValue("@sid", sessionId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInsertedReassessmentTrainees(int[] sessionIds)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select m.finalAssessmentSessionId as sessionId, m.traineeId, t.fullName "
                    + "from trainee_module m inner join trainee t on m.traineeId=t.traineeId and m.defunct='N' "
                    + "where m.finalAssessmentSessionId in (";

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < sessionIds.Length; i++)
                {
                    sb.Append("@s" + i + ",");
                    cmd.Parameters.AddWithValue("@s" + i, sessionIds[i]);
                }

                cmd.CommandText += sb.ToString().Substring(0, sb.Length - 1) + ") ";
                cmd.CommandText += "order by m.finalAssessmentSessionId, t.fullName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getInsertedReassessmentTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInsertedSitInTrainees(int[] sessionIds)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.sessionId, m.traineeId, t.fullName "
                    + "from trainee_module m inner join trainee t on m.traineeId=t.traineeId and m.defunct='N' inner join batchModule_session s on s.batchModuleId=m.batchModuleId "
                    + "where m.sitInModule='Y' and s.sessionId in (";

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < sessionIds.Length; i++)
                {
                    sb.Append("@s" + i + ",");
                    cmd.Parameters.AddWithValue("@s" + i, sessionIds[i]);
                }

                cmd.CommandText += sb.ToString().Substring(0, sb.Length - 1) + ") ";
                cmd.CommandText += "order by s.sessionId, t.fullName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getInsertedSitInTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInsertedAbsentTrainees(int[] sessionIds)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < sessionIds.Length; i++)
                {
                    sb.Append("@s" + i + ",");
                    cmd.Parameters.AddWithValue("@s" + i, sessionIds[i]);
                }
                cmd.CommandText = "select a.insertedSessionId as sessionId, a.traineeId, t.fullName "
                    + "from trainee_absence_record a inner join trainee t on t.traineeId=a.traineeId and a.defunct='N' "
                    + "where a.insertedSessionId in (" + sb.ToString().Substring(0, sb.Length - 1) + ") union "
                    + "select a.insertedSessionId as sessionId, a.traineeId, t.fullName "
                    + "from trainee_absence_removed a inner join trainee t on t.traineeId=a.traineeId and a.defunct='N' "
                    + "where a.insertedSessionId in (" + sb.ToString().Substring(0, sb.Length - 1) + ") ";
                cmd.CommandText += "order by 1, 3";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getInsertedAbsentTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSessionList(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.sessionId, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.sessionPeriod, sc1.codeValueDisplay as sessionPeriodDisp, "
                    + "s.batchModuleId, b.batchCode, m.moduleCode, m.moduleTitle, p.programmeCode, programmeTitle "

                    + "from batchModule_session s inner join code_reference sc1 on sc1.codeValue=s.sessionPeriod and sc1.codeType='PERIOD' and s.defunct='N' "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId "
                    + "inner join module_structure m on m.moduleId=bm.moduleId inner join programme_structure p on p.programmeId=b.programmeId "
                    + "where s.batchModuleId in (select tm.batchModuleId from trainee_module tm where tm.sitInModule='N' and tm.defunct='N') and "
                    + condition + " order by s.sessionDate, sc1.codeOrder, b.batchCode";

                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getSessionList()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getExistingInsertedAbsentees(int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                //if the attendance has been marked before, and yet the trainee makeup record is still in the table, means the trainee was absent for the makeup session
                cmd.CommandText = "select a.traineeId, t.fullName from trainee_absence_record a inner join trainee t on t.traineeId=a.traineeId and a.defunct='N' "
                    + "and a.insertedSessionId=@sid inner join batchModule_session s on s.sessionId=a.insertedSessionId and attendanceMarked='Y'";

                cmd.Parameters.AddWithValue("@sid", sessionId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getExistingInsertedAbsentees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getExistingEnrolledAbsentees(int sessionId, string[] newAbsentees, bool withMakeup)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                string abs = "";
                for (int i = 0; i < newAbsentees.Length; i++)
                {
                    abs += "@t" + i + ",";
                    cmd.Parameters.AddWithValue("@t" + i, newAbsentees[i]);
                }
                if (abs != "") abs = abs.Substring(0, abs.Length - 1);

                cmd.CommandText = "select a.traineeId, t.fullName from trainee_absence_record a inner join trainee t on t.traineeId=a.traineeId and a.defunct='N' "
                    + "where " + (newAbsentees.Length == 0 ? "" : "a.traineeId not in (" + abs + ") and ") + " a.defunct='N' "
                    + (withMakeup ? "and a.insertedSessionId is not null " : "") + " and a.sessionId=@sid "
                    + "union select a.traineeId, t.fullName from trainee_absence_removed a inner join trainee t on t.traineeId=a.traineeId and a.defunct='N' "
                    + "where " + (newAbsentees.Length == 0 ? "" : "a.traineeId not in (" + abs + ") and ") + " a.sessionId=@sid ";

                cmd.Parameters.AddWithValue("@sid", sessionId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getExistingEnrolledAbsentees()", ex.Message, -1);

                return null;
            }
        }

        public bool updateAttendance(int sessionId, int batchModuleId, string[] absentees, string[] insertedAbsentees, string[] insertedTrainees, int userId)
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
                    //for those inserted trainnees who has attended make up, move the abs record to removed table
                    //meaning those NOT found in insertedAbsentees
                    StringBuilder sbInsAbs = new StringBuilder();
                    List<SqlParameter> lstInsAbsParams = new List<SqlParameter>();
                    if (insertedAbsentees.Length > 0)
                    {
                        sbInsAbs.Append("@ta0");
                        lstInsAbsParams.Add(new SqlParameter("@ta0", insertedAbsentees[0]));

                        for (int i = 1; i < insertedAbsentees.Length; i++)
                        {
                            sbInsAbs.Append(",@ta" + i);
                            lstInsAbsParams.Add(new SqlParameter("@ta" + i, insertedAbsentees[i]));
                        }
                    }
                    StringBuilder sbInsTrn = new StringBuilder();
                    List<SqlParameter> lstInsTrnParams = new List<SqlParameter>();
                    if (insertedTrainees.Length > 0)
                    {
                        sbInsTrn.Append("@tt0");
                        lstInsTrnParams.Add(new SqlParameter("@tt0", insertedTrainees[0]));

                        for (int i = 1; i < insertedTrainees.Length; i++)
                        {
                            sbInsTrn.Append(",@tt" + i);
                            lstInsTrnParams.Add(new SqlParameter("@tt" + i, insertedTrainees[i]));
                        }
                    }

                    cmd.CommandText = "insert into trainee_absence_removed(absentId, traineeId, batchModuleId, sessionId, isAbsentValid, AbsentRemarks, insertedbatchModuleId, insertedSessionId, createdBy) "
                        + "select absentId, traineeId, batchModuleId, sessionId, isAbsentValid, AbsentRemarks, insertedbatchModuleId, insertedSessionId, @uid from trainee_absence_record "
                        + "where defunct='N' and insertedSessionId=@sid ";

                    if (insertedAbsentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbInsAbs.ToString() + ")";
                        cmd.Parameters.AddRange(lstInsAbsParams.ToArray());
                    }
                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //remove those records that are moved to removed table
                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                    delete from trainee_absence_record where defunct='N' and insertedSessionId=@sid ";

                    if (insertedAbsentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbInsAbs.ToString() + ")";
                        cmd.Parameters.AddRange(lstInsAbsParams.ToArray());
                    }
                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //trainees who attended the session as makeup and who was mark present but later mark absent, move record from removed table back to makeup table
                    //meaning those found in insertedAbsentees
                    cmd.CommandText = "SET IDENTITY_INSERT trainee_absence_record ON; "
                        + "insert into trainee_absence_record (absentId, traineeId, batchModuleId, sessionId, isAbsentValid, AbsentRemarks, insertedbatchModuleId, "
                        + "insertedSessionId, createdBy) select absentId, traineeId, batchModuleId, sessionId, isAbsentValid, AbsentRemarks, insertedbatchModuleId, "
                        + "insertedSessionId, @uid from trainee_absence_removed where insertedSessionId=@sid and defunct='N' ";

                    if (insertedAbsentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId in (" + sbInsAbs.ToString() + ") ";
                        cmd.Parameters.AddRange(lstInsAbsParams.ToArray());
                    }
                    //exclude trainees who has been present currently
                    if (insertedTrainees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbInsTrn.ToString() + ") ";
                        cmd.Parameters.AddRange(lstInsTrnParams.ToArray());
                    }

                    cmd.CommandText += "; SET IDENTITY_INSERT trainee_absence_record OFF; ";
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //delete those moved records
                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from trainee_absence_removed where insertedSessionId=@sid ";
                    if (insertedAbsentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId in (" + sbInsAbs.ToString() + ")";
                        cmd.Parameters.AddRange(lstInsAbsParams.ToArray());
                    }
                    //exclude trainees who has been present currently
                    if (insertedTrainees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbInsTrn.ToString() + ")";
                        cmd.Parameters.AddRange(lstInsTrnParams.ToArray());
                    }
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //remove abs records (both in abs and removed table) for enrolled students who has been marked absent but now marked NOT absent
                    StringBuilder sbAbs = new StringBuilder();
                    List<SqlParameter> lstParams = new List<SqlParameter>();
                    if (absentees.Length > 0)
                    {
                        sbAbs.Append("@t0");
                        lstParams.Add(new SqlParameter("@t0", absentees[0]));

                        for (int i = 1; i < absentees.Length; i++)
                        {
                            sbAbs.Append(",@t" + i);
                            lstParams.Add(new SqlParameter("@t" + i, absentees[i]));
                        }
                    }

                    //for enrolled trainees who was prev marked absent (record would have been inserted in the abs table or removed table if they have completed makeup) 
                    //and now marked present
                    //meaning those NOT in absentees
                    cmd.CommandText = "update trainee_absence_record set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where sessionId=@sid ";
                    if (absentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbAbs.ToString() + ")";
                        cmd.Parameters.AddRange(lstParams.ToArray());
                    }
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update trainee_absence_removed set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where sessionId=@sid ";
                    if (absentees.Length > 0)
                    {
                        cmd.CommandText += "and traineeId not in (" + sbAbs.ToString() + ")";
                        cmd.Parameters.AddRange(lstParams.ToArray());
                    }
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //for enrolled trainees who was absent, insert into abs table
                    string existSQL = "select traineeId from trainee_absence_record where traineeId=@tid and sessionId=@sid and defunct='N' union "
                        + "select traineeId from trainee_absence_removed where traineeId=@tid and sessionId=@sid and defunct='N' ";
                    string newAbsSql = "insert into trainee_absence_record (traineeId, batchModuleId, sessionId, createdBy) values (@tid, @bid, @sid, @uid)";

                    foreach (string t in absentees)
                    {
                        //first check if the trainee abs record is already in the table (happen when the user has already marked attendance and edit again)
                        cmd.Parameters.Clear();
                        cmd.CommandText = existSQL;
                        cmd.Parameters.AddWithValue("@tid", t);
                        cmd.Parameters.AddWithValue("@sid", sessionId);

                        if (cmd.ExecuteScalar() == null)
                        {
                            //when not exist, then insert record
                            cmd.Parameters.Clear();
                            cmd.CommandText = newAbsSql;
                            cmd.Parameters.AddWithValue("@tid", t);
                            cmd.Parameters.AddWithValue("@bid", batchModuleId);
                            cmd.Parameters.AddWithValue("@sid", sessionId);
                            cmd.Parameters.AddWithValue("@uid", userId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    cmd.Parameters.Clear();

                    //update the attendanceMarked to Y
                    cmd.CommandText = "update batchModule_session set attendanceMarked='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where sessionId=@sid";

                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Attendance.cs", "updateAttendance()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable getTraineeModuleAbsentSession(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select sessionId from trainee_absence_record a where a.traineeId=@tid and a.batchModuleId=@bmId and a.defunct='N'";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@bmId", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Attendance.cs", "getTraineeModuleAbsentSession()", ex.Message, -1);

                return null;
            }
        }
    }
}