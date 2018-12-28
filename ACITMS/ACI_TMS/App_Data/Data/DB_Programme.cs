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
    public class DB_Programme
    {
        //Initialize connection string
        private Database_Connection dbConnection = new Database_Connection();

        public DataTable getProgrammeById(string pid)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select programmeId, programmeCode,programmeVersion, programmeLevel, programmeCategory, programmeTitle, programmeDescription, courseCode,  
                                    programmeType, bundleid from programme_structure where programmeId=@pid and defunct = '" + General_Constance.STATUS_NO + "'";

                cmd.Parameters.AddWithValue("@pid", pid);
                DataTable dtProgrammes = dbConnection.getDataTable(cmd);

                return dtProgrammes;


            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAllAvailableProgrammes()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getProgrammesModules(string bundleid)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from module_structure m left join bundle_module bm on m.moduleid = bm.moduleid where bm.bundleid = @bundleid order by bm.moduleOrder";
                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@bundleid", bundleid);
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getProgrammesModules()", ex.Message, -1);

                return null;
            }
        }

        //Get all programmes which are not defunct
        public DataTable getAllAvailableProgrammes()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select programmeId, programmeCode,programmeVersion, programmeLevel, programmeCategory, programmeTitle, programmeDescription, courseCode,  
                                    programmeType, bundleid from programme_structure where defunct = '" + General_Constance.STATUS_NO + "'";
      

                DataTable dtProgrammes = dbConnection.getDataTable(cmd);

                return dtProgrammes;


            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAllAvailableProgrammes()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllAvailableProgrammesWS(string progCat)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                string sql = @" select programmeId, programmeCode,programmeVersion, programmeLevel, programmeCategory, programmeTitle, programmeDescription, courseCode,  
                                    cr.codevaluedisplay as programmeType, bundleid,isnull(lastModifiedDate, createdOn) as lastModifiedTime from programme_structure ps left join code_reference cr on ps.programmeType = cr.codevalue and cr.codetype = 'PGTYPE' where";

                if(progCat != "")
                    sql += " ps.programmeCategory = @ProgCat AND";

                sql+= " ps.defunct = '" + General_Constance.STATUS_NO + "' order by programmeType, programmeTitle, lastModifiedTime";

                cmd.CommandText = sql;

                if (progCat != "")
                    cmd.Parameters.AddWithValue("@ProgCat", progCat);


                DataTable dtProgrammes = dbConnection.getDataTable(cmd);

                return dtProgrammes;


            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAllAvailableProgrammes()", ex.Message, -1);

                return null;
            }
        }
        public string getEnrollmentTemplate(int progId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select enrollmentTemplate from programme_structure where programmeId=@id";
                cmd.Parameters.AddWithValue("@id", progId);

                return dbConnection.executeScalarString(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getEnrollmentTemplate()", ex.Message, -1);

                return null;
            }
        }


        public bool updateEnrollmentTemplate(int progId, string template)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update programme_structure set enrollmentTemplate=@template where programmeId=@id";
                cmd.Parameters.AddWithValue("@id", progId);
                cmd.Parameters.AddWithValue("@template", template);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "updateEnrollmentTemplate()", ex.Message, -1);

                return false;
            }
        }

        public DataTable searchProgramme(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT programmeId, programmeCode, programmeTitle, programmeVersion, courseCode from programme_structure where defunct='N' and " + condition + " order by programmeTitle, programmeCode, programmeVersion";

                cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "searchProgramme()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAvailableProgrammeForReg(string programmeCategory, bool progAfterReg)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT distinct pb.programmeId, ps.programmeTitle FROM programme_batch as pb "
                + "INNER JOIN programme_structure as ps ON pb.programmeId = ps.programmeId and pb.defunct='N' and ps.programmeCategory=@cat "
                + "and DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) between pb.programmeRegStartDate and " + (progAfterReg ? "dateadd(d, -1, pb.programmeStartDate)" : "pb.programmeRegEndDate") + " "
                + "order by ps.programmeTitle";

                cmd.Parameters.AddWithValue("@cat", programmeCategory);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAvailableProgrammeForReg()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAvailableProgrammeDateForReg(int programmeId, bool progAfterReg)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT pb.programmeBatchId, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDate, convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDate, "
                + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDate, "
                + "convert(nvarchar, pb.programmeStartDate, 106) + ' to ' + convert(nvarchar, pb.programmeCompletionDate, 106) as programmeDate "
                + "FROM [programme_batch] as pb "
                + "WHERE pb.defunct='N' and pb.programmeId = @programmeId AND DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) between pb.programmeRegStartDate AND "
                + (progAfterReg ? "dateadd(d, -1, pb.programmeStartDate)" : "pb.programmeRegEndDate") + " "
                + "order by pb.programmeStartDate";

                cmd.Parameters.AddWithValue("@programmeId", programmeId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAvailableProgrammeDateForReg()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getType()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PGTYPE' order by codeOrder";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getType()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getProgrammeDetails(string programmeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select p.programmeCode, p.courseCode, p.programmeImage, p.programmeVersion, p.programmeLevel, p.programmeCategory, p.programmeTitle, p.programmeDescription, p.numberOfSOA, p.SSGRefNum, p.bundleId, p.programmeType, b.bundleCode, p.enrollmentTemplate, "
                    + "c1.codeValueDisplay as programmeTypeDisp, c2.codeValueDisplay as programmeCategoryDisplay, c3.codeValueDisplay as programmeLevelDisplay, (SELECT COUNT(*)+1 FROM [programme_structure] as ps WHERE ps.programmeCode = p.programmeCode) as maxProgVer "
                    + "from programme_structure p inner join code_reference c1 on c1.codeValue=p.programmeType and c1.codeType='PGTYPE' "
                    + "inner join code_reference c2 on c2.codeValue = p.programmeCategory and c2.codeType='PGCAT' inner join code_reference c3 on c3.codeValue = p.programmeLevel "
                    + "and c3.codeType = 'PGLVL' inner join bundle b on b.bundleId=p.bundleId where programmeId=@pid";

                cmd.Parameters.AddWithValue("@pid", programmeId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getProgrammeDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getLevel()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PGLVL' order by codeOrder";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getLevel()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAvaProg(string catCode, string lvlCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select distinct programmeTitle from programme_structure where defunct='N' ";
                if ((catCode != null && catCode != "") || (lvlCode != null && lvlCode != ""))
                {
                    cmd.CommandText += "and ";

                    if (catCode != null && catCode != "")
                    {
                        cmd.CommandText += "programmeCategory=@cat and ";
                        cmd.Parameters.AddWithValue("@cat", catCode);
                    }

                    if (lvlCode != null && lvlCode != "")
                    {
                        cmd.CommandText += "programmeLevel=@lvl and ";
                        cmd.Parameters.AddWithValue("@lvl", lvlCode);
                    }

                    cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 4);
                }

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAvaProg()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAvaProgVersion(string catCode, string lvlCode, string title)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select programmeId, programmeVersion from programme_structure where defunct='N' and programmeCategory=@cat and programmeLevel=@lvl and programmeTitle=@t";
                cmd.Parameters.AddWithValue("@cat", catCode);
                cmd.Parameters.AddWithValue("@lvl", lvlCode);
                cmd.Parameters.AddWithValue("@t", title);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getAvaProgVersion()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getCategory()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PGCAT' order by codeOrder";

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getCategory()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getProgrammeByProgrammeCategory(string programmeCategory)
        {
            try
            {
                string sqlStatement = @"SELECT ps.programmeId, ps.programmeCode, ps.programmeVersion, ps.programmeTitle, ps.SSGRefNum
                                    FROM [programme_structure] as ps
                                    WHERE ps.programmeCategory = @programmeCategory AND defunct = 'N'";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory);
                DataTable dtProgrammeStructure = dbConnection.getDataTable(cmd);

                return dtProgrammeStructure;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getProgrammeByProgrammeCategory()", ex.Message, -1);

                return null;
            }
        }

        public bool checkBatchCodeExist(string batchcode, string lessontype)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select count(*) from programme_batch where UPPER(batchCode) = 
                                    UPPER(CONCAT(@batchCode, (select codeValue from code_reference where codetype = 'CLTYPE' 
                                    and upper(codeValueDisplay) = @lessonType)))";
                cmd.Parameters.AddWithValue("@batchCode", batchcode.ToUpper());
                cmd.Parameters.AddWithValue("@lessonType", lessontype);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "checkProgrammeCodeExist()", ex.Message, -1);

                return false;
            }
            
            //
           
        }

        public bool checkProgrammeCodeExist(string programmeCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from programme_structure where UPPER(programmeCode) = @programmeCode";
                cmd.Parameters.AddWithValue("@programmeCode", programmeCode.ToUpper());

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "checkProgrammeCodeExist()", ex.Message, -1);

                return false;
            }
        }

        //create a new programme_structure
        //public bool createProgramme(string programmeCode, string courseCode, int programmeVersion, string programmeLevel, string programmeCategory, string programmeTitle,
        //    string programmeDescription, int numOfSOA, string SSGRefNum, string bundleCode, string programmeType, Byte[] programmeImage, int userId)
        public bool createProgramme(string programmeCode, string courseCode, int programmeVersion, string programmeLevel, string programmeCategory, string programmeTitle,
            string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId, string programmeType, Byte[] programmeImage, int userId)
        {
            try
            {
                string sqlStatement = @"INSERT INTO [programme_structure]
                                      (programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, programmeTitle,
                                      programmeDescription, numberOfSOA, SSGRefNum, bundleId, programmeType, programmeImage, createdBy)
                                      VALUES
                                      (@programmeCode, @courseCode, @programmeVersion, @programmeLevel, @programmeCategory, @programmeTitle,
                                      @programmeDescription, @numberOfSOA, @SSGRefNum, @bundleId, @programmeType, @programmeImage, @createdBy)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
                cmd.Parameters.AddWithValue("@courseCode", courseCode);
                cmd.Parameters.AddWithValue("@programmeVersion", programmeVersion);
                cmd.Parameters.AddWithValue("@programmeLevel", programmeLevel);
                cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory);
                cmd.Parameters.AddWithValue("@programmeTitle", programmeTitle);
                cmd.Parameters.AddWithValue("@programmeDescription", programmeDescription);
                cmd.Parameters.AddWithValue("@numberOfSOA", numOfSOA);
                cmd.Parameters.AddWithValue("@SSGRefNum", SSGRefNum);
                cmd.Parameters.AddWithValue("@bundleId", bundleId);
                cmd.Parameters.AddWithValue("@programmeType", programmeType);
                cmd.Parameters.AddWithValue("@programmeImage", programmeImage);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "createProgramme()", ex.Message, -1);

                return false;
            }
        }

        //update current version programme structure
        public bool updateProgramme(int programmeId, string programmeCode, string courseCode, int programmeVersion, string programmeLevel,
            string programmeCategory, string programmeTitle, string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId,
            string programmeType, Byte[] programmeImage, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = "UPDATE [programme_structure] SET programmeCode = @programmeCode, courseCode = @courseCode, programmeVersion = @programmeVersion, "
                    + "programmeLevel = @programmeLevel, programmeCategory = @programmeCategory, programmeType=@programmeType, programmeTitle = @programmeTitle, programmeDescription = @programmeDescription, "
                    + "numberOfSOA = @numberOfSOA, SSGRefNum = @SSGRefNum, bundleId = @bundleId, lastModifiedBy = @lastModifiedBy, lastModifiedDate = @lastModifiedDate "
                    + (programmeImage != null && programmeImage.Length > 0 ? ", programmeImage = @programmeImage" : "") 
                    + " WHERE programmeId = @programmeId";


                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
                cmd.Parameters.AddWithValue("@courseCode", courseCode);
                cmd.Parameters.AddWithValue("@programmeVersion", programmeVersion);
                cmd.Parameters.AddWithValue("@programmeLevel", programmeLevel);
                cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory);
                cmd.Parameters.AddWithValue("@programmeTitle", programmeTitle);
                cmd.Parameters.AddWithValue("@programmeType", programmeType);
                cmd.Parameters.AddWithValue("@programmeDescription", programmeDescription);
                if (programmeImage != null && programmeImage.Length > 0) cmd.Parameters.AddWithValue("@programmeImage", programmeImage);
                cmd.Parameters.AddWithValue("@numberOfSOA", numOfSOA);
                cmd.Parameters.AddWithValue("@SSGRefNum", SSGRefNum);
                cmd.Parameters.AddWithValue("@bundleId", bundleId);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("@lastModifiedDate", now);
                cmd.Parameters.AddWithValue("@programmeId", programmeId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "updateProgramme()", ex.Message, -1);

                return false;
            }
        }

        //update existing programme to new version
        public int updateProgrammeNewVer(string programmeCode, string courseCode, string programmeLevel, int programmeVersion,
            string programmeCategory, string programmeTitle, string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId,
            string programmeType, Byte[] programmeImage, int prevProgrammeId, int userId)
        {
            try
            {
                string sqlStatement = "INSERT INTO [programme_structure] (programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, "
                    + "programmeTitle, programmeDescription, programmeImage, numberOfSOA, SSGRefNum, bundleId, programmeType, createdBy) "
                    + "VALUES (@programmeCode, @courseCode, @programmeVersion, @programmeLevel, @programmeCategory, @programmeTitle, @programmeDescription, "
                    + (programmeImage != null && programmeImage.Length > 0 ? "@programmeImage, " : "(select programmeImage from programme_structure where programmeId=@pid), ")
                    + " @numberOfSOA, @SSGRefNum, @bundleId, @programmeType, @createdBy); SELECT CAST(scope_identity() AS int);";

                SqlCommand cmd = new SqlCommand(sqlStatement);


                cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
                cmd.Parameters.AddWithValue("@courseCode", courseCode);
                cmd.Parameters.AddWithValue("@programmeLevel", programmeLevel);
                cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory);
                cmd.Parameters.AddWithValue("@programmeVersion", programmeVersion);
                cmd.Parameters.AddWithValue("@programmeTitle", programmeTitle);
                cmd.Parameters.AddWithValue("@programmeDescription", programmeDescription);
                if (programmeImage != null && programmeImage.Length > 0) cmd.Parameters.AddWithValue("@programmeImage", programmeImage);
                else cmd.Parameters.AddWithValue("@pid", prevProgrammeId);
                cmd.Parameters.AddWithValue("@numberOfSOA", numOfSOA);
                cmd.Parameters.AddWithValue("@SSGRefNum", SSGRefNum);
                cmd.Parameters.AddWithValue("@bundleId", bundleId);
                cmd.Parameters.AddWithValue("@programmeType", programmeType);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                int programmeId = dbConnection.executeScalarInt(cmd);

                return programmeId;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "updateProgrammeNewVer()", ex.Message, -1);

                return 0;
            }
        }

        public DataTable validateProgrammeUsed(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT DISTINCT pb.programmeBatchId FROM [programme_batch] as pb "
                + "INNER JOIN [programme_structure] as ps ON pb.programmeId = ps.programmeId "
                + "WHERE ps.programmeId = @pid "
                + "AND (pb.programmeRegStartDate < DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) AND pb.programmeCompletionDate > DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0))";

                cmd.Parameters.AddWithValue("@pid", id);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "validateProgrammeUsed()", ex.Message, -1);

                return null;
            }
        }

        //Defunct programme
        public bool deleteProgramme(int programmeId, int userId)
        {
            try
            {
                string sqlStatement = @"UPDATE [programme_structure]
                                      SET defunct = 'Ý', lastModifiedBy = @lastModifiedBy, lastModifiedDate = getdate() WHERE programmeId = @programmeId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("programmeId", programmeId);
                cmd.Parameters.AddWithValue("lastModifiedBy", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "deleteProgramme()", ex.Message, -1);

                return false;
            }
        }

        //public DataTable getAvailableProgrammeForReg(string programmeCategory)
        //{
        //    try
        //    {
        //        SqlCommand cmd = new SqlCommand();
        //        cmd.CommandText = "SELECT pb.programmeBatchId, pb.programmeId, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDate, "
        //        + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDate, "
        //        + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDate, ps.programmeTitle FROM [programme_batch] as pb "
        //        + "INNER JOIN [programme_structure] as ps ON pb.programmeId = ps.programmeId "
        //        + "WHERE ps.programmeCategory = @programmeCategory AND pb.programmeRegStartDate < getDate() AND "
        //        + "pb.programmeRegEndDate > getDate() order by pb.programmeRegStartDate";

        //        cmd.Parameters.AddWithValue("@programmeCategory", programmeCategory);

        //        DataTable dt = dbConnection.getDataTable(cmd);

        //        return dt;
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Handler lh = new Log_Handler();
        //        lh.WriteLog(ex, "DB_Programme.cs", "getAvailableProgrammeForReg()", ex.Message, -1);

        //        return null;
        //    }
        //}

        public DataTable getProgrammeWithSubsidyInfo()
        {
            try
            {
                string sqlStatement = @"SELECT *
                                      FROM programme_structure as ps
                                      INNER JOIN
                                      subsidy as s
                                      ON ps.programmeId = s.programmeId";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtProgrammeWithSubsidy = dbConnection.getDataTable(cmd);

                return dtProgrammeWithSubsidy;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getProgrammeSubsidyInfo()", ex.Message, -1);

                return null;
            }
        }

        //retrieving list of programme code
        public DataTable getCourseInfo()
        {
            try
            {
                string sqlStatement = @"SELECT *
                                FROM programme_structure
                                WHERE defunct = @defunct";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);
                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "getCourseInfo()", ex.Message, -1);

                return null;
            }
        }

        //retrieving list of course code
        //        public DataTable getCourseInfo()
        //        {
        //            try
        //            {
        //                string sqlStatement = @"SELECT DISTINCT programme_structure.programmeCode,programme_structure.programmeTitle
        //                                      FROM [tmsdb].[programme_structure] as programme_structure
        //                                      WHERE programme_structure.defunct = @defunct";

        //                SqlCommand cmd = new SqlCommand(sqlStatement);

        //                cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);

        //                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

        //                return dtCourseStructure;
        //            }
        //            catch (Exception)
        //            {
        //                return null;
        //            }
        //        }

        #region old codes
        public DataTable getAllCourseCode()
        {
            try
            {
                string sqlStatement = @"SELECT DISTINCT course_structure.courseCode, course_structure.courseTitle
                                      FROM [course_structure] as course_structure
                                      WHERE course_structure.defunct = @defunct";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_NO);

                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //Get Course structure by course code
        public DataTable getCourseStructureByCourseCode(string courseCode, int courseVersion)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                    FROM [course_structure] as course_structure
                                    WHERE course_structure.courseCode = @courseCode AND course_structure.courseVersion = @courseVersion";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@courseCode", courseCode);
                cmd.Parameters.AddWithValue("@courseVersion", courseVersion);
                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Yong Xiang Code
        

        //check whether courseCode selected belongs to a short course 
        public string checkIfIsAShortCourse(string programmeCode)
        {
            StringBuilder sql;
            string value = null;

            SqlConnection conn = dbConnection.getDBConnection();
            conn.Open();

            sql = new StringBuilder();
            sql.AppendLine(@"SELECT isShortCourse FROM [programme_structure] where programmeCode = @programmeCode");
            SqlCommand cmd = new SqlCommand(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("@programmeCode", programmeCode);
            cmd.CommandTimeout = 1200;
            SqlDataReader dr = cmd.ExecuteReader();

            try
            {
                while (dr.Read())
                {
                    value = dr["isShortCourse"].ToString();
                }

            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Programme.cs", "checkIfIsAShortCourse()", ex.Message, -1);
            }
            finally
            {
                conn.Close();
            }

            return value;
        }

        //retrieving list of course code
        public DataTable getSubsidyInfo(string programmeCode)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                      FROM [programme_subsidy_value] 
                                      WHERE programmeCode = @programmeCode";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@programmeCode", programmeCode);

                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //retrieving list of programme code
        public DataTable getAllProgrammeCode()
        {
            try
            {
                string sqlStatement = @"SELECT programmeCode
                                      FROM [programme_structure]";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception)
            {
                return null;
            }
        }

        //retrieving list of programme title
        public DataTable getAllProgrammeTitle()
        {
            try
            {
                string sqlStatement = @"SELECT programmeTitle
                                      FROM [programme_structure]";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                DataTable dtCourseStructure = dbConnection.getDataTable(cmd);

                return dtCourseStructure;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion
    }
}
