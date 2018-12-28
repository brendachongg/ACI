using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Data.SqlClient;

namespace LogicLayer
{
    public class Programme_Management
    {
        private DB_Programme dbProgramme = new DB_Programme();

        public DataTable getAllAvailableProgrammesWS(string progCat)
        {
            return dbProgramme.getAllAvailableProgrammesWS(progCat);
        }
        public DataTable getAllAvailableProgrammes()
        {
            return dbProgramme.getAllAvailableProgrammes();
        }

        public DataTable getProgrammeById(string pid)
        {
            return dbProgramme.getProgrammeById(pid);
        }

        public DataTable getProgrammesModules(string bundleid)
        {
            return dbProgramme.getProgrammesModules(bundleid);
        }

        public string getEnrollmentTemplate(int progId)
        {
            return dbProgramme.getEnrollmentTemplate(progId);
        }

        public Tuple<bool, string> updateEnrollmentTemplate(int progId, string template)
        {
            if (dbProgramme.updateEnrollmentTemplate(progId, template))
                return new Tuple<bool, string>(true, "Template saved successfully.");
            else return new Tuple<bool, string>(false, "Error saving template.");
        }

        public DataTable searchProgramme(string type, string value)
        {
            string condition = "";
            if (type == "CODE") condition = "UPPER(programmeCode) like @sv";
            else if (type == "TITLE") condition = "UPPER(programmeTitle) like @sv";
            else condition = "UPPER(courseCode) like @sv";

            SqlParameter p = new SqlParameter("@sv", "%" + value.ToUpper() + "%");

            return dbProgramme.searchProgramme(condition, p);
        }

        public DataTable getAvailableProgrammeForReg(string programmeCategory, bool progAfterReg = false)
        {
            return dbProgramme.getAvailableProgrammeForReg(programmeCategory, progAfterReg);
        }

        public DataTable getAvailableProgrammeDateForReg(int programmeId, bool progAfterReg = false)
        {
            return dbProgramme.getAvailableProgrammeDateForReg(programmeId, progAfterReg);
        }

        public DataTable getProgrammeType()
        {
            return dbProgramme.getType();
        }

        public DataTable getProgrammeDetails(string programmeId)
        {
            return dbProgramme.getProgrammeDetails(programmeId);
        }

        public DataTable getProgrammeCategory()
        {
            return dbProgramme.getCategory();
        }

        public DataTable getProgrammeLevel()
        {
            return dbProgramme.getLevel();
        }

        public DataTable getAvaProgrammeTitle(string categoryCode, string levelCode)
        {
            return dbProgramme.getAvaProg(categoryCode, levelCode);
        }

        public DataTable getAvaProgrammeVersion(string categoryCode, string levelCode, string title)
        {
            return dbProgramme.getAvaProgVersion(categoryCode, levelCode, title);
        }

        //Get programme structure details by programme category
        public DataTable getProgrammeByProgrammeCategory(string programmeCategory)
        {
            DataTable dtCourse = dbProgramme.getProgrammeByProgrammeCategory(programmeCategory);

            return dtCourse;
        }

        //Create new programme structure
        public Tuple<bool, string> createProgramme(string programmeCode, string courseCode, int programmeVersion, string programmeLevel, string programmeCategory, string programmeTitle,
            string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId, string programmeType, Byte[] programmeImage, int userId)
        {
            if (dbProgramme.checkProgrammeCodeExist(programmeCode))
            {
                return new Tuple<bool, string>(false, "Programme code already exist.");
            }
            else
            {
                if (dbProgramme.createProgramme(programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, programmeTitle,
                    programmeDescription, numOfSOA, SSGRefNum, bundleId, programmeType, programmeImage, userId))
                    return new Tuple<bool, string>(true, "Programme created successfully.");
                else
                    return new Tuple<bool, string>(false, "Error creating programme");
            }
        }

        //update existing programme version
        public Tuple<bool, string> updateProgramme(int programmeId, string programmeCode, string courseCode, int programmeVersion, string programmeLevel,
            string programmeCategory, string programmeTitle, string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId,
            string programmeType, Byte[] programmeImage, int userId)
        {
            if (dbProgramme.updateProgramme(programmeId, programmeCode, courseCode, programmeVersion, programmeLevel,
            programmeCategory, programmeTitle, programmeDescription, numOfSOA, SSGRefNum, bundleId,
            programmeType, programmeImage, userId))
                return new Tuple<bool, string>(true, "Programme saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating programme.");
        }

