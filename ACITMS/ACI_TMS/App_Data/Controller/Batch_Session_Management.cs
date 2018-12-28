using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLayer;
using GeneralLayer;
using System.Data;
using System.Globalization;
using System.Data.SqlClient;

namespace LogicLayer
{
    [Serializable]
    public class BatchDetails
    {
        public string progCatCode;
        public string progLvlCode;
        public string progTitle;
        public int progId;
        public string progCode;
        public string progType;
        public int bundleId;
        public string bundleCode;
        public string bundleEffDate;
        public decimal bundleCost;
        public string batchCode;
        public string clsTypeCode;
        public string projCode;
        public string regStartDate;
        public string regEndDate;
        public DateTime batchStartDate;
        public DateTime batchEndDate;
        public int batchCapacity;
        public string batchModeCode;
    }

    public enum BatchDateType
    {
        REGISTRATION,
        COMMENCEMENT
    }

    public class Batch_Session_Management
    {
        private DB_Batch_Session dbBatchSess = new DB_Batch_Session();

        public string getEnrollmentLetter(int programmeBatchId)
        {
            return dbBatchSess.getEnrollmentLetter(programmeBatchId);
        }

        public Tuple<bool, string> updateEnrollmentLetter(int programmeBatchId, string letter)
        {
            if (dbBatchSess.updateEnrollmentLetter(programmeBatchId, letter))
                return new Tuple<bool, string>(true, "Letter saved successfully.");
            else return new Tuple<bool, string>(false, "Error saving letter.");
        }

        public int getNoOfAvaBatch()
        {
            return dbBatchSess.getNoOfAvaBatch();
        }

        public DataTable getBatchModuleByProgrammeBatchId(string programmeBatchId)
        {
            DataTable dt = dbBatchSess.getBatchModuleByProgrammeBatchId(programmeBatchId);

            return dt;
        }

        public DataTable getBatchModuleInfo(int batchModuleId)
        {
            return dbBatchSess.getBatchModuleInfo(batchModuleId);
        }

        public DataTable getBatchesByModule(int moduleId)
        {
            return dbBatchSess.getBatchesByModule(moduleId);
        }

        public bool checkBatchCode(string code)
        {
            return dbBatchSess.isBatchCodeExist(code);
        }

        public DataTable getClassMode()
        {
            return dbBatchSess.getClsMode();
        }

        public DataTable getClassType()
        {
            return dbBatchSess.getClsType();
        }

        public DataTable filterBatches(string progType, string progCat, string status)
        {
            string condition = "p.programmeType=@pType and p.programmeCategory = @pCat and ";

            if (status == "AVA") condition += "GETDATE() between programmeRegStartDate and programmeRegEndDate";
            else if (status == "OS") condition += "programmeRegStartDate > getdate()";
            else if (status == "CL") condition += "programmeRegEndDate < getdate()";


            List<SqlParameter> parameters = new List<SqlParameter>();
            // Loop start - for / while / foreach 
            SqlParameter param = new SqlParameter("@pType", progType);

            parameters.Add(param);

            param = new SqlParameter("@pCat", progCat);
            parameters.Add(param);
            // Loop end

            return dbBatchSess.searchBatches(condition, parameters.ToArray());
        }

        public DataTable searchBatches(string criteria, string value)
        {
            if (criteria != null && criteria != "")
            {
                string condition = "";
                if (criteria == "BTC") condition = "UPPER(b.batchCode) like @sv";
                else if (criteria == "PJC") condition = "UPPER(b.projectCode) like @sv";
                else if (criteria == "PGC") condition = "UPPER(p.programmeCode) like @sv";
                else if (criteria == "BDC") condition = "UPPER(p.programmeCode) like @sv";
                else if (criteria == "AVA") condition = "GETDATE() between programmeRegStartDate and programmeRegEndDate";

                List<SqlParameter> parameters = new List<SqlParameter>();
                // Loop start - for / while / foreach 
                if (criteria != "AVA")
                {
                    SqlParameter param = new SqlParameter("@sv", "%" + value.ToUpper() + "%");

                    parameters.Add(param);
                }

                return dbBatchSess.searchBatches(condition, value == null ? null : parameters.ToArray());
            }
            else return dbBatchSess.searchBatches(null, null);
        }

