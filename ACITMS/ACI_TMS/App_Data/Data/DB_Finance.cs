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
using System.Globalization;
using System.Text.RegularExpressions;


namespace DataLayer
{

    public class DB_Finance
    {
        //Initialize connection string
        Database_Connection dbConnection = new Database_Connection();

        //Finance to Oracle DB

        public Tuple<bool, string> insertOracleFinance(string paymentMode, decimal fees, string receipttype, string currency, string filepath)
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
                    cmd.CommandText = @"insert into finance(paymentmode, fees, receipttype,currency, filepath) VALUES (@paymentmode, @fees, @receipttype, @currency, @filepath)";

                    cmd.Parameters.AddWithValue("@paymentmode", paymentMode);
                    cmd.Parameters.AddWithValue("@fees", fees);
                    cmd.Parameters.AddWithValue("@receipttype", receipttype);
                    cmd.Parameters.AddWithValue("@currency", currency);
                    cmd.Parameters.AddWithValue("@filepath", filepath);


                    cmd.ExecuteNonQuery();

                    trans.Commit();

                    return new Tuple<bool, string>(true, "Insert into the finance table .");
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "addMakeupPayment()", ex.Message, -1);

                    trans.Rollback();

                    return new Tuple<bool, string>(false, "Unable to insert into the table.");
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable getMakeupPaymentModules(int paymentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select m.moduleId, m.moduleTitle, m.moduleCode, count(*) as noOfSessions
                                    from payment_history ph inner join payment_details pd on ph.paymentId=pd.paymentId and ph.paymentId=@pid
                                    inner join trainee_absence_record a on a.absentId=pd.recordRefId
                                    inner join batch_module bm on bm.batchModuleId=a.batchModuleId
                                    inner join module_structure m on m.moduleId=bm.moduleId
                                    group by m.moduleId, m.moduleTitle, m.moduleCode
                                    union
                                    select m.moduleId, m.moduleTitle, m.moduleCode, count(*) as noOfSessions
                                    from payment_history ph inner join payment_details pd on ph.paymentId=pd.paymentId and ph.paymentId=@pid
                                    inner join trainee_absence_removed a on a.absentId=pd.recordRefId
                                    inner join batch_module bm on bm.batchModuleId=a.batchModuleId
                                    inner join module_structure m on m.moduleId=bm.moduleId
                                    group by m.moduleId, m.moduleTitle, m.moduleCode";

                cmd.Parameters.AddWithValue("@pid", paymentId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getMakeupPaymentModules()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getMakeupPaymentDetails(int paymentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select ph.paymentId, ph.paymentDate, convert(nvarchar, ph.paymentDate, 106) as paymentDateDisp, ph.paymentAmount, ph.paymentMode, 
                                    c1.codeValueDisplay as paymentModeDisp, ph.referenceNumber, ph.paymentRemarks, ph.receiptNumber
                                    from payment_history ph inner join code_reference c1 on c1.codeValue=ph.paymentMode and c1.codeType='PMODE' 
                                    inner join code_reference c2 on c2.codeValue=ph.paymentStatus and c2.codeType='PAYMNT' 
                                    where ph.paymentId=@pid";

                cmd.Parameters.AddWithValue("@pid", paymentId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getMakeupPaymentDetails()", ex.Message, -1);

                return null;
            }
        }

        public bool voidMakeupPayment(int paymentId, DateTime dt, string reason, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update payment_history set paymentStatus=@status, lastModifiedBy=@uid, lastModifiedDate=getdate(), voidDate=@dt, voidBy=@uid, voidReason=@reason "
                    + "where paymentId=@pid";
                cmd.Parameters.AddWithValue("@pid", paymentId);
                cmd.Parameters.AddWithValue("@dt", dt);
                cmd.Parameters.AddWithValue("@reason", reason);
                cmd.Parameters.AddWithValue("@status", PaymentStatus.VOID.ToString());
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "voidMakeupPayment()", ex.Message, -1);

                return false;
            }
        }

        public string getReceiptNumber()
        {
            string sql = "select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            return dbConnection.executeScalarString(cmd);

        }

        public bool updateMakeupPaymentStatus(int paymentId, PaymentStatus status, DateTime dtBankIn, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update payment_history set paymentStatus=@status, " + (dtBankIn != DateTime.MaxValue ? "bankInDate=@bankDt," : "")
                    + (status == PaymentStatus.PAID ? "receiptNumber=(select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history)," : "")
                    + " lastModifiedBy=@uid, lastModifiedDate=getdate() where paymentId=@pid";
                cmd.Parameters.AddWithValue("@pid", paymentId);
                cmd.Parameters.AddWithValue("@status", status.ToString());
                if (dtBankIn != DateTime.MaxValue) cmd.Parameters.AddWithValue("@bankDt", dtBankIn);
                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.executeNonQuery(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "updateMakeupPaymentStatus()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getMakeupPaymentHistory(int absentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select ph.paymentId, ph.paymentDate, convert(nvarchar, ph.paymentDate, 106) as paymentDateDisp, ph.paymentAmount, ph.paymentMode, 
                                    c1.codeValueDisplay as paymentModeDisp, ph.referenceNumber, ph.paymentRemarks, ph.voidDate, convert(nvarchar, ph.voidDate, 106) as voidDateDisp, 
                                    ph.voidBy, u.userName as voidByName, ph.voidReason, ph.paymentStatus, c2.codeValueDisplay as paymentStatusDisp 
                                    from payment_history ph inner join payment_details pd on ph.paymentId=pd.paymentId and recordRefId=@aid 
                                    inner join code_reference c1 on c1.codeValue=ph.paymentMode and c1.codeType='PMODE' 
                                    inner join code_reference c2 on c2.codeValue=ph.paymentStatus and c2.codeType='PAYMNT' 
                                    left outer join aci_user u on u.userId=ph.voidBy 
                                    order by ph.paymentDate";

                cmd.Parameters.AddWithValue("@aid", absentId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getMakeupPaymentHistory()", ex.Message, -1);

                return null;
            }
        }

        public int addMakeupPayment(string traineeId, int batchId, int[] sessions, DateTime dt, PaymentMode mode, string refNum, decimal amt, string remarks, PaymentStatus status, int userId)
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
                    cmd.CommandText = @"insert into payment_history(traineeId, programmeBatchId, paymentDate, paymentAmount, idNumber, paymentMode, receiptNumber, referenceNumber, "
                        + "paymentRemarks, paymentType, paymentStatus, createdBy, createdOn) "
                        + "values (@tid, @bid, @dt, @amt, (select idNumber from trainee where traineeId=@tid), @m, "
                        + (status == PaymentStatus.PAID ? "(select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history)" : "null")
                        + ", @refNum, @remark, '" + PaymentType.MAKEUP.ToString() + "', @s, @uid, getdate()); SELECT CAST(scope_identity() AS int);";
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@bid", batchId);
                    cmd.Parameters.AddWithValue("@dt", dt);
                    cmd.Parameters.AddWithValue("@amt", amt);
                    cmd.Parameters.AddWithValue("@m", mode.ToString());
                    cmd.Parameters.AddWithValue("@refNum", refNum);
                    cmd.Parameters.AddWithValue("@remark", remarks == null ? (object)DBNull.Value : Regex.Replace(remarks, @"\r\n?|\n", " "));
                    cmd.Parameters.AddWithValue("@s", status.ToString());
                    cmd.Parameters.AddWithValue("@uid", userId);

                    int paymentId = (int)cmd.ExecuteScalar();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "insert into payment_details(paymentId, recordRefId) values (@pid, @rid)";
                    foreach (int sid in sessions)
                    {
                        cmd.Parameters.AddWithValue("@pid", paymentId);
                        cmd.Parameters.AddWithValue("@rid", sid);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return paymentId;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "addMakeupPayment()", ex.Message, -1);

                    trans.Rollback();

                    return -1;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public DataTable getTraineeAbsentPaymentDetails(string traineeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //each absent record is only joined with the latest payment record
                cmd.CommandText = @";WITH payment_CTE AS (
                            select a.absentId, max(ph.paymentId) as paymentId, count(*) as noOfPayments
                            from payment_history ph inner join payment_details pd on ph.paymentId=pd.paymentId and ph.paymentType='MAKEUP' and ph.traineeId=@tid
                            inner join trainee_absence_record a on pd.recordRefId=a.absentId and a.defunct='N'
                            group by a.absentId
                        )"
                    + "select a.absentId, a.traineeId, a.batchModuleId, a.sessionId, t.fullName, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, "
                    + "s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, c.codeOrder, m.moduleCode, m.moduleTitle, p.batchCode, "
                    + "ph.paymentId, ph.paymentStatus, c2.codeValueDisplay as paymentStatusDisp, ph.voidDate, ph.voidBy, ph.voidReason, convert(nvarchar, ph.voidDate, 106) as voidDateDisp, "
                    + "u.userName as voidByName, ph.bankInDate, isnull(pp.noOfPayments, 0) as noOfPayments, ph.paymentDate, convert(nvarchar, ph.paymentDate, 106) as paymentDateDisp, "
                    + "ph.paymentAmount, ph.paymentMode, c3.codeValueDisplay as paymentModeDisp, ph.referenceNumber, ph.paymentRemarks "
                    + "from trainee_absence_record a inner join trainee t on a.traineeId=t.traineeId and a.defunct='N' "
                    + "inner join batchModule_session s on s.sessionId=a.sessionId and a.batchModuleId=s.batchModuleId "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId "
                    + "inner join module_structure m on bm.moduleId=m.moduleId "
                    + "inner join programme_batch p on  p.programmeBatchId=bm.programmeBatchId "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + "left outer join payment_CTE pp on pp.absentId=a.absentId "
                    + "left outer join payment_history ph on ph.paymentId=pp.paymentId "
                    + "left outer join code_reference c2 on c2.codeValue=ph.paymentStatus and c2.codeType='PAYMNT' "
                    + "left outer join aci_user u on u.userId=ph.voidBy "
                    + "left outer join code_reference c3 on c3.codeValue=ph.paymentMode and c3.codeType='PMODE' "
                    //only absent record where user has not determine if it is valid or payment contains pending will be shown
                    + "where a.traineeId=@tid and ((a.isAbsentValid is null or a.isAbsentValid='N') or ph.paymentStatus='" + PaymentStatus.PEND.ToString() + "') "
                    + "union "
                    //also include absent record which may have been completed in user want to regenerate receipt
                    + "select a.absentId, a.traineeId, a.batchModuleId, a.sessionId, t.fullName, s.sessionDate, convert(nvarchar, s.sessionDate, 106) as sessionDateDisp, "
                    + "s.sessionPeriod, c.codeValueDisplay as sessionPeriodDisp, c.codeOrder, m.moduleCode, m.moduleTitle, p.batchCode, "
                    + "ph.paymentId, ph.paymentStatus, c2.codeValueDisplay as paymentStatusDisp, ph.voidDate, ph.voidBy, "
                    + "ph.voidReason, convert(nvarchar, ph.voidDate, 106) as voidDateDisp, u.userName as voidByName, ph.bankInDate, isnull(pp.noOfPayments, 0) as noOfPayments, "
                    + "ph.paymentDate, convert(nvarchar, ph.paymentDate, 106) as paymentDateDisp, ph.paymentAmount, ph.paymentMode, c3.codeValueDisplay as paymentModeDisp, ph.referenceNumber, ph.paymentRemarks "
                    + "from trainee_absence_removed a inner join trainee t on a.traineeId=t.traineeId and a.defunct='N' "
                    + "inner join batchModule_session s on s.sessionId=a.sessionId and a.batchModuleId=s.batchModuleId "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId "
                    + "inner join module_structure m on bm.moduleId=m.moduleId "
                    + "inner join programme_batch p on  p.programmeBatchId=bm.programmeBatchId "
                    + "inner join code_reference c on c.codeValue=s.sessionPeriod and c.codeType='PERIOD' "
                    + "left outer join payment_CTE pp on pp.absentId=a.absentId "
                    + "left outer join payment_history ph on ph.paymentId=pp.paymentId "
                    + "left outer join code_reference c2 on c2.codeValue=ph.paymentStatus and c2.codeType='PAYMNT' "
                    + "left outer join aci_user u on u.userId=ph.voidBy "
                    + "left outer join code_reference c3 on c3.codeValue=ph.paymentMode and c3.codeType='PMODE' "
                    //only absent record where user has not determine if it is valid or payment contains pending will be shown
                    + "where a.traineeId=@tid and ((a.isAbsentValid is null or a.isAbsentValid='N') or ph.paymentStatus='" + PaymentStatus.PEND.ToString() + "') "
                    + "order by 6, 10, 12";
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getTraineeAbsentPaymentDetails()", ex.Message, -1);

                return null;
            }
        }

        public decimal getApplnCsePaymentMade(string applicantId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select isnull(sum(paymentAmount), 0) from payment_history where applicantId=@aid and paymentStatus='" + PaymentStatus.PAID.ToString() + "'";
                cmd.Parameters.AddWithValue("@aid", applicantId);

                return dbConnection.executeScalarDecimal(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getApplnCsePaymentMade()", ex.Message, -1);

                return 0;
            }
        }

        public bool checkExistingRegFee(DateTime dt)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from registration_fee where effectiveDate=@dt";
                cmd.Parameters.AddWithValue("@dt", dt);

                return dbConnection.executeScalarInt(cmd) > 0 ? false : true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "checkExistingRegFee()", ex.Message, -1);

                return false;
            }
        }

        public bool addRegFee(DateTime dt, decimal fee, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "insert into registration_fee(registrationFee, effectiveDate, createdBy) values (@fee, @dt, @uid)";
                cmd.Parameters.AddWithValue("@dt", dt);
                cmd.Parameters.AddWithValue("@fee", fee);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "addRegFee()", ex.Message, -1);

                return false;
            }
        }

        public bool updateRegFee(int feeId, decimal fee, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update registration_fee set registrationFee=@fee, lastModifiedBy=@uid, lastModifiedDate=getdate() where feeId=@fid";
                cmd.Parameters.AddWithValue("@fid", feeId);
                cmd.Parameters.AddWithValue("@fee", fee);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "updateRegFee()", ex.Message, -1);

                return false;
            }
        }

        public bool delRegFee(int feeId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                    set CONTEXT_INFO @usrid;
                                    delete from registration_fee where feeId=@fid";
                cmd.Parameters.AddWithValue("@fid", feeId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "delRegFee()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getAllRegFee()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select feeId, registrationFee, format(registrationFee, '#,##0.00') as registrationFeeDisp, effectiveDate, "
                    + "convert(nvarchar, effectiveDate, 106) as effectiveDateDisp from registration_fee";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getAllRegFee()", ex.Message, -1);

                return null;
            }
        }

        public decimal getCurrentRegFee()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select top 1 registrationFee from registration_fee where effectiveDate<=getdate() order by effectiveDate desc";

                DataTable dt = dbConnection.getDataTable(cmd);
                if (dt.Rows.Count == 0) return 0;
                else return (decimal)dt.Rows[0][0];
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getCurrentRegFee()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getPaymentModes()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PMODE' and defunct='N'";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getPaymentModes()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAvailableSubsidy(int programmeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select subsidyId, subsidyType, subsidyValue, subsidyScheme from subsidy where (programmeId=@pid or programmeId is null) "
                    + "and defunct='N' and effectiveDate<=DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0)";
                cmd.Parameters.AddWithValue("@pid", programmeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getPaymentModes()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getAllSubsidy()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select s.subsidyId, s.subsidyType, c.CodeValueDisplay as subsidyTypeDisp, s.subsidyValue, format(s.subsidyValue, '#,##0.00') as subsidyValueDisp, "
                    + "s.subsidyScheme, s.effectiveDate, convert(nvarchar, s.effectiveDate, 106) as effectiveDateDisp, s.programmeId, p.programmeCode, p.programmeTitle, "
                    + "count(a.subsidyId) as isUsed "
                    + "from subsidy s inner join code_reference c on s.subsidyType=c.codeValue and c.codeType='SUBTYP' and s.defunct='N' "
                    + "left outer join programme_structure p on p.programmeId=s.programmeId "
                    + "left outer join applicant a on a.subsidyId=s.subsidyId "
                    + "group by s.subsidyId, s.subsidyType, c.CodeValueDisplay, s.subsidyValue, s.subsidyScheme, s.effectiveDate, s.programmeId, p.programmeCode, p.programmeTitle";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getAllSubsidy()", ex.Message, -1);

                return null;
            }
        }

        public bool isExistingSubsidy(string scheme, int progId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from subsidy where UPPER(subsidyScheme)=@s and defunct='N' ";
                if (progId != -1)
                {
                    cmd.CommandText += "and programmeId=@pid ";
                    cmd.Parameters.AddWithValue("@pid", progId);
                }
                else cmd.CommandText += "and programmeId is null ";

                cmd.Parameters.AddWithValue("@s", scheme.ToUpper());

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "isExistingSubsidy()", ex.Message, -1);

                return false;
            }
        }

        public bool addSubsidy(string scheme, int progId, string type, decimal value, DateTime effDt, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "insert into subsidy(subsidyType, subsidyValue, programmeId, effectiveDate, subsidyScheme, createdBy) values (@t, @v, @pid, @dt, @s, @uid)";
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@s", scheme);
                cmd.Parameters.AddWithValue("@pid", progId == -1 ? DBNull.Value : (object)progId);
                cmd.Parameters.AddWithValue("@dt", effDt);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "addSubsidy()", ex.Message, -1);

                return false;
            }
        }

        public bool updateSubsidy(int subId, string type, decimal value, DateTime effDt, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update subsidy set subsidyType=@t, subsidyValue=@v, effectiveDate=@dt, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                    + "where subsidyId=@sid";
                cmd.Parameters.AddWithValue("@sid", subId);
                cmd.Parameters.AddWithValue("@t", type);
                cmd.Parameters.AddWithValue("@dt", effDt);
                cmd.Parameters.AddWithValue("@v", value);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "updateSubsidy()", ex.Message, -1);

                return false;
            }
        }

        public bool delSubsidy(int subId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update subsidy set defunct='Y', lastModifiedBy=@uid, lastModifiedDate=getdate() where subsidyId=@sid";
                cmd.Parameters.AddWithValue("@sid", subId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "delSubsidy()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getSubsidyTypes()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='SUBTYP' and defunct='N'";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getSubsidyTypes()", ex.Message, -1);

                return null;
            }
        }

        public string getApplnClassReceiptNum(string applicantId, PaymentType pymType)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select distinct receiptNumber from payment_history where applicantId=@aid and paymentType=@pType";
                cmd.Parameters.AddWithValue("@aid", applicantId);
                cmd.Parameters.AddWithValue("@pType", pymType.ToString());

                string tmp = dbConnection.executeScalarString(cmd);
                return tmp == null ? "" : tmp;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getReceiptNum()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getApplnClassPaymentTypes(string applicantId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select distinct paymentType from payment_history where applicantId=@aid and paymentType in ('" + PaymentType.REG.ToString() + "', '" + PaymentType.PROG.ToString() + "', '" + PaymentType.BOTH.ToString() + "')";
                cmd.Parameters.AddWithValue("@aid", applicantId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getApplnClassPaymentTypes()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeClassPaymentTypes(string traineeId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select distinct paymentType from payment_history where traineeId=@tid and paymentType in ('" + PaymentType.REG.ToString() + "', '" + PaymentType.PROG.ToString() + "', '" + PaymentType.BOTH.ToString() + "')";
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getTraineeClassPaymentTypes()", ex.Message, -1);

                return null;
            }
        }

        public int getNoOfOutstandingClassPayments()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select sum(num) from ( select count(*) as num "
                                    + "from applicant a inner join code_reference c on c.codeValue='" + PaymentType.BOTH.ToString() + "' and c.codeType='PYTYPE' "
                                    + "inner join programme_batch pb on pb.programmeBatchId=a.programmeBatchId "
                                    + "where a.rejectstatus = 'N' and (a.registrationFee is null or a.GSTPayableAmount is null) "
                                    + "union "
                                    + "select count(*) as num "
                                    + "from applicant a inner join ( "
                                    + "select sum(paymentAmount) as paymentAmount, applicantId, paymentType "
                                    + "from payment_history where paymentStatus in ('" + PaymentStatus.PAID.ToString() + "', '" + PaymentStatus.WAIVED.ToString() + "') and applicantId is not null "
                                    + "group by applicantId, paymentType) p on a.applicantId=p.applicantId "
                                    + "inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'"
                                    + "inner join programme_batch pb on pb.programmeBatchId=a.programmeBatchId "
                                    + "where a.rejectstatus = 'N' and a.registrationFee is not null and a.GSTPayableAmount is not null "
                                    + "and  ( "
                                    + "    (p.paymentType='" + PaymentType.REG.ToString() + "' and round(a.registrationFee * 1.07, 2) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.PROG.ToString() + "' and (a.GSTPayableAmount+a.programmePayableAmount-isnull(a.subsidyAmt, 0)) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.BOTH.ToString() + "' and (a.registrationFee+a.GSTPayableAmount+a.programmePayableAmount-isnull(a.subsidyAmt, 0)) <> p.paymentAmount) "
                                    + ") "
                                    + "union "
                                    + "select count(*) as num "
                                    + "from trainee t inner join trainee_programme tp on t.traineeId=tp.traineeId "
                                    + "inner join ( "
                                    + "select sum(paymentAmount) as paymentAmount, traineeId, paymentType "
                                    + "from payment_history where paymentStatus in ('" + PaymentStatus.PAID.ToString() + "', '" + PaymentStatus.WAIVED.ToString() + "') and traineeId is not null "
                                    + "group by traineeId, paymentType) p on t.traineeId=p.traineeId "
                                    + "inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE' "
                                    + "inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId "
                                    + "where  ( "
                                    + "    (p.paymentType='" + PaymentType.REG.ToString() + "' and round(tp.registrationFee * 1.07, 2) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.PROG.ToString() + "' and (tp.GSTPayableAmount+tp.programmePayableAmount-isnull(tp.subsidyAmt, 0)) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.BOTH.ToString() + "' and (tp.registrationFee+tp.GSTPayableAmount+tp.programmePayableAmount-isnull(tp.subsidyAmt, 0)) <> p.paymentAmount) "
                                    + ") ) tbl";

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getNoOfOutstandingClassPayments()", ex.Message, -1);

                return -1;
            }
        }

        public DataTable getOutstandingClassPayments()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select null as paymentId, a.applicantId as userId, 'BOTH' as paymentType, c.codeValueDisplay as paymentTypeDisp, a.fullName, a.programmeBatchId, pb.batchCode, 'A' as userType, 'N' as canDel "
                                    + "from applicant a inner join code_reference c on c.codeValue='" + PaymentType.BOTH.ToString() + "' and c.codeType='PYTYPE' "
                                    + "inner join programme_batch pb on pb.programmeBatchId=a.programmeBatchId "
                                    + "where a.rejectstatus = 'N' and (a.registrationFee is null or a.GSTPayableAmount is null) "
                                    + "union "
                                    + "select null as paymentId, a.applicantId as userId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, a.fullName, a.programmeBatchId, pb.batchCode, 'A' as userType, 'Y' as canDel "
                                    + "from applicant a inner join ( "
                                    + "select sum(paymentAmount) as paymentAmount, applicantId, paymentType "
                                    + "from payment_history where paymentStatus in ('" + PaymentStatus.PAID.ToString() + "', '" + PaymentStatus.WAIVED.ToString() + "') and applicantId is not null "
                                    + "group by applicantId, paymentType) p on a.applicantId=p.applicantId "
                                    + "inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'"
                                    + "inner join programme_batch pb on pb.programmeBatchId=a.programmeBatchId "
                                    + "where a.rejectstatus = 'N' and a.registrationFee is not null and a.GSTPayableAmount is not null "
                                    + "and  ( "
                                    + "    (p.paymentType='" + PaymentType.REG.ToString() + "' and round(a.registrationFee * 1.07, 2) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.PROG.ToString() + "' and (a.GSTPayableAmount+a.programmePayableAmount-isnull(a.subsidyAmt, 0)) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.BOTH.ToString() + "' and (a.registrationFee+a.GSTPayableAmount+a.programmePayableAmount-isnull(a.subsidyAmt, 0)) <> p.paymentAmount) "
                                    + ") "
                                    + "union "
                                    + "select null as paymentId, t.traineeId as userId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, t.fullName, tp.programmeBatchId, pb.batchCode, 'T' as userType, 'N' as canDel "
                                    + "from trainee t inner join trainee_programme tp on t.traineeId=tp.traineeId "
                                    + "inner join ( "
                                    + "select sum(paymentAmount) as paymentAmount, traineeId, paymentType "
                                    + "from payment_history where paymentStatus in ('" + PaymentStatus.PAID.ToString() + "', '" + PaymentStatus.WAIVED.ToString() + "') and traineeId is not null "
                                    + "group by traineeId, paymentType) p on t.traineeId=p.traineeId "
                                    + "inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE' "
                                    + "inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId "
                                    + "where  ( "
                                    + "    (p.paymentType='" + PaymentType.REG.ToString() + "' and round(tp.registrationFee * 1.07, 2) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.PROG.ToString() + "' and (tp.GSTPayableAmount+tp.programmePayableAmount-isnull(tp.subsidyAmt, 0)) <> p.paymentAmount) or "
                                    + "    (p.paymentType='" + PaymentType.BOTH.ToString() + "' and (tp.registrationFee+tp.GSTPayableAmount+tp.programmePayableAmount-isnull(tp.subsidyAmt, 0)) <> p.paymentAmount) "
                                    + ") "
                                    + "order by 5, 7";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "searchPaymegetOutstandingClassPaymentsntsByTrainee()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchPaymentsByTrainee(string searchValue)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select null as paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, t.traineeId as userId, p.programmeBatchId, t.fullName, b.batchCode, 'T' as userType,
                        'N' as canDel
                        from payment_history p inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'
                        inner join trainee t on t.traineeId=p.traineeId inner join programme_batch b on b.programmeBatchId=p.programmeBatchId 
                        where (UPPER(t.traineeId) like @sv or UPPER(t.fullName) like @sv) and 
                        p.paymentType in ('" + PaymentType.BOTH.ToString() + @"','" + PaymentType.REG.ToString() + @"','" + PaymentType.PROG.ToString() + @"')
                        group by p.paymentType, c.codeValueDisplay, t.traineeId, p.programmeBatchId, t.fullName, b.batchCode
                        union
                        select p.paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, t.traineeId as userId, p.programmeBatchId, t.fullName, b.batchCode, 'T' as userType,
                        'Y' as canDel
                        from payment_history p inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'
                        inner join trainee t on t.traineeId=p.traineeId inner join programme_batch b on b.programmeBatchId=p.programmeBatchId 
                        where (UPPER(t.traineeId) like @sv or UPPER(t.fullName) like @sv) and 
                        p.paymentType not in ('" + PaymentType.BOTH.ToString() + @"','" + PaymentType.REG.ToString() + @"','" + PaymentType.PROG.ToString() + @"')
                        group by p.paymentId, p.paymentType, c.codeValueDisplay, t.traineeId, p.programmeBatchId, t.fullName, b.batchCode
                        order by 6,7";

                cmd.Parameters.AddWithValue("@sv", "%" + searchValue.ToUpper() + "%");

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "searchPaymentsByTrainee()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchPaymentsByApplicant(string searchValue)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select null as paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, p.applicantId as userId, p.programmeBatchId, a.fullName, b.batchCode, 'A' as userType, 'Y' as canDel
                        from payment_history p inner join applicant a on p.applicantId=a.applicantId
                        inner join programme_batch b on b.programmeBatchId=p.programmeBatchId inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE' 
                        where (UPPER(p.applicantId) like @sv or UPPER(a.fullName) like @sv)
                        group by p.paymentType, c.codeValueDisplay, p.applicantId, p.programmeBatchId, a.fullName, b.batchCode 
                        order by 6,7";

                cmd.Parameters.AddWithValue("@sv", "%" + searchValue.ToUpper() + "%");

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "searchPaymentsByApplicant()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchPaymentsByRef(string refNum)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
                        select null as paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, p.applicantId as userId, p.programmeBatchId, a.fullName, b.batchCode, 'A' as userType, 'Y' as canDel
                        from payment_history p inner join applicant a on p.applicantId=a.applicantId
                        inner join programme_batch b on b.programmeBatchId=p.programmeBatchId inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE' 
                        where UPPER(p.referenceNumber) like @refNum
                        group by p.paymentType, c.codeValueDisplay, p.applicantId, p.programmeBatchId, a.fullName, b.batchCode 
                        union
                        select p.paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, t.traineeId as userId, p.programmeBatchId, t.fullName, b.batchCode, 'T' as userType,
                        case when p.paymentType='BOTH' or p.paymentType='REG' or p.paymentType='PROG' then 'N' else 'Y' end as canDel
                        from payment_history p inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'
                        inner join trainee t on t.traineeId=p.traineeId inner join programme_batch b on b.programmeBatchId=p.programmeBatchId 
                        where UPPER(p.referenceNumber) like @refNum
                        group by p.paymentId, p.paymentType, c.codeValueDisplay, t.traineeId, p.programmeBatchId, t.fullName, b.batchCode) t
                        order by 6,7";
                cmd.Parameters.AddWithValue("@refNum", "%" + refNum.ToUpper() + "%");

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "searchPaymentsByRef()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchPaymentsByDate(DateTime dt)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select * from (
                        select null as paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, p.applicantId as userId, p.programmeBatchId, a.fullName, b.batchCode, 'A' as userType, 'Y' as canDel
                        from payment_history p inner join applicant a on p.applicantId=a.applicantId
                        inner join programme_batch b on b.programmeBatchId=p.programmeBatchId inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE' 
                        where p.paymentDate=@dt
                        group by p.paymentType, c.codeValueDisplay, p.applicantId, p.programmeBatchId, a.fullName, b.batchCode 
                        union
                        select p.paymentId, p.paymentType, c.codeValueDisplay as paymentTypeDisp, t.traineeId as userId, p.programmeBatchId, t.fullName, b.batchCode, 'T' as userType,
                        case when p.paymentType='BOTH' or p.paymentType='REG' or p.paymentType='PROG' then 'N' else 'Y' end as canDel
                        from payment_history p inner join code_reference c on c.codeValue=p.paymentType and c.codeType='PYTYPE'
                        inner join trainee t on t.traineeId=p.traineeId inner join programme_batch b on b.programmeBatchId=p.programmeBatchId 
                        where p.paymentDate=@dt
                        group by p.paymentId, p.paymentType, c.codeValueDisplay, t.traineeId, p.programmeBatchId, t.fullName, b.batchCode) t
                        order by 6,7";
                cmd.Parameters.AddWithValue("@dt", dt);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "searchPaymentsByDate()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getApplnClassPaymentDetails(string applicantId, PaymentType pymType, bool includeVoid = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select ph.paymentId, ph.paymentMode, c.codeValueDisplay as paymentModeDisp, ph.referenceNumber, convert(nvarchar, ph.paymentDate, 106) as paymentDate, "
                    + "ph.paymentAmount, ph.paymentRemarks, ph.paymentStatus, c1.codeValueDisplay as paymentStatusDisp, ph.bankInDate, "
                    + "ph.paymentType, ph.bankInDate, ph.voidDate, ph.voidBy, u.UserName as voidByName, ph.voidReason, ph.receiptNumber "
                    + "from payment_history ph inner join code_reference c on c.codeValue=ph.paymentMode and c.codeType='PMODE' "
                    + "inner join code_reference c1 on c1.codeValue=ph.paymentStatus and c1.codeType='PAYMNT' "
                    + "left outer join aci_user u on ph.voidBy=u.userId "
                    + "where ph.applicantId=@aid " + (includeVoid ? "" : " and ph.paymentStatus!='" + PaymentStatus.VOID.ToString() + "' ")
                    + "and ph.paymentType='" + pymType.ToString() + "' order by ph.paymentDate ";

                cmd.Parameters.AddWithValue("@aid", applicantId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getApplnClassPaymentDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeClassPaymentDetails(string traineeId, PaymentType pymType, bool includeVoid = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select ph.paymentId, ph.paymentMode, c.codeValueDisplay as paymentModeDisp, ph.referenceNumber, convert(nvarchar, ph.paymentDate, 106) as paymentDate, "
                    + "ph.paymentAmount, ph.paymentRemarks, ph.paymentStatus, c1.codeValueDisplay as paymentStatusDisp, ph.bankInDate, "
                    + "ph.paymentType, ph.bankInDate, ph.voidDate, ph.voidBy, u.UserName as voidByName, ph.voidReason, ph.receiptNumber "
                    + "from payment_history ph inner join code_reference c on c.codeValue=ph.paymentMode and c.codeType='PMODE' "
                    + "inner join code_reference c1 on c1.codeValue=ph.paymentStatus and c1.codeType='PAYMNT' "
                    + "left outer join aci_user u on ph.voidBy=u.userId "
                    + "where ph.traineeId=@tid " + (includeVoid ? "" : " and ph.paymentStatus!='" + PaymentStatus.VOID.ToString() + "' ")
                    + "and ph.paymentType='" + pymType.ToString() + "' order by ph.paymentDate ";

                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getTraineeClassPaymentDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getClassPaymentDetails(int paymentId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select ph.paymentId, ph.paymentMode, c.codeValueDisplay as paymentModeDisp, ph.referenceNumber, convert(nvarchar, ph.paymentDate, 106) as paymentDate, "
                    + "ph.paymentAmount, ph.paymentRemarks, ph.paymentStatus, c1.codeValueDisplay as paymentStatusDisp, "
                    + "ph.paymentType, ph.bankInDate, ph.voidDate, ph.voidBy, u.UserName as voidByName, ph.voidReason, ph.receiptNumber "
                    + "from payment_history ph inner join code_reference c on c.codeValue=ph.paymentMode and c.codeType='PMODE' "
                    + "inner join code_reference c1 on c1.codeValue=ph.paymentStatus and c1.codeType='PAYMNT' "
                    + "left outer join aci_user u on ph.voidBy=u.userId "
                    + "where ph.paymentId=@pid ";

                cmd.Parameters.AddWithValue("@pid", paymentId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getClassPaymentDetails()", ex.Message, -1);

                return null;
            }
        }

        public bool delMakeupPayment(int paymentId, int userId)
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
                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from payment_history where paymentId=@pid;";
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@pid", paymentId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from payment_details where paymentId=@pid;";
                    cmd.Parameters.AddWithValue("@pid", paymentId);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "delMakeupPayment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool delApplnClassPayment(string applicantId, PaymentType pymType, int userId)
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
                    cmd.CommandText = @"declare @usrid varbinary(32)=CAST (@uid AS binary);
                                        set CONTEXT_INFO @usrid;
                                        delete from payment_history where applicantId=@aid and paymentType=@ptype;";
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@aid", applicantId);
                    cmd.Parameters.AddWithValue("@ptype", pymType.ToString());

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    if (pymType == PaymentType.REG)
                        cmd.CommandText = "update applicant set registrationFee=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where applicantId=@aid";
                    else if (pymType == PaymentType.PROG)
                        cmd.CommandText = "update applicant set subsidyId=null, subsidyAmt=null, GSTPayableAmount=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where applicantId=@aid";
                    else
                        cmd.CommandText = "update applicant set registrationFee=null, subsidyId=null, subsidyAmt=null, GSTPayableAmount=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where applicantId=@aid";

                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.Parameters.AddWithValue("@aid", applicantId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "delApplnClassPayment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool updateApplSubsidy(string applicantId, decimal regFee, int subsidyId, decimal subFee, decimal gst, int userId)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection sqlConn = dbConnection.getDBConnection();
            sqlConn.Open();
            cmd.Connection = sqlConn;

            cmd.CommandText = "update applicant set subsidyId=@sid, subsidyAmt=@sAmt, lastModifiedBy=@uid, lastModifiedDate=getdate() ";

            if (regFee != -1)
            {
                cmd.CommandText += ", registrationFee=@reg ";
                cmd.Parameters.AddWithValue("@reg", regFee);
            }

            //if payment type is combined or for programme fees, save the gst
            cmd.CommandText += ", GSTPayableAmount=@gst ";
            cmd.Parameters.AddWithValue("@gst", gst);

            cmd.CommandText += "where applicantId=@appId";

            cmd.Parameters.AddWithValue("@appId", applicantId);
            cmd.Parameters.AddWithValue("@sid", subsidyId == -1 ? DBNull.Value : (object)subsidyId);
            cmd.Parameters.AddWithValue("@sAmt", subsidyId == -1 ? DBNull.Value : (object)subFee);
            cmd.Parameters.AddWithValue("@uid", userId);

            cmd.ExecuteNonQuery();
            cmd.Parameters.Clear();

            return true;
        }

        public bool insertOnlineApplnPayment(string applicantid, decimal paymentAmt, string paymentType, string billReferenceNo)
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
                    string insertSql = "insert into payment_history (applicantId, programmeBatchId, paymentDate, paymentAmount, idNumber, paymentMode, "
                      + "referenceNumber,paymentType, paymentStatus, createdBy, receiptNumber) values "
                      + "(@appId, (select programmeBatchId from applicant where applicantid = @appId), GetDate(), @pAmt, (select idNumber from applicant where applicantid = @appId), @pMode, @ref, @pType, @pStatus, -1,(select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history))";
                    cmd.CommandText = insertSql;

                    cmd.Parameters.AddWithValue("@appId", applicantid);
                    cmd.Parameters.AddWithValue("@pAmt", paymentAmt);
                    cmd.Parameters.AddWithValue("@pMode", GeneralLayer.PaymentMode.NETS.ToString());
                    cmd.Parameters.AddWithValue("@ref", billReferenceNo);
                    cmd.Parameters.AddWithValue("@pType", paymentType);
                    cmd.Parameters.AddWithValue("@pStatus", GeneralLayer.PaymentStatus.PAID.ToString());   

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();
                        
                    return true;

                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "updateApplnClassPayment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }

        }

        public bool updateApplnClassPayment(string applicantId, int programmeBatchId, string idNumber, decimal regFee, int subsidyId, decimal subFee, decimal gst, DataTable dt, int userId)
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
                    string insertSql = "insert into payment_history (applicantId, programmeBatchId, paymentDate, paymentAmount, idNumber, paymentMode, bankInDate, "
                        + "referenceNumber, paymentRemarks, voidDate, voidBy, voidReason, paymentType, paymentStatus, createdBy, receiptNumber) values "
                        + "(@appId, @bid, @pDt, @pAmt, @id, @pMode, @bankDt, @ref, @remarks, @vDt, @vBy, @vReason, @pType, @pStatus, @uid, @rcpt); ";
                    string updateSql = "update payment_history set referenceNumber=@ref, paymentRemarks=@remarks, receiptNumber=@rcpt, voidDate=@vDt, voidBy=@vBy, voidReason=@vReason, bankInDate=@bankDt, paymentStatus=@pStatus, lastModifiedBy=@uid, lastModifiedDate=getdate() where paymentId=@pid";

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["paymentId"] == DBNull.Value)
                        {
                            cmd.CommandText = insertSql;
                            cmd.Parameters.AddWithValue("@appId", applicantId);
                            cmd.Parameters.AddWithValue("@bid", programmeBatchId);
                            cmd.Parameters.AddWithValue("@pDt", DateTime.ParseExact(dr["paymentDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@pAmt", dr["paymentAmount"]);
                            cmd.Parameters.AddWithValue("@id", idNumber);
                            cmd.Parameters.AddWithValue("@pMode", dr["paymentMode"].ToString());
                            cmd.Parameters.AddWithValue("@bankDt", dr["bankInDate"]);
                            cmd.Parameters.AddWithValue("@ref", dr["referenceNumber"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", dr["paymentRemarks"] == DBNull.Value ? (object)DBNull.Value : Regex.Replace(dr["paymentRemarks"].ToString(), @"\r\n?|\n", " "));
                            cmd.Parameters.AddWithValue("@vDt", dr["voidDate"]);
                            cmd.Parameters.AddWithValue("@vBy", dr["voidBy"]);
                            cmd.Parameters.AddWithValue("@vReason", dr["voidReason"]);
                            cmd.Parameters.AddWithValue("@pType", dr["paymentType"].ToString());
                            cmd.Parameters.AddWithValue("@pStatus", dr["paymentStatus"].ToString());
                            cmd.Parameters.AddWithValue("@uid", userId);

                            if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
                                cmd.CommandText = cmd.CommandText.Replace("@rcpt", "(select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history)");
                            else cmd.CommandText = cmd.CommandText.Replace("@rcpt", "null");

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        else
                        {
                            cmd.CommandText = updateSql;
                            cmd.Parameters.AddWithValue("@ref", dr["referenceNumber"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", dr["paymentRemarks"] == DBNull.Value ? (object)DBNull.Value : Regex.Replace(dr["paymentRemarks"].ToString(), @"\r\n?|\n", " "));
                            cmd.Parameters.AddWithValue("@pid", dr["paymentId"]);
                            cmd.Parameters.AddWithValue("@vDt", dr["voidDate"]);
                            cmd.Parameters.AddWithValue("@vBy", dr["voidBy"]);
                            cmd.Parameters.AddWithValue("@vReason", dr["voidReason"]);
                            cmd.Parameters.AddWithValue("@bankDt", dr["bankInDate"]);
                            cmd.Parameters.AddWithValue("@pStatus", dr["paymentStatus"].ToString());
                            cmd.Parameters.AddWithValue("@uid", userId);

                            if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
                                cmd.CommandText = cmd.CommandText.Replace("@rcpt", "(case when receiptNumber is null then (select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history) else receiptNumber end)");
                            else cmd.CommandText = cmd.CommandText.Replace("@rcpt", "receiptNumber");

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                    }


                    cmd.CommandText = "update applicant set subsidyId=@sid, subsidyAmt=@sAmt, lastModifiedBy=@uid, lastModifiedDate=getdate() ";

                    if (regFee != -1)
                    {
                        cmd.CommandText += ", registrationFee=@reg ";
                        cmd.Parameters.AddWithValue("@reg", regFee);
                    }
                    if (dt.Rows[0]["paymentType"].ToString() != PaymentType.REG.ToString())
                    {
                        //if payment type is combined or for programme fees, save the gst
                        cmd.CommandText += ", GSTPayableAmount=@gst ";
                        cmd.Parameters.AddWithValue("@gst", gst);
                    }
                    cmd.CommandText += "where applicantId=@appId";

                    cmd.Parameters.AddWithValue("@appId", applicantId);
                    cmd.Parameters.AddWithValue("@sid", subsidyId == -1 ? DBNull.Value : (object)subsidyId);
                    cmd.Parameters.AddWithValue("@sAmt", subsidyId == -1 ? DBNull.Value : (object)subFee);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "updateApplnClassPayment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool updateTraineeClassPayment(string traineeId, int programmeBatchId, string idNumber, decimal regFee, int subsidyId, decimal subFee, decimal gst, DataTable dt, int userId)
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
                    string insertSql = "insert into payment_history (traineeId, programmeBatchId, paymentDate, paymentAmount, idNumber, paymentMode, bankInDate, "
                        + "referenceNumber, paymentRemarks, voidDate, voidBy, voidReason, paymentType, paymentStatus, createdBy, receiptNumber) values "
                        + "(@trId, @bid, @pDt, @pAmt, @id, @pMode, @bankDt, @ref, @remarks, @vDt, @vBy, @vReason, @pType, @pStatus, @uid, @rcpt); ";
                    string updateSql = "update payment_history set referenceNumber=@ref, paymentRemarks=@remarks, receiptNumber=@rcpt, voidDate=@vDt, voidBy=@vBy, voidReason=@vReason, bankInDate=@bankDt, paymentStatus=@pStatus, lastModifiedBy=@uid, lastModifiedDate=getdate() where paymentId=@pid";

                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr["paymentId"] == DBNull.Value)
                        {
                            cmd.CommandText = insertSql;
                            cmd.Parameters.AddWithValue("@trId", traineeId);
                            cmd.Parameters.AddWithValue("@bid", programmeBatchId);
                            cmd.Parameters.AddWithValue("@pDt", DateTime.ParseExact(dr["paymentDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture));
                            cmd.Parameters.AddWithValue("@pAmt", dr["paymentAmount"]);
                            cmd.Parameters.AddWithValue("@id", idNumber);
                            cmd.Parameters.AddWithValue("@pMode", dr["paymentMode"].ToString());
                            cmd.Parameters.AddWithValue("@bankDt", dr["bankInDate"]);
                            cmd.Parameters.AddWithValue("@ref", dr["referenceNumber"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", dr["paymentRemarks"] == DBNull.Value ? (object)DBNull.Value : Regex.Replace(dr["paymentRemarks"].ToString(), @"\r\n?|\n", " "));
                            cmd.Parameters.AddWithValue("@vDt", dr["voidDate"]);
                            cmd.Parameters.AddWithValue("@vBy", dr["voidBy"]);
                            cmd.Parameters.AddWithValue("@vReason", dr["voidReason"]);
                            cmd.Parameters.AddWithValue("@pType", dr["paymentType"].ToString());
                            cmd.Parameters.AddWithValue("@pStatus", dr["paymentStatus"].ToString());
                            cmd.Parameters.AddWithValue("@uid", userId);

                            if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
                                cmd.CommandText = cmd.CommandText.Replace("@rcpt", "(select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history)");
                            else cmd.CommandText = cmd.CommandText.Replace("@rcpt", "null");

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }
                        else
                        {
                            cmd.CommandText = updateSql;
                            cmd.Parameters.AddWithValue("@ref", dr["referenceNumber"].ToString());
                            cmd.Parameters.AddWithValue("@remarks", dr["paymentRemarks"] == DBNull.Value ? (object)DBNull.Value : Regex.Replace(dr["paymentRemarks"].ToString(), @"\r\n?|\n", " "));
                            cmd.Parameters.AddWithValue("@pid", dr["paymentId"]);
                            cmd.Parameters.AddWithValue("@vDt", dr["voidDate"]);
                            cmd.Parameters.AddWithValue("@vBy", dr["voidBy"]);
                            cmd.Parameters.AddWithValue("@vReason", dr["voidReason"]);
                            cmd.Parameters.AddWithValue("@bankDt", dr["bankInDate"]);
                            cmd.Parameters.AddWithValue("@pStatus", dr["paymentStatus"].ToString());
                            cmd.Parameters.AddWithValue("@uid", userId);

                            if (dr["paymentStatus"].ToString() == PaymentStatus.PAID.ToString())
                                cmd.CommandText = cmd.CommandText.Replace("@rcpt", "(case when receiptNumber is null then (select 'ACIS' + REPLACE(STR(isnull(max(right(receiptNumber, 7)),0)+1, 7), SPACE(1), '0')  from payment_history) else receiptNumber end)");
                            else cmd.CommandText = cmd.CommandText.Replace("@rcpt", "receiptNumber");

                            cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                        }

                    }


                    cmd.CommandText = "update trainee_programme set subsidyId=@sid, subsidyAmt=@sAmt, lastModifiedBy=@uid, lastModifiedDate=getdate() ";

                    if (regFee != -1)
                    {
                        cmd.CommandText += ", registrationFee=@reg ";
                        cmd.Parameters.AddWithValue("@reg", regFee);
                    }
                    if (dt.Rows[0]["paymentType"].ToString() != PaymentType.REG.ToString())
                    {
                        //if payment type is combined or for programme fees, save the gst
                        cmd.CommandText += ", GSTPayableAmount=@gst ";
                        cmd.Parameters.AddWithValue("@gst", gst);
                    }
                    cmd.CommandText += "where traineeId=@trId";

                    cmd.Parameters.AddWithValue("@trId", traineeId);
                    cmd.Parameters.AddWithValue("@sid", subsidyId == -1 ? DBNull.Value : (object)subsidyId);
                    cmd.Parameters.AddWithValue("@sAmt", subsidyId == -1 ? DBNull.Value : (object)subFee);
                    cmd.Parameters.AddWithValue("@uid", userId);

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "updateTraineeClassPayment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }



        //Get payment history by applicantId
        public DataTable getPaymentHistoryByApplicantId(string applicantId)
        {
            try
            {
                string sqlStatement = @"SELECT *
                                    FROM [payment_history] as payment
                                                                        
                                    WHERE payment.applicantId = @applicantId and paymentStatus <> @paymentStatus";

                SqlCommand cmd = new SqlCommand(sqlStatement);
                cmd.Parameters.AddWithValue("@applicantId", applicantId);
                cmd.Parameters.AddWithValue("@paymentStatus", PaymentStatus.VOID.ToString());
                DataTable dtPaymentHistory = dbConnection.getDataTable(cmd);

                return dtPaymentHistory;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getPaymentHistoryByApplicantId()", ex.Message, -1);

                return null;
            }
        }

        // retrieve void receipt number by applicant Id this is used once registration receipt is voided.
        //this will be used in applicant details. aspx
        public string getReptNumberREG(string applicantId)
        {
            SqlConnection conn = dbConnection.getDBConnection();

            try
            {
                string sqlStatement = @"SELECT top 1 receiptNumber
                                        FROM [payment_history]
                                        where applicantId = @applicantId AND paymentType = 'REGIS_FEE'
                                        Order by receiptNumber desc;";
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                cmd.Parameters.AddWithValue("@applicantId", applicantId);

                SqlDataReader dr = cmd.ExecuteReader();
                string value = null;
                try
                {
                    while (dr.Read())
                    {
                        value = dr["receiptNumber"].ToString();
                    }

                    return value;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberREG()", ex.Message, -1);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberREG()", ex.Message, -1);

                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        // retrieve void receipt number by applicant Id this is used once course receipt is voided.
        //this will be used in applicant details. aspx
        public string getReptNumberCSE(string applicantId)
        {
            SqlConnection conn = dbConnection.getDBConnection();

            try
            {
                string sqlStatement = @"SELECT top 1 receiptNumber
                                        FROM [payment_history]
                                        where applicantId = @applicantId AND paymentType = 'COURSE_FEE'
                                        Order by receiptNumber desc;";
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                cmd.Parameters.AddWithValue("@applicantId", applicantId);

                SqlDataReader dr = cmd.ExecuteReader();
                string value = null;
                try
                {
                    while (dr.Read())
                    {
                        value = dr["receiptNumber"].ToString();
                    }

                    return value;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberCSE()", ex.Message, -1);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberCSE()", ex.Message, -1);

                return null;
            }
            finally
            {
                conn.Close();
            }
        }

        // retrieve void receipt number by applicant Id this is used once course receipt is voided.
        //this will be used in applicant details. aspx (for full payment)
        public string getReptNumberFULLCSEPayment(string applicantId)
        {
            SqlConnection conn = dbConnection.getDBConnection();

            try
            {
                string sqlStatement = @"SELECT top 1 receiptNumber
                                        FROM [payment_history]
                                        where applicantId = @applicantId AND paymentType = 'N/A'
                                        Order by receiptNumber desc;";
                conn.Open();
                SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                cmd.Parameters.AddWithValue("@applicantId", applicantId);

                SqlDataReader dr = cmd.ExecuteReader();
                string value = null;
                try
                {
                    while (dr.Read())
                    {
                        value = dr["receiptNumber"].ToString();
                    }

                    return value;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberFULLCSEPayment()", ex.Message, -1);

                    return null;
                }
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Finance.cs", "getReptNumberFULLCSEPayment()", ex.Message, -1);

                return null;
            }
            finally
            {
                conn.Close();
            }
        }

    }
}
