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
    public partial class assessment_management : BasePage
    {
        public const string PAGE_NAME = "assessment-management.aspx";

        private const string GV_TRAINEE_DATA = "trainee";
        private const string GV_MODULE_DATA = "module";

        public assessment_management()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ASSESSMENT_EDIT, AccessRight_Constance.ASSESSMENT_EDIT_ALL })
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            Assessment_Management am = new Assessment_Management();

            gvModule.Visible = false;
            gvTrainee.Visible = false;

            if (ddlSearch.SelectedValue == "T")
            {
                DataTable dt = am.searchTrainee(txtSearch.Text, LoginID, checkAccessRights(AccessRight_Constance.ASSESSMENT_VIEW_ALL));
                gvTrainee.Visible = true;
                ViewState[GV_TRAINEE_DATA] = dt;
                gvTrainee.DataSource = dt;
                gvTrainee.DataBind();
            }
            else if (ddlSearch.SelectedValue == "M")
            {
                DataTable dt = am.searchModule(txtSearch.Text, LoginID, checkAccessRights(AccessRight_Constance.ASSESSMENT_VIEW_ALL));
                gvModule.Visible = true;
                ViewState[GV_MODULE_DATA] = dt;
                gvModule.DataSource = dt;
                gvModule.DataBind();
            }
            else if (ddlSearch.SelectedValue == "CC")
            {
                DataTable dt = am.searchClass(txtSearch.Text, LoginID, checkAccessRights(AccessRight_Constance.ASSESSMENT_VIEW_ALL));
                gvModule.Visible = true;
                ViewState[GV_MODULE_DATA] = dt;
                gvModule.DataSource = dt;
                gvModule.DataBind();
            }
        }

        protected void gvTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrainee.DataSource = (DataTable)ViewState[GV_TRAINEE_DATA];
            gvTrainee.PageIndex = e.NewPageIndex;     
            gvTrainee.DataBind();
        }

        protected void gvModule_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvModule.DataSource = (DataTable)ViewState[GV_MODULE_DATA];
            gvModule.PageIndex = e.NewPageIndex;
            gvModule.DataBind();
        }

        protected void gvModule_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "module")
            {
                Server.Transfer(assessment_view_module.PAGE_NAME + "?" + assessment_view_module.QUERY_MODULE + "=" + HttpUtility.UrlEncode(e.CommandArgument.ToString()));
            }
        }

        protected void gvTrainee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "trainee")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = (DataTable)ViewState[GV_TRAINEE_DATA];

                //string tid = ((HiddenField)gvTrainee.Rows[index].Cells[0].FindControl("hfTrainee")).Value;
                //string bmid = ((HiddenField)gvTrainee.Rows[index].Cells[0].FindControl("hfBatchModule")).Value;

                Server.Transfer(assessment_view_trainee.PAGE_NAME + "?" + assessment_view_trainee.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["traineeId"].ToString()) + "&"
                    + assessment_view_trainee.BATCH_MODULE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["batchModuleId"].ToString()));
            }           
        }

    }
}