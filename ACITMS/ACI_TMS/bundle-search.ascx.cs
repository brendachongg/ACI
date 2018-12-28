using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;

namespace ACI_TMS
{
    public partial class bundle_search : System.Web.UI.UserControl
    {
        public const string RB = "recentBundle";
        public const string BDT = "dtBundle";
        public const string PAGE_NAME = "bundle-search.ascx";

        private Bundle_Management pm = new Bundle_Management();
        public delegate void SelectBundle(int bundleId, string bundleCode);
        public event SelectBundle selectBundle;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadRecentBundle();
                loadListBundle("A", "D");
            }
        }

        private void loadRecentBundle()
        {      
            DataTable dt= pm.getRecentModBundle();
            ViewState[RB] = dt;

            if (dt != null)
            {
                gvRecentBundle.DataSource = ViewState[RB] as DataTable;
                gvRecentBundle.DataBind();
            }
            else
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving bundle listing.");
                return;
            }
            
        }

        private void loadListBundle(string frm, string to)
        {
            DataTable dt = pm.getListBundle(frm, to);

            if (dt != null)
            {
                ViewState[BDT] = dt;
                gvListBundle.DataSource = ViewState[BDT] as DataTable;
                gvListBundle.DataBind();
            }
            else
            {
                //redirect to error page
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving bundle listing.");
                return;
            }
        }

        protected void btnPkg_Click(object sender, EventArgs e)
        {
            string frm = ((Button)sender).CommandArgument.Substring(0, 1);
            string to = ((Button)sender).CommandArgument.Substring(1);

            loadListBundle(frm, to);

            btnPkgAD.CssClass = "btn btn-default";
            btnPkgEH.CssClass = "btn btn-default";
            btnPkgIL.CssClass = "btn btn-default";
            btnPkgMP.CssClass = "btn btn-default";
            btnPkgQT.CssClass = "btn btn-default";
            btnPkgUX.CssClass = "btn btn-default";
            btnPkgYZ.CssClass = "btn btn-default";

            ((Button)sender).CssClass = "btn btn-primary";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showPackageDialog();showListPackage();", true);
        }

        protected void gvBundle_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectBundle"))
            {
                GridViewRow gvr = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                selectBundle(int.Parse(((HiddenField)gvr.FindControl("hfBundleId")).Value), ((HiddenField)gvr.FindControl("hfBundleCode")).Value);
            }
        }

        protected void gvListBundle_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListBundle.DataSource = ViewState[BDT] as DataTable;
            gvListBundle.PageIndex = e.NewPageIndex;
            gvListBundle.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showPackageDialog();showListPackage();", true);
        }
    }
}