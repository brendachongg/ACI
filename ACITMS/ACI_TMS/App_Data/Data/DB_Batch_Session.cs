using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;
using GeneralLayer;
using LogicLayer;

namespace DataLayer
{
    public class DB_Batch_Session
    {
        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();

        public string getEnrollmentLetter(int programmeBatchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select enrollmentLetter from programme_batch where programmeBatchId=@id";
                cmd.Parameters.AddWithValue("@id", programmeBatchId);

                return dbConnection.executeScalarString(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getEnrollmentLetter()", ex.Message, -1);

                return null;
            }
        }

        public bool updateEnrollmentLetter(int programmeBatchId, string letter)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update programme_batch set enrollmentLetter=@letter where programmeBatchId=@id";
                cmd.Parameters.AddWithValue("@id", programmeBatchId);
                cmd.Parameters.AddWithValue("@letter", letter);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "updateEnrollmentLetter()", ex.Message, -1);

                return false;
            }
        }

        public int getNoOfAvaBatch()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from programme_batch where defunct='N' and DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) between programmeRegStartDate and programmeRegEndDate";

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getNoOfAvaBatch()", ex.Message, -1);

                return -1;
            }
        }

        //public DateTime getMinBatchStartDate(string bundleCode)
        public DateTime getMinBatchStartDate(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select min(pb.programmeStartDate) as minDate "
                    + "from programme_structure p inner join programme_batch pb on p.programmeId=pb.programmeId and p.bundleId=@bid and pb.defunct='N' ";

                cmd.Parameters.AddWithValue("@bid", bundleId);

                DataTable dt=dbConnection.getDataTable(cmd);
                if (dt.Rows.Count == 0) return DateTime.MinValue;

                if (dt.Rows[0][0] == DBNull.Value) return DateTime.MaxValue;

                return (DateTime)dt.Rows[0][0];
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getMinBatchStartDate()", ex.Message, -1);

                return DateTime.MinValue;
            }
        }

        public int getBatchCapacityBySession(int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchCapacity from batch_module bm inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId "
                    + "inner join batchModule_session s on s.batchModuleId=bm.batchModuleId and s.sessionId=@sid";

                cmd.Parameters.AddWithValue("@sid", sessionId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchCapacity()", ex.Message, -1);

                return -1;
            }
        }

        public int getBatchModuleId(int programmeBatchId, int moduleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchModuleId from batch_module where programmeBatchId=@bid and moduleId=@mid";

                cmd.Parameters.AddWithValue("@bid", programmeBatchId);
                cmd.Parameters.AddWithValue("@mid", moduleId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleId()", ex.Message, -1);

                return -1;
            }
        }

        public int getBatchCapacity(int programmeBatchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchCapacity from programme_batch where programmeBatchId=@bid";

                cmd.Parameters.AddWithValue("@bid", programmeBatchId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchCapacity()", ex.Message, -1);

                return -1;
            }
        }

        public int getBatchCapacityByBatchModule(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchCapacity from batch_module bm inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId and bm.batchModuleId=@bmid";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchCapacityByBatchModule()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getBatchesByModule(int moduleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select pb.programmeBatchId, pb.batchCode, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, p.programmeCode, p.programmeTitle, bm.batchModuleId, bm.startDate as moduleStartDate, "
                    + "convert(nvarchar, bm.startDate, 106) as moduleStartDateDisp, bm.endDate as moduleEndDate, convert(nvarchar, bm.endDate, 106) as moduleEndDateDisp "
                    + "from programme_batch pb inner join programme_structure p on p.programmeId=pb.programmeId and pb.defunct='N' "
                    + "inner join batch_module bm on bm.programmeBatchId=pb.programmeBatchId and bm.defunct='N' and bm.moduleId=@mid "
                    + "order by pb.batchCode";
                cmd.Parameters.AddWithValue("@mid", moduleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchesByModule()", ex.Message, -1);

                return null;
            }
        }

        public bool isBatchCodeExist(string code)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from programme_batch where batchCode=@bc and defunct='N'";
                cmd.Parameters.AddWithValue("@bc", code);

                int n = dbConnection.executeScalarInt(cmd);
                return (n > 0 ? true : false);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "isBatchCodeExist()", ex.Message, -1);

                return true;
            }
        }

        public Tuple<DateTime, DateTime> getMaxMinSessionDates(int id, string extraCondition="", SqlParameter[] p=null)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select max(sessionDate) as maxDt, min(sessionDate) as minDt from batchModule_session where defunct='N' and batchModuleId=@id " + extraCondition;
                cmd.Parameters.AddWithValue("@id", id);
                if (p != null) cmd.Parameters.AddRange(p);

                DataTable dt=dbConnection.getDataTable(cmd);

                if(dt.Rows.Count==0) return null;
                else return new Tuple<DateTime,DateTime>((DateTime)dt.Rows[0]["maxDt"], (DateTime)dt.Rows[0]["minDt"]);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleDates()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchModuleInfo(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select pb.programmeBatchId, bm.moduleId, bm.assessorUserId, m.moduleCode, m.moduleTitle, left(pb.batchCode, len(pb.batchCode)-len(c1.codeValue)) as batchCode, c1.codeValue as batchType, "
                    + "c1.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, pb.programmeRegEndDate, "
                    + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c2.codeValueDisplay as classModeDisp, pb.programmeId, p.programmeCode, p.programmeVersion, "
                    + "p.programmeLevel, c3.codeValueDisplay as programmeLevelDisp, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeTitle, p.programmeType, c5.codeValueDisplay as programmeTypeDisp "

                    + "from batch_module bm inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId and bm.batchModuleId=@bmid "
                    + "inner join module_structure m on bm.moduleId=m.moduleId "

                    + "inner join code_reference c1 on c1.codeValue=right(pb.batchCode, 5) and c1.codeType='CLTYPE' "
                    + "inner join code_reference c2 on c2.codeValue=pb.classMode and c2.codeType='CLMODE' inner join programme_structure p on p.programmeId=pb.programmeId "
                    + "inner join code_reference c3 on c3.codeValue=p.programmeLevel and c3.codeType='PGLVL' inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' "
                    + "inner join code_reference c5 on c5.codeValue=p.programmeType and c5.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchAllModulesInfo(int batchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select b.batchModuleId, b.moduleId, m.moduleCode, m.moduleTitle, b.day, b.startDate, convert(nvarchar, b.startDate, 106) as startDateDisp, b.endDate, convert(nvarchar, b.endDate, 106) as endDateDisp, "
                    + "b.trainerUserId1, u1.userName as trainerUserName1, b.trainerUserId2, u2.userName as trainerUserName2, b.assessorUserId, u3.userName as assessorUserName, count(*) as ModuleNumOfSession "
                    + "from batch_module b inner join batchModule_session s on b.batchModuleId=s.batchModuleId and s.defunct='N' and b.defunct='N' and b.programmeBatchId=@bid "
                    + "inner join module_structure m on m.moduleId=b.moduleId left outer join aci_user u1 on b.trainerUserId1=u1.userId left outer join aci_user u2 on b.trainerUserId2=u2.userId "
                    + "left outer join aci_user u3 on b.assessorUserId=u3.userId "
                    + "group by b.batchModuleId, b.moduleId, m.moduleCode, m.moduleTitle, b.day, b.startDate, b.endDate, b.trainerUserId1, u1.userName, b.trainerUserId2, u2.userName, b.assessorUserId, u3.userName "
                    + "order by b.startDate, b.endDate";

                cmd.Parameters.AddWithValue("@bid", batchId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchAllModulesInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchModuleInfo(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchModuleId, programmeBatchId, moduleId, startDate, convert(nvarchar, startDate, 106) as startDateDisp, "
                    + "endDate, convert(nvarchar, endDate, 106) as endDateDisp, Day from batch_module "
                    + "where " + condition;
                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleId()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchModuleSessions(int batchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select b.batchModuleId, b.moduleId, s.sessionId, s.sessionDate, s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, "
                    + "convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.venueId, v.venueLocation "
                    + "from batchModule_session s inner join venue v on s.venueId=v.venueID and s.defunct='N'  "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + "inner join batch_module b on b.batchModuleId=s.batchModuleId "
                    + "where b.programmeBatchId=@bid order by b.batchModuleId, s.sessionDate, c.codeOrder";

                cmd.Parameters.AddWithValue("@bid", batchId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleSessions()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchModuleSessions(int programmeBatchId, int moduleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select ROW_NUMBER() OVER(ORDER BY s.sessionDate, c.codeOrder) as sessionNumber, s.sessionId, s.sessionDate, s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, "
                    + "convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.venueId, v.venueLocation, s.batchModuleId "
                    + "from batchModule_session s inner join venue v on s.venueId=v.venueID and s.defunct='N'  "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + "inner join batch_module b on b.batchModuleId=s.batchModuleId and b.programmeBatchId=@pid and b.moduleId=@mid ";

                cmd.Parameters.AddWithValue("@pid", programmeBatchId);
                cmd.Parameters.AddWithValue("@mid", moduleId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleSessions()", ex.Message, -1);

                return null;
            }
        }
        public DataTable searchBatches(string condition, SqlParameter [] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select b.programmeBatchId, b.projectCode, b.batchCode, b.programmeRegStartDate, convert(nvarchar, b.programmeRegStartDate, 106) as programmeRegStartDateDisp, 
                    b.programmeRegEndDate, convert(nvarchar, b.programmeRegEndDate, 106) as programmeRegEndDateDisp, b.programmeStartDate, 
                    convert(nvarchar, b.programmeStartDate, 106) as programmeStartDateDisp, b.programmeCompletionDate, 
                    convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDateDisp, b.batchCapacity, b.classMode, c.codeValueDisplay as classModeDisp, p.programmeCode, 
                    p.programmeTitle, isnull(t.cnt, 0) as enrolledCnt, isnull(a.cnt, 0) as appliedCnt
                    from programme_batch b inner join code_reference c on b.classMode=c.codeValue and c.codeType='CLMODE' 
                    inner join programme_structure p on p.programmeId=b.programmeId  
                    left outer join (
	                    select count(*) as cnt, programmebatchId from trainee_programme where traineeStatus in ('E', 'C') group by programmeBatchId
                    ) t on b.programmeBatchId=t.programmeBatchId
                    left outer join (
	                    select count(*) as cnt, programmeBatchId from applicant where applicationStatus <> 'WD' and rejectstatus = 'N' group by programmeBatchId
                    ) a on a.programmeBatchId=b.programmeBatchId
                    where b.defunct='N' ";

                if (condition != null && condition != "")
                {
                    cmd.CommandText += "and " + condition;
                    if (p != null)
                    {
                        foreach (SqlParameter p1 in p)
                        {
                            cmd.Parameters.Add(p1);
                        }
                    }
                }

                //cmd.CommandText += " order by b.lastModifiedDate desc, b.createdOn desc";

                cmd.CommandText += " order by b.programmeStartDate asc";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "searchBatches()", ex.Message, -1);

                return null;
            }
        }

//        public DataTable searchBatches(string condition, SqlParameter p)
//        {
//            try
//            {
//                SqlCommand cmd = new SqlCommand();
//                cmd.CommandText = @"select b.programmeBatchId, b.projectCode, b.batchCode, b.programmeRegStartDate, convert(nvarchar, b.programmeRegStartDate, 106) as programmeRegStartDateDisp, 
//                    b.programmeRegEndDate, convert(nvarchar, b.programmeRegEndDate, 106) as programmeRegEndDateDisp, b.programmeStartDate, 
//                    convert(nvarchar, b.programmeStartDate, 106) as programmeStartDateDisp, b.programmeCompletionDate, 
//                    convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDateDisp, b.batchCapacity, b.classMode, c.codeValueDisplay as classModeDisp, p.programmeCode, 
//                    p.programmeTitle, isnull(t.cnt, 0) as enrolledCnt, isnull(a.cnt, 0) as appliedCnt
//                    from programme_batch b inner join code_reference c on b.classMode=c.codeValue and c.codeType='CLMODE' 
//                    inner join programme_structure p on p.programmeId=b.programmeId  
//                    left outer join (
//	                    select count(*) as cnt, programmebatchId from trainee_programme where traineeStatus in ('E', 'C') group by programmeBatchId
//                    ) t on b.programmeBatchId=t.programmeBatchId
//                    left outer join (
//	                    select count(*) as cnt, programmeBatchId from applicant where applicationStatus <> 'WD' and rejectstatus = 'N' group by programmeBatchId
//                    ) a on a.programmeBatchId=b.programmeBatchId
//                    where b.defunct='N' ";

//                if (condition != null && condition != "")
//                {
//                    cmd.CommandText += "and " + condition;
//                    if (p != null) cmd.Parameters.Add(p);
//                }

//                cmd.CommandText += " order by b.lastModifiedDate desc, b.createdOn desc";

//                return dbConnection.getDataTable(cmd);
//            }
//            catch (Exception ex)
//            {
//                Log_Handler lh = new Log_Handler();
//                lh.WriteLog(ex, "DB_Batch_Session.cs", "searchBatches()", ex.Message, -1);

//                return null;
//            }
//        }

        public DataTable getClsMode()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='CLMODE' order by codeOrder";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getClsMode()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getClsType()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='CLTYPE' order by codeOrder";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchType()", ex.Message, -1);

                return null;
            }
        }

        public bool createBatchNSession(BatchDetails batch, DataTable dtMod, DataTable dtSession, int userId)
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
                    //create batch info
                    cmd.CommandText = "insert into programme_batch(programmeId, projectCode, batchCode, programmeRegStartDate, programmeRegEndDate, programmeStartDate, programmeCompletionDate, batchCapacity, classMode, enrollmentLetter, createdBy) "
                        + "values(@pid, @pjCode, @bCode, @pStart, @pEnd, @cStart, @cEnd, @cap, @cls, (select enrollmentTemplate from programme_structure where programmeId=@pid), @uid); SELECT CAST(scope_identity() AS int);";
                    cmd.Parameters.AddWithValue("@pid", batch.progId);
                    cmd.Parameters.AddWithValue("@pjCode", batch.projCode);
                    cmd.Parameters.AddWithValue("@bCode", batch.batchCode + batch.clsTypeCode);
                    cmd.Parameters.AddWithValue("@pStart", batch.regStartDate);
                    cmd.Parameters.AddWithValue("@pEnd", batch.regEndDate);
                    cmd.Parameters.AddWithValue("@cStart", batch.batchStartDate);
                    cmd.Parameters.AddWithValue("@cEnd", batch.batchEndDate);
                    cmd.Parameters.AddWithValue("@cap", batch.batchCapacity);
                    cmd.Parameters.AddWithValue("@cls", batch.batchModeCode);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    int batchId = (int)cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                    //create batch modules
                    dtSession.Columns.Add(new DataColumn("batchModuleId", typeof(int)));
                    cmd.CommandText = "insert into batch_module(programmeBatchId, moduleId, day, startDate, endDate, trainerUserId1, trainerUserId2, assessorUserId, createdBy) "
                        + "values (@pid, @mid, @d, @sd, @ed, @t1, @t2, @a, @uid); SELECT CAST(scope_identity() AS int);";
                    int batchModuleId;
                    foreach (DataRow dr in dtMod.Rows)
                    {
                        cmd.Parameters.AddWithValue("@pid", batchId);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                        cmd.Parameters.AddWithValue("@d", dr["day"]);
                        cmd.Parameters.AddWithValue("@sd", dr["startDate"]);
                        cmd.Parameters.AddWithValue("@ed", dr["endDate"]);
                        cmd.Parameters.AddWithValue("@t1", dr["trainerUserId1"]);
                        cmd.Parameters.AddWithValue("@t2", dr["trainerUserId2"]);
                        cmd.Parameters.AddWithValue("@a", dr["assessorUserId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        batchModuleId = (int)cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        //update session records with the id
                        foreach (DataRow d in dtSession.Select("moduleId=" + dr["moduleId"].ToString()))
                            d["batchModuleId"] = batchModuleId;
                    }

                    //create module sessions & venue bookings
                    string sessionSql = "insert into batchModule_session(batchModuleId, sessionDate, sessionPeriod, venueId, createdBy) "
                        + "values (@bid, @dt, @pt, @vid, @uid); SELECT CAST(scope_identity() AS int);";
                    string venueSql = "insert into venue_booking_record(bookingDate, bookingPeriod, venueId, sessionId, createdBy) values (@dt, @pt, @vid, @sid, @uid)";
                    int sessionId;

                    foreach (DataRow dr in dtSession.Rows)
                    {
                        cmd.CommandText = sessionSql;
                        cmd.Parameters.AddWithValue("@bid", dr["batchModuleId"]);
                        cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                        cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                        cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        sessionId = (int)cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        cmd.CommandText = venueSql;
                        cmd.Parameters.AddWithValue("@sid", sessionId);
                        cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                        cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                        cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "createBatchNSession()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable getSessionDetails(int sessionId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.sessionId, s.batchModuleId, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.sessionPeriod, "
                    + "sc1.codeValueDisplay as sessionPeriodDisp, s.venueId, v.venueLocation, "
                    + "bm.moduleId, bm.programmeBatchId, bm.day, bm.startDate, convert(nvarchar, bm.startDate, 106) as startDateDisp, "
                    + "bm.endDate, convert(nvarchar, bm.endDate, 106) as endDateDisp, bm.trainerUserId1, u1.userName as trainerUserName1, "
                    + "bm.trainerUserId2, u2.userName as trainerUserName2, bm.assessorUserId, u3.userName as assessorUserName, "
                    + "m.moduleCode, m.moduleLevel, m.moduleTitle, m.moduleEffectDate, convert(nvarchar, m.moduleEffectDate, 106) as moduleEffectDateDisp, "
                    + "m.moduleTrainingHour, left(b.batchCode, len(b.batchCode)-len(bc2.codeValue)) as batchCode, bc2.codeValue as batchType, bc2.codeValueDisplay as batchTypeDisp, "
                    + "b.programmeRegStartDate, convert(nvarchar, b.programmeRegStartDate, 106) as programmeRegStartDateDisp, b.programmeRegEndDate, convert(nvarchar, b.programmeRegEndDate, 106) as programmeRegEndDateDisp, "
                    + "b.projectCode, b.programmeStartDate, convert(nvarchar, b.programmeStartDate, 106) as programmeStartDateDisp, b.programmeCompletionDate, "
                    + "convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDateDisp, b.batchCapacity, b.programmeId, b.classMode, bc1.codeValueDisplay as classModeDisp, "
                    + "p.programmeCode, p.programmeCategory, pc1.codeValueDisplay as programmeCategoryDisp, p.programmeLevel, pc2.codeValueDisplay as programmeLevelDisp, "
                    + "p.programmeTitle, p.programmeVersion, p.programmeType, pc3.codeValueDisplay as programmeTypeDisp, p.bundleId, pb.bundleCode, pbm.ModuleNumOfSession "

                    + "from batchModule_session s inner join venue v on s.defunct='N' and s.sessionId=@id and s.venueId=v.venueId "
                    + "inner join code_reference sc1 on sc1.codeValue=s.sessionPeriod and sc1.codeType='PERIOD' "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join module_structure m on m.moduleId=bm.moduleId "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId inner join programme_structure p on p.programmeId=b.programmeId "
                    + "inner join code_reference pc1 on pc1.codeValue=p.programmeCategory and pc1.codeType='PGCAT' inner join code_reference pc2 on pc2.codeValue=p.programmeLevel and pc2.codeType='PGLVL' "
                    + "inner join code_reference pc3 on pc3.codeValue=p.programmeType and pc3.codeType='PGTYPE' inner join code_reference bc1 on bc1.codeValue=b.classMode and bc1.codeType='CLMODE' "
                    //+ "inner join bundle pb on pb.bundleCode=p.bundleCode and pb.moduleId=bm.moduleId "
                    + "inner join bundle pb on pb.bundleId=p.bundleId inner join bundle_module pbm on pbm.bundleId=pb.bundleId and pbm.moduleId=bm.moduleId and pbm.defunct='N' "

                    + "inner join code_reference bc2 on bc2.codeValue=right(b.batchCode, 5) and bc2.codeType='CLTYPE' "
                    + "left outer join aci_user u1 on u1.userId=bm.trainerUserId1  left outer join aci_user u3 on u3.userId=bm.assessorUserId  left outer join aci_user u2 on u2.userId=bm.trainerUserId2";

                cmd.Parameters.AddWithValue("@id", sessionId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getSessionDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchDetails(int batchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select b.programmeId, p.programmeTitle, p.programmeCategory, pc1.codeValueDisplay as programmeCategoryDisp, p.programmeLevel, pc2.codeValueDisplay as programmeLevelDisp, p.programmeVersion, "
                    + "p.programmeCode, p.programmeType, pc3.codeValueDisplay as programmeTypeDisp, b.projectCode, b.batchCode as fullBatchCode, bc2.codeValue as batchType, bc2.codeValueDisplay as batchTypeDisp, "
                    + "left(b.batchCode, len(b.batchCode)-len(bc2.codeValue)) as batchCode, b.projectCode, b.programmeRegStartDate, convert(nvarchar, b.programmeRegStartDate, 106) as programmeRegStartDateDisp, b.programmeRegEndDate, "
                    + "convert(nvarchar, b.programmeRegEndDate, 106) as programmeRegEndDateDisp, b.programmeStartDate, convert(nvarchar, b.programmeStartDate, 106) as programmeStartDateDisp, b.programmeCompletionDate, "
                    + "convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDateDisp, b.batchCapacity, b.classMode, bc1.codeValueDisplay as classModeDisp, b.enrollmentLetter "
                    + "from programme_batch b inner join programme_structure p on b.programmeId=p.programmeId inner join code_reference pc1 on pc1.codeValue=p.programmeCategory and pc1.codeType='PGCAT' "
                    + "inner join code_reference pc2 on pc2.codeValue=p.programmeLevel and pc2.codeType='PGLVL' inner join code_reference pc3 on pc3.codeValue=p.programmeType and pc3.codeType='PGTYPE' "
                    + "inner join code_reference bc1 on bc1.codeValue=b.classMode and bc1.codeType='CLMODE' "
                    + "inner join code_reference bc2 on bc2.codeValue=RIGHT(b.batchCode, 5) and bc2.codeType='CLTYPE' "
                    + "where b.programmeBatchId=@id";

                cmd.Parameters.AddWithValue("@id", batchId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchType()", ex.Message, -1);

                return null;
            }
        }

        public bool updateSession(int sessionId, DateTime dt, DayPeriod period, string venueId, int userId)
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
                    cmd.CommandText = "update venue_booking_record set bookingDate=@dt, bookingPeriod=@pt, venueId=@vid, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where sessionId=@sid and defunct='N' and (bookingDate<>@dt or bookingPeriod<>@pt or venueId<>@vid)";
                    cmd.Parameters.AddWithValue("@dt", dt);
                    cmd.Parameters.AddWithValue("@pt", period.ToString());
                    cmd.Parameters.AddWithValue("@vid", venueId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update batchModule_session set sessionDate=@dt, sessionPeriod=@pt, venueId=@vid, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where sessionId=@sid and (sessionDate<>@dt or sessionPeriod<>@pt or venueId<>@vid)";
                    cmd.Parameters.AddWithValue("@dt", dt);
                    cmd.Parameters.AddWithValue("@pt", period.ToString());
                    cmd.Parameters.AddWithValue("@vid", venueId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@sid", sessionId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "deleteBatch()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool deleteBatch(int batchId, int userId)
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
                    //update all venue booking records
                    cmd.CommandText = "update venue_booking_record set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where sessionId in ("
                        + "select s.sessionId from batchModule_session s inner join batch_module b on s.batchModuleId=b.batchModuleId and b.programmeBatchId=@id and s.defunct='N')";
                    cmd.Parameters.AddWithValue("@id", batchId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //update sessions
                    cmd.CommandText = "update batchModule_session set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where batchModuleId in ("
                        + "select batchModuleId from batch_module where programmeBatchId=@id and defunct='N')";
                    cmd.Parameters.AddWithValue("@id", batchId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //update modules
                    cmd.CommandText = "update batch_module set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where programmeBatchId=@id and defunct='N'";
                    cmd.Parameters.AddWithValue("@id", batchId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //update batch
                    cmd.CommandText = "update programme_batch set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where programmeBatchId=@id";
                    cmd.Parameters.AddWithValue("@id", batchId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "deleteBatch()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool updateBatch(int batchId, string batchCode, string projCode, DateTime dtRegStart, DateTime dtRegEnd, DateTime dtBatchStart, DateTime dtBatchEnd,
            int capacity, string mode, DataTable dtMod, DataTable dtSession, int userId)
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
                    //update batch
                    cmd.CommandText = "update programme_batch set batchcode=Concat(@batchCode, RIGHT(batchCode, 5)), projectCode=@pjCode, programmeRegStartDate=@regStart, programmeRegEndDate=@regEnd, programmeStartDate=@start, "
                        + "programmeCompletionDate=@end, batchCapacity=@cap, classMode=@mode, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where programmeBatchId=@id and (projectCode<>@pjCode or programmeRegStartDate<>@regStart or programmeRegEndDate<>@regEnd "
                        + "or programmeStartDate<>@start or programmeCompletionDate<>@end or batchCapacity<>@cap or classMode<>@mode or batchCode <> @batchCode)";
                    cmd.Parameters.AddWithValue("@id", batchId);
                    cmd.Parameters.AddWithValue("@pjCode", projCode);
                    cmd.Parameters.AddWithValue("@regStart", dtRegStart);
                    cmd.Parameters.AddWithValue("@regEnd", dtRegEnd);
                    cmd.Parameters.AddWithValue("@start", dtBatchStart);
                    cmd.Parameters.AddWithValue("@end", dtBatchEnd);
                    cmd.Parameters.AddWithValue("@cap", capacity);
                    cmd.Parameters.AddWithValue("@mode", mode);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@batchCode", batchCode);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();


                    //update batch modules
                    cmd.CommandText = "update batch_module set trainerUserId1=@tuid1, trainerUserId2=@tuid2, assessorUserId=@taid, startDate=@sdt, endDate=@edt, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where programmeBatchId=@id and moduleId=@mid and ((trainerUserId1 is null and @tuid1 is not null) or (trainerUserId1 is not null and @tuid1 is null) or trainerUserId1<>@tuid1 "
                        + "or (assessorUserId is null and @taid is not null) or (assessorUserId is not null and @taid is null) or assessorUserId<>@taid "
                        + "or (trainerUserId2 is null and @tuid2 is not null) or (trainerUserId2 is not null and @tuid2 is null) or trainerUserId2<>@tuid2 or startDate<>@sdt or endDate<@edt)";

                    foreach (DataRow dr in dtMod.Rows)
                    {
                        cmd.Parameters.AddWithValue("@id", batchId);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                        cmd.Parameters.AddWithValue("@tuid1", dr["trainerUserId1"]);
                        cmd.Parameters.AddWithValue("@tuid2", dr["trainerUserId2"]);
                        cmd.Parameters.AddWithValue("@taid", dr["assessorUserId"]);
                        cmd.Parameters.AddWithValue("@sdt", dr["startDate"]);
                        cmd.Parameters.AddWithValue("@edt", dr["endDate"]);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    //update sessions
                    string sessionSQL = "update batchModule_session set sessionDate=@dt, sessionPeriod=@pt, venueId=@vid, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where sessionId=@sid and (sessionDate<>@dt or sessionPeriod<>@pt or venueId<>@vid)";
                    string bookingSQL = "update venue_booking_record set bookingDate=@dt, bookingPeriod=@pt, venueId=@vid, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where sessionId=@sid and defunct='N' and (bookingDate<>@dt or bookingPeriod<>@pt or venueId<>@vid)";

                    foreach (DataRow dr in dtSession.Rows)
                    {
                        cmd.CommandText = sessionSQL;
                        cmd.Parameters.AddWithValue("@sid", dr["sessionId"]);
                        cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                        cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                        cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        cmd.CommandText = bookingSQL;
                        cmd.Parameters.AddWithValue("@sid", dr["sessionId"]);
                        cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                        cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                        cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "updateBatch()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable searchSessions(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.sessionId, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, "
                    + "s.venueId, v.venueLocation, b.batchCode, b.projectCode, p.programmeCode, p.programmeTitle, mm.moduleCode, mm.moduleTitle "
                    + "from batchModule_session s inner join batch_module m on m.batchModuleId=s.batchModuleId and s.defunct='N' and m.defunct='N' "
                    + "inner join programme_batch b on b.programmeBatchId=m.programmeBatchId and b.defunct='N' inner join programme_structure p on p.programmeId=b.programmeId "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' inner join venue v on s.venueID=v.venueID "
                    + "inner join module_structure mm on mm.moduleId=m.moduleId where " + condition
                    + " order by b.batchCode, mm.moduleCode, s.sessionDate, c.codeOrder";

                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "searchSessions()", ex.Message, -1);

                return null;
            }
        }

        //Batches that are open for registration and enrollment
        public DataTable getAllBatchForRegistration()
        {
            try
            {
                string sqlStatement = @"SELECT * FROM programme_batch as pb
                                        LEFT OUTER JOIN
                                        programme_structure as ps
                                        ON pb.programmeId = ps.programmeId
                                        WHERE pb.programmeStartDate > DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) and pb.defunct = 'N'";
                //WHERE pb.programmeRegStartDate <= getDate() AND pb.programmeRegEndDate >= getDate()";
                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtCourseBatch = dbConnection.getDataTable(cmd);
                return dtCourseBatch;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getAllBatchForRegistration()", ex.Message, -1);
                return null;
            }
        }

        public DataTable getProgrammeBatchByProgrammeBatchId(string programmeBatchId)
        {
            string sql = @"SELECT pb.defunct as isDefunct, CONCAT(programmeTitle, ' (', projectCode, ' )') as ProgrammeCode, * FROM programme_batch as pb
                           LEFT OUTER JOIN programme_structure as ps ON pb.programmeId = ps.programmeId where programmeBatchId = @programmeBatchId";

//            string sql = @"SELECT CONCAT(programmeTitle, ' (', projectCode, ' )') as ProgrammeCode, * FROM programme_batch as pb
//
//                                        LEFT OUTER JOIN
//
//                                        programme_structure as ps
//
//                                        ON pb.programmeId = ps.programmeId where pb.programmeStartDate > getdate() and pb.defunct = 'N' and programmeBatchId = @programmeBatchId";

            SqlCommand cmd = new SqlCommand(sql);
            cmd.Parameters.AddWithValue("@programmeBatchId", programmeBatchId);
            DataTable dtCourseBatch = dbConnection.getDataTable(cmd);
            return dtCourseBatch;
        }

        //Get all batch regardless of defuct
        public DataTable getAllBatchForDisplay()
        {
            try
            {
                string sqlStatement = @"SELECT CONCAT(programmeTitle, ' (', projectCode, ' )') as ProgrammeCode,  * FROM programme_batch as pb

                                        LEFT OUTER JOIN

                                        programme_structure as ps

                                        ON pb.programmeId = ps.programmeId where pb.programmeStartDate > DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) and pb.defunct = 'N'";

                //WHERE pb.programmeStartDate > getdate()";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtCourseBatch = dbConnection.getDataTable(cmd);

                return dtCourseBatch;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getAllBatchForDisplay()", ex.Message, -1);

                return null;
            }
        }

        //Get all batch records by selected category (In use)
        public DataTable getBatchById(string programmeBatchId)
        {
            try
            {
                string sqlStatement = @"Select pb.*, ps.*, b.*, bm.*, m.* FROM

                                    programme_batch as pb

                                    LEFT OUTER JOIN

                                    programme_structure as ps

                                    ON pb.programmeId = ps.programmeId

                                    LEFT OUTER JOIN

                                    bundle as b

                                    ON ps.bundleId = b.bundleId

                                    left outer join

                                    bundle_module bm

                                    on bm.bundleId=b.bundleId and bm.defunct='N'

                                    LEFT OUTER JOIN

                                    module_structure as m

                                    ON bm.moduleId = m.moduleId

                                    WHERE pb.programmeBatchId = @programmeBatchId";



                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeBatchId", programmeBatchId);
                DataTable dtCourseBatch = dbConnection.getDataTable(cmd);

                return dtCourseBatch;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchById()", ex.Message, -1);

                return null;
            }

        }

        public DataTable getBatchModuleByProgrammeBatchId(string programmeBatchId)
        {
            try
            {
                string sqlStatement = @"SELECT pb.programmeId, ps.programmeTitle, b.bundleCode, bm.moduleId, 

                                    ms.WSQCompetencyCode, ms.moduleTitle, ms.moduleCredit, ms.moduleCost
                                    
                                    FROM programme_batch as pb 

                                    LEFT OUTER JOIN programme_structure as ps

                                    ON pb.programmeId = ps.programmeId

                                    LEFT OUTER JOIN  bundle as b

                                    ON ps.bundleId  = b.bundleId

                                    left outer join bundle_module bm

                                    on bm.bundleId=b.bundleId and bm.defunct='N'

                                    LEFT OUTER JOIN  module_structure as ms

                                    ON bm.moduleId = ms.moduleId

                                    WHERE pb.programmeBatchId = @programmeBatchId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeBatchId", programmeBatchId);
                DataTable dtBatchModule = dbConnection.getDataTable(cmd);

                return dtBatchModule;

            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Batch_Session.cs", "getBatchModuleByProgrammeBatchId()", ex.Message, -1);

                return null;
            }
        }
    }
}
