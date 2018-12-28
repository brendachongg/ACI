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
    public class DB_Applicant
    {
        //Initialize connection string
        private Database_Connection dbConnection = new Database_Connection();

        //private string getExtensions(string file_type)
        //{
        //    string ext = "";
        //    switch (file_type.ToUpper())
        //    {
        //        case ".JPG":
        //            ext = "image/jpg";
        //            break;
        //        case ".JPEG":
        //            ext = "image/jpeg";
        //            break;
        //        case "image/png":
        //            ext = "";
        //            break;
        //        case "application/pdf":
        //            ext = ".PDF";
        //            break;
        //        case "application/vnd.ms-word":
        //            ext = ".DOCX";
        //            break;
        //    }
        //}

        public Tuple<string, byte[]> getApplicantSignature(string applicantid)
        {
            using (SqlConnection sqlConn = dbConnection.getACIWPConnection())
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;

                cmd.CommandText = "SELECT ApplicantSignature,DocumentType FROM ApplicantSignature where applicantsid =@applid";
                cmd.Parameters.AddWithValue("@applid", applicantid);

                DataTable dt = dbConnection.getACIWPDataTable(cmd);

                if (dt != null)
                {

                    byte[] signImg = (byte[])dt.Rows[0]["ApplicantSignature"];
                    string docType = dt.Rows[0]["DocumentType"].ToString();

                    return new Tuple<string, byte[]>(docType, signImg);
                }
                else
                {
                    return new Tuple<string, byte[]>("", null);
                }

            }

        }

        //tuple -> item1: file location item2: file type (nric, wts etc) 
        public List<Tuple<string, string>> getApplicantDocuments(string applicantid)
        {
            using (SqlConnection sqlConn = dbConnection.getACIWPConnection())
            {
                sqlConn.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;

                try
                {
                    cmd.CommandText = "SELECT ApplicantIDNo,DocumentType,Documents,DocumentName,DocumentFileType,DocumentExtensions, DocumentFilePath FROM ApplicantsDocuments where ApplicantIDNo = @applicantId";
                    cmd.Parameters.AddWithValue("@applicantId", applicantid);
                    DataTable dt = dbConnection.getACIWPDataTable(cmd);

                    List<Tuple<string, string>> listDocuments = new List<Tuple<string, string>>();


                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["Documents"] == (object)DBNull.Value)
                            listDocuments.Add(new Tuple<string, string>(dr["DocumentFilePath"].ToString(), dr["DocumentType"].ToString()));
                        else
                        {
                            byte[] document = (byte[])dr["Documents"];
                            string file = "\\ApplicantsDocuments\\" + applicantid + "_" + dr["DocumentType"].ToString() + dr["DocumentExtensions"].ToString();
                            string ToSaveFileTo = System.Web.HttpContext.Current.Server.MapPath("~\\ApplicantsDocuments\\" + applicantid + "_" + dr["DocumentType"].ToString() + dr["DocumentExtensions"].ToString());

                            System.IO.FileStream fs = new System.IO.FileStream(ToSaveFileTo, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite);
                            System.IO.BinaryWriter bw = new System.IO.BinaryWriter(fs);
                            bw.Write(document);
                            bw.Close();

                            cmd.Parameters.Clear();
                            cmd = new SqlCommand();
                            cmd.Connection = sqlConn;

                            cmd.CommandText = "Update ApplicantsDocuments set Documents = NULL, DocumentFilePath=@dFilePath where applicantidno = @applicantid and DocumentType = @docType";
                            cmd.Parameters.AddWithValue("@applicantid", applicantid);
                            cmd.Parameters.AddWithValue("@dFilePath", file);
                            cmd.Parameters.AddWithValue("@docType", dr["DocumentType"].ToString());
                            cmd.ExecuteNonQuery();
                            listDocuments.Add(new Tuple<string, string>(file, dr["DocumentType"].ToString()));
                        }

                    }
                    return listDocuments;


                }


                catch (Exception ex)
                {
                    List<Tuple<string, string>> lsEx = new List<Tuple<string, string>>();
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Applicant", "getApplicantDocuments()", ex.ToString(), -1, false);

                    lsEx.Add(new Tuple<string, string>("", ""));
                    return lsEx;
                }
            }
        }


        public string registerApplicantFull(int programmeBatchId, InterviewStatus interview, Sponsorship spon, string fullName, IDType idType, string id, string nationality, string race, string gender, DateTime dtDOB
            , string email, string contact1, string contact2, string addr, string postal, LanguageProficiency[] lang, string highEdulvl, EmploymentRecord[] empl, GetToKnowChannel[] knowChannel, int userId, bool isOnline = false, string preferredModeOfPayment = "")
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
                    cmd.CommandText = @"select format(getdate(), 'yyMM')+right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int, case when isnumeric(right(applicantId, 4))= 0 then '0' else right(applicantId, 4) end)),0)+1)), 4) as id
                            from applicant where left(applicantId, 4)=format(getdate(), 'yyMM')";
                    string appId = (string)cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @";with PROG_FEES_CTE as (
                            select bundleCost
                            from bundle b inner join programme_structure p on b.bundleId=p.bundleId
                            inner join programme_batch pb on pb.programmeId=p.programmeId and pb.programmeBatchId=@pbid
                        )
                        ,BLACKLIST_CTE as (
                            select case when cnt=0 then 'N' else 'Y' end as status
                            from (select count(*) as cnt from aci_suspended_list where idNumber=@id and getdate() between startDate and endDate) t
                        )
                        insert into applicant(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2, emailAddress, race, birthDate, addressLine, postalCode
                        , highestEducation, spokenLanguage, writtenLanguage, applicationDate, selfSponsored, programmeBatchId, programmePayableAmount, interviewStatus, getToKnowChannel 
                        , shortlistStatus, blacklistStatus, applicationStatus, rejectStatus, createdBy, createdOn, isOnlineApplicant, preferredModeOfPayment) 
                        select @aid, @fname, @id, @idType, @nation, @gender, @contact1, @contact2, @email, @race, @dob, @addr, @postal, @highEdu, @spokeLang, @writeLang, getdate(), @spon, @pbid
                        , p.bundleCost, @iStatus, @kChannel
                        , 'Y', b.status, '" + ApplicantStatus.NEW.ToString() + @"', 'N', @uid, getdate(), @isonline, @paymentmode
                        from PROG_FEES_CTE p cross join BLACKLIST_CTE b";

                    cmd.Parameters.AddWithValue("@aid", appId);
                    cmd.Parameters.AddWithValue("@fname", fullName.ToUpper());
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@idType", ((int)idType).ToString());
                    cmd.Parameters.AddWithValue("@nation", nationality);
                    cmd.Parameters.AddWithValue("@gender", gender);
                    cmd.Parameters.AddWithValue("@contact1", contact1);
                    cmd.Parameters.AddWithValue("@contact2", contact2 == null ? (object)DBNull.Value : contact2);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@race", race);
                    cmd.Parameters.AddWithValue("@dob", dtDOB);
                    cmd.Parameters.AddWithValue("@addr", addr);
                    cmd.Parameters.AddWithValue("@postal", postal);
                    cmd.Parameters.AddWithValue("@highEdu", highEdulvl);
                    cmd.Parameters.AddWithValue("@isonline", isOnline == false ? General_Constance.STATUS_NO.ToString() : General_Constance.STATUS_YES.ToString());
                    cmd.Parameters.AddWithValue("@paymentmode", preferredModeOfPayment == "" ? (object)DBNull.Value : preferredModeOfPayment);

                    if (lang == null || lang.Length == 0)
                    {
                        cmd.Parameters.AddWithValue("@spokeLang", DBNull.Value);
                        cmd.Parameters.AddWithValue("@writeLang", DBNull.Value);
                    }
                    else
                    {
                        string spoken = "", written = "";
                        foreach (LanguageProficiency lp in lang)
                        {
                            spoken += lp.lang + ":" + ((int)lp.spoken).ToString() + ";";
                            written += lp.lang + ":" + ((int)lp.written).ToString() + ";";
                        }
                        cmd.Parameters.AddWithValue("@spokeLang", spoken);
                        cmd.Parameters.AddWithValue("@writeLang", written);
                    }

                    cmd.Parameters.AddWithValue("@spon", spon.ToString());
                    cmd.Parameters.AddWithValue("@pbid", programmeBatchId);

                    if (knowChannel == null || knowChannel.Length == 0)
                        cmd.Parameters.AddWithValue("@kChannel", DBNull.Value);
                    else
                    {
                        string ch = "";
                        foreach (GetToKnowChannel kn in knowChannel) ch += kn.ToString() + ",";
                        cmd.Parameters.AddWithValue("@kChannel", ch);
                    }

                    cmd.Parameters.AddWithValue("@istatus", interview.ToString());
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"insert into applicant_employment_history(applicantId, companyName, companyDepartment, salaryAmount, position, employmentStatus, currentEmployment, employmentStartDate, employmentEndDate 
                        ,occupationCode, createdBy, createdOn) values (@aid, @coName, @dept, @salary, @designation, @status, @isCurr, @dtStart, @dtEnd, @type, @uid, getdate())";

                    if (empl != null)
                    {
                        foreach (EmploymentRecord er in empl)
                        {
                            cmd.Parameters.AddWithValue("@aid", appId);
                            cmd.Parameters.AddWithValue("@coName", er.companyName);
                            cmd.Parameters.AddWithValue("@dept", er.dept);
                            cmd.Parameters.AddWithValue("@salary", er.salary);
                            cmd.Parameters.AddWithValue("@designation", er.designation);
                            cmd.Parameters.AddWithValue("@status", er.status.ToString());
                            cmd.Parameters.AddWithValue("@isCurr", er.dtEnd == DateTime.MaxValue ? "Y" : "N");
                            cmd.Parameters.AddWithValue("@dtStart", er.dtStart);
                            cmd.Parameters.AddWithValue("@dtEnd", er.dtEnd == DateTime.MaxValue ? (object)DBNull.Value : er.dtEnd);
                            cmd.Parameters.AddWithValue("@type", ((int)er.occupationType).ToString("D2"));
                            cmd.Parameters.AddWithValue("@uid", userId);

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                    }

                    trans.Commit();

                    return appId;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Applicant.cs", "registerApplicantFull()", ex.Message, -1);

                    trans.Rollback();

                    return null;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool registerApplicantQuick(string name, DateTime dtDOB, string id, IDType idType, Sponsorship spon, int programmeBatchId, InterviewStatus interview, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @";with APP_ID_CTE as (
                        select format(getdate(), 'yyMM')+right('0000'+ rtrim(convert(nvarchar,isnull(max(convert(int, case when isnumeric(right(applicantId, 4))= 0 then '0' else right(applicantId, 4) end)),0)+1)), 4) as id
                        from applicant where left(applicantId, 4)=format(getdate(), 'yyMM')
                    )
                    ,PROG_FEES_CTE as (
                        select bundleCost
                        from bundle b inner join programme_structure p on b.bundleId=p.bundleId
                        inner join programme_batch pb on pb.programmeId=p.programmeId and pb.programmeBatchId=@pbid
                    )
                    ,BLACKLIST_CTE as (
                        select case when cnt=0 then 'N' else 'Y' end as status
                        from (select count(*) as cnt from aci_suspended_list where idNumber=@id and getdate() between startDate and endDate) t
                    )
                    insert into applicant(applicantId, fullName, idNumber, idType, birthDate, applicationDate, selfSponsored, programmeBatchId, programmePayableAmount, interviewStatus, shortlistStatus, blacklistStatus, applicationStatus
                    , rejectStatus, createdBy, createdOn) 
                    select a.id, @fname, @id, @idType, @dob, getdate(), @spon, @pbid, p.bundleCost, @istatus, 'Y', b.status, '" + ApplicantStatus.NEW.ToString() + @"'
                    , 'N', @uid, getdate() 
                    from APP_ID_CTE a cross join PROG_FEES_CTE p cross join BLACKLIST_CTE b";

                cmd.Parameters.AddWithValue("@fname", name.ToUpper());
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@idType", ((int)idType).ToString());
                cmd.Parameters.AddWithValue("@dob", dtDOB);
                cmd.Parameters.AddWithValue("@spon", spon.ToString());
                cmd.Parameters.AddWithValue("@pbid", programmeBatchId);
                cmd.Parameters.AddWithValue("@istatus", interview.ToString());
                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.executeNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "registerApplicantQuick()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getSponsorship()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='SPON' and defunct='N' order by codeOrder";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getSponsorship()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getEmploymentStatus()
        {
            try
            {
                //SqlCommand cmd = new SqlCommand();
                string sqlStatement = "select codeValue, codeValueDisplay from code_reference where codeType='EMSTAT' and defunct='N' order by codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtEmpStatus = dbConnection.getDataTable(cmd);

                return dtEmpStatus;

                //return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getEmploymentStatus()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getEmploymentJob()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='EMJOB' and defunct='N' order by codeOrder";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getEmploymentJob()", ex.Message, -1);

                return null;
            }
        }

        //Get identification code reference
        public DataTable getIdentificationTypeCodeReference()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                      WHERE codeType = 'IDTYPE'
                                      ORDER BY codeOrder");

                DataTable dtCodeReference = dbConnection.getDataTable(cmd);

                return dtCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getIdentificationTypeCodeReference()", ex.Message, -1);

                return null;
            }
        }

        //Get nationality code reference
        public DataTable getNationalityCodeReference()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT codeValue, codeValueDisplay
                                      FROM code_reference                      
                                      WHERE codeType = 'NATION'
                                      ORDER BY codeOrder");

                DataTable dtCodeReference = dbConnection.getDataTable(cmd);

                return dtCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getNationalityCodeReference()", ex.Message, -1);

                return null;
            }
        }

        //Get race code reference
        public DataTable getRaceCodeReference()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                      WHERE codeType = 'RACE'
                                      ORDER BY codeOrder");

                DataTable dtCodeReference = dbConnection.getDataTable(cmd);

                return dtCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getRaceCodeReference()", ex.Message, -1);

                return null;
            }
        }

        //Get occupation code reference
        public DataTable getOccupationCodeReference()
        {
            try
            {
                SqlCommand cmd = new SqlCommand(@"SELECT codeValue, codeValueDisplay
                                      FROM code_reference                          
                                      WHERE codeType = 'EMJOB'
                                      ORDER BY codeOrder");

                DataTable dtCodeReference = dbConnection.getDataTable(cmd);

                return dtCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getOccupationCodeReference()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getApplicantProgBatchDetails(string applicantId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select a.programmeBatchId, a.applicantExemModule, b.programmeId, p.programmeType, p.programmeTitle, a.interviewStatus, b.programmeStartDate "
                    + "from applicant a inner join programme_batch b on a.programmeBatchId=b.programmeBatchId "
                    + "inner join programme_structure p on p.programmeId=b.programmeId "
                    + "where a.applicantId=@aid";
                cmd.Parameters.AddWithValue("@aid", applicantId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getApplicantBatchDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getCodeValueDisplay(string codeValue, string codeType)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codevaluedisplay from code_reference where codeType = @CodeType and codeValue = @codeValue";
                cmd.Parameters.AddWithValue("@CodeType", codeType);
                cmd.Parameters.AddWithValue("@codeValue", codeValue);
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getCodeValueDisplay()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getApplicantDetailsForPayment(string applicantId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select a.fullName, a.idNumber, a.programmeBatchId, a.applicationDate, a.subsidyId, a.subsidyAmt, a.GSTPayableAmount, a.selfSponsored, a.applicantExemModule, "
                    + "b.projectCode, b.batchCode, p.programmeId, p.bundleId, p.courseCode, p.programmeTitle, a.registrationFee, a.programmePayableAmount, s.subsidyScheme, "
                    + "convert(nvarchar, b.programmeStartDate, 106) as programmeStartDate, convert(nvarchar, b.programmeCompletionDate, 106) as programmeCompletionDate, "
                    + "eh.companyName "
                    + "from applicant a inner join programme_batch b on a.programmeBatchId=b.programmeBatchId and a.applicantId=@aid "
                    + "inner join programme_structure p on p.programmeId=b.programmeId "
                    + "left outer join subsidy s on s.subsidyId=a.subsidyId "
                    + "left outer join applicant_employment_history eh on eh.applicantId=a.applicantId and eh.currentEmployment='Y'";
                cmd.Parameters.AddWithValue("@aid", applicantId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getApplicantDetailsForPayment()", ex.Message, -1);

                return null;
            }
        }

        public bool updateApplicantStatusReject(string applicantId, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;
                string sqlStatement = @"UPDATE [applicant] SET rejectStatus = @rejectStatus, lastModifiedDate = GetDate(), lastModifiedBy = @lastModifiedBy
                                      WHERE applicantId = @applicantId";
                SqlCommand cmd = new SqlCommand(sqlStatement);


                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@rejectStatus", General_Constance.STATUS_YES);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "updateApplicantStatusReject()", ex.Message, -1);

                return false;
            }
        }
        public DataTable getAllEducationCodeReference()
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                    
                                      WHERE codeType = @codeType
                                      ORDER BY codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", "EDU");
                DataTable dtEducationCodeReference = dbConnection.getDataTable(cmd);

                return dtEducationCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getAllEducationCodeReference()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllLanguageProficiencyCodeReference()
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                    
                                      WHERE codeType = @codeType
                                      ORDER BY codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", "LANGPR");
                DataTable dtLangPrCodeReference = dbConnection.getDataTable(cmd);

                return dtLangPrCodeReference;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getAllLanguageProficiencyCodeReference()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInterviewStatus()
        {
            try
            {
                string sql = "Select codevalue, codevaluedisplay from code_reference where codetype = @codeType order by codeorder";
                SqlCommand cmd = new SqlCommand(sql);
                cmd.Parameters.AddWithValue("@codeType", "VWSTAT");
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getInterviewStatus()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllLanguageCodeReference()
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                    
                                      WHERE codeType = @codeType
                                      ORDER BY codeOrder ";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", "LANG");
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

        public DataTable getOtherLanguages()
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                      WHERE codeType = @codeType and codeValue <> 'ENG' AND codeValue <> 'CHIN'
                                      ORDER BY codeOrder ";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", "LANG");
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

        public DataTable getAllGetToKnowChannelCodeReference()
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay
                                      FROM code_reference       
                                    
                                      WHERE codeType = @codeType
                                      ORDER BY codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", "KNOWN");
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

        public bool checkIfApplicantDetailsIsUpdated(string applicantId)
        {
            try
            {
                string sql = @"select count(1) from applicant
                            where (idtype is null or nationality is null 
                            or gender is null or race is null 
                            or contactNumber1 is null
                            or addressLine is null or postalcode is null)
                            and applicantid = @applicantId";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                if (dbConnection.executeScalarInt(cmd) > 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "checkIfApplicantsDetailsIsUpdate()", ex.Message, -1);

                return false;
            }
        }

        //Get application by NRIC or name or applicantId
        public DataTable getApplicantByValue(string nricValue, string nameValue, string applicantId)
        {
            try
            {
                string sqlStatement = @"SELECT *, applicant.applicantId as ID
                                      FROM applicant as applicant
                                    
                                      LEFT OUTER JOIN

                                      programme_batch as pb
                                      ON applicant.programmeBatchId = pb.programmeBatchId

                                      LEFT OUTER JOIN
                                    
                                      programme_structure as ps
                                      ON pb.programmeId= ps.programmeId                                    
                                    
                                      WHERE applicant.idNumber LIKE @nricValue OR applicant.fullName LIKE @nameValue OR applicant.applicantId LIKE @applicantId
                                      ORDER BY applicant.applicationDate DESC";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@rejectStatus", General_Constance.STATUS_NO);
                cmd.Parameters.AddWithValue("@nameValue", "%" + nameValue.ToUpper() + "%");
                cmd.Parameters.AddWithValue("@applicantId", "%" + applicantId + "%");
                cmd.Parameters.AddWithValue("@nricValue", nricValue);
                DataTable dtApplication = dbConnection.getDataTable(cmd);

                return dtApplication;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getApplicantByValue()", ex.Message, -1);

                return null;
            }
        }

        public int getUnprocessedApplicationCount()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT COUNT(*) FROM [applicant] as applicant WHERE rejectStatus = 'N'";

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getUnprocessedApplicationCount()", ex.Message, -1);

                return -1;
            }
        }

        //Get all unprocessed application 
        public DataTable getListOfApplicant()
        {
            try
            {
                DateTime now = DateTime.Now.Date;

                string sqlStatement = @"SELECT *
                                        FROM applicant as a
                                    
                                        LEFT OUTER JOIN
                                    
                                        programme_batch as pb
                                        ON a.programmeBatchId = pb.programmeBatchId                                    
                                        
                                        LEFT OUTER JOIN
                                        
                                        programme_structure as ps
                                        ON pb.programmeId = ps.programmeId

                                        WHERE a.rejectStatus = @rejectStatus
                                        ORDER BY a.applicationDate DESC";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@rejectStatus", General_Constance.STATUS_NO);
                DataTable dtAllApplication = dbConnection.getDataTable(cmd);

                return dtAllApplication;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getListOfApplicant()", ex.Message, -1);

                return null;
            }
        }

        //Get application details by applicantId
        public DataSet getApplicationDetailsByApplicantId(string applicantId)
        {
            try
            {
                DateTime now = DateTime.Now.Date;

                string sqlStatement = @"SELECT TOP 1 *
                                      FROM applicant as applicant                                                                                                           
                                    
                                      LEFT OUTER JOIN

                                      programme_batch as pb

                                      ON applicant.programmeBatchId = pb.programmeBatchId

                                      LEFT OUTER JOIN
                                    
                                      programme_structure as ps
                                      ON pb.programmeId= ps.programmeId  

                                      WHERE applicant.applicantId = @applicantId;
                    
                                      SELECT * 
                                      FROM applicant_employment_history as emph
                                      WHERE emph.applicantId = @applicantId
                                      ORDER BY emph.employmentStartDate DESC;
                                        
                                      SELECT interview.interviewDate, interview.interviewerId, interview.interviewRemarks, 
                                      a.shortlistStatus, a.interviewStatus FROM applicant_interview_result interview 
                                      inner join applicant a on interview.applicantId = a.applicantId 
                                      WHERE interview.applicantId = @applicantId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                DataSet dsAllApplicationDetails = dbConnection.getDataSet(cmd);

                return dsAllApplicationDetails;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getApplicationDetailsByApplicantId()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getApplicantExemptedModule(string applicantId)
        {
            try
            {
                DateTime now = DateTime.Now.Date;

                string sqlStatement = @"SELECT *

                                      FROM applicant as applicant

                                      WHERE applicant.applicantId = @applicantId;";


                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                DataTable dsAllApplicationDetails = dbConnection.getDataTable(cmd);

                return dsAllApplicationDetails;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getApplicantExemptedModule()", ex.Message, -1);

                return null;
            }
        }

        //Get application by NRIC or name
        public DataTable getListOfApplicantByValue(string nricValue, string nameValue)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                      FROM applicant as applicant
                                    
                                      LEFT OUTER JOIN

                                      programme_batch as pb
                                      ON applicant.programmeBatchId = pb.programmeBatchId

                                      LEFT OUTER JOIN
                                    
                                      programme_structure as ps
                                      ON pb.programmeId= ps.programmeId                                    
                                    
                                      WHERE applicant.rejectStatus = @rejectStatus AND (applicant.idNumber LIKE @nricValue OR applicant.fullName LIKE @nameValue OR ps.programmeTitle LIKE @nameValue OR pb.batchcode LIKE @nameValue) 
                                      ORDER BY applicant.applicationDate DESC";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@rejectStatus", General_Constance.STATUS_NO);
                cmd.Parameters.AddWithValue("@nameValue", "%" + nameValue.ToUpper() + "%");
                cmd.Parameters.AddWithValue("@nricValue", nricValue);
                DataTable dtApplication = dbConnection.getDataTable(cmd);

                return dtApplication;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "getListOfApplicantByValue()", ex.Message, -1);

                return null;
            }
        }

        //Update applicant's particular
        public bool updateApplicantParticular(string applicantId, string fullName, string idNumber, string identityType, string nationality,
            string gender, string contactNumber1, string contactNumber2, string emailAddress, string race, DateTime birthDate, string addressLine,
            string postalCode, string highestEducation, string highestEduRemarks, string spokenLanguage, string writtenLanguage,
            string channel, string sponsorship, int updatedBy, string applicantRemarks)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"UPDATE applicant
                                      SET fullName = @fullName, idNumber = @idNumber, idType = @idType, nationality = @nationality,
                                      gender = @gender, contactNumber1 = @contactNumber1, contactNumber2 = @contactNumber2, emailAddress = @emailAddress,
                                      race = @race, birthDate = @birthDate, addressLine = @addressLine, postalCode = @postalCode, 
                                      highestEducation = @highestEducation, highestEduRemarks = @highestEduRemarks, lastModifiedDate = @lastModifiedDate,
                                      spokenLanguage = @spokenLanguage, writtenLanguage = @writtenLanguage, getToKnowChannel = @getToKnowChannel, selfSponsored = @sponsorship, lastModifiedBy = @lastModifiedBy, applicantRemarks = @applicantRemarks 
                                      WHERE applicantId = @applicantId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@fullName", fullName.ToUpper());
                cmd.Parameters.AddWithValue("@idNumber", idNumber);
                cmd.Parameters.AddWithValue("@idType", identityType);
                cmd.Parameters.AddWithValue("@nationality", nationality);
                cmd.Parameters.AddWithValue("@gender", gender);
                cmd.Parameters.AddWithValue("@contactNumber1", (contactNumber1.Equals("") ? (object)DBNull.Value : contactNumber1));
                cmd.Parameters.AddWithValue("@contactNumber2", (contactNumber2.Equals("") ? (object)DBNull.Value : contactNumber2));
                cmd.Parameters.AddWithValue("@emailAddress", (emailAddress.Equals("") ? (object)DBNull.Value : emailAddress));
                cmd.Parameters.AddWithValue("@race", race);
                cmd.Parameters.AddWithValue("@birthDate", birthDate);
                cmd.Parameters.AddWithValue("@addressLine", addressLine);
                cmd.Parameters.AddWithValue("@postalCode", postalCode);

                cmd.Parameters.AddWithValue("@highestEducation", highestEducation);
                cmd.Parameters.AddWithValue("@highestEduRemarks", (highestEduRemarks.Equals("") ? (object)DBNull.Value : highestEduRemarks));

                cmd.Parameters.AddWithValue("@spokenLanguage", spokenLanguage);
                cmd.Parameters.AddWithValue("@writtenLanguage", writtenLanguage);

                cmd.Parameters.AddWithValue("@getToKnowChannel", (channel.Equals("") ? (object)DBNull.Value : channel));

                cmd.Parameters.AddWithValue("@sponsorship", sponsorship);

                cmd.Parameters.AddWithValue("@lastModifiedDate", now);
                cmd.Parameters.AddWithValue("@lastModifiedBy", updatedBy);
                cmd.Parameters.AddWithValue("@applicantRemarks", (applicantRemarks.Equals("") ? (object)DBNull.Value : applicantRemarks));

                bool success = dbConnection.executeNonQuery(cmd);

                return success;

            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "updateApplicantParticular()", ex.Message, -1);

                return false;
            }
        }

        //Update applicant's employment details by applicantId and employmentHistoryId
        public bool updateApplicantEmployment(string applicantId, int employmentHistoryId, string companyName,
                string position, DateTime employmentStartDate, DateTime employmentEndDate, decimal salaryAmount, string currentEmployment, string employmentStatus, int lastModifiedBy, string occupationCode, string companyDept)
        {

            try
            {
                DateTime now = DateTime.Now;
                //, occupationRemarks = @occupationRemarks

                string sqlStatement = @"UPDATE applicant_employment_history

                                      SET companyName = @companyName, salaryAmount = @salaryAmount, position = @position,
                                      currentEmployment = @currentEmployment, employmentStatus = @employmentStatus,
                                      employmentStartDate = @employmentStartDate, employmentEndDate = @employmentEndDate,
                                      lastModifiedDate = getdate(), lastModifiedBy = @lastModifiedBy, occupationCode = @occupationCode,
                                      companyDepartment = @companyDepartment
                                      WHERE applicantId = @applicantId AND employmentHistoryId = @employmentHistoryId;
                                      
                                      IF @@ROWCOUNT=0
                                      INSERT INTO applicant_employment_history
                                      
                                      (applicantId, companyName, salaryAmount, position, employmentStatus, currentEmployment,
                                      employmentStartDate, employmentEndDate, createdBy, createdOn, occupationCode, companyDepartment)
                                      VALUES(@applicantId, @companyName, @salaryAmount, @position, @employmentStatus, @currentEmployment,
                                      @employmentStartDate, @employmentEndDate, @createdBy, getdate(), @occupationCode, @companyDepartment)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@employmentHistoryId", employmentHistoryId);
                cmd.Parameters.AddWithValue("@companyName", companyName);
                cmd.Parameters.AddWithValue("@salaryAmount", salaryAmount);
                cmd.Parameters.AddWithValue("@position", position);
                cmd.Parameters.AddWithValue("@occupationCode", occupationCode);
                cmd.Parameters.AddWithValue("@companyDepartment", companyDept);
                //if (occupationRemarks.Equals(""))
                //{
                //    cmd.Parameters.AddWithValue("@occupationRemarks", DBNull.Value);
                //}
                //else
                //{
                //    cmd.Parameters.AddWithValue("@occupationRemarks", occupationRemarks);
                //}

                cmd.Parameters.AddWithValue("@currentEmployment", currentEmployment);
                cmd.Parameters.AddWithValue("@employmentStatus", employmentStatus);
                cmd.Parameters.AddWithValue("@employmentStartDate", employmentStartDate);
                cmd.Parameters.AddWithValue("@employmentEndDate", employmentEndDate);

                cmd.Parameters.AddWithValue("@lastModifiedBy", lastModifiedBy);
                cmd.Parameters.AddWithValue("@createdBy", lastModifiedBy);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;

            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "updateApplicantEmployment()", ex.Message, -1);

                return false;
            }
        }

        //Update applicant's remarks
        public bool updateApplicantRemarks(string applicantId, string applicantRemarks, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"UPDATE [applicant]

                                     SET applicantRemarks = @applicantRemarks, lastModifiedDate = @lastModifiedDate,
                                     lastModifiedBy = @lastModifiedBy
                                     WHERE applicantId = @applicantId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@applicantRemarks", applicantRemarks);

                cmd.Parameters.AddWithValue("@lastModifiedDate", now);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant", "updateApplicantRemarks()", ex.Message, -1);

                return false;
            }
        }

        //Change applicant's course and project codes
        public bool updateApplicantCourseProjectCode(string applicantId, string programmBatchId)
        {
            try
            {
                DateTime now = DateTime.Now;




                string sqlStatement = @"
                with PROG_FEES_CTE as (
                            select bundleCost
                            from bundle b inner join programme_structure p on b.bundleId=p.bundleId
                            inner join programme_batch pb on pb.programmeId=p.programmeId and pb.programmeBatchId=@programmBatchId
                        )
						
						UPDATE [applicant]

                        SET programmePayableAmount = (select bundlecost from PROG_FEES_CTE), applicantExemModule = NULL, programmeBatchId = @programmBatchId, lastModifiedDate = getdate()
                                      
                                      WHERE applicantId = @applicantId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@programmBatchId", programmBatchId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant", "updateApplicantCourseProjectCode()", ex.Message, -1);

                return false;
            }
        }

        //update applicant's interview status and schedule
        public bool updateApplicantInterviewDetails(string applicantId, int interviewerId, string interviewStatus, string shortlistStatus, DateTime interviewDate, string interviewRemarks, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"UPDATE applicant

                                      SET interviewStatus = @interviewStatus, shortlistStatus = @shortlistStatus, lastModifiedDate = GetDate(), lastModifiedBy= @userId

                                      WHERE applicantId = @applicantId;

                                      IF EXISTS (SELECT * FROM applicant_interview_result WHERE applicantId = @applicantId)
                                            UPDATE applicant_interview_result SET interviewDate = @interviewDate, interviewRemarks = @interviewRemarks, lastModifiedBy = @userId,
                                            lastModifiedDate = GetDate(), interviewerId = @interviewerId;

                                      ELSE 
                                            INSERT INTO applicant_interview_result
                                      (applicantId, interviewDate, interviewRemarks, createdOn, createdBy, interviewerId)
                                      VALUES  (@applicantId, @interviewDate, @interviewRemarks, GetDate(), @userId, @interviewerId)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@interviewStatus", interviewStatus);

                if (interviewStatus == GeneralLayer.InterviewStatus.NREQ.ToString() || interviewStatus == GeneralLayer.InterviewStatus.PD.ToString() || interviewStatus == GeneralLayer.InterviewStatus.NYD.ToString())
                    cmd.Parameters.AddWithValue("@interviewDate", SqlDateTime.Null);
                else
                    cmd.Parameters.AddWithValue("@interviewDate", interviewDate);

                cmd.Parameters.AddWithValue("@shortlistStatus", (shortlistStatus == "") ? (object)DBNull.Value : shortlistStatus);
                cmd.Parameters.AddWithValue("@interviewRemarks", (interviewRemarks == "") ? (object)DBNull.Value : interviewRemarks);
                cmd.Parameters.AddWithValue("@userId", userId);
                cmd.Parameters.AddWithValue("@interviewerId", (interviewerId == 0) ? (object)DBNull.Value : interviewerId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "updateApplicantInterviewDetails()", ex.Message, -1);

                return false;
            }
        }

        //Update module exemption
        public bool updateApplicantModuleExemption(string applicantId, string exemptedModule, decimal programmePayableAmount, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"UPDATE [applicant]

                                      SET applicantExemModule = @applicantExemptedModule, programmePayableAmount = @programmePayableAmount, 
                                      lastModifiedDate = @lastModifiedDate, lastModifiedBy = @lastModifiedBy
                                      
                                      WHERE applicantId = @applicantId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@applicantExemptedModule", exemptedModule);
                cmd.Parameters.AddWithValue("@programmePayableAmount", programmePayableAmount);

                cmd.Parameters.AddWithValue("@lastModifiedDate", now);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Applicant.cs", "updateApplicantModuleExemption()", ex.Message, -1);

                return false;
            }
        }

    }
}