        public DataTable searchSessions(string progCatCode, string progLvlCode, string batchCode, string progTitle, int moduleId, string progCode, string pjCode)
        {
            string condition = "";
            List<SqlParameter> p = new List<SqlParameter>();

            if (progCatCode != null && progCatCode != "")
            {
                condition += "p.programmeCategory=@pcat and ";
                p.Add(new SqlParameter("@pcat", progCatCode));
            }

            if (progLvlCode != null && progLvlCode != "")
            {
                condition += "p.programmeLevel=@plvl and ";
                p.Add(new SqlParameter("@plvl", progLvlCode));
            }

            if (batchCode != null && batchCode != "")
            {
                condition += "UPPER(b.batchCode) LIKE @bc and ";
                p.Add(new SqlParameter("@bc", "%" + batchCode.ToUpper() + "%"));
            }

            if (progTitle != null && progTitle != "")
            {
                condition += "UPPER(p.programmeTitle) LIKE @pt and ";
                p.Add(new SqlParameter("@pt", "%" + progTitle.ToUpper() + "%"));
            }

            if (progCode != null && progCode != "")
            {
                condition += "UPPER(p.programmeCode) LIKE @pc and ";
                p.Add(new SqlParameter("@pc", "%" + progCode.ToUpper() + "%"));
            }

            if (pjCode != null && pjCode != "")
            {
                condition += "UPPER(b.projectCode) LIKE @pjc and ";
                p.Add(new SqlParameter("@pjc", "%" + pjCode.ToUpper() + "%"));
            }

            if (moduleId > 0)
            {
                condition += "mm.moduleId=@mid and ";
                p.Add(new SqlParameter("@mid", moduleId));
            }

            condition = condition.Substring(0, condition.Length - 4);

            return dbBatchSess.searchSessions(condition, p.ToArray());
        }

        //sessions are modified directly in the dtModSessions
        public void postponeSessions(string sessionInfo, DateTime dtStart, string prevModPeriod, DateTime dtEnd, DataTable dtModSessions, int startSessionIndex)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("day", typeof(int)));
            dt.Columns.Add(new DataColumn("period", typeof(string)));

            List<DataRow> drSessionInfo = new List<DataRow>();
            DataRow dr;
            foreach (string d in sessionInfo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] t = d.Split(new char[] { '/' });

