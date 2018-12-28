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
    public class Module_Management
    {
        private DB_Module dbModule = new DB_Module();

        public DataTable getRecentModulesInBundle()
        {
            return dbModule.getRecentModulesInBundle();
        }

        public DataTable getListModules(string frm, string to)
        {
            bool filterNum = false;
            if (to.Contains("#"))
            {
                filterNum = true;
                to.Replace("#", "");
            }
            return dbModule.getListModules(frm, to, filterNum);
        }

        //Search module structure
        public DataTable searchModuleStructure(string criteria, string value)
        {
            if (criteria != null && criteria != "")
            {
                string condition = "";
                if (criteria == "MC") condition = "UPPER(moduleCode) like @sv";
                else if (criteria == "WSQ") condition = "UPPER(WSQCompetencyCode) like @sv";
                else if (criteria == "MT") condition = "UPPER(moduleTitle) like @sv";

                SqlParameter p = new SqlParameter("@sv", "%" + value.ToUpper() + "%");
                return dbModule.searchModuleStructure(condition, p);
            }
            else return dbModule.searchModuleStructure(null, null);
        }

        public DataTable getModule(int moduleId)
        {
            return dbModule.getModule(moduleId);
        }

        //Get all module structure
        public DataTable getAllModules()
        {
            DataTable dtModule = dbModule.getAllModules();

            return dtModule;
        }

        //Create new module
        public Tuple<bool, string> createModule(string moduleCode, int moduleVersion, string moduleLevel, string moduleTitle, string moduleDescription,
            int moduleCredit, decimal moduleCost, DateTime moduleEffectDate, decimal moduleTrainingHour, string WSQCompetencyCode, int userId)
        {
            if (dbModule.checkModuleCodeExist(moduleCode))
            {
                return new Tuple<bool, string>(false, "Module code already exist.");
            }
            else
            {
                if (dbModule.createModule(moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription,
           moduleCredit, moduleCost, moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, userId))
                    return new Tuple<bool, string>(true, "Module created successfully.");
                else
                    return new Tuple<bool, string>(false, "Error creating module.");
            }

        }

        public bool validateModuleUsed(int id)
        {
            return dbModule.validateModuleUsed(id);
        }

        //update current module version
        public Tuple<bool, string> updateModule(int moduleId, string moduleTitle, int moduleVersion, string moduleLevel, int moduleCredit, decimal moduleTrgHour, decimal moduleCost,
            DateTime moduleEffectiveDate, string WSQCompetencyCode, string moduleDescription, int userId)
        {
            if (dbModule.updateModule(moduleId, moduleTitle, moduleVersion, moduleLevel, moduleCredit, moduleTrgHour, moduleCost,
            moduleEffectiveDate, WSQCompetencyCode, moduleDescription, userId))
                return new Tuple<bool, string>(true, "Module saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating module.");
        }

        //update module version to new version
        public Tuple<bool, string, int> updateModuleNewVer(string moduleCode, string moduleTitle, int moduleVersion, string moduleLevel, int moduleCredit, decimal moduleTrgHour, decimal moduleCost,
            DateTime moduleEffectiveDate, string WSQCompetencyCode, string moduleDescription, int userId)
        {
            int update = dbModule.updateModuleNewVer(moduleCode, moduleTitle, moduleVersion, moduleLevel, moduleCredit, moduleTrgHour, moduleCost,
                moduleEffectiveDate, WSQCompetencyCode, moduleDescription, userId);

            if (update > 0)
                return new Tuple<bool, string, int>(true, "Module saved successfully.", update);
            else
                return new Tuple<bool, string, int>(false, "Error updating module.", 0);
        }

        public Tuple<bool, string> deleteModule(int moduleId, string moduleCode, int userId)
        {
            //allow module to be logically deleted even when used by bundles
            //DataTable dtBatchModule = validateModuleUsed(moduleCode);

            //if (dtBatchModule.Rows.Count != 0)
            //{
            //    return new Tuple<bool, string>(false, "Error deleting module.");
            //}
            //else
            //{
            //    if (dbModule.deleteModule(moduleId, userId))
            //        return new Tuple<bool, string>(true, "Module deleted successfully.");
            //    else
            //        return new Tuple<bool, string>(false, "Error deleting module.");
            //}

            if (dbModule.deleteModule(moduleId, userId))
                return new Tuple<bool, string>(true, "Module deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting module.");
        }
    }
}
