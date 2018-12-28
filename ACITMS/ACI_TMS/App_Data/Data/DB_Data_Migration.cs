using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using LogicLayer;
using GeneralLayer;


namespace DataLayer
{

    public struct applicantResult
    {
        public string applicantId;
        public string sno;
        public string name;
        public string msg;
        public string exemMod;
    }

    public class DB_Data_Migration
    {
        Module_Management mm = new Module_Management();
        Bundle_Management bm = new Bundle_Management();
        DB_Bundle dbBundle = new DB_Bundle();

        private Database_Connection dbConnection = new Database_Connection();

        public Tuple<bool, string> insertPayment(TraineeS ts, int userId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"insert into payment_history (applicantId, programmebatchid, paymentdate, paymentamount, idnumber, paymentmode, 
                                        receiptnumber, bankindate, paymenttype, paymentstatus, referencenumber, createdby) VALUES (@applicantId, @progBatchId, @paymentdate, @paymentamount, 
                                        (select idNumber from applicant where applicantId = @applicantId), @paymentMode,
                                        (select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history), @bankIndate, @paymenttype, @paymentstatus, @refnum, @createdby)";

                    cmd.Parameters.AddWithValue("@applicantId", ts.applicantId);
                    cmd.Parameters.AddWithValue("@progBatchId", ts.programmeBatchId);
                    cmd.Parameters.AddWithValue("@paymentdate", ts.PaymentDate);
                    cmd.Parameters.AddWithValue("@paymentamount", ts.paymentAmt);
                    cmd.Parameters.AddWithValue("@paymentMode", ts.paymentMode);
                    //cmd.Parameters.AddWithValue("@receiptNumber", ts.receiptNumber == "" ? (object)DBNull.Value : ts.receiptNumber);
                    cmd.Parameters.AddWithValue("@bankIndate", ts.convertBankInDate == true ? ts.BankInDate : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@paymenttype", ts.paymentFor);
                    cmd.Parameters.AddWithValue("@paymentstatus", GeneralLayer.PaymentStatus.PAID.ToString());
                    cmd.Parameters.AddWithValue("@refnum", ts.ReferenceNo);
                    cmd.Parameters.AddWithValue("@createdby", userId);

                    int rowAffected = cmd.ExecuteNonQuery();

                    if (rowAffected > 0)
                    {
                        cmd.Parameters.Clear();
                        if (!(ts.paymentFor == GeneralLayer.PaymentType.REG.ToString()))
                        {
                            cmd = new SqlCommand();
                            cmd.Connection = sqlConn;
                            cmd.CommandText = "UPDATE applicant set GSTPayableAmount = @gst where applicantid = @aId";
                            cmd.Parameters.AddWithValue("@aId", ts.applicantId);
                            cmd.Parameters.AddWithValue("@gst", ts.gstPayableAmt);
                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();

                        }
                        return new Tuple<bool, string>(true, "Payment Inserted Successfully For " + ts.applicantId);
                    }
                    else
                        return new Tuple<bool, string>(false, "Payment not inserted successfully For " + ts.applicantId);
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "insertPayment()", ex.Message, -1);
                    return new Tuple<bool, string>(false, "DB Error: " + ex.Message);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public int getUserId(string interviewerName)
        {
            try
            {

                SqlCommand cmd = new SqlCommand();
                string sql = @"select userId from aci_user where [userName]=@UserName";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@UserName", interviewerName);

                if (dbConnection.getDataTable(cmd).Rows.Count < 0)
                    return -1;
                else
                    return int.Parse(dbConnection.getDataTable(cmd).Rows[0]["userId"].ToString());
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Data_Migration.cs", "getUserId()", ex.Message, -1);
                return -1;
            }
        }

        public Tuple<bool, string, int, decimal> getModuleId(string classCode, string moduleCode)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"select m.moduleid, m.moduleCost from bundle_module bm inner join module_structure m on bm.moduleId = m.moduleid where 
                                        bm.bundleid = (select bundleid from programme_structure ps inner join programme_batch pb on ps.programmeid = pb.programmeid where pb.batchCode = @BatchCode) 
                                        and m.moduleCode = @ModuleCode";

                    cmd.Parameters.AddWithValue("@BatchCode", classCode);
                    cmd.Parameters.AddWithValue("@ModuleCode", moduleCode);

                    DataTable rs = dbConnection.getDataTable(cmd);

                    if (rs.Rows.Count <= 0)
                        return new Tuple<bool, string, int, decimal>(false, "Exempted Module Not Found For Applicant's Programme", -1, -1);
                    else
                        return new Tuple<bool, string, int, decimal>(true, "", int.Parse(rs.Rows[0]["moduleid"].ToString()), decimal.Parse(rs.Rows[0]["moduleCost"].ToString()));


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "getCodeValue()", ex.Message, -1);

