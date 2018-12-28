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
    public partial class batch_management : BasePage
    {
        public const string PAGE_NAME = "batch-management.aspx";

        public batch_management()
            : base(PAGE_NAME, AccessRight_Constance.BATCH_VIEW)
        {

        }

        Batch_Session_Management bsm = new Batch_Session_Management();
        private const string GV_DATA = "BATCH";

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearchBatch.UniqueID;

            if (!IsPostBack)
            {
                if (Request.QueryString["AVA"] != null)
                {
                    searchBatch("AVA", null);
                    ddlSearchBatchType.SelectedValue = "AVA";
                }
                else searchBatch(null, null);

                if (!checkAccessRights(AccessRight_Constance.BATCH_NEW))
                    panelNewBatch.Visible = false;

                if (PreviousPage is batch_session_creation)
                {
                    //if transfer from create batch session, means create batch successful
                    lblSuccess.Text = ((batch_session_creation)PreviousPage).StatusMsg;
                    panelSuccess.Visible = true;
                }
            }
            else panelSuccess.Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "chk", "checkValidators();", true);
        }

        private void searchBatch(string criteria, string value)
        {
            DataTable dt = bsm.searchBatches(criteria, value);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvBatch.DataSource = dt;
            gvBatch.DataBind();

            if (criteria == "AVA") Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void gvBatch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBatch.DataSource = ViewState[GV_DATA] as DataTable;
            gvBatch.PageIndex = e.NewPageIndex;
            gvBatch.DataBind();

            if (ddlSearchBatchType.SelectedValue == "AVA") Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void btnSearchBatch_Click(object sender, EventArgs e)
        {
            searchBatch(ddlSearchBatchType.SelectedValue, tbSearchBatch.Text);
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearchBatchType.SelectedIndex = 0;
            tbSearchBatch.Text = "";
            searchBatch(null, null);
        }

        protected void gvBatch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectBatch"))
            {
                Response.Redirect(batch_view.PAGE_NAME + "?" + batch_view.QUERY_ID + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void lkbtnCreateBatch_Click(object sender, EventArgs e)
        {
            Response.Redirect(batch_creation.PAGE_NAME);
        }
    }
}