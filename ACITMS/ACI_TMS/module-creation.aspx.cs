using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;
using System.Drawing;

namespace ACI_TMS
{
    public partial class module_creation : BasePage
    {
        public const string PAGE_NAME = "module-creation.aspx";

        private Module_Management mm = new Module_Management();

        public module_creation()
            : base(PAGE_NAME, AccessRight_Constance.MODULE_NEW, module_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbModuleCodeValue.Text = "";
            tbModuleTitleValue.Text = "";
            tbModuleLevelValue.Text = "";
            tbModuleCostValue.Text = "";
            tbModuleDescription.Text = "";
            tbModuleCreditValue.Text = "";
            tbWSQCompetencyCodeValue.Text = "";
            tbModuleVersion.Text = "1";
            tbModuleEffectiveDate.Text = "";
            tbModuleTrgHr.Text = "";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string moduleCode = tbModuleCodeValue.Text;
            int moduleVersion = Convert.ToInt32(tbModuleVersion.Text);
            string moduleLevel = tbModuleLevelValue.Text;
            string moduleTitle = tbModuleTitleValue.Text;
            string moduleDescription = tbModuleDescription.Text;
            int moduleCredit = Convert.ToInt32(tbModuleCreditValue.Text);
            decimal moduleCost = Convert.ToDecimal(tbModuleCostValue.Text);
            DateTime moduleEffectDate = Convert.ToDateTime(tbModuleEffectiveDate.Text);
            decimal moduleTrainingHour = decimal.Parse(tbModuleTrgHr.Text);
            string WSQCompetencyCode = tbWSQCompetencyCodeValue.Text;

            Tuple<bool, string> success = mm.createModule(moduleCode, moduleVersion, moduleLevel, moduleTitle, moduleDescription,
        moduleCredit, moduleCost, moduleEffectDate, moduleTrainingHour, WSQCompetencyCode, LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
                btnClear_Click(null, null);
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

    }

}