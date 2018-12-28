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
    public partial class module_edit : BasePage
    {
        public const string PAGE_NAME = "module-edit.aspx";
        public const string QUERY_ID = "mId";
        public const string NEW_VER = "newVer";

        private Module_Management mm = new Module_Management();
        private Bundle_Management bm = new Bundle_Management();

        public module_edit()
            : base(PAGE_NAME, AccessRight_Constance.MODULE_VIEW, module_view.PAGE_NAME)
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing module information.", module_management.PAGE_NAME);
                    return;
                }

                PrevPage = PrevPage + "?" + module_view.QUERY_ID + "=" + Request.QueryString[QUERY_ID];
                hfModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);
                loadModuleDetails();
            }
            else
            {
                PrevPage = PrevPage + "?" + module_view.QUERY_ID + "=" + HttpUtility.UrlEncode(hfModuleId.Value);
                panelSysError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadModuleDetails()
        {
            DataTable dtModuleStructure = mm.getModule(Convert.ToInt32(hfModuleId.Value));
            ViewState["dtModule"] = dtModuleStructure;

            if (dtModuleStructure == null || dtModuleStructure.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving module details.");
                return;
            }

            foreach (DataRow dr in dtModuleStructure.Rows)
            {
                if (Request.QueryString[NEW_VER] == "Y")
                {
                    lbModuleVersionValue.Text = dr["maxModVer"].ToString();
                }
                else
                {
                    lbModuleVersionValue.Text = dr["moduleVersion"].ToString();
                }

                lbModuleCodeValue.Text = dr["moduleCode"].ToString();
                tbModuleTitleValue.Text = dr["moduleTitle"].ToString();
                tbModuleLevelValue.Text = dr["moduleLevel"].ToString();
                tbModuleCreditValue.Text = dr["moduleCredit"].ToString();
                tbModuleTrgHrValue.Text = dr["moduleTrainingHour"].ToString();
                tbModuleCostValue.Text = dr["moduleCost"].ToString();
                tbModuleEffectiveDateValue.Text = Convert.ToDateTime(dr["moduleEffectDate"]).ToString("dd MMM yyyy");
                tbWSQCompetencyCodeValue.Text = dr["WSQCompetencyCode"].ToString();
                tbModuleDescriptionValue.Text = dr["moduleDescription"].ToString();
            }

            if (Request.QueryString[NEW_VER] == "N")
            {
                if (mm.validateModuleUsed(Convert.ToInt32(hfModuleId.Value)))
                {
                    tbModuleCostValue.Enabled = false;
                    tbModuleEffectiveDateValue.Enabled = false;
                }
            }
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Request.QueryString[NEW_VER] == "Y")
            {
                updateNew();
            }
            else
            {
                updateCurrent();
            }
        }

        private void updateNew()
        {
            Tuple<bool, string, int> updateModule = mm.updateModuleNewVer(lbModuleCodeValue.Text, tbModuleTitleValue.Text, Int32.Parse(lbModuleVersionValue.Text), tbModuleLevelValue.Text, Convert.ToInt32(tbModuleCreditValue.Text),
            decimal.Parse(tbModuleTrgHrValue.Text), Convert.ToDecimal(tbModuleCostValue.Text), DateTime.ParseExact(tbModuleEffectiveDateValue.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
            tbWSQCompetencyCodeValue.Text, tbModuleDescriptionValue.Text, LoginID);

            if (updateModule.Item1)
            {
                hfModuleId.Value = updateModule.Item3.ToString();
                lblSuccess.Text = updateModule.Item2;
                panelSuccess.Visible = true;
            }

            else
            {
                lblSysError.Text = updateModule.Item2;
                panelSysError.Visible = true;
            }
        }

        private void updateCurrent()
        {
            Tuple<bool, string> updateModule = mm.updateModule(Convert.ToInt32(hfModuleId.Value), tbModuleTitleValue.Text, Convert.ToInt32(lbModuleVersionValue.Text), tbModuleLevelValue.Text, Convert.ToInt32(tbModuleCreditValue.Text),
             decimal.Parse(tbModuleTrgHrValue.Text), Convert.ToDecimal(tbModuleCostValue.Text), DateTime.ParseExact(tbModuleEffectiveDateValue.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
            tbWSQCompetencyCodeValue.Text, tbModuleDescriptionValue.Text, LoginID);

            if (updateModule.Item1)
            {
                lblSuccess.Text = updateModule.Item2;
                panelSuccess.Visible = true;
            }

            else
            {
                lblSysError.Text = updateModule.Item2;
                panelSysError.Visible = true;
            }
        }

        protected void btnClearModule_Click(object sender, EventArgs e)
        {
            loadModuleDetails();
        }

    }
}