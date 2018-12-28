using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using GeneralLayer;

namespace DataLayer
{
    public class DB_Venue
    {
        private Database_Connection dbConnection = new Database_Connection();

        public bool isVenueIdExist(string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from [venue] where UPPER(venueId) = @venueId";
                cmd.Parameters.AddWithValue("@venueId", venueId.ToUpper());

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "isVenueIdExist()", ex.Message, -1);

                return false;
            }
        }

        public bool createVenue(string venueId, string venueLocation, string venueDesc, int venueCapacity, DateTime venueEffectDate, int userId)
        {
            try
            {
                string sqlStatement = @"INSERT INTO [venue]
                                      (venueId, venueLocation, venueCapacity, venueDesc, venueEffectDate, createdBy)
                                      VALUES
                                      (@venueId, @venueLocation, @venueCapacity, @venueDesc, @venueEffectDate, @createdBy)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@venueId", venueId);
                cmd.Parameters.AddWithValue("@venueLocation", venueLocation);
                cmd.Parameters.AddWithValue("@venueCapacity", venueCapacity);
                cmd.Parameters.AddWithValue("@venueDesc", venueDesc);
                cmd.Parameters.AddWithValue("@venueEffectDate", venueEffectDate);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "createVenue()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getAllVenue()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT v.venueId, v.venueLocation, v.venueCapacity, v.venueDesc, "
                + "convert(nvarchar, v.venueEffectDate, 106) as venueEffectDate FROM [venue] as v WHERE v.defunct = 'N'";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getAllVenue()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getVenue(string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT v.venueId, v.venueLocation, v.venueCapacity, v.venueDesc,  convert(nvarchar, v.venueEffectDate, 106) as venueEffectDate "
                + "FROM [venue] as v WHERE v.venueId = @venueId";

                cmd.Parameters.AddWithValue("@venueId", venueId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getVenue()", ex.Message, -1);

                return null;
            }
        }

        public DataTable isVenueUsed(string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT vbr.bookingId FROM [venue_booking_record] as vbr "
                + "INNER JOIN [batchModule_session] as bms ON vbr.sessionId = bms.sessionId and bms.defunct='N' and vbr.defunct='N' "
                + "WHERE ";

                cmd.CommandText += "vbr.venueId = @venueId "
                + "AND bms.sessionDate >= getDate()";

                cmd.Parameters.AddWithValue("@venueId", venueId);

                DataTable dt = dbConnection.getDataTable(cmd);

                return dt;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "isVenueUsed()", ex.Message, -1);

                return null;
            }
        }

        //Update venue details
        public bool updateVenue(string venueId, string venueLocation, string venueDesc, int venueCapacity,
            DateTime venueEffectDate, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"UPDATE [venue]
                                    
                                      SET venueLocation = @venueLocation, venueDesc = @venueDesc, venueCapacity = @venueCapacity,
                                      venueEffectDate = @venueEffectDate, lastModifiedBy = @lastModifiedBy, 
                                      lastModifiedDate = @lastModifiedDate WHERE venueId = @venueId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@venueId", venueId);
                cmd.Parameters.AddWithValue("@venueLocation", venueLocation);
                cmd.Parameters.AddWithValue("@venueDesc", venueDesc);
                cmd.Parameters.AddWithValue("@venueCapacity", venueCapacity);
                cmd.Parameters.AddWithValue("@venueEffectDate", venueEffectDate);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("@lastModifiedDate", now);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "updateVenue()", ex.Message, -1);

                return false;
            }
        }

        //Defunct venue
        public bool deleteVenue(string venueId, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;
                string defunct = "Y";

                string sqlStatement = @"
                                      UPDATE [venue]
                                      SET defunct = @defunct, lastModifiedBy = @lastModifiedBy, lastModifiedDate = @lastModifiedDate WHERE venueId = @venueId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@venueId", venueId);
                cmd.Parameters.AddWithValue("@defunct", defunct);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("@lastModifiedDate", now);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "deleteVenue()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getRecentVenues(string condition)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select top 10 v.venueId, v.venueLocation, v.venueCapacity, v.venueDesc, max(isnull(b.lastModifiedDate, b.createdOn)) as dt "
                    + "from Venue v inner join venue_booking_record b on b.venueId=v.venueId and b.defunct='N' "
                    + condition + " group by v.venueId, v.venueLocation, v.venueCapacity, v.venueDesc order by dt desc";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getRecentVenues()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getListVenues(string frm, string to, bool containNum = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = @"SELECT venueId, venueLocation, venueCapacity, venueDesc
                                      FROM Venue
                                      where (SUBSTRING(UPPER(venueLocation),1 ,1) BETWEEN @frm AND @to ";

                if (containNum) cmd.CommandText += "or SUBSTRING(venueLocation,1 ,1) BETWEEN '0' AND '9' ";

                cmd.CommandText += ") and defunct='N' order by venueLocation";

                cmd.Parameters.AddWithValue("@frm", frm);
                cmd.Parameters.AddWithValue("@to", to);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getListVenues()", ex.Message, -1);

                return null;
            }
        }

        public int getVenueCapacity(string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select venueCapacity from Venue where venueID=@vid";

                cmd.Parameters.AddWithValue("@vid", venueId);

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getVenueCapacity()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getVenueCapacity(string[] venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select venueID, venueLocation, venueCapacity from Venue where venueID in (";

                int i = 0;
                foreach (string vid in venueId)
                {
                    cmd.CommandText += "@vid" + i + ",";
                    cmd.Parameters.AddWithValue("@vid" + i, vid);
                    i++;
                }

                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 1) + ")";
                

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getVenueCapacity()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getPeriods()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PERIOD' order by codeOrder";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getPeriods()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getVenueBookings(int day, DayPeriod period, DateTime dtStart, DateTime dtEnd, string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //mon is 0, sun is 6 in sql, so given the day where mon is 1, have to -1
                cmd.CommandText = "WITH dtList AS (select case when DATEADD(wk, DATEDIFF(wk,0,@start), @day-1) < @start then "
                    + "dateadd(day, 7, DATEADD(wk, DATEDIFF(wk,0,@start), @day-1)) else DATEADD(wk, DATEDIFF(wk,0,@start), @day-1) end as dt "
                    + "union all select DATEADD(dd, 7, dt) FROM dtList s WHERE DATEADD(dd, 7, dt) <= @end) "
                    + "select d.dt, convert(nvarchar, d.dt, 106) as dtDisp, c.codeValue as period, c.codeValueDisplay as periodDisp, "
                    + "case when b.bookingId is null then 'Available' else 'Booked' end as status "
                    + "from dtList d inner join code_reference c on c.codeType='PERIOD' and "
                    + (period == DayPeriod.FD ? "(c.codeValue='" + DayPeriod.AM.ToString() + "' or c.codeValue='" + DayPeriod.PM.ToString() + "')" : "c.codeValue=@pt ")
                    + "left outer join venue_booking_record b on b.bookingDate=d.dt and b.bookingPeriod=c.codeValue and b.venueId=@vid and b.defunct='N' "
                    + "order by d.dt, c.codeOrder";

                cmd.Parameters.AddWithValue("@start", dtStart);
                cmd.Parameters.AddWithValue("@end", dtEnd);
                if (period != DayPeriod.FD) cmd.Parameters.AddWithValue("@pt", period.ToString());
                cmd.Parameters.AddWithValue("@vid", venueId);
                cmd.Parameters.AddWithValue("@day", day);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getVenueBookings()", ex.Message, -1);

                return null;
            }
        }

        public DataTable isVenueAvailable(string condition, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select bookingDate, bookingPeriod, b.venueId, venueLocation, c.codeValueDisplay as bookingPeriodDisp "
                    + "from venue_booking_record b inner join venue v on b.venueId=v.venueId inner join code_reference c on c.codeType='PERIOD' and c.codeValue=b.bookingPeriod "
                    + "where " + condition;

                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "isVenueAvailable()", ex.Message, -1);

                return null;
            }
        }

        public bool isVenueAvailable(DateTime dt, DayPeriod period, string venueId, int capacity, int existSessionId = -1)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from venue_booking_record b inner join venue v on v.venueID=b.venueID "
                    + "where b.bookingDate=@dt and b.bookingPeriod=@p and b.venueId=@vid and b.defunct='N' ";

                if (existSessionId != -1)
                {
                    cmd.CommandText += "and b.sessionId!=@sid ";
                    cmd.Parameters.AddWithValue("@sid", existSessionId);
                }

                cmd.Parameters.AddWithValue("@dt", dt.ToString("dd-MMM-yyyy"));
                cmd.Parameters.AddWithValue("@p", period.ToString());
                cmd.Parameters.AddWithValue("@vid", venueId);

                if (dbConnection.executeScalarInt(cmd) != 0) return false;

                //check capacity
                cmd.Parameters.Clear();
                cmd.CommandText = "select count(*) from venue v where v.venueID=@vid and v.venueCapacity >= @cap";
                cmd.Parameters.AddWithValue("@vid", venueId);
                cmd.Parameters.AddWithValue("@cap", capacity);

                return dbConnection.executeScalarInt(cmd) == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "isVenueAvailable()", ex.Message, -1);

                return false;
            }
        }

        public bool isVenueAvailableForBatch(DateTime dt, DayPeriod period, string venueId, int programmeBatchId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from venue_booking_record b inner join venue v on v.venueID=b.venueID "
                    + "where b.bookingDate=@dt and b.bookingPeriod=@p and b.venueId=@vid and b.defunct='N' "
                    + "and v.venueCapacity >= (select batchCapacity from programme_batch where programmeBatchId=@pid)";

                cmd.Parameters.AddWithValue("@dt", dt.ToString("dd-MMM-yyyy"));
                cmd.Parameters.AddWithValue("@p", period.ToString());
                cmd.Parameters.AddWithValue("@vid", venueId);
                cmd.Parameters.AddWithValue("@pid", programmeBatchId);

                return dbConnection.executeScalarInt(cmd) == 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "isVenueAvailable()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getBooking(int bookingId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT vbr.bookingId, convert(nvarchar, vbr.bookingDate, 106) as bookingDate, vbr.bookingPeriod, "
                + "vbr.venueId, vbr.sessionId, vbr.bookingRemarks, v.venueLocation, c.CodeValueDisplay as bookingPeriodDisp, pb.batchCode, ms.moduleId, ms.moduleTitle, ms.moduleCode "
                + "FROM [venue_booking_record] as vbr "
                + "INNER JOIN [venue] v ON vbr.venueId = v.venueId and vbr.bookingId = @bookingId AND vbr.defunct = 'N' "
                + "inner join code_reference c on c.codeValue=vbr.bookingPeriod and c.CodeType='PERIOD' "
                + "LEFT JOIN [batchModule_session] bms ON vbr.sessionId = bms.sessionId "
                + "LEFT JOIN [batch_module] bm ON bms.batchModuleId = bm.batchModuleId "
                + "LEFT JOIN [module_structure] ms ON bm.moduleId = ms.moduleId "
                + "LEFT JOIN [programme_batch] pb ON bm.programmeBatchId = pb.programmeBatchId ";

                cmd.Parameters.AddWithValue("@bookingId", bookingId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getBooking()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBooking(string venueId, DateTime bookingStartDate, DateTime bookingEndDate)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT vbr.bookingId, convert(nvarchar, vbr.bookingDate, 106) as bookingDate, vbr.bookingPeriod, vbr.venueId, vbr.sessionId, "
                + "vbr.bookingRemarks, pb.batchCode, ms.moduleId, ms.moduleTitle, ms.moduleCode, cf.codeValueDisplay, v.venueLocation "
                + "FROM [venue_booking_record] as vbr inner join Venue v on v.venueId=vbr.venueId and vbr.defunct = 'N' "
                + (venueId == null || venueId == "" ? "" : "AND vbr.venueId = @venueId ")
                + (bookingStartDate == DateTime.MinValue ? "" : "AND vbr.bookingDate >= @bookingStartDate ")
                + (bookingEndDate == DateTime.MaxValue ? "" : "AND vbr.bookingDate <= @bookingEndDate ")
                + "LEFT JOIN [batchModule_session] bms ON vbr.sessionId = bms.sessionId "
                + "LEFT JOIN [batch_module] bm ON bms.batchModuleId = bm.batchModuleId "
                + "LEFT JOIN [module_structure] ms ON bm.moduleId = ms.moduleId "
                + "LEFT JOIN [programme_batch] pb ON bm.programmeBatchId = pb.programmeBatchId "
                + "LEFT JOIN [code_reference] cf ON vbr.bookingPeriod = cf.codeValue and cf.codeType='PERIOD' "             
                + "order by vbr.bookingDate, cf.codeOrder";

                if (venueId != null && venueId != "") cmd.Parameters.AddWithValue("@venueId", venueId);
                if (bookingStartDate != DateTime.MinValue) cmd.Parameters.AddWithValue("@bookingStartDate", bookingStartDate);
                if (bookingStartDate != DateTime.MaxValue) cmd.Parameters.AddWithValue("@bookingEndDate", bookingEndDate);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getBooking()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBooking(DateTime dt, string venueId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "SELECT vbr.bookingId, cf.codeValue as bookingPeriod, vbr.venueId, vbr.sessionId, "
                + "vbr.bookingRemarks, pb.batchCode, ms.moduleId, ms.moduleTitle, ms.moduleCode, cf.codeValueDisplay, v.venueLocation "
                + "from code_reference cf left outer join venue_booking_record vbr "
                + "on cf.codeValue=vbr.bookingPeriod and vbr.defunct = 'N' and vbr.bookingDate = @dt and vbr.venueId=@vid and vbr.defunct = 'N' "
                + "left outer join Venue v on v.venueId=vbr.venueId  "
                + "LEFT outer JOIN [batchModule_session] bms ON vbr.sessionId = bms.sessionId "
                + "LEFT outer JOIN [batch_module] bm ON bms.batchModuleId = bm.batchModuleId "
                + "LEFT outer JOIN [module_structure] ms ON bm.moduleId = ms.moduleId "
                + "LEFT outer JOIN [programme_batch] pb ON bm.programmeBatchId = pb.programmeBatchId "
                + "where cf.codeType='PERIOD' and cf.defunct='N' "
                + "order by cf.codeOrder";

                cmd.Parameters.AddWithValue("@dt", dt);
                cmd.Parameters.AddWithValue("@vid", venueId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "getBooking()", ex.Message, -1);

                return null;
            }
        }

        public bool updateBooking(int bookingId, string bookingRemark, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "update venue_booking_record set bookingRemarks=@r, lastModifiedBy=@uid, lastModifiedDate=getdate() where bookingId=@bid";

                cmd.Parameters.AddWithValue("@bid", bookingId);
                cmd.Parameters.AddWithValue("@r", bookingRemark);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "updateBooking()", ex.Message, -1);

                return false;
            }
        }

        public bool createBooking(string venueId, DateTime bookingDate, DayPeriod[] bookingPeriod, string bookingRemark, int userId)
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
                    cmd.CommandText = @"INSERT INTO [venue_booking_record] (bookingDate, bookingPeriod, venueId, bookingRemarks, createdBy)
                                      VALUES (@bookingDate, @bookingPeriod, @venueId, @bookingRemarks, @createdBy)";

                    foreach (DayPeriod p in bookingPeriod)
                    {
                        cmd.Parameters.AddWithValue("@bookingDate", bookingDate);
                        cmd.Parameters.AddWithValue("@bookingPeriod", p.ToString());
                        cmd.Parameters.AddWithValue("@venueId", venueId);
                        cmd.Parameters.AddWithValue("@bookingRemarks", bookingRemark);
                        cmd.Parameters.AddWithValue("@createdBy", userId);
                        cmd.ExecuteNonQuery();

                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Bundle.cs", "createBooking()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool deleteBooking(int bookingId, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;
                string defunct = "Y";

                string sqlStatement = @"UPDATE [venue_booking_record] SET defunct = @defunct, lastModifiedBy = @lastModifiedBy, "
                + "lastModifiedDate = @lastModifiedDate WHERE bookingId = @bookingId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@bookingId", bookingId);
                cmd.Parameters.AddWithValue("@defunct", defunct);
                cmd.Parameters.AddWithValue("@lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("@lastModifiedDate", now);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Venue.cs", "deleteBooking()", ex.Message, -1);

                return false;
            }
        }
    }
}