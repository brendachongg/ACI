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
    public partial class module_search : System.Web.UI.UserControl
    {
        private const string DATA_KEY = "dtModules";
        private Module_Management mm = new Module_Management();

        public delegate void SelectModule(int moduleId, string title);
        public event SelectModule selectModule;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadRecentModule();
                loadListModule("A", "D");
            }
        }

        private void loadRecentModule()
        {
            DataTable dt = mm.getRecentModulesInBundle();

            if (dt != null)
            {
                gvRecentModule.DataSource = dt;
                gvRecentModule.Columns[0].Visible = true;
                gvRecentModule.DataBind();
                gvRecentModule.Columns[0].Visible = false;
            }
            else
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving recent modules.");
            }

        }

        private void loadListModule(string frm, string to)
        {
            DataTable dt = mm.getListModules(frm, to);

            if (dt != null)
            {
                ViewState[DATA_KEY] = dt;
                gvListModule.DataSource = dt;
                gvListModule.Columns[0].Visible = true;
                gvListModule.DataBind();
                gvListModule.Columns[0].Visible = false;
            }
            else
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving module listing.");
            }
        }

        protected void btnMod_Click(object sender, EventArgs e)
        {
            string frm = ((Button)sender).CommandArgument.Substring(0, 1);
            string to = ((Button)sender).CommandArgument.Substring(1);

            loadListModule(frm, to);

            btnModAD.CssClass = "btn btn-default";
            btnModEH.CssClass = "btn btn-default";
            btnModIL.CssClass = "btn btn-default";
            btnModMP.CssClass = "btn btn-default";
            btnModQT.CssClass = "btn btn-default";
            btnModUX.CssClass = "btn btn-default";
            btnModYZ.CssClass = "btn btn-default";

            ((Button)sender).CssClass = "btn btn-primary";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showModuleDialog();showListModule();", true);
        }

        protected void gvModule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectModuleList"))
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[DATA_KEY] as DataTable;

                selectModule((int)dt.Rows[index]["moduleId"], dt.Rows[index]["moduleTitle"].ToString());
            }
            else if (e.CommandName.Equals("selectModuleRecent"))
            {
                int index = int.Parse(e.CommandArgument.ToString());

                selectModule(int.Parse(((GridView)sender).Rows[index].Cells[0].Text), ((GridView)sender).Rows[index].Cells[3].Text);
            }
        }

        protected void gvListModule_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListModule.DataSource = ViewState[DATA_KEY] as DataTable;
            gvListModule.PageIndex = e.NewPageIndex;
            gvListModule.Columns[0].Visible = true;
            gvListModule.DataBind();
            gvListModule.Columns[0].Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showModuleDialog();showListModule();", true);
        }
    }
}