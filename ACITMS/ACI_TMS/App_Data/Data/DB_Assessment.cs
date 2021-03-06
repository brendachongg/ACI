﻿using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace DataLayer
{
    public class DB_Assessment
    {
        private Database_Connection dbConnection = new Database_Connection();

        public bool isOwnBatchModule(int userId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from batch_module bm inner join batchModule_session s on bm.batchModuleId=s.batchModuleId where "
                    + "assessorUserId=@uid and bm.batchModuleId=@id";

                cmd.Parameters.AddWithValue("@id", batchModuleId);
                cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.executeScalarInt(cmd) == 0 ? false : true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "isOwnBatchModule()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getSOADetails(List<Tuple<string, int>> lstTrainees)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = @";with CTE_Salary as (
                        select c1.codeValue, convert(int, c1.codeValueDisplay) as SalaryFrom,  convert(int, c2.codeValueDisplay) as SalaryTo 
                        from code_reference c1 inner join code_reference c2 on c1.codeType='PAYRG' and c2.codeType='PAYRG' and c1.defunct='N' and c2.defunct='N'
                         and convert(int, c2.codeValue)=convert(int, c1.codeValue) + 1 )
                    select t.idType as [Trainee IS Type], 
                            t.idNumber as [Trainee ID Number], 
                            t.fullName as [Trainee Name (As in NRIC)], 
                            t.gender as [Trainee Gender], 
                            t.nationality as [Trainee Nationality], 
                            format(t.birthDate, 'ddMMyyyy') as [Trainee Date of Birth DDMMYYYY], 
                            t.race as [Trainee Race], 
                            isnull(t.contactNumber1, '') as [Trainee Contact No (Mobile)], 
                            isnull(t.contactNumber2, '') as [Trainee Contact No (Others)], 
                            isnull(t.emailAddress, '') as [Trainee Email Address], 
                            isnull(eh.companyName, 'NA') as [Company Name (Key in NA if not applicable)], 
                            isnull(eh.occupationCode, 'X') as [Designation], 
                            'ENG' as [Medium of Assessment], t.highestEducation as [Education Level], 
                            case when scf.codeValue is not null then scf.codeValue when eh.salaryAmount is null or eh.salaryAmount=0 then '00' else '07' end as [Salary Range],
                            'Asian Culinary Institute' as [Assessment Venue], 
                            format(pb.programmeStartDate, 'ddMMyyyy') as [Course Start Date (DDMMYYYY)], 
                            p.SSGRefNum as [Course Reference Number (Refer to Course Listing in SkillsConnect)], 
                            ms.WSQCompetencyCode as [Competency Standard Code (Refer to Course Listing in SkillsConnect)], 
                            'SOA-001' as [Cert Code], 
                            --case when tm.processSOADate is null then 'N' else 'U' end as [Submission Type], 
                            'N' as [Submission Type], 
                            format(isnull(tm.finalAssessmentDate, tm.firstAssessmentDate), 'ddMMyyyy') as [Date Of Assessment DDMMYYYY], 
                            tm.moduleResult as [Result], 
                            u1.idNumber as [Trainer ID], 
                            u2.idNumber as [Assessor ID], 
                        '1' as [Generating of e-Cert (For Finance SOA, please select “No” as no e-Cert will be generated)], 
                            'OTHERS' as [Type Of Registration], 
                            'ATO-00042' as [Company Registration Number] 
                    from trainee t inner join trainee_module tm on tm.defunct='N' and tm.traineeId=t.traineeId and tm.sitInModule='N' and tm.assessmentCompleted='Y'
                        inner join module_structure ms on ms.moduleId=tm.moduleId
                        inner join programme_batch pb on pb.programmeBatchId=tm.programmeBatchId
                        inner join programme_structure p on pb.programmeId=p.programmeId 
                        inner join batch_module bm on bm.batchModuleId=tm.batchModuleId
                        inner join aci_user u2 on u2.userId=isnull(tm.finalAssessorId, tm.firstAssessorId) 
                        left outer join aci_user u1 on u1.userId=bm.trainerUserId1 
                        left outer join trainee_employment_history eh on eh.traineeId=t.traineeId and eh.currentEmployment='Y'
                        left outer join CTE_Salary scf on eh.salaryAmount is not null and eh.salaryAmount > scf.SalaryFrom and eh.salaryAmount <= scf.SalaryTo 
                    where ";

                int i = 0;
                foreach (Tuple<string, int> t in lstTrainees)
                {
                    cmd.CommandText += "(tm.traineeId=@tid" + i + " and tm.batchModuleId=@bmid" + i + " and tm.defunct='N') or ";
                    cmd.Parameters.AddWithValue("@tid" + i, t.Item1);
                    cmd.Parameters.AddWithValue("@bmid" + i, t.Item2);
                    i++;
                }

                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 3);
                
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getSOADetails()", ex.Message, -1);

                return null;
            }
        }

        public bool updateSOAStatus(List<Tuple<string, int>> lstTrainees, SOAStatus status, string dtColName,  int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_module set SOAStatus='" + status.ToString() + "', " + dtColName + "=getdate(), lastModifiedBy=@uid, lastModifiedDate=getdate() where ";
                
                int i = 0;
                foreach (Tuple<string, int> t in lstTrainees)
                {
                    cmd.CommandText += "(traineeId=@tid" + i + " and batchModuleId=@bmid" + i + " and defunct='N') or ";
                    cmd.Parameters.AddWithValue("@tid" + i, t.Item1);
                    cmd.Parameters.AddWithValue("@bmid" + i, t.Item2);
                    i++;
                }

                cmd.CommandText = cmd.CommandText.Substring(0, cmd.CommandText.Length - 3);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "updateSOAStatus()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getClashSession(string traineeId, string subquery, SqlParameter[] p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select sessionDate, convert(nvarchar, sessionDate, 106) as sessionDateDisp, sessionPeriod, codeValueDisplay as sessionPeriodDisp, "
                    + "batchCode, moduleTitle, moduleCode, type from ("

                    //main enrolled module sessions (including those repeat and sit in)
                    + "select s.sessionId, s.sessionDate, s.sessionPeriod, pb.batchCode, m.moduleTitle, m.moduleCode, 'MAIN' as type "
                    + "from trainee_module tm inner join batch_module bm on tm.batchModuleId=bm.batchModuleId and tm.traineeId=@tid and (tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + "' or tm.moduleResult is null) and tm.defunct='N' "
                    + "inner join batchModule_session s on bm.batchModuleId=s.batchModuleId inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId "
                    + "inner join module_structure m on m.moduleId=bm.moduleId "
                    //make up sessions
                    + "union select s.sessionId, s.sessionDate, s.sessionPeriod, pb.batchCode, m.moduleTitle, m.moduleCode, 'MAKEUP' as type "
                    + "from trainee_absence_record a inner join batchModule_session s on a.insertedSessionId=s.sessionId and a.traineeId=@tid and a.defunct='N' "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId "
                    + "inner join module_structure m on m.moduleId=bm.moduleId "
                    //reassessment session
                    + "union select s.sessionId, s.sessionDate, s.sessionPeriod, pb.batchCode, m.moduleTitle, m.moduleCode, 'REASSESS' as type "
                    + "from trainee_module tm inner join batchModule_session s on tm.finalAssessmentSessionId=s.sessionId and tm.traineeId=@tid and tm.defunct='N' "
                    + "inner join batch_module bm on bm.batchModuleId=s.batchModuleId inner join programme_batch pb on pb.programmeBatchId=bm.programmeBatchId "
                    + "inner join module_structure m on m.moduleId=bm.moduleId "

                    + ") t inner join code_reference c on c.codeValue=t.sessionPeriod and c.codeType='PERIOD' "
                    + "where exists (" + subquery + ") ";

                cmd.Parameters.AddRange(p);
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getClashSession()", ex.Message, -1);

                return null;
            }
        }

        public bool updateRepeatMod(string traineeId, int batchModuleId, int repeatProgrammeBatchId, int repeatModuleId, int userId)
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
                    cmd.CommandText = "update trainee_module set reTakeBatchModuleId=(select batchModuleId from batch_module where programmeBatchId=@pid and moduleId=@mid), "
                    + "lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";

                    cmd.Parameters.AddWithValue("@pid", repeatProgrammeBatchId);
                    cmd.Parameters.AddWithValue("@mid", repeatModuleId);
                    cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    cmd.CommandText = "insert into trainee_module(traineeId, programmeBatchId, batchModuleId, moduleId, sitInModule, createdBy) "
                        + "values (@tid, @pid, (select batchModuleId from batch_module where programmeBatchId=@pid and moduleId=@mid), @mid, 'N', @uid)";

                    cmd.Parameters.AddWithValue("@pid", repeatProgrammeBatchId);
                    cmd.Parameters.AddWithValue("@mid", repeatModuleId);
                    cmd.Parameters.AddWithValue("@tid", traineeId);
                    cmd.Parameters.AddWithValue("@uid", userId);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Assessment.cs", "updateRepeatMod()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool updateReasessment(string traineeId, int batchModuleId, int sessionId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_module set finalAssessmentSessionId=@sid, lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";

                cmd.Parameters.AddWithValue("@sid", sessionId);
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "updateReasessment()", ex.Message, -1);

                return false;
            }
        }

        public bool removeReassessment(string traineeId, int batchModuleId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_module set finalAssessmentSessionId=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "removeReassessment()", ex.Message, -1);

                return false;
            }
        }

        public int getNoOfUnprocessedSOA()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from trainee_module where defunct='N' and moduleResult='" + ModuleResult.C.ToString() + "' and SOAStatus='" + SOAStatus.NYA.ToString() + "'";

                return dbConnection.executeScalarInt(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getNoOfUnprocessedSOA()", ex.Message, -1);

                return -1;
            }
        }

        public bool removeRepeatMod(string traineeId, int batchModuleId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_module set reTakeBatchModuleId=null, lastModifiedBy=@uid, lastModifiedDate=getdate() where traineeId=@tid and batchModuleId=@bmid";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@uid", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "removeRepeatMod()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getRepeatModDetails(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, tm.reTakeBatchModuleId, tm.programmeBatchId, tm.moduleId, m.moduleCode, m.moduleTitle, left(pb.batchCode, len(pb.batchCode)-len(c1.codeValue)) as batchCode, c1.codeValue as batchType, "
                    + "c1.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, pb.programmeRegEndDate, "
                    + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c2.codeValueDisplay as classModeDisp, pb.programmeId, p.programmeCode, p.programmeVersion, "
                    + "p.programmeLevel, c3.codeValueDisplay as programmeLevelDisp, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeTitle, p.programmeType, c5.codeValueDisplay as programmeTypeDisp "

                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.traineeId=@tid and tm.batchModuleId=@bmid and tm.reTakeModule='Y' and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join programme_batch pb on pb.programmeBatchId=tm.programmeBatchId "

                    + "inner join code_reference c1 on c1.codeValue=right(pb.batchCode, 5) and c1.codeType='CLTYPE' "
                    + "inner join code_reference c2 on c2.codeValue=pb.classMode and c2.codeType='CLMODE' "
                    + "inner join programme_structure p on p.programmeId=pb.programmeId inner join code_reference c3 on c3.codeValue=p.programmeLevel and c3.codeType='PGLVL' "
                    + "inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' inner join code_reference c5 on c5.codeValue=p.programmeType and c5.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getRepeatModDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getReassessmentDetails(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, tm.finalAssessmentSessionId, tm.programmeBatchId, tm.moduleId, m.moduleCode, m.moduleTitle, left(pb.batchCode, len(pb.batchCode)-len(c1.codeValue)) as batchCode, c1.codeValue as batchType, "
                    + "c1.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, pb.programmeRegEndDate, "
                    + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c2.codeValueDisplay as classModeDisp, pb.programmeId, p.programmeCode, p.programmeVersion, "
                    + "p.programmeLevel, c3.codeValueDisplay as programmeLevelDisp, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeTitle, p.programmeType, c5.codeValueDisplay as programmeTypeDisp "

                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.traineeId=@tid and tm.batchModuleId=@bmid and tm.reAssessment='Y' and tm.moduleResult is null and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join programme_batch pb on pb.programmeBatchId=tm.programmeBatchId "
                    + "inner join code_reference c1 on c1.codeValue=right(pb.batchCode, 5) and c1.codeType='CLTYPE' "
                    + "inner join code_reference c2 on c2.codeValue=pb.classMode and c2.codeType='CLMODE' "
                    + "inner join programme_structure p on p.programmeId=pb.programmeId inner join code_reference c3 on c3.codeValue=p.programmeLevel and c3.codeType='PGLVL' "
                    + "inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' inner join code_reference c5 on c5.codeValue=p.programmeType and c5.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                cmd.Parameters.AddWithValue("@tid", traineeId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getReassessmentDetails()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getBatchModuleTraineesAssessment(int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select t.fullName, tm.traineeId, tm.moduleResult, tm.assessmentCompleted, tm.firstAssessmentDate, "
                    + "convert(nvarchar, tm.firstAssessmentDate, 106) as firstAssessmentDateDisp, tm.firstAssessorId, u1.userName as firstAssessorName, "
                    + "tm.reAssessment, tm.reTakeModule, tm.finalAssessmentDate, convert(nvarchar, tm.finalAssessmentDate, 106) as finalAssessmentDateDisp, "
                    + "tm.finalAssessorId, u2.userName as finalAssessorName "
                    + "from trainee_module tm inner join trainee t on tm.traineeId=t.traineeId and batchModuleId=@bmid and tm.sitInModule='N' and tm.defunct='N' "
                    + "and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() + "') "
                    + "left outer join aci_user u1 on u1.userId=tm.firstAssessorId left outer join aci_user u2 on u2.userId=tm.finalAssessorId "
                    + "order by t.fullName";

                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getBatchModuleTraineesAssessment()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchReassessmentTrainees(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, t.idNumber, tm.batchModuleId, m.moduleId, m.moduleCode, m.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode "
                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.reAssessment='Y' and tm.moduleResult is null and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join batch_module bm on bm.batchModuleId=tm.batchModuleId and tm.moduleId=bm.moduleId "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId inner join programme_structure p on p.programmeId=b.programmeId "
                    + (condition != null && condition != "" ? "where " + condition : "") + " order by t.fullName, m.moduleTitle";

                if (p != null) cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchReassessmentTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchRepeatModTrainees(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, t.idNumber, tm.batchModuleId, m.moduleId, m.moduleCode, m.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode "
                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.reTakeModule='Y' and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join batch_module bm on bm.batchModuleId=tm.batchModuleId and tm.moduleId=bm.moduleId "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId inner join programme_structure p on p.programmeId=b.programmeId "
                    + (condition != null && condition != "" ? "where " + condition : "") + " order by t.fullName, m.moduleTitle";

                if (p != null) cmd.Parameters.Add(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchRepeatModTrainees()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchBatchModuleInfo(string search, int userId, bool searchAll)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select bm.batchModuleId, m.moduleId, m.moduleCode, m.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode "
                    + "from batch_module bm inner join module_structure m on bm.moduleId=m.moduleId and bm.defunct='N' "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId and b.defunct='N' "
                    //batch must have started first
                    + "and b.programmeStartDate<=DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) "
                    + "inner join programme_structure p on p.programmeId=b.programmeId where (UPPER(m.moduleCode) like @m or UPPER(m.moduleTitle) like @m) "
                    + "and exists (select 1 from trainee_module tm where tm.batchModuleId=bm.batchModuleId and tm.defunct='N' and tm.sitInModule='N') "
                    + (searchAll ? "" : "and bm.assessorUserId=@uid ")
                    + "order by m.moduleTitle, b.batchCode";

                cmd.Parameters.AddWithValue("@m", "%" + search.ToUpper() + "%");
                if (!searchAll) cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchBatchModuleInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchSOATrainee(string condition, SqlParameter[] p,  SOAStatus[] status)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select m.traineeId, t.fullName, t.idNumber, m.moduleId, mod.moduleCode, mod.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode, m.batchModuleId, "
                    + "isnull(m.SOAStatus, '" + SOAStatus.NYA.ToString() + "') as SOAStatus, isnull(c.codeValueDisplay, 'Not Yet Attain') as SOAStatusDisp "
                    + "from trainee_module m inner join trainee t on t.traineeId=m.traineeId and m.sitInModule='N' and m.defunct='N' "
                    //trainee must have pass the module
                    + "and m.moduleResult='" + ModuleResult.C.ToString() + "' "
                    + "and m.SOAStatus in ('" + status[0].ToString() + "'";

                for (int i = 1; i < status.Length; i++)
                    cmd.CommandText += ",'" + status[i].ToString() + "'";

                //batch must have started first
                cmd.CommandText += ") inner join programme_batch b on b.programmeBatchId=m.programmeBatchId and b.defunct='N' and b.programmeStartDate<=getdate() "
                    + "inner join programme_structure p on p.programmeId=b.programmeId inner join module_structure mod on mod.moduleId=m.moduleId "
                    + "inner join batch_module bm on bm.programmeBatchId=b.programmeBatchId and bm.moduleId=m.moduleId and bm.defunct='N' "
                    + "left outer join code_reference c on m.SOAStatus=c.CodeValue and c.codeType='SOA' "
                    + "where " + condition + " order by b.batchCode, mod.moduleCode, t.fullName, t.idNumber";

                cmd.Parameters.AddRange(p);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchSOATrainee()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchClassInfo(string search, int userid, bool searchAll)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select bm.batchModuleId, m.moduleId, m.moduleCode, m.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode "
                    + "from batch_module bm inner join module_structure m on bm.moduleId=m.moduleId and bm.defunct='N' "
                    + "inner join programme_batch b on b.programmeBatchId=bm.programmeBatchId and b.defunct='N' "
                    //batch must have started first
                    + "and b.programmeStartDate<=DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) "
                    + "inner join programme_structure p on p.programmeId=b.programmeId where (UPPER(b.batchCode) like @batchCode) "
                    + "and exists (select 1 from trainee_module tm where tm.batchModuleId=bm.batchModuleId and tm.defunct='N' and tm.sitInModule='N') "
                    + (searchAll ? "" : "and bm.assessorUserId=@uid ")
                    + "order by m.moduleTitle, b.batchCode";

                cmd.Parameters.AddWithValue("@batchCode", "%" + search.ToUpper() + "%");
                if (!searchAll) cmd.Parameters.AddWithValue("@uid", userid);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchBatchModuleInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable searchTraineeInfo(string search, int userId, bool searchAll)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select m.traineeId, t.fullName, t.idNumber, m.moduleId, mod.moduleCode, mod.moduleTitle, p.programmeId, p.programmeCode, p.programmeTitle, b.batchCode, m.batchModuleId "
                    + "from trainee_module m inner join trainee t on t.traineeId=m.traineeId and m.sitInModule='N' and (m.moduleResult is null or m.moduleResult<>'" + ModuleResult.EXEM.ToString() + "') and m.defunct='N' "
                    //batch must have started first
                    + "inner join programme_batch b on b.programmeBatchId=m.programmeBatchId and b.defunct='N' and b.programmeStartDate<=getdate() "
                    + "inner join programme_structure p on p.programmeId=b.programmeId inner join module_structure mod on mod.moduleId=m.moduleId "
                    + "inner join batch_module bm on bm.programmeBatchId=b.programmeBatchId and bm.moduleId=m.moduleId and bm.defunct='N' "
                    + "where (UPPER(t.fullName) like @t or UPPER(m.traineeId) like @t) "
                    + (searchAll ? "" : "and bm.assessorUserId=@uid ")
                    + "order by t.fullName, m.traineeId ";

                cmd.Parameters.AddWithValue("@t", "%" + search.ToUpper() + "%");
                if (!searchAll) cmd.Parameters.AddWithValue("@uid", userId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "searchTraineeInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeModuleInfo(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.traineeId, t.fullName, tm.programmeBatchId, tm.moduleId, m.moduleCode, m.moduleTitle, left(pb.batchCode, len(pb.batchCode)-len(c1.codeValue)) as batchCode, c1.codeValue as batchType, "
                    + "c1.codeValueDisplay as batchTypeDisp, pb.projectCode, pb.programmeRegStartDate, convert(nvarchar, pb.programmeRegStartDate, 106) as programmeRegStartDateDisp, pb.programmeRegEndDate, "
                    + "convert(nvarchar, pb.programmeRegEndDate, 106) as programmeRegEndDateDisp, pb.programmeStartDate, convert(nvarchar, pb.programmeStartDate, 106) as programmeStartDateDisp, pb.programmeCompletionDate, "
                    + "convert(nvarchar, pb.programmeCompletionDate, 106) as programmeCompletionDateDisp, pb.batchCapacity, pb.classMode, c2.codeValueDisplay as classModeDisp, pb.programmeId, p.programmeCode, p.programmeVersion, "
                    + "p.programmeLevel, c3.codeValueDisplay as programmeLevelDisp, p.programmeCategory, c4.codeValueDisplay as programmeCategoryDisp, p.programmeTitle, p.programmeType, c5.codeValueDisplay as programmeTypeDisp "

                    + "from trainee_module tm inner join trainee t on t.traineeId=tm.traineeId and tm.traineeId=@tid and tm.batchModuleId=@bmid and tm.sitInModule='N' and (tm.moduleResult is null or tm.moduleResult<>'" + ModuleResult.EXEM.ToString() 
                    + "') and tm.defunct='N' "
                    + "inner join module_structure m on tm.moduleId=m.moduleId inner join programme_batch pb on pb.programmeBatchId=tm.programmeBatchId "

                    + "inner join code_reference c1 on c1.codeValue=right(pb.batchCode, 5) and c1.codeType='CLTYPE' "
                    + "inner join code_reference c2 on c2.codeValue=pb.classMode and c2.codeType='CLMODE' "
                    + "inner join programme_structure p on p.programmeId=pb.programmeId inner join code_reference c3 on c3.codeValue=p.programmeLevel and c3.codeType='PGLVL' "
                    + "inner join code_reference c4 on c4.codeValue=p.programmeCategory and c4.codeType='PGCAT' inner join code_reference c5 on c5.codeValue=p.programmeType and c5.codeType='PGTYPE' ";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getTraineeModuleInfo()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getTraineeModuleAssessment(string traineeId, int batchModuleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select tm.moduleResult, c.codeValueDisplay as moduleResultDisp, tm.assessmentCompleted, tm.firstAssessmentDate, convert(nvarchar, tm.firstAssessmentDate, 106) as firstAssessmentDateDisp, tm.firstAssessorId, "
                    + "a1.userName as firstAssessorName, tm.reAssessment, isnull(tm.finalAssessmentDate, s.sessionDate) as finalAssessmentDate, convert(nvarchar, isnull(tm.finalAssessmentDate, s.sessionDate), 106) as finalAssessmentDateDisp, "
                    + "tm.finalAssessorId, a2.userName as finalAssessorName "
                    + "from trainee_module tm left outer join aci_user a1 on a1.userId=tm.firstAssessorId and tm.defunct='N' left outer join aci_user a2 on a2.userId=tm.finalAssessorId "
                    + "left outer join batchModule_session s on tm.finalAssessmentSessionId = s.sessionId left outer join code_reference c on c.codeValue=tm.moduleResult and c.codeType='RESULT' "
                    + "where tm.traineeId=@tid and tm.batchModuleId=@bmid and tm.sitInModule='N'";

                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);
                 
                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "getTraineeModuleAssessment()", ex.Message, -1);

                return null;
            }
        }

        public bool updateBatchModuleAssessment(int batchModuleId, DataTable dtAssessment, int userId)
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
                    cmd.CommandText = "update trainee_module set moduleResult=@r, assessmentCompleted=@complete, firstAssessmentDate=@dt1, firstAssessorId=@a1, reAssessment=@ra, reTakeModule=@rm, "
                    + "finalAssessmentDate=@dt2, finalAssessorId=@a2, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                    + "where traineeId=@tid and batchModuleId=@bmid and ((moduleResult is null and @r is not null) or (moduleResult is not null and @r is null) or moduleResult<>@r "
                    + "or (assessmentCompleted is null and @complete is not null) or (assessmentCompleted is not null and @complete is null) or assessmentCompleted<>@complete "
                    + "or (firstAssessmentDate is null and @dt1 is not null) or (firstAssessmentDate is not null and @dt1 is null) or firstAssessmentDate<>@dt1 "
                    + "or (firstAssessorId is null and @a1 is not null) or (firstAssessorId is not null and @a1 is null) or firstAssessorId<>@a1 "
                    + "or (reAssessment is null and @ra is not null) or (reAssessment is not null and @ra is null) or reAssessment<>@ra "
                    + "or (reTakeModule is null and @rm is not null) or (reTakeModule is not null and @rm is null) or reTakeModule<>@rm "
                    + "or (finalAssessmentDate is null and @dt2 is not null) or (finalAssessmentDate is not null and @dt2 is null) or finalAssessmentDate<>@dt2 "
                    + "or (finalAssessorId is null and @a2 is not null) or (finalAssessorId is not null and @a2 is null) or finalAssessorId<>@a2 "
                    + ")";

                    foreach (DataRow dr in dtAssessment.Rows)
                    {
                        cmd.Parameters.AddWithValue("@r", dr["moduleResult"]);
                        cmd.Parameters.AddWithValue("@complete", dr["assessmentCompleted"]);
                        cmd.Parameters.AddWithValue("@dt1", dr["assessmentDate1"]);
                        cmd.Parameters.AddWithValue("@a1", dr["assessorId1"]);
                        cmd.Parameters.AddWithValue("@ra",  dr["reAssessment"]);
                        cmd.Parameters.AddWithValue("@rm",  dr["reTakeModule"]);
                        cmd.Parameters.AddWithValue("@dt2", dr["assessmentDate2"]);
                        cmd.Parameters.AddWithValue("@a2", dr["assessorId2"]);
                        cmd.Parameters.AddWithValue("@uid", userId);
                        cmd.Parameters.AddWithValue("@tid", dr["traineeId"]);
                        cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                        cmd.ExecuteNonQuery();
                        cmd.Parameters.Clear();
                    }

                    trans.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    Log_Handler lh = new Log_Handler();
                    lh.WriteLog(ex, "DB_Assessment.cs", "updateBatchModuleAssessment()", ex.Message, -1);

                    trans.Rollback();

                    return false;
                }
                finally
                {
                    sqlConn.Close();
                }
            }
        }

        public bool updateTraineeModuleAssessment(string traineeId, int batchModuleId, string moduleResult, string assessmentCompleted, DateTime firstAssessmentDate, int firstAssessorId, string reAssessment, string reTakeModule,
            DateTime finalAssessmentDate, int finalAssessorId, int userId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "update trainee_module set moduleResult=@r, assessmentCompleted=@complete, firstAssessmentDate=@dt1, firstAssessorId=@a1, reAssessment=@ra, reTakeModule=@rm, "
                    + "finalAssessmentDate=@dt2, finalAssessorId=@a2, lastModifiedBy=@uid, lastModifiedDate=getdate() "
                    + "where traineeId=@tid and batchModuleId=@bmid and ((moduleResult is null and @r is not null) or (moduleResult is not null and @r is null) or moduleResult<>@r "
                    + "or (assessmentCompleted is null and @complete is not null) or (assessmentCompleted is not null and @complete is null) or assessmentCompleted<>@complete "
                    + "or (firstAssessmentDate is null and @dt1 is not null) or (firstAssessmentDate is not null and @dt1 is null) or firstAssessmentDate<>@dt1 "
                    + "or (firstAssessorId is null and @a1 is not null) or (firstAssessorId is not null and @a1 is null) or firstAssessorId<>@a1 "
                    + "or (reAssessment is null and @ra is not null) or (reAssessment is not null and @ra is null) or reAssessment<>@ra "
                    + "or (reTakeModule is null and @rm is not null) or (reTakeModule is not null and @rm is null) or reTakeModule<>@rm "
                    + "or (finalAssessmentDate is null and @dt2 is not null) or (finalAssessmentDate is not null and @dt2 is null) or finalAssessmentDate<>@dt2 "
                    + "or (finalAssessorId is null and @a2 is not null) or (finalAssessorId is not null and @a2 is null) or finalAssessorId<>@a2 "
                    + ")";

                cmd.Parameters.AddWithValue("@r", moduleResult == null ? (object)DBNull.Value : moduleResult);
                cmd.Parameters.AddWithValue("@complete", assessmentCompleted == null ? (object)DBNull.Value : assessmentCompleted);
                cmd.Parameters.AddWithValue("@dt1", firstAssessmentDate == DateTime.MinValue ? (object)DBNull.Value : firstAssessmentDate);
                cmd.Parameters.AddWithValue("@a1", firstAssessorId == -1 ? (object)DBNull.Value : firstAssessorId);
                cmd.Parameters.AddWithValue("@ra", reAssessment == null ? (object)DBNull.Value : reAssessment);
                cmd.Parameters.AddWithValue("@rm", reTakeModule == null ? (object)DBNull.Value : reTakeModule);
                cmd.Parameters.AddWithValue("@dt2", finalAssessmentDate == DateTime.MinValue ? (object)DBNull.Value : finalAssessmentDate);
                cmd.Parameters.AddWithValue("@a2", finalAssessorId == -1 ? (object)DBNull.Value : finalAssessorId);
                cmd.Parameters.AddWithValue("@uid", userId);
                cmd.Parameters.AddWithValue("@tid", traineeId);
                cmd.Parameters.AddWithValue("@bmid", batchModuleId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Assessment.cs", "updateTraineeModuleAssessment()", ex.Message, -1);

                return false;
            }
        }
    }
}