                    return new Tuple<bool, string, int, decimal>(false, "DB Error", -1, -1);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool insertSubsidy(int programmeId, string schemeName, DateTime effectiveDate, string subType, decimal value, int userId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"insert into subsidy (subsidyType, subsidyValue, programmeId, effectiveDate, subsidyScheme, defunct, createdBy) VALUES
                                        (@subsidyType, @subsidyValue, @programmeId, @effectiveDate, @subsidyScheme, @defunct, @createdBy)";
                    cmd.Parameters.AddWithValue("@subsidyType", subType);
                    cmd.Parameters.AddWithValue("@subsidyValue", value);
                    cmd.Parameters.AddWithValue("@programmeId", programmeId);
                    cmd.Parameters.AddWithValue("@effectiveDate", effectiveDate);
                    cmd.Parameters.AddWithValue("@subsidyScheme", schemeName);
                    cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                    cmd.Parameters.AddWithValue("@createdBy", userId);

                    int result = cmd.ExecuteNonQuery();

                    if (result <= 0)
                        return false;
                    else
                        return true;


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "insertSubsidy()", ex.Message, -1);

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public int findProgramme(int version, string programmecode, string coursecode)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"select programmeId from programme_structure where programmeCode = @ProgrammeCode and programmeVersion = @version and coursecode =@coursecode";
                    cmd.Parameters.AddWithValue("@ProgrammeCode", programmecode);
                    cmd.Parameters.AddWithValue("@version", version);
                    cmd.Parameters.AddWithValue("@coursecode", coursecode);
                    DataTable dt = dbConnection.getDataTable(cmd);
                    if (dt.Rows.Count <= 0)
                        return -1;
                    else
                        return int.Parse(dt.Rows[0]["programmeId"].ToString());
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "findProgramme()", ex.Message, -1);

                    return -1;
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public Tuple<string, int> getProgrammeId(string classCode)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"select programmeBatchId from programme_batch where batchCode = @BatchCode";

                    cmd.Parameters.AddWithValue("@BatchCode", classCode);

                    DataTable rs = dbConnection.getDataTable(cmd);
                    if (rs.Rows.Count > 0)
                        return new Tuple<string, int>("", int.Parse(rs.Rows[0]["programmeBatchId"].ToString()));
                    else
                        return new Tuple<string, int>("Programmed Not Found for the Class Code", -1);


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "getCodeValue()", ex.Message, -1);

                    return new Tuple<string, int>("DB Error!", -1);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
        public Tuple<bool, string, decimal, int> getSubsidy(string subsidyScheme)
        {


            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = @"select subsidyid, subsidyType, subsidyValue from subsidy s where s.subsidyscheme = @SubsidyScheme";


                    cmd.Parameters.AddWithValue("@SubsidyScheme", subsidyScheme);

                    DataTable rs = dbConnection.getDataTable(cmd);

                    if (rs.Rows.Count <= 0)
                        return new Tuple<bool, string, decimal, int>(false, "Subsidy Scheme Not Found", 0, -1);
                    else
                        return new Tuple<bool, string, decimal, int>(true, rs.Rows[0]["subsidyType"].ToString(), Convert.ToDecimal(rs.Rows[0]["subsidyValue"].ToString()), int.Parse(rs.Rows[0]["SubsidyId"].ToString()));


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "getCodeValue()", ex.Message, -1);

                    return new Tuple<bool, string, decimal, int>(false, "DB Error", 0, -1);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
        public string getCodeValue(string codeType, string codeValueDisplay)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = "select codeValue from code_reference where codetype = @codeType and codeValueDisplay = @codeValueDisplay";

                    cmd.Parameters.AddWithValue("@codeValueDisplay", codeValueDisplay);
                    cmd.Parameters.AddWithValue("@codeType", codeType);

                    string codeVal = dbConnection.executeScalarString(cmd);
                    return codeVal;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "getCodeValue()", ex.Message, -1);

                    return "";
                }
                finally
                {
                    sqlConn.Close();
                }

            }
        }
        public bool checkClassType(string classType)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = "select count(1) from code_reference where codetype = 'CLTYPE' and codevalue = @codevalue";
                    cmd.Parameters.AddWithValue("@codevalue", classType);

                    int count = (int)cmd.ExecuteScalar();


                    if (count > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "checkClassType()", ex.Message, -1);

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }

            }
            return false;
        }

        public bool checkClassValue(string classValue)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                try
                {
                    sqlConn.Open();

                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = sqlConn;

                    cmd.CommandText = "select count(1) from code_reference where codetype = 'CLTYPE' and UPPER(codeValueDisplay) = @codeValueDisplay";
                    cmd.Parameters.AddWithValue("@codeValueDisplay", classValue.ToUpper());

                    int count = (int)cmd.ExecuteScalar();


                    if (count > 0)
                        return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "checkClassType()", ex.Message, -1);

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }

            }
            return false;
        }

        //string reAssessmentRequired, string ReTakeRequired,
        //        int reTakeModuleCodeId,
        public bool updateSOA(string assessmentCompleted, string moduleResult, string fAssessmentDate, int accessorId, int finalAccessorId,
                                string soaModStatus, string dtSOAProcessedDate, string dtSOAReceivedDate, string TraineeID, string ClassCode,
                                int ModuleId, int BatchModuleId, string finalAssessmentDate, int programmeBatchId, string reassessment, int userid)
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
                    cmd.CommandText = @"update trainee_module set moduleResult = @moduleResult, assessmentcompleted = @assessmentCompleted, 
                                        firstAssessmentDate = @firstAssessmentDate, firstAssessorId = @firstAccessorId,finalAssessmentDate = @finalAssessmentDate,  
                                        finalAssessorId = @finalAssessorId, SOAStatus=@SOAStatus, processSOADate = @processSOADate, receivedSOADate = @receivedSOADate, 
                                        lastModifiedBy=@lastModifiedBy, lastModifiedDate=GetDate(),reAssessment = @reAssessment  where [traineeId] =@traineeId and programmeBatchId=@programmeBatchId 
                                        and batchModuleId = @batchModuleId and moduleId =@moduleId";

                    cmd.Parameters.AddWithValue("@moduleResult", moduleResult);
                    cmd.Parameters.AddWithValue("@assessmentCompleted", assessmentCompleted);
                    cmd.Parameters.AddWithValue("@firstAssessmentDate", Convert.ToDateTime(fAssessmentDate));
                    cmd.Parameters.AddWithValue("@firstAccessorId", accessorId);
                    cmd.Parameters.AddWithValue("@finalAssessmentDate", finalAssessmentDate == "" ? (object)DBNull.Value : Convert.ToDateTime(finalAssessmentDate));
                    cmd.Parameters.AddWithValue("@finalAssessorId", finalAccessorId == -1 ? (object)DBNull.Value : finalAccessorId);
                    cmd.Parameters.AddWithValue("@SOAStatus", soaModStatus);
                    cmd.Parameters.AddWithValue("@processSOADate", dtSOAProcessedDate == "" ? (object)DBNull.Value : Convert.ToDateTime(dtSOAProcessedDate));
                    cmd.Parameters.AddWithValue("@receivedSOADate", dtSOAReceivedDate == "" ? (object)DBNull.Value : Convert.ToDateTime(dtSOAReceivedDate));
                    cmd.Parameters.AddWithValue("@lastModifiedBy", userid);
                    cmd.Parameters.AddWithValue("@traineeId", TraineeID);
                    cmd.Parameters.AddWithValue("@programmeBatchId", programmeBatchId);
                    cmd.Parameters.AddWithValue("@batchModuleId", BatchModuleId);
                    cmd.Parameters.AddWithValue("@moduleId", ModuleId);
                    cmd.Parameters.AddWithValue("@reAssessment", reassessment);
                    trans.Commit();
                    return cmd.ExecuteNonQuery() > 0 ? true : false;

                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "updateSOA()", ex.Message, -1);
                    trans.Rollback();
                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
            // 
            //
        }

        //DONE//
        public Tuple<bool, string, string> insertIntoModule(string moduleCode, int moduleVersion, string moduleLevel, string moduleTitle, string moduleDescription,
            int moduleCredit, decimal moduleCost, DateTime moduleEffectDate, decimal moduleTrainingHour, string WSQCompetencyCode, int userId)
        {
            Tuple<bool, string> result = mm.createModule(moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription, moduleCredit, moduleCost, moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, userId);

            return new Tuple<bool, string, string>(result.Item1, result.Item2, moduleCode);
        }

        //DONE//
        public Tuple<bool, string> createBundle(string bundleCode, string bundleType, DataTable dtBundleMod, int userId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;


                if (dbBundle.checkBundleCodeExist(bundleCode))
                {
                    return new Tuple<bool, string>(false, ("Bundle Code: " + bundleCode + " Exists. Insertion Failed."));
                }
                else
                {
                    try
                    {
                        cmd.CommandText = @"insert into bundle (bundleCode, bundleType, bundleEffectDate, bundleCost, createdBy, createdOn) 
                                            values (@bc, @bt, getdate(), 0, @usr, getdate()); SELECT CAST(scope_identity() AS int);";

                        cmd.Parameters.AddWithValue("@bc", bundleCode);
                        cmd.Parameters.AddWithValue("bt", bundleType);
                        cmd.Parameters.AddWithValue("@usr", userId);
                        int bundleId = (int)cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        for (int i = 0; i < dtBundleMod.Rows.Count; i++)
                        {

                            string moduleCode = dtBundleMod.Rows[i]["ModuleCode"].ToString();

                            if ((new DB_Module()).checkModuleCodeExist(moduleCode))
                            {
                                int numOfSession = int.Parse(dtBundleMod.Rows[i]["NumOfSession"].ToString());
                                int moduleOrder = int.Parse(dtBundleMod.Rows[i]["ModuleOrder"].ToString());

                                cmd.CommandText = @"insert into bundle_module (bundleId, moduleId, ModuleNumOfSession, moduleOrder, createdBy, createdOn, defunct) 
                                                    values (@bc1, (select moduleid from module_structure where modulecode = @mc), @numOfSessions, @moduleOrder, @usr, getdate(), 'N')";

                                cmd.Parameters.AddWithValue("@bc1", bundleId);
                                cmd.Parameters.AddWithValue("@mc", moduleCode);
                                cmd.Parameters.AddWithValue("@numOfSessions", numOfSession);
                                cmd.Parameters.AddWithValue("@moduleOrder", moduleOrder);
                                cmd.Parameters.AddWithValue("@usr", userId);
                                cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                                cmd.ExecuteNonQuery();
                                cmd.Parameters.Clear();
                            }
                            else
                            {
                                trans.Rollback();
                                return new Tuple<bool, string>(false, ("Module Code: " + moduleCode + "does not exists! Bundle Insertion Failed."));

                            }

                        }

                        cmd.CommandText = @"update bundle set bundleEffectDate = (select max(moduleEffectDate) from module_structure ms right join 
                                            bundle_module bm on ms.moduleid = bm.moduleId where bm.bundleid = @bi), bundleCost = (select sum(modulecost) 
                                            from module_structure ms left join bundle_module bm on ms.moduleid = bm.moduleid where bundleid = @bi) where bundleId = @bi";


                        cmd.Parameters.AddWithValue("@bi", bundleId);
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();

                        trans.Commit();


                    }
                    catch (Exception ex)
                    {
                        Log_Handler lh = new Log_Handler();
                        lh.WriteLog(ex, "DB_Data_Migration.cs", "createBundle()", ex.Message, -1);

                        trans.Rollback();
                        return new Tuple<bool, string>(false, ("DB Transaction Error!"));

                    }
                    finally
                    {
                        sqlConn.Close();
                    }

                    return new Tuple<bool, string>(true, (bundleCode + " inserted successfully. All modules under this bundle are also inserted."));

                }
            }
        }

        public Tuple<bool, string, int> createProgrammeBatch(DataRow dt, int userId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;

                string programmeCode = dt["ProgrammeCode"].ToString();
                int version = int.Parse(dt["Version"].ToString());
                string lvl = dt["ProgrammeLevel"].ToString().ToUpper();
                string category = dt["ProgrammeCategory"].ToString().ToUpper();
                string projectCode = dt["ProjectCode"].ToString();
                string classCode = dt["ClassCode"].ToString();
                string lessonType = dt["LessonType"].ToString().ToUpper();
                DateTime dtRegStartDate = Convert.ToDateTime(dt["RegistrationStartDate"].ToString());
                DateTime dtRegEndDate = Convert.ToDateTime(dt["RegistrationEndDate"].ToString());
                DateTime dtStartDate = Convert.ToDateTime(dt["CommencementStartDate"].ToString());
                DateTime dtEndDate = Convert.ToDateTime(dt["CommencementEndDate"].ToString());
                int capacity = int.Parse(dt["Capacity"].ToString());
                string mode = dt["Mode"].ToString().ToUpper();

                try
                {
                    string sqlProgrammeID = @"select ProgrammeId from programme_structure where programmecode = @programmeCode and programmeversion = @programmeVersion and 
                                            programmelevel = (select codeValue from code_reference where codetype = 'PGLVL' and upper(codeValueDisplay) = @programmeLvl) 
                                            and programmeCategory = (select codeValue from code_reference where codetype = 'PGCAT' and upper(codeValueDisplay) = @programmeCat)";

                    cmd.CommandText = sqlProgrammeID;
                    cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
                    cmd.Parameters.AddWithValue("@programmeVersion", version);
                    cmd.Parameters.AddWithValue("@programmeLvl", lvl);
                    cmd.Parameters.AddWithValue("@programmeCat", category);
                    int programmeId = (int)cmd.ExecuteScalar();

                    cmd.CommandText = @"insert into programme_batch(programmeId, batchCode, programmeRegStartDate, programmeRegEndDate, programmeStartDate, programmeCompletionDate, 
                                        batchCapacity, classMode, defunct, createdBy, projectCode) values(@ProgrammeId, CONCAT(@batchCode, (select codeValue from code_reference 
                                        where codetype = 'CLTYPE' and upper(codeValueDisplay) = @lessonType)), @programmeRegStartDate, @programmeRegEndDate, @programmeStartDate, 
                                        @programmeCompletionDate, @batchCapcity, (select codeValue from code_reference where codetype = 'CLMODE' and upper(codeValueDisplay) = @classMode), 
                                        @defunct, @createBy, @projectCode);SELECT CAST(scope_identity() AS int)";

                    cmd.Parameters.AddWithValue("@ProgrammeId", programmeId);
                    cmd.Parameters.AddWithValue("@batchCode", classCode);
                    cmd.Parameters.AddWithValue("@lessonType", lessonType);
                    cmd.Parameters.AddWithValue("@programmeRegStartDate", dtRegStartDate);
                    cmd.Parameters.AddWithValue("@programmeRegEndDate", dtRegEndDate);
                    cmd.Parameters.AddWithValue("@programmeStartDate", dtStartDate);
                    cmd.Parameters.AddWithValue("@programmeCompletionDate", dtEndDate);
                    cmd.Parameters.AddWithValue("@batchCapcity", capacity);
                    cmd.Parameters.AddWithValue("@classMode", mode);
                    cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                    cmd.Parameters.AddWithValue("@createBy", userId);
                    cmd.Parameters.AddWithValue("@projectCode", projectCode);
                    int programmeBatchId = (int)cmd.ExecuteScalar();

                    trans.Commit();

                    return new Tuple<bool, string, int>(true, classCode + " Inserted Successfully", programmeBatchId);
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "createProgrammeBatch()", ex.Message, -1);

                    trans.Rollback();
                    return new Tuple<bool, string, int>(false, "DB Transaction Error For: " + classCode, -1);
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }
        public Tuple<bool, string> createBatchModule(string modCode, string convertedDay, DateTime startDate, DateTime endDate, int userId, int programmeBatchId, int trainer1, int trainer2, int assessor, DataRow[] dtSessionDetails)
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
                    cmd.CommandText = @"insert into batch_module (programmeBatchId, moduleId, day, startDate, endDate, defunct, createdBy, traineruserid1, traineruserid2, assessoruserid) VALUES
                                        ( @programmeBatchId, (select moduleid from module_structure where moduleCode = @moduleCode), 
                                        @day, @startDate, @endDate, @defunct, @createdBy, @traineruserid1, @traineruserid2, @assessoruserid); SELECT CAST(scope_identity() AS int);";

                    cmd.Parameters.AddWithValue("@programmeBatchId", programmeBatchId);
                    cmd.Parameters.AddWithValue("@moduleCode", modCode);
                    cmd.Parameters.AddWithValue("@day", convertedDay);
                    cmd.Parameters.AddWithValue("@startDate", startDate);
                    cmd.Parameters.AddWithValue("@endDate", endDate);
                    cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                    cmd.Parameters.AddWithValue("@createdBy", userId);
                    cmd.Parameters.AddWithValue("@traineruserid1", trainer1 == -1 ? (object)DBNull.Value : trainer1);
                    cmd.Parameters.AddWithValue("@traineruserid2", trainer2 == -1 ? (object)DBNull.Value : trainer2);
                    cmd.Parameters.AddWithValue("@assessoruserid", assessor == -1 ? (object)DBNull.Value : assessor);
                    int batchId = (int)cmd.ExecuteScalar();

                    for (int i = 0; i < dtSessionDetails.Length; i++)
                    {

                        string SessionclassCode = dtSessionDetails[i]["ClassCode"].ToString();
                        string moduleCode = dtSessionDetails[i]["ModuleCode"].ToString();
                        DateTime date = Convert.ToDateTime(dtSessionDetails[i]["Date"].ToString());
                        string period = dtSessionDetails[i]["Period"].ToString();
                        string venue = dtSessionDetails[i]["Venue"].ToString();

                        cmd.Parameters.Clear();

                        cmd.CommandText = @"insert into batchModule_Session (batchModuleId, sessionDate, sessionPeriod, venueId, attendanceMarked, defunct, createdBy) 
                                            VALUES (@batchModuleId, @sessionDate, @sessionPeriod, @venue, @attendanceMarked, @defunct, @createdBy)";

                        cmd.Parameters.AddWithValue("@batchModuleId", batchId);
                        cmd.Parameters.AddWithValue("@sessionDate", date);
                        cmd.Parameters.AddWithValue("@sessionPeriod", period);
                        cmd.Parameters.AddWithValue("@venue", venue);
                        cmd.Parameters.AddWithValue("@attendanceMarked", General_Constance.STATUS_NO);
                        cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                        cmd.Parameters.AddWithValue("@createdBy", userId);
                        cmd.ExecuteNonQuery();



                    }

                    trans.Commit();

                    return new Tuple<bool, string>(true, "All Class Details are inserted");



                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "createBatchModule()", ex.Message, -1);
                    trans.Rollback();
                    return new Tuple<bool, string>(false, "Something went wrong, please check your data.");

                }
                finally
                {
                    sqlConn.Close();

                }
            }
        }
        public bool createProgramme(string programmeCode, string courseCode, int programmeVersion, string programmeLevel, string programmeCategory, string programmeTitle,
            string programmeDescription, int numOfSOA, string SSGRefNum, string bundleCode, string programmeType, int userId)
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
                    cmd.CommandText = @"INSERT INTO [programme_structure]
                                      (programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, programmeTitle,
                                      programmeDescription, numberOfSOA, SSGRefNum, bundleId, programmeType, createdBy)
                                      VALUES
                                      (@programmeCode, @courseCode, @programmeVersion, (select codeValue from code_reference where codetype = 'PGLVL' and UPPER(codevaluedisplay) = @programmeLevel), 
                                      (select codeValue from code_reference where codetype = 'PGCAT' and UPPER(codevaluedisplay) = @programmeCategory), @programmeTitle,
                                      @programmeDescription, @numberOfSOA, @SSGRefNum, (select bundleId from bundle where bundlecode = @bundleCode), @programmeType, @createdBy)";

                    cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
                    cmd.Parameters.AddWithValue("@courseCode", courseCode);
                    cmd.Parameters.AddWithValue("@programmeVersion", programmeVersion);
                    cmd.Parameters.AddWithValue("@programmeLevel", programmeLevel.ToUpper());
                    cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory.ToUpper());
                    cmd.Parameters.AddWithValue("@programmeTitle", programmeTitle);
                    cmd.Parameters.AddWithValue("@programmeDescription", programmeDescription);
                    cmd.Parameters.AddWithValue("@numberOfSOA", numOfSOA);
                    cmd.Parameters.AddWithValue("@SSGRefNum", SSGRefNum);
                    cmd.Parameters.AddWithValue("@bundleCode", bundleCode);
                    cmd.Parameters.AddWithValue("@programmeType", programmeType);
                    cmd.Parameters.AddWithValue("@createdBy", userId);

                    cmd.ExecuteNonQuery();
                    trans.Commit();

                    cmd.Parameters.Clear();

                    return true;


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "createProgramme()", ex.Message, -1);

                    trans.Rollback();
                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public Tuple<bool, applicantResult> insertApplicant(ApplicantRec applicantRow, int userId)
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

                    cmd.CommandText = @"select format((cast(@applicationDate as datetime)), 'yyMM')+right('0000'+ rtrim(
                                        convert(nvarchar,isnull(max(convert(int, case when isnumeric(right(applicantId, 4))= 0 then '0' else right(applicantId, 4) end)),0)+1)), 4) as id 
                                        from applicant where left(applicantId, 4)=format((cast(@applicationDate as datetime)), 'yyMM')";

                    cmd.Parameters.AddWithValue("@applicationDate", applicantRow.applDate);

                    string applicantId = (string)cmd.ExecuteScalar();

                    cmd.Parameters.Clear();

                    cmd.CommandText = @";with PROG_FEES_CTE as (
                            select bundleCost - @exemptedCost as progCost
                            from bundle b inner join programme_structure p on b.bundleId=p.bundleId
                            inner join programme_batch pb on pb.programmeId=p.programmeId and pb.programmeBatchId=@pbid
                        ),
                     SUBSIDY_CTE as (
                            SELECT CAST(PROG_FEES_CTE.progCost * NULLIF(@subsidyAmt, 0) as DECIMAL (18,2)) as subsidyAmount from PROG_FEES_CTE),
                        BLACKLIST_CTE as (
                            select case when cnt=0 then 'N' else 'Y' end as status
                            from (select count(*) as cnt from aci_suspended_list where idNumber=@id and @applicationDate between startDate and endDate) t
                        ) "
                       +
                        "insert into applicant(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode " +
                        ", highestEducation, spokenLanguage, writtenLanguage, applicationDate, selfSponsored, programmeBatchId, programmePayableAmount, interviewStatus, getToKnowChannel " +
                        ", shortlistStatus, blacklistStatus, applicationStatus, rejectStatus, createdBy, createdOn, applicantExemModule, highestEduRemarks, registrationFee, subsidyid, subsidyamt) " +
                        "select @aid, @fname, @id, @idType, @nation, @gender, @contact1, @contact2, @email, @race, @dob, @addr, @postal, @highEdu, @spokeLang, @writeLang, getdate(), @spon, @pbid " +
                        ", p.progCost, @iStatus, @kChannel " +
                        ", 'Y', b.status, '" + ApplicantStatus.NEW.ToString() + @"', 'N', @uid, getdate(), @exemModule, @highestEduRemarks, @regFees, @subsidyid";
                    if (applicantRow.subsidyType == GeneralLayer.SubsidyType.RATE.ToString())
                    {
                        cmd.CommandText += ", (select scte.subsidyAmount from SUBSIDY_CTE scte)";
                    }
                    else
                    {
                        cmd.CommandText += ", @subsidyAmt";
                    }
                    cmd.CommandText += " from PROG_FEES_CTE p cross join BLACKLIST_CTE b";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@fname", applicantRow.Name);
                    cmd.Parameters.AddWithValue("@id", applicantRow.idNumber);
                    cmd.Parameters.AddWithValue("@idType", applicantRow.idType);
                    cmd.Parameters.AddWithValue("@nation", applicantRow.nationality);
                    cmd.Parameters.AddWithValue("@gender", applicantRow.gender);
                    cmd.Parameters.AddWithValue("@contact1", applicantRow.contactNo1);
                    cmd.Parameters.AddWithValue("@contact2", applicantRow.contactNo2 == "" ? (object)DBNull.Value : applicantRow.contactNo2);
                    cmd.Parameters.AddWithValue("@email", applicantRow.emailAddress == "" ? (object)DBNull.Value : applicantRow.emailAddress);
                    cmd.Parameters.AddWithValue("@race", applicantRow.race);
                    cmd.Parameters.AddWithValue("@dob", applicantRow.dob);
                    cmd.Parameters.AddWithValue("@addr", applicantRow.address);
                    cmd.Parameters.AddWithValue("@postal", applicantRow.postalCode);
                    cmd.Parameters.AddWithValue("@highEdu", applicantRow.highestEdu);
                    cmd.Parameters.AddWithValue("@spokeLang", applicantRow.spokenLang);
                    cmd.Parameters.AddWithValue("@writeLang", applicantRow.writtenLang);
                    cmd.Parameters.AddWithValue("@exemModule", applicantRow.exempMod);
                    cmd.Parameters.AddWithValue("@highestEduRemarks", applicantRow.highedEduRemarks == "" ? (object)DBNull.Value : applicantRow.highedEduRemarks);
                    cmd.Parameters.AddWithValue("@spon", applicantRow.selfSpon);
                    cmd.Parameters.AddWithValue("@pbid", applicantRow.programmeBatchId);
                    cmd.Parameters.AddWithValue("@applicationDate", applicantRow.applDate);
                    cmd.Parameters.AddWithValue("@exemptedCost", applicantRow.exemptedCost);
                    cmd.Parameters.AddWithValue("@regFees", applicantRow.registrationFees == -1 ? (object)DBNull.Value : applicantRow.registrationFees);
                    cmd.Parameters.AddWithValue("@subsidyId", applicantRow.subsidyId == -1 ? (object)DBNull.Value : applicantRow.subsidyId);

                    if (applicantRow.subsidyId == -1)
                        cmd.Parameters.AddWithValue("@subsidyAmt", (object)DBNull.Value);
                    else
                    {
                        cmd.Parameters.AddWithValue("@subsidyAmt", applicantRow.subsidyAmt);


                    }


                    if (applicantRow.knowChannel == null || applicantRow.knowChannel.Length == 0)
                        cmd.Parameters.AddWithValue("@kChannel", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@kChannel", applicantRow.knowChannel);

                    cmd.Parameters.AddWithValue("@istatus", applicantRow.interviewStatus);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    if (applicantRow.interviewStatus == GeneralLayer.InterviewStatus.FAILED.ToString() || applicantRow.interviewStatus == GeneralLayer.InterviewStatus.PASSED.ToString())
                    {
                        // Update Interviewer Details

                        string sqlStatement = @"INSERT INTO applicant_interview_result
                                      (applicantId, interviewDate, interviewRemarks, createdOn, createdBy, interviewerId)
                                      VALUES  (@applicantId, @interviewDate, @interviewRemarks, GetDate(), @userId, @interviewerId)";

                        cmd.Parameters.AddWithValue("@applicantId", applicantId);
                        cmd.Parameters.AddWithValue("@interviewDate", applicantRow.interviewDetails.interviewedDate);
                        cmd.Parameters.AddWithValue("@interviewRemarks", applicantRow.interviewDetails.interviewRemarks == "" ? (object)DBNull.Value : applicantRow.interviewDetails.interviewRemarks);
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@interviewerId", applicantRow.interviewDetails.interviewerId);

                        cmd.CommandText = sqlStatement;
                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    if (applicantRow.selfSpon == GeneralLayer.Sponsorship.COMP.ToString())
                    {
                        cmd.CommandText = @"insert into applicant_employment_history(applicantId, companyName, companyDepartment, salaryAmount, position, employmentStatus, currentEmployment, employmentStartDate, employmentEndDate 
                        ,occupationCode, createdBy, createdOn) values (@aid, @coName, @dept, @salary, @designation, @status, @isCurr, @dtStart, @dtEnd, @type, @uid, getdate())";


                        foreach (empRec er in applicantRow.employmentRecords)
                        {
                            cmd.Parameters.AddWithValue("@aid", applicantId);
                            cmd.Parameters.AddWithValue("@coName", er.companyName);
                            cmd.Parameters.AddWithValue("@dept", er.dept);
                            cmd.Parameters.AddWithValue("@salary", er.salary);
                            cmd.Parameters.AddWithValue("@designation", er.designation);
                            cmd.Parameters.AddWithValue("@status", er.status.ToString());
                            cmd.Parameters.AddWithValue("@isCurr", er.current == true ? "Y" : "N");
                            cmd.Parameters.AddWithValue("@dtStart", er.dtStart);
                            cmd.Parameters.AddWithValue("@dtEnd", er.dtEnd);
                            cmd.Parameters.AddWithValue("@type", er.occupationType);
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }

                    trans.Commit();
                    //string traineeId = enrollTrainee(applicantId, applicantRow.exempMod, applicantRow.enrollmentDate, userId);

                    //if (traineeId == "" || traineeId == null || traineeId.Length == 0)
                    //{
                    //    applicantResult applicantR = new applicantResult()
                    //    {
                    //        applicantId = applicantId,
                    //        traineeId = "",
                    //        sno = applicantRow.sno,
                    //        name = applicantRow.Name,
                    //        msg = "Applicant: " + applicantRow.Name + " is inserted succesfully into the applicant table and was not succcessfully enrolled. Applicant ID: " + applicantId

                    //    };
                    //    return new Tuple<bool, applicantResult>(false, applicantR);
                    //}


                    applicantResult applicantResult = new applicantResult()
                    {
                        applicantId = applicantId,
                        exemMod = applicantRow.exempMod,
                        //traineeId = traineeId,
                        sno = applicantRow.sno,
                        name = applicantRow.Name,
                        msg = ""

                    };

                    return new Tuple<bool, applicantResult>(true, applicantResult);


                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "insertApplicant()", ex.Message, -1);
                    trans.Rollback();
                    applicantResult applicantR = new applicantResult()
                    {
                        applicantId = "",
                        sno = applicantRow.sno,
                        name = applicantRow.Name,
                        msg = applicantRow.sno + " - " + ex.Message

                    };
                    return new Tuple<bool, applicantResult>(false, applicantR);
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public DataTable generateSOAExcel(string traineeId)
        {
            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;

                try
                {
                    cmd.CommandText = @"select t.fullName, t.idNumber, pb.batchCode, tm.traineeid, tm.programmeBatchId, tm.moduleid, tm.batchModuleId, ms.moduleTitle, ms.modulecode, 
                                        tm.moduleResult from trainee_module tm left join programme_batch pb on tm.programmeBatchId = pb.programmeBatchId left join batch_module bm on bm.batchModuleId = 
                                        tm.batchModuleId left join module_structure ms on ms.moduleid = tm.moduleId left join trainee t on t.traineeId = tm.traineeId where tm.traineeid = @TraineeID and tm.moduleResult is null";

                    cmd.Parameters.AddWithValue("@TraineeID", traineeId);

                    DataTable dt = dbConnection.getDataTable(cmd);
                    return dt;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Data_Migration.cs", "generateSOAExcel()", ex.Message, -1);



                    return null;
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public string enrollTrainee(string applicantId, string exemptedModStr, DateTime enrollmentDate, int userId)
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
                    cmd.CommandText = @"with RN_CTE as(
	                                        select convert(int,format(@enrolledDate, 'yy')) as yr, isnull(max(convert(int,substring(traineeId, 3, 4))),0)+1 as rn,
	                                         right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int,substring(traineeId, 3, 4))),0)+1)), 4)  as rnstr from trainee
                                             where convert(int,left(traineeId, 2))=convert(int,format(@enrolledDate, 'mmyy'))
                                        )
                                        select convert(nvarchar, yr) + rnstr + 
                                        (case 
                                            when (yr+rn)%23=0 then 'A' when (yr+rn)%23=1 then 'B' when (yr+rn)%23=2 then 'C' when (yr+rn)%23=3 then 'D' when (yr+rn)%23=4 then 'E' 
                                            when (yr+rn)%23=5 then 'F' when (yr+rn)%23=6 then 'G' when (yr+rn)%23=7 then 'H' when (yr+rn)%23=8 then 'I' when (yr+rn)%23=9 then 'J' 
                                            when (yr+rn)%23=10 then 'K' when (yr+rn)%23=11 then 'L' when (yr+rn)%23=12 then 'M' when (yr+rn)%23=13 then 'N' when (yr+rn)%23=14 then 'O' 
                                            when (yr+rn)%23=15 then 'P' when (yr+rn)%23=16 then 'Q' when (yr+rn)%23=17 then 'R' when (yr+rn)%23=18 then 'S' when (yr+rn)%23=19 then 'T' 
                                            when (yr+rn)%23=20 then 'U' when (yr+rn)%23=21 then 'V' when (yr+rn)%23=22 then 'W'
                                        end) as traineeId from RN_CTE";
                    cmd.Parameters.AddWithValue("@enrolledDate", enrollmentDate);
                    object tmp = cmd.ExecuteScalar();
                    if (tmp == DBNull.Value) throw new NullReferenceException("Trainee ID is null.");
                    string traineeId = tmp as string;

                    cmd.CommandText = "insert into trainee (traineeId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode, highestEducation, highestEduRemarks, "
                        + "spokenLanguage, writtenLanguage, createdBy, createdOn) "
                        + "select @tid, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode, highestEducation, highestEduRemarks, spokenLanguage, "
                        + "writtenLanguage, @uid, @enrollmentDate from applicant where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@enrollmentDate", enrollmentDate);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update applicant_interview_result set traineeId=@tid, applicantId=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"insert into trainee_employment_history (traineeId, companyName, companyDepartment, salaryAmount, position, employmentStatus, currentEmployment, employmentStartDate, employmentEndDate, 
                                        occupationCode, occupationRemarks, createdBy, createdOn) 
                                        select @tid, companyName, companyDepartment, salaryAmount, position, employmentStatus, currentEmployment, employmentStartDate, employmentEndDate, 
                                        occupationCode, occupationRemarks, @uid, getdate() from applicant_employment_history where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from applicant_employment_history where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update payment_history set traineeId=@tid, applicantId=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "insert into trainee_programme (programmeBatchId, programmeId, traineeId, EnrolDate, programmePayableAmount, registrationFee, traineeStatus, applicationDate, selfSponsored, subsidyId, subsidyAmt, GSTPayableAmount, createdBy) "
                        + "select a.programmeBatchId, b.programmeId, @tid, getdate(), a.programmePayableAmount, a.registrationFee, '" + TraineeStatus.E.ToString() + "', a.applicationDate, a.selfSponsored, a.subsidyId, a.subsidyAmt, a.GSTPayableAmount, @uid "
                        + "from applicant a inner join programme_batch b on b.programmeBatchId=a.programmeBatchId where a.applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "insert into trainee_module(traineeId, programmeBatchId, batchModuleId, moduleId, sitInModule, SOAStatus, createdBy) "
                        + "select @tid, a.programmeBatchId, bm.batchModuleId, bm.moduleId, 'N', '" + SOAStatus.NYA.ToString() + "', @uid "
                        + "from applicant a inner join programme_batch b on b.programmeBatchId=a.programmeBatchId "
                        + "inner join batch_module bm on b.programmeBatchId=bm.programmeBatchId and bm.defunct='N' "
                        + "where a.applicantId=@aid " + (exemptedModStr == null || exemptedModStr == "" ? "" : "and bm.moduleId not in (" + exemptedModStr.Replace(";", ",") + ")");
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    if (exemptedModStr != null && exemptedModStr != "")
                    {
                        cmd.CommandText = "insert into trainee_module(traineeId, programmeBatchId, batchModuleId, moduleId, sitInModule, moduleResult, SOAStatus, createdBy) "
                        + "select @tid, a.programmeBatchId, bm.batchModuleId, bm.moduleId, 'N', '" + ModuleResult.EXEM.ToString() + "', '" + SOAStatus.NYA.ToString() + "', @uid "
                        + "from applicant a inner join programme_batch b on b.programmeBatchId=a.programmeBatchId "
                        + "inner join batch_module bm on b.programmeBatchId=bm.programmeBatchId and bm.defunct='N' "
                        + "where a.applicantId=@aid and bm.moduleId in (" + exemptedModStr.Replace(";", ",") + ")";
                        cmd.Parameters.AddWithValue("@aid", applicantId);
                        cmd.Parameters.AddWithValue("@tid", traineeId);
                        cmd.Parameters.AddWithValue("@uid", userId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from applicant where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return traineeId;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Trainee.cs", "enrollApplicant()", ex.Message, -1);

                    trans.Rollback();

                    return null;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

    }


}




