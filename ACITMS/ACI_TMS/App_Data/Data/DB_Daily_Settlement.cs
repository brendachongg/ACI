using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GeneralLayer;
using LogicLayer;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Configuration;


namespace DataLayer
{
    public class DB_Daily_Settlement
    {
        private Database_Connection dbConnection = new Database_Connection();
        Log_Handler logHandler = new Log_Handler();

        public Tuple<bool, string> rejectSettlement(int loginid, int settlementid, string remarks, bool isACI = true)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlConnection conn = dbConnection.getDBConnection();
                conn.Open();
                cmd.Connection = conn;
                string sql = "update daily_settlement set verifiedby = @loginid, lastmodifiedby =@loginid, lastmodifieddate = Getdate(), status =@status ";

                if (isACI)
                    sql += ", rejectRemarksACI = @remarks ";
                else
                    sql += ", rejectRemarksFIN = @remarks ";

                sql += "where dailysettlementid = @id";
                cmd.CommandText = sql;
                cmd.Parameters.AddWithValue("@loginid", loginid);
                cmd.Parameters.AddWithValue("@status", GeneralLayer.DailySettlementStatus.R.ToString());
                cmd.Parameters.AddWithValue("@id", settlementid);
                cmd.Parameters.AddWithValue("@remarks", remarks);
                cmd.ExecuteNonQuery();
                return new Tuple<bool, string>(true, "Record updated successfully");
            }
            catch (Exception ex)
            {
                logHandler.WriteLog(ex, "DB_Daily_Settlement", "confirmSettlement", ex.ToString(), loginid, false);
                return new Tuple<bool, string>(false, "Unable to update");

            }
        }

        public Tuple<bool, string> confirmSettlement(int loginid, int dailysettlementid)
        {

            try
            {
                SqlCommand cmd = new SqlCommand();
                SqlConnection conn = dbConnection.getDBConnection();
                conn.Open();
                cmd.Connection = conn;
                cmd.CommandText = "update daily_settlement set verifiedby = @loginid, lastmodifiedby =@loginid, lastmodifieddate = Getdate(), status =@status where dailysettlementid = @id";
                cmd.Parameters.AddWithValue("@loginid", loginid);
                cmd.Parameters.AddWithValue("@status", GeneralLayer.DailySettlementStatus.V.ToString());
                cmd.Parameters.AddWithValue("@id", dailysettlementid);
                cmd.ExecuteNonQuery();
                return new Tuple<bool, string>(true, "Record updated successfully");
            }
            catch (Exception ex)
            {
                logHandler.WriteLog(ex, "DB_Daily_Settlement", "confirmSettlement", ex.ToString(), loginid, false);
                return new Tuple<bool, string>(false, "Unable to update");

            }


        }

        public DataTable getDailySettlementRecords(int dailysettlementid)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select * from daily_settlement_records where dailysettlementid = @dailysettlementid";
            cmd.Parameters.AddWithValue("@dailysettlementid", dailysettlementid);
            DataTable dtSettlementRecords = dbConnection.getDataTable(cmd);
            return dtSettlementRecords;
        }

        public DataTable getDailySettlment()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "select dailysettlementid, aciuser.username, CONVERT(NVARCHAR,CAST(ds.createddate AS DATETIME), 106) as createon, CONVERT(NVARCHAR,CAST(ds.settlementdate AS DATETIME), 106)as settlementdate from daily_settlement ds left join aci_user aciuser on ds.preparedby = aciuser.userId where ds.status = @status";
            cmd.Parameters.AddWithValue("@status", GeneralLayer.DailySettlementStatus.P.ToString());
            DataTable dtPendingSettlement = dbConnection.getDataTable(cmd);
            return dtPendingSettlement;
        }

        public Tuple<bool, string> insertSettlementRecords(int loginid, DateTime settlementdate, List<DailySettlementRecords> settlementRecords, List<DailySettlementDetails> settlementDetails)
        {
            //cmd.CommandText = "insert into bundle (bundleCode, bundleType, bundleEffectDate, bundleCost, createdBy) values (@bc, @bt, @bdt, @tc, @usr); SELECT CAST(scope_identity() AS int);";
            //cmd.Parameters.AddWithValue("@bc", bundleCode);
            //cmd.Parameters.AddWithValue("@bt", bundleType);
            //cmd.Parameters.AddWithValue("@bdt", bundleEffDate);
            //cmd.Parameters.AddWithValue("@tc", totalCost);
            //cmd.Parameters.AddWithValue("@usr", userId);

            //int bundleId = (int)cmd.ExecuteScalar();
            //cmd.Parameters.Clear();


            using (SqlConnection sqlConn = dbConnection.getDBConnection())
            {
                sqlConn.Open();
                SqlTransaction trans = sqlConn.BeginTransaction();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = sqlConn;
                cmd.Transaction = trans;
                try
                {
                    {
                        cmd.CommandText = "insert into daily_settlement (preparedby, settlementdate, createddate, createdby, status) values (@loginid, @settlementdate, getdate(), @loginid, @status); SELECT CAST(scope_identity() AS int);";
                        cmd.Parameters.AddWithValue("@loginid", loginid);
                        cmd.Parameters.AddWithValue("@settlementdate", settlementdate);
                        cmd.Parameters.AddWithValue("@status", GeneralLayer.DailySettlementStatus.P.ToString());
                        int dailysettlementid = (int)cmd.ExecuteScalar();
                        cmd.Parameters.Clear();

                        foreach (DailySettlementDetails dsd in settlementDetails)
                        {
                            string paymentKey = ConfigurationManager.AppSettings[dsd.paymentMode.ToString()];

                            cmd.CommandText = "insert into daily_settlement_details (dailysettlementid, receiptmethod, receiptamount) values(@dailysettlementid, @rmethod, @ramt);";
                            cmd.Parameters.AddWithValue("@dailysettlementid", dailysettlementid);
                            cmd.Parameters.AddWithValue("@rmethod", paymentKey + "1");
                            cmd.Parameters.AddWithValue("@ramt", dsd.feesCollected);
                            cmd.ExecuteNonQuery();

                            cmd.Parameters.Clear();

                            cmd.CommandText = "insert into daily_settlement_details (dailysettlementid, receiptmethod, receiptamount) values(@dailysettlementid1, @rmethod1, @ramt1);";
                            cmd.Parameters.AddWithValue("@dailysettlementid1", dailysettlementid);
                            cmd.Parameters.AddWithValue("@rmethod1", paymentKey + "2");
                            cmd.Parameters.AddWithValue("@ramt1", dsd.lessSubsidy);
                            cmd.ExecuteNonQuery();


                        }

                        foreach (DailySettlementRecords dsr in settlementRecords)
                        {
                            cmd.CommandText = "insert into daily_settlement_records (dailysettlementid,applicantname,applicantid,programmename,projectcode,coursecode,programmestartdate,programmeenddate,paymentmode,adminfeeswogst,adminfeesgst,adminfeeswgst,coursefeeswogst,coursefeesgst,coursefeeswgst,totalcoursefees,lessschemeamt,totalfeescollected,remarks) values (@dailysettlementid,@applicantname,@applicantid,@programmename,@projectcode,@coursecode,@programmestartdate,@programmeenddate,@paymentmode,@adminfeeswogst,@adminfeesgst,@adminfeeswgst,@coursefeeswogst,@coursefeesgst,@coursefeeswgst,@totalcoursefees,@lessschemeamt,@totalfeescollected,@remarks)";

                            cmd.Parameters.Clear();

                            cmd.Parameters.AddWithValue("@dailysettlementid", dailysettlementid);
                            cmd.Parameters.AddWithValue("@applicantname", dsr.applicantname);
                            cmd.Parameters.AddWithValue("@applicantid", dsr.applicantnric);
                            cmd.Parameters.AddWithValue("@programmename", dsr.programmeName);

                            cmd.Parameters.AddWithValue("@projectcode", dsr.projectCode);
                            cmd.Parameters.AddWithValue("@coursecode", dsr.courseCode);
                            cmd.Parameters.AddWithValue("@programmestartdate", dsr.progStartDate);
                            cmd.Parameters.AddWithValue("@programmeenddate", dsr.progEndDate);


                            cmd.Parameters.AddWithValue("@paymentmode", dsr.paymentMode);
                            cmd.Parameters.AddWithValue("@adminfeeswogst", dsr.adminFeesWOGst);
                            cmd.Parameters.AddWithValue("@adminfeeswgst", dsr.adminFeesWGst);
                            cmd.Parameters.AddWithValue("@adminfeesgst", dsr.adminFeesGst);

                            cmd.Parameters.AddWithValue("@coursefeeswogst", dsr.courseFeesWOGst);
                            cmd.Parameters.AddWithValue("@coursefeesgst", dsr.courseFessGst);
                            cmd.Parameters.AddWithValue("@coursefeeswgst", dsr.courseFeesWGst);
                            cmd.Parameters.AddWithValue("@totalcoursefees", dsr.totalCourseFees);

                            cmd.Parameters.AddWithValue("@lessschemeamt", dsr.lessScheme);
                            cmd.Parameters.AddWithValue("@totalfeescollected", dsr.totalFeesCollected);
                            cmd.Parameters.AddWithValue("@remarks", dsr.remarks == "" ? (object)DBNull.Value : dsr.remarks);

                            cmd.ExecuteNonQuery();



                        }

                        trans.Commit();
                    }
                }

                catch (Exception ex)
                {
                    trans.Rollback();
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Daily_Settlement", "insertSettlementRecords()", "", loginid, false);

                    return new Tuple<bool, string>(false, "Unable to insert...");
                }
                finally
                {
                    sqlConn.Close();
                }
                return new Tuple<bool, string>(true, "AAA");
            }
        }
    }
}