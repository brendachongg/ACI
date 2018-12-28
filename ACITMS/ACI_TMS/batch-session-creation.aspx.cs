using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class batch_session_creation : BasePage
    {
        public const string PAGE_NAME = "batch-session-creation.aspx";
        private const string GV_SESSION_DATA = "DAY";
        private const string GV_MODULE_DATA = "MODULE";
        private const string BATCH_DATA = "BATCH";
        private const string SAME_SESSION_DATA = "SAME_SESSION";

        private const string SESSION_MODULE = "SESSION_MODULE";
        private const string ALL_SESSIONS = "ALL_SESSIONS";

        private Batch_Session_Management bsm = new Batch_Session_Management();
        private DataTable dtTrainers, dtAssessors, dtPeriods, dtExistModules, dtExistSessions;
        private int currModuleId;

        //originate from batch-creation page
        public BatchDetails PageDetails
        {
            get { return (BatchDetails)ViewState[BATCH_DATA]; }
        }

        //originate from batch-creation page
        public DataTable BundleModule
        {
            get { return (DataTable)ViewState[GV_MODULE_DATA]; }
        }

        //originate from batch-creation page
        public DataTable Sessions
        {
            get { return (DataTable)ViewState[GV_SESSION_DATA]; }
        }

        //originate from batch-creation page
        public bool isSessionSameAllMod
        {
            get { return (bool)ViewState[SAME_SESSION_DATA]; }
        }

        //contains all the sessions from all modules
        public DataTable AllSessions
        {
            get { return (DataTable)ViewState[ALL_SESSIONS]; }
        }

        //contains the modules in the bundle of the selected programme of the batch
        public DataTable SessionModules
        {
            get { return (DataTable)ViewState[SESSION_MODULE]; }
        }

        public string StatusMsg
        {
            get { return hfStatusMsg.Value; }
        }

        public batch_session_creation()
            : base(PAGE_NAME, AccessRight_Constance.BATCH_NEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!(PreviousPage is batch_creation))
                {
                    redirectToErrorPg("Uable to retrieve class information.");
                    return;
                }

                //store all the previous page data in view state
                batch_creation p = PreviousPage as batch_creation;
                ViewState[GV_MODULE_DATA] = p.BundleModules;
                ViewState[GV_SESSION_DATA] = p.Sessions;
                ViewState[BATCH_DATA] = p.PageDetails;
                ViewState[SAME_SESSION_DATA] = p.isSessionSameAllMod;
                dtExistModules = p.SessionModules;
                dtExistSessions = p.AllSessions;

                ACI_Staff_User su = new ACI_Staff_User();
                dtTrainers = su.getTrainers();
                dtAssessors = su.getAssessors();
                dtPeriods = (new Venue_Management()).getPeriods();

                loadModuleTabs();
                loadModuleContent();

                lbBatchStartDate.Text = p.PageDetails.batchStartDate.ToString("dd MMM yyyy");
                lbBatchEndDate.Text = p.PageDetails.batchEndDate.ToString("dd MMM yyyy");
                hfBatchEndDate.Value = p.PageDetails.batchEndDate.ToString("dd MMM yyyy");
                hfBatchStartDate.Value = p.PageDetails.batchStartDate.ToString("dd MMM yyyy");
            }
            else
            {
                panelSysError.Visible = false;
                panelSuccess.Visible = false;
            }

            venuesearch.type = venue_search.RecentType.SESSION;
            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
            venuesearch.venueRefresh += new venue_search.VenueRefresh(venueRefresh);
        }

        private void loadModuleTabs()
        {
            //modules should be displayed in order of the module start and end date if is not same session for all modules
            DataTable dt;
            if (!(bool)ViewState[SAME_SESSION_DATA])
                dt = (new DataView((DataTable)ViewState[GV_SESSION_DATA], "", "moduleStartDt, moduleEndDt, moduleCode", DataViewRowState.CurrentRows))
                    .ToTable(true, new string[] { "moduleId", "moduleCode", "moduleTitle" });
            else
                dt = (DataTable)ViewState[GV_MODULE_DATA];

            rpModuleTabs.DataSource = dt;
            rpModuleTabs.DataBind();
        }

        private DataTable generateModuleContent()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleCode", typeof(string)));
            dt.Columns.Add(new DataColumn("moduleTitle", typeof(string)));
            dt.Columns.Add(new DataColumn("dayDisp", typeof(string)));
            dt.Columns.Add(new DataColumn("day", typeof(string)));
            dt.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("endDate", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("startDateDisp", typeof(string)));
            dt.Columns.Add(new DataColumn("endDateDisp", typeof(string)));
            dt.Columns.Add(new DataColumn("ModuleNumOfSession", typeof(int)));

            bool isSameSession = (bool)ViewState[SAME_SESSION_DATA];
            string day = null, dayDisp = null;
            if (isSameSession)
            {
                Tuple<string, string> t = getSessionDay(-1);
                day = t.Item1;
                dayDisp = t.Item2;
            }

            foreach (DataRow dr in ((DataTable)ViewState[GV_MODULE_DATA]).Rows)
            {
                DataRow drMod = dt.NewRow();

                //fill module info
                drMod["moduleId"] = dr["moduleId"];
                drMod["moduleCode"] = dr["moduleCode"];
                drMod["moduleTitle"] = dr["moduleTitle"];
                drMod["ModuleNumOfSession"] = dr["ModuleNumOfSession"];

                //fill batch module info
                if (isSameSession)
                {
                    drMod["day"] = day;
                    drMod["dayDisp"] = dayDisp;
                }
                else
                {
                    Tuple<string, string> t = getSessionDay((int)dr["moduleId"]);
                    drMod["day"] = t.Item1;
                    drMod["dayDisp"] = t.Item2;

                    drMod["startDate"] = ((DataTable)ViewState[GV_SESSION_DATA]).Select("moduleId=" + dr["moduleId"])[0]["moduleStartDt"];
                    drMod["endDate"] = ((DataTable)ViewState[GV_SESSION_DATA]).Select("moduleId=" + dr["moduleId"])[0]["moduleEndDt"];
                    drMod["startDateDisp"] = ((DateTime)drMod["startDate"]).ToString("dd MMM yyyy");
                    drMod["endDateDisp"] = ((DateTime)drMod["EndDate"]).ToString("dd MMM yyyy");
                }

                dt.Rows.Add(drMod);
            }

            return dt;
        }

        private void loadModuleContent()
        {
            DataTable dt = dtExistModules == null ? generateModuleContent() : dtExistModules;
            
            //check the session dates again see if within commencement date in case user go back and change
            if (dtExistSessions != null)
            {
                foreach (DataRow dr in dtExistSessions.Rows)
                {
                    if (((DateTime)dr["sessionDate"]).CompareTo(((BatchDetails)ViewState[BATCH_DATA]).batchStartDate) < 0 ||
                        ((DateTime)dr["sessionDate"]).CompareTo(((BatchDetails)ViewState[BATCH_DATA]).batchEndDate) > 0)
                        dr["isDtExceed"] = true;
                }
            }

            //if same day for all modules, then start date and end date is generated by controller
            //but set the first module start date to the commencement date
            if ((bool)ViewState[SAME_SESSION_DATA]) 
            { 
                //prevModEndDt = ((BatchDetails)ViewState[BATCH_DATA]).batchStartDate.AddDays(-1);
                prevModEndDt = ((BatchDetails)ViewState[BATCH_DATA]).batchStartDate;
            }

            rpModuleContent.DataSource = dt;
            rpModuleContent.DataBind();

            //sets the first tab as active
            ((HtmlGenericControl)rpModuleTabs.Items[0].FindControl("tabMod")).Attributes.Add("class", "active");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showFirstModule", "showModule('" + dt.Rows[0]["moduleId"].ToString() + "');", true);
            hfSelModule.Value = dt.Rows[0]["moduleId"].ToString();
        }

        //the class variables are only used if scheduling is based on same session for all modules
        DateTime prevModEndDt;
        string prevModEndPeriod = null;
        private Tuple<string, string> getSessionDay(int moduleId)
        {
            string day = "", dayDisp = "";
            string conditon = moduleId == -1 ? "" : "moduleId=" + moduleId;
            foreach (DataRow dr in ((DataTable)ViewState[GV_SESSION_DATA]).Select(conditon, "day, periodOrder"))
            {
                day += dr["day"].ToString() + "/" + dr["period"].ToString() + ";";
                dayDisp += dr["dayDisp"].ToString() + "(" + dr["periodDisp"].ToString() + "), ";
            }
            return new Tuple<string, string>(day.Substring(0, day.Length - 1), dayDisp.Substring(0, dayDisp.Length - 2));
        }

        protected void rpModuleContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataTable dt;
            DataRow[] drs;
            if (dtExistSessions == null)
            {
                //generate the sessions
                string condition = (bool)ViewState[SAME_SESSION_DATA] ? "" : "moduleId=" + ((DataRowView)e.Item.DataItem)["moduleId"].ToString();
                drs = ((DataTable)ViewState[GV_SESSION_DATA]).Select(condition, "day, periodOrder");
                DateTime dtS, dtE;
                if ((bool)ViewState[SAME_SESSION_DATA])
                {
                    //if all module same day, then next module start date will be 1 day after the prev module end date
                    //the latest the module can end is the batch end date
                    //dtS = prevModEndDt.AddDays(1); 
                    dtS = prevModEndDt;
                    dtE = ((BatchDetails)ViewState[BATCH_DATA]).batchEndDate;
                }
                else
                {
                    dtS = (DateTime)drs[0]["moduleStartDt"];
                    dtE = (DateTime)drs[0]["moduleEndDt"];
                }

                dt = bsm.genSessions(drs, dtS, prevModEndPeriod, dtE, (int)((DataRowView)e.Item.DataItem)["ModuleNumOfSession"]);

                //set the end date to the last session date
                if ((bool)ViewState[SAME_SESSION_DATA])
                {
                    prevModEndDt = (DateTime)dt.Rows[dt.Rows.Count - 1]["sessionDate"];
                    prevModEndPeriod = dt.Rows[dt.Rows.Count - 1]["sessionPeriod"].ToString();

                    //set the module start date and end date
                    //if the date exceeded the commencement end date then flag as error
                    TextBox tb = e.Item.FindControl("tbDtFrm") as TextBox;
                    tb.Text = ((DateTime)dt.Rows[0]["sessionDate"]).ToString("dd MMM yyyy");
                    if ((DateTime)dt.Rows[0]["sessionDate"] > dtE) tb.BackColor = System.Drawing.Color.LightPink;

                    tb = e.Item.FindControl("tbDtTo") as TextBox;
                    tb.Text = prevModEndDt.ToString("dd MMM yyyy");
                    if (prevModEndDt > dtE) tb.BackColor = System.Drawing.Color.LightPink;
                }
            }
            else
            {
                drs = dtExistSessions.Select("moduleId=" + ((DataRowView)e.Item.DataItem)["moduleId"].ToString());
                dt = drs.CopyToDataTable();

                if ((DateTime)((DataRowView)e.Item.DataItem)["startDate"] > ((BatchDetails)ViewState[BATCH_DATA]).batchEndDate)
                    ((TextBox)e.Item.FindControl("tbDtFrm")).BackColor = System.Drawing.Color.LightPink;

                if ((DateTime)((DataRowView)e.Item.DataItem)["endDate"] > ((BatchDetails)ViewState[BATCH_DATA]).batchEndDate)
                    ((TextBox)e.Item.FindControl("tbDtTo")).BackColor = System.Drawing.Color.LightPink;
            }

            //set current moduleId
            currModuleId = (int)((DataRowView)e.Item.DataItem)["moduleId"];

            Repeater rpSessions = e.Item.FindControl("rpSessions") as Repeater;
            rpSessions.DataSource = dt;
            rpSessions.DataBind();
            hfSessionNamingContainer.Text += rpSessions.ClientID + ";";
            hfSessionCount.Text += dt.Rows.Count + ";";

            DropDownList ddl = e.Item.FindControl("ddlTrainer1") as DropDownList;
            ddl.DataSource = dtTrainers;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (dtExistModules != null && ((DataRowView)e.Item.DataItem)["trainerUserId1"] != DBNull.Value)
                ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["trainerUserId1"].ToString();

            ddl = e.Item.FindControl("ddlTrainer2") as DropDownList;
            ddl.DataSource = dtTrainers;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (dtExistModules != null && ((DataRowView)e.Item.DataItem)["trainerUserId2"] != DBNull.Value)
                ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["trainerUserId2"].ToString();

            ddl = e.Item.FindControl("ddlAssessor") as DropDownList;
            ddl.DataSource = dtAssessors;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (dtExistModules != null && ((DataRowView)e.Item.DataItem)["assessorUserId"] != DBNull.Value)
                ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["assessorUserId"].ToString();
        }

        protected void rpSessions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataRowView dr = (DataRowView)e.Item.DataItem;

            Label lbVenueAva = e.Item.FindControl("lbVenueAva") as Label;
            if ((bool)dr["isVenueAva"])
            {
                lbVenueAva.Text = "(Available)";
                lbVenueAva.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lbVenueAva.Text = "(Not Available)";
                lbVenueAva.ForeColor = System.Drawing.Color.Red;
            }

            if ((bool)dr["isDtExceed"])
            {
                ((TextBox)e.Item.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.LightPink;
            }

            if (!(bool)ViewState[SAME_SESSION_DATA]) e.Item.FindControl("lbtnPostpone").Visible = false;
            else
            {
                ((Label)e.Item.FindControl("lbtnPostpone")).Attributes.Add("onclick", "postponeSession('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                    + ", " + dr["sessionNo"].ToString() + ");");
            }
            //e.Item.FindControl("lbtnPostpone").Visible = false;

            DropDownList ddl = e.Item.FindControl("ddlSessionPeriod") as DropDownList;
            ddl.DataSource = dtPeriods;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Select--", ""));

            ddl.SelectedValue = dr["sessionPeriod"].ToString();

            Label lbl = e.Item.FindControl("lbtnSearchVenue") as Label;
            lbl.Attributes.Add("onclick", "showVenueDialog('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                + ", " + dr["sessionNo"].ToString() + ");");

            lbl = e.Item.FindControl("lbtnRefreshVenue") as Label;
            lbl.Attributes.Add("onclick", "refreshVenueAvailability('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                + ", " + dr["sessionNo"].ToString() + ");");

            if (dtExistModules != null)
            {
                ((TextBox)e.Item.FindControl("tbSessionVenue")).Text = ((DataRowView)e.Item.DataItem)["venueLocation"].ToString();
                ((HiddenField)e.Item.FindControl("hfSessionVenueId")).Value = ((DataRowView)e.Item.DataItem)["venueId"].ToString();
            }
        }

        private void selectVenue(string id, string location)
        {
            HiddenField hf;
            string currModule;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                if (currModule != hfVenueSelModule.Value) continue;

                Repeater rpSessions = rMod.FindControl("rpSessions") as Repeater;
                foreach (RepeaterItem rSess in rpSessions.Items)
                {
                    hf = rSess.FindControl("hfSessionNo") as HiddenField;

                    if (hf.Value != hfVenueSelSession.Value) continue;

                    ((TextBox)rSess.FindControl("tbSessionVenue")).Text = location;
                    ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value = id;

                    Label lb = rSess.FindControl("lbVenueAva") as Label;
                    //check venue available
                    if((new Venue_Management()).checkVenueAvailable(DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        (DayPeriod)Enum.Parse(typeof(DayPeriod), ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue), id, ((BatchDetails)ViewState[BATCH_DATA]).batchCapacity))
                    {
                        lb.Text = "(Available)";
                        lb.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lb.Text="(Not Available)";
                        lb.ForeColor = System.Drawing.Color.Red;
                    }

                    break;
                }

                break;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfVenueSelModule.Value + "');", true);
        }

        protected void lbtnRefreshVenue_Click(object sender, EventArgs e)
        {
            selectVenue(hfSelVenueId.Value, hfSelVenueLoc.Value);
        }

        private void venueRefresh()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfVenueSelModule.Value + "');", true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            Tuple<DataTable, DataTable, bool, string> data = getAllData(false);
            ViewState[SESSION_MODULE] = data.Item1;
            ViewState[ALL_SESSIONS] = data.Item2;

            Server.Transfer(batch_creation.PAGE_NAME, true);
        }

        private Tuple<DataTable, DataTable, bool, string> getAllData(bool checkError)
        {
            //get all the sessions
            DataTable dtSession = new DataTable();
            dtSession.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtSession.Columns.Add(new DataColumn("moduleTitle", typeof(string)));  //for error message purposes
            dtSession.Columns.Add(new DataColumn("sessionNo", typeof(int)));
            dtSession.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtSession.Columns.Add(new DataColumn("sessionDateDisp", typeof(string)));
            dtSession.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));
            dtSession.Columns.Add(new DataColumn("sessionPeriodDisp", typeof(string)));    //for error message purposes
            dtSession.Columns.Add(new DataColumn("venueId", typeof(string)));
            dtSession.Columns.Add(new DataColumn("venueLocation", typeof(string)));    //for error message purposes
            dtSession.Columns.Add(new DataColumn("isVenueAva", typeof(bool)));
            dtSession.Columns.Add(new DataColumn("isDtExceed", typeof(bool))); 

            DataTable dtMod = new DataTable();
            dtMod.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtMod.Columns.Add(new DataColumn("moduleTitle", typeof(string)));
            dtMod.Columns.Add(new DataColumn("day", typeof(string)));
            dtMod.Columns.Add(new DataColumn("dayDisp", typeof(string)));
            dtMod.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dtMod.Columns.Add(new DataColumn("endDate", typeof(DateTime)));
            dtMod.Columns.Add(new DataColumn("endDateDisp", typeof(string)));
            dtMod.Columns.Add(new DataColumn("startDateDisp", typeof(string)));
            dtMod.Columns.Add(new DataColumn("trainerUserId1", typeof(int)));
            dtMod.Columns.Add(new DataColumn("trainerUserId2", typeof(int)));
            dtMod.Columns.Add(new DataColumn("assessorUserId", typeof(int)));
            dtMod.Columns.Add(new DataColumn("ModuleNumOfSession", typeof(int)));

            DataRow drMod, drSession;
            Repeater rpSession;
            bool hasError = false;
            string errMsg = "";
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                drMod = dtMod.NewRow();
                drMod["moduleId"] = int.Parse(((HiddenField)rMod.FindControl("hfModuleId")).Value);
                drMod["moduleTitle"] = ((HiddenField)rMod.FindControl("hfModuleTitle")).Value;
                drMod["ModuleNumOfSession"] = int.Parse(((HiddenField)rMod.FindControl("hfNumSession")).Value);
                drMod["day"] = ((HiddenField)rMod.FindControl("hfDay")).Value;
                drMod["dayDisp"] = ((TextBox)rMod.FindControl("tbDay")).Text;
                drMod["startDate"] = DateTime.ParseExact(((TextBox)rMod.FindControl("tbDtFrm")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                drMod["endDate"] = DateTime.ParseExact(((TextBox)rMod.FindControl("tbDtTo")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                drMod["startDateDisp"] = ((TextBox)rMod.FindControl("tbDtFrm")).Text;
                drMod["endDateDisp"] = ((TextBox)rMod.FindControl("tbDtTo")).Text;
                drMod["trainerUserId1"] = ((DropDownList)rMod.FindControl("ddlTrainer1")).SelectedIndex == 0 ? (object)DBNull.Value : ((DropDownList)rMod.FindControl("ddlTrainer1")).SelectedValue;
                drMod["trainerUserId2"] = ((DropDownList)rMod.FindControl("ddlTrainer2")).SelectedIndex == 0 ? (object)DBNull.Value : ((DropDownList)rMod.FindControl("ddlTrainer2")).SelectedValue;
                drMod["assessorUserId"] = ((DropDownList)rMod.FindControl("ddlAssessor")).SelectedIndex == 0 ? (object)DBNull.Value : ((DropDownList)rMod.FindControl("ddlAssessor")).SelectedValue; 
                dtMod.Rows.Add(drMod);

                rpSession = rMod.FindControl("rpSessions") as Repeater;
                foreach (RepeaterItem rSess in rpSession.Items)
                {
                    drSession = dtSession.NewRow();
                    drSession["moduleId"] = drMod["moduleId"];
                    drSession["moduleTitle"] = ((HiddenField)rMod.FindControl("hfModuleTitle")).Value;
                    drSession["sessionNo"] = int.Parse(((HiddenField)rSess.FindControl("hfSessionNo")).Value);
                    drSession["sessionDate"] =((TextBox)rSess.FindControl("tbSessionDt")).Text == "" ? (object)DBNull.Value : DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                    drSession["sessionDateDisp"] = ((TextBox)rSess.FindControl("tbSessionDt")).Text == "" ? (object)DBNull.Value : ((TextBox)rSess.FindControl("tbSessionDt")).Text;
                    drSession["sessionPeriod"] = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue;
                    drSession["sessionPeriodDisp"] = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedItem.Text;
                    drSession["venueId"] = ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value;
                    drSession["venueLocation"] = ((TextBox)rSess.FindControl("tbSessionVenue")).Text;

                    if (((TextBox)rSess.FindControl("tbSessionDt")).Text == "") drSession["isDtExceed"] = false;
                    else 
                    {
                        if ((DateTime)drSession["sessionDate"] > ((BatchDetails)ViewState[BATCH_DATA]).batchEndDate 
                            || (DateTime)drSession["sessionDate"] < ((BatchDetails)ViewState[BATCH_DATA]).batchStartDate)
                            drSession["isDtExceed"] = true;
                        else drSession["isDtExceed"] = false;
                    }

                    if (((Label)rSess.FindControl("lbVenueAva")).Text.Contains("Not")) 
                    { 
                        drSession["isVenueAva"] = false;
                        errMsg += "<li>Venue " + drSession["venueLocation"].ToString() + " is not available on " + ((TextBox)rSess.FindControl("tbSessionDt")).Text + " " + drSession["sessionPeriodDisp"].ToString() + ".</li>";
                        hasError = true;
                    } else drSession["isVenueAva"] = true;

                    //check if the session exist
                    if (checkError && dtSession.Select("sessionDate=#" + ((DateTime)drSession["sessionDate"]).ToString("MM/dd/yyyy") + "# and sessionPeriod='" + drSession["sessionPeriod"].ToString() + "'").Length > 0)
                    {
                        errMsg += "<li>Duplicated session on " + ((DateTime)drSession["sessionDate"]).ToString("dd MMM yyyy") + "/" + drSession["sessionPeriodDisp"].ToString() + "</li>";
                        hasError = true;
                    }

                    dtSession.Rows.Add(drSession);
                }
            }

            return new Tuple<DataTable, DataTable, bool, string>(dtMod, dtSession, hasError, errMsg);
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            //get all the sessions
            Tuple<DataTable, DataTable, bool, string> data = getAllData(true);

            if (data.Item3)
            {
                lblSysError.Text = data.Item4;
                panelSysError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfSelModule.Value + "');", true);
                return;
            }

            Tuple<bool, string> status = bsm.createBatchNSession(PageDetails, data.Item1, data.Item2, LoginID);

            if (status.Item1)
            {
                hfStatusMsg.Value = status.Item2;
                Server.Transfer(batch_management.PAGE_NAME, true);
                return;
            }

            lblSysError.Text = status.Item2;
            panelSysError.Visible = true;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfSelModule.Value + "');", true);
        }

        //only used when sessions for module are generated based on same day session data in previous page
        protected void lbtnPostpone_Click(object sender, EventArgs e)
        {
            HiddenField hf;
            string currModule;
            bool toRegenerate = false;

            DataRow[] drs = ((DataTable)ViewState[GV_SESSION_DATA]).Select("", "day, periodOrder");
            DateTime dtS, dtE;
            dtE = ((BatchDetails)ViewState[BATCH_DATA]).batchEndDate;
            DataTable dt;
            int numOfSessions;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                if (currModule == hfSessionSelModule.Value)
                {
                    numOfSessions = int.Parse(((HiddenField)rMod.FindControl("hfNumSession")).Value);

                    Repeater rpSessions = rMod.FindControl("rpSessions") as Repeater;
                    int cnt = 1, n = 0;
                    dt = null;
                    foreach (RepeaterItem rSess in rpSessions.Items)
                    {
                        if (((HiddenField)rSess.FindControl("hfSessionNo")).Value == hfSessionSelSession.Value)
                        {
                            dtS = DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                            prevModEndPeriod = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue;

                            //check if current venue is available on the new date
                            Label lbVenueAva = rSess.FindControl("lbVenueAva") as Label;
                            if ((new Venue_Management()).checkVenueAvailable(dtS, (DayPeriod)Enum.Parse(typeof(DayPeriod), prevModEndPeriod), ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value, 
                                ((BatchDetails)ViewState[BATCH_DATA]).batchCapacity))
                            {
                                lbVenueAva.Text = "(Available)";
                                lbVenueAva.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                lbVenueAva.Text = "(Not Available)";
                                lbVenueAva.ForeColor = System.Drawing.Color.Red;
                            }

                            //check if new date is after batch end date
                            if (dtS > dtE) ((TextBox)rSess.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.LightPink;

                            //if the number of new sessions to generate is 0, means user tries to postpone on the last session of the module, so no need to call the gensession function, just set the module end date
                            if (numOfSessions - cnt > 0) dt = bsm.genSessions(drs, dtS, prevModEndPeriod, dtE, numOfSessions - cnt);
                            else prevModEndDt = dtS;
                            
                        }
                        else if (dt != null)
                        {
                            //sessions has been regenerated, fill in input fields
                            ((TextBox)rSess.FindControl("tbSessionDt")).Text = dt.Rows[n]["sessionDateDisp"].ToString();
                            ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue = dt.Rows[n]["sessionPeriod"].ToString();
                            ((TextBox)rSess.FindControl("tbSessionVenue")).Text = dt.Rows[n]["venueLocation"].ToString();
                            ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value = dt.Rows[n]["venueId"].ToString();

                            Label lbVenueAva = rSess.FindControl("lbVenueAva") as Label;
                            if ((bool)dt.Rows[n]["isVenueAva"])
                            {
                                lbVenueAva.Text = "(Available)";
                                lbVenueAva.ForeColor = System.Drawing.Color.Green;
                            }
                            else
                            {
                                lbVenueAva.Text = "(Not Available)";
                                lbVenueAva.ForeColor = System.Drawing.Color.Red;
                            }

                            if ((bool)dt.Rows[n]["isDtExceed"])
                            {
                                ((TextBox)rSess.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.LightPink;
                            }

                            n++;
                        }

                        cnt++;
                    }

                    toRegenerate = true;
                    if (dt != null)
                    {
                    prevModEndDt = (DateTime)dt.Rows[dt.Rows.Count - 1]["sessionDate"];
                    prevModEndPeriod = dt.Rows[dt.Rows.Count - 1]["sessionPeriod"].ToString();
                    }
                    //set the module end date
                    ((TextBox)rMod.FindControl("tbDtTo")).Text = prevModEndDt.ToString("dd MMM yyyy");
                    if (prevModEndDt > dtE) ((TextBox)rMod.FindControl("tbDtTo")).BackColor = System.Drawing.Color.LightPink;

                    //get the period so can bind the period ddl in other modules
                    dtPeriods = (new Venue_Management()).getPeriods();
                }
                else if (toRegenerate)
                {
                    dt = bsm.genSessions(drs, prevModEndDt, prevModEndPeriod, dtE, int.Parse(((HiddenField)rMod.FindControl("hfNumSession")).Value));

                    prevModEndDt = (DateTime)dt.Rows[dt.Rows.Count - 1]["sessionDate"];
                    prevModEndPeriod = dt.Rows[dt.Rows.Count - 1]["sessionPeriod"].ToString();

                    //set the module start date and end date
                    //if the date exceeded the commencement end date then flag as error
                    TextBox tb = rMod.FindControl("tbDtFrm") as TextBox;
                    tb.Text = ((DateTime)dt.Rows[0]["sessionDate"]).ToString("dd MMM yyyy");
                    if ((DateTime)dt.Rows[0]["sessionDate"] > dtE) tb.BackColor = System.Drawing.Color.LightPink;

                    tb = rMod.FindControl("tbDtTo") as TextBox;
                    tb.Text = prevModEndDt.ToString("dd MMM yyyy");
                    if (prevModEndDt > dtE) tb.BackColor = System.Drawing.Color.LightPink;

                    //set current moduleId
                    currModuleId = int.Parse(currModule);

                    Repeater rpSessions = rMod.FindControl("rpSessions") as Repeater;
                    rpSessions.DataSource = dt;
                    rpSessions.DataBind();
                }

            }

            lblSuccess.Text = "Remaining sessions has been postponed accordingly.";
            panelSuccess.Visible = true;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfSessionSelModule.Value + "');", true);
        }
    }
}