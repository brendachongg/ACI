using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class bundle_edit_sessions : BasePage
    {
        public const string PAGE_NAME = "bundle-edit-sessions.aspx";
        private const string DATA_KEY = "dtSelModules";

        private Bundle_Management bm = new Bundle_Management();

        private DataTable dtAffProg, dtAffBatch, dtAddSession, dtRemSession, dtNewModule, dtRemModule;
        private HiddenField hfCurrSelBatch;

        public int BundleId
        {
            get { return int.Parse(hfBundleId.Value); }
        }

        public string BundleCode
        {
            get { return hfBundleCode.Value; }
        }

        public string BundleType
        {
            get { return hfBundleType.Value; }
        }

        public DataTable BundleModules
        {
            get { return ViewState[DATA_KEY] as DataTable; }
        }

        public string BundleEffDate
        {
            get { return hfEffectiveDate.Value; }
        }

        public decimal BundleCost
        {
            get { return decimal.Parse(hfCost.Value); }
        }

        public string RtnMode
        {
            get { return hfRtnMode.Value; }
        }

        public bundle_edit_sessions()
            : base(PAGE_NAME, AccessRight_Constance.BUNDLE_EDIT, bundle_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!(PreviousPage is bundle_edit))
                {
                    redirectToErrorPg("Uable to retrieve selected bundle information.");
                    return;
                }

                hfBundleId.Value = ((bundle_edit)PreviousPage).BundleId.ToString();
                hfBundleCode.Value = ((bundle_edit)PreviousPage).BundleCode;
                hfBundleType.Value = ((bundle_edit)PreviousPage).BundleType;
                hfEffectiveDate.Value = ((bundle_edit)PreviousPage).BundleEffDate;
                hfCost.Value = ((bundle_edit)PreviousPage).BundleCost.ToString();
                ViewState[DATA_KEY] = ((bundle_edit)PreviousPage).BundleModules;

                DataTable dtAffModules = bm.getUpdateBundleAffectedModules(int.Parse(hfBundleId.Value), ViewState[DATA_KEY] as DataTable);

                dtAffProg = bm.getUpdateBundleAffectedProgrammes(int.Parse(hfBundleId.Value));
                dtAffProg.Columns.Add(new DataColumn("moduleId", typeof(int)));

                dtAffBatch = bm.getUpdateBundleAffectedBatches(int.Parse(hfBundleId.Value));
                dtAffBatch.Columns.Add(new DataColumn("moduleId", typeof(int)));

                rpModuleTabs.DataSource = dtAffModules;
                rpModuleTabs.DataBind();
                rpModuleContent.DataSource = dtAffModules;
                rpModuleContent.DataBind();

                //sets the first tab as active
                ((HtmlGenericControl)rpModuleTabs.Items[0].FindControl("tabMod")).Attributes.Add("class", "active");
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showFirstModule", "showModule('" + dtAffModules.Rows[0]["moduleId"].ToString() + "');", true);
                hfSelModule.Value = dtAffModules.Rows[0]["moduleId"].ToString();
            }
            else
            {
                panelSuccess.Visible = false;
                panelSysError.Visible = false;
                panelWarning.Visible = false;
            }

            venuesearch.type = venue_search.RecentType.SESSION;
            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
            venuesearch.venueRefresh += new venue_search.VenueRefresh(refreshVenue);
            dayselect.selectDay += new day_select.SelectDay(selectDay);
        }


        protected void rpModuleContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            Repeater r = e.Item.FindControl("rpProgramme") as Repeater;
            if (r == null) return;

            int moduleId = (int) ((DataRowView)e.Item.DataItem)["moduleId"];

            DataTable dt = dtAffProg.Copy();
            foreach (DataRow dr in dt.Rows) dr["moduleId"] = moduleId;

            r.DataSource = dt;
            r.DataBind();
        }

        protected void rpBatchPills_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataRowView drv = ((DataRowView)e.Item.DataItem);
            HtmlAnchor c = (HtmlAnchor)e.Item.FindControl("pillBatchLnk");
            string id = drv["moduleId"] + "_" + drv["programmeId"] + "_" + drv["programmeBatchId"];
            c.Attributes.Add("onClick", "setSelValue('" + hfCurrSelBatch.ClientID + "', '" + id + "');");
            c.Attributes.Add("href", "#" + id);
        }

        protected void rpProgramme_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            Repeater rPills = e.Item.FindControl("rpBatchPills") as Repeater;
            Repeater rContent = e.Item.FindControl("rpBatchContent") as Repeater;
            //set the current hidden field
            hfCurrSelBatch=((HiddenField)e.Item.FindControl("hfSelBatch"));

            if (rPills == null || rContent == null) return;

            string moduleId = ((DataRowView)e.Item.DataItem)["moduleId"].ToString();
            string progId = ((DataRowView)e.Item.DataItem)["programmeId"].ToString();

            DataRow[] drs = dtAffBatch.Select("programmeId=" + progId);
            DataTable dt = drs.CopyToDataTable<DataRow>();

            foreach (DataRow dr in dt.Rows)
            {
                dr["moduleId"] = moduleId;
            }

            rPills.DataSource = dt;
            rPills.DataBind();
            rContent.DataSource = dt;
            rContent.DataBind();

            //sets the first tab as active
            ((HtmlGenericControl)rPills.Items[0].FindControl("pillBatch")).Attributes.Add("class", "active");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showFirstBatch" + moduleId + progId
                + dt.Rows[0]["programmeBatchId"].ToString(), "showBatch('" + moduleId + "_" + progId + "_" + dt.Rows[0]["programmeBatchId"].ToString() + "');", true);
            hfCurrSelBatch.Value = moduleId + "_" + progId + "_" + dt.Rows[0]["programmeBatchId"].ToString();
        }

        protected void rpBatchContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            int moduleId = (int)((DataRowView)e.Item.DataItem)["moduleId"];
            int batchId = (int)((DataRowView)e.Item.DataItem)["programmeBatchId"];
            int programmeId = (int)((DataRowView)e.Item.DataItem)["programmeId"];

            DataRow[] drs = ((DataTable)rpModuleContent.DataSource).Select("moduleId=" + moduleId);

            bundle_module_new_session ns = ((bundle_module_new_session)e.Item.FindControl("ns"));
            bundle_module_rem_session rs = ((bundle_module_rem_session)e.Item.FindControl("rs"));

            if (drs[0]["chgType"].ToString() == "DEC" || drs[0]["chgType"].ToString() == "REM")
            {
                ns.Visible = false;

                rs.Visible = true;
                hfRemSessionLblNamingContainer.Text += rs.FindControl("lblNumSession").ClientID + ";";
                hfRemSessionGvNamingContainer.Text += rs.FindControl("gvRemovalSession").ClientID + ";";
                hfRemSessionTotalNamingContainer.Text += rs.FindControl("hfTotalSession").ClientID + ";";
                rs.loadSessions(batchId, moduleId, (int)drs[0]["sessionDiff"]);
            }
            else if (drs[0]["chgType"].ToString() == "INC")
            {
                ns.Visible = true;
                hfNewSessionDtNamingContainer.Text += ((Repeater)e.Item.Parent).ClientID + ";";
                hfNewSessionLblNamingContainer.Text += ns.FindControl("lblNumSession").ClientID + ";";
                hfNewSessionRpNamingContainer.Text += ns.FindControl("rpNewSessions").ClientID + ";";
                ns.loadModule(programmeId, batchId, moduleId, DateTime.ParseExact(((DataRowView)e.Item.DataItem)["programmeStartDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture),
                    (int)drs[0]["sessionDiff"], "INC");

                rs.Visible = false;
            }
            else if (drs[0]["chgType"].ToString() == "NEW")
            {
                ns.Visible = true;
                hfNewSessionDtNamingContainer.Text += ((Repeater)e.Item.Parent).ClientID + ";";
                hfNewSessionLblNamingContainer.Text += ns.FindControl("lblNumSession").ClientID + ";";
                hfNewSessionRpNamingContainer.Text += ns.FindControl("rpNewSessions").ClientID + ";";
                ns.loadModule(programmeId, batchId, moduleId, DateTime.ParseExact(((DataRowView)e.Item.DataItem)["programmeStartDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture),
                    (int)drs[0]["sessionDiff"], "NEW");

                rs.Visible = false;
            }
        }

        private void selectDay(string value, string disp)
        {
            HiddenField hf, hfSelBatch;
            StringBuilder sb = new StringBuilder();
            string currModule;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                Repeater rpProg = rMod.FindControl("rpProgramme") as Repeater;
                if (rpProg == null) continue;

                foreach (RepeaterItem rProg in rpProg.Items)
                {
                    hfSelBatch = rProg.FindControl("hfSelBatch") as HiddenField;
                    if (hfSelBatch != null && hfSelBatch.Value != "") sb.Append("showBatch('" + hfSelBatch.Value + "');");

                    hf = rProg.FindControl("hfProgId") as HiddenField;
                    if (hf == null || hf.Value != hfDaySelProg.Value || currModule != hfDaySelModule.Value) continue;

                    Repeater rpBatch = rProg.FindControl("rpBatchContent") as Repeater;
                    if (rpBatch == null) continue;

                    foreach (RepeaterItem rBatch in rpBatch.Items)
                    {
                        hf = rBatch.FindControl("hfBatchId") as HiddenField;
                        if (hf == null || hf.Value != hfDaySelBatch.Value) continue;

                        bundle_module_new_session ns = rBatch.FindControl("ns") as bundle_module_new_session;
                        ((TextBox)ns.FindControl("tbDay")).Text = disp;
                        ((HiddenField)ns.FindControl("hfDay")).Value = value;

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrModule", "showModule('" + hfDaySelModule.Value + "');", true);
                    }
                }
            }

            //also need to show batch for each programme
            if (sb.Length != 0)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrBatches", sb.ToString(), true);
        }

        private void refreshVenue()
        {
            showCurrentTabsNPills();
        }

        private void selectVenue(string id, string location)
        {
            HiddenField hf, hfSelBatch;
            StringBuilder sb = new StringBuilder();
            string currModule;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                Repeater rpProg = rMod.FindControl("rpProgramme") as Repeater;
                if (rpProg == null) continue;

                foreach (RepeaterItem rProg in rpProg.Items)
                {
                    hfSelBatch = rProg.FindControl("hfSelBatch") as HiddenField;
                    if (hfSelBatch != null && hfSelBatch.Value != "") sb.Append("showBatch('" + hfSelBatch.Value + "');");

                    hf = rProg.FindControl("hfProgId") as HiddenField;
                    if (hf == null || hf.Value != hfVenueSelProg.Value || currModule != hfVenueSelModule.Value) continue;

                    Repeater rpBatch = rProg.FindControl("rpBatchContent") as Repeater;
                    if (rpBatch == null) continue;

                    foreach (RepeaterItem rBatch in rpBatch.Items)
                    {
                        hf = rBatch.FindControl("hfBatchId") as HiddenField;
                        if (hf == null || hf.Value != hfVenueSelBatch.Value) continue;

                        bundle_module_new_session ns = rBatch.FindControl("ns") as bundle_module_new_session;
                        ns.selectVenue(int.Parse(hfVenueSelSession.Value), id, location);

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrModule", "showModule('" + hfVenueSelModule.Value + "');", true);
                    }
                }
            }

            //also need to show batch for each programme
            if (sb.Length != 0)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrBatches", sb.ToString(), true);
        }

        protected void lbtnRefreshVenue_Click(object sender, EventArgs e)
        {
            HiddenField hf, hfSelBatch;
            StringBuilder sb = new StringBuilder();
            string currModule;

            //get selected module
            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                hf = rMod.FindControl("hfModuleId") as HiddenField;
                currModule = hf.Value;

                Repeater rpProg = rMod.FindControl("rpProgramme") as Repeater;
                if (rpProg == null) continue;

                foreach (RepeaterItem rProg in rpProg.Items)
                {
                    hfSelBatch = rProg.FindControl("hfSelBatch") as HiddenField;
                    if (hfSelBatch != null && hfSelBatch.Value != "") sb.Append("showBatch('" + hfSelBatch.Value + "');");

                    hf = rProg.FindControl("hfProgId") as HiddenField;
                    if (hf == null || hf.Value != hfVenueSelProg.Value || currModule != hfVenueSelModule.Value) continue;

                    Repeater rpBatch = rProg.FindControl("rpBatchContent") as Repeater;
                    if (rpBatch == null) continue;

                    foreach (RepeaterItem rBatch in rpBatch.Items)
                    {
                        hf = rBatch.FindControl("hfBatchId") as HiddenField;
                        if (hf == null || hf.Value != hfVenueSelBatch.Value) continue;

                        bundle_module_new_session ns = rBatch.FindControl("ns") as bundle_module_new_session;
                        ns.refreshVenueAva(int.Parse(hfVenueSelSession.Value));

                        Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrModule", "showModule('" + hfVenueSelModule.Value + "');", true);
                    }
                }
            }

            //also need to show batch for each programme
            if (sb.Length != 0)
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrBatches", sb.ToString(), true);
        }

        public void showCurrentTabsNPills()
        {
            HiddenField hfSelBatch;
            StringBuilder sb = new StringBuilder();

            sb.Append("showModule('" + hfSelModule.Value + "');");

            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                Repeater rpProg = rMod.FindControl("rpProgramme") as Repeater;
                if (rpProg == null) continue;

                foreach (RepeaterItem rProg in rpProg.Items)
                {
                    hfSelBatch = rProg.FindControl("hfSelBatch") as HiddenField;
                    if (hfSelBatch != null && hfSelBatch.Value != "") sb.Append("showBatch('" + hfSelBatch.Value + "');");
                }
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showCurrModuleNBatches", sb.ToString(), true);
        }

        protected void btnBack_Click(object sender, EventArgs e)
        {
            hfRtnMode.Value = "BACK";
            Server.Transfer(bundle_edit.PAGE_NAME, true);
        }

        protected void btnConfirm_Click(object sender, EventArgs e)
        {
            createDataTablesForUpdate();

            //retrieve user input
            string chgType;
            int moduleId, programmeId, programmeBatchId;
            DataRow dr;
            bool hasError = false;
            DropDownList ddl;
            lblSysError.Text = "";

            foreach (RepeaterItem rMod in rpModuleContent.Items)
            {
                chgType = ((HiddenField)rMod.FindControl("hfChgType")).Value;
                moduleId = int.Parse(((HiddenField)rMod.FindControl("hfModuleId")).Value);

                Repeater rpProg = rMod.FindControl("rpProgramme") as Repeater;
                if (rpProg == null) continue;

                foreach(RepeaterItem rProg in rpProg.Items)
                {
                    programmeId = int.Parse(((HiddenField)rProg.FindControl("hfProgId")).Value);

                    Repeater rpBatch = rProg.FindControl("rpBatchContent") as Repeater;
                    if (rpBatch == null) continue;

                    foreach(RepeaterItem rBatch in rpBatch.Items)
                    {
                        programmeBatchId = int.Parse(((HiddenField)rBatch.FindControl("hfBatchId")).Value);

                        if (chgType == "NEW")
                        {
                            bundle_module_new_session ns = rBatch.FindControl("ns") as bundle_module_new_session;

                            dr = dtNewModule.NewRow();
                            dr["moduleId"] = moduleId;
                            dr["programmeId"] = programmeId;
                            dr["programmeBatchId"] = programmeBatchId;
                            dr["day"] = ((HiddenField)ns.FindControl("hfDay")).Value;
                            dr["startDate"] = DateTime.ParseExact(((TextBox)ns.FindControl("tbModDtFrm")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                            dr["endDate"] = DateTime.ParseExact(((TextBox)ns.FindControl("tbModDtTo")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                            dr["trainerUserId1"] = ((DropDownList)ns.FindControl("ddlTrainer1")).SelectedValue;
                            dr["assessorUserId"] = ((DropDownList)ns.FindControl("ddlAssessor")).SelectedValue;

                            ddl = (DropDownList)ns.FindControl("ddlTrainer2");
                            dr["trainerUserId2"] = ddl.SelectedIndex > 0 ? ddl.SelectedValue : (object)DBNull.Value;

                            dtNewModule.Rows.Add(dr);

                            if (!getNewSessions((Repeater)ns.FindControl("rpNewSessions"), moduleId, programmeId, programmeBatchId, true) && hasError == false)
                                hasError = true;
                        }
                        else if (chgType == "INC")
                        {
                            bundle_module_new_session ns = rBatch.FindControl("ns") as bundle_module_new_session;
                            if (!getNewSessions((Repeater)ns.FindControl("rpNewSessions"), moduleId, programmeId, programmeBatchId, false) && hasError == false)
                                hasError = true;
                        }
                        else if (chgType == "DEC")
                        {
                            bundle_module_rem_session rs = rBatch.FindControl("rs") as bundle_module_rem_session;
                            foreach (GridViewRow r in ((GridView)rs.FindControl("gvRemovalSession")).Rows)
                            {
                                if (((CheckBox)r.Cells[1].FindControl("cb")).Checked)
                                {
                                    dr = dtRemSession.NewRow();
                                    dr["moduleId"] = moduleId;
                                    dr["programmeId"] = programmeId;
                                    dr["programmeBatchId"] = programmeBatchId;
                                    dr["sessionId"] = int.Parse(r.Cells[0].Text);
                                    dr["sessionDate"] = DateTime.ParseExact(r.Cells[4].Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                                    dr["sessionPeriod"] = r.Cells[1].Text;
                                    dtRemSession.Rows.Add(dr);
                                }
                            }
                        }
                        else if (chgType == "REM")
                        {
                            bundle_module_rem_session rs = rBatch.FindControl("rs") as bundle_module_rem_session;
                            foreach (GridViewRow r in ((GridView)rs.FindControl("gvRemovalSession")).Rows)
                            {
                                dr = dtRemModule.NewRow();
                                dr["moduleId"] = moduleId;
                                dr["programmeId"] = programmeId;
                                dr["programmeBatchId"] = programmeBatchId;
                                dr["sessionId"] = int.Parse(r.Cells[0].Text);
                                dr["sessionDate"] = DateTime.ParseExact(r.Cells[4].Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                                dr["sessionPeriod"] = r.Cells[1].Text;
                                dtRemModule.Rows.Add(dr);
                            }
                        }
                    }
                }
            }


            if (hasError)
            {
                panelSysError.Visible = true;
                showCurrentTabsNPills();
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
                return;
            }

            Tuple<bool, string> status = bm.updateBundle(int.Parse(hfBundleId.Value), hfBundleType.Value, DateTime.ParseExact(hfEffectiveDate.Value, "dd MMM yyyy", CultureInfo.InvariantCulture),
                BundleCost, LoginID, dtAddSession, dtRemSession, dtNewModule, dtRemModule, ViewState[DATA_KEY] as DataTable);
            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                hfRtnMode.Value = "SAVED";
                Server.Transfer(bundle_edit.PAGE_NAME, true);
            }
            else
            {
                lblSysError.Text = status.Item2;
                panelSysError.Visible = true;
            }

            showCurrentTabsNPills();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

        private bool getNewSessions(Repeater rp, int moduleId, int programmeId, int programmeBatchId, bool isNewModule)
        {
            bool success = true;

            foreach (RepeaterItem rSession in rp.Items)
            {
                DataRow dr = dtAddSession.NewRow();
                dr["moduleId"] = moduleId;
                dr["programmeId"] = programmeId;
                dr["programmeBatchId"] = programmeBatchId;
                dr["sessionDate"] = DateTime.ParseExact(((TextBox)rSession.FindControl("tbNewSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                dr["sessionPeriod"] = ((DropDownList)rSession.FindControl("ddlNewSessionPeriod")).SelectedValue;
                dr["venueId"] = ((HiddenField)rSession.FindControl("hfNewSessionVenueId")).Value;
                dr["isNewModule"] = isNewModule;

                //check if any duplicate session date/period within the module of the batch
                if (dtAddSession.Select("moduleId=" + moduleId + " and programmeId=" + programmeId + " and programmeBatchId=" + programmeBatchId
                    + " and sessionDate=#" + ((DateTime)dr["sessionDate"]).ToString("MM/dd/yyyy") + "# and sessionPeriod='" + dr["sessionPeriod"].ToString() + "'").Length > 0)
                {
                    success = false;
                    lblSysError.Text += "<li>Duplicated session date " + ((DateTime)dr["sessionDate"]).ToString("dd MMM yyyy") + " found.</li>";
                }

                //check if any duplicate venue (same session date/period) within all new sessions
                if (dtAddSession.Select("sessionDate=#" + ((DateTime)dr["sessionDate"]).ToString("MM/dd/yyyy")
                    + "# and sessionPeriod='" + dr["sessionPeriod"].ToString() + "' and venueId='" + dr["venueId"].ToString() + "'").Length > 0)
                {
                    success = false;
                    lblSysError.Text = "<li>Duplicate venue (" + ((TextBox)rSession.FindControl("tbNewSessionVenue")).Text + ") used in sessions.";
                }

                dtAddSession.Rows.Add(dr);
            }

            return success;
        }

        private void createDataTablesForUpdate()
        {
            //create datatable to store info for new session on existing module
            dtAddSession = new DataTable();
            dtAddSession.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtAddSession.Columns.Add(new DataColumn("programmeId", typeof(int)));
            dtAddSession.Columns.Add(new DataColumn("programmeBatchId", typeof(int)));
            dtAddSession.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtAddSession.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));
            dtAddSession.Columns.Add(new DataColumn("venueId", typeof(string)));
            dtAddSession.Columns.Add(new DataColumn("isNewModule", typeof(bool)));

            //create datatable to store info for remove session on existing module
            dtRemSession = new DataTable();
            dtRemSession.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtRemSession.Columns.Add(new DataColumn("programmeId", typeof(int)));
            dtRemSession.Columns.Add(new DataColumn("programmeBatchId", typeof(int)));
            dtRemSession.Columns.Add(new DataColumn("sessionId", typeof(int)));
            dtRemSession.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtRemSession.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));

            //create datatable to store info for new module
            dtNewModule = new DataTable();
            dtNewModule.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtNewModule.Columns.Add(new DataColumn("programmeId", typeof(int)));
            dtNewModule.Columns.Add(new DataColumn("programmeBatchId", typeof(int)));
            dtNewModule.Columns.Add(new DataColumn("day", typeof(string)));
            dtNewModule.Columns.Add(new DataColumn("startDate", typeof(DateTime)));
            dtNewModule.Columns.Add(new DataColumn("endDate", typeof(DateTime)));
            dtNewModule.Columns.Add(new DataColumn("trainerUserId1", typeof(int)));
            dtNewModule.Columns.Add(new DataColumn("trainerUserId2", typeof(int)));
            dtNewModule.Columns.Add(new DataColumn("assessorUserId", typeof(int)));

            dtRemModule = new DataTable();
            dtRemModule.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dtRemModule.Columns.Add(new DataColumn("programmeId", typeof(int)));
            dtRemModule.Columns.Add(new DataColumn("programmeBatchId", typeof(int)));
            dtRemModule.Columns.Add(new DataColumn("sessionId", typeof(int)));
            dtRemModule.Columns.Add(new DataColumn("sessionDate", typeof(DateTime)));
            dtRemModule.Columns.Add(new DataColumn("sessionPeriod", typeof(string)));
        }

        public void showWarning(string msg)
        {
            lblWaring.Text = msg;
            panelWarning.Visible = true;
        }

    }
}