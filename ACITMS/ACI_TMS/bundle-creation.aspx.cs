using System;
using LogicLayer;
using GeneralLayer;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace ACI_TMS
{
    public partial class bundle_creation : BasePage
    {
        public const string PAGE_NAME="bundle-creation.aspx";
        private const string DATA_KEY = "dtSelModules";

        public bundle_creation()
            : base(PAGE_NAME, AccessRight_Constance.BUNDLE_NEW, bundle_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            modulesearch.selectModule += new module_search.SelectModule(loadModuleDetails);

            if (!IsPostBack)
            {
                populateBundleTypes();
                createEmptyModuleTable();
            }
            else
            {
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

        private void createEmptyModuleTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleCode", typeof(string)));
            dt.Columns.Add(new DataColumn("moduleVersion", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleLevel", typeof(string)));
            dt.Columns.Add(new DataColumn("moduleTitle", typeof(string)));
            dt.Columns.Add(new DataColumn("moduleEffectDate", typeof(string)));
            dt.Columns.Add(new DataColumn("ModuleNumOfSession", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleTrainingHour", typeof(decimal)));
            dt.Columns.Add(new DataColumn("moduleCost", typeof(decimal)));

            bindModuleGridView(dt);
        }

        private void bindModuleGridView(DataTable dt)
        {
            ViewState[DATA_KEY] = dt;
            gvModule.DataSource = dt;
            gvModule.Columns[0].Visible = true;
            gvModule.Columns[1].Visible = true;
            gvModule.DataBind();
            gvModule.Columns[0].Visible = false;

            if (dt.Rows.Count == 1)
            {
                //if only 1 module, hide the order column
                gvModule.Columns[1].Visible = false;
            }
            else if (dt.Rows.Count > 1)
            {
                //hide the up button and the break of the 1st row
                gvModule.Rows[0].Cells[1].FindControl("lbtnUp").Visible = false;
                gvModule.Rows[0].Cells[1].FindControl("lbNewLine").Visible = false;
                gvModule.Rows[0].Cells[1].FindControl("lbSpace").Visible = false;
                //hide the down button and the break of the last row
                gvModule.Rows[dt.Rows.Count - 1].Cells[1].FindControl("lbtnDown").Visible = false;
                gvModule.Rows[dt.Rows.Count - 1].Cells[1].FindControl("lbNewLine").Visible = false;
                gvModule.Rows[dt.Rows.Count - 1].Cells[1].FindControl("lbSpace").Visible = false;
            }
        }

        private void loadModuleDetails(int moduleId, string title)
        {
            DataTable dt = (new Module_Management()).getModule(moduleId);

            if (dt == null || dt.Rows.Count==0)
            {
                lblError.Text = "Unable to load module details.";
                panelError.Visible = true;
                return;
            }

            tbModuleCode.Text = dt.Rows[0]["moduleCode"].ToString();
            lbModule.Text = dt.Rows[0]["moduleTitle"].ToString();
            lbModuleLevel.Text = dt.Rows[0]["moduleLevel"].ToString();
            lbModuleEffectiveDate.Text = dt.Rows[0]["moduleEffectDate"].ToString();
            lbModuleTrainingHour.Text = dt.Rows[0]["moduleTrainingHour"].ToString();
            lbModuleVersion.Text = dt.Rows[0]["moduleVersion"].ToString();
            lbModuleCost.Text = dt.Rows[0]["moduleCost"].ToString();
            tbNumOfSession.Text = "";
            hdSelModuleId.Value = moduleId.ToString();

            btnSaveModule.Text = "Add";
            btnClearModule.Text = "Clear";
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

            if (btnSaveModule.Text.ToUpper()=="SAVE")   
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
            lbTotalCost.Text = (decimal.Parse(lbTotalCost.Text) - (decimal) drs[0]["moduleCost"]).ToString();

            dt.Rows.Remove(drs[0]);

            //check if bundle effective date requires update
            updateBundleEffDate(dt);

            bindModuleGridView(dt);

            btnClearModule_Click(null, null);
        }

        protected void btnClearBundle_Click(object sender, EventArgs e)
        {
            tbBundleCode.Text = "";
            lbEffectiveDate.Text = "";
            lbTotalCost.Text = "0";
            ddlBundleType.SelectedIndex = 0;
            createEmptyModuleTable();

            btnClearModule_Click(null, null);

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

        protected void btnCreateBundle_Click(object sender, EventArgs e)
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

            Tuple<bool, string> t = (new Bundle_Management()).createBundle(tbBundleCode.Text, ddlBundleType.SelectedValue,
                DateTime.ParseExact(lbEffectiveDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), decimal.Parse(lbTotalCost.Text), dt, LoginID.ToString());

            if (t.Item1)
            {
                //success
                lblSuccess.Text = t.Item2;
                panelSuccess.Visible = true;
                btnClearBundle_Click(null, null);
            }
            else
            {
                lblError.Text = t.Item2;
                panelError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }
    }
}