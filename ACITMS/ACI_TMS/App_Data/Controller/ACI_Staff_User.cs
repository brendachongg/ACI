using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Data;
using System.Web;
using GeneralLayer;
using System.Configuration;

namespace LogicLayer
{
    public class ACI_Staff_User
    {
        private DB_Users dbUser = new DB_Users();

        public DataTable getCodeReferenceValues(string codeType)
        {
            return dbUser.getCodeReferenceValues(codeType);
        }

        public DataTable findACIStaffs(string value)
        {
            Cryptography encrypt = new Cryptography();
            DataTable dt = dbUser.findACIStaff(encrypt.encryptInfo(value), encrypt.encryptInfo(value), value);
            Cryptography decryptContent = new Cryptography();

            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["idNumber"] = decryptContent.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public Tuple<Boolean, String> deleteACIStaff(int uId, int loginId)
        {
            bool delSuccess = dbUser.deleteACIStaff(uId, loginId);
            if (delSuccess) return new Tuple<Boolean, String>(delSuccess, "ACI User Deleted Successfully");
            else return new Tuple<Boolean, String>(delSuccess, "Error in deleting.");
        }

        public Tuple<Boolean, String> updateACIUser(string name, string email, string idNum, int sal, bool canLogin, bool isTrainer, bool isAssessor, bool isInterviewer, string emType,
          string contact1, string contact2, string address, string postalCode, int uId, int loginId, string wsLoginId)
        {
            Cryptography encrypt = new Cryptography();
            bool updateSuccess = dbUser.updateACIUser(name, encrypt.encryptInfo(email), encrypt.encryptInfo(idNum), sal, canLogin, isTrainer, isAssessor, isInterviewer, emType, contact1, contact2, address, postalCode, uId, loginId, wsLoginId);
            if (updateSuccess == true) return new Tuple<Boolean, String>(updateSuccess, "Update Successfully");
            else return new Tuple<Boolean, String>(updateSuccess, "Error in updating.");
        }

        public string getACIUserEmpType(string loginId)
        {
            return dbUser.getACIUserEmpType(loginId);
        }


        public Tuple<Boolean, DataTable> validatesStaffAccountAuthorizationByLoginId(string loginId)
        {
            DataTable dtStaffUser = dbUser.getUserAccountByLoginId(loginId);
            bool userAuthorizationSuccess = false;
            if (dtStaffUser == null) return new Tuple<bool, DataTable>(false, null);

            if (dtStaffUser.Rows.Count > 0)
            {
                if (dtStaffUser.Rows[0]["userAuthorization"].ToString().Equals(General_Constance.STATUS_YES))
                {
                    userAuthorizationSuccess = true;
                    Session_Handler sh = new Session_Handler();
                    sh.setStaffAccountSession(dtStaffUser);
                }
            }

            //Return true/false and user account information
            return new Tuple<Boolean, DataTable>(userAuthorizationSuccess, dtStaffUser);
        }

        //Validates staff authorization to this Trainee Management System.        
        public Tuple<Boolean, DataTable> validatesStaffAccountAuthorization(string staffEmail, string staffPassword)
        {
            Cryptography encryptContent = new Cryptography();
            string encryptedStaffEmail = encryptContent.encryptInfo(staffEmail);
            string encryptedStaffPass = encryptContent.encryptInfo(staffPassword);

            string userAuthorizationStatus = "Y";
            bool userAuthorizationSuccess = false;

            DataTable dtStaffUser = dbUser.getUserAccount(encryptedStaffEmail, encryptedStaffPass);

            if (dtStaffUser == null) return new Tuple<bool, DataTable>(false, null);

            if (dtStaffUser.Rows.Count > 0)
            {
                if (dtStaffUser.Rows[0]["userAuthorization"].ToString().Equals(userAuthorizationStatus))
                {
                    userAuthorizationSuccess = true;
                    Session_Handler sh = new Session_Handler();
                    sh.setStaffAccountSession(dtStaffUser);
                }
            }

            //Return true/false and user account information
            return new Tuple<Boolean, DataTable>(userAuthorizationSuccess, dtStaffUser);
        }

        public DataTable getSOAContacts()
        {
            DataTable dt = dbUser.getSOAContacts();

            dt.Columns.Add(new DataColumn("userDisp", typeof(string)));
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["userDisp"] = dr["userName"].ToString() + " (" + dr["userEmail"].ToString() + ")";
            }

            return dt;
        }

        public DataTable getACIStaff(int userId)
        {
            DataTable dt = dbUser.getACIStaff(userId);
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["idNumber"] = decryptContent.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }

