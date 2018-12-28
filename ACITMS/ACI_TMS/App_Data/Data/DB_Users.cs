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
    public class DB_Users
    {
        //Initialize connection string
        private Database_Connection dbConnection = new Database_Connection();

        public bool ifStaffExists(string idNumber)
        {
            bool ifStaffExists = true;
            try
            {
                string sql = @"Select Count(1) from aci_user where idNumber = @idNumber";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@idNumber", idNumber);
                cmd.CommandText = sql;

                if (dbConnection.executeScalarInt(cmd) > 0)
                {
                    //staff exists
                    ifStaffExists = true;
                }
                else
                {
                    ifStaffExists = false;
                }



                return ifStaffExists;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "ifStaffExists()", ex.Message, -1);
                return ifStaffExists;
            }
        }

        public DataTable findACIStaff(string idNumber, string email, string name)
        {
            try
            {
                DataTable dtSearch = null;
                string sql = @"SELECT a.userid, a.username, a.useremail, a.idNumber, a.contactnumber1, c.codevaluedisplay  
                               FROM [aci_user] a left join code_reference c on a.[emplType] = c.codeValue and codetype='USREMP' where a.defunct = 'N' AND 
                               (a.userName LIKE @name OR a.useremail = @email or a.idNumber = @nric)";

                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.AddWithValue("@name", "%" + name + "%");
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@nric", idNumber);
                cmd.CommandText = sql;
                dtSearch = dbConnection.getDataTable(cmd);
                return dtSearch;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "findACIStaff()", ex.Message, -1);
                return null;
            }
        }

        public bool deleteACIStaff(int uId, int loginId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"update aci_user set defunct = @defunct, lastModifiedBy=@loginId, lastModifiedDate=getdate() where userId = @uId";
                cmd.Parameters.AddWithValue("@defunct", General_Constance.STATUS_YES);
                cmd.Parameters.AddWithValue("@uId", uId);
                cmd.Parameters.AddWithValue("@loginId", loginId);

                cmd.CommandText = sql;
                dbConnection.executeNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "updateACIUser()", ex.Message, -1);
                return false;
            }

        }
        public DataTable getCodeReferenceValues(string codeType)
        {
            try
            {
                string sqlStatement = @"SELECT codeValue, codeValueDisplay FROM code_reference WHERE codeType = @codeType ORDER BY codeOrder";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@codeType", codeType);
                DataTable dt = dbConnection.getDataTable(cmd);
               
                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getCodeReferenceValues()", ex.Message, -1);

                return null;
            }
        }

        public bool updateACIUser(string name, string email, string idNum, int sal, bool canLogin, bool isTrainer, bool isAssessor,bool isInterviewer, string emType,
          string contact1, string contact2, string address, string postalCode, int uId, int loginId, string wsLoginId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                string sql = @"update aci_user set username = @name, userEmail = @email, idNumber = @idNum, salutation=@sal, userAuthorization=@canLogin, 
                               isTrainer = @isTrainer, isAssessor =@isAssessor, emplType = @empType, isInterviewer = @isInterviewer,
                               contactNumber1 = @c1, contactNumber2 = @c2, addressLine = @addr, postalCode = @postal, lastModifiedBy=@loginId, lastModifiedDate=getdate(), loginId = @wsLoginId where userId = @uId";

                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@idNum", idNum);
                cmd.Parameters.AddWithValue("@sal", sal.ToString());
                cmd.Parameters.AddWithValue("@canLogin", canLogin ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isTrainer", isTrainer ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isAssessor", isAssessor ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isInterviewer", isInterviewer ? "Y" : "N");
                cmd.Parameters.AddWithValue("@empType", emType);
                cmd.Parameters.AddWithValue("@c1", contact1);
                cmd.Parameters.AddWithValue("@c2", contact2 == "" ? (object)DBNull.Value : contact2);
                cmd.Parameters.AddWithValue("@addr", address);
                cmd.Parameters.AddWithValue("@postal", postalCode);
                cmd.Parameters.AddWithValue("@loginId", loginId);
                cmd.Parameters.AddWithValue("@uId", uId);
                cmd.Parameters.AddWithValue("@wsLoginId", wsLoginId == "" ? (object)DBNull.Value : wsLoginId);

                dbConnection.executeNonQuery(cmd);
                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "updateACIUser()", ex.Message, -1);
                return false;
            }
        }

        public bool addACIUser(string name, string email, string password, string idNum, int sal, bool canLogin, bool isTrainer, bool isAssessor, bool isInterviewer, string emType,
          string contact1, string contact2, string address, string postalCode, int userId, string wsLoginId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "insert into aci_user(userName, userEmail, userPassword, idNumber, salutation, userAuthorization, isTrainer, isAssessor, isInterviewer, emplType, contactNumber1, "
                    + "contactNumber2, addressLine, postalCode, defunct, createdBy, createdOn, loginId) values (@name, @email, @pwd, @idNum, @sal, @usrAuth, @isTrainer, @isAssessor, @isInterviewer, @empType, "
                    + "@c1, @c2, @addr, @postal, 'N', @uid, getdate(), @loginId)";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pwd", password);
                cmd.Parameters.AddWithValue("@idNum", idNum);
                cmd.Parameters.AddWithValue("@sal", sal.ToString());
                cmd.Parameters.AddWithValue("@usrAuth", canLogin ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isTrainer", isTrainer ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isAssessor", isAssessor ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isInterviewer", isInterviewer ? "Y" : "N");
                cmd.Parameters.AddWithValue("@empType", emType);
                cmd.Parameters.AddWithValue("@c1", contact1);
                cmd.Parameters.AddWithValue("@c2", contact2 == null ? (object)DBNull.Value : contact2);
                cmd.Parameters.AddWithValue("@addr", address);
                cmd.Parameters.AddWithValue("@postal", postalCode);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@loginId", wsLoginId == null ? (object)DBNull.Value : wsLoginId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "addNewUser()", ex.Message, -1);

                return false;
            }
        }

        public bool addNewUser(string name, string email, string password, string idNum, Salutation sal, bool canLogin, bool isTrainer, bool isAccessor, StaffEmploymentType emType,
            string contact1, string contact2, string address, string postalCode, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "insert into aci_user(userName, userEmail, userPassword, idNumber, salutation, userAuthorization, isTrainer, isAssessor, emplType, contactNumber1, "
                    + "contactNumber2, addressLine, postalCode, defunct, createdBy, createdOn) values (@name, @email, @pwd, @idNum, @sal, @usrAuth, @isTrainer, @isAssessor, @empType, "
                    + "@c1, @c2, @addr, @postal, 'N', @uid, getdate())";

                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@pwd", password);
                cmd.Parameters.AddWithValue("@idNum", idNum);
                cmd.Parameters.AddWithValue("@sal", ((int)sal).ToString());
                cmd.Parameters.AddWithValue("@usrAuth", canLogin ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isTrainer", isTrainer ? "Y" : "N");
                cmd.Parameters.AddWithValue("@isAssessor", isAccessor ? "Y" : "N");
                cmd.Parameters.AddWithValue("@empType", emType.ToString());
                cmd.Parameters.AddWithValue("@c1", contact1);
                cmd.Parameters.AddWithValue("@c2", contact2 == null ? (object)DBNull.Value : contact2);
                cmd.Parameters.AddWithValue("@addr", address);
                cmd.Parameters.AddWithValue("@postal", postalCode);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "addNewUser()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getSOAContacts()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userId, userName, userEmail from aci_user where defunct='N' and "
                    + "emplType in ('" + StaffEmploymentType.FT.ToString() + "', '" + StaffEmploymentType.CON.ToString() + "') order by userName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getSOAContacts()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getACIStaff(int userId)
        {
            try
            {

                SqlCommand cmd = new SqlCommand();
                string sql = @"select userName, userEmail, idNumber, salutation, isTrainer, userAuthorization, isAssessor, emplType, 
                            contactNumber1, contactNumber2, addressLine, postalCode, isInterviewer, loginId from aci_user where userId=@uId";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@uid", userId);
                return dbConnection.getDataTable(cmd);

            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getACIStaff()", ex.Message, -1);
                return null;
            }
        }

        public DataTable getUser(int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select u.userName, u.userEmail, u.idNumber, u.salutation, c1.codeValueDisplay as salutationDisp, u.isTrainer, u.userAuthorization, u.isAssessor, u.emplType, c2.codeValueDisplay as emplTypeDisp, "
                    + "u.contactNumber1, u.contactNumber2, u.addressLine, u.postalCode from aci_user u inner join code_reference c1 on c1.codeValue=u.salutation and c1.codeType='SAL' "
                    + "inner join code_reference c2 on c2.codeValue=u.emplType and c2.codeType='USREMP' and u.userId=@uid";

                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getUser()", ex.Message, -1);

                return null;
            }
        }

        public string getUserName(int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userName from aci_user where userId=@uid";

                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.executeScalarString(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getUserName()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAssessors()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userId, userName, userEmail from aci_user where defunct='N' and isAssessor='Y' order by userName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getAssessors()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTrainers()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userId, userName, userEmail from aci_user where defunct='N' and isTrainer='Y' order by userName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getTrainers()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getInterviewer()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userId, userName, userEmail from aci_user where defunct = '" + General_Constance.STATUS_NO + "' and isInterviewer = '" + General_Constance.STATUS_YES + "' order by userName";
                
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getInterviewer()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllUser()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select userId, userName, userEmail from aci_user where defunct='N' and userAuthorization='Y' order by userName";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getAllUser()", ex.Message, -1);

                return null;
            }
        }


        public string getACIUserEmpType(string loginId)
        {
            try
            {
                string sqlStatement = "Select emplType from aci_user where userEmail = @userEmail OR loginID = @loginID and defunct ='" + General_Constance.STATUS_NO + "'" ;
                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@loginID", loginId);
                cmd.Parameters.AddWithValue("@userEmail", new Cryptography().encryptInfo(loginId));
            

                DataTable dtUserAccount = dbConnection.getDataTable(cmd);

                if (dtUserAccount.Rows.Count > 0)
                    return dtUserAccount.Rows[0]["emplType"].ToString();
                else
                    return "";
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getACIUserEmpType()", ex.Message, -1);

                return "";
            }
        }

        public DataTable getUserAccountByLoginId(string loginId)
        {
            try
            {
                string sqlStatement = "SELECT * FROM [aci_user] WHERE loginId = @loginId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@loginId", loginId);
         

                DataTable dtUserAccount = dbConnection.getDataTable(cmd);

                return dtUserAccount;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getUserAccount()", ex.Message, -1);

                return null;
            }

        }

        public void Insert_Into_LoginLog(string userid, DateTime dtLogin, bool IsSuccess)
        {
            string sql = "insert into login_log(loginid, logindate, issuccess values(@loginid,getdate(), @issuccess)";
            SqlCommand cmd = new SqlCommand();
            SqlConnection conn = dbConnection.getDBConnection();
            try
            {
                conn.Open();
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@loginid", userid);
                cmd.Parameters.AddWithValue("@issuccess", IsSuccess == true ? General_Constance.STATUS_YES.ToString() : General_Constance.STATUS_NO.ToString());
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_User", "Insert_Into_LoginLog", ex.ToString(), -1);
            }
         
        }

        //Get staff account 
        public DataTable getUserAccount(string userEmail, string userPass)
        {
            try
            {
                string sqlStatement = "SELECT * FROM [aci_user] WHERE userEmail = @userEmail AND userPassword = @userPass and defunct = '" + General_Constance.STATUS_NO + "'";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@userEmail", userEmail);
                cmd.Parameters.AddWithValue("@userPass", userPass);

                DataTable dtUserAccount = dbConnection.getDataTable(cmd);

                return dtUserAccount;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getUserAccount()", ex.Message, -1);

                return null;
            }

        }

        //Get all roles by staff user id
        public DataTable getRolesByUserId(int userId)
        {
            try
            {
                string sqlStatement = @"SELECT * FROM [aci_user_role] as user_role
                                    INNER JOIN [aci_role] as aci_role
                                    ON user_role.roleId = aci_role.roleId WHERE user_role.userId = @userId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@userId", userId);

                DataTable dtRoles = dbConnection.getDataTable(cmd);

                return dtRoles;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getRolesByUserId()", ex.Message, -1);

                return null;
            }

        }

        public DataTable getAllStaffs()
        {

            try
            {
                string sql = @"SELECT a.userid, a.username, a.useremail, a.idNumber, a.contactnumber1, c.codevaluedisplay  FROM [aci_user] a left join code_reference c on a.[emplType] = c.codeValue and codetype='USREMP' where a.defunct = 'N'";

                SqlCommand cmd = new SqlCommand(sql);
                DataTable dtAllStaffs = dbConnection.getDataTable(cmd);

                return dtAllStaffs;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Users.cs", "getAllStaffs()", ex.Message, -1);

                return null;
            }
        }

    }
}
