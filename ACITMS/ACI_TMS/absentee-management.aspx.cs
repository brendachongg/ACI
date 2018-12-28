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
    public partial class absentee_management : BasePage
    {
        public const string PAGE_NAME = "absentee-management.aspx";

        private const string GV_DATA = "data";

        public absentee_management()
            : base(PAGE_NAME, AccessRight_Constance.MAKEUP_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                if (Request.QueryString["AVA"] != null) ddlSearchType.SelectedValue = "AVA"; 
                searchAbsentees();
            }

            //cos the date field is dynamically created at runtime, so everytime the page load need to recreate again
            Page.ClientScript.RegisterStartupScript(this.GetType(), "formatField", "formatSearchTxt();", true);
        }

        private void searchAbsentees()
        {
            DataTable dt = (new Attendance_Management()).searchAbsentees(ddlSearchType.SelectedValue, tbSearch.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving absentees.");
                return;
            }

            ViewState[GV_DATA] = dt;
            gvAbsentList.DataSource = dt;
            gvAbsentList.DataBind();

            if (ddlSearchType.SelectedValue == "AVA")
                Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void gvAbsentList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAbsentList.DataSource = ViewState[GV_DATA] as DataTable;
            gvAbsentList.PageIndex = e.NewPageIndex;
            gvAbsentList.DataBind();

            if (ddlSearchType.SelectedValue == "AVA")
                Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchAbsentees();
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearchType.SelectedIndex = 0;
            tbSearch.Text = "";
            searchAbsentees();
        }

        protected void gvAbsentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "selectTrainee")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_DATA] as DataTable;

                //string traineeId = ((HiddenField)gvAbsentList.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].FindControl("hfTraineeId")).Value;
                //int sessionId = int.Parse(((HiddenField)gvAbsentList.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].FindControl("hfSessionId")).Value);

                Server.Transfer(absentee_makeup_view.PAGE_NAME + "?" + absentee_makeup_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["traineeId"].ToString()) + "&"
                    + absentee_makeup_view.SESSION_QUERY + "=" + dt.Rows[index]["sessionId"].ToString());
            }
        }
    }
}