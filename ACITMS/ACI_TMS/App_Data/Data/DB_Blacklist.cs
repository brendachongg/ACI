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
    public class DB_Blacklist
    {
        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();

        public DataTable getSuspensionListWithinPeriod()
        {
            try
            {
                string sql = @"Select * from [aci_suspended_list] where startDate <= DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) and endDate >= DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)";

                SqlCommand cmd = new SqlCommand(sql);

                DataTable dtSuspendedList = dbConnection.getDataTable(cmd);

                return dtSuspendedList;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "getSuspendedList()", ex.Message, -1);

                return null;
            }
        }

        public bool searchBlackListRecordByNRICSuspensionDate(string nric, DateTime startDate, DateTime endDate)
        {
            bool exist = false;

            try
            {
                string sql = @"select * from [aci_suspended_list] where idnumber = @nric and endDate >= @StartDate and startDate <= @EndDate";
                SqlCommand cmd = new SqlCommand(sql);

                cmd.Parameters.AddWithValue("@nric", nric);
                cmd.Parameters.AddWithValue("@StartDate", startDate);
                cmd.Parameters.AddWithValue("@EndDate", endDate);
                exist = dbConnection.executeScalar(cmd);

                return exist;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "searchBlackListRecordByNRICSuspensionDate()", ex.Message, -1);

                return false;
            }
        }


        public bool searchBlacklistedRecordByNRIC(string identity)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                      FROM aci_suspended_list as aci_suspended_list
                                      WHERE aci_suspended_list.idNumber = @idNumber";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@idNumber", identity);
                bool exist = dbConnection.executeScalar(cmd);

                return exist;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "searchBlacklistedRecordByNRIC()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getSuspendedList()
        {
            try
            {
                DateTime now = DateTime.Now.Date;

                string sqlStatement = @"SELECT *

                                      FROM aci_suspended_list as aci_suspended_list";


                SqlCommand cmd = new SqlCommand(sqlStatement);

                DataTable dtSuspendedList = dbConnection.getDataTable(cmd);

                return dtSuspendedList;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "getSuspendedList()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSuspendedListByValue(string nricValue, string nameValue)
        {
            try
            {
                DateTime now = DateTime.Now.Date;

                string sqlStatement = @"SELECT *

                                      FROM aci_suspended_list as aci_suspended_list

                                      WHERE aci_suspended_list.idNumber = @nricValue OR aci_suspended_list.fullName LIKE @nameValue";


                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@nameValue", "%" + nameValue + "%");
                cmd.Parameters.AddWithValue("@nricValue", nricValue);

                DataTable dtSuspendedListByValue = dbConnection.getDataTable(cmd);

                return dtSuspendedListByValue;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "getSuspendedListByValue()", ex.Message, -1);

                return null;
            }
        }

        //remove from suspension
        public bool removefromSuspension(string suspendId)
        {
            try
            {
                string sqlStatement = @"DELETE

                                      FROM aci_suspended_list

                                      WHERE suspendId = @suspendId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@suspendId", suspendId);
                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "removefromSuspension()", ex.Message, -1);

                return false;
            }
        }

        //add into blacklist/suspension
        public bool addSuspension(string idNumber, string fullName, DateTime startDate, DateTime endDate, string byOrganization, string suspendedRemarks, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"INSERT INTO aci_suspended_list
                                    
                                      (idNumber, fullName, startDate, endDate, byOrganization, suspendedRemarks, createdBy)
                                      VALUES
                                      (@idNumber, @fullName, @startDate, @endDate, @byOrganization, @suspendedRemarks, @createdBy)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@idNumber", idNumber);
                cmd.Parameters.AddWithValue("@fullName", fullName);
                cmd.Parameters.AddWithValue("@startDate", startDate);
                cmd.Parameters.AddWithValue("@endDate", endDate);
                cmd.Parameters.AddWithValue("@byOrganization", byOrganization);
                cmd.Parameters.AddWithValue("@suspendedRemarks", suspendedRemarks);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Blacklist.cs", "addSuspension()", ex.Message, -1);

                return false;
            }
        }
    }
}