        //update existing programme to new version
        public Tuple<bool, string, int> updateProgrammeNewVer(string programmeCode, string courseCode, string programmeLevel, int programmeVersion,
            string programmeCategory, string programmeTitle, string programmeDescription, int numOfSOA, string SSGRefNum, int bundleId,
            string programmeType, Byte[] programmeImage, int prevProgrammeId, int userId)
        {
            int update = dbProgramme.updateProgrammeNewVer(programmeCode, courseCode, programmeLevel, programmeVersion,
            programmeCategory, programmeTitle, programmeDescription, numOfSOA, SSGRefNum, bundleId,
            programmeType, programmeImage, prevProgrammeId, userId);

            if (update > 0)
                return new Tuple<bool, string, int>(true, "Programme saved successfully.", update);
            else
                return new Tuple<bool, string, int>(false, "Error updating programme.", 0);
        }

        //Defunct programme
        public Tuple<bool, string> deleteProgramme(int programmeId, string programmeCode, int userId)
        {
            //DataTable dtBatchModule = dbProgramme.validateProgrammeUsed(programmeId);

            //if (dtBatchModule.Rows.Count != 0)
            //{
            //    return new Tuple<bool, string>(false, "Error deleting programme.");
            //}
            //else
            //{
                if (dbProgramme.deleteProgramme(programmeId, userId))
                    return new Tuple<bool, string>(true, "Programme deleted successfully.");
                else
                    return new Tuple<bool, string>(false, "Error deleting programme.");
            //}
        }

        public DataTable validateProgrammeUsed(int id)
        {
            return dbProgramme.validateProgrammeUsed(id);
        }

        public DataTable getProgrammeWithSubsidyInfo()
        {
            DataTable dt = dbProgramme.getProgrammeWithSubsidyInfo();

            return dt;
        }

        #region old code
        public DataTable getAllCourseCode()
        {
            DataTable dtCourse = dbProgramme.getAllCourseCode();

            return dtCourse;
        }

        //Get course structure details by course code
        public DataTable getCourseStructureByCourseCode(string courseCode, string courseVersion)
        {
            DataTable dtCourse = dbProgramme.getCourseStructureByCourseCode(courseCode, int.Parse(courseVersion));
            dtCourse.Columns.Add("courseImageTemp", typeof(string));

            if (dtCourse.Rows.Count > 0)
            {
                foreach (DataRow dr in dtCourse.Rows)
                {
                    if (!dr["imageUrl"].ToString().Equals(null))
                    {
                        dr["imageUrl"] = "/Resource/course_image/" + dr["imageUrl"].ToString();
                    }

                    if (!dr["courseImage"].ToString().Equals(""))
                    {
                        byte[] imgByte = (byte[])dr["courseImage"];
                        string str = Convert.ToBase64String(imgByte);
                        dr["courseImageTemp"] = "data:Image/png;base64," + str;
                    }
                }
            }

            return dtCourse;
        }
        #endregion

        #region Yong Xiang Codes
        //retrieving list of programme title
        public DataTable getAllProgrammeTitle()
        {
            DataTable tb = new DataTable();
            tb = dbProgramme.getAllProgrammeTitle();
            return tb;
        }

        public DataTable getCourseInfo()
        {
            DataTable dtCourse = dbProgramme.getCourseInfo();
            return dtCourse;
        }

        //retrieving subsidy details
        public DataTable getSubsidyInfo(string courseCode)
        {
            DataTable tb = new DataTable();
            tb = dbProgramme.getSubsidyInfo(courseCode);
            return tb;

        }

        //check whether courseCode selected belongs to a short course 
        public string checkIfIsAShortCourse(string courseCode)
        {
            return dbProgramme.checkIfIsAShortCourse(courseCode);

        }

        //retrieving list of programme code
        public DataTable getAllProgrammeCode()
        {
            DataTable tb = new DataTable();
            tb = dbProgramme.getAllProgrammeCode();
            return tb;
        }
        #endregion
    }
}
