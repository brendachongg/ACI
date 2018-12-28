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
    public partial class batch_creation : BasePage
    {
        public const string PAGE_NAME = "batch-creation.aspx";
        private const string GV_SESSION_DATA = "DAY";
        private const string GV_MODULE_DATA = "MODULE";
        private const string SESSION_MODULE = "SESSION_MODULE";
        private const string ALL_SESSIONS = "ALL_SESSIONS";
        private const string GV_SESSION_ORI_DATA = "ORI_DAY";
       
        //contain module information (no session info) of the selected programme
        public DataTable BundleModules{
            get { return (ViewState[GV_MODULE_DATA] as DataTable); }
        }

        //contain session scheduling information
        public DataTable Sessions
        {
            get { return (ViewState[GV_SESSION_DATA] as DataTable); }
        }

        public bool isSessionSameAllMod
        {
            get { return cbSame.Checked; }
        }

        //from batch-session-creation page containing the module information of the sessions
        public DataTable SessionModules
        {
            get { return (DataTable)ViewState[SESSION_MODULE]; }
        }

        //from batch-session-creation page containing all the sessions of all modules
        public DataTable AllSessions
        {
            get { return (DataTable)ViewState[ALL_SESSIONS]; }
        }

        //contain batch information
        public BatchDetails PageDetails{
            get{
                BatchDetails bd = new BatchDetails()
                {
                    progCatCode = ddlProgrammeCategory.SelectedValue,
                    progLvlCode = ddlProgrammeLevel.SelectedValue,
                    progTitle = ddlProgramme.SelectedValue,
                    progId = int.Parse(ddlProgrammeVersion.SelectedValue),
                    progCode = lbProgrammeCode.Text,
                    progType = lbProgrammeType.Text,
                    bundleCode = lbBundle.Text,
                    bundleId = int.Parse(hfBundleId.Value),
                    bundleEffDate = lbBundleEffDt.Text,
                    bundleCost = decimal.Parse(lbBundleCost.Text),
                    batchCode = tbBatchCode.Text,
                    clsTypeCode = ddlClsType.SelectedValue,
                    projCode = tbProjCode.Text,
                    regStartDate = tbRegStartDate.Text,
                    regEndDate = tbRegEndDate.Text,
                    batchStartDate = DateTime.ParseExact(tbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    batchEndDate = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    batchCapacity = int.Parse(tbCapacity.Text),
                    batchModeCode = ddlMode.SelectedValue
                };
                
                return bd;
            }
        }

        public batch_creation()
            : base(PAGE_NAME, AccessRight_Constance.BATCH_NEW, batch_management.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadProgCategory();
                loadProgLevel();
                loadMode();
                loadSessionPeriods();
                loadClsType();

                if (PreviousPage is batch_session_creation) loadData();
                else enableBatchNSessionControls(false);
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
                panelSessionError.Visible = false;
            }

            venuesearch.type = venue_search.RecentType.SESSION;
            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
        }

        private void loadData()
        {
            BatchDetails bd = ((batch_session_creation)PreviousPage).PageDetails;
            ddlProgrammeCategory.SelectedValue = bd.progCatCode;
            ddlProgrammeLevel.SelectedValue = bd.progLvlCode;
            loadProgTitle();
            ddlProgramme.SelectedValue = bd.progTitle;
            ddlProgramme_SelectedIndexChanged(null, null);
            if (ddlProgrammeVersion.SelectedValue == "")
            {
                ddlProgrammeVersion.SelectedValue = bd.progId.ToString();
                ddlProgrammeVersion_SelectedIndexChanged(null, null);
            }

            tbBatchCode.Text = bd.batchCode;
            ddlClsType.SelectedValue = bd.clsTypeCode;
            tbProjCode.Text = bd.projCode;
            tbRegStartDate.Text = bd.regStartDate;
            tbRegEndDate.Text = bd.regEndDate;
            tbBatchStartDate.Text = bd.batchStartDate.ToString("dd MMM yyyy");
            tbBatchEndDate.Text = bd.batchEndDate.ToString("dd MMM yyyy");
            tbCapacity.Text = bd.batchCapacity.ToString();
            ddlMode.SelectedValue = bd.batchModeCode;

            cbSame.Checked = ((batch_session_creation)PreviousPage).isSessionSameAllMod;
            cbSame_CheckedChanged(null, null);
            populateDayGrid(((batch_session_creation)PreviousPage).Sessions);
            hfHasDayRows.Value = "1";

            ViewState[SESSION_MODULE] = ((batch_session_creation)PreviousPage).SessionModules;
            ViewState[ALL_SESSIONS] = ((batch_session_creation)PreviousPage).AllSessions;
        }

        private void loadClsType()
        {
            DataTable dt = (new Batch_Session_Management()).getClassType();
            ddlClsType.DataSource = dt;
            ddlClsType.DataBind();
            ddlClsType.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadSessionPeriods()
        {
            DataTable dt = (new Venue_Management()).getPeriods();
            ddlPeriod.DataSource = dt;
            ddlPeriod.DataBind();
            ddlPeriod.Items.Insert(0, new ListItem("--Select--", ""));
            ddlPeriod.Items.Add(new ListItem("Full Day", DayPeriod.FD.ToString()));
        }

        private void loadProgCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();

            ddlProgrammeCategory.DataSource = dt;
            ddlProgrammeCategory.DataBind();
            ddlProgrammeCategory.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadProgLevel()
        {
            DataTable dt = (new Programme_Management()).getProgrammeLevel();

            ddlProgrammeLevel.DataSource = dt;
            ddlProgrammeLevel.DataBind();
            ddlProgrammeLevel.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadMode()
        {
            DataTable dt = (new Batch_Session_Management()).getClassMode();

            ddlMode.DataSource = dt;
            ddlMode.DataBind();
            ddlMode.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void ddlProgrammeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if option is changed, have to check if session data has been enter for individual modules, if yes, prompt user that
            //in order to change have to clear away existing session data (and retain back the old selected values)
            hfNewProgCat.Value = ddlProgrammeCategory.SelectedValue;
            if (!cbSame.Checked && ViewState[GV_SESSION_DATA] != null && ((DataTable)ViewState[GV_SESSION_DATA]).Rows.Count > 0)
            {
                hfProgChgType.Value = "CAT";
                ddlProgrammeCategory.SelectedValue = hfSelProgCat.Value;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPrompt", "promptConfirmation();", true);
                return;
            }

            loadProgTitle();
            hfSelProgCat.Value = ddlProgrammeCategory.SelectedValue;
        }

        protected void ddlProgrammeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if option is changed, have to check if session data has been enter for individual modules, if yes, prompt user that
            //in order to change have to clear away existing session data (and retain back the old selected values)
            hfNewProgLvl.Value = ddlProgrammeLevel.SelectedValue;
            if (!cbSame.Checked && ViewState[GV_SESSION_DATA] != null && ((DataTable)ViewState[GV_SESSION_DATA]).Rows.Count > 0)
            {
                hfProgChgType.Value = "LVL";
                ddlProgrammeLevel.SelectedValue = hfSelProgLvl.Value;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPrompt", "promptConfirmation();", true);
                return;
            }

            loadProgTitle();
            hfSelProgLvl.Value = ddlProgrammeLevel.SelectedValue;
        }

        private void loadProgTitle()
        {
            if (ddlProgrammeCategory.SelectedIndex > 0 && ddlProgrammeLevel.SelectedIndex > 0)
            {
                DataTable dt = (new Programme_Management()).getAvaProgrammeTitle(ddlProgrammeCategory.SelectedValue, ddlProgrammeLevel.SelectedValue);

                ddlProgramme.Items.Clear();

                ddlProgramme.DataSource = dt;
                ddlProgramme.DataBind();
                ddlProgramme.Items.Insert(0, new ListItem("--Select--", ""));

                ddlProgramme.Enabled = true;
            }
            else
            {
                ddlProgramme.Items.Clear();
                ddlProgramme.Enabled = false;
            }

            ddlProgrammeVersion.Items.Clear();
            ddlProgrammeVersion.Enabled = false;
            clearProgrammeNBundleInfo();
            enableBatchNSessionControls(false);
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if option is changed, have to check if session data has been enter for individual modules, if yes, prompt user that
            //in order to change have to clear away existing session data (and retain back the old selected values)
            hfNewProg.Value = ddlProgramme.SelectedValue;
            if (!cbSame.Checked && ViewState[GV_SESSION_DATA] != null && ((DataTable)ViewState[GV_SESSION_DATA]).Rows.Count > 0)
            {
                hfProgChgType.Value = "PROG";
                ddlProgramme.SelectedValue = hfSelProg.Value;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPrompt", "promptConfirmation();", true);
                return;
            }

            hfSelProg.Value = ddlProgramme.SelectedValue;
            if (ddlProgramme.SelectedIndex > 0)
            {
                DataTable dt = (new Programme_Management()).getAvaProgrammeVersion(ddlProgrammeCategory.SelectedValue, ddlProgrammeLevel.SelectedValue, ddlProgramme.SelectedValue);

                ddlProgrammeVersion.DataSource = dt;
                ddlProgrammeVersion.DataBind();

                if (dt.Rows.Count == 1)
                {
                    //don need user to select auto select
                    ddlProgrammeVersion.SelectedIndex = 0;
                    ddlProgrammeVersion_SelectedIndexChanged(null, null);
                    return;
                }
                else
                {
                    ddlProgrammeVersion.Items.Insert(0, new ListItem("--Select--", ""));
                    ddlProgrammeVersion.Enabled = true;
                }
            }
            else
            {
                ddlProgrammeVersion.Items.Clear();
                ddlProgrammeVersion.Enabled = false;
                enableBatchNSessionControls(false);
            }

            clearProgrammeNBundleInfo();
        }

        protected void ddlProgrammeVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if option is changed, have to check if session data has been enter for individual modules, if yes, prompt user that
            //in order to change have to clear away existing session data (and retain back the old selected values)
            hfNewProgVersion.Value = ddlProgrammeVersion.SelectedValue;
            if (ViewState[GV_SESSION_DATA] != null && ((DataTable)ViewState[GV_SESSION_DATA]).Rows.Count > 0)
            {
                hfProgChgType.Value = "VER";
                ddlProgrammeVersion.SelectedValue = hfSelProgVersion.Value;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPrompt", "promptConfirmation();", true);
                return;
            }

            hfSelProgVersion.Value = ddlProgrammeVersion.SelectedValue;
            //get programme code
            DataTable dt = (new Programme_Management()).getProgrammeDetails(ddlProgrammeVersion.SelectedValue);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving programme details.");
                return;
            }

            lbProgrammeCode.Text = dt.Rows[0]["programmeCode"].ToString();
            lbProgrammeType.Text = dt.Rows[0]["programmeTypeDisp"].ToString();
            loadBundleModules((int)dt.Rows[0]["bundleId"]);

            enableBatchNSessionControls(true);
        }

        private void clearProgrammeNBundleInfo()
        {
            lbProgrammeCode.Text = "";
            lbProgrammeType.Text = "";
            lbBundle.Text = "";
            hfBundleId.Value = "";
            lbBundleEffDt.Text = "";
            lbBundleCost.Text = "";
            gvModule.Visible = false;
        }

        //private void loadBundleModules(string bundleCode)
        private void loadBundleModules(int bundleId)
        {
            Tuple<string, string, string, string, decimal, bool, DataTable> bundle = (new Bundle_Management()).getBundle(bundleId, true);
            if (bundle == null)
            {
                redirectToErrorPg("Error retrieving bundle details.");
                return;
            }

            hfBundleId.Value = bundleId.ToString();
            lbBundle.Text = bundle.Item1;
            lbBundleEffDt.Text = bundle.Item4;
            lbBundleCost.Text = bundle.Item5.ToString();
            gvModule.Visible = true;
            ViewState[GV_MODULE_DATA] = bundle.Item7;
            gvModule.Columns[0].Visible = true;
            gvModule.DataSource = bundle.Item7;
            gvModule.DataBind();
            gvModule.Columns[0].Visible = false;

            ddlModule.Items.Clear();
            ddlModule.DataSource = bundle.Item7;
            ddlModule.DataBind();
            ddlModule.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void selectVenue(string id, string location)
        {
            DateTime dtStart, dtEnd;
            dtStart = DateTime.ParseExact(tbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            dtEnd = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

            Venue_Management vm = new Venue_Management();

            int vCap = vm.getVenueCapacity(id);
            if (vCap < int.Parse(tbCapacity.Text))
            {
                lblSessionError.Text = "Venue " + location + " is unable to accomodate class capacity.";
                panelSessionError.Visible = true;
                return;
            }

            panelVenueBooking.Visible = true;
            DataTable dt = vm.getVenueBookings(int.Parse(ddlDay.SelectedValue), (DayPeriod)Enum.Parse(typeof(DayPeriod), ddlPeriod.SelectedValue), dtStart, dtEnd, id);
            gvVenueBooking.DataSource = dt;
            gvVenueBooking.DataBind();

            tbVenue.Text = location;
            hfVenueId.Value = id;
        }

        protected void cbSame_CheckedChanged(object sender, EventArgs e)
        {
            if (cbSame.Checked)
            {
                panelModule.Visible = false;
            }
            else
            {
                panelModule.Visible = true;
            }
        }

        private void populateDayGrid(DataTable dt)
        {
            gvDay.Visible = true;
            ViewState[GV_SESSION_DATA] = dt;
            gvDay.DataSource = new DataView(dt, "", "moduleStartDt, day, periodOrder", DataViewRowState.CurrentRows);
            gvDay.Columns[1].Visible = true;
            gvDay.Columns[0].Visible = true;
            gvDay.Columns[2].Visible = true;
            gvDay.Columns[3].Visible = true;
            gvDay.DataBind();
            if (cbSame.Checked)
            {
                gvDay.Columns[1].Visible = false;
                gvDay.Columns[2].Visible = false;
                gvDay.Columns[3].Visible = false;
            }
            gvDay.Columns[0].Visible = false;
        }

        protected void btnClearAllDay_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[GV_SESSION_DATA] as DataTable;
            dt.Rows.Clear();

            populateDayGrid(dt);

            hfHasDayRows.Value = "0";
            //panelModule.Visible = !panelModule.Visible;
            //cbSame.Checked = !cbSame.Checked;

            //clear away existing session data as wells
            ViewState[SESSION_MODULE] = null;
            ViewState[ALL_SESSIONS] = null;

            //set the newly selected values (if this is invoked from the prompt due to change of prog ddl values)
            if (hfProgChgType.Value == "CAT")
            {
                ddlProgrammeCategory.SelectedValue = hfNewProgCat.Value;
                ddlProgrammeCategory_SelectedIndexChanged(null, null);
            }
            else if (hfProgChgType.Value == "LVL")
            {
                ddlProgrammeLevel.SelectedValue = hfNewProgLvl.Value;
                ddlProgrammeLevel_SelectedIndexChanged(null, null);
            }
            else if (hfProgChgType.Value == "PROG")
            {
                ddlProgramme.SelectedValue = hfNewProg.Value;
                ddlProgramme_SelectedIndexChanged(null, null);
            }
            else if (hfProgChgType.Value == "VER")
            {
                ddlProgrammeVersion.SelectedValue = hfNewProgVersion.Value;
                ddlProgrammeVersion_SelectedIndexChanged(null, null);
            }
        }

        private bool isValidDay(int day, string period, string clsType)
        {
            if ((clsType == ClassType.SAT_D.ToString().Replace("_", "/") && (day != 6 || period == DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.SAT_E.ToString().Replace("_", "/") && (day != 6 || period != DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.SUN_D.ToString().Replace("_", "/") && (day != 7 || period == DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.SUN_E.ToString().Replace("_", "/") && (day != 7 || period != DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.WDY_D.ToString().Replace("_", "/") && (day == 6 || day == 7 || period == DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.WDY_E.ToString().Replace("_", "/") && (day == 6 || day == 7 || period != DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.WEN_D.ToString().Replace("_", "/") && ((day != 6 && day != 7) || period == DayPeriod.EVE.ToString())) ||
                (clsType == ClassType.WEN_E.ToString().Replace("_", "/") && ((day != 6 && day != 7) || period != DayPeriod.EVE.ToString())))
                return false;

            return true;
        }

        protected void ddlModule_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlModule.SelectedIndex == 0)
            {
                tbModEndDate.Text = "";
                tbModStartDate.Text = "";
                return;
            }

            //check if user has prev enter and then auto fill in the date range
            DataTable dt = ViewState[GV_SESSION_DATA] as DataTable;
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow[] dr = dt.Select("moduleId=" + ddlModule.SelectedValue);
            if (dr.Length == 0)
            {
                tbModEndDate.Text = "";
                tbModStartDate.Text = "";
                return;
            }

            tbModStartDate.Text = ((DateTime)dr[0]["moduleStartDt"]).ToString("dd MMM yyyy");
            tbModEndDate.Text = ((DateTime)dr[0]["moduleEndDt"]).ToString("dd MMM yyyy");
        }

        private void addDay()
        {
            DataTable dt = ViewState[GV_SESSION_DATA] as DataTable;
            if (dt == null)
            {
                dt = new DataTable();
                dt.Columns.Add(new DataColumn("moduleId", typeof(int)));
                dt.Columns.Add(new DataColumn("moduleCode", typeof(string)));
                dt.Columns.Add(new DataColumn("moduleTitle", typeof(string)));
                dt.Columns.Add(new DataColumn("moduleStartDt", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("moduleEndDt", typeof(DateTime)));
                dt.Columns.Add(new DataColumn("day", typeof(int)));
                dt.Columns.Add(new DataColumn("dayDisp", typeof(string)));
                dt.Columns.Add(new DataColumn("period", typeof(string)));
                dt.Columns.Add(new DataColumn("periodOrder", typeof(int)));
                dt.Columns.Add(new DataColumn("periodDisp", typeof(string)));
                dt.Columns.Add(new DataColumn("venueLocation", typeof(string)));
                dt.Columns.Add(new DataColumn("venueId", typeof(string)));
            }

            DateTime dtS = DateTime.MinValue, dtE = DateTime.MaxValue;
            if (!cbSame.Checked)
            {
                dtS = DateTime.ParseExact(tbModStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dtE = DateTime.ParseExact(tbModEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            }

            if (ddlPeriod.SelectedValue == DayPeriod.FD.ToString())
            {
                if (dt.Select("day=" + ddlDay.SelectedValue + " and (period='" + DayPeriod.AM.ToString() + "' or period='" + DayPeriod.PM.ToString() + "')"
                    + (cbSame.Checked ? "" : " and moduleStartDt <= #" + dtE.ToString("MM/dd/yyyy") + "# and moduleEndDt >= #" + dtS.ToString("MM/dd/yyyy") + "#")).Length > 0)
                {
                    lblSessionError.Text = "Duplicated session found.";
                    panelSessionError.Visible = true;
                    return;
                }

                DataRow dr1 = dt.NewRow();
                DataRow dr2 = dt.NewRow();
                if (!cbSame.Checked)
                {
                    dr1["moduleId"] = ddlModule.SelectedValue;
                    dr2["moduleId"] = ddlModule.SelectedValue;
                    dr1["moduleTitle"] = ddlModule.SelectedItem.Text;
                    dr2["moduleTitle"] = ddlModule.SelectedItem.Text;
                    dr1["moduleCode"] = ((DataTable)ViewState[GV_MODULE_DATA]).Select("moduleId=" + ddlModule.SelectedValue)[0]["moduleCode"].ToString();
                    dr2["moduleCode"] = ((DataTable)ViewState[GV_MODULE_DATA]).Select("moduleId=" + ddlModule.SelectedValue)[0]["moduleCode"].ToString();
                    dr1["moduleStartDt"] = dtS;
                    dr2["moduleStartDt"] = dtS;
                    dr1["moduleEndDt"] = dtE;
                    dr2["moduleEndDt"] = dtE;

                    //update all the same module start and end date
                    foreach (DataRow d in dt.Select("moduleId=" + ddlModule.SelectedValue))
                    {
                        d["moduleStartDt"] = dtS;
                        d["moduleEndDt"] = dtE;
                    }
                }
                dr1["day"] = int.Parse(ddlDay.SelectedValue);
                dr1["dayDisp"] = ddlDay.SelectedItem.Text;
                dr1["period"] = DayPeriod.AM.ToString();
                dr1["periodDisp"] = DayPeriod.AM.ToString();
                dr1["periodOrder"] = ddlPeriod.SelectedIndex;
                dr1["venueId"] = hfVenueId.Value;
                dr1["venueLocation"] = tbVenue.Text;
                dt.Rows.Add(dr1);

                dr2["day"] = int.Parse(ddlDay.SelectedValue);
                dr2["dayDisp"] = ddlDay.SelectedItem.Text;
                dr2["period"] = DayPeriod.PM.ToString();
                dr2["periodDisp"] = DayPeriod.PM.ToString();
                dr2["periodOrder"] = ddlPeriod.SelectedIndex;
                dr2["venueId"] = hfVenueId.Value;
                dr2["venueLocation"] = tbVenue.Text;
                dt.Rows.Add(dr2);
            }
            else
            {
                //check for duplicate
                if (dt.Select("day=" + ddlDay.SelectedValue + " and period='" + ddlPeriod.SelectedValue + "'"
                    + (cbSame.Checked ? "" : " and moduleStartDt <= #" + dtE.ToString("MM/dd/yyyy") + "# and moduleEndDt >= #" + dtS.ToString("MM/dd/yyyy") + "#")).Length > 0)
                {
                    lblSessionError.Text = "Duplicated session found.";
                    panelSessionError.Visible = true;
                    return;
                }

                DataRow dr = dt.NewRow();
                if (!cbSame.Checked)
                {
                    dr["moduleId"] = ddlModule.SelectedValue;
                    dr["moduleTitle"] = ddlModule.SelectedItem.Text;
                    dr["moduleCode"] = ((DataTable)ViewState[GV_MODULE_DATA]).Select("moduleId=" + ddlModule.SelectedValue)[0]["moduleCode"].ToString();
                    dr["moduleStartDt"] = dtS;
                    dr["moduleEndDt"] = dtE;

                    //update all the same module start and end date
                    foreach (DataRow d in dt.Select("moduleId=" + ddlModule.SelectedValue))
                    {
                        d["moduleStartDt"] = dtS;
                        d["moduleEndDt"] = dtE;
                    }
                }
                dr["day"] = int.Parse(ddlDay.SelectedValue);
                dr["dayDisp"] = ddlDay.SelectedItem.Text;
                dr["period"] = ddlPeriod.SelectedValue;
                dr["periodDisp"] = ddlPeriod.SelectedItem.Text;
                dr["periodOrder"] = ddlPeriod.SelectedIndex;
                dr["venueId"] = hfVenueId.Value;
                dr["venueLocation"] = tbVenue.Text;
                dt.Rows.Add(dr);
            }

            populateDayGrid(dt);

            hfHasDayRows.Value = "1";

            ddlModule.SelectedIndex = 0;
            tbModEndDate.Text = "";
            tbModStartDate.Text = "";
            ddlDay.SelectedIndex = 0;
            ddlPeriod.SelectedIndex = 0;
            tbVenue.Text = "";
            hfVenueId.Value = "";
            panelVenueBooking.Visible = false;
        }

        protected void btnAddDay_Click(object sender, EventArgs e)
        {
            //check user select correct day according to cls type
            if (!isValidDay(int.Parse(ddlDay.SelectedValue), ddlPeriod.SelectedValue, ddlClsType.SelectedValue))
            {
                lblSessionError.Text = "Select day/period does not match selected class type.";
                panelSessionError.Visible = true;
                return;
            }

            if (ViewState[ALL_SESSIONS] != null)
            {
                hfDayMode.Value = "ADD";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showPrompt", "promptConfirmation();", true);
                return;
            }

            addDay();
        }

        protected void btnClearDay_Click(object sender, EventArgs e)
        {
            if (ddlModule.Items.Count > 0) ddlModule.SelectedIndex = 0;
            tbModEndDate.Text = "";
            tbModStartDate.Text = "";
            ddlDay.SelectedIndex = 0;
            if (ddlPeriod.Items.Count > 0) ddlPeriod.SelectedIndex = 0;
            tbVenue.Text = "";
            hfVenueId.Value = "";
        }

        protected void btnRemDay_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[GV_SESSION_DATA] as DataTable;
            DataRow dr = dt.Select("day=" + hdSelDay.Value + " and period='" + hdSelPeriod.Value + "'")[0];
            dt.Rows.Remove(dr);

            populateDayGrid(dt);

            if (dt.Rows.Count == 0) hfHasDayRows.Value = "0";
        }

        private void enableBatchNSessionControls(bool enable)
        {
            tbBatchCode.Enabled = enable;
            ddlClsType.Enabled = enable;
            tbProjCode.Enabled = enable;
            tbRegEndDate.Enabled = enable;
            tbRegStartDate.Enabled = enable;
            tbBatchEndDate.Enabled = enable;
            tbBatchStartDate.Enabled = enable;
            tbCapacity.Enabled = enable;
            ddlMode.Enabled = enable;
            cbSame.Enabled = enable;
            ddlDay.Enabled = enable;
            ddlPeriod.Enabled = enable;
            lbtnSearchVenue.Visible = enable;
            btnAddDay.Enabled = enable;
            btnClearDay.Enabled = enable;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            if (ddlProgrammeCategory.Items.Count > 0) ddlProgrammeCategory.SelectedIndex = 0;
            if (ddlProgrammeLevel.Items.Count > 0) ddlProgrammeLevel.SelectedIndex = 0;
            if (ddlProgramme.Items.Count > 0) ddlProgramme.SelectedIndex = 0;
            ddlProgramme.Enabled = false;
            if (ddlProgrammeVersion.Items.Count > 0) ddlProgrammeVersion.SelectedIndex = 0;
            ddlProgrammeVersion.Enabled = false;

            lbProgrammeCode.Text = "";
            lbProgrammeType.Text = "";
            lbBundle.Text = "";
            hfBundleId.Value = "";
            lbBundleEffDt.Text = "";
            lbBundleCost.Text = "";
            gvModule.Visible = false;
            tbBatchCode.Text = "";
            if (ddlClsType.Items.Count > 0) ddlClsType.SelectedIndex = 0;
            tbProjCode.Text = "";
            tbRegEndDate.Text = "";
            tbRegStartDate.Text = "";
            tbBatchEndDate.Text = "";
            tbBatchStartDate.Text = "";
            tbCapacity.Text = "";
            if (ddlMode.Items.Count > 0) ddlMode.SelectedIndex = 0;
            cbSame.Checked = true;
            panelModule.Visible = false;
            ddlDay.SelectedIndex = 0;
            if (ddlPeriod.Items.Count > 0) ddlPeriod.SelectedIndex = 0;
            tbVenue.Text = "";
            hfVenueId.Value = "";
            hfHasDayRows.Value = "0";
            gvDay.Visible = false;

            btnClearAllDay_Click(null, null);

            enableBatchNSessionControls(false);
        }

        protected void btnSessions_Click(object sender, EventArgs e)
        {
            //check batch code exist
            lblError.Text = "Please correct the following:";
            bool hasError = false;

            //check if registration date earlier than today
            DateTime dtRegStart = DateTime.ParseExact(tbRegStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            if (dtRegStart.CompareTo(DateTime.Now) < 0)
            {
                lblError.Text += "<li>Registration date cannot be earlier than today.</li>";
                hasError = true;
            }

            if((new Batch_Session_Management()).checkBatchCode(tbBatchCode.Text)){
                lblError.Text += "<li>Duplicated class code.</li>";
                hasError = true;
            }

            //check if each session date/period follows the selected class type
            //and within the commencement date
            DataTable dt = ViewState[GV_SESSION_DATA] as DataTable;
            if (dt == null || dt.Rows.Count == 0)
            {
                lblError.Text += "<li>Missing session information.</li>";
                hasError = true;
            }
            else if (!cbSame.Checked)
            {
                //check if day for all modules are entered
                foreach (DataRow dr in ((DataTable)ViewState[GV_MODULE_DATA]).Rows)
                {
                    if (dt.Select("moduleId=" + dr["moduleId"].ToString()).Length == 0)
                    {
                        lblError.Text += "<li>Missing session details for module " + dr["moduleTitle"].ToString() + "</li>";
                        hasError = true;
                    }
                }
            }

            if (dt != null)
            {
                DateTime dtBatchStart = DateTime.ParseExact(tbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                DateTime dtBatchEnd = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                HashSet<string> venue = new HashSet<string>();
                foreach (DataRow dr in dt.Rows)
                {
                    if (!isValidDay((int)dr["day"], dr["period"].ToString(), ddlClsType.SelectedValue))
                    {
                        lblError.Text += "<li>Selected day " + dr["dayDisp"].ToString() + ", period " + dr["periodDisp"].ToString() + " does not match with selected class type.</li>";
                        hasError = true;
                    }

                    if (!cbSame.Checked)
                    {
                        if (((DateTime)dr["moduleStartDt"]) < dtBatchStart || ((DateTime)dr["moduleStartDt"]) > dtBatchEnd || ((DateTime)dr["moduleEndDt"]) < dtBatchStart || ((DateTime)dr["moduleEndDt"]) > dtBatchEnd)
                        {
                            lblError.Text += "<li>Session for " + dr["moduleTitle"].ToString() + " from " + ((DateTime)dr["moduleStartDt"]).ToString("dd MMM yyyy") + " to "
                                + ((DateTime)dr["moduleEndDt"]).ToString("dd MMM yyyy") + " is not within commencement date</li>";
                            hasError = true;
                        }
                    }

                    venue.Add(dr["venueId"].ToString());
                }

                //check if venue can accomodate capacity (in case user change capacity after selecting venue)
                if (venue.Count > 0)
                {
                    DataTable dtCap = (new Venue_Management()).getVenueCapacity(venue.ToArray());
                    if (dtCap == null)
                    {
                        redirectToErrorPg("Error retrieving venue details.");
                        return;
                    }
                    int capacity = int.Parse(tbCapacity.Text);
                    foreach (DataRow drC in dtCap.Rows)
                    {
                        if ((int)drC["venueCapacity"] < capacity)
                        {
                            lblError.Text += "<li>Venue " + drC["venueLocation"].ToString() + " is unable to accomodate class due to limited capacity.</li>";
                            hasError = true;
                        }
                    }
                }
            }

            if (hasError)
            {
                panelError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
                return;
            }

            //transfer to session page
            Server.Transfer(batch_session_creation.PAGE_NAME, true);
        }

        protected void btnClearSession_Click(object sender, EventArgs e)
        {
            ViewState[ALL_SESSIONS] = null;
            if (hfDayMode.Value == "ADD") addDay();
            else if (hfDayMode.Value == "DEL") btnRemDay_Click(null, null);
            hfDayMode.Value = "";
        }

        protected void gvDay_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            HtmlButton ctrl = (HtmlButton)e.Row.FindControl("btnDelDay");
            ctrl.Attributes.Add("data-target", ViewState[ALL_SESSIONS] != null ? "#diagClearSession" : "#diagRemDay");
            ctrl.Attributes.Add("onClick", "confirmDel(" + ((DataRowView)e.Row.DataItem)["day"].ToString() + ", '" + ((DataRowView)e.Row.DataItem)["period"].ToString() + "');");
        }     
    }
}