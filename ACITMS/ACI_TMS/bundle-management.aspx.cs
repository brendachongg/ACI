using LogicLayer;
using GeneralLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class bundle_management : BasePage
    {
        public const string PAGE_NAME = "bundle-management.aspx";

        public bundle_management()
            : base(PAGE_NAME, AccessRight_Constance.BUNDLE_VIEW)
        {

        }

        Bundle_Management bm = new Bundle_Management();
        private const string GV_DATA = "BUNDLE";

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearchBundle.UniqueID;

            if (!IsPostBack)
            {
                loadAllBundles();

                if (!checkAccessRights(AccessRight_Constance.BUNDLE_NEW))
                    panelNewBundle.Visible = false;
            }
        }

        private void loadAllBundles()
        {
            DataTable dt = bm.getAllBundles();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving bundles.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvBundle.DataSource = dt;
            gvBundle.DataBind();
        }

        protected void gvBundle_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBundle.DataSource = ViewState[GV_DATA] as DataTable;
            gvBundle.PageIndex = e.NewPageIndex;
            gvBundle.DataBind();
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearchBundleType.SelectedIndex = 0;
            tbSearchBundle.Text = "";
            loadAllBundles();
        }

        protected void btnSearchBundle_Click(object sender, EventArgs e)
        {
            if (ddlSearchBundleType.SelectedValue == "" || tbSearchBundle.Text.Trim() == "") return;

            DataTable dt = bm.searchBundle(ddlSearchBundleType.SelectedValue, tbSearchBundle.Text.Trim());
            if (dt == null)
            {
                lblError.Text = "Error searching bundles.";
                panelError.Visible = true;
                return;
            }

            ViewState[GV_DATA] = dt;

            gvBundle.DataSource = dt;
            gvBundle.DataBind();
        }

        protected void gvBundle_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectBundle"))
            {
                Response.Redirect(bundle_view.PAGE_NAME + "?" + bundle_view.QUERY_ID + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void lkbtnCreateBundle_Click(object sender, EventArgs e)
        {
            Response.Redirect(bundle_creation.PAGE_NAME);
        }
    }
}