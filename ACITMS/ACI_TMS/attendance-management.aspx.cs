using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;
using System.Web.UI.HtmlControls;

namespace ACI_TMS
{
    public partial class attendance_management : BasePage
    {
        public const string PAGE_NAME = "attendance-management.aspx";

        private const string GV_DATA = "attendance";

        public attendance_management()
            : base(PAGE_NAME, new string[] {AccessRight_Constance.ATTENDANCE_VIEW, AccessRight_Constance.ATTENDANCE_VIEW_ALL})
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                txtSearch.Text = DateTime.Now.ToString("dd MMM yyyy");
                searchSessions();
            }
        }

        private void searchSessions()
        {
            DataTable dt = (new Attendance_Management()).getAttendanceSessions(ddlSearch.SelectedValue, txtSearch.Text, LoginID, checkAccessRights(AccessRight_Constance.ATTENDANCE_VIEW_ALL));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving sessions.");
                return;
            }

            ViewState[GV_DATA] = dt;
            gvAttendance.DataSource = dt;
            gvAttendance.DataBind();
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;
            gvAttendance.DataSource = (DataTable)ViewState[GV_DATA];
            gvAttendance.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchSessions();
        }

        protected void gvAttendance_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (!((bool)((DataRowView)e.Row.DataItem)["hasInserted"]))
                e.Row.FindControl("btnInserted").Visible = false;
            else
                ((HtmlGenericControl)e.Row.FindControl("btnInserted")).Attributes.Add("onClick", "exportAttendance(" + ((DataRowView)e.Row.DataItem)["batchModuleId"].ToString() + ", 'I')");
        }

        protected void gvAttendance_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "View")
            {
                Server.Transfer(attendance_listing.PAGE_NAME + "?" + attendance_listing.QUERY_ID + "=" + HttpUtility.UrlEncode(e.CommandArgument.ToString()));
            }
        }
    }
}