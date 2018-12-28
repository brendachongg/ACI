using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class programme_management : BasePage
    {
        public const string PAGE_NAME = "programme-management.aspx";

        private Programme_Management pm = new Programme_Management();
        private const string GV_DATA = "PROGRAMME";

        public programme_management()
            : base(PAGE_NAME, AccessRight_Constance.PROGRAM_VIEW)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadProgrammeCategory();
                loadProgramme();

                if (!checkAccessRights(AccessRight_Constance.PROGRAM_NEW))
                    panelNewProgramme.Visible = false;
            }

        }

        private void loadProgrammeCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programme categories.");
                return;
            }

            ddlProgrammeCategory.DataSource = dt;
            ddlProgrammeCategory.DataBind();
            ddlProgrammeCategory.SelectedIndex = 0;
        }

        protected void ddlProgrammeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            string category = ddlProgrammeCategory.SelectedValue;
            loadProgramme();
        }

        private void loadProgramme()
        {
            DataTable dt = pm.getProgrammeByProgrammeCategory(ddlProgrammeCategory.SelectedValue);

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programmes.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvProgramme.DataSource = dt;
            gvProgramme.DataBind();
        }

        protected void lkbtnCreateProgramme_Click(object sender, EventArgs e)
        {
            Response.Redirect(programme_creation.PAGE_NAME);
        }

        protected void gvProgramme_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewProgrammeDetails"))
            {
                Response.Redirect(programme_view.PAGE_NAME + "?" + programme_view.QUERY_ID + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void gvProgramme_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProgramme.DataSource = ViewState[GV_DATA] as DataTable;
            gvProgramme.PageIndex = e.NewPageIndex;
            gvProgramme.DataBind();
        }


    }
}