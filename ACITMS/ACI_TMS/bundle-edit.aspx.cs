using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class bundle_edit : BasePage
    {
        public const string PAGE_NAME = "bundle-edit.aspx";
        public const string QUERY_ID = "id";

        private const string DATA_KEY = "dtSelModules";

        public int BundleId
        {
            get { return int.Parse(hfBundleId.Value); }
        }

        public string BundleCode
        {
            get { return lbBundleCode.Text; }
        }

        public string BundleType
        {
            get { return ddlBundleType.SelectedValue; }
        }

        public string BundleEffDate
        {
            get { return lbEffectiveDate.Text; }
        }

        public decimal BundleCost
        {
            get { return decimal.Parse(lbTotalCost.Text); }
        }

        public DataTable BundleModules
        {
            get { return ViewState[DATA_KEY] as DataTable; }
        }

        public bundle_edit()
            : base(PAGE_NAME, AccessRight_Constance.BUNDLE_EDIT, bundle_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            modulesearch.selectModule += new module_search.SelectModule(loadModuleDetails);

            if (!IsPostBack)
            {
                populateBundleTypes();

                //check if return from bundle edit session page from somewhere else
                if(PreviousPage is bundle_edit_sessions)
                {
                    PrevPage += "?" + bundle_view.QUERY_ID + "=" + ((bundle_edit_sessions)PreviousPage).BundleId;

                    if (((bundle_edit_sessions)PreviousPage).RtnMode == "BACK") 
                    {
                        hfBundleId.Value = ((bundle_edit_sessions)PreviousPage).BundleId.ToString();
                        lbBundleCode.Text = ((bundle_edit_sessions)PreviousPage).BundleCode;
                        lbEffectiveDate.Text = ((bundle_edit_sessions)PreviousPage).BundleEffDate;
                        ddlBundleType.SelectedValue = ((bundle_edit_sessions)PreviousPage).BundleType;
                        bindModuleGridView(((bundle_edit_sessions)PreviousPage).BundleModules);
                    }
                    else if (((bundle_edit_sessions)PreviousPage).RtnMode == "SAVED")
                    {
                        loadBundle(((bundle_edit_sessions)PreviousPage).BundleId);
                        lblSuccess.Text = "Bundle and session information saved successfully.";
                        panelSuccess.Visible = true;
                    }
                }
                else
                {
                    //load selected bundle details
                    if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID].ToString().Trim() == "")
                    {
                        redirectToErrorPg("Missing bundle information.", bundle_management.PAGE_NAME);
                        return;
                    }

                    PrevPage += "?" + bundle_view.QUERY_ID + "=" + Request.QueryString[QUERY_ID];
                    int bundleId;
                    if (!int.TryParse(HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]), out bundleId))
                    {
                        redirectToErrorPg("Invalid bundle information.", bundle_management.PAGE_NAME);
                        return;
                    }

                    //check if any of the batch using the bundle has already started class, if so cannot edit bundle
                    if ((new Bundle_Management()).checkBatchStarted(bundleId))
                    {
                        redirectToErrorPg("Unable to edit bundle as one or more batch using the bundle has already started.");
                        return;
                    }

                    loadBundle(bundleId);
                }
            }
            else
            {
                PrevPage += "?" + bundle_view.QUERY_ID + "=" + hfBundleId.Value;
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        public void populateBundleTypes()
        {
            DataTable dt = (new Bundle_Management()).getBundleTypes();
            ddlBundleType.DataSource = dt;
            ddlBundleType.DataBind();
            ddlBundleType.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadModuleDetails(int moduleId, string title)
        {
            DataTable dt = (new Module_Management()).getModule(moduleId);

            if (dt == null || dt.Rows.Count == 0)
            {
                lblError.Text = "Unable to load module details.";
                panelError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
                return;
            }

            tbModuleCode.Text = dt.Rows[0]["moduleCode"].ToString();
            lbModule.Text = dt.Rows[0]["moduleTitle"].ToString();
            lbModuleLevel.Text = dt.Rows[0]["moduleLevel"].ToString();
            lbModuleEffectiveDate.Text = dt.Rows[0]["moduleEffectDate"].ToString();
            lbModuleCost.Text = dt.Rows[0]["moduleCost"].ToString();
            lbModuleTrainingHour.Text = dt.Rows[0]["moduleTrainingHour"].ToString();
            lbModuleVersion.Text = dt.Rows[0]["moduleVersion"].ToString();
            tbNumOfSession.Text = "";
            hdSelModuleId.Value = moduleId.ToString();

            btnSaveModule.Text = "Add";
            btnClearModule.Text = "Clear";
        }

        //private void loadBundle(string bundleCode)
        private void loadBundle(int bundleId)
        {
            Bundle_Management bm = new Bundle_Management();

            Tuple<string, string, string, string, decimal, bool, DataTable> b = bm.getBundle(bundleId);
            if (b == null)
            {
                redirectToErrorPg("Error retrieving bundle details.");
                return;
            }

            if (!b.Item6)
            {
                redirectToErrorPg("Bundle is already removed, unable to edit.");
                return;
            }

            hfBundleId.Value = bundleId.ToString();
            lbBundleCode.Text = b.Item1;
            lbEffectiveDate.Text = b.Item4;
            lbTotalCost.Text = b.Item5.ToString();

            gvModule.EditIndex = -1;
            bindModuleGridView(b.Item7);

            ddlBundleType.SelectedValue = b.Item2;
        }

        private void bindModuleGridView(DataTable dt)
        {
            ViewState[DATA_KEY] = dt;
            gvModule.DataSource = dt;
            gvModule.Columns[0].Visible = true;
            gvModule.Columns[1].Visible = true;
            gvModule.Columns[2].Visible = true;
            gvModule.DataBind();
            gvModule.Columns[0].Visible = false;
            gvModule.Columns[1].Visible = false;

            if (dt.Rows.Count == 1)
            {
                //if only 1 module, hide the order column
                gvModule.Columns[2].Visible = false;
            }
            else if (dt.Rows.Count > 1)
            {
                //hide the up button and the break of the 1st row
                gvModule.Rows[0].Cells[2].FindControl("lbtnUp").Visible = false;
                gvModule.Rows[0].Cells[2].FindControl("lbNewLine").Visible = false;
                gvModule.Rows[0].Cells[1].FindControl("lbSpace").Visible = false;
                //hide the down button and the break of the last row
                gvModule.Rows[dt.Rows.Count - 1].Cells[2].FindControl("lbtnDown").Visible = false;
                gvModule.Rows[dt.Rows.Count - 1].Cells[2].FindControl("lbNewLine").Visible = false;
                gvModule.Rows[dt.Rows.Count - 1].Cells[1].FindControl("lbSpace").Visible = false;
            }
        }

        protected void gvModule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            DataTable dt = ViewState[DATA_KEY] as DataTable;

            if (e.CommandName == "editModule")
            {
                DataRow[] drs = dt.Select("moduleId=" + e.CommandArgument.ToString());
                if (drs == null || drs.Length == 0) return;

                hdSelModuleId.Value = e.CommandArgument.ToString();
                tbModuleCode.Text = drs[0]["moduleCode"].ToString();
                lnkbtnSearchModule.Visible = false;
                lbModule.Text = drs[0]["moduleTitle"].ToString();
                lbModuleVersion.Text = drs[0]["moduleVersion"].ToString();
                tbNumOfSession.Text = drs[0]["ModuleNumOfSession"].ToString();
                lbModuleLevel.Text = drs[0]["moduleLevel"].ToString();
                lbModuleTrainingHour.Text = drs[0]["moduleTrainingHour"].ToString();
                lbModuleEffectiveDate.Text = drs[0]["moduleEffectDate"].ToString();
                lbModuleCost.Text = drs[0]["moduleCost"].ToString();

                btnClearModule.Text = "Cancel";
                btnSaveModule.Text = "Save";
            }
            else if (e.CommandName == "moveUp")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                if (index == 0) return;
                DataRow drOld = dt.Rows[index];
                DataRow drNew = dt.NewRow();
                copyRow(drOld, drNew);

                dt.Rows.RemoveAt(index);
                dt.Rows.InsertAt(drNew, index - 1);

                bindModuleGridView(dt);
            }
            else if (e.CommandName == "moveDown")
            {
                int index = int.Parse(e.CommandArgument.ToString());
                if (index == dt.Rows.Count - 1) return;
                DataRow drOld = dt.Rows[index];
                DataRow drNew = dt.NewRow();
                copyRow(drOld, drNew);

                dt.Rows.RemoveAt(index);
                dt.Rows.InsertAt(drNew, index + 1);

                bindModuleGridView(dt);
            }
        }

        private void copyRow(DataRow drOld, DataRow drNew)
        {
            drNew["bundleModuleId"] = drOld["bundleModuleId"];
            drNew["moduleId"] = drOld["moduleId"];
            drNew["moduleCode"] = drOld["moduleCode"];
            drNew["moduleVersion"] = drOld["moduleVersion"];
            drNew["moduleLevel"] = drOld["moduleLevel"];
            drNew["moduleTitle"] = drOld["moduleTitle"];
            drNew["moduleEffectDate"] = drOld["moduleEffectDate"];
            drNew["ModuleNumOfSession"] = drOld["ModuleNumOfSession"];
            drNew["moduleTrainingHour"] = drOld["moduleTrainingHour"];
            drNew["moduleCost"] = drOld["moduleCost"];
        }

        protected void btnClearModule_Click(object sender, EventArgs e)
        {
            tbModuleCode.Text = "";
            tbNumOfSession.Text = "";
            lbModule.Text = "";
            lbModuleEffectiveDate.Text = "";
            lbModuleLevel.Text = "";
            lbModuleTrainingHour.Text = "";
            lbModuleVersion.Text = "";
            lbModuleCost.Text = "";

            hdSelModuleId.Value = "";
            btnClearModule.Text = "Clear";
            btnSaveModule.Text = "Add";
            lnkbtnSearchModule.Visible = true;
        }

        protected void btnSaveModule_Click(object sender, EventArgs e)
        {
            DataTable dt = ViewState[DATA_KEY] as DataTable;

            if (btnSaveModule.Text.ToUpper() == "SAVE")
            {
                //update existing
                DataRow[] drs = dt.Select("moduleId=" + hdSelModuleId.Value);
                if (drs == null || drs.Length == 0)
                {
                    lblError.Text = "Error updating module.";
                    panelError.Visible = true;
                    return;
                }

                drs[0]["ModuleNumOfSession"] = tbNumOfSession.Text;
            }
            else
            {
                //check for duplicates
                DataRow[] drs = dt.Select("moduleId=" + hdSelModuleId.Value);
                if (drs != null && drs.Length > 0)
                {
                    lblError.Text = "Module has already been added.";
                    panelError.Visible = true;
                    return;
                }

                //add new
                DataRow dr = dt.NewRow();
                dr["moduleId"] = hdSelModuleId.Value;
                dr["moduleCode"] = tbModuleCode.Text;
                dr["moduleVersion"] = lbModuleVersion.Text;
                dr["moduleLevel"] = lbModuleLevel.Text;
                dr["moduleTitle"] = lbModule.Text;
                dr["moduleEffectDate"] = lbModuleEffectiveDate.Text;
                dr["ModuleNumOfSession"] = int.Parse(tbNumOfSession.Text);
                dr["moduleTrainingHour"] = decimal.Parse(lbModuleTrainingHour.Text);
                dr["moduleCost"] = decimal.Parse(lbModuleCost.Text);
                dt.Rows.Add(dr);

                //determine the bundle effective date which is the latest modules eff date
                updateBundleEffDate(dt);

                //determine total module cost
                lbTotalCost.Text = (decimal.Parse(lbTotalCost.Text) + decimal.Parse(lbModuleCost.Text)).ToString(); 
            }

            btnClearModule_Click(null, null);
            bindModuleGridView(dt);
        }

        private void updateBundleEffDate(DataTable dt)
        {
            if (dt.Rows.Count == 0)
            {
                lbEffectiveDate.Text = "";
                return;
            }

            DateTime maxDt = DateTime.ParseExact(dt.Rows[0]["moduleEffectDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture);
            DateTime d;
            foreach (DataRow r in dt.Rows)
            {
                d = DateTime.ParseExact(r["moduleEffectDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture);
                if (d.CompareTo(maxDt) > 0) maxDt = DateTime.ParseExact(r["moduleEffectDate"].ToString(), "dd MMM yyyy", CultureInfo.InvariantCulture);
            }
            lbEffectiveDate.Text = maxDt.ToString("dd MMM yyyy");
        }

        protected void btnRemMod_Click(object sender, EventArgs e)
        {
            if (hdSelModuleId.Value == "") return;

            DataTable dt = ViewState[DATA_KEY] as DataTable;
            DataRow[] drs = dt.Select("moduleId=" + hdSelModuleId.Value);
            if (drs == null || drs.Length == 0) return;

            //update module cost
            lbTotalCost.Text = (decimal.Parse(lbTotalCost.Text) - (decimal)drs[0]["moduleCost"]).ToString();

            dt.Rows.Remove(drs[0]);

            //check if bundle effective date requires update
            updateBundleEffDate(dt);

            bindModuleGridView(dt);

            btnClearModule_Click(null, null);
        }

        protected void btnClearBundle_Click(object sender, EventArgs e)
        {
            //revert back to original data
            loadBundle(int.Parse(hfBundleId.Value));
        }

        protected void btnSaveBundle_Click(object sender, EventArgs e)
        {
            //must have at least 1 module
            DataTable dt = ViewState[DATA_KEY] as DataTable;

            if (dt.Rows.Count == 0)
            {
                lblError.Text = "Must select at least 1 module.";
                panelError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
                return;
            }

            Bundle_Management bm = new Bundle_Management();

            Tuple<int, string> status = bm.updateBundle(int.Parse(hfBundleId.Value), ddlBundleType.SelectedValue,
                DateTime.ParseExact(lbEffectiveDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                decimal.Parse(lbTotalCost.Text), ViewState[DATA_KEY] as DataTable, LoginID);

            if (status.Item1 == -1)
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
            else if (status.Item1 == 0)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                Server.Transfer(bundle_edit_sessions.PAGE_NAME, true);
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }
    }
}