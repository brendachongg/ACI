using System;
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
    public partial class module_management : BasePage
    {
        public const string PAGE_NAME = "module-management.aspx";
        public const string GV_DATA = "MODULE";
        private Module_Management mm = new Module_Management();

        public module_management()
            : base(PAGE_NAME, AccessRight_Constance.MODULE_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearchModule.UniqueID;

            if (!IsPostBack)
            {
                searchModule(null, null);

                if (!checkAccessRights(AccessRight_Constance.MODULE_NEW))
                    panelNewModule.Visible = false;

                if (PreviousPage is batch_session_creation)
                {
                    //if transfer from create module, means create module successful
                    lblSuccess.Text = ((batch_session_creation)PreviousPage).StatusMsg;
                    panelSuccess.Visible = true;
                }

                loadModuleStructure();
            }
            else panelSuccess.Visible = false;
        }


        //check if module is empty when searching
        private void searchModule(string criteria, string value)
        {
            DataTable dt = mm.searchModuleStructure(criteria, value);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvModule.DataSource = dt;
            gvModule.DataBind();
        }

        private void loadModuleStructure()
        {
            Module_Management mm = new Module_Management();
            DataTable dtModuleStructure = mm.getAllModules();
            ViewState["dtModuleStructure"] = dtModuleStructure;
            gvModule.DataSource = dtModuleStructure;
            gvModule.DataBind();
        }

        protected void lkbtnCreateModule_Click(object sender, EventArgs e)
        {
            Response.Redirect(module_creation.PAGE_NAME);
        }

        protected void gvModule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewModuleDetails"))
            {
                Response.Redirect(module_view.PAGE_NAME + "?" + module_view.QUERY_ID + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void gvModule_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvModule.DataSource = ViewState["dtModuleStructure"] as DataTable;
            gvModule.PageIndex = e.NewPageIndex;
            gvModule.DataBind();
        }

        protected void btnSearchModule_Click(object sender, EventArgs e)
        {
            searchModule(ddlSearchModule.SelectedValue, tbSearchModule.Text);
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearchModule.SelectedIndex = 0;
            tbSearchModule.Text = "";
            gvModule.DataSource = ViewState["dtModuleStructure"] as DataTable;
            gvModule.DataBind();
        }

    }
}