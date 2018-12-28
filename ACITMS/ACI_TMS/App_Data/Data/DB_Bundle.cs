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

namespace DataLayer
{
    public class DB_Bundle
    {
        //Initialize connection string
        private Database_Connection dbConnection = new Database_Connection();

        public int getBundleModuleNoOfSessions(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select bbm.ModuleNumOfSession from batch_module bm inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId and bm.batchModuleId=@bmid inner join programme_structure p on p.programmeId=pb.programmeId "
                    + "inner join Bundle b on b.bundleId=p.bundleId inner join bundle_module bbm on bbm.bundleId=b.bundleId and bbm.moduleId=bm.moduleId and bbm.defunct='N'";
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundleModuleNoOfSessions()", ex.Message, -1);

                return -1;
            }
        }

        public bool checkBundleCodeExist(string bundleCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from bundle where UPPER(bundleCode)=@bc";
                cmd.Parameters.AddWithValue("@bc", bundleCode.ToUpper());

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "checkBundleCodeExist()", ex.Message, -1);

                return false;
            }
        }

        public bool createBundle(string bundleCode, string bundleType, DateTime bundleEffDate, decimal totalCost, DataTable dtModules, string userId)
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
                    cmd.CommandText = "insert into bundle (bundleCode, bundleType, bundleEffectDate, bundleCost, createdBy) values (@bc, @bt, @bdt, @tc, @usr); SELECT CAST(scope_identity() AS int);";
                    cmd.Parameters.AddWithValue("@bc", bundleCode);
                    cmd.Parameters.AddWithValue("@bt", bundleType);
                    cmd.Parameters.AddWithValue("@bdt", bundleEffDate);
                    cmd.Parameters.AddWithValue("@tc", totalCost);
                    cmd.Parameters.AddWithValue("@usr", userId);

                    int bundleId = (int)cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "insert into bundle_module (bundleId, moduleId, moduleOrder, ModuleNumOfSession,createdBy) values (@bid, @mid, @cnt, @ns, @usr)";

                    int cnt = 1;
                    foreach (DataRow dr in dtModules.Rows)
                    {
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                        cmd.Parameters.AddWithValue("@cnt", cnt);
                        cmd.Parameters.AddWithValue("@ns", dr["ModuleNumOfSession"]);
                        cmd.Parameters.AddWithValue("@usr", userId);
                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                        cnt++;
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "createBundle()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        //Get all package
        public DataTable getAllBundles()
        {
            try
            {
                string sqlStatement = @"SELECT b.bundleId, b.bundleCode, count(*) as noOfModules

                                        FROM Bundle b inner join bundle_module bm on b.bundleId=bm.bundleId and b.defunct='N' and bm.defunct='N' group by b.bundleId, b.bundleCode order by b.bundleCode ";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                
                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getAllBundle()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchBundle(string searchKey, string searchValue)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select b.bundleId, b.bundleCode, count(*) as NoOfModules from Bundle b inner join bundle_module bm on b.bundleId=bm.bundleId and bm.defunct='N' where b.defunct='N' and ";

                if (searchKey == "BC") cmd.CommandText += "UPPER(bundleCode) like @sv ";
                if (searchKey == "MC") cmd.CommandText += "moduleId in (select moduleId from module_structure where UPPER(moduleCode) like @sv) ";
                if (searchKey == "MT") cmd.CommandText += "moduleId in (select moduleId from module_structure where UPPER(moduleTitle) like @sv) ";

                cmd.CommandText += "group by b.bundleId, b.bundleCode order by b.bundleCode";

                cmd.Parameters.AddWithValue("@sv", "%" + searchValue.ToUpper() + "%");

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "searchBundle()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBundleTypes()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='BNTYPE'";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundleTypes()", ex.Message, -1);

                return null;
            }
        }

        //public DataTable getBundle(string bundleCode)
        public DataTable getBundle(int bundleId)
        {
            try
            {
                string sqlStatement = @"SELECT b.bundleCode, c.codeValueDisplay as bundleType, b.bundleType as bundleTypeCode, 
                                      convert(nvarchar, bundleEffectDate, 106) as bundleEffectDate, b.defunct, b.bundleCost
                                      FROM bundle b inner join code_reference c on b.bundleType=c.codeValue and c.codeType='BNTYPE' WHERE b.bundleId = @bid";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@bid", bundleId);

                return dbConnection.getDataTable(cmd); ;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundle()", ex.Message, -1);

                return null;
            }
        }

        //public DataTable getBundleModules(string bundleCode, bool includeDefunct=false)
        public DataTable getBundleModules(int bundleId, bool includeDefunct = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"SELECT bm.bundleModuleId, bm.moduleId, bm.ModuleNumOfSession, m.moduleCode, m.moduleVersion, moduleLevel, m.moduleTitle, convert(nvarchar, m.moduleEffectDate, 106) as moduleEffectDate,
                                        m.moduleTrainingHour, m.moduleCost from bundle b inner join bundle_module bm on b.bundleId=bm.bundleId and bm.defunct='N' inner join module_structure m on bm.moduleId=m.moduleId                                  
                                        WHERE b.bundleId = @bid " + (includeDefunct ? "" : " and b.defunct='N'") + " order by bm.moduleOrder";

                cmd.Parameters.AddWithValue("@bid", bundleId);
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundleModules()", ex.Message, -1);

                return null;
            }
        }

        //public DataTable getBundleProgrammes(string bundleCode)
        public DataTable getBundleProgrammes(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select distinct p.programmeId, p.programmeCode, p.programmeTitle 
                    from programme_structure p inner join programme_batch b on  b.programmeId=p.programmeId
                    and p.bundleId=@bid order by p.programmeCode, p.programmeTitle";

                cmd.Parameters.AddWithValue("@bid", bundleId);
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundleProgrammes()", ex.Message, -1);

                return null;
            }
        }

        //public DataTable getBundleProgrammeBatches(string bundleCode)
        public DataTable getBundleProgrammeBatches(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select b.programmeBatchId, b.projectCode, b.batchCode, p.programmeCode, b.programmeId, 
                    convert(nvarchar, b.programmeRegStartDate, 106) as programmeRegStartDate, 
                    convert(nvarchar, b.programmeRegEndDate, 106) as programmeRegEndDate, convert(nvarchar, b.programmeStartDate, 106) as programmeStartDate, 
                    convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDate, b.batchCapacity, b.classMode as classModeCode, c.codeValueDisplay as classMode 
                    from programme_batch b inner join code_reference c on b.classMode=c.codeValue and c.codeType='CLMODE' 
                    inner join programme_structure p on b.programmeId=p.programmeId and p.bundleId=@bid 
                    order by b.batchCode";

                cmd.Parameters.AddWithValue("@bid", bundleId);
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getBundleProgrammeBatches()", ex.Message, -1);

                return null;
            }
        }

        //public bool delBundle(string bundleCode, string usrId)
        public bool delBundle(int bundleId, int usrId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update bundle set defunct='Y', lastModifiedBy=@usr, lastModifiedDate=getdate() where bundleId=@bid";

                cmd.Parameters.AddWithValue("@usr", usrId);
                cmd.Parameters.AddWithValue("@bid", bundleId);

                return dbConnection.executeNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "delBundle()", ex.Message, -1);

                return false;
            }
        }

        //public int getMaxProgrammesSOA(string bundleCode)
        public int getMaxProgrammesSOA(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select max(numberOfSOA) from programme_structure p where p.bundleId=@bid and 
                    (p.defunct='N' or exists(select 1 from programme_batch b where b.programmeId=p.programmeId and b.defunct='N'))";

                cmd.Parameters.AddWithValue("@bid", bundleId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getMaxProgrammesSOA()", ex.Message, -1);

                return -1;
            }
        }

        //public bool checkProgrammeExist(string bundleCode)
        public bool checkProgrammeExist(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from programme_structure p where bundleId=@bid and defunct='N'";
                cmd.Parameters.AddWithValue("@bid", bundleId);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "checkProgrammeExist()", ex.Message, -1);

                return false;
            }
        }

        //public bool checkSessionExist(string bundleCode)
        public bool checkSessionExist(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from batchModule_session s inner join batch_module m on s.batchModuleId=m.batchModuleId and s.defunct='N' "
                    + "inner join programme_batch b on m.programmeBatchId=b.programmeBatchId and b.defunct='N' "
                    + "inner join programme_structure p on p.programmeId=b.programmeId "
                    + "where p.bundleId=@bid";
                cmd.Parameters.AddWithValue("@bid", bundleId);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "checkSessionExist()", ex.Message, -1);

                return false;
            }
        }

        //public bool checkBatchStarted(string bundleCode)
        public bool checkBatchStarted(int bundleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from batchModule_session s inner join batch_module m on s.batchModuleId=m.batchModuleId "
                    + "inner join programme_batch b on m.programmeBatchId=b.programmeBatchId "
                    + "inner join programme_structure p on p.programmeId=b.programmeId "
                    + "where p.bundleId=@bid and b.programmeStartDate < DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)";
                cmd.Parameters.AddWithValue("@bid", bundleId);

                return (dbConnection.executeScalarInt(cmd) > 0 ? true : false);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "checkSessionExist()", ex.Message, -1);

                return false;
            }
        }

        public DataTable checkDuplicatedSession(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select convert(nvarchar, s.sessionDate, 106) as sessionDate, c.codeValueDisplay as sessionPeriod, m.moduleCode, p.programmeCode, pb.batchCode "
                    + "from batch_module b inner join batchModule_session s on b.batchModuleId=s.batchModuleId "
                    + "inner join programme_batch pb on pb.programmeBatchId=b.programmeBatchId "
                    + "inner join programme_structure p on p.programmeId=pb.programmeId "
                    + "inner join module_structure m on m.moduleId=b.moduleId "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + "where " + condition;
                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "checkSessionExist()", ex.Message, -1);

                return null;
            }
        }

        //public bool updateBundle(string bundleCode, string bundleType, DateTime bundleEffDt, decimal bundleCost, int userId,
        //    DataTable dtNewSession, DataTable dtRemSession, DataTable dtNewModule, DataTable dtRemModule, DataTable dtModDates, DataTable dtModOrder)
        public bool updateBundle(int bundleId, string bundleType, DateTime bundleEffDt, decimal bundleCost, int userId,
            DataTable dtNewSession, DataTable dtRemSession, DataTable dtNewModule, DataTable dtRemModule, DataTable dtModDates, DataTable dtModOrder)
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
                    //update batch module dates
                    cmd.CommandText = "update batch_module set startDate=@sdt, endDate=@edt, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                        + "where batchModuleId=@bid and (startDate<>@sdt or endDate<>@edt)";
                    foreach (DataRow dr in dtModDates.Rows)
                    {
                        cmd.Parameters.AddWithValue("@sdt", dr["startDate"]);
                        cmd.Parameters.AddWithValue("@edt", dr["endDate"]);
                        cmd.Parameters.AddWithValue("@bid", dr["batchModuleId"]);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    //remove modules
                    cmd.CommandText = "update batchModule_session set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where batchModuleId in "
                        + "(select batchModuleId from batch_module where programmeBatchId=@pbid and moduleId=@mid); "
                        + "update batch_module set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where programmeBatchId=@pbid and moduleId=@mid; "
                        + "update bundle_module set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where bundleId=@bid and moduleId=@mid";

                    //find distinct rows first
                    DataTable dtRemMod = (new DataView(dtRemModule)).ToTable(true, new string[] { "moduleId", "programmeId", "programmeBatchId" });

                    foreach (DataRow dr in dtRemMod.Rows)
                    {
                        cmd.Parameters.AddWithValue("@pbid", dr["programmeBatchId"]);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    //removed sessions
                    string bundleSql = "update bundle_module set ModuleNumOfSession=ModuleNumOfSession-@n, lastModifiedBy=@uid, lastModifiedDate=getdate() where bundleId=@bid and moduleId=@mid";
                    string bookingSql = "update venue_booking_record set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where sessionId=@sid";
                    string sessionSql = "update batchModule_session set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where sessionId=@sid";

                    int prevMod, prevBatch, cnt = 0;

                    if (dtRemSession.Rows.Count > 0)
                    {
                        //sort the table
                        DataRow[] drRemSessions = dtRemSession.Select("1=1", "moduleId, programmeBatchId");
                        prevMod = (int)drRemSessions[0]["moduleId"];
                        prevBatch = (int)drRemSessions[0]["programmeBatchId"];

                        foreach (DataRow dr in drRemSessions)
                        {
                            cmd.CommandText = sessionSql;
                            cmd.Parameters.AddWithValue("@sid", dr["sessionId"]);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            cmd.CommandText = bookingSql;
                            cmd.Parameters.AddWithValue("@sid", dr["sessionId"]);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            if (prevBatch == (int)dr["programmeBatchId"] && prevMod == (int)dr["moduleId"]) cnt++;
                            if (prevMod != (int)dr["moduleId"])
                            {
                                cmd.CommandText = bundleSql;
                                cmd.Parameters.AddWithValue("@n", cnt);
                                cmd.Parameters.AddWithValue("@bid", bundleId);
                                cmd.Parameters.AddWithValue("@mid", prevMod);
                                cmd.Parameters.AddWithValue("@uid", userId);

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                //reset values
                                prevBatch = (int)dr["programmeBatchId"];
                                cnt = 1;
                                prevMod = (int)dr["moduleId"];
                            }
                        }
                        cmd.CommandText = bundleSql;
                        cmd.Parameters.AddWithValue("@n", cnt);
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@mid", prevMod);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    //new sessions
                    sessionSql = "insert into batchModule_session(batchModuleId, sessionDate, sessionPeriod, venueId, defunct, createdBy, createdOn) "
                            + "values (@mid, @dt, @pt, @vid, 'N', @uid, getdate()); SELECT CAST(scope_identity() AS int);";

                    bookingSql = "insert into venue_booking_record (bookingDate, bookingPeriod, venueId, sessionId, defunct, createdBy, createdOn) "
                        + "values (@dt, @pt, @vid, @sid, 'N', @uid, getdate())";

                    bundleSql = "update bundle_module set ModuleNumOfSession=ModuleNumOfSession+@n, bundleEffectDate=@efDt, bundleCost=@cost, lastModifiedBy=@uid, lastModifiedDate=getdate() where bundleId=@bid and moduleId=@mid";

                    int sessionId;
                    DataRow[] drNewSessions = dtNewSession.Select("not isNewModule", "moduleId, programmeBatchId");
                    if (drNewSessions.Length > 0)
                    {
                        prevMod = (int)drNewSessions[0]["moduleId"];
                        prevBatch = (int)drNewSessions[0]["programmeBatchId"];           
                        foreach (DataRow dr in dtNewSession.Select("not isNewModule", "moduleId, programmeBatchId"))
                        {
                            cmd.CommandText = sessionSql;
                            cmd.Parameters.AddWithValue("@mid", dr["batchModuleId"]);
                            cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                            cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                            cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            sessionId = (int)cmd.ExecuteScalar();
                            cmd.Parameters.Clear();

                            cmd.CommandText = bookingSql;
                            cmd.Parameters.AddWithValue("@dt", dr["sessionDate"]);
                            cmd.Parameters.AddWithValue("@pt", dr["sessionPeriod"]);
                            cmd.Parameters.AddWithValue("@vid", dr["venueId"]);
                            cmd.Parameters.AddWithValue("@sid", sessionId);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            if (prevBatch == (int)dr["programmeBatchId"] && prevMod == (int)dr["moduleId"]) cnt++;
                            if (prevMod != (int)dr["moduleId"])
                            {
                                cmd.CommandText = bundleSql;
                                cmd.Parameters.AddWithValue("@n", cnt);
                                cmd.Parameters.AddWithValue("@bid", bundleId);
                                cmd.Parameters.AddWithValue("@cost", bundleCost);
                                cmd.Parameters.AddWithValue("@efDt", bundleEffDt);
                                cmd.Parameters.AddWithValue("@mid", prevMod);
                                cmd.Parameters.AddWithValue("@uid", userId);

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();

                                //reset values
                                prevBatch = (int)dr["programmeBatchId"];
                                cnt = 1;
                                prevMod = (int)dr["moduleId"];
                            }
                        }
                        cmd.CommandText = bundleSql;
                        cmd.Parameters.AddWithValue("@n", cnt);
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@cost", bundleCost);
                        cmd.Parameters.AddWithValue("@efDt", bundleEffDt);
                        cmd.Parameters.AddWithValue("@mid", prevMod);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                   //new modules
                    if (dtNewModule.Rows.Count > 0)
                    {
                        //module order will be updated later
                        string newModSql = "insert into bundle_module (bundleId, moduleId, moduleOrder, ModuleNumOfSession, defunct, createdBy, createdOn) "
                            + "values (@bid, @mid, 0, @numSession, 'N', @uid, getdate());";

                        string newBatchModSql = "insert into batch_module (programmeBatchId, moduleId, day, startDate, endDate, trainerUserId1, trainerUserId2, assessorUserId, defunct, createdBy, createdOn) "
                            + "values(@pic, @mid, @d, @sdt, @edt, @tid1, @tid2, @aid, 'N', @uid, getdate()); SELECT CAST(scope_identity() AS int);";


                        var query = from s in dtNewSession.AsEnumerable()
                                    group s by new
                                    {
                                        moduleId = s.Field<int>("moduleId"),
                                        programmeBatchId = s.Field<int>("programmeBatchId")
                                    } into g
                                    select new
                                    {
                                        g.Key.moduleId,
                                        g.Key.programmeBatchId,
                                        Count = g.Count()
                                    };
                        

                        DataView view = new DataView(dtNewModule);
                        DataTable dtMod = view.ToTable(true, "moduleId");
                        foreach (DataRow dr in dtMod.Rows)
                        {
                            cmd.CommandText = newModSql;
                            cmd.Parameters.AddWithValue("@bid", bundleId);
                            cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                            cmd.Parameters.AddWithValue("@bType", bundleType);
                            cmd.Parameters.AddWithValue("@numSession", query.First(r => r.moduleId == (int)dr["moduleId"]).Count);
                            cmd.Parameters.AddWithValue("@effDt", bundleEffDt);
                            cmd.Parameters.AddWithValue("@cost", bundleCost);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                        int batchModuleId;
                        foreach (DataRow dr in dtNewModule.Rows)
                        {
                            cmd.CommandText = newBatchModSql;
                            cmd.Parameters.AddWithValue("@pic", dr["programmeBatchId"]);
                            cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                            cmd.Parameters.AddWithValue("@d", dr["day"]);
                            cmd.Parameters.AddWithValue("@sdt", dr["startDate"]);
                            cmd.Parameters.AddWithValue("@edt", dr["endDate"]);
                            cmd.Parameters.AddWithValue("@tid1", dr["trainerUserId1"]);
                            cmd.Parameters.AddWithValue("@tid2", dr["trainerUserId2"]);
                            cmd.Parameters.AddWithValue("@aid", dr["assessorUserId"]);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            batchModuleId = (int)cmd.ExecuteScalar();
                            cmd.Parameters.Clear();

                            foreach (DataRow n in dtNewSession.Select("moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString()))
                            {
                                cmd.CommandText = sessionSql;
                                cmd.Parameters.AddWithValue("@mid", batchModuleId);
                                cmd.Parameters.AddWithValue("@dt", n["sessionDate"]);
                                cmd.Parameters.AddWithValue("@pt", n["sessionPeriod"]);
                                cmd.Parameters.AddWithValue("@vid", n["venueId"]);
                                cmd.Parameters.AddWithValue("@uid", userId);

                                sessionId = (int)cmd.ExecuteScalar();
                                cmd.Parameters.Clear();

                                cmd.CommandText = bookingSql;
                                cmd.Parameters.AddWithValue("@dt", n["sessionDate"]);
                                cmd.Parameters.AddWithValue("@pt", n["sessionPeriod"]);
                                cmd.Parameters.AddWithValue("@vid", n["venueId"]);
                                cmd.Parameters.AddWithValue("@sid", sessionId);
                                cmd.Parameters.AddWithValue("@uid", userId);

                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                        }
                    }

                    //module order
                    cmd.CommandText = "update bundle_module set moduleOrder=@cnt where bundleId=@bid and moduleId=@mid";
                    cnt = 1;
                    foreach (DataRow dr in dtModOrder.Rows)
                    {
                        cmd.Parameters.AddWithValue("@cnt", cnt);
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                        cnt++;
                    }

                    //bundle
                    cmd.CommandText = "update bundle set bundleType=@bt, bundleEffectDate=@bdt, bundleCost=@cost, lastModifiedBy=@usr, lastModifiedDate=getdate() where bundleId=@bid";
                    cmd.Parameters.AddWithValue("@bt", bundleType);
                    cmd.Parameters.AddWithValue("@bdt", bundleEffDt);
                    cmd.Parameters.AddWithValue("@cost", bundleCost);
                    cmd.Parameters.AddWithValue("@usr", userId);
                    cmd.Parameters.AddWithValue("@bid", bundleId);

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "updateBundle()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        //public bool updateBundle(string bundleCode, string bundleType, DateTime bundleEffDate, decimal bundleCost, DataTable dtModules, int userId)
        public bool updateBundle(int bundleId, string bundleType, DateTime bundleEffDate, decimal bundleCost, DataTable dtModules, int userId)
        {
            DataTable dtOriModules = getBundleModules(bundleId);
            if (dtOriModules == null) return false;

            //add a column to define the order
            dtModules.Columns.Add(new DataColumn("moduleOrder", typeof(int)));
            for (int i = 0; i < dtModules.Rows.Count; i++) dtModules.Rows[i]["moduleOrder"] = i + 1;

            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;

                try
                {
                    cmd.CommandText = "update bundle set bundleType=@bt, bundleEffectDate=@bdt, bundleCost=@cost, lastModifiedBy=@usr, lastModifiedDate=getdate() where bundleId=@bid";
                    cmd.Parameters.AddWithValue("@bt", bundleType);
                    cmd.Parameters.AddWithValue("@bdt", bundleEffDate);
                    cmd.Parameters.AddWithValue("@cost", bundleCost);
                    cmd.Parameters.AddWithValue("@usr", userId);
                    cmd.Parameters.AddWithValue("@bid", bundleId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    //put the updated list of modules into a list
                    List<int> modules = new List<int>();
                    foreach (DataRow dr in dtModules.Rows) modules.Add((int)dr["moduleId"]);

                    string delModSql = "update bundle_module set defunct='Y', lastModifiedBy=@usr, lastModifiedDate=getdate() where bundleModuleId=@bmid";
                    string updModSql = "update bundle_module set moduleOrder=@cnt, ModuleNumOfSession=@ns, lastModifiedBy=@usr, lastModifiedDate=getdate() where bundleModuleId=@bmid";

                    foreach (DataRow dr in dtOriModules.Rows)
                    {
                        DataRow[] drTmp = dtModules.Select("moduleId=" + dr["moduleId"].ToString());

                        if (drTmp == null || drTmp.Length == 0)
                        {
                            //module don exist in updated list, delete
                            cmd.CommandText = delModSql;
                            cmd.Parameters.AddWithValue("@usr", userId);
                            cmd.Parameters.AddWithValue("@bmid", (int)dr["bundleModuleId"]);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        else
                        {
                            //modules exist, update
                            cmd.CommandText = updModSql;
                            cmd.Parameters.AddWithValue("@cnt", (int)drTmp[0]["moduleOrder"]);
                            cmd.Parameters.AddWithValue("@ns", (int)drTmp[0]["ModuleNumOfSession"]);
                            cmd.Parameters.AddWithValue("@usr", userId);
                            cmd.Parameters.AddWithValue("@bmid", (int)dr["bundleModuleId"]);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                            //remove from updated list
                            dtModules.Rows.Remove(drTmp[0]);
                        }
                    }

                    //what is left in the updated module list datatable are new modules
                    cmd.CommandText = "insert into bundle_module(bundleId, moduleId, moduleOrder, ModuleNumOfSession, createdBy) values (@bid, @mid, @cnt, @ns, @usr)";
                    foreach (DataRow dr in dtModules.Rows)
                    {
                        cmd.Parameters.AddWithValue("@bid", bundleId);
                        cmd.Parameters.AddWithValue("@mid", dr["moduleId"]);
                        cmd.Parameters.AddWithValue("@cnt", dr["moduleOrder"]);
                        cmd.Parameters.AddWithValue("@ns", dr["ModuleNumOfSession"]);
                        cmd.Parameters.AddWithValue("@usr", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "updateBundle()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    //remove the extra column added
                    dtModules.Columns.Remove("moduleOrder");

                    sqlConn.Close();
                }
            }
        }

        public DataTable getRecentModBundle()
        {
            try
            {
                string sqlStatement = @"SELECT top 10 bundleCode, bundleId, isnull(lastModifiedDate, createdOn) FROM bundle where defunct='N' order by 3 desc";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                DataTable dtPackage = dbConnection.getDataTable(cmd);


                return dtPackage;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getRecentModBundle()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getListBundle(string frm, string to, bool containNum = false)
        {
            try
            {
                string sqlStatement = @"SELECT bundleCode, bundleId FROM bundle
                                      where defunct='N' and (SUBSTRING(UPPER(bundleCode),1 ,1) BETWEEN @frm AND @to "
                                        + (containNum ? "or SUBSTRING(bundleCode,1 ,1) BETWEEN '0' AND '9'" : "") + ") order by bundleCode";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@frm", frm);
                cmd.Parameters.AddWithValue("@to", to);

                DataTable dtPackage = dbConnection.getDataTable(cmd);


                return dtPackage;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Bundle.cs", "getListBundle()", ex.Message, -1);

                return null;
            }
        }

    }
}
