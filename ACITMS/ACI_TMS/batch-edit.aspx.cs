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
    public partial class batch_edit : BasePage
    {
        public const string PAGE_NAME = "batch-edit.aspx";
        public const string QUERY_ID = "id";

        private DataTable dtTrainers, dtAssessors, dtPeriods;
        private Batch_Session_Management bsm = new Batch_Session_Management();
        private int currModuleId;

        private const string ACCESS_EDIT_DATES = "editDates";
        private const string MODULE_DAY = "modules";
        private const string GV_SESSION_DATA = "sessions";

        public batch_edit()
            : base(PAGE_NAME, AccessRight_Constance.BATCH_EDIT, batch_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            PrevPage = PrevPage + "?" + batch_view.QUERY_ID + "=" + hfBatchId.Value;

            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing class information.", batch_management.PAGE_NAME);
                    return;
                }

                hfBatchId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);

                loadMode();
                loadProgrammeDetails();

                //if registration has started, check if user has access rights to edit the date
                if (bsm.isBatchStarted(int.Parse(hfBatchId.Value), BatchDateType.REGISTRATION) && !checkAccessRights(AccessRight_Constance.BATCH_EDIT_DATES))
                {
                    tbRegStartDate.Enabled = false;
                    tbRegEndDate.Enabled = false;
                    tbBatchStartDate.Enabled = false;
                    tbBatchEndDate.Enabled = false;
                    tbBatchCode.Enabled = false;

                    ViewState[ACCESS_EDIT_DATES] = false;
                }
                else ViewState[ACCESS_EDIT_DATES] = true;

                loadModuleSessionDetails();
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

        private void loadMode()
        {
            DataTable dt = (new Batch_Session_Management()).getClassMode();

            ddlMode.DataSource = dt;
            ddlMode.DataBind();
            ddlMode.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadProgrammeDetails()
        {
            DataTable dt = bsm.getBatchDetails(int.Parse(hfBatchId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving class details.");
                return;
            }

            DataRow dr = dt.Rows[0];
            lbProgrammeCategory.Text = dr["programmeCategoryDisp"].ToString();
            lbProgrammeLevel.Text = dr["programmeLevelDisp"].ToString();
            lbProgramme.Text = dr["programmeTitle"].ToString();
            lbProgrammeVersion.Text = dr["programmeVersion"].ToString();
            lbProgrammeCode.Text = dr["programmeCode"].ToString();
            lbProgrammeType.Text = dr["programmeTypeDisp"].ToString();

            tbBatchCode.Text = dr["batchCode"].ToString();
            lbClsType.Text = dr["batchTypeDisp"].ToString();
            tbProjCode.Text = dr["projectCode"].ToString();
            tbRegStartDate.Text = dr["programmeRegStartDateDisp"].ToString();
            tbRegEndDate.Text = dr["programmeRegEndDateDisp"].ToString();
            tbBatchStartDate.Text = dr["programmeStartDateDisp"].ToString();
            tbBatchEndDate.Text = dr["programmeCompletionDateDisp"].ToString();
            tbCapacity.Text = dr["batchCapacity"].ToString();
            ddlMode.SelectedValue = dr["classMode"].ToString();
        }

        private void loadModuleSessionDetails()
        {
            Tuple<DataTable, DataTable> t = bsm.getBatchModulesNSessions(int.Parse(hfBatchId.Value));

            if (t.Item1 == null || t.Item1.Rows.Count == 0 || t.Item2 == null || t.Item2.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving class module and session details.");
                return;
            }

            ACI_Staff_User su = new ACI_Staff_User();
            dtTrainers = su.getTrainers();
            dtAssessors = su.getAssessors();
            dtPeriods = (new Venue_Management()).getPeriods();
            DataRow drPeriod = dtPeriods.NewRow();
            drPeriod["codeValueDisplay"] = "--Select--";
            drPeriod["codeValue"] = "";
            dtPeriods.Rows.InsertAt(drPeriod, 0);

            rpModuleTabs.DataSource = t.Item1;
            rpModuleTabs.DataBind();
            //check if all the days in the modules are the same or different
            ViewState[MODULE_DAY] = ((new DataView(t.Item1)).ToTable(true, "day").Rows.Count == 1 ? t.Item1.Rows[0]["day"].ToString() : null);

            DataTable dt = t.Item2;
            //add temp columns for postpone of session if need
            dt.Columns.Add(new DataColumn("isVenueAva", typeof(bool)));
            dt.Columns.Add(new DataColumn("isDtExceed", typeof(bool)));
            foreach(DataRow dr in dt.Rows) {
                dr["isVenueAva"] = true;
                dr["isDtExceed"] = false;
            }
            ViewState[GV_SESSION_DATA] = dt;
            rpModuleContent.DataSource = t.Item1;
            rpModuleContent.DataBind();

            //set the first module tab to show
            ((HtmlGenericControl)rpModuleTabs.Items[0].FindControl("tabMod")).Attributes.Add("class", "active");
            hfSelModule.Value = t.Item1.Rows[0]["moduleId"].ToString();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showFirstModule", "showModule('" + hfSelModule.Value + "');", true);
        }

        protected void rpModuleContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DropDownList ddl = e.Item.FindControl("ddlTrainer1") as DropDownList;
            ddl.DataSource = dtTrainers;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (((DataRowView)e.Item.DataItem)["trainerUserId1"] != DBNull.Value) ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["trainerUserId1"].ToString();

            ddl = e.Item.FindControl("ddlTrainer2") as DropDownList;
            ddl.DataSource = dtTrainers;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (((DataRowView)e.Item.DataItem)["trainerUserId2"] != DBNull.Value) ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["trainerUserId2"].ToString();

            ddl = e.Item.FindControl("ddlAssessor") as DropDownList;
            ddl.DataSource = dtAssessors;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Not Assigned--", ""));
            if (((DataRowView)e.Item.DataItem)["assessorUserId"] != DBNull.Value) ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["assessorUserId"].ToString();

            currModuleId = (int)((DataRowView)e.Item.DataItem)["moduleId"];

            Repeater rp = e.Item.FindControl("rpSessions") as Repeater;
            DataTable dt = ((DataTable)ViewState[GV_SESSION_DATA]).Select("batchModuleId=" + ((DataRowView)e.Item.DataItem)["batchModuleId"].ToString()).CopyToDataTable();
            rp.DataSource = dt;
            rp.DataBind();
            hfSessionNamingContainer.Text += rp.ClientID + ";";
            hfSessionCount.Text += dt.Rows.Count + ";";
        }

        protected void rpSessions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DropDownList ddl = e.Item.FindControl("ddlSessionPeriod") as DropDownList;
            ddl.DataSource = dtPeriods;
            ddl.DataBind();
            ddl.SelectedValue = ((DataRowView)e.Item.DataItem)["sessionPeriod"].ToString();

            Label lbl = e.Item.FindControl("lbtnSearchVenue") as Label;
            lbl.Attributes.Add("onclick", "showVenueDialog('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                + ", " + ((DataRowView)e.Item.DataItem)["sessionNo"].ToString() + ");");

            lbl = e.Item.FindControl("lbtnRefreshVenue") as Label;
            lbl.Attributes.Add("onclick", "refreshVenueAvailability('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                + ", " + ((DataRowView)e.Item.DataItem)["sessionNo"].ToString() + ");");

            lbl = e.Item.FindControl("lbtnPostpone") as Label;
            if (ViewState[MODULE_DAY] != null && (bool)ViewState[ACCESS_EDIT_DATES])
            {
                //only able to have the postpone function if all modules are using the same days
                lbl.Attributes.Add("onclick", "postponeSession('" + e.Item.Parent.ClientID + "', " + e.Item.ItemIndex + ", " + currModuleId
                        + ", " + ((DataRowView)e.Item.DataItem)["sessionNo"].ToString() + ");");
            }
            else lbl.Visible = false;

            ddl.Enabled = ((TextBox)e.Item.FindControl("tbSessionDt")).Enabled = (bool)ViewState[ACCESS_EDIT_DATES];
                
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
                    if ((new Venue_Management()).checkVenueAvailable(DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        (DayPeriod)Enum.Parse(typeof(DayPeriod), ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue), id, int.Parse(tbCapacity.Text), 
                        int.Parse(((HiddenField)rSess.FindControl("hfSessionId")).Value)))
                    {
                        lb.Text = "(Available)";
                        lb.ForeColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        lb.Text = "(Not Available)";
                        lb.ForeColor = System.Drawing.Color.Red;
                    }

                    break;
                }

                break;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfVenueSelModule.Value + "');", true);
        }

        protected void lbtnPostpone_Click(object sender, EventArgs e)
        {
            HiddenField hf;
            string currModule;
            bool toRegenerate = false;

            DateTime dtS, dtE;
            dtE = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            DataTable dt;
            int numOfSessions;
            string prevModEndPeriod = null;
            DateTime prevModEndDt = DateTime.MinValue;
            Label lb;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                Repeater rpSessions = rMod.FindControl("rpSessions") as Repeater;

                if (currModule == hfSessionSelModule.Value)
                {
                    numOfSessions = int.Parse(((HiddenField)rMod.FindControl("hfNumSession")).Value);
                 
                    int cnt = 1;
                    dt = null;
                    foreach (RepeaterItem rSess in rpSessions.Items)
                    {
                        if (((HiddenField)rSess.FindControl("hfSessionNo")).Value == hfSessionSelSession.Value)
                        {
                            dtS = DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                            prevModEndPeriod = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue;

                            dt = ((DataTable)ViewState[GV_SESSION_DATA]).Select("moduleId=" + currModule).CopyToDataTable();

                            //check if current venue is available on the new date
                            Label lbVenueAva = rSess.FindControl("lbVenueAva") as Label;
                            if ((new Venue_Management()).checkVenueAvailable(dtS, (DayPeriod)Enum.Parse(typeof(DayPeriod), prevModEndPeriod), ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value,
                                int.Parse(tbCapacity.Text), int.Parse(((HiddenField)rSess.FindControl("hfSessionId")).Value)))
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
                            else ((TextBox)rSess.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.Transparent;

                            if (cnt < rpSessions.Items.Count) bsm.postponeSessions((string)ViewState[MODULE_DAY], dtS, prevModEndPeriod, dtE, dt, cnt);
                            else prevModEndDt = dtS;

                            break;
                        }

                        cnt++;
                    }

                    //if it is not the last session of the module being postpone, populate the rest of the session in the module with the new data
                    if (cnt < rpSessions.Items.Count)
                    {
                        populatePostponeSessions(dt, cnt, rpSessions);
                        prevModEndDt = (DateTime)dt.Rows[dt.Rows.Count - 1]["sessionDate"];
                        prevModEndPeriod = dt.Rows[dt.Rows.Count - 1]["sessionPeriod"].ToString();
                    }

                    toRegenerate = true;

                    //set the module end date
                    lb = rMod.FindControl("lbDtTo") as Label;
                    lb.Text = prevModEndDt.ToString("dd MMM yyyy");
                    if (prevModEndDt > dtE) lb.BackColor = System.Drawing.Color.LightPink;
                    else lb.BackColor = System.Drawing.Color.LightGray;
                }
                else if (toRegenerate)
                {
                    dt = ((DataTable)ViewState[GV_SESSION_DATA]).Select("moduleId=" + currModule).CopyToDataTable();
                    bsm.postponeSessions((string)ViewState[MODULE_DAY], prevModEndDt, prevModEndPeriod, dtE, dt, 0);

                    populatePostponeSessions(dt, 0, rpSessions);

                    prevModEndDt = (DateTime)dt.Rows[dt.Rows.Count - 1]["sessionDate"];
                    prevModEndPeriod = dt.Rows[dt.Rows.Count - 1]["sessionPeriod"].ToString();

                    //set the module start date and end date
                    //if the date exceeded the commencement end date then flag as error
                    lb = rMod.FindControl("lbDtFrm") as Label;
                    lb.Text = ((DateTime)dt.Rows[0]["sessionDate"]).ToString("dd MMM yyyy");
                    if ((DateTime)dt.Rows[0]["sessionDate"] > dtE) lb.BackColor = System.Drawing.Color.LightPink;

                    //set the module end date
                    lb = rMod.FindControl("lbDtTo") as Label;
                    lb.Text = prevModEndDt.ToString("dd MMM yyyy");
                    if (prevModEndDt > dtE) lb.BackColor = System.Drawing.Color.LightPink;
                    else lb.BackColor = System.Drawing.Color.LightGray;
                }

            }

            lblSuccess.Text = "Remaining sessions has been postponed accordingly.";
            panelSuccess.Visible = true;
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfSessionSelModule.Value + "');", true);
        }

        private void populatePostponeSessions(DataTable dt, int startIndex, Repeater rpSessions)
        {
            for (int i = 0; i < rpSessions.Items.Count; i++)
            {
                if (i < startIndex) continue;

                RepeaterItem rSess = rpSessions.Items[i];
                DataRow dr = dt.Rows[i];

                ((TextBox)rSess.FindControl("tbSessionDt")).Text = dr["sessionDateDisp"].ToString();
                ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue = dr["sessionPeriod"].ToString();
                ((TextBox)rSess.FindControl("tbSessionVenue")).Text = dr["venueLocation"].ToString();
                ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value = dr["venueId"].ToString();

                Label lbVenueAva = rSess.FindControl("lbVenueAva") as Label;
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

                if ((bool)dr["isDtExceed"]) ((TextBox)rSess.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.LightPink;
                else ((TextBox)rSess.FindControl("tbSessionDt")).BackColor = System.Drawing.Color.Transparent;
            }
        }

        protected void lbtnRefreshVenue_Click(object sender, EventArgs e)
        {
            selectVenue(hfSelVenueId.Value, hfSelVenueLoc.Value);
        }

        private void venueRefresh()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfVenueSelModule.Value + "');", true);
        }

        private DataTable getModules()
        {
            DataTable dtMod = new DataTable();
            dtMod.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtMod.Columns.Add(new DataColumn("trainerUserId1", typeof(int)));
            dtMod.Columns.Add(new DataColumn("trainerUserId2", typeof(int)));
            dtMod.Columns.Add(new DataColumn("assessorUserId", typeof(int)));
            dtMod.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dtMod.Columns.Add(new DataColumn("endDate", typeof(DateTime)));

            DataRow dr;
            DropDownList ddl;
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                dr = dtMod.NewRow();
                dr["moduleId"] = int.Parse(((HiddenField)rMod.FindControl("hfModuleId")).Value);
                dr["startDate"] = DateTime.ParseExact(((HiddenField)rMod.FindControl("hfDtFrm")).Value, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dr["endDate"] = DateTime.ParseExact(((HiddenField)rMod.FindControl("hfDtTo")).Value, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dr["trainerUserId1"] = ((DropDownList)rMod.FindControl("ddlTrainer1")).SelectedIndex == 0 ? (object)DBNull.Value : int.Parse(((DropDownList)rMod.FindControl("ddlTrainer1")).SelectedValue);
                dr["trainerUserId2"] = ((DropDownList)rMod.FindControl("ddlTrainer2")).SelectedIndex == 0 ? (object)DBNull.Value : int.Parse(((DropDownList)rMod.FindControl("ddlTrainer2")).SelectedValue);
                dr["assessorUserId"] = ((DropDownList)rMod.FindControl("ddlAssessor")).SelectedIndex == 0 ? (object)DBNull.Value : int.Parse(((DropDownList)rMod.FindControl("ddlAssessor")).SelectedValue);

                dtMod.Rows.Add(dr);
            }

            return dtMod;
        }

        private Tuple<DataTable, bool, string> getSessions()
        {
            //get all the sessions
            DataTable dtSession = new DataTable();
            dtSession.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtSession.Columns.Add(new DataColumn("moduleTitle", typeof(string)));  //for error message purposes
            dtSession.Columns.Add(new DataColumn("sessionId", typeof(int)));
            dtSession.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtSession.Columns.Add(new DataColumn("sessionDateDisp", typeof(string)));
            dtSession.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));
            dtSession.Columns.Add(new DataColumn("sessionPeriodDisp", typeof(string)));    //for error message purposes
            dtSession.Columns.Add(new DataColumn("venueId", typeof(string)));
            dtSession.Columns.Add(new DataColumn("venueLocation", typeof(string)));    //for error message purposes

            Repeater rpSession;
            DataRow drSession;
            int moduleId;
            string moduleTitle, errMsg = "";
            bool hasError = false;
            DateTime dt, dtTmp;
            DateTime dtBatchStart = DateTime.ParseExact(tbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            DateTime dtBatchEnd = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                moduleId = int.Parse(((HiddenField)rMod.FindControl("hfModuleId")).Value);
                moduleTitle = ((HiddenField)rMod.FindControl("hfModuleTitle")).Value;

                rpSession = rMod.FindControl("rpSessions") as Repeater;
                foreach (RepeaterItem rSess in rpSession.Items)
                {
                    drSession = dtSession.NewRow();
                    drSession["moduleId"] = moduleId;
                    drSession["moduleTitle"] = moduleTitle;
                    drSession["sessionId"] = int.Parse(((HiddenField)rSess.FindControl("hfSessionId")).Value);
                    drSession["sessionDate"] = DateTime.ParseExact(((TextBox)rSess.FindControl("tbSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                    drSession["sessionDateDisp"] = ((TextBox)rSess.FindControl("tbSessionDt")).Text;
                    drSession["sessionPeriod"] = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedValue;
                    drSession["sessionPeriodDisp"] = ((DropDownList)rSess.FindControl("ddlSessionPeriod")).SelectedItem.Text;
                    drSession["venueId"] = ((HiddenField)rSess.FindControl("hfSessionVenueId")).Value;
                    drSession["venueLocation"] = ((TextBox)rSess.FindControl("tbSessionVenue")).Text;

                    //check if the session exist
                    if (dtSession.Select("sessionDate=#" + ((DateTime)drSession["sessionDate"]).ToString("MM/dd/yyyy") + "# and sessionPeriod='" + drSession["sessionPeriod"].ToString() + "'").Length > 0)
                    {
                        errMsg += "<li>Duplicated session on " + ((DateTime)drSession["sessionDate"]).ToString("dd MMM yyyy") + "/" + drSession["sessionPeriodDisp"].ToString() + "</li>";
                        hasError = true;
                    }

                    dtSession.Rows.Add(drSession);
                }

                //determine if batch module start and end date need to be changed

                //if current batch module start date is later than min session date (of the module) or earlier than commencement date, change to min session date
                dt = DateTime.ParseExact(((Label)rMod.FindControl("lbDtFrm")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dtTmp = (DateTime)dtSession.Compute("min(sessionDate)", "moduleId=" + moduleId);
                if (dt.CompareTo(dtTmp) > 0 || dt.CompareTo(dtBatchStart) < 0)
                    ((Label)rMod.FindControl("lbDtFrm")).Text = ((HiddenField)rMod.FindControl("hfDtFrm")).Value = dtTmp.ToString("dd MMM yyyy");

                //if current batch module end date is earlier than max session date (of the module) or later than batch end date, change to max session date
                dt = DateTime.ParseExact(((Label)rMod.FindControl("lbDtTo")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dtTmp = (DateTime)dtSession.Compute("max(sessionDate)", "moduleId=" + moduleId);
                if (dt.CompareTo(dtTmp) < 0 || dt.CompareTo(dtBatchEnd) > 0)
                    ((Label)rMod.FindControl("lbDtTo")).Text = ((HiddenField)rMod.FindControl("hfDtTo")).Value = dtTmp.ToString("dd MMM yyyy");
            }

            return new Tuple<DataTable, bool, string>(dtSession, hasError, errMsg);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            lblSysError.Text = "Please correct the following:";
            bool hasError = false;

            Tuple<DataTable, bool, string> sessions = getSessions();
            if (sessions.Item2)
            {
                lblSysError.Text += sessions.Item3;
                hasError = true;
            }

            DateTime dtStart = DateTime.ParseExact(tbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            DateTime dtEnd = DateTime.ParseExact(tbBatchEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            if (dtStart.CompareTo((DateTime)sessions.Item1.Compute("min(sessionDate)", "")) > 0 || dtEnd.CompareTo((DateTime)sessions.Item1.Compute("max(sessionDate)", "")) < 0)
            {
                lblSysError.Text += "<li>One or more session dates is not within the commencement start and end date</li>";
                hasError = true;
            }

            if (hasError)
            {
                panelSysError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelectedModule", "showModule('" + hfSelModule.Value + "');scrollTopPage();", true);
                return;
            }

            //update batch
            Tuple<bool, string> status = bsm.updateBatch(int.Parse(hfBatchId.Value), tbBatchCode.Text, tbProjCode.Text, DateTime.ParseExact(tbRegStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                DateTime.ParseExact(tbRegEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), dtStart, dtEnd, int.Parse(tbCapacity.Text), ddlMode.SelectedValue,
                getModules(), sessions.Item1, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblSysError.Text = status.Item2;
                panelSysError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelectedModule", "showModule('" + hfSelModule.Value + "');scrollTopPage();", true);
        }

        protected void btnClearBatch_Click(object sender, EventArgs e)
        {
            loadProgrammeDetails();
            loadModuleSessionDetails();
        }

        
    }
}