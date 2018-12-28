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
    public class DB_Module
    {
        //Initialize connection string
        private Database_Connection dbConnection = new Database_Connection();

        //Create new module structure
        public bool createModule(string moduleCode, int moduleVersion, string moduleLevel, string moduleTitle, string moduleDescription,
            int moduleCredit, decimal moduleCost, DateTime moduleEffectDate, decimal moduleTrainingHour, string WSQCompetencyCode, int userId)
        {
            try
            {
                string sqlStatement = @"INSERT INTO [module_structure]
                                    
                                      (moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription, moduleCredit, moduleCost,
                                       moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, createdBy)
                                      VALUES
                                      (@moduleCode, @moduleVersion, @moduleLevel, @moduleTitle, @moduleDescription, @moduleCredit, @moduleCost,
                                       @moduleEffectDate, @moduleTrainingHour, @WSQCompetencyCode, @createdBy)";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("@moduleCode", moduleCode);
                cmd.Parameters.AddWithValue("@moduleVersion", moduleVersion);
                cmd.Parameters.AddWithValue("@moduleLevel", moduleLevel);
                cmd.Parameters.AddWithValue("@moduleTitle", moduleTitle);
                cmd.Parameters.AddWithValue("@moduleDescription", moduleDescription);
                cmd.Parameters.AddWithValue("@moduleCredit", moduleCredit);
                cmd.Parameters.AddWithValue("@moduleCost", moduleCost);
                cmd.Parameters.AddWithValue("@moduleEffectDate", moduleEffectDate);
                cmd.Parameters.AddWithValue("@moduleTrainingHour", moduleTrainingHour);
                cmd.Parameters.AddWithValue("@WSQCompetencyCode", WSQCompetencyCode);
                cmd.Parameters.AddWithValue("@createdBy", userId);

                dbConnection.executeNonQuery(cmd);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "createModule()", ex.Message, -1);

                return false;
            }
        }

        public DataTable searchModuleStructure(string condition, SqlParameter p)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT ms.moduleId, ms.moduleCode, ms.moduleVersion, ms.moduleTitle, " +
                    "ms.moduleCredit, ms.WSQCompetencyCode FROM module_structure as ms WHERE ms.defunct = 'N'";

                if (condition != null && condition != "")
                {
                    cmd.CommandText += "and " + condition;
                    cmd.Parameters.Add(p);
                }

                cmd.CommandText += " order by moduleCode, moduleVersion";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "searchModuleStructure()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getRecentModulesInBundle()
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                //each module retrieved is the latest version
                cmd.CommandText = "select top 10 b.moduleid, max(isnull(b.lastModifiedDate, b.createdOn)) as dt, m.moduleCode, m.moduleVersion, m.moduleTitle, convert(nvarchar, m.moduleEffectDate, 106) as moduleEffectDate "
                    + "from (select s1.moduleId, s1.lastModifiedDate, s1.createdOn, s2.moduleCode, s1.defunct from bundle bd inner join bundle_module s1 on bd.bundleId=s1.bundleId and bd.defunct='N' and s1.defunct='N' "
                    + "inner join module_structure s2 on s1.moduleId=s2.moduleId and s2.defunct='N') b "
                    + "inner join module_structure m on m.moduleid=b.moduleid "
                    + "and exists(select 1 from module_structure m2 where m.moduleCode=m2.moduleCode and m2.defunct='N' group by m2.moduleCode having m.moduleVersion=max(m2.moduleVersion)) "
                    + "group by b.moduleid, m.moduleCode, m.moduleVersion, m.moduleTitle, m.moduleEffectDate "
                    + "order by dt desc";

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "getRecentModulesInBundle()", ex.Message, -1);

                return null;
            }
        }

        public DataTable getListModules(string frm, string to, bool containNum = false)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                //each module retrieved is the latest version
                cmd.CommandText = @"SELECT moduleId, moduleCode, moduleVersion, moduleTitle, convert(nvarchar, moduleEffectDate, 106) as moduleEffectDate
                                      FROM module_structure m1
                                      where exists(select 1 from module_structure m2 where m1.moduleCode=m2.moduleCode and m2.defunct='N' group by m2.moduleCode having m1.moduleVersion=max(m2.moduleVersion)) 
                                      and (SUBSTRING(UPPER(moduleCode),1 ,1) BETWEEN @frm AND @to ";

                if (containNum) cmd.CommandText += "or SUBSTRING(moduleCode,1 ,1) BETWEEN '0' AND '9' ";

                cmd.CommandText += ") and defunct='N' order by moduleCode, moduleTitle";
                
                cmd.Parameters.AddWithValue("@frm", frm);
                cmd.Parameters.AddWithValue("@to", to);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "getListModules()", ex.Message, -1);
                
                return null;
            }
        }

        public bool checkModuleCodeExist(string moduleCode)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from module_structure where UPPER(moduleCode) = @moduleCode";
                cmd.Parameters.AddWithValue("@moduleCode", moduleCode.ToUpper());

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "checkModuleCodeExist()", ex.Message, -1);

                return false;
            }
        }

        public DataTable getModule(int moduleId)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();

                cmd.CommandText = "select moduleId, module_structure.moduleCode, moduleVersion, moduleLevel, (SELECT COUNT(*)+1 FROM [module_structure] as ms WHERE ms.moduleCode = module_structure.moduleCode) as maxModVer, "
                + "moduleTitle, moduleDescription, moduleCredit, moduleCost, convert(nvarchar, moduleEffectDate, 106) as moduleEffectDate, "
                + "moduleTrainingHour, WSQCompetencyCode from module_structure where moduleId=@id";

                cmd.Parameters.AddWithValue("@id", moduleId);

                return dbConnection.getDataTable(cmd);
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "getModule()", ex.Message, -1);

                return null;
            }
        }

        public bool validateModuleUsed(int id)
        {
            try
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "SELECT count(*) FROM [programme_batch] as pb "
                + "INNER JOIN [programme_structure] as ps ON pb.programmeId = ps.programmeId "
                + "INNER JOIN [bundle] as b ON ps.bundleId = b.bundleId "
                + "INNER JOIN bundle_module as bm on b.bundleId=bm.bundleId and bm.defunct='N' "
                + "INNER JOIN [module_structure] as ms ON bm.moduleId = ms.moduleId "
                + "WHERE ms.moduleId = @id "
                    //only get classes where the registration has started or the class has not completed
                + "AND DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) >= pb.programmeRegStartDate and DATEADD(dd, DATEDIFF(dd, 0, getdate()), 0) < pb.programmeCompletionDate";

                cmd.Parameters.AddWithValue("@id", id);

                return dbConnection.executeScalarInt(cmd) > 0 ? true : false;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "validateModuleUsed()", ex.Message, -1);

                return true;
            }
        }

        //Get all module structure
        public DataTable getAllModules()
        {
            try
            {
                string sqlStatement = @"SELECT ms.moduleId, ms.moduleCode, ms.moduleVersion, ms.moduleTitle, " +
                    "ms.moduleCredit, ms.WSQCompetencyCode FROM [module_structure] as ms where ms.defunct='N' order by moduleCode, moduleVersion";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                DataTable dtModuleStructure = dbConnection.getDataTable(cmd);

                return dtModuleStructure;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "getAllModules()", ex.Message, -1);

                return null;
            }
        }

        //New version of existing module
        public int updateModuleNewVer(string moduleCode, string moduleTitle, int moduleVersion, string moduleLevel, int moduleCredit, decimal moduleTrgHour, decimal moduleCost,
            DateTime moduleEffectiveDate, string WSQCompetencyCode, string moduleDescription, int userId)
        {
            try
            {
                string sqlStatement = @"
                                      INSERT INTO [module_structure]
                                    
                                      (moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription, moduleCredit, moduleCost,
                                      moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, createdBy)
                                      VALUES
                                      (@moduleCode, @moduleVersion, @moduleLevel, @moduleTitle, @moduleDescription, @moduleCredit, @moduleCost,
                                      @moduleEffectDate, @moduleTrainingHour, @WSQCompetencyCode, @createdBy); SELECT CAST(scope_identity() AS int);";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("moduleCode", moduleCode);
                cmd.Parameters.AddWithValue("moduleTitle", moduleTitle);
                cmd.Parameters.AddWithValue("moduleVersion", moduleVersion);
                cmd.Parameters.AddWithValue("moduleLevel", moduleLevel);
                cmd.Parameters.AddWithValue("moduleCredit", moduleCredit);
                cmd.Parameters.AddWithValue("moduleTrainingHour", moduleTrgHour);
                cmd.Parameters.AddWithValue("moduleCost", moduleCost);
                cmd.Parameters.AddWithValue("moduleEffectDate", moduleEffectiveDate);
                cmd.Parameters.AddWithValue("moduleDescription", moduleDescription);
                cmd.Parameters.AddWithValue("WSQCompetencyCode", WSQCompetencyCode);
                cmd.Parameters.AddWithValue("createdBy", userId);

                int moduleId = dbConnection.executeScalarInt(cmd);

                return moduleId;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "updateModuleNewVer()", ex.Message, -1);

                return 0;
            }
        }

        //Update module structure of the current version
        public bool updateModule(int moduleId, string moduleTitle, int moduleVersion, string moduleLevel, int moduleCredit, decimal moduleTrgHour, decimal moduleCost,
            DateTime moduleEffectiveDate, string WSQCompetencyCode, string moduleDescription, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;

                string sqlStatement = @"
                                      UPDATE [module_structure]
                                      SET moduleTitle = @moduleTitle, moduleLevel = @moduleLevel, moduleCredit = @moduleCredit,
                                      moduleTrainingHour = @moduleTrgHour, moduleCost = @moduleCost, moduleEffectDate = @moduleEffectiveDate, WSQCompetencyCode = @WSQCompetencyCode,
                                      moduleDescription = @moduleDescription, lastModifiedBy = @lastModifiedBy, lastModifiedDate = @lastModifiedDate WHERE moduleId = @moduleId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("moduleTitle", moduleTitle);
                cmd.Parameters.AddWithValue("moduleLevel", moduleLevel);
                cmd.Parameters.AddWithValue("moduleCredit", moduleCredit);
                cmd.Parameters.AddWithValue("moduleTrgHour", moduleTrgHour);
                cmd.Parameters.AddWithValue("moduleCost", moduleCost);
                cmd.Parameters.AddWithValue("moduleEffectiveDate", moduleEffectiveDate);
                cmd.Parameters.AddWithValue("WSQCompetencyCode", WSQCompetencyCode);
                cmd.Parameters.AddWithValue("moduleDescription", moduleDescription);
                cmd.Parameters.AddWithValue("moduleId", moduleId);
                cmd.Parameters.AddWithValue("lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("lastModifiedDate", now);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "updateModule()", ex.Message, -1);

                return false;
            }
        }

        public bool deleteModule(int moduleId, int userId)
        {
            try
            {
                DateTime now = DateTime.Now;
                string defunct = "Y";

                string sqlStatement = @"
                                      UPDATE [module_structure]
                                      SET defunct = @defunct, lastModifiedBy = @lastModifiedBy, lastModifiedDate = @lastModifiedDate WHERE moduleId = @moduleId";

                SqlCommand cmd = new SqlCommand(sqlStatement);

                cmd.Parameters.AddWithValue("moduleId", moduleId);
                cmd.Parameters.AddWithValue("defunct", defunct);
                cmd.Parameters.AddWithValue("lastModifiedBy", userId);
                cmd.Parameters.AddWithValue("lastModifiedDate", now);

                bool success = dbConnection.executeNonQuery(cmd);

                return success;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "DB_Module.cs", "deleteModule()", ex.Message, -1);

                return false;
            }
        }
    }
}
