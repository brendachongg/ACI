using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

namespace LogicLayer
{
    public class Bundle_Management
    {
        private DB_Bundle dbBundle = new DB_Bundle();

        //Get all package
        public DataTable getAllBundles()
        {
            return dbBundle.getAllBundles();
        }

        public DataTable searchBundle(string searchKey, string searchValue)
        {
            return dbBundle.searchBundle(searchKey, searchValue);
        }

        public DataTable getBundleTypes()
        {
            return dbBundle.getBundleTypes();
        }

        public Tuple<bool, string> createBundle(string bundleCode, string bundleType, DateTime bundleEffDate, decimal totalCost, DataTable dtModules, string userId)
        {
            if (dbBundle.checkBundleCodeExist(bundleCode))
                return new Tuple<bool, string>(false, "Bundle code already exist.");

            bool success = dbBundle.createBundle(bundleCode, bundleType, bundleEffDate, totalCost, dtModules, userId);
            string msg = success ? "Bundle created successfully." : "Error creating bundle.";

            return new Tuple<bool, string>(success, msg);
        }

        //public Tuple<string, string, string, decimal, bool, DataTable> getBundle(string bundleCode, bool includeDefunct = false)
        public Tuple<string, string, string, string, decimal, bool, DataTable> getBundle(int bundleId, bool includeDefunct = false)
        {
            DataTable dt = dbBundle.getBundle(bundleId);
            if (dt == null || dt.Rows.Count == 0) return null;

            string code = dt.Rows[0]["bundleCode"].ToString();
            string type = dt.Rows[0]["bundleTypeCode"].ToString();
            string typeDesc = dt.Rows[0]["bundleType"].ToString();
            string effDt = dt.Rows[0]["bundleEffectDate"].ToString();
            decimal totalCost = (decimal)dt.Rows[0]["bundleCost"];
            bool defunct = dt.Rows[0]["defunct"].ToString() == "Y" ? false : true;

            dt = dbBundle.getBundleModules(bundleId, includeDefunct);
            if (dt == null || dt.Rows.Count == 0) return null;

            return new Tuple<string, string, string, string, decimal, bool, DataTable>(code, type, typeDesc, effDt, totalCost, defunct, dt);
        }

        //public bool delBundle(string bundleCode, string userId)
        public bool delBundle(int bundleId, int userId)
        {
            return dbBundle.delBundle(bundleId, userId);
        }

        //public bool checkBatchStarted(string bundleCode)
        public bool checkBatchStarted(int bundleId)
        {
            return dbBundle.checkBatchStarted(bundleId);
        }

        private DataTable getBatchModuleInfo(DataTable dt, string filter)
        {
            DataView view = new DataView(dt, filter, "", DataViewRowState.CurrentRows);
            if (view.Count > 0)
            {
                DataTable dtBatchMod = view.ToTable(true, new string[] { "moduleId", "programmeBatchId" });
                StringBuilder sb = new StringBuilder();
                SqlParameter[] p = new SqlParameter[dtBatchMod.Rows.Count * 2];

                int i = 0;
                foreach (DataRow dr in dtBatchMod.Rows)
                {
                    sb.Append("(programmeBatchId=@pid" + i + " and moduleId=@mid" + i + " ) or ");
                    p[i * 2] = new SqlParameter("@pid" + i, dr["programmeBatchId"]);
                    p[i * 2 + 1] = new SqlParameter("@mid" + i, dr["moduleId"]);

                    i++;
                }
                dtBatchMod = (new DB_Batch_Session()).getBatchModuleInfo(sb.ToString().Substring(0, sb.Length - 3), p);

                return dtBatchMod;
            }

            return null;
        }

