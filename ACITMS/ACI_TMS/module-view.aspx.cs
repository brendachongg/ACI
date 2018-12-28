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
    public partial class module_view : BasePage
    {
        public const string PAGE_NAME = "module-view.aspx";
        public const string QUERY_ID = "mId";
        public const string VERSION_ID = "vId";

        private Module_Management mm = new Module_Management();

        public module_view()
            : base(PAGE_NAME, AccessRight_Constance.MODULE_VIEW, module_management.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null)
                {
                    redirectToErrorPg("Missing module information.");
                    return;
                }

                hfModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);

                loadModuleDetails();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadModuleDetails()
        {
            DataTable dtModuleStructure = mm.getModule(Convert.ToInt32(hfModuleId.Value));

            if (dtModuleStructure == null || dtModuleStructure.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving module details.");
                return;
            }

            foreach (DataRow dr in dtModuleStructure.Rows)
            {
                lbModuleCodeValue.Text = dr["moduleCode"].ToString();
                lbModuleTitleValue.Text = dr["moduleTitle"].ToString();
                lbModuleVersionValue.Text = dr["moduleVersion"].ToString();
                lbModuleLevelValue.Text = dr["moduleLevel"].ToString();
                lbModuleCreditValue.Text = dr["moduleCredit"].ToString();
                lbModuleTrgHrValue.Text = dr["moduleTrainingHour"].ToString();
                lbModuleCostValue.Text = dr["moduleCost"].ToString();
                lbModuleEffectiveDateValue.Text = dr["moduleEffectDate"].ToString();
                lbWSQCompetencyCodeValue.Text = dr["WSQCompetencyCode"].ToString();
                lbModuleDescriptionValue.Text = dr["moduleDescription"].ToString();
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(module_edit.PAGE_NAME + "?" + module_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfModuleId.Value));
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnUpdateNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(module_edit.PAGE_NAME + "?" + module_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfModuleId.Value) + "&" + module_edit.NEW_VER + "=Y");
        }

        protected void btnUpdateCurrent_Click(object sender, EventArgs e)
        {
            Response.Redirect(module_edit.PAGE_NAME + "?" + module_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfModuleId.Value) + "&" + module_edit.NEW_VER + "=N");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = mm.deleteModule(int.Parse(hfModuleId.Value), lbModuleCodeValue.Text, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnConfirmDel.Visible = false;
                btnUpdateCurrent.Visible = false;
                btnUpdateNew.Visible = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }
    }
}