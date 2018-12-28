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
    public partial class trainee_repeat_module_management : BasePage
    {
        public const string PAGE_NAME = "trainee-repeat-module-management.aspx";

        private const string GV_DATA = "data";

        public trainee_repeat_module_management()
            : base(PAGE_NAME, AccessRight_Constance.REPEATMOD_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                //load everyting
                searchRepeatMod();
            }
        }

        private void searchRepeatMod()
        {
            DataTable dt = (new Assessment_Management()).searchRepeatModTrainees(ddlSearch.SelectedValue == "" ? null : ddlSearch.SelectedValue, txtSearch.Text);

            ViewState[GV_DATA] = dt;
            gvRepeatMod.DataSource = dt;
            gvRepeatMod.DataBind();
        }

        protected void gvRepeatMod_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvRepeatMod.DataSource = ViewState[GV_DATA] as DataTable;
            gvRepeatMod.PageIndex = e.NewPageIndex;
            gvRepeatMod.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchRepeatMod();
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearch.SelectedIndex = 0;
            txtSearch.Text = "";
            searchRepeatMod();
        }


        protected void gvRepeatMod_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_DATA] as DataTable;

                //string traineeId = ((HiddenField)gvRepeatMod.Rows[index].Cells[0].FindControl("hfTrainee")).Value;
                //string batchModuleId = ((HiddenField)gvRepeatMod.Rows[index].Cells[0].FindControl("hfBatchModule")).Value;

                Server.Transfer(trainee_repeat_module_view.PAGE_NAME + "?" + trainee_repeat_module_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["traineeId"].ToString()) + "&" 
                    + trainee_repeat_module_view.BATCH_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["batchModuleId"].ToString()));
            }
        }
    }
}