                dr = dt.NewRow();
                dr["day"] = int.Parse(t[0]);
                dr["period"] = t[1];
                drSessionInfo.Add(dr);
            }

            Tuple<int, bool> tmp = findStartSessionDay(drSessionInfo.ToArray(), dtStart, prevModPeriod);
            int dIndex = tmp.Item1;
            if (tmp.Item2) dtStart = dtStart.AddDays(1);

            DateTime dtCurr = dtStart;
            StringBuilder venueCondition = new StringBuilder();
            SqlParameter[] p = new SqlParameter[((dtModSessions.Rows.Count - startSessionIndex) * 3) + 1];
            int sIndex = startSessionIndex;

            for (int i = startSessionIndex; i < dtModSessions.Rows.Count; i++)
            {
                int daysToAdd = ((int)drSessionInfo[dIndex]["day"] - ((int)dtCurr.DayOfWeek == 0 ? 7 : (int)dtCurr.DayOfWeek) + 7) % 7;
                dtCurr = dtCurr.AddDays(daysToAdd);

                dr = dtModSessions.Rows[sIndex];
                dr["sessionDate"] = dtCurr;
                dr["sessionDateDisp"] = dtCurr.ToString("dd MMM yyyy");
                dr["sessionPeriod"] = drSessionInfo[dIndex]["period"];

                //check venue available using consolidated sql to save database io
                venueCondition.Append("(bookingDate=@dt" + i + " and bookingPeriod=@pt" + i + " and b.venueId=@vid" + i + " and b.defunct='N') or ");
                p[(i - startSessionIndex) * 3] = new SqlParameter("@dt" + i, dtCurr);
                p[((i - startSessionIndex) * 3) + 1] = new SqlParameter("@pt" + i, dr["sessionPeriod"].ToString());
                p[((i - startSessionIndex) * 3) + 2] = new SqlParameter("@vid" + i, dr["venueId"].ToString());
                //default set it to available first
                dr["isVenueAva"] = true;

                //check if session date exceeded given end date if yes, flag error
                dr["isDtExceed"] = (dtEnd < dtCurr ? true : false);

                //move the index forward
                if (dIndex == drSessionInfo.Count - 1)
                {
                    dIndex = 0;
                    //move the date forward so it will result in non zero daysToAdd 
                    //(E.g day selected is only monday, without this statment, the session date generated will always be on the same date as daysToAdd will be calculated as zero)
                    dtCurr = dtCurr.AddDays(1);
                }
                else dIndex++;

                sIndex++;
            }

            p[p.Length - 1] = new SqlParameter("@bmid", (int)dtModSessions.Rows[0]["batchModuleId"]);

            //check venue available
            DataTable dtBookings = (new DB_Venue()).isVenueAvailable(@"b.defunct='N' and (" + venueCondition.ToString().Substring(0, venueCondition.Length - 3) + @") and 
                b.sessionId not in (select sessionId from batchModule_session where batchModuleId=@bmid and defunct='N')", p);
            foreach (DataRow drBook in dtBookings.Rows)
            {
                foreach (DataRow drTmp in dtModSessions.Select("sessionDate=#" + ((DateTime)drBook["bookingDate"]).ToString("MM/dd/yyyy")
                    + "# and sessionPeriod='" + drBook["bookingPeriod"].ToString() + "' and venueId='" + drBook["venueId"].ToString() + "'"))
                {
                    drTmp["isVenueAva"] = false;
                }
            }
        }

        private Tuple<int, bool> findStartSessionDay(DataRow[] drSessionInfo, DateTime dtStart, string prevModPeriod)
        {
            int dIndex = -1;

            //find the day that the 1st session should start at
            DataRow dr;
            bool addDay = false, isDayFound = false;
            for (int i = 0; i < drSessionInfo.Length; i++)
            {
                dr = drSessionInfo[i];

                //if day of the start date is equal to the current row day or smaller, then the current row day should be the day the 1st session start
                if ((int)dr["day"] == ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek) || (int)dr["day"] > ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek))
                {
                    isDayFound = true;

                    if (prevModPeriod == null)
                    {
                        //no prev module period, either scheduling don go by all modules same session or is the first session of the schedule (where all modules same session)
                        dIndex = i;
                        break;
                    }
                    else if (prevModPeriod != dr["period"].ToString())
                    {
                        //even if the the date condition matches, must see if it matches the period, if not continue to loop
                        continue;
                    }
                    else
                    {
                        //if period also matches, then the scheduling should start from the next record
                        if (i + 1 >= drSessionInfo.Length)
                        {
                            dIndex = 0;
                            //since the first session is reset to the 1st record, start date has to increment
                            addDay = true;
                        }
                        else
                        {
                            dIndex = i + 1;
                            //if the day of the current row is not the same as the next row where the first session will start, increment start date
                            if ((int)dr["day"] != (int)drSessionInfo[i + 1]["day"]) addDay = true;
                        }

                        break;
                    }

                }
            }
            //if not found, means the day of the start date is later than any of the day in the datatable, so the nearest should be the day in the first record
            if (dIndex == -1)
            {
                if (isDayFound)
                {
                    //when reach here means can match the day but cannot match the period, probably called to postpone so given period is defined by the user rather than the system auto schedule from the day session data give
                    //so just fix the day
                    for (int i = 0; i < drSessionInfo.Length; i++)
                    {
                        dr = drSessionInfo[i];
                        if ((int)dr["day"] == ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek))
                        {
                            //if on the same day, the given period is later than the first period of the day then set the 1st session to next record
                            if (dr["period"].ToString() == DayPeriod.EVE.ToString() || (dr["period"].ToString() == DayPeriod.AM.ToString() && (prevModPeriod == DayPeriod.PM.ToString() || prevModPeriod == DayPeriod.EVE.ToString())) ||
                                (dr["period"].ToString() == DayPeriod.PM.ToString() && prevModPeriod == DayPeriod.EVE.ToString()))
                            {
                                if (i + 1 >= drSessionInfo.Length)
                                {
                                    //if all the day session data belong to a single day then need to increament the date
                                    if ((int)dr["day"] == (int)drSessionInfo[0]["day"]) addDay = true;
                                    dIndex = 0;
                                }
                                else
                                {

                                    if ((int)dr["day"] == (int)drSessionInfo[i + 1]["day"])
                                    {
                                        //if next record is the same day, if yes, need to compare the period again
                                        continue;
                                    }
                                    else dIndex = i + 1;
                                }
                            }
                            else dIndex = i;
                            break;
                        }
                        else if ((int)dr["day"] > ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek))
                        {
                            dIndex = i;
                            break;
                        }
                    }
                }
                else dIndex = 0;
            }

            return new Tuple<int, bool>(dIndex, addDay);
        }

        public DataTable genSessions(DataRow[] drSessionInfo, DateTime dtStart, string prevModPeriod, DateTime dtEnd, int noOfSessions)
        {
            //IMPT: drSessionInfo must be sorted by day and period in ascending order!

            DataTable dtSessions = new DataTable();
            dtSessions.Columns.Add(new DataColumn("sessionNo", typeof(int)));
            dtSessions.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtSessions.Columns.Add(new DataColumn("sessionDateDisp", typeof(string)));
            dtSessions.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));
            dtSessions.Columns.Add(new DataColumn("venueId", typeof(string)));
            dtSessions.Columns.Add(new DataColumn("venueLocation", typeof(string)));
            dtSessions.Columns.Add(new DataColumn("isVenueAva", typeof(bool)));
            dtSessions.Columns.Add(new DataColumn("isDtExceed", typeof(bool)));

            Tuple<int, bool> tmp = findStartSessionDay(drSessionInfo, dtStart, prevModPeriod);
            int dIndex = tmp.Item1;
            if (tmp.Item2) dtStart = dtStart.AddDays(1);

            DataRow dr;
            DateTime dtCurr = dtStart;
            StringBuilder venueCondition = new StringBuilder();
            SqlParameter[] p = new SqlParameter[noOfSessions * 3];
            for (int i = 0; i < noOfSessions; i++)
            {
                int daysToAdd = ((int)drSessionInfo[dIndex]["day"] - ((int)dtCurr.DayOfWeek == 0 ? 7 : (int)dtCurr.DayOfWeek) + 7) % 7;
                dtCurr = dtCurr.AddDays(daysToAdd);

                dr = dtSessions.NewRow();
                dr["sessionNo"] = i + 1;
                dr["sessionDate"] = dtCurr;
                dr["sessionDateDisp"] = dtCurr.ToString("dd MMM yyyy");
                dr["sessionPeriod"] = drSessionInfo[dIndex]["period"];
                dr["venueId"] = drSessionInfo[dIndex]["venueId"];
                dr["venueLocation"] = drSessionInfo[dIndex]["venueLocation"];

                //check venue available using consolidated sql to save database io
                venueCondition.Append("(bookingDate=@dt" + i + " and bookingPeriod=@pt" + i + " and b.venueId=@vid" + i + " and b.defunct='N') or ");
                p[i * 3] = new SqlParameter("@dt" + i, dtCurr);
                p[(i * 3) + 1] = new SqlParameter("@pt" + i, dr["sessionPeriod"].ToString());
                p[(i * 3) + 2] = new SqlParameter("@vid" + i, dr["venueId"].ToString());
                //default set it to available first
                dr["isVenueAva"] = true;

                //check if session date exceeded given end date if yes, flag error
                dr["isDtExceed"] = (dtEnd < dtCurr ? true : false);

                dtSessions.Rows.Add(dr);

                //move the index forward
                if (dIndex == drSessionInfo.Length - 1)
                {
                    dIndex = 0;
                    //move the date forward so it will result in non zero daysToAdd 
                    //(E.g day selected is only monday, without this statment, the session date generated will always be on the same date as daysToAdd will be calculated as zero)
                    dtCurr = dtCurr.AddDays(1);
                }
                else dIndex++;
            }
            //check venue available
            DataTable dtBookings = (new DB_Venue()).isVenueAvailable("b.defunct='N' and (" + venueCondition.ToString().Substring(0, venueCondition.Length - 3) + ")", p);
            foreach (DataRow drBook in dtBookings.Rows)
            {
                foreach (DataRow drTmp in dtSessions.Select("sessionDate=#" + ((DateTime)drBook["bookingDate"]).ToString("MM/dd/yyyy")
                    + "# and sessionPeriod='" + drBook["bookingPeriod"].ToString() + "' and venueId='" + drBook["venueId"].ToString() + "'"))
                {
                    drTmp["isVenueAva"] = false;
                }
            }

            return dtSessions;
        }

        public string[,] genSessions(string moduleDay, DateTime dtStart, DayPeriod startPeriod, int noOfSessions)
        {
            int dIndex = -1;

            //find the default day according to the info
            string[] tmp = moduleDay.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            string[,] days = new string[tmp.Length, 2];
            for (int i = 0; i < tmp.Length; i++)
            {
                string[] t = tmp[i].Split(new char[] { '/' });
                days[i, 0] = t[0];
                days[i, 1] = t[1];
            }

            //if there is only 1 default day/period selected, the next session should be on the same day/period
            if (days.GetLength(0) == 1) dIndex = 0;
            else
            {
                //find the next session
                for (int i = 0; i < days.GetLength(0); i++)
                {
                    //DayOfWeek is zero based start from sunday
                    if (int.Parse(days[i, 0]) == ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek) && days[i, 1] == startPeriod.ToString())
                    {
                        if (i == days.GetLength(0) - 1) dIndex = 0;
                        else dIndex = i + 1;

                        break;
                    }
                }
                //if the last session do not match any of the default days find the nearest one
                if (dIndex == -1)
                {
                    //1/AM;1/NIGHT;3/AM
                    //1/PM

                    //1/AM;1/PM;3/AM
                    //2/AM
                    for (int i = 0; i < days.GetLength(0); i++)
                    {
                        //DayOfWeek is zero based start from sunday
                        if (int.Parse(days[i, 0]) == ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek))
                        {
                            //if current period is later than last session period
                            if (days[i, 1] == DayPeriod.PM.ToString() && startPeriod == DayPeriod.AM)
                            {
                                dIndex = i;
                                break;
                            }
                            else if (days[i, 1] == DayPeriod.EVE.ToString() && (startPeriod == DayPeriod.AM || startPeriod == DayPeriod.PM))
                            {
                                dIndex = i;
                                break;
                            }
                        }
                        else if (int.Parse(days[i, 0]) < ((int)dtStart.DayOfWeek == 0 ? 7 : (int)dtStart.DayOfWeek))
                        {
                            dIndex = i;
                            break;
                        }
                    }

                    //if dIndex is still -1, means that the day of the start date is smaller then the selected days. e.g. day of start date is 1 (mon), selected day is 3 (wed) and 4 (thu)
                    //then set it to the first selected day
                    if (dIndex == -1) dIndex = 0;
                }
            }

            //generate the new session dates
            DateTime dtCurr = dtStart;
            string[,] sessionDates = new string[noOfSessions, 2];
            for (int i = 0; i < noOfSessions; i++)
            {
                int daysToAdd = (int.Parse(days[dIndex, 0]) - ((int)dtCurr.DayOfWeek == 0 ? 7 : (int)dtCurr.DayOfWeek) + 7) % 7;
                dtCurr = dtCurr.AddDays(daysToAdd);

                sessionDates[i, 0] = dtCurr.ToString("dd MMM yyyy");
                sessionDates[i, 1] = days[dIndex, 1];

                //move the index forward
                if (dIndex == days.GetLength(0) - 1)
                {
                    dIndex = 0;
                    //move the date forward so it will result in non zero daysToAdd 
                    //(E.g day selected is only monday, without this statment, the session date generated will always be on the same date as daysToAdd will be calculated as zero)
                    dtCurr = dtCurr.AddDays(1);
                }
                else dIndex++;
            }

            return sessionDates;
        }

        public DataTable getBatchModuleSessions(int programmeBatchId, int moduleId)
        {
            return dbBatchSess.getBatchModuleSessions(programmeBatchId, moduleId);
        }

        public DataTable getBatchModuleDates(int programmeBatchId, int moduleId)
        {
            string condition = " programmeBatchId=@pid and moduleId=@mid ";
            SqlParameter[] p = { new SqlParameter("@pid", programmeBatchId), new SqlParameter("@mid", moduleId) };

            return dbBatchSess.getBatchModuleInfo(condition, p);
        }

        public Tuple<bool, string> createBatchNSession(BatchDetails batch, DataTable dtMod, DataTable dtSession, int userId)
        {
            //check if batch code exist
            if (dbBatchSess.isBatchCodeExist(batch.batchCode))
                return new Tuple<bool, string>(false, "Duplicated batch code.");

            //check if the venue available
            StringBuilder sbVenueCondition = new StringBuilder();
            SqlParameter[] venueParams = new SqlParameter[dtSession.Rows.Count * 3];
            int cnt = 0;
            foreach (DataRow dr in dtSession.Rows)
            {
                sbVenueCondition.Append("(bookingDate=@dt" + cnt + " and bookingPeriod=@pt" + cnt + " and b.venueId=@vid" + cnt + " ) or ");
                venueParams[cnt * 3] = new SqlParameter("@dt" + cnt, dr["sessionDate"]);
                venueParams[cnt * 3 + 1] = new SqlParameter("@pt" + cnt, dr["sessionPeriod"]);
                venueParams[cnt * 3 + 2] = new SqlParameter("@vid" + cnt, dr["venueId"]);

                cnt++;
            }
            DataTable dtBooking = (new DB_Venue()).isVenueAvailable("b.defunct='N' and (" + sbVenueCondition.ToString().Substring(0, sbVenueCondition.Length - 3) + ")", venueParams);
            if (dtBooking == null) return new Tuple<bool, string>(false, "Error checking venue availability.");
            if (dtBooking.Rows.Count > 0)
            {
                string err = "";
                foreach (DataRow dr in dtBooking.Rows)
                    err += "<li>Venue " + dr["venueLocation"].ToString() + " is not available on " + ((DateTime)dr["bookingDate"]).ToString("dd MMM yyyy") + "/" + dr["bookingPeriodDisp"].ToString() + "</li>";

                return new Tuple<bool, string>(false, err);
            }

            //update the batch dates based on session dates (if needed)
            DateTime dt;
            foreach (DataRow dr in dtMod.Rows)
            {
                dt = (DateTime)dtSession.Compute("min(sessionDate)", "moduleId=" + dr["moduleId"].ToString());
                if (dt.CompareTo((DateTime)dr["startDate"]) < 0) dr["startDate"] = dt;

                dt = (DateTime)dtSession.Compute("max(sessionDate)", "moduleId=" + dr["moduleId"].ToString());
                if (dt.CompareTo((DateTime)dr["endDate"]) > 0) dr["endDate"] = dt;
            }


            if (dbBatchSess.createBatchNSession(batch, dtMod, dtSession, userId)) return new Tuple<bool, string>(true, "Batch and session saved successfully.");
            else return new Tuple<bool, string>(false, "Error saving batch and session");
        }

        public DataTable getBatchDetails(int batchId)
        {
            return dbBatchSess.getBatchDetails(batchId);
        }

        public DataTable getSessionDetails(int sessionId)
        {
            DataTable dt = dbBatchSess.getSessionDetails(sessionId);
            if (dt == null || dt.Rows.Count == 0) return null;

            //ACI_Staff_User u = new ACI_Staff_User();
            //dt.Rows[0]["trainerUserName1"] = u.decryptionString(dt.Rows[0]["trainerUserName1"].ToString());
            //dt.Rows[0]["assessorUserName"] = u.decryptionString(dt.Rows[0]["assessorUserName"].ToString());
            //if (dt.Rows[0]["trainerUserName2"] != DBNull.Value) dt.Rows[0]["trainerUserName2"] = u.decryptionString(dt.Rows[0]["trainerUserName2"].ToString());

            return dt;
        }

        public Tuple<bool, string> updateSession(int sessionId, DateTime dt, DayPeriod period, string venueId, int capacity, int userId)
        {
            //check venue available
            if (!(new DB_Venue()).isVenueAvailable(dt, period, venueId, capacity))
                return new Tuple<bool, string>(false, "Selected venue is not available");

            //check if the session is the makeup for any existing absentee, if so then the new session date (if changed) cannot be earlier than absent session
            Tuple<DateTime, string> minMakeup = (new DB_Attendance()).getMinAbsenceSessionDate(sessionId);
            if (minMakeup == null) return new Tuple<bool, string>(false, "Error validating session date.");

            if (minMakeup.Item1 != DateTime.MinValue)
            {
                bool exceedDt = false;
                if (dt.CompareTo(minMakeup.Item1) < 0) exceedDt = true;
                else if (dt.CompareTo(minMakeup.Item1) == 0)
                {
                    if (minMakeup.Item2 == DayPeriod.PM.ToString() && period == DayPeriod.AM) exceedDt = true;
                    if (minMakeup.Item2 == DayPeriod.EVE.ToString() && (period == DayPeriod.AM || period == DayPeriod.PM)) exceedDt = true;
                }
                if (exceedDt)
                {
                    string p = null;
                    if (minMakeup.Item2 == DayPeriod.AM.ToString()) p = DayPeriod.AM.ToString();
                    else if (minMakeup.Item2 == DayPeriod.PM.ToString()) p = DayPeriod.PM.ToString();
                    else p = "Evening";
                    return new Tuple<bool, string>(false, "New session date/period cannot be earlier than " + minMakeup.Item1.ToString("dd MMM yyyy") + " "
                        + p + " because make-up session(s) has been arranged. ");
                }
            }

            if (dbBatchSess.updateSession(sessionId, dt, period, venueId, userId))
                return new Tuple<bool, string>(true, "Session saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error saving session.");
        }

        public Tuple<DataTable, DataTable> getBatchModulesNSessions(int batchId)
        {
            DataTable dtMod = dbBatchSess.getBatchAllModulesInfo(batchId);

            //populate the day description
            dtMod.Columns.Add(new DataColumn("dayDisp", typeof(string)));
            //ACI_Staff_User su = new ACI_Staff_User();
            DataTable dtPeriods = (new DB_Venue()).getPeriods();
            foreach (DataRow dr in dtMod.Rows)
            {
                dr["dayDisp"] = formatDayStr(dr["day"].ToString(), dtPeriods);
                //also decode the trainer
                //dr["trainerUserName1"] = su.decryptionString(dr["trainerUserName1"].ToString());
                //if (dr["trainerUserName2"] != DBNull.Value) dr["trainerUserName2"] = su.decryptionString(dr["trainerUserName2"].ToString());
                //dr["assessorUserName"] = su.decryptionString(dr["assessorUserName"].ToString());
            }

            DataTable dtSessions = dbBatchSess.getBatchModuleSessions(batchId);

            //populate the session number for each module
            dtSessions.Columns.Add(new DataColumn("sessionNo", typeof(int)));

            int no = 1;
            int prevMod = (int)dtSessions.Rows[0]["moduleId"];
            foreach (DataRow dr in dtSessions.Rows)
            {
                if (prevMod != (int)dr["moduleId"])
                {
                    prevMod = (int)dr["moduleId"];
                    no = 1;
                }

                dr["sessionNo"] = no;
                no++;
            }

            return new Tuple<DataTable, DataTable>(dtMod, dtSessions);
        }

        public bool isBatchStarted(int batchId, BatchDateType beforeDate)
        {
            DataTable dt = dbBatchSess.getBatchDetails(batchId);
            if (dt == null || dt.Rows.Count == 0) return false;

            if ((beforeDate == BatchDateType.REGISTRATION && ((DateTime)dt.Rows[0]["programmeRegStartDate"]).CompareTo(DateTime.Now) >= 0)
                || (beforeDate == BatchDateType.COMMENCEMENT && ((DateTime)dt.Rows[0]["programmeStartDate"]).CompareTo(DateTime.Now) >= 0))
                return false;
            else
                return true;
        }

        public Tuple<bool, string> deleteBatch(int batchId, int userId)
        {
            if (isBatchStarted(batchId, BatchDateType.REGISTRATION))
                return new Tuple<bool, string>(false, "Batch has already started, unable to delete.");

            if (dbBatchSess.deleteBatch(batchId, userId))
                return new Tuple<bool, string>(true, "Batch deleted successfully.");
            else
                return new Tuple<bool, string>(false, "Error deleting batch.");
        }

        public Tuple<bool, string> updateBatch(int batchId, string batchCode, string projCode, DateTime dtRegStart, DateTime dtRegEnd, DateTime dtBatchStart, DateTime dtBatchEnd,
            int capacity, string mode, DataTable dtMod, DataTable dtSession, int userId)
        {
            //check if the venue available
            StringBuilder sbVenueCondition = new StringBuilder();
            SqlParameter[] venueParams = new SqlParameter[(dtSession.Rows.Count * 3) + 1];
            HashSet<string> venues = new HashSet<string>();
            int cnt = 0;
            foreach (DataRow dr in dtSession.Rows)
            {
                sbVenueCondition.Append("(bookingDate=@dt" + cnt + " and bookingPeriod=@pt" + cnt + " and b.venueId=@vid" + cnt + ") or ");
                venueParams[cnt * 3] = new SqlParameter("@dt" + cnt, dr["sessionDate"]);
                venueParams[cnt * 3 + 1] = new SqlParameter("@pt" + cnt, dr["sessionPeriod"]);
                venueParams[cnt * 3 + 2] = new SqlParameter("@vid" + cnt, dr["venueId"]);

                venues.Add(dr["venueId"].ToString());

                cnt++;
            }

            DB_Venue dbVenue = new DB_Venue();
            string err = "";

            venueParams[venueParams.Length - 1] = new SqlParameter("@bid", batchId);
            DataTable dtBooking = dbVenue.isVenueAvailable(@"b.defunct='N' and (" + sbVenueCondition.ToString().Substring(0, sbVenueCondition.Length - 3) + @") 
                and sessionId not in (select sessionId from batchModule_session s inner join batch_module bm on s.batchModuleId=bm.batchModuleId and s.defunct='N' and bm.defunct='N' and bm.programmeBatchId=@bid)", venueParams);
            if (dtBooking == null) return new Tuple<bool, string>(false, "Error checking venue availability.");
            foreach (DataRow dr in dtBooking.Rows)
                err += "<li>Venue " + dr["venueLocation"].ToString() + " is not available on " + ((DateTime)dr["bookingDate"]).ToString("dd MMM yyyy") + "/" + dr["bookingPeriodDisp"].ToString() + "</li>";


            //check capacity
            DataTable dtCapacity = dbVenue.getVenueCapacity(venues.ToArray());
            if (dtCapacity == null) return new Tuple<bool, string>(false, "Error checking venue capacity.");
            foreach (DataRow drCap in dtCapacity.Rows)
            {
                if ((int)drCap["venueCapacity"] < capacity)
                {
                    err += "<li>Venue " + drCap["venueLocation"].ToString() + " is unable to accomodate capacity of " + capacity + ".</li>";
                }
            }

            if (err != "") return new Tuple<bool, string>(false, err);

            //update the batch module dates based on session dates (if needed)
            DateTime dt;
            foreach (DataRow dr in dtMod.Rows)
            {
                //if current batch module start date is later than min session date (of the module) or earlier than commencement date, change to min session date
                dt = (DateTime)dtSession.Compute("min(sessionDate)", "moduleId=" + dr["moduleId"].ToString());
                if (((DateTime)dr["startDate"]).CompareTo(dt) > 0 || ((DateTime)dr["startDate"]).CompareTo(dtBatchStart) < 0)
                    dr["startDate"] = dt;

                //if current batch module end date is earlier than max session date (of the module) or later than batch end date, change to max session date
                dt = (DateTime)dtSession.Compute("max(sessionDate)", "moduleId=" + dr["moduleId"].ToString());
                if (((DateTime)dr["endDate"]).CompareTo(dt) < 0 || ((DateTime)dr["endDate"]).CompareTo(dtBatchEnd) > 0)
                    dr["endDate"] = dt;
            }

            if (dbBatchSess.updateBatch(batchId, batchCode, projCode, dtRegStart, dtRegEnd, dtBatchStart, dtBatchEnd, capacity, mode, dtMod, dtSession, userId))
                return new Tuple<bool, string>(true, "Class saved successfully.");
            else
                return new Tuple<bool, string>(false, "Error updating class.");
        }

        public string formatDayStr(string str, DataTable dtPeriods = null)
        {
            if (dtPeriods == null) dtPeriods = (new DB_Venue()).getPeriods();

            string[] n;
            string desc = "";
            foreach (string d in str.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                n = d.Split(new char[] { '/' });
                switch (int.Parse(n[0]))
                {
                    case 1: desc += "Mon";
                        break;
                    case 2: desc += "Tue";
                        break;
                    case 3: desc += "Wed";
                        break;
                    case 4: desc += "Thu";
                        break;
                    case 5: desc += "Fri";
                        break;
                    case 6: desc += "Sat";
                        break;
                    case 7: desc += "Sun";
                        break;
                }

                desc += " (" + dtPeriods.Select("codeValue='" + n[1] + "'")[0]["codeValueDisplay"].ToString() + "), ";
            }

            return desc.Substring(0, desc.Length - 2);
        }

        public Tuple<string, DataTable> getProgrammeBatchByProgrammeBatchId(string programmeBatchId)
        {
            DataTable dtBatch = dbBatchSess.getProgrammeBatchByProgrammeBatchId(programmeBatchId);

            if (dtBatch == null)
                return new Tuple<string, DataTable>("No programme was found", null);

            if (Convert.ToDateTime(dtBatch.Rows[0]["programmeStartDate"].ToString()) <= DateTime.Now)
                return new Tuple<string, DataTable>("Applicant's selected programme had started.", null);

            if (dtBatch.Rows[0]["isDefunct"].ToString().Equals(General_Constance.STATUS_YES))
                return new Tuple<string, DataTable>("Applicant's selected programme is defunct.", null);

            return new Tuple<string, DataTable>("", dtBatch);

        }

        //Batches that are open for registration and enrollment
        public DataTable getAllBatchForRegistration()
        {
            DataTable dtCourseBatch = dbBatchSess.getAllBatchForRegistration();

            if (dtCourseBatch == null) return null;

            dtCourseBatch.Columns.Add("programmeStartDateDisplay", typeof(String));
            dtCourseBatch.Columns.Add("programmeCompletionDateDisplay", typeof(String));

            foreach (DataRow dr in dtCourseBatch.Rows)
            {
                dr["programmeStartDateDisplay"] = Convert.ToDateTime(dr["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                dr["programmeCompletionDateDisplay"] = Convert.ToDateTime(dr["programmeCompletionDate"].ToString()).Date.ToString("dd MMM yyyy");
            }

            return dtCourseBatch;
        }

        //Batch for display regardless of defuct
        public DataTable getAllBatchForDisplay()
        {
            DataTable dtCourseBatch = dbBatchSess.getAllBatchForDisplay();

            if (dtCourseBatch == null) return null;

            dtCourseBatch.Columns.Add("programmeStartDateDisplay", typeof(String));
            dtCourseBatch.Columns.Add("programmeCompletionDateDisplay", typeof(String));

            foreach (DataRow dr in dtCourseBatch.Rows)
            {
                dr["programmeStartDateDisplay"] = Convert.ToDateTime(dr["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                dr["programmeCompletionDateDisplay"] = Convert.ToDateTime(dr["programmeCompletionDate"].ToString()).Date.ToString("dd MMM yyyy");
            }

            return dtCourseBatch;
        }

    }
}
