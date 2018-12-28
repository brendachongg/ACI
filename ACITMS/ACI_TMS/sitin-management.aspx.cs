using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;

namespace ACI_TMS
{
    public partial class sitin_management : BasePage
    {
        public const string PAGE_NAME = "sitin-management.aspx";
        public const string REMOVE_QUERY = "r";

        private const string GV_DATA = "data";

        public sitin_management()
            : base(PAGE_NAME, AccessRight_Constance.SITIN_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                //if cannot create sit in, don show
                if (!checkAccessRights(AccessRight_Constance.SITIN_NEW))
                    panelNewSitIn.Visible = false;

                //load everyting
                searchSitIn();

                if (Request.QueryString[REMOVE_QUERY] != "" && Request.QueryString[REMOVE_QUERY] == "Y")
                {
                    lblSuccess.Text = "Sit-in removed successfully.";
                    panelSuccess.Visible = true;
                }
            }
            else
            {
                panelSuccess.Visible = false;
            }
        }

        private void searchSitIn()
        {
            DataTable dt = (new Trainee_Management()).searchSitIn(ddlSearch.SelectedValue == "" ? null : ddlSearch.SelectedValue, txtSearch.Text);

            ViewState[GV_DATA] = dt;
            gvSitIn.DataSource = dt;
            gvSitIn.DataBind();
        }

        protected void gvSitIn_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSitIn.DataSource = ViewState[GV_DATA] as DataTable;
            gvSitIn.PageIndex = e.NewPageIndex;
            gvSitIn.DataBind();
        }

        protected void lkbtnCreateBatch_Click(object sender, EventArgs e)
        {
            Server.Transfer(sitin_creation.PAGE_NAME);
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearch.SelectedIndex = 0;
            txtSearch.Text = "";
            searchSitIn();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchSitIn();
        }

        protected void gvSitIn_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_DATA] as DataTable; 

                //string traineeId = ((HiddenField)gvSitIn.Rows[index].Cells[0].FindControl("hfTrainee")).Value;
                //string batchModuleId = ((HiddenField)gvSitIn.Rows[index].Cells[0].FindControl("hfBatchModule")).Value;

                Server.Transfer(sitin_view.PAGE_NAME + "?" + sitin_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["traineeId"].ToString()) + "&"
                    + sitin_view.BATCH_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["batchModuleId"].ToString()));
            }
        }
    }
}