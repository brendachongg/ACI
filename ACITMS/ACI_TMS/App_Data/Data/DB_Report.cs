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
    public class DB_Report
    {
        private Database_Connection dbConnection = new Database_Connection();

        public DataTable TraineeReport(DateTime dtStart, DateTime dtEnd, bool showParticulars, bool showModResults, string batchCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                string select = @"select t.traineeId, t.fullName, t.idNumber, pb.batchCode, pb.projectCode, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, 
                                pb.programmeCompletionDate, convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, p.programmeTitle, "
                    , from = @"from trainee t inner join trainee_programme tp on tp.traineeId=t.traineeId and tp.traineeStatus<>'" + TraineeStatus.W.ToString() + @"' inner join programme_batch pb on tp.programmeBatchId=pb.programmeBatchId 
                            inner join programme_structure p on p.programmeId=pb.programmeId ";

                if (showParticulars)
                {
                    select += @"t.idType, c1.codeValueDisplay as idTypeDisp, t.nationality, c2.codeValueDisplay as nationalityDisp, t.birthDate, convert(nvarchar, t.birthDate, 106) as birthDateDisp,
                                    t.race, c3.codeValueDisplay as raceDisp, t.gender, t.contactNumber1, t.contactNumber2, t.emailAddress, t.addressLine, t.postalCode,
                                    t.highestEducation, c4.codeValueDisplay as highestEducationDisp, t.highestEduRemarks, t.spokenLanguage, t.writtenLanguage, t.traineeRemarks ";
                    from += @"inner join code_reference c1 on c1.codeValue=t.idType and c1.codeType='IDTYPE'
                                    inner join code_reference c2 on c2.codeValue=t.nationality and c2.codeType='NATION'
                                    inner join code_reference c3 on c3.codeValue=t.race and c3.codeType='RACE'
                                    inner join code_reference c4 on c4.codeValue=t.highestEducation and c4.codeType='EDU' ";
                }

                if (showModResults)
                {
                    select += (showParticulars ? "," : "") + @"tm.moduleId, tm.moduleResult, c5.codeValueDisplay as moduleResultDisp, m.moduleCode, m.moduleTitle,
                                tm.firstAssessmentDate, case when tm.firstAssessmentDate is null then null else convert(nvarchar, tm.firstAssessmentDate, 106) end as firstAssessmentDateDisp,
                                tm.finalAssessmentDate, case when tm.finalAssessmentDate is null then null else convert(nvarchar, tm.finalAssessmentDate, 106) end as finalAssessmentDateDisp,
                                tm.firstAssessorId, tm.finalAssessorId, u1.userName as firstAssessorName, u2.userName as finalAssessorName, tm.SOAStatus, c6.codeValueDisplay as SOAStatusDsip,
                                tm.processSOADate, case when tm.processSOADate is null then null else convert(nvarchar, tm.processSOADate, 106) end as processSOADateDisp, 
                                tm.receivedSOADate, case when tm.receivedSOADate is null then null else convert(nvarchar, tm.receivedSOADate, 106) end as receivedSOADateDisp,
                                tm.traineeModuleRemarks ";
                    from += @"inner join trainee_module tm on tm.traineeId=t.traineeId and tm.defunct='N'
                                inner join module_structure m on tm.moduleId=m.moduleId
                                inner join code_reference c5 on c5.codeValue=tm.moduleResult and c5.codeType='RESULT'
                                inner join code_reference c6 on c6.codeValue=tm.SOAStatus and c6.codeType='SOA'
                                left outer join aci_user u1 on u1.userId=tm.firstAssessorId
                                left outer join aci_user u2 on u2.userId=tm.finalAssessorId ";
                }

                cmd.CommandText = select + from + (dtStart == DateTime.MinValue && dtEnd == DateTime.MaxValue ? "" : "where ")
                    + (dtStart == DateTime.MinValue ? "" : " pb.programmeStartDate >= @dtStart ")
                    + (dtEnd == DateTime.MaxValue ? "" : (dtStart == DateTime.MinValue ? "" : "and ") + "pb.programmeStartDate <= @dtEnd ") + (batchCode == null || batchCode == "" ? "" : "AND UPPER(pb.batchCode) like @batchCode ")
                    + " order by t.fullName, pb.batchCode";
                //(new Log_Handler()).WriteLog("SQL", "DB_Report.cs", "TraineeReport()", cmd.CommandText, -1);

                if (dtStart != DateTime.MinValue) cmd.Parameters.AddWithValue("@dtStart", dtStart);
                if (dtEnd != DateTime.MaxValue) cmd.Parameters.AddWithValue("@dtEnd", dtEnd);
                if (batchCode != null && batchCode != "") cmd.Parameters.AddWithValue("@batchCode", "%" + batchCode.ToUpper() + "%");

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "TraineeReport()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSettlementMode()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select codeValue, codeValueDisplay from code_reference where codeType='PMODE' and defunct='N' order by codeOrder";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "getSettlementMode()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getSettlement(DateTime dtSettlementDate, string settlementMode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //// cmd.CommandText = @"WITH CTE AS ( SELECT idnumber, programmeBatchId, SUM(paymentAmount)  as payment FROM payment_history where 
                //                     paymentStatus = 'PAID' and voidBy IS NULL and paymentDate = @PaymentDate GROUP BY programmeBatchId, idnumber)
                //                     select tp.programmeBatchId, cte.payment, t.fullName, t.idnumber, ps.programmeTitle, REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeStartDate AS DATETIME), 106), ' ', '-') as StartDate , REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeCompletionDate AS DATETIME), 106), ' ', '-') as EndDate, 
                //                     pb.projectcode, ps.programmeCode, tp.programmePayableAmount, tp.registrationFee, tp.subsidyAmt, ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) as regGST, 
                //                     tp.GSTPayableAmount, isnull(tp.registrationFee, 0) +  ROUND(((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0))),2) as totalReg, tp.programmePayableAmount + tp.GSTPayableAmount as totalProgrammeAmt, 
                //                     ISNULL(tp.subsidyAmt, 0), ROUND((tp.programmePayableAmount + tp.GSTPayableAmount +  ROUND((" + General_Constance.GST_RATE + @" * tp.registrationFee), 2) + tp.registrationFee - ISNULL(tp.subsidyAmt, 0)),2) 
                //                     as afterSubsidyFees from  trainee_programme tp left join programme_batch pb on tp.programmeBatchId = pb.programmeBatchId left join trainee t on t.traineeId = 
                //                     tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId right join cte on cte.idnumber = t.idNumber ORDER BY  tp.programmeBatchId";

                //                string sql = @"WITH CTE AS ( SELECT idnumber, programmeBatchId, SUM(paymentAmount)  as payment FROM payment_history where 
                //                                    paymentStatus = @paymentStatus and voidBy IS NULL and paymentDate = @PaymentDate ";

                //                if (!settlementMode.Equals(""))
                //                {
                //                    sql += "AND paymentMode = @paymentMode ";

                //                }
                //                sql += "GROUP BY programmeBatchId, idnumber)select tp.programmeBatchId, cte.payment, t.fullName, t.idnumber, ps.programmeTitle, REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeStartDate AS DATETIME), 106), ' ', '-') as StartDate , REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeCompletionDate AS DATETIME), 106), ' ', '-') as EndDate, pb.projectcode, ps.courseCode, ISNULL(tp.programmePayableAmount, 0) as programmePayableAmount, ISNULL(tp.registrationFee, 0) as registrationFee, ISNULL(tp.subsidyAmt, 0) as subsidyAmt, ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) as regGST, 
                //                                    ISNULL(tp.GSTPayableAmount, 0) as GSTPayableAmount, isnull(tp.registrationFee, 0) +  ROUND(((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0))),2) as totalReg, ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) as totalProgrammeAmt,ROUND((ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) +  ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) + ISNULL(tp.registrationFee,0) - ISNULL(tp.subsidyAmt, 0)),2) 
                //                                    as afterSubsidyFees from  trainee_programme tp left join programme_batch pb on tp.programmeBatchId = pb.programmeBatchId left join trainee t on t.traineeId = 
                //                                    tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId right join cte on cte.idnumber = t.idNumber and cte.programmeBatchId = tp.programmeBatchId ORDER BY  tp.programmeBatchId";

                //                string sql = @"WITH CTE AS ( SELECT idnumber, programmeBatchId, SUM(paymentAmount)  as payment, paymentmode FROM payment_history where 
                //                                    paymentStatus = @paymentStatus and voidBy IS NULL and paymentDate = @PaymentDate ";

                //                if (!settlementMode.Equals(""))
                //                {
                //                    sql += "AND paymentMode = @paymentMode ";

                //                }
                //                sql += "GROUP BY programmeBatchId, idnumber, paymentmode)select t.traineeid, cte.paymentmode, tp.programmeBatchId, cte.payment, t.fullName, t.idnumber, ps.programmeTitle, REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeStartDate AS DATETIME), 106), ' ', '-') as StartDate , REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeCompletionDate AS DATETIME), 106), ' ', '-') as EndDate, pb.projectcode, ps.courseCode, ISNULL(tp.programmePayableAmount, 0) as programmePayableAmount, ISNULL(tp.registrationFee, 0) as registrationFee, ISNULL(tp.subsidyAmt, 0) as subsidyAmt, ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) as regGST, 
                //                                    ISNULL(tp.GSTPayableAmount, 0) as GSTPayableAmount, isnull(tp.registrationFee, 0) +  ROUND(((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0))),2) as totalReg, ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) as totalProgrammeAmt,ROUND((ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) +  ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) + ISNULL(tp.registrationFee,0) - ISNULL(tp.subsidyAmt, 0)),2) 
                //                                    as afterSubsidyFees from  trainee_programme tp left join programme_batch pb on tp.programmeBatchId = pb.programmeBatchId left join trainee t on t.traineeId = 
                //                                    tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId right join cte on cte.idnumber = t.idNumber and cte.programmeBatchId = tp.programmeBatchId ORDER BY  tp.programmeBatchId";


                string sql = @"WITH CTE AS (SELECT paymentMode, ISNULL(traineeid, applicantid) as id, programmeBatchId, Sum(paymentAmount)  as payment FROM payment_history where 
                                    paymentStatus = @paymentStatus and voidBy IS NULL and paymentDate = @PaymentDate ";

                if (!settlementMode.Equals(""))
                    sql += "AND paymentMode = @paymentMode ";

                sql += @"group by traineeid, applicantid, programmeBatchId, paymentMode) select cte.id, Isnull(tp.programmebatchid, a.programmebatchid) as programmebatchid, (select sum(cte.payment) from cte where (cte.id = a.applicantid OR cte.id = t.traineeid) 
                        and (cte.programmeBatchId = tp.programmeBatchId OR cte.programmeBatchId = a.programmeBatchId)) as payment, cte.paymentMode, isnull(t.fullName, a.fullname) as fullname, ISNULL(t.idnumber, a.idnumber) as idnumber, ps.programmeTitle,
                        REPLACE(CONVERT(NVARCHAR,CAST(isnull(pb.programmeStartDate, pb1.programmestartdate) AS DATETIME), 106), ' ', '-') as StartDate , 
                        REPLACE(CONVERT(NVARCHAR,CAST(isnull(pb.programmeCompletionDate, pb1.programmeCompletionDate) AS DATETIME), 106), ' ', '-') as EndDate, isnull(pb.projectcode, pb1.projectcode) as projectcode, ps.courseCode, 
                        ISNULL(ISNULL(tp.programmePayableAmount, a.programmePayableAmount), 0) as programmePayableAmount, ISNULL(ISNULL(tp.registrationFee, a.registrationFee), 0) as registrationFee, ISNULL(ISNULL(tp.subsidyAmt, a.subsidyAmt),0) as subsidyAmt, 
                        ROUND((0.07 * ISNULL(ISNULL(tp.registrationFee, a.registrationFee), 0)), 2) as regGST, isnull(ISNULL(tp.GSTPayableAmount, a.GSTPayableAmount),0) as GSTPayableAmount, 
                        isnull(isnull(tp.registrationFee, a.registrationfee),0) +  ROUND(((0.07 * isnull(ISNULL(tp.registrationFee, a.registrationfee), 0))),2) as totalReg, isnull(ISNULL(tp.programmePayableAmount, a.programmePayableAmount),0) + 
                        isnull(ISNULL(tp.GSTPayableAmount, a.GSTPayableAmount),0) as totalProgrammeAmt, ROUND((isnull(ISNULL(tp.programmePayableAmount, a.programmePayableAmount), 0) + 
                        isnull(ISNULL(tp.GSTPayableAmount, a.gstpayableamount),0) +  ROUND((0.07 * isnull(ISNULL(tp.registrationFee, a.registrationFee), 0)), 2) + isnull(ISNULL(tp.registrationFee,a.registrationFee),0) - isnull(ISNULL(tp.subsidyAmt, a.subsidyAmt),0)),2)  as afterSubsidyFees, 
                        ROUND((isnull(ISNULL(tp.programmePayableAmount, a.programmePayableAmount), 0) - isnull(ISNULL(tp.subsidyAmt, a.subsidyAmt),0)),2)  as afterFees, 
                        ROUND((isnull(ISNULL(tp.programmePayableAmount, a.programmePayableAmount), 0) - isnull(ISNULL(tp.subsidyAmt, a.subsidyAmt),0) +  ROUND((isnull(ISNULL(tp.GSTPayableAmount, a.GSTPayableAmount), 0)), 2) + isnull(ISNULL(tp.registrationFee,a.registrationFee),0)),2) as afterFeesS 
                        from  trainee_programme tp left join programme_batch pb on (tp.programmeBatchId = pb.programmeBatchId) left join trainee t on t.traineeId = 
                        tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId right join cte on cte.id = t.traineeid and cte.programmeBatchId = tp.programmeBatchId 
                        left outer join applicant a on a.applicantid = cte.id left join programme_batch pb1 on a.programmeBatchId = pb1.programmeBatchId left join programme_structure ps1 on pb1.programmeId = ps1.programmeId where tp.traineeStatus <> '" + GeneralLayer.TraineeStatus.W.ToString() + "' or a.rejectStatus <> '" + General_Constance.STATUS_YES.ToString() + "' order BY  tp.programmeBatchId";


                //                string sql = @"WITH CTE AS ( SELECT traineeId, programmeBatchId, Sum(paymentAmount)  as payment FROM payment_history where 
                //                                    paymentStatus = @paymentStatus and voidBy IS NULL and paymentDate = @PaymentDate ";

                //                if (!settlementMode.Equals(""))
                //                {
                //                    sql += "AND paymentMode = @paymentMode ";

                //                }
                //                sql += "group by traineeid, programmeBatchId)select t.traineeid, tp.programmeBatchId, (select sum(cte.payment) from cte where cte.traineeid = t.traineeid and cte.programmeBatchId = tp.programmeBatchId) as payment, t.fullName, t.idnumber, ps.programmeTitle, REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeStartDate AS DATETIME), 106), ' ', '-') as StartDate , REPLACE(CONVERT(NVARCHAR,CAST(pb.programmeCompletionDate AS DATETIME), 106), ' ', '-') as EndDate, pb.projectcode, ps.courseCode, ISNULL(tp.programmePayableAmount, 0) as programmePayableAmount, ISNULL(tp.registrationFee, 0) as registrationFee, ISNULL(tp.subsidyAmt, 0) as subsidyAmt, ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) as regGST, 
                //                                                    ISNULL(tp.GSTPayableAmount, 0) as GSTPayableAmount, isnull(tp.registrationFee, 0) +  ROUND(((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0))),2) as totalReg, ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) as totalProgrammeAmt,ROUND((ISNULL(tp.programmePayableAmount, 0) + ISNULL(tp.GSTPayableAmount, 0) +  ROUND((" + General_Constance.GST_RATE + @" * ISNULL(tp.registrationFee, 0)), 2) + ISNULL(tp.registrationFee,0) - ISNULL(tp.subsidyAmt, 0)),2)  as afterSubsidyFees from  trainee_programme tp left join programme_batch pb on tp.programmeBatchId = pb.programmeBatchId left join trainee t on t.traineeId = 
                //                                    tp.traineeId left join programme_structure ps on pb.programmeId = ps.programmeId right join cte on cte.traineeid = t.traineeid and cte.programmeBatchId = tp.programmeBatchId  ORDER BY  tp.programmeBatchId";

                cmd.CommandText = sql;

                cmd.Parameters.AddWithValue("@PaymentDate", dtSettlementDate);
                cmd.Parameters.AddWithValue("@paymentStatus", PaymentStatus.PAID.ToString());
                if (!settlementMode.Equals(""))
                    cmd.Parameters.AddWithValue("@paymentMode", settlementMode);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "getSettlement()", ex.Message, -1);

                return null;
            }
        }

        public DataTable CourseFeeReceived(int yr)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select p.courseCode, pb.projectCode, sum(tp.programmePayableAmount-isnull(tp.subsidyAmt,0)) as nettFee
                from programme_batch pb inner join programme_structure p on pb.programmeBatchId=p.programmeId and pb.defunct='N' and pb.programmeStartDate between CONVERT(date, concat(@yr, '-04-01')) and CONVERT(date, concat(@yr + 1, '-03-31')) and pb.defunct='N'
                inner join trainee_programme tp on tp.programmeBatchId=pb.programmeBatchId
                group by p.courseCode, pb.projectCode
                order by p.courseCode, pb.projectCode";

                cmd.Parameters.AddWithValue("@yr", yr);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "CourseFeeReceived()", ex.Message, -1);

                return null;
            }
        }

        public DataTable CourseFeeCollection(string progLvl, DateTime dtStart, DateTime dtEnd)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select p.programmeCategory, p.courseCode, pb.projectCode, pb.batchCode, pb.programmeBatchId, p.programmeTitle, convert(varchar,pb.programmeStartDate,106) as programmeStartDate, convert(varchar,pb.programmeCompletionDate, 106) as programmeCompletionDate
                , t.traineeId, t.fullName, t.idNumber, isnull(tp.registrationFee, 0) as registrationFee, round(isnull(tp.registrationFee, 0)*" + General_Constance.GST_RATE + @", 2) as registrationFeeGST
                , STUFF((SELECT '; ' + ph.referenceNumber AS [text()] FROM (select traineeId, programmeBatchId, referenceNumber from payment_history where paymentStatus='" + PaymentStatus.PAID.ToString() + @"' 
                        and paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"')) ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as referenceNumber
                , STUFF((SELECT '; ' + ph.paymentModeDisp AS [text()] FROM (select distinct traineeId, programmeBatchId, paymentMode, codeValueDisplay as paymentModeDisp 
                    from payment_history ph1 inner join code_reference c on ph1.paymentMode=c.codeValue and c.codeType='PMODE' where paymentStatus='" + PaymentStatus.PAID.ToString() + @"' and paymentType in ('" + PaymentType.BOTH.ToString() + @"', '" + PaymentType.PROG.ToString() + @"')) ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as paymentMode
                , STUFF((SELECT '; ' + convert(varchar, ph.paymentDate, 106) AS [text()] 
	                FROM (select distinct traineeId, programmeBatchId, paymentDate from payment_history where paymentStatus='" + PaymentStatus.PAID.ToString() + @"' and paymentType in ('" + PaymentType.BOTH.ToString() + @"', '" + PaymentType.PROG.ToString() + @"')) ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as paymentDate
                , STUFF((SELECT '; ' + convert(varchar, ph.bankInDate, 106) AS [text()] 
	                FROM (select distinct traineeId, programmeBatchId, bankInDate from payment_history where paymentStatus='" + PaymentStatus.PAID.ToString() + @"' and paymentMode='" + PaymentMode.CHEQ.ToString() + @"' and paymentType in ('" + PaymentType.BOTH.ToString() + @"', '" + PaymentType.PROG.ToString() + @"')) ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as bankInDate
                , tp.programmePayableAmount-isnull(tp.subsidyAmt,0) as programmeNettAmount, tp.GSTPayableAmount-(case when pay.paymentType='" + PaymentType.BOTH.ToString() + @"' then round(isnull(tp.registrationFee, 0)*" + General_Constance.GST_RATE + @", 2) else 0 end) as programmeGSTAmount
                , tp.programmePayableAmount-isnull(tp.subsidyAmt,0)+tp.GSTPayableAmount-(case when pay.paymentType='" + PaymentType.BOTH.ToString() + @"' then round(isnull(tp.registrationFee, 0)*" + General_Constance.GST_RATE + @", 2) else 0 end) as programmeTotalAmount
                , STUFF((SELECT ';' + ph.paymentRemarks AS [text()] 
	                FROM (select distinct traineeId, programmeBatchId, paymentRemarks from payment_history where paymentStatus='" + PaymentStatus.PAID.ToString() + @"' and paymentType in ('" + PaymentType.BOTH.ToString() + @"', '" + PaymentType.PROG.ToString() + @"')) ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as paymentRemarks
                from programme_batch pb inner join programme_structure p on pb.programmeId=p.programmeId and pb.defunct='N' and p.programmeLevel=@pgLvl 
                and (@dtStart is null or (@dtStart is not null and pb.programmeStartDate>=@dtStart)) and (@dtEnd is null or (@dtEnd is not null and pb.programmeStartDate <= @dtEnd))
                inner join trainee_programme tp on tp.programmeBatchId=pb.programmeBatchId
                inner join trainee t on t.traineeId=tp.traineeId
                --each applicant should only have either programme payment or combined payment
                inner join (select distinct traineeId, paymentType from payment_history where traineeId is not null and paymentStatus='" + PaymentStatus.PAID.ToString() + @"' and paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"')) pay on pay.traineeId=tp.traineeId
                order by p.programmeCategory, p.courseCode, pb.projectCode, pb.batchCode, p.programmeTitle, pb.programmeStartDate, pb.programmeCompletionDate
                --sort by programmeTotalAmount
                ,19 , t.fullName, t.idNumber";

                cmd.Parameters.AddWithValue("@pgLvl", progLvl);
                cmd.Parameters.AddWithValue("@dtStart", dtStart == DateTime.MinValue ? (object)DBNull.Value : dtStart);
                cmd.Parameters.AddWithValue("@dtEnd", dtEnd == DateTime.MaxValue ? (object)DBNull.Value : dtEnd);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "CourseFeeCollection()", ex.Message, -1);

                return null;
            }
        }

        public DataTable QPOSummary(DateTime dtStart, DateTime dtEnd)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select 'No. of Runs' as [Definition], cr1.codeValueDisplay as [Programme Type], count(*) as [Count]
                from programme_batch pb inner join programme_structure p on pb.programmeId=p.programmeId and pb.defunct='N' and pb.programmeStartDate between @dtStart and @dtEnd
                inner join code_reference cr1 on cr1.codeValue=p.programmeType and cr1.codeType='PGTYPE'
                group by p.programmeType, cr1.codeValueDisplay
                union
                select 'Training Places', cr1.codeValueDisplay, count(*) as cnt
                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' and tm.moduleResult='" + ModuleResult.C.ToString() + @"'
                inner join programme_structure p on p.programmeId=tp.programmeId
                inner join code_reference cr1 on cr1.codeValue=p.programmeType and cr1.codeType='PGTYPE'
                group by p.programmeType, cr1.codeValueDisplay";

                cmd.Parameters.AddWithValue("@dtStart", dtStart);
                cmd.Parameters.AddWithValue("@dtEnd", dtEnd);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "QPOSummary()", ex.Message, -1);

                return null;
            }
        }

        public DataTable QPODetails(DateTime dt)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select year(@ay) as AY, p.programmeType, cr1.codeValueDisplay as programmeTypeDisp, p.programmeTitle, convert(varchar,pb.programmeStartDate, 112) as programmeStartDate, convert(varchar, pb.programmeCompletionDate, 112) as programmeCompletionDate
                , p.courseCode, pb.projectCode, count(*) as hc, trainplc.pax as trainingPlc, pb.programmeBatchId
                from programme_batch pb inner join programme_structure p on pb.programmeId=p.programmeId and pb.programmeStartDate between @ay and DATEADD(day, -1, DATEADD(year, 1, @ay)) and pb.defunct='N'
                inner join code_reference cr1 on cr1.codeValue=p.programmeType and cr1.codeType='PGTYPE'
                inner join trainee_programme tp on tp.programmeBatchId=pb.programmeBatchId
                inner join (
	                select tp.programmeBatchId, count(*) as pax
	                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.moduleResult='" + ModuleResult.C.ToString() + @"' and tm.defunct='N'
	                group by tp.programmeBatchId
                ) trainplc on trainplc.programmeBatchId=pb.programmeBatchId
                group by p.programmeType, cr1.codeValueDisplay, p.programmeTitle, pb.programmeStartDate, pb.programmeCompletionDate, p.courseCode, pb.projectCode, trainplc.pax, pb.programmeBatchId
                order by cr1.codeValueDisplay, p.programmeTitle, pb.programmeStartDate, pb.programmeCompletionDate, p.courseCode, pb.projectCode";

                cmd.Parameters.AddWithValue("@ay", dt);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "ALLSSummary()", ex.Message, -1);

                return null;
            }
        }

        public DataTable ALLSSummary(int yr)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select cr.codeValueDisplay as [Programme Type], count(*) as [No. of Runs]
                from programme_batch pb inner join programme_structure p on pb.programmeId=p.programmeId and pb.defunct='N'
                and programmeCompletionDate between CONVERT(date, concat(@yr, '-04-01')) and CONVERT(date, concat(@yr + 1, '-03-31'))
                inner join code_reference cr on cr.codeValue=p.programmeType and cr.codeType='PGTYPE'
                group by p.programmeType, cr.codeValueDisplay
                order by cr.codeValueDisplay";

                cmd.Parameters.AddWithValue("@yr", yr);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "ALLSSummary()", ex.Message, -1);

                return null;
            }
        }

        public DataTable ALLSDetails(int yr)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                --find the programmes that have classes with selected FY
                ;with PROG_CTE as (
	                select programmeId, classMode, count(*) as inclCrossRun
	                , sum(case when programmeCompletionDate between CONVERT(date, concat(@yr, '-04-01')) and CONVERT(date, concat(@yr + 1, '-03-31')) then 1 else 0 end) as newRun
	                from programme_batch where programmeStartDate between CONVERT(date, concat(@yr, '-04-01')) and CONVERT(date, concat(@yr + 1, '-03-31')) and defunct='N'
	                group by programmeId, classMode
                )
                --count no of enrolled trainees for each programme
                , CLASS_HC_CTE as (
	                select programmeId, count(*) as pax
	                from trainee_programme where traineeStatus='" + TraineeStatus.E.ToString() + @"'
	                group by programmeId
                )
                --count no of trainees for each module of the programme
                , MOD_HC_CTE as (
	                select tp.programmeId, tm.moduleId, sum(case when tm.moduleResult='" + ModuleResult.EXEM.ToString() + @"' then 1 else 0 end) as exemHC, count(*) as totalHC
	                from trainee_module tm inner join trainee_programme tp on tp.traineeId=tm.traineeId and tm.defunct='N' and tm.sitInModule='N' and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"'
	                group by tp.programmeId, tm.moduleId
                )
                select p.programmeId, m.moduleId
                , p.programmeType, cr1.codeValueDisplay as programmeTypeDisp, p.courseCode, p.programmeTitle, m.moduleTitle, cr2.codeValueDisplay as classModeDisp
                , m.moduleTrainingHour, cls.pax as uniqueHC, md.totalHC-md.exemHC as trainPlcMod, md.totalHC as trainPlcPDC, (md.totalHC-md.exemHC)*m.moduleTrainingHour as totalTrainingHour
                , pb.newRun, pb.inclCrossRun
                from PROG_CTE pb inner join programme_structure p on p.programmeId=pb.programmeId 
                inner join code_reference cr1 on cr1.codeValue=p.programmeType and cr1.codeType='PGTYPE'
                inner join bundle b on b.bundleId=p.bundleId
                inner join bundle_module bm on bm.bundleId=b.bundleId and bm.defunct='N'
                inner join module_structure m on m.moduleId=bm.moduleId
                inner join code_reference cr2 on cr2.codeValue=pb.classMode and cr2.codeType='CLMODE'
                inner join CLASS_HC_CTE cls on cls.programmeId=pb.programmeId
                inner join MOD_HC_CTE md on md.programmeId=pb.programmeId and md.moduleId=bm.moduleId
                order by cr1.codeValueDisplay, p.programmeId, m.moduleTitle";

                cmd.Parameters.AddWithValue("@yr", yr);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "ALLSDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable SFCDisbursement(int yr, string subsidyName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select t.fullName, t.idNumber, t.idType, cr1.codeValueDisplay as idTypeDisp, p.programmeTitle, p.courseCode, pb.projectCode, pb.batchCode
                , convert(varchar,pb.programmeStartDate,106) as programmeStartDate, convert(varchar,pb.programmeCompletionDate,106) as programmeCompletionDate
                , STUFF((SELECT ';' + ph.referenceNumber AS [text()] FROM (select traineeId, programmeBatchId, referenceNumber from payment_history where paymentMode='" + PaymentMode.SFC.ToString() + @"' 
                    and paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"') and paymentStatus<>'" + PaymentStatus.VOID.ToString() + @"') ph 
	                WHERE ph.traineeId=t.traineeId and ph.programmeBatchId=pb.programmeBatchId FOR XML PATH('')), 1, 1, '' )  as referenceNumber
                , tp.programmePayableAmount-tp.subsidyAmt as programmetNettAmount, tp.subsidyAmt, tp.GSTPayableAmount-(case when ph.paymentType='" + PaymentType.PROG.ToString() + @"' then 0 else tp.registrationFee*" + General_Constance.GST_RATE + @" end) as gst
                , ph.paymentAmount as SFCAmount, tp.programmePayableAmount-tp.subsidyAmt-ph.paymentAmount as othAmount
                from trainee_programme tp inner join trainee t on tp.traineeId=t.traineeId
                inner join subsidy sub on sub.subsidyScheme=@subsidyName and sub.subsidyId=tp.subsidyId
                inner join code_reference cr1 on cr1.codeValue=t.idType and cr1.codeType='IDTYPE'
                inner join programme_structure p on p.programmeId=tp.programmeId
                inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId and pb.programmeStartDate between CONVERT(date, concat(@yr, '-04-01')) and CONVERT(date, concat(@yr + 1, '-03-31')) and pb.defunct='N'
                inner join (
	                select traineeId, paymentType, sum(paymentAmount) as paymentAmount from payment_history 
                    where paymentMode='" + PaymentMode.SFC.ToString() + @"' and paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"') and paymentStatus<>'" + PaymentStatus.VOID.ToString() + @"'
                    group by traineeId, paymentType
                ) ph on ph.traineeId=t.traineeId";

                cmd.Parameters.AddWithValue("@yr", yr);
                cmd.Parameters.AddWithValue("@subsidyName", subsidyName);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "SSGFundDisbursement()", ex.Message, -1);

                return null;
            }
        }

        public DataTable FullQualQuarter(DateTime dt)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @";with PROG_MOD_CTE as (
	                select p.programmeId, count(*) as noOfMod
	                from programme_structure p inner join bundle b on p.bundleId=b.bundleId
	                inner join bundle_module bm on bm.bundleId=b.bundleId and bm.defunct='N'
	                group by p.programmeId
                )
                , DATA_CTE as (
	                select p.programmeCategory, p.programmeId, pb.programmeBatchId, m.moduleId, tp.subsidyId, sub.subsidyValue, sub.subsidyScheme
	                ,p.programmeLevel, p.programmeTitle, m.moduleTitle, m.moduleCost, pb.batchCode, p.courseCode, pb.projectCode, pb.programmeStartDate, pb.programmeCompletionDate
	                , tp.programmePayableAmount-isnull(tp.subsidyAmt, 0) as programmeNettAmount, count(*) as pax, pm.noOfMod
	                from programme_batch pb inner join batch_module bm on pb.programmeBatchId=bm.programmeBatchId and bm.endDate > @dt and bm.defunct='N' and pb.defunct='N'
	                inner join module_structure m on m.moduleId=bm.moduleId
	                inner join programme_structure p on p.programmeId=pb.programmeId and p.programmeType='" + ProgrammeType.FQ.ToString() + @"'
	                inner join trainee_module tm on tm.programmeBatchId=pb.programmeBatchId and tm.moduleId=m.moduleId and tm.defunct='N' and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + @"')
	                inner join trainee_programme tp on tp.traineeId=tm.traineeId and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"'
	                inner join PROG_MOD_CTE pm on pm.programmeId=p.programmeId
	                left outer join subsidy sub on sub.subsidyId=tp.subsidyId
	                group by p.programmeCategory, p.programmeLevel, p.programmeTitle, m.moduleTitle, m.moduleCost, pb.batchCode, p.courseCode, pb.projectCode, pb.programmeStartDate
	                , pb.programmeCompletionDate, p.programmeId, pb.programmeBatchId, m.moduleId, tp.programmePayableAmount, tp.subsidyAmt, tp.subsidyId, pm.noOfMod, sub.subsidyValue, sub.subsidyScheme
                )
                --find out how many modules end in which quarter for each class
                , MOD_END_CTE as (
	                select programmeBatchId, [0], [1], [2], [3], [4]
	                from (
		                select programmeBatchId, quat, count(*) as cnt
		                from (
			                select  programmeBatchId , moduleId,
			                case when endDate < convert(date, concat(year(@dt), '-03-31')) then 0
				                 when endDate between convert(date, concat(year(@dt), '-04-01')) and convert(date, concat(year(@dt), '-06-30')) then 1
				                 when endDate between convert(date, concat(year(@dt), '-07-01')) and convert(date, concat(year(@dt), '-09-30')) then 2
				                 when endDate between convert(date, concat(year(@dt), '-10-01')) and convert(date, concat(year(@dt), '-12-31')) then 3
				                 when endDate between convert(date, concat(year(@dt) + 1, '-01-01')) and convert(date, concat(year(@dt) + 1, '-03-31')) then 4
			                end as quat
			                from batch_module where defunct='N' and programmeBatchId in (select programmeBatchId from batch_module where endDate > @dt and defunct='N') 
		                ) tbl group by  programmeBatchId, quat
	                ) as src
	                pivot(
		                sum(cnt)
		                for quat in ([0], [1], [2], [3], [4]) 
	                ) as pvt
                )
                select d1.programmeCategory, d1.programmeLevel, d1.programmeId, d1.programmeBatchId, d1.moduleId, d1.subsidyId, d1.subsidyScheme, d1.programmeTitle, d1.moduleTitle
                , (1-isnull(d1.subsidyValue, 0))*d1.moduleCost as nettModCost, d2.defModFee, d1.programmeNettAmount, d1.batchCode, d1.courseCode, d1.projectCode
                , convert(varchar,d1.programmeStartDate,106) as programmeStartDate, convert(varchar,d1.programmeCompletionDate,106) as programmeCompletionDate
                , d1.pax, d1.noOfMod, d1.pax*d1.programmeNettAmount as totalRevFee, d1.pax*d2.defModFee as totalDefFee, d2.noDefMod, d1.noOfMod-d2.noDefMod as noCompleteMod
                , m.[0] as beforeFY, m.[1] as fyQ1, m.[2] as fyQ2, m.[3] as fyQ3, m.[4] as fyQ4
                from DATA_CTE d1 inner join (
	                select programmeId, programmeBatchId, programmeNettAmount, subsidyId, count(*) as noDefMod, sum((1-isnull(subsidyValue, 0))*moduleCost) as defModFee
	                from DATA_CTE 
	                group by programmeId, programmeBatchId, programmeNettAmount, subsidyId
                ) d2 on d1.programmeBatchId=d2.programmeBatchId and d1.programmeNettAmount=d2.programmeNettAmount and (d1.subsidyId=d2.subsidyId or (d1.subsidyId is null and d2.subsidyId is null))
                inner join MOD_END_CTE m on m.programmeBatchId=d1.programmeBatchId
                order by d1.programmeCategory,d1.programmeLevel,d1.programmeId, d1.batchCode, d1.programmeNettAmount,d1.subsidyId, d1.moduleTitle";

                cmd.Parameters.AddWithValue("@dt", dt);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "FullQualQuarter()", ex.Message, -1);

                return null;
            }
        }

        public DataTable WTSDisbursement(int yr, string subsidyName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select t.fullName, t.idNumber, t.idType, cr1.codeValueDisplay as idTypeDisp, p.programmeTitle, p.courseCode, pb.projectCode, pb.batchCode
                , convert(varchar,pb.programmeStartDate,106) as programmeStartDate, convert(varchar,pb.programmeCompletionDate,106) as programmeCompletionDate
                from trainee_programme tp inner join trainee t on tp.traineeId=t.traineeId and t.idType='" + (int)IDType.NRIC + @"'
                inner join subsidy sub on sub.subsidyScheme=@subsidyName and sub.subsidyId=tp.subsidyId
                inner join code_reference cr1 on cr1.codeValue=t.idType and cr1.codeType='IDTYPE'
                inner join programme_structure p on p.programmeId=tp.programmeId
                inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId and pb.programmeStartDate between CONVERT(date, concat(@yr, '-04-01')) 
                and CONVERT(date, concat(@yr + 1, '-03-31')) and pb.defunct='N'";

                cmd.Parameters.AddWithValue("@yr", yr);
                cmd.Parameters.AddWithValue("@subsidyName", subsidyName);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "SSGFundDisbursement()", ex.Message, -1);

                return null;
            }
        }

        public DataTable cseFeeGrantDrawDown(int mth, int yr, string subsidyName)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"select p.programmeId, p.programmeType, cr1.codeValueDisplay as programmeTypeDisp, p.programmeLevel, cr2.codeValueDisplay as programmeLevelDisp, p.programmeTitle 
                , convert(varchar,ISNULL(tm.finalAssessmentDate, tm.firstAssessmentDate),106) as assessmentDate, p.courseCode, pb.projectCode, m.moduleTitle 
                --total headcount (hc) is counted as SC/PR WITH subsidy, if a trainee is SC/PR but does not take subsidy (ie due to internal staff or attended course before etc) then is counted under the non subsidy hc
                , sum(case when t.idType in ('" + (int)IDType.NRIC + @"', '" + (int)IDType.FIN + @"') and  tp.subsidyId=sub.subsidyId then 1 else 0 end) as hc, sum(case when tp.subsidyId is null then 1 else 0 end) as nonSubsidyHC
                , case when sub.subsidyValue is not null then m.moduleCost*sub.subsidyValue else 0 end as feeGrant
                , case when sub.subsidyValue is not null then m.moduleCost*sub.subsidyValue else 0 end * sum(case when t.idType in ('" + (int)IDType.NRIC + @"', '" + (int)IDType.FIN + @"') and tp.subsidyId=sub.subsidyId then 1 else 0 end) as totalFeeGrant 
                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' 
                and tm.moduleResult='" + ModuleResult.C.ToString() + @"' and  MONTH(ISNULL(tm.finalAssessmentDate, tm.firstAssessmentDate))=@mth and YEAR(ISNULL(tm.finalAssessmentDate, tm.firstAssessmentDate))=@yr
                inner join trainee t on t.traineeId=tm.traineeId
                inner join programme_batch pb on pb.programmeBatchId=tp.programmeBatchId and pb.defunct='N'
                inner join programme_structure p on p.programmeId=pb.programmeId
                inner join module_structure m on m.moduleId=tm.moduleId
                inner join code_reference cr1 on cr1.codeValue=p.programmeType and cr1.codeType='PGTYPE'
                inner join code_reference cr2 on cr2.codeValue=p.programmeLevel and cr2.codeType='PGLVL'
                left outer join subsidy sub on sub.subsidyScheme=@subsidyName and (sub.programmeId=tp.programmeId or sub.programmeId is null) and sub.subsidyType='" + SubsidyType.RATE.ToString() + @"' 
                group by p.programmeId, p.programmeType, cr1.codeValueDisplay, p.programmeLevel, cr2.codeValueDisplay, p.programmeTitle,
                tm.finalAssessmentDate, tm.firstAssessmentDate, p.courseCode, pb.projectCode, m.moduleTitle, sub.subsidyValue, m.moduleCost
                order by cr1.codeValueDisplay, cr2.codeValueDisplay, p.programmeTitle, tm.finalAssessmentDate, tm.firstAssessmentDate, m.moduleTitle";

                cmd.Parameters.AddWithValue("@mth", mth);
                cmd.Parameters.AddWithValue("@yr", yr);
                cmd.Parameters.AddWithValue("@subsidyName", subsidyName);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "cseFeeGrantDrawDown()", ex.Message, -1);

                return null;
            }
        }

        public DataTable SSGKPIAchievement(int fyStart, int fyEnd)
        {
            try
            {
                string selEnrolledSQL = "", selSOASQL = "", tblEnrolledSQL = "", tblSOASQL = "", tblEnrolledNWSQSQL = "", tblSOANWSQSQL = "";
                for (int i = 0; i < fyEnd - fyStart + 1; i++)
                {
                    selEnrolledSQL += ", sum(isnull(e" + i + ".pax, 0)) as [FY " + (fyStart + i).ToString().Substring(2) + " HC*]";
                    selSOASQL += ", sum(isnull(s" + i + ".soa, 0)) as [FY " + (fyStart + i).ToString().Substring(2) + " SOA]";


                    tblEnrolledSQL += "left outer join (select * from COMPLETED_CTE where [level]=" + (i + 1) + ") e" + i + " on e" + i + ".programmeId=p.programmeId ";
                    tblSOASQL += "left outer join (select * from SOA_CTE where [level]=" + (i + 1) + ") s" + i + " on s" + i + ".programmeId=p.programmeId ";

                    tblEnrolledNWSQSQL += "left outer join (select * from ENROLLED_CTE where [level]=" + (i + 1) + ") e" + i + " on e" + i + ".programmeId=p.programmeId ";
                    tblSOANWSQSQL += "left outer join (select * from TRAINPLC_CTE_SC where [level]=" + (i + 1) + ") s" + i + " on s" + i + ".programmeId=p.programmeId ";
                }

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                -- find all the fy ranges
                ;with FY_CTE as ( 
	                SELECT convert(date, CONCAT( @fys,'-04-01')) AS [StartDate], convert(date, CONCAT( @fys+1,'-03-31')) AS [EndDate], 1 AS [level]
	                UNION ALL
	                SELECT DATEADD(YEAR, 1, [StartDate]), DATEADD(YEAR, 1, [EndDate]), [level] + 1
	                FROM FY_CTE WHERE [StartDate] < convert(date, CONCAT( @fye,'-04-01')) 
                )
                -- find all the batches within FY range
                , CLASS_CTE as (
	                select pb.programmeBatchId, pb.programmeId, fy.[level]
	                from programme_batch pb inner join FY_CTE fy on pb.programmeStartDate between fy.StartDate and fy.EndDate and pb.defunct='N'
                )
                --find all trainees that has compeleted all sessions of the module for short course non esq
                , FULL_ATTENDANCE_CTE_SC as (
	                select tp.traineeId, p.programmeId, tm.programmeBatchId, tm.batchModuleId, tm.moduleId
	                from trainee_programme tp inner join trainee_module tm on tp.traineeId=tm.traineeId and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"'
	                inner join programme_batch pb on tm.programmeBatchId=pb.programmeBatchId and tm.defunct='N'
	                inner join programme_structure p on p.programmeId=pb.programmeId and p.programmeType in ('" + ProgrammeType.SCNWSQ.ToString() + @"')
	                where not exists (select 1 from trainee_absence_record a where a.traineeId=tp.traineeId and tm.batchModuleId=a.batchModuleId and a.defunct='N')
                )
                --count the trainees completed where trainee pass all modules of programme for full qual and short course wsq
                , COMPLETED_CTE as (
	                select cls.programmeId, p.programmeLevel, p.programmeType, cls.[level], sum(tp.completeHC) as pax
	                from (
		                select tm.programmeBatchId, count(*) as completeHC
		                from (select m.traineeId, m.programmeBatchId, count(*) as passMod from trainee_module m inner join trainee_programme p on m.traineeId=p.traineeId
			                where (m.moduleResult='" + ModuleResult.C.ToString() + @"' or m.moduleResult='" + ModuleResult.EXEM.ToString() + @"') and m.defunct='N' and p.traineeStatus='" + TraineeStatus.E.ToString() + @"'
			                group by m.traineeId, m.programmeBatchId) tm 
		                inner join (select programmeBatchId, count(*) as noOfMod from batch_module where defunct='N' group by programmeBatchId) bm on tm.programmeBatchId=bm.programmeBatchId
		                where tm.passMod=bm.noOfMod group by tm.programmeBatchId) tp 
	                inner join CLASS_CTE cls on tp.programmeBatchId=cls.programmeBatchId 
	                inner join programme_structure p on cls.programmeId=p.programmeId and p.programmeType in ('" + ProgrammeType.FQ.ToString() + @"', '" + ProgrammeType.SCWSQ.ToString() + @"')
	                group by cls.programmeId, p.programmeLevel, p.programmeType, cls.[level]
                )
                --count the trainees enrolled for short course non wsq
                , ENROLLED_CTE as (
	                select cls.programmeId, p.programmeLevel, p.programmeType, cls.[level], count(*) as pax
	                from trainee_programme tp inner join CLASS_CTE cls on tp.programmeBatchId=cls.programmeBatchId and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"'
	                inner join programme_structure p on tp.programmeId=p.programmeId and p.programmeType in ('" + ProgrammeType.SCNWSQ.ToString() + @"')
                    --only those trainee who has completed all attendance of all modules is counted
	                inner join (select distinct traineeId from FULL_ATTENDANCE_CTE_SC) a on tp.traineeId=a.traineeId
	                group by cls.programmeId, p.programmeLevel, p.programmeType, cls.[level]
                )
                --count no of modules passed for full qual and short course wsq
                , SOA_CTE as (
	                select tp.programmeId, cls.[level], count(*) as soa
	                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' and tm.moduleResult='C' and tp.traineeStatus='E'
	                inner join CLASS_CTE cls on cls.programmeBatchId=tm.programmeBatchId
                    inner join programme_structure p on cls.programmeId=p.programmeId and p.programmeType in ('" + ProgrammeType.FQ.ToString() + @"', '" + ProgrammeType.SCWSQ.ToString() + @"')
	                group by tp.programmeId, cls.[level]
                )
                -- count no of trainning places for short course non wsq
                , TRAINPLC_CTE_SC as (
	                select tp.programmeId, cls.[level], count(*) as soa
	                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + @"') 
                    and tp.traineeStatus='" + TraineeStatus.E.ToString() + @"' and tm.sitInModule='N'
	                inner join CLASS_CTE cls on cls.programmeBatchId=tm.programmeBatchId
	                inner join programme_structure p on cls.programmeId=p.programmeId and p.programmeType in ('" + ProgrammeType.SCNWSQ.ToString() + @"')
                    inner join FULL_ATTENDANCE_CTE_SC a on a.traineeId=tp.traineeId and a.programmeBatchId=tp.programmeBatchId and tm.batchModuleId=a.batchModuleId
	                group by tp.programmeId, cls.[level]
                )
                --retrieve figures for full qual and short course wsq
                select c1.codeValueDisplay as [Programme Type], c2.codeValueDisplay as [Programme Levels]"
                + selEnrolledSQL + selSOASQL +
                @"from programme_structure p inner join code_reference c1 on c1.codeValue=p.programmeType and c1.codeType='PGTYPE' 
                and p.programmeType in ('" + ProgrammeType.FQ.ToString() + @"', '" + ProgrammeType.SCWSQ.ToString() + @"') 
                inner join code_reference c2 on p.programmeLevel=c2.codeValue and c2.codeType='PGLVL' "
                + tblEnrolledSQL + tblSOASQL +
                @"group by p.programmeType, p.programmeLevel, c1.codeValueDisplay, c2.codeValueDisplay 
                union
                --retrieve figures for short course non wsq
                select c1.codeValueDisplay as [Programme Type], c2.codeValueDisplay as [Programme Levels]"
                + selEnrolledSQL + selSOASQL +
                @"from programme_structure p inner join code_reference c1 on c1.codeValue=p.programmeType and c1.codeType='PGTYPE'
                and p.programmeType in ('" + ProgrammeType.SCNWSQ.ToString() + @"')
                inner join code_reference c2 on p.programmeLevel=c2.codeValue and c2.codeType='PGLVL' "
                + tblEnrolledNWSQSQL + tblSOANWSQSQL +
                @"group by p.programmeType, p.programmeLevel, c1.codeValueDisplay, c2.codeValueDisplay 
                order by 3, 4";

                cmd.Parameters.AddWithValue("@fys", fyStart);
                cmd.Parameters.AddWithValue("@fye", fyEnd);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "SSGKPIAchievement()", ex.Message, -1);

                return null;
            }
        }

        public DataTable ACIMonthlyDetails(DateTime dtStart, DateTime dtEnd)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                --find out the programmes that have classes that start within selected date and have enrolled trainees
                ;with CLASS_CTE as (	
	                select pb.programmeBatchId, pb.programmeId, pb.programmeStartDate, pb.programmeCompletionDate, pb.projectCode, p.courseCode, p.programmeTitle
	                from programme_batch pb inner join programme_structure p on pb.programmeId=p.programmeId and pb.programmeStartDate between @startDt and @endDt and pb.defunct='N'
	                where exists (select 1 from trainee_programme tp where tp.programmeBatchId=pb.programmeBatchId)
                )
                --find out the different types of subsidy and prog fees for each class, as well as how many enrolled trainees uses that subsidy
                , SUBSIDY_CTE as (
	                select tp.programmeBatchId, tp.programmeId, tp.subsidyId, s.subsidyScheme, programmePayableAmount-isnull(subsidyAmt, 0) as programmeNettAmount
	                from trainee_programme tp left outer join subsidy s on s.subsidyId=tp.subsidyId and tp.programmeId in (select programmeId from CLASS_CTE) 
	                group by programmeBatchId, tp.programmeId, tp.subsidyId, s.subsidyScheme, programmePayableAmount, subsidyAmt
                )
                --compute the RFI columns (haven merge yet)
                , RFI_PAY_CTE as (	
	                select count(*) as pax, eh.companyName, ph.referenceNumber, convert(nvarchar, ph.paymentDate, 106) as paymentDate, sub.subsidyId, tp.programmePayableAmount-isnull(tp.subsidyAmt, 0) as programmeNettAmount, tp.programmeBatchId, tp.programmeId
	                --each applicant should only have 1 valid full cse RFI payment
	                from payment_history ph inner join trainee_employment_history eh on ph.paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"') and ph.paymentMode='" + PaymentMode.RFI.ToString() + @"' and ph.paymentStatus='" + PaymentStatus.PAID.ToString() + @"'
	                --only retreive those payment that are for the selected classes
	                and ph.programmeBatchId in (select programmeBatchId from CLASS_CTE)
	                --only enrolled trainees
	                and ph.traineeId=eh.traineeId and eh.currentEmployment='Y'
	                --join to find out the subsidy of each payment, as well as the prog fees so know which RFI belong to which subsidy and prog fee
	                inner join trainee_programme tp on tp.traineeId=ph.traineeId and tp.programmeBatchId=ph.programmeBatchId
	                inner join SUBSIDY_CTE sub on sub.programmeBatchId=ph.programmeBatchId and ((sub.subsidyId is null and tp.subsidyId is null) or tp.subsidyId=sub.subsidyId) and sub.programmeNettAmount=tp.programmePayableAmount-isnull(tp.subsidyAmt, 0)
	                group by tp.programmeId, tp.programmeBatchId, eh.companyName, tp.programmePayableAmount, sub.subsidyId, tp.subsidyAmt, ph.referenceNumber, ph.paymentDate 
                )
                --compute the final RFI columns, to merge the multiple reference no and payment dates from the same class and subsidy into 1 string
                , RFI_PAY_MERGE_CTE as (
	                select a.cnt, a.programmeId, a.programmeBatchId, a.subsidyId, a.programmeTotalAmount, a.programmeNettAmount, a.companyName, a.pax
	                --use select distinct to remove duplicated values
	                , STUFF((SELECT ',' + b.referenceNumber AS [text()] FROM (select distinct programmeId, programmeBatchId, subsidyId, programmeNettAmount, companyName, referenceNumber from RFI_PAY_CTE) b WHERE a.programmeId=b.programmeId and a.programmeBatchId=b.programmeBatchId and ((a.subsidyId is null and b.subsidyId is null) or a.subsidyId=b.subsidyId) and a.programmeNettAmount=b.programmeNettAmount and a.companyName = b.companyName 
	                FOR XML PATH('')), 1, 1, '' )  as referenceNumber
	                , STUFF((SELECT ',' + b.paymentDate AS [text()] FROM (select distinct programmeId, programmeBatchId, subsidyId, programmeNettAmount, companyName, paymentDate from RFI_PAY_CTE) b WHERE a.programmeId=b.programmeId and a.programmeBatchId=b.programmeBatchId and ((a.subsidyId is null and b.subsidyId is null) or a.subsidyId=b.subsidyId) and a.programmeNettAmount=b.programmeNettAmount and a.companyName = b.companyName FOR XML PATH('')), 1, 1, '' )  as paymentDate
	                from ( 
		                select count(*) as cnt, sum(pax) as pax, programmeId, programmeBatchId, subsidyId, sum(programmeNettAmount) as programmeTotalAmount, programmeNettAmount, companyName from RFI_PAY_CTE 
		                group by programmeId, programmeBatchId, subsidyId, programmeNettAmount, companyName) a
                )
                --find out the no of trainee that DO NOT pay via RFI
                , OTHER_PAY_CTE as (
	                select count(*) as pax, tp.programmeBatchId, tp.subsidyId, tp.programmePayableAmount-isnull(tp.subsidyAmt,0) as programmeNettAmount
	                from trainee_programme tp inner join (
		                --find out all the trainees who is not paid using RFI
		                select distinct ph.traineeId, ph.programmeBatchId from payment_history ph inner join CLASS_CTE cls on ph.programmeBatchId=cls.programmeBatchId and paymentType in ('" + PaymentType.PROG.ToString() + @"', '" + PaymentType.BOTH.ToString() + @"') and paymentMode <> 'RFI' and paymentStatus='" + PaymentStatus.PAID.ToString() + @"'
	                ) ph on tp.traineeId=ph.traineeId and tp.programmeBatchId=ph.programmeBatchId
	                group by tp.programmeBatchId, tp.programmePayableAmount, tp.subsidyId, tp.subsidyAmt
                )
                --find the no of modules per programme
                , PROG_MOD_CTE as (
	                select count(*) as noOfMod, p.programmeId
	                from programme_structure p inner join bundle b on p.bundleId=b.bundleId inner join bundle_module bm on bm.bundleId=b.bundleId and bm.defunct='N'
	                where p.programmeId in (select programmeId from CLASS_CTE)
	                group by p.programmeId
                )
                --find the no of trainees who has complete the class (meaning pass all modules including exemption)
                , COMPLETE_HC_CTE as (
	                select programmeId, programmeBatchId, subsidyId, programmeNettAmount, count(*) as pax
	                from (
		                select tm.programmeId, programmeBatchId, traineeId, tm.subsidyId, tm.programmePayableAmount-isnull(tm.subsidyAmt, 0) as programmeNettAmount, case when tm.noOfPassedMod=pm.noOfMod then 1 else 0 end as isComplete 
		                from (
			                select tp.programmeId, tm.programmeBatchId, tm.traineeId, tp.programmePayableAmount, tp.subsidyId, tp.subsidyAmt, count(*) as noOfPassedMod
			                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' and tm.moduleResult in ('" + ModuleResult.C.ToString() + @"', '" + ModuleResult.EXEM.ToString() + @"') and tp.programmeBatchId in (select programmeBatchId from CLASS_CTE)
			                group by tp.programmeId, tm.programmeBatchId, tm.traineeId, tp.programmePayableAmount, tp.subsidyAmt, tp.subsidyId
		                ) tm inner join PROG_MOD_CTE pm on tm.programmeId=pm.programmeId
	                ) tbl where isComplete=1
	                group by programmeId, programmeBatchId, subsidyId, programmeNettAmount
                )
                , SOA_CTE as (
	                select tp.programmeBatchId, tp.programmeId, tp.subsidyId, tp.programmePayableAmount-isnull(tp.subsidyAmt,0) as programmeNettAmount, count(*) as noOfSOA
	                from trainee_module tm inner join trainee_programme tp on tm.traineeId=tp.traineeId and tm.defunct='N' and tp.programmeBatchId in (select programmeBatchId from CLASS_CTE) and tm.moduleResult='" + ModuleResult.C.ToString() + @"'
	                group by tp.programmeBatchId, tp.programmeId, tp.subsidyId, tp.programmePayableAmount, tp.subsidyAmt
                )
                select cls.programmeId, cls.programmeTitle, cls.programmeBatchId, cls.courseCode, cls.projectCode, cls.programmeStartDate, cls.programmeCompletionDate, convert(varchar, cls.programmeStartDate, 106) as programmeStartDateDisp, convert(varchar, cls.programmeCompletionDate, 106) as programmeCompletionDateDisp
                , sub.subsidyId, sub.subsidyScheme, sub.programmeNettAmount
                , isnull(oth.pax, 0) as nonRFI_HC, isnull(oth.pax, 0) * sub.programmeNettAmount as nonRFIAmt
                , rfi.cnt as noOfRFI, rfi.companyName as RFICompany, rfi.pax as RFI_HC, rfi.programmeTotalAmount as RFIAmount, rfi.referenceNumber as RFIInv, rfi.paymentDate as RFIInvDate
                , isnull(oth.pax, 0) + isnull(rfi.pax,0) as totalHC, (isnull(oth.pax, 0) * sub.programmeNettAmount) + isnull(rfi.programmeTotalAmount,0) as totalAmt
                , isnull(soa.noOfSOA, 0) as soa, isnull(comp.pax,0) as completeHC
                from SUBSIDY_CTE sub inner join CLASS_CTE cls on sub.programmeBatchId=cls.programmeBatchId
                inner join PROG_MOD_CTE pm on pm.programmeId=sub.programmeId
                left outer join RFI_PAY_MERGE_CTE rfi on rfi.programmeBatchId=sub.programmeBatchId and ((sub.subsidyId is null and rfi.subsidyId is null) or sub.subsidyId=rfi.subsidyId) and sub.programmeNettAmount=rfi.programmeNettAmount
                left outer join OTHER_PAY_CTE oth on oth.programmeBatchId=sub.programmeBatchId and ((sub.subsidyId is null and oth.subsidyId is null) or sub.subsidyId=oth.subsidyId) and sub.programmeNettAmount=oth.programmeNettAmount
                left outer join SOA_CTE soa on soa.programmeBatchId=sub.programmeBatchId and ((sub.subsidyId is null and soa.subsidyId is null) or sub.subsidyId=soa.subsidyId) and sub.programmeNettAmount=soa.programmeNettAmount
                left outer join COMPLETE_HC_CTE comp on comp.programmeBatchId=sub.programmeBatchId and ((sub.subsidyId is null and comp.subsidyId is null) or sub.subsidyId=comp.subsidyId) and sub.programmeNettAmount=comp.programmeNettAmount
                order by cls.courseCode, cls.programmeStartDate, cls.programmeCompletionDate, cls.projectCode, sub.programmeNettAmount
                ";

                cmd.Parameters.AddWithValue("@startDt", dtStart);
                cmd.Parameters.AddWithValue("@endDt", dtEnd);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "ACIMonthlyReportSummary()", ex.Message, -1);

                return null;
            }
        }

        public DataTable ACIMonthlySummary(int fyStart, int fyEnd)
        {
            try
            {
                //have to be dyanmic depending on how many fy
                string selEnrolledSQL = "", selCompleteSQL = "", selSOASQL = "", tblEnrolledSQL = "", tblCompleteSQL = "", tblSOASQL = "", totalEnrolledSQL = "", totalCompleteSQL = "", totalSOASQL = "";
                for (int i = 1; i <= fyEnd - fyStart + 1; i++)
                {
                    selEnrolledSQL += ", isnull(e" + i + ".pax, 0) as Enrolled_FY" + i + " ";
                    selCompleteSQL += ", isnull(c" + i + ".pax, 0) as Complete_FY" + i + " ";
                    selSOASQL += ", isnull(s" + i + ".soa, 0) as SOA_FY" + i + " ";

                    totalEnrolledSQL += "isnull(e" + i + ".pax, 0) + ";
                    totalCompleteSQL += "isnull(c" + i + ".pax, 0) + ";
                    totalSOASQL += "isnull(s" + i + ".soa, 0) + ";

                    tblEnrolledSQL += "left outer join (select * from ENROLL_CTE where [level]=" + i + ") e" + i + " on e" + i + ".programmeId=p.programmeId ";
                    tblCompleteSQL += "left outer join (select * from COMPLETE_CTE where [level]=" + i + ") c" + i + " on c" + i + ".programmeId=p.programmeId ";
                    tblSOASQL += "left outer join (select * from SOA_CTE where [level]=" + i + ") s" + i + " on s" + i + ".programmeId=p.programmeId ";
                }
                totalEnrolledSQL = "," + totalEnrolledSQL.Substring(0, totalEnrolledSQL.Length - 2) + "as Enrolled_Total ";
                totalCompleteSQL = "," + totalCompleteSQL.Substring(0, totalCompleteSQL.Length - 2) + "as Complete_Total ";
                totalSOASQL = "," + totalSOASQL.Substring(0, totalSOASQL.Length - 2) + "as SOA_Total ";

                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @"
                -- find all the fy ranges
                ;with FY_CTE as ( 
	                SELECT convert(date, CONCAT( @fys,'-04-01')) AS [StartDate], convert(date, CONCAT( @fys+1,'-03-31')) AS [EndDate], 1 AS [level]
	                UNION ALL
	                SELECT DATEADD(YEAR, 1, [StartDate]), DATEADD(YEAR, 1, [EndDate]), [level] + 1
	                FROM FY_CTE WHERE [StartDate] < convert(date, CONCAT( @fye,'-04-01')) 
                )
                -- find all the batches within FY range
                , CLASS_CTE as (
	                select pb.programmeBatchId, pb.programmeId, fy.[level]
	                from programme_batch pb inner join FY_CTE fy on pb.programmeStartDate between fy.StartDate and fy.EndDate and pb.defunct='N'
                ) 
                -- count the enrolled trainees for each programme
                , ENROLL_CTE as (
	                select count(*) as pax, tp.programmeId, pb.[level]
	                from trainee_programme tp inner join CLASS_CTE pb on pb.programmeBatchId=tp.programmeBatchId and pb.programmeId=tp.programmeId
	                group by tp.programmeId, pb.[level]
                )
                --count the trainees for each programme that has passed all module assessment
                , COMPLETE_CTE as (
	                select tm.programmeId, fy.[level], count(*) as pax from (
		                --check if each trainee of the class has complete all module assessment
		                select tm.programmeBatchId, pb.programmeId, case when tm.modulePassed=pb.noOfMod then 'Y' else 'N' end as isComplete, 
		                --determine the latest assessment date from all the completed modules of each trainee
		                case when tm.finalAssessmentDate is null then tm.firstAssessmentDate when tm.finalAssessmentDate>tm.firstAssessmentDate then tm.finalAssessmentDate else tm.firstAssessmentDate end as assessmentDate
		                from (
			                --count the no of modules that each trainee pass (including exempted)
			                select tm.traineeId, tm.programmeBatchId, count(*) as modulePassed, max(tm.firstAssessmentDate) as firstAssessmentDate, max(finalAssessmentDate) as finalAssessmentDate
			                from trainee_module tm
			                where tm.moduleResult in ('" + ModuleResult.C.ToString() + @"', '" + ModuleResult.EXEM.ToString() + @"') and tm.defunct='N'
			                group by tm.traineeId, tm.programmeBatchId
		                ) tm inner join (
			                --count the no of modules that each class has
			                select pb.programmeBatchId, pb.programmeId, count(*) as noOfMod
			                from programme_batch pb inner join programme_structure p on p.programmeId=pb.programmeId and pb.defunct='N'
			                inner join bundle b on b.bundleId=p.bundleId inner join bundle_module bm on bm.bundleId=b.bundleId and bm.defunct='N'
			                group by pb.programmeBatchId, pb.programmeId
		                ) pb on tm.programmeBatchId=pb.programmeBatchId
	                --count is based on assessment date is in which FY
	                ) tm inner join FY_CTE fy on tm.assessmentDate between fy.StartDate and fy.EndDate and tm.isComplete='Y'
	                group by tm.programmeId, fy.[level]
                )
                --count the trainees on their completed modules (which count as SOA)
                , SOA_CTE as (
	                select tm.programmeId, fy.[level], count(*) as soa from (
		                select pb.programmeId, tm.programmeBatchId, case when finalAssessmentDate is null then firstAssessmentDate when finalAssessmentDate>firstAssessmentDate then finalAssessmentDate else firstAssessmentDate end as assessmentDate
		                from trainee_module tm inner join programme_batch pb on tm.programmeBatchId=pb.programmeBatchId and tm.moduleResult ='" + ModuleResult.C.ToString() + @"' and tm.defunct='N' and pb.defunct='N'
	                ) tm inner join FY_CTE fy on tm.assessmentDate between fy.StartDate and fy.EndDate
	                group by tm.programmeId, fy.[level]
                )
                select p.programmeId, p.programmeType, cr1.codeValueDisplay as programmeTypeDisp, p.programmeLevel, cr2.codeValueDisplay as programmeLevelDisp, p.programmeCode, p.programmeTitle "
                + selEnrolledSQL + totalEnrolledSQL + selCompleteSQL + totalCompleteSQL + selSOASQL + totalSOASQL + @" 
                from programme_structure p inner join code_reference cr1 on p.programmeType=cr1.codeValue and cr1.codeType='PGTYPE' and p.programmeId in (select programmeId from CLASS_CTE) 
                inner join code_reference cr2 on p.programmeLevel=cr2.codeValue and cr2.codeType='PGLVL' "
                + tblEnrolledSQL + tblCompleteSQL + tblSOASQL + @"
                order by p.programmeType, p.programmeLevel, p.programmeCode, p.programmeTitle";

                cmd.Parameters.AddWithValue("@fys", fyStart);
                cmd.Parameters.AddWithValue("@fye", fyEnd);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Report.cs", "ACIMonthlyReportSummary()", ex.Message, -1);

                return null;
            }
        }
    }
}