        public DataTable getUser(int userId)
        {
            DataTable dt = dbUser.getUser(userId);
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["idNumber"] = decryptContent.decryptInfo(dr["idNumber"].ToString());
            }

            return dt;
        }


        public DataTable getAssessors()
        {
            DataTable dt = dbUser.getAssessors();

            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
            }

            return dt;
        }

        public DataTable getTrainers()
        {
            DataTable dt = dbUser.getTrainers();

            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
            }

            return dt;
        }

        public DataTable getAllStaff()
        {
            DataTable dt = dbUser.getAllStaffs();
            if (dt == null) return null;
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["idNumber"] = decryptContent.decryptInfo(dr["idNumber"].ToString());
                dr["useremail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
            }
            return dt;

        }

        public DataTable getInterviewer()
        {
            DataTable dt = dbUser.getInterviewer();
            if (dt == null) return null;

            dt.Columns.Add(new DataColumn("userDisplay", typeof(string)));
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["userDisplay"] = dr["userName"].ToString() + " (" + dr["userEmail"].ToString() + ")";
            }

            return dt;
        }
        public DataTable getStaff()
        {
            DataTable dt = dbUser.getAllUser();
            if (dt == null) return null;

            dt.Columns.Add(new DataColumn("userDisplay", typeof(string)));
            Cryptography decryptContent = new Cryptography();
            foreach (DataRow dr in dt.Rows)
            {
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
                dr["userDisplay"] = dr["userName"].ToString() + " (" + dr["userEmail"].ToString() + ")";
            }

            return dt;
        }

        public Tuple<bool, string> addACIStaff(string name, string email, string password, string idNum, int sal, bool canLogin, bool isTrainer, bool isAccessor, bool isInterviewer, string emType,
           string contact1, string contact2, string address, string postalCode, int userId, string wsLoginId)
        {
            //TODO: check duplicate email and ID number

            Cryptography encrypt = new Cryptography();
            if (dbUser.ifStaffExists(encrypt.encryptInfo(idNum)))
            {
                return new Tuple<bool, string>(false, "This staff exists in the system.");
            }
            else
            {

                if (dbUser.addACIUser(name, encrypt.encryptInfo(email), encrypt.encryptInfo(password), encrypt.encryptInfo(idNum), sal, canLogin, isTrainer, isAccessor, isInterviewer, emType, contact1,
                    contact2 == "" ? null : contact2, address, postalCode, userId, wsLoginId == "" ? null : wsLoginId))
                {
                    if (emType == GeneralLayer.StaffEmploymentType.CON.ToString() || emType == GeneralLayer.StaffEmploymentType.INT.ToString() || emType == GeneralLayer.StaffEmploymentType.TMP.ToString())
                    {
                        Email_Handler eh = new Email_Handler();
                        string sender = ConfigurationManager.AppSettings["FrmEmail"].ToString();
                        string url = ConfigurationManager.AppSettings["ACI_TMS_URL"].ToString();
                        string body = "Dear " + name + ", <br>";
                        body += "Please be informed of your login credential to ACI Trainee Management System " + url + "<br>";
                        body += "Your login ID and Password are: <br>";
                        body += "Login ID: " + email + "<br>";
                        body += "Password: " + password + "<br>";

                        body += "<br><br><br> <i>[This is a system-generated email. Please do not reply to this e-mail as we are not able to respond to the messages sent to this e-mail address. ]</i>";

                        bool sendEmail = eh.SendEmail(sender, email, null, null, "ACI Trainee Managment System - Login Credentials", body);
                        if (sendEmail)
                            return new Tuple<bool, string>(true, "Staff saved successfully. Email containing login credentials was sent to staff successfully");
                        else
                            return new Tuple<bool, string>(true, "Staff saved successfully. Email containing login credentials was not sent to staff.");
                    }
                    return new Tuple<bool, string>(true, "Staff saved successfully.");
                }
                else return new Tuple<bool, string>(false, "Error saving staff information.");
            }
        }

        public Tuple<bool, string> addStaff(string name, string email, string password, string idNum, Salutation sal, bool canLogin, bool isTrainer, bool isAccessor, StaffEmploymentType emType,
            string contact1, string contact2, string address, string postalCode, int userId)
        {
            //TODO: check duplicate email and ID number

            Cryptography encrypt = new Cryptography();

            if (dbUser.addNewUser(name, encrypt.encryptInfo(email), encrypt.encryptInfo(password), encrypt.encryptInfo(idNum), sal, canLogin, isTrainer, isAccessor, emType, contact1,
                contact2 == "" ? null : contact2, address, postalCode, userId))
                return new Tuple<bool, string>(true, "Staff saved successfully.");
            else return new Tuple<bool, string>(false, "Error saving staff information.");
        }
    }
}
