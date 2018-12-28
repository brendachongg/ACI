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
    public partial class bundle_view : BasePage
    {
        public const string PAGE_NAME = "bundle-view.aspx";
        public const string QUERY_ID = "id";
        private const string DATA_KEY = "dtModules";

        public bundle_view()
            : base(PAGE_NAME, AccessRight_Constance.BUNDLE_VIEW, bundle_management.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //load selected bundle details
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID].ToString().Trim() == "")
                {
                    redirectToErrorPg("Missing bundle code.");
                    return;
                }

                int bundleId;

                if (!int.TryParse(HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]), out bundleId))
                {
                    redirectToErrorPg("Invalid bundle information.");
                    return;
                }

                //check if edit btn should be shown, hide if any of the batch using the bundle has already started class
                if ((new Bundle_Management()).checkBatchStarted(bundleId))
                    btnEditBundle.Visible = false;

                loadBundle(bundleId);

                //check if can show edit btn
                if (!checkAccessRights(AccessRight_Constance.BUNDLE_EDIT)) btnEditBundle.Visible = false;

                //check if can show delete btn
                if (!checkAccessRights(AccessRight_Constance.BUNDLE_DEL)) btnRemBundle.Visible = false;
            }
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

            hfBundleId.Value = bundleId.ToString();
            lbBundleCode.Text = b.Item1;
            lbBundleType.Text = b.Item3;
            lbEffectiveDate.Text = b.Item4;
            lbBundleCost.Text = b.Item5.ToString();

            if (!b.Item6)
            {
                btnEditBundle.Visible = false;
                btnRemBundle.Visible = false;
            }

            ViewState[DATA_KEY] = b.Item7;
            gvModule.DataSource = b.Item7;
            gvModule.DataBind();
        }

        protected void btnEditBundle_Click(object sender, EventArgs e)
        {
            Response.Redirect(bundle_edit.PAGE_NAME + "?" + bundle_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfBundleId.Value));
        }

        protected void btnRemBundle_Click(object sender, EventArgs e)
        {
            Bundle_Management bm = new Bundle_Management();
            if (bm.delBundle(int.Parse(hfBundleId.Value), LoginID))
            {
                lblSuccess.Text = "Bundle is successfully removed.";
                panelSuccess.Visible = true;
                btnRemBundle.Visible = false;
                btnEditBundle.Visible = false;
            }
            else
            {
                lblError.Text = "Error removing bundle.";
                panelError.Visible = true;
            }
        }
    }
}