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
    public partial class absentee_list : BasePage
    {
        public const string PAGE_NAME = "absentee-list.aspx";

        private const string GV_DATA = "data";

        public absentee_list()
            : base(PAGE_NAME, AccessRight_Constance.ATTENDANCE_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                searchAbsentees();
            }
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
        }

        protected void gvAbsentList_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAbsentList.DataSource = ViewState[GV_DATA] as DataTable;
            gvAbsentList.PageIndex = e.NewPageIndex;
            gvAbsentList.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchAbsentees();
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            searchAbsentees();
        }

        protected void gvAbsentList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "selectTrainee")
            {
                string traineeId = ((HiddenField)gvAbsentList.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].FindControl("hfTraineeId")).Value;
                int sessionId = int.Parse(((HiddenField)gvAbsentList.Rows[int.Parse(e.CommandArgument.ToString())].Cells[0].FindControl("hfSessionId")).Value);

                Server.Transfer(absentee_makeup_view.PAGE_NAME + "?" + absentee_makeup_view.TRAINEE_QUERY + "=" + traineeId + "&" + absentee_makeup_view.SESSION_QUERY + "=" + sessionId);
            }
        }
    }
}