        //public Tuple<bool, string> updateBundle(string bundleCode, string bundleType, DateTime bundleEffDt, decimal bundleCost, int userId,
        //    DataTable dtNewSession, DataTable dtRemSession, DataTable dtNewModule, DataTable dtRemModule, DataTable dtModOrder)
        public Tuple<bool, string> updateBundle(int bundleId, string bundleType, DateTime bundleEffDt, decimal bundleCost, int userId,
            DataTable dtNewSession, DataTable dtRemSession, DataTable dtNewModule, DataTable dtRemModule, DataTable dtModOrder)
        {
            //for each module with new sessions, check if any clashes
            //eg new module's sessions clashes with other existing modules' session
            //eg. existing modules' new sessions clashes with its own or other existings modules' session
            StringBuilder sb = new StringBuilder();
            SqlParameter[] p;
            int i;
            if (dtNewSession.Rows.Count > 0)
            {
                p = new SqlParameter[dtNewSession.Rows.Count * 3];
                i = 0;
                foreach (DataRow dr in dtNewSession.Rows)
                {
                    //if session date is one of the removed session/module then confirm won clash (cos it's like replacement)
                    if (dtRemSession.Select("programmeBatchId=" + dr["programmeBatchId"].ToString()
                        + " and sessionDate=#" + ((DateTime)dr["sessionDate"]).ToString("MM/dd/yyyy") + "# and sessionPeriod='" + dr["sessionPeriod"].ToString() + "'").Length > 0) continue;
                    if (dtRemModule.Select("programmeBatchId=" + dr["programmeBatchId"].ToString()
                        + " and sessionDate=#" + ((DateTime)dr["sessionDate"]).ToString("MM/dd/yyyy") + "# and sessionPeriod='" + dr["sessionPeriod"].ToString() + "'").Length > 0) continue;

                    sb.Append("(b.programmeBatchId=@pid" + i + " and sessionDate=@dt" + i + " and sessionPeriod=@pt" + i + ") or ");
                    p[i * 3] = new SqlParameter("@pid" + i, dr["programmeBatchId"]);
                    p[i * 3 + 1] = new SqlParameter("@dt" + i, dr["sessionDate"]);
                    p[i * 3 + 2] = new SqlParameter("@pt" + i, dr["sessionPeriod"]);

                    i++;
                }

                if (sb.Length > 0) { 
                    //retrieve any existing duplicated session (excluding those going to be removed)
                    DataTable dtDup=dbBundle.checkDuplicatedSession(sb.ToString().Substring(0, sb.Length - 3), p);
                    if(dtDup==null) return new Tuple<bool,string>(false, "Error checking sessions.");
                    if (dtDup.Rows.Count>0)
                    {
                        string msg = "Duplicated session found for:<br/>";
                        foreach (DataRow dr in dtDup.Rows)
                        {
                            msg += "<li>module " + dr["moduleCode"].ToString() + " of programme " + dr["programmeCode"].ToString() + " of batch " + dr["batchCode"].ToString()
                                + " on " + dr["sessionDate"].ToString() + "/" + dr["sessionPeriod"].ToString() + "</li>";
                        }
                        return new Tuple<bool, string>(false, msg);
                    }
                }
            }

            //check if the venue is available for all new sessions (including those in new module)
            //but if venue is used by any removed sessions (including those in removed module) then not an error
            if (dtNewSession.Rows.Count > 0)
            {
                StringBuilder sbVenueCondition = new StringBuilder("(");
                List<SqlParameter> venueParams = new List<SqlParameter>();
                for (int cnt = 0; cnt < dtNewSession.Rows.Count; cnt++)
                {
                    DataRow dr = dtNewSession.Rows[cnt];
                    sbVenueCondition.Append("(bookingDate=@dt" + cnt + " and bookingPeriod=@pt" + cnt + " and b.venueId=@vid" + cnt + " ) or ");
                    venueParams.Add(new SqlParameter("@dt" + cnt, dr["sessionDate"]));
                    venueParams.Add(new SqlParameter("@pt" + cnt, dr["sessionPeriod"]));
                    venueParams.Add(new SqlParameter("@vid" + cnt, dr["venueId"]));
                }
            
                sbVenueCondition.Remove(sbVenueCondition.Length - 3, 3);
                sbVenueCondition.Append(") " + (dtRemSession.Rows.Count > 0 || dtRemModule.Rows.Count > 0 ? "and sessionId not in ( " : ""));

                for (int cnt = 0; cnt < dtRemSession.Rows.Count; cnt++)
                {
                    sbVenueCondition.Append("@sid" + cnt + ",");
                    venueParams.Add(new SqlParameter("@sid" + cnt, dtRemSession.Rows[cnt]["sessionId"]));
                }
                for (int cnt = 0; cnt < dtRemModule.Rows.Count; cnt++)
                {
                    sbVenueCondition.Append("@sid" + (cnt + dtRemSession.Rows.Count) + ",");
                    venueParams.Add(new SqlParameter("@sid" + (cnt + dtRemSession.Rows.Count), dtRemModule.Rows[cnt]["sessionId"]));
                }

                string venueCondition = ((dtRemSession.Rows.Count > 0 || dtRemModule.Rows.Count > 0)
                    ? sbVenueCondition.ToString().Substring(0, sbVenueCondition.Length - 1) : sbVenueCondition.ToString().Substring(0, sbVenueCondition.Length - 3))
                    + ") and b.defunct='N'";

                DataTable dtBooking = (new DB_Venue()).isVenueAvailable(venueCondition, venueParams.ToArray());
                if (dtBooking.Rows.Count > 0)
                {
                    StringBuilder sbErr = new StringBuilder();
                    foreach (DataRow dr in dtBooking.Rows)
                        sbErr.Append("<li>Venue " + dr["venueLocation"].ToString() + " is not available on " + ((DateTime)dr["bookingDate"]).ToString("dd MMM yyyy") + "/" + dr["bookingPeriodDisp"].ToString()+"</li>");

                    return new Tuple<bool, string>(false, sbErr.ToString());
                }
            }

            //check the end date of the new module (in case user self enter session dates)
            foreach (DataRow dr in dtNewModule.Rows)
            {
                DateTime maxDt = Convert.ToDateTime(dtNewSession.Compute("max([sessionDate])",
                    "moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString()));
                dr["endDate"] = maxDt;
            }

            dtNewSession.Columns.Add(new DataColumn("batchModuleId", typeof(int)));

            DataTable dtModDates = new DataTable();
            dtModDates.Columns.Add(new DataColumn("batchModuleId", typeof(int)));
            dtModDates.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dtModDates.Columns.Add(new DataColumn("endDate", typeof(DateTime)));

            Tuple<DateTime, DateTime> dt;
            DateTime d;
            DataRow drDt = null;
            DB_Batch_Session dbProgBatch = new DB_Batch_Session();

            //removed sessions determine if batch module start date and end date need to change

            //find distinct prog batch id and module id in order to find the batchmoduleid
            DataTable dtBatchMod=getBatchModuleInfo(dtRemSession, "1=1");
            if (dtBatchMod != null && dtBatchMod.Rows.Count > 0)
            {
                foreach (DataRow dr in dtBatchMod.Rows)
                {
                    DataRow[] sdt = dtRemSession.Select("moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString());

                    sb = new StringBuilder();
                    p = new SqlParameter[sdt.Length];
                    i = 0;
                    foreach (DataRow s in sdt)
                    {
                        sb.Append(" and sessionDate <> @dt" + i);
                        p[i] = new SqlParameter("@dt" + i, s["sessionDate"]);
                        i++;
                    }

                    dt = dbProgBatch.getMaxMinSessionDates((int)dr["batchModuleId"], sb.ToString(), p);

                    drDt = dtModDates.NewRow();
                    drDt["batchModuleId"] = dr["batchModuleId"];
                    drDt["endDate"] = dt.Item1;
                    drDt["startDate"] = dt.Item2;
                    dtModDates.Rows.Add(drDt);
                }
            }

            //for new sessions determine if batch module start date and end date need to change

            //find distinct prog batch id and module id in order to find the batchmoduleid
            dtBatchMod = getBatchModuleInfo(dtNewSession, "not isNewModule");
            if (dtBatchMod != null && dtBatchMod.Rows.Count > 0)
            {
                foreach (DataRow dr in dtBatchMod.Rows)
                {
                    dt = dbProgBatch.getMaxMinSessionDates((int)dr["batchModuleId"]);

                    if (dt == null) return new Tuple<bool, string>(false, "Error retrieving session information.");

                    d = (DateTime)dtNewSession.Compute("max(sessionDate)", "moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString());
                    if (dt.Item1.CompareTo(d) < 0)
                    {
                        drDt = dtModDates.NewRow();
                        drDt["batchModuleId"] = dr["batchModuleId"];
                        drDt["endDate"] = d;
                    }
                    d = (DateTime)dtNewSession.Compute("min(sessionDate)", "moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString());
                    if (dt.Item2.CompareTo(d) > 0)
                    {
                        if (drDt == null)
                        {
                            drDt = dtModDates.NewRow();
                            drDt["batchModuleId"] = dr["batchModuleId"];
                            drDt["endDate"] = dt.Item1;
                        }
                        drDt["startDate"] = d;
                    }
                    if (drDt != null)
                    {
                        if (drDt["startDate"] == DBNull.Value) drDt["startDate"] = dt.Item2;
                        dtModDates.Rows.Add(drDt);
                        drDt = null;
                    }

                    foreach (DataRow s in dtNewSession.Select("moduleId=" + dr["moduleId"].ToString() + " and programmeBatchId=" + dr["programmeBatchId"].ToString() + ""))
                    {
                        s["batchModuleId"] = dr["batchModuleId"];
                    }
                }
            }

            if (dbBundle.updateBundle(bundleId, bundleType, bundleEffDt, bundleCost, userId, dtNewSession, dtRemSession, dtNewModule, dtRemModule, dtModDates, dtModOrder))
            {
                return new Tuple<bool, string>(true, "Bundle saved successfully.");
            }
            else
            {
                return new Tuple<bool, string>(false, "Error updating bundle.");
            }
            
        }

        //public Tuple<int, string> updateBundle(string bundleCode, string bundleType, DateTime bundleEffDt, decimal bundleCost, DataTable dtModules, int userId)
        public Tuple<int, string> updateBundle(int bundleId, string bundleType, DateTime bundleEffDt, decimal bundleCost, DataTable dtModules, int userId)
        {
            //check if bundle is in used by any of the programmes
            bool isProgrammeExist = dbBundle.checkProgrammeExist(bundleId);
            //check if bundle is in used by any of the batches
            bool isSessionExist = dbBundle.checkSessionExist(bundleId);

            //if yes check if the no of modules is more than SOA
            if (isSessionExist || isProgrammeExist)
            {
                int numSOA = dbBundle.getMaxProgrammesSOA(bundleId);
                if (numSOA == -1) return new Tuple<int, string>(-1, "Error validating bundle.");

                if (numSOA > dtModules.Rows.Count) return new Tuple<int, string>(-1, "Number of modules exceeded one or more programme's SOA.");
            }
            
            if (!isSessionExist)
            {
                //sessions for any of the programme using the bundle not generated, just update db straight away
                if (dbBundle.updateBundle(bundleId, bundleType, bundleEffDt, bundleCost, dtModules, userId))
                    return new Tuple<int, string>(0, "Bundle saved sucessfully.");
                else
                    return new Tuple<int, string>(-1, "Error saving bundle.");
            }

            //get the min batch start date of all the batches using the programmes which uses the bundle
            DateTime dtBatchMinStart = (new DB_Batch_Session()).getMinBatchStartDate(bundleId);
            if (dtBatchMinStart == DateTime.MinValue) return new Tuple<int, string>(-1, "Error retrieving classes start date for validating bundle effective date.");
            //if min start date is ealier than bundle eff date, error
            if (dtBatchMinStart.CompareTo(bundleEffDt) < 0) return new Tuple<int, string>(-1, "Bundle effective date is later than class commencement date (" + dtBatchMinStart.ToString("dd MMM yyyy") + ").");


            DataTable dtOriModules = dbBundle.getBundleModules(bundleId);
            if (dtOriModules == null) return new Tuple<int, string>(-1, "Error saving bundle.");

            //check if there is any new modules, if yes ask to redirect to another page
            foreach (DataRow dr in dtModules.Rows)
            {
                DataRow[] drs = dtOriModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (drs == null || drs.Length == 0) return new Tuple<int, string>(1, "Modules added");
               
            }

            //check if there any any removed modules, if yes ask to redirect to another page
            foreach (DataRow dr in dtOriModules.Rows)
            {
                DataRow[] drs = dtModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (drs == null || drs.Length == 0) return new Tuple<int, string>(1, "Modules removed");
            }

            //check if there is any change in module sessions, if yes ask to redirect to another page
            foreach (DataRow dr in dtOriModules.Rows)
            {
                DataRow[] drs = dtModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (((int)dr["ModuleNumOfSession"]) != ((int)drs[0]["ModuleNumOfSession"]))
                    return new Tuple<int, string>(1, "Number of sessions changed");
            }
            
            //nothing has changed, update bundle straight away
            if (dbBundle.updateBundle(bundleId, bundleType, bundleEffDt, bundleCost, dtModules, userId))
                return new Tuple<int, string>(0, "Bundle saved sucessfully.");
            else
                return new Tuple<int, string>(-1, "Error saving bundle.");
        }

        //public DataTable getUpdateBundleAffectedProgrammes(string bundleCode)
        public DataTable getUpdateBundleAffectedProgrammes(int bundleId)
        {
            return dbBundle.getBundleProgrammes(bundleId);
        }

        //public DataTable getUpdateBundleAffectedBatches(string bundleCode)
        public DataTable getUpdateBundleAffectedBatches(int bundleId)
        {
            return dbBundle.getBundleProgrammeBatches(bundleId);
        }

        //public DataTable getUpdateBundleAffectedModules(string bundleCode, DataTable selModules)
        public DataTable getUpdateBundleAffectedModules(int bundleId, DataTable selModules)
        {
            DataTable dtAffModules = new DataTable();
            dtAffModules.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtAffModules.Columns.Add(new DataColumn("moduleCode", typeof(string)));
            dtAffModules.Columns.Add(new DataColumn("moduleTitle", typeof(string)));
            dtAffModules.Columns.Add(new DataColumn("chgType", typeof(string)));
            dtAffModules.Columns.Add(new DataColumn("sessionDiff", typeof(int)));

            DataTable dtOriModules = dbBundle.getBundleModules(bundleId);

            //check if there is any new modules
            foreach (DataRow dr in selModules.Rows)
            {
                DataRow[] drs = dtOriModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (drs == null || drs.Length == 0)
                {
                    DataRow drNew = dtAffModules.NewRow();
                    drNew["moduleId"] = dr["moduleId"];
                    drNew["moduleCode"] = dr["moduleCode"];
                    drNew["moduleTitle"] = dr["moduleTitle"];
                    drNew["chgType"] = "NEW";
                    drNew["sessionDiff"] = dr["ModuleNumOfSession"];
                    dtAffModules.Rows.Add(drNew);
                }
            }

            //check if there any any removed modules
            foreach (DataRow dr in dtOriModules.Rows)
            {
                DataRow[] drs = selModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (drs == null || drs.Length == 0)
                {
                    DataRow drNew = dtAffModules.NewRow();
                    drNew["moduleId"] = dr["moduleId"];
                    drNew["moduleCode"] = dr["moduleCode"];
                    drNew["moduleTitle"] = dr["moduleTitle"];
                    drNew["chgType"] = "REM";
                    drNew["sessionDiff"] = 0;
                    dtAffModules.Rows.Add(drNew);
                }
            }

            //check if there is any change in module sessions
            foreach (DataRow dr in dtOriModules.Rows)
            {
                DataRow[] drs = selModules.Select("moduleId=" + dr["moduleId"].ToString());
                if (drs == null || drs.Length == 0) continue;

                if (((int)dr["ModuleNumOfSession"]) != ((int)drs[0]["ModuleNumOfSession"]))
                {
                    DataRow drNew = dtAffModules.NewRow();
                    drNew["moduleId"] = dr["moduleId"];
                    drNew["moduleCode"] = dr["moduleCode"];
                    drNew["moduleTitle"] = dr["moduleTitle"];
                    drNew["chgType"] = ((int)dr["ModuleNumOfSession"]) > ((int)drs[0]["ModuleNumOfSession"]) ? "DEC" : "INC";
                    drNew["sessionDiff"] = Math.Abs(((int)dr["ModuleNumOfSession"]) - ((int)drs[0]["ModuleNumOfSession"]));
                    dtAffModules.Rows.Add(drNew);
                }
            }

            return dtAffModules;
        }

        //Get bundle by bundleCode
        //public DataTable getBundleModule(string bundleCode, bool includeDefunct = false)
        public DataTable getBundleModule(int bundleId, bool includeDefunct = false)
        {
            DataTable dtBundle = dbBundle.getBundleModules(bundleId, includeDefunct);

            return dtBundle;
        }

        public DataTable getRecentModBundle()
        {
            return dbBundle.getRecentModBundle();
        }

        public DataTable getListBundle(string frm, string to)
        {
            bool filterNum = false;
            if (to.Contains("#"))
            {
                filterNum = true;
                to.Replace("#", "");
            }
            return dbBundle.getListBundle(frm, to, filterNum);
        }
    }
}
