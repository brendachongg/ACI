using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using System.Data;
using System.Web;
using GeneralLayer;

namespace LogicLayer
{
   public class Access_Control_Management
    {
       private DB_Access_Control dbAccess = new DB_Access_Control();

       public DataTable getAllFunctionGrps()
       {
           return dbAccess.getAllFunctionGrps();
       }

       //get all the functions from database
       public DataTable getFunctions()
       {
           return dbAccess.getAccessFunctions();
       }

       public bool updateFunction(string grp, int id, string desc, int userId)
       {
           return dbAccess.updateFunction(grp, id, desc, userId);
       }

       public bool deleteFunction(int functionId, int userId)
       {
           return dbAccess.deleteFunction(functionId, userId);
       }

       //add a new function
       public Tuple<bool, string> addFunction(string group, string function, string desc, int userId)
       {
            if (dbAccess.checkExistingFunction(function))
               return new Tuple<bool, string>(false, "Function already exist.");

            if (dbAccess.addFunction(group, function, desc, userId))
                return new Tuple<bool, string>(true, "Function created successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving function.");
       }

       public DataTable getFunctionByGroup(string group)
       {
           return dbAccess.getFunctionByGroup(group);
       }

       public bool checkAccessRight(int userId, string functionName)
       {
           return dbAccess.checkUserAccess(userId, functionName);
       }

       public bool assignAccessRights(List<int> selFunction, int staffId, int userId)
       {
           return dbAccess.assignAccessRights(selFunction, staffId, userId);
       }

       public DataTable getUserAccess(int staffId)
       {
           return dbAccess.getUserAccess(staffId); 
       }

       
       public List<string> getUserAccessFunctName(int staffId)
       {
           List<string> access = new List<string>();

           DataTable dt = dbAccess.getUserAccess(staffId);
           if (dt == null || dt.Rows.Count == 0) return access;

           foreach (DataRow dr in dt.Rows) access.Add(dr["functionName"].ToString());

           //temporary to return the list of access right according to fix user id. When access right deployed should get from database
           //if (staffId == 1 || staffId==2)
           //{
           //    access.Add(AccessRight_Constance.BUNDLE_VIEW);
           //    access.Add(AccessRight_Constance.BATCH_VIEW);
           //    access.Add(AccessRight_Constance.MODULE_VIEW);
           //    access.Add(AccessRight_Constance.PROGRAM_VIEW);
           //    access.Add(AccessRight_Constance.SESSION_VIEW);
           //    access.Add(AccessRight_Constance.ATTENDANCE_VIEW);
           //    access.Add(AccessRight_Constance.ASSESSMENT_VIEW);
           //    access.Add(AccessRight_Constance.REASSESSMENT_VIEW);
           //    access.Add(AccessRight_Constance.REPEATMOD_VIEW);
           //    access.Add(AccessRight_Constance.MAKEUP_VIEW);
           //    access.Add(AccessRight_Constance.SITIN_VIEW);
           //    access.Add(AccessRight_Constance.VENUE_VIEW);
           //    access.Add(AccessRight_Constance.BOOKING_VIEW);
           //    access.Add(AccessRight_Constance.REG_NEW);
           //    access.Add(AccessRight_Constance.ACCESS_CONTROL);
           //    access.Add(AccessRight_Constance.ACCESS_FUNCT);
           //    access.Add(AccessRight_Constance.AUDIT_VIEW);
           //    access.Add(AccessRight_Constance.PAYMT_VIEW);
           //    access.Add(AccessRight_Constance.REGFEE_VIEW);
           //    access.Add(AccessRight_Constance.SUBSIDY_VIEW);
           //    access.Add(AccessRight_Constance.SOA_VIEW);
           //    access.Add(AccessRight_Constance.APPLN_VIEW);
           //    access.Add(AccessRight_Constance.TRAINEE_VIEW);
           //    access.Add(AccessRight_Constance.SUSPEND_VIEW);
           //}
           //else if (staffId ==3)
           //{
           //    access.Add(AccessRight_Constance.APPLN_VIEW);
           //    access.Add(AccessRight_Constance.SUSPEND_VIEW);
           //    access.Add(AccessRight_Constance.TRAINEE_VIEW);
           //    access.Add(AccessRight_Constance.PAYMT_VIEW);
           //    access.Add(AccessRight_Constance.REGFEE_VIEW);
           //    access.Add(AccessRight_Constance.SUBSIDY_VIEW);
           //    access.Add(AccessRight_Constance.REG_NEW);
           //}
           //else if (staffId == 4)
           //{
           //    access.Add(AccessRight_Constance.BUNDLE_VIEW);
           //    access.Add(AccessRight_Constance.BATCH_VIEW);
           //    access.Add(AccessRight_Constance.MODULE_VIEW);
           //    access.Add(AccessRight_Constance.PROGRAM_VIEW);
           //    access.Add(AccessRight_Constance.SESSION_VIEW);
           //    access.Add(AccessRight_Constance.ATTENDANCE_VIEW);
           //    access.Add(AccessRight_Constance.ASSESSMENT_VIEW);
           //    access.Add(AccessRight_Constance.REASSESSMENT_VIEW);
           //    access.Add(AccessRight_Constance.REPEATMOD_VIEW);
           //    access.Add(AccessRight_Constance.MAKEUP_VIEW);
           //    access.Add(AccessRight_Constance.SITIN_VIEW);
           //    access.Add(AccessRight_Constance.VENUE_VIEW);
           //    access.Add(AccessRight_Constance.BOOKING_VIEW);
           //    access.Add(AccessRight_Constance.SOA_VIEW);
           //}

           return access;
       }
       
    }
}
