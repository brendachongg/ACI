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
    public class DB_Trainee
    {
        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();

        public DataTable getOtherLanguages(string codeType)
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                      WHERE codeType = @codeType and codeValue <> 'ENG' AND codeValue <> 'CHIN'
                                      ORDER BY codeOrder ";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", codeType);
                DataTable dtLangCodeReference = dbConnection.getDataTable(cmd);

                return dtLangCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getAllLanguageCodeReference()", ex.Message, -1);

                return null;
            }
        }


        public DataTable getCodeReferenceValues(string codeType)
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay FROM code_reference WHERE codeType = @codeType ORDER BY codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", codeType);
                DataTable dtLangPrCodeReference = dbConnection.getDataTable(cmd);

                return dtLangPrCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getCodeReferenceValues()", ex.Message, -1);

                return null;
            }
        }

        public bool updateTraineeParticular(string traineeId, string fullName, string idNumber, string identityType, string nationality,
           string gender, string contactNumber1, string contactNumber2, string emailAddress, string race, DateTime birthDate, string addressLine,
           string postalCode, string highestEducation, string highestEduRemarks, string spokenLanguage, string writtenLanguage, int updatedBy)
        {
            try
            {
                DateTime now = DateTime.Now;
                string sqlStatement = @"UPDATE trainee
                                     SET fullName = @fullName, idNumber = @idNumber, idType = @idType, nationality = @nationality,
                                     gender = @gender, contactNumber1 = @contactNumber1, contactNumber2 = @contactNumber2, emailAddress = @emailAddress,
                                     race = @race, birthDate = @birthDate, addressLine = @addressLine, postalCode = @postalCode,
                                     highestEducation = @highestEducation, highestEduRemarks = @highestEduRemarks, lastModifiedDate = @lastModifiedDate,
                                     spokenLanguage = @spokenLanguage, writtenLanguage = @writtenLanguage, lastModifiedBy = @lastModifiedBy
                                     WHERE traineeId = @traineeId";
                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@traineeId", traineeId);
                cmd.Parameters.AddWithValue("@fullName", fullName.ToUpper());
                cmd.Parameters.AddWithValue("@idNumber", idNumber);
                cmd.Parameters.AddWithValue("@idType", identityType);
                cmd.Parameters.AddWithValue("@nationality", nationality);
                cmd.Parameters.AddWithValue("@gender", gender);
                //cmd.Parameters.AddWithValue("@contactNumber1", contactNumber1);
                //cmd.Parameters.AddWithValue("@contactNumber2", contactNumber2);
                //cmd.Parameters.AddWithValue("@emailAddress", emailAddress);
                cmd.Parameters.AddWithValue("@contactNumber1", (contactNumber1.Equals("") ? (object)DBNull.Value : contactNumber1));
                cmd.Parameters.AddWithValue("@contactNumber2", (contactNumber2.Equals("") ? (object)DBNull.Value : contactNumber2));
                cmd.Parameters.AddWithValue("@emailAddress", (emailAddress.Equals("") ? (object)DBNull.Value : emailAddress));
                cmd.Parameters.AddWithValue("@race", race);
                cmd.Parameters.AddWithValue("@birthDate", birthDate);
                cmd.Parameters.AddWithValue("@addressLine", addressLine);
                cmd.Parameters.AddWithValue("@postalCode", postalCode);
                cmd.Parameters.AddWithValue("@highestEducation", highestEducation);
                cmd.Parameters.AddWithValue("@highestEduRemarks", highestEduRemarks);
                cmd.Parameters.AddWithValue("@spokenLanguage", spokenLanguage);
                cmd.Parameters.AddWithValue("@writtenLanguage", writtenLanguage);
                cmd.Parameters.AddWithValue("@lastModifiedDate", now);
                cmd.Parameters.AddWithValue("@lastModifiedBy", updatedBy);
                bool success = dbConnection.executeNonQuery(cmd);
                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "updateTraineeParticular()", ex.Message, -1);
                return false;
            }
        }

        public DataTable getTraineeDetailsByPayment(int paymentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select t.traineeId, t.fullName, tp.programmeBatchId, p.programmeId, p.programmeTitle, p.courseCode, b.projectCode, b.batchCode, 
                                    b.programmeStartDate, convert(nvarchar, b.programmeStartDate, 106) as programmeStartDateDisp, b.programmeCompletionDate, 
                                    convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDateDisp
                                    from payment_history ph inner join trainee t on ph.paymentId=@pid and ph.traineeId=t.traineeId 
                                    inner join trainee_programme tp on tp.traineeId=t.traineeId 
                                    inner join programme_structure p on p.programmeId=tp.programmeId 
                                    inner join programme_batch b on b.programmeBatchId=tp.programmeBatchId";

                cmd.Parameters.AddWithValue("@pid", paymentId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeDetailsByPayment()", ex.Message, -1);

                return null;
            }
        }

        public bool withdrawTrainee(string traineeId, int userId, string reason)
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
                    cmd.CommandText = @"update trainee_programme set traineeStatus='" + TraineeStatus.W.ToString() + "', programmeWithdrawnDate=getdate(), lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid";
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"update trainee set traineeRemarks = @withdrawreason, lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid";
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@withdrawreason", reason);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"update trainee_module set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid";
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Trainee.cs", "withdrawTrainee()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public string enrollApplicant(string applicantId, string exemptedModStr, int userId)
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
	                                        select convert(int,format(getdate(), 'yy')) as yr, isnull(max(convert(int,substring(traineeId, 3, 4))),0)+1 as rn,
	                                         right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int,substring(traineeId, 3, 4))),0)+1)), 4)  as rnstr from trainee
                                             where convert(int,left(traineeId, 2))=convert(int,format(getdate(), 'yy'))
                                        )
                                        select convert(nvarchar, yr) + rnstr + 
                                        (case 
                                            when (yr+rn)%23=0 then 'A' when (yr+rn)%23=1 then 'B' when (yr+rn)%23=2 then 'C' when (yr+rn)%23=3 then 'D' when (yr+rn)%23=4 then 'E' 
                                            when (yr+rn)%23=5 then 'F' when (yr+rn)%23=6 then 'G' when (yr+rn)%23=7 then 'H' when (yr+rn)%23=8 then 'I' when (yr+rn)%23=9 then 'J' 
                                            when (yr+rn)%23=10 then 'K' when (yr+rn)%23=11 then 'L' when (yr+rn)%23=12 then 'M' when (yr+rn)%23=13 then 'N' when (yr+rn)%23=14 then 'O' 
                                            when (yr+rn)%23=15 then 'P' when (yr+rn)%23=16 then 'Q' when (yr+rn)%23=17 then 'R' when (yr+rn)%23=18 then 'S' when (yr+rn)%23=19 then 'T' 
                                            when (yr+rn)%23=20 then 'U' when (yr+rn)%23=21 then 'V' when (yr+rn)%23=22 then 'W'
                                        end) as traineeId from RN_CTE";
                    object tmp = cmd.ExecuteScalar();
                    if (tmp == DBNull.Value) throw new NullReferenceException("Trainee ID is null.");
                    string traineeId = tmp as string;

                    cmd.CommandText = "insert into trainee (traineeId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode, highestEducation, highestEduRemarks, "
                        + "spokenLanguage, writtenLanguage, createdBy) "
                        + "select @tid, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode, highestEducation, highestEduRemarks, spokenLanguage, "
                        + "writtenLanguage, @uid from applicant where applicantId=@aid";
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);

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
                        + "where a.applicantId=@aid " + (exemptedModStr == null || exemptedModStr == "" ? "" : "and bm.moduleId not in (" + exemptedModStr + ")");
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
                        + "where a.applicantId=@aid and bm.moduleId in (" + exemptedModStr + ")";
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

        public DataTable getTraineeDetailsForPayment(string traineeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select t.fullName, t.idNumber, tp.programmeBatchId, tp.applicationDate, tp.subsidyId, tp.subsidyAmt, tp.GSTPayableAmount, tp.selfSponsored, "
                    + "b.projectCode, b.batchCode, p.programmeId, p.bundleId, p.courseCode, p.programmeTitle, tp.registrationFee, tp.programmePayableAmount, s.subsidyScheme, "
                    + "convert(nvarchar, b.programmeStartDate, 106) as programmeStartDate, convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDate, "
                    + "eh.companyName "
                    + "from trainee t inner join trainee_programme tp on t.traineeId=tp.traineeId and tp.traineeId=@tid "
                    + "inner join programme_batch b on tp.programmeBatchId=b.programmeBatchId "
                    + "inner join programme_structure p on p.programmeId=b.programmeId "
                    + "left outer join subsidy s on s.subsidyId=tp.subsidyId "
                    + "left outer join trainee_employment_history eh on eh.traineeId=t.traineeId and eh.currentEmployment='Y'";
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getTraineeDetailsForPayment()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeExemMod(string traineeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select moduleId from trainee_module where traineeId=@tid and moduleResult='" + ModuleResult.EXEM.ToString() + "' and defunct='N'";
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getTraineeExemMod()", ex.Message, -1);

                return null;
            }
        }

        //Get trainee by NRIC or Name or  Trainee ID
        public DataTable getTraineeByValue(string traineeID, string nricValue, string nameValue)
        {
            try
            {
                string sqlStatement = @"SELECT *, t.traineeID as ID
                                      FROM trainee as t
                                    
                                      LEFT OUTER JOIN
                                    
                                      trainee_programme as tp
                                      ON t.traineeId = tp.traineeId                                   
                                    
                                      LEFT OUTER JOIN 

                                      programme_structure as ps
                                      ON tp.programmeId = ps.programmeId

                                      WHERE t.traineeID LIKE @traineeID OR t.idNumber = @nricValue OR t.fullName LIKE @nameValue 
                                      ORDER BY enrolDate DESC";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@traineeID", "%" + traineeID + "%");
                cmd.Parameters.AddWithValue("@nameValue", "%" + nameValue.ToUpper() + "%");
                cmd.Parameters.AddWithValue("@nricValue", nricValue);
                DataTable dtTrainee = dbConnection.getDataTable(cmd);

                return dtTrainee;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeByValue()", ex.Message, -1);

                return null;
            }
        }

        public bool isTraineeInModule(string traineeId, int programmeBatchId, int moduleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select batchModuleId from trainee_module where traineeId=@tid and programmeBatchId=@pid and moduleId=@mid and defunct='N' and moduleResult != '" + ModuleResult.EXEM.ToString() + "'";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@pid", programmeBatchId);
                cmd.Parameters.AddWithValue("@mid", moduleId);

                return dbConnection.executeScalar(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "isTraineeInModule()", ex.Message, -1);

                return true;
            }
        }

        public DataTable searchTrainee(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select t.traineeId, t.fullName, t.idNumber, pb.batchCode, ps.programmeTitle, convert(nvarchar, tp.enrolDate, 106) as enrolDateDisp
                    , isnull(u.userName, 'N/A') as userName, isnull(convert(nvarchar, tp.enrolLetterSendOn, 106), 'N/A') as enrolLetterSendOnDisp
                    from trainee t inner join trainee_programme tp on t.traineeId=tp.traineeId 
                    inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId 
                    inner join programme_structure ps on ps.programmeId=tp.programmeId
                    left outer join aci_user u on tp.enrolLetterSendBy = u.userId
                    where traineeStatus<>'" + TraineeStatus.W.ToString() + "' " + (condition != null && condition != "" ? "and " + condition : "");

                cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "searchTrainee()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeEnrollmentLetter(string[] traineeIds)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select t.traineeId, t.fullName, t.idNumber, t.emailAddress, tp.programmeBatchId, p.programmeTitle, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp,
                    convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.enrollmentLetter
                    from trainee t inner join trainee_programme tp on t.traineeId=tp.traineeId 
                    inner join programme_structure p on p.programmeId=tp.programmeId
                    inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId 
                    where t.traineeId in (";

                int n = 0;
                foreach (string tid in traineeIds)
                {
                    cmd.CommandText += "@id" + n + ",";
                    cmd.Parameters.AddWithValue("@id" + n, tid);
                    n++;
                }

                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeEnrollmentLetter()", ex.Message, -1);

                return null;
            }
        }

        public bool addSitIn(string traineeId, int programmeBatchId, int batchModuleId, int moduleId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "insert into trainee_module(traineeId, programmeBatchId, batchModuleId, moduleId, sitInModule, createdBy) "
                    + "values(@tid, @bid, @bmid, @mid, 'Y', @uid)";

                cmd.Parameters.AddWithValue("@bid", programmeBatchId);
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@mid", moduleId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "addSitIn()", ex.Message, -1);

                return false;
            }
        }

        public bool removeSitIn(string traineeId, int batchModuleId, int userId)
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
                    cmd.CommandText = "update trainee_module set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";
                    cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update trainee_absence_record set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";
                    cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "update trainee_absence_removed set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";
                    cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Trainee.cs", "removeSitIn()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable getSitInDetails(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, tm.programmeBatchId, tm.moduleId, m.moduleCode, m.moduleTitle, left(pb.batchCode, len(pb.batchCode)-len(c1.codeValue)) as batchCode, c1.codeValue as batchType, "
                    + "c1.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, pb.programmeRegEndDate, "
                    + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c2.codeValueDisplay as classModeDisp, pb.programmeId, p.programmeCode, p.programmeVersion, "
                    + "p.programmeLevel, c3.codeValueDisplay as programmeLevelDisp, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeTitle, p.programmeType, c5.codeValueDisplay as programmeTypeDisp "

                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.traineeId=@tid and tm.batchModuleId=@bmid and tm.sitInModule='Y' and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join programme_batch pb on pb.programmeBatchId=tm.programmeBatchId "

                    + "inner join code_reference c1 on c1.codeValue=right(pb.batchCode, 5) and c1.codeType='CLTYPE' "
                    + "inner join code_reference c2 on c2.codeValue=pb.classMode and c2.codeType='CLMODE' "
                    + "inner join programme_structure p on p.programmeId=pb.programmeId inner join code_reference c3 on c3.codeValue=p.programmeLevel and c3.codeType='PGLVL' "
                    + "inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' inner join code_reference c5 on c5.codeValue=p.programmeType and c5.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getSitInDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchSitIn(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, t.idNumber, tm.batchModuleId, m.moduleId, m.moduleCode, m.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode "
                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.sitInModule='Y' and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join batch_module bm on bm.batchModuleId=tm.batchModuleId and tm.moduleId=bm.moduleId "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId inner join programme_structure p on p.programmeId=b.programmeId "
                    + (condition != null && condition != "" ? "where " + condition : "") + " order by t.fullName, m.moduleTitle";

                if (p != null) cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "searchSitIn()", ex.Message, -1);

                return null;
            }
        }

        //Retrieve all trainee
        public DataTable getListOfTrainee(int pageIndex)
        {
            try
            {
                //                string sqlStatement = @"SELECT *, ROW_NUMBER() OVER(ORDER BY t.traineeid DESC) as tRank
                //                                        FROM trainee as t
                //                                        LEFT JOIN trainee_programme as tp
                //                                        ON t.traineeId = tp.traineeId
                //                                        LEFT JOIN programme_structure as ps
                //                                        ON tp.programmeId = ps.programmeId Where traineeStatus != '" + TraineeStatus.W.ToString() +"'";

                string sqlStatement = @"Select traineeId, fullname, enrolDate, programmeTitle, tRank from (Select t.traineeId, t.fullname, tp.enrolDate, ps.programmeTitle, ROW_NUMBER() OVER(ORDER BY t.traineeid DESC) as tRank
                                        FROM trainee as t LEFT JOIN trainee_programme as tp ON t.traineeId = tp.traineeId LEFT JOIN programme_structure 
                                        as ps ON tp.programmeId = ps.programmeId  Where traineeStatus != '" + TraineeStatus.W.ToString() + "') as TraineeRanked where tRank > @pageSize * @currentPage AND tRank <= @pageSize * (@currentPage +1)";

                //where tRank > @startRowIndex AND tRank <= (@startRowIndex + @maximumRows)";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@currentPage", pageIndex);
                cmd.Parameters.AddWithValue("@pageSize", 10);
                DataTable dtAllTrainee = dbConnection.getDataTable(cmd);

                return dtAllTrainee;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getListOfTrainee()", ex.Message, -1);

                return null;
            }
        }


        public int getTotalCountOfTrainees()
        {

            try
            {
                string sqlStatement = @"SELECT *, ROW_NUMBER() OVER(ORDER BY t.traineeid DESC) as tRank
                                                        FROM trainee as t
                                                        LEFT JOIN trainee_programme as tp
                                                        ON t.traineeId = tp.traineeId
                                                        LEFT JOIN programme_structure as ps
                                                        ON tp.programmeId = ps.programmeId Where traineeStatus != '" + TraineeStatus.W.ToString() + "'";



                SqlCommand cmd = new SqlCommand(sqlStatement);

                DataTable dtAllTrainee = dbConnection.getDataTable(cmd);

                return dtAllTrainee.Rows.Count;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getListOfTrainee()", ex.Message, -1);

                return -1;
            }


        }
        //Get trainee by NRIC or Name or  Trainee ID
        public DataTable getListOfTraineeByValue(string traineeID, string nricValue, string nameValue)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                        FROM trainee as t
                                        LEFT JOIN trainee_programme as tp
                                        ON t.traineeId = tp.traineeId
                                        LEFT JOIN programme_structure as ps
                                        ON tp.programmeId = ps.programmeId
                                        left join programme_batch pb 
									    on tp.programmeBatchId = pb.programmeBatchId
                                        WHERE t.traineeId LIKE @traineeID OR t.idNumber = @nricValue OR t.fullName LIKE @nameValue OR ps.programmeTitle LIKE @nameValue OR pb.batchCode LIKE @nameValue
                                        ORDER BY enrolDate DESC";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@traineeID", "%" + traineeID + "%");
                cmd.Parameters.AddWithValue("@nameValue", "%" + nameValue + "%");
                cmd.Parameters.AddWithValue("@nricValue", nricValue);
                DataTable dtTrainee = dbConnection.getDataTable(cmd);

                return dtTrainee;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getListOfTraineeByValue()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeBriefDetails(string traineeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select t.fullName, t.idNumber, t.emailAddress, p.programmeBatchId from trainee t inner join trainee_programme p on t.traineeId=p.traineeId where t.traineeId=@tid";
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeBriefDetails()", ex.Message, -1);

                return null;
            }
        }


        public DataTable getTraineeDetailsByTraineeNRIC(string idNumber)
        {
            try
            {
                string sqlStatement = @"SELECT TOP 1 * FROM trainee t left join trainee_programme tp on t.traineeid= tp.traineeid WHERE  t.idNumber = @idNumber and tp.traineeStatus <> '" + TraineeStatus.W.ToString() + "'  order by t.createdOn desc";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@idNumber", idNumber);

                DataTable dtTrainee = dbConnection.getDataTable(cmd);

                return dtTrainee;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeDetailsByTraineeNRIC()", ex.Message, -1);

                return null;
            }
        }

        public DataSet getTraineeDetailsByTraineeId(string traineeId)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                      FROM trainee as t 
                                      LEFT OUTER JOIN trainee_programme as tp
                                      ON t.traineeId = tp.traineeId

                                      WHERE t.traineeId = @traineeId

                                      SELECT * 
                                      FROM trainee_employment_history as emph
                                      WHERE emph.traineeId = @traineeId
                                      ORDER BY emph.employmentStartDate DESC;";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@traineeId", traineeId);

                DataSet dsTraineeDetails = dbConnection.getDataSet(cmd);

                return dsTraineeDetails;

                //DataTable dtTrainee = dbConnection.getDataTable(cmd);

                //return dtTrainee;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeDetailsByTraineeId()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeProgrammeInfo(string traineeId)
        {
            try
            {
                string sqlStatement = @"SELECT * FROM  trainee_programme as tp
                                      
                                      LEFT OUTER JOIN
                                      programme_structure as ps
                                      ON tp.programmeId = ps.programmeId
                                      
                                      LEFT OUTER JOIN 
                                      programme_batch as pb
                                      ON tp.programmeBatchId = pb.programmeBatchId

                                      WHERE tp.traineeId = @traineeId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@traineeId", traineeId);
                DataTable dtTraineeProgrammeInfo = dbConnection.getDataTable(cmd);

                return dtTraineeProgrammeInfo;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "getTraineeProgrammeInfo()", ex.Message, -1);

                return null;
            }

        }

        public bool updateEnrollmentLetterSend(string[] traineeIds, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand("update trainee_programme set enrolLetterSendOn=getdate(), enrolLetterSendBy=@uid, lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId in (");
                int i = 0;
                foreach (string tid in traineeIds)
                {
                    cmd.CommandText += "@tid" + i + ",";
                    cmd.Parameters.AddWithValue("@tid" + i, tid);
                    i++;
                }
                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";

                cmd.Parameters.AddWithValue("@uid", userId);
                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Trainee.cs", "updateEnrollmentLetterSend()", ex.Message, -1);

                return false;
            }
        }

    }
}
