using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class attendance_listing : BasePage
    {
        public const string PAGE_NAME = "attendance-listing.aspx";
        public const string QUERY_ID = "id";

        private Attendance_Management am = new Attendance_Management();
        private DataTable dtAbsentees;
        private bool canEdit = false;

        public attendance_listing()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ATTENDANCE_VIEW, AccessRight_Constance.ATTENDANCE_VIEW_ALL }, attendance_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing session information.");
                    return;
                }

                hfSessionId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);
                bool ownSession = am.isOwnSession(LoginID, int.Parse(hfSessionId.Value));
                if (!checkAccessRights(AccessRight_Constance.ATTENDANCE_VIEW_ALL) && !ownSession)
                {
                    redirectToErrorPg("You are not authorized to view the attendance of this session.");
                    return;
                }

                canEdit = (checkAccessRights(AccessRight_Constance.ATTENDANCE_MARK_ALL) || (checkAccessRights(AccessRight_Constance.ATTENDANCE_MARK) && ownSession));

                loadSessionData();
                dtAbsentees = am.getSessionAbsentees(int.Parse(hfSessionId.Value));
                loadTrainees();
                loadInsertedTrainees();

                //check if session date is over current and if have right, if not cannot save
                btnSave.Visible = !(DateTime.ParseExact(lbSessionDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.Today) > 0) && canEdit;
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = false;
            }
        }

        private void loadSessionData()
        {
            Batch_Session_Management bsm = new Batch_Session_Management();
            DataTable dt = bsm.getSessionDetails(int.Parse(hfSessionId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving session details");
                return;
            }

            DataRow dr = dt.Rows[0];

            hfBatchModuleId.Value = dr["batchModuleId"].ToString();

            lbProgramme.Text = dr["programmeTitle"].ToString();
            lbProgrammeCode.Text = dr["programmeCode"].ToString();

            lbBatchCode.Text = dr["batchCode"].ToString();
            lbBatchType.Text = dr["batchTypeDisp"].ToString();         
            lbCapacity.Text = dr["batchCapacity"].ToString();

            lbModule.Text = dr["moduleTitle"].ToString();
            lbModuleCode.Text = dr["moduleCode"].ToString();
            lbTrainer1.Text = dr["trainerUserName1"].ToString();
            lbTrainer2.Text = dr["trainerUserName2"] == DBNull.Value ? "" : dr["trainerUserName2"].ToString();
            lbAssessor.Text = dr["assessorUserName"].ToString();

            lbSessionDate.Text = dr["sessionDateDisp"].ToString();
            lbSessionPeriod.Text = dr["sessionPeriodDisp"].ToString();
            lbVenue.Text = dr["venueLocation"].ToString();
        }

        private void loadTrainees()
        {
            DataTable dt = am.getSessionTrainees(int.Parse(hfSessionId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving attendance list.");
                return;
            }

            gvOri.DataSource = dt;
            gvOri.DataBind();
        }

        private void loadInsertedTrainees()
        {
            DataTable dt = am.getSessionInsertedTrainees(int.Parse(hfSessionId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving attendance list.");
                return;
            }

            panelMakeup.Visible = false;
            if (dt.Rows.Count > 0)
            {
                gvInserted.DataSource = dt;
                gvInserted.DataBind();
                panelMakeup.Visible = true;
            }
        }

        protected void gv_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (dtAbsentees == null || dtAbsentees.Rows.Count == 0) return;

            if (dtAbsentees.Select("traineeId='" + e.Row.Cells[0].Text + "'").Length > 0)
                ((CheckBox)e.Row.Cells[2].FindControl("cbAbsent")).Checked = true;

            if (!canEdit) ((CheckBox)e.Row.Cells[2].FindControl("cbAbsent")).Enabled = false;
        }

        private string[] getEnrolledAbsentees()
        {
            List<string> absTrainees = new List<string>();
            foreach (GridViewRow gvr in gvOri.Rows)
            {
                if (((CheckBox)gvr.Cells[2].FindControl("cbAbsent")).Checked)
                    absTrainees.Add(gvr.Cells[0].Text);
            }

            return absTrainees.ToArray();
        }

        private void getInsertedAbsentees()
        {
            List<string> absentees = new List<string>();
            List<string> trainees = new List<string>();

            if (panelMakeup.Visible)
            {
                foreach (GridViewRow gvr in gvInserted.Rows)
                {
                    if (((CheckBox)gvr.Cells[2].FindControl("cbAbsent")).Checked)
                        absentees.Add(gvr.Cells[0].Text);
                    else
                        trainees.Add(gvr.Cells[0].Text);
                }
            }

            insertedAbsentees = absentees.ToArray();
            insertedTrainees = trainees.ToArray();
        }

        private string[] insertedAbsentees, insertedTrainees;
        protected void btnSave_Click(object sender, EventArgs e)
        {
            getInsertedAbsentees();

            Tuple<int, string> status = (new Attendance_Management()).updateAttendance(int.Parse(hfSessionId.Value), int.Parse(hfBatchModuleId.Value),
                getEnrolledAbsentees(), insertedAbsentees, insertedTrainees, LoginID, false);

            if (status.Item1 == -1)
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
            }
            else if (status.Item1 == 0)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
            }
            else
            {
                lbTrainees.Text = status.Item2;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "displayPrompt", "showOverwrite();", true);
            }
        }

        protected void btnOverwrite_Click(object sender, EventArgs e)
        {
            getInsertedAbsentees();

            Tuple<int, string> status = (new Attendance_Management()).updateAttendance(int.Parse(hfSessionId.Value), int.Parse(hfBatchModuleId.Value),
                getEnrolledAbsentees(), insertedAbsentees, insertedTrainees, LoginID, true);

            if (status.Item1 == -1)
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
            else if (status.Item1 == 0)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "exportMain", "window.open('" + attendance_sheet.PAGE_NAME + "?" + attendance_sheet.BATCH_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value)
                + "&" + attendance_sheet.MODE_QUERY + "=M', '_blank', 'menubar=no,location=no');", true);

            if (gvInserted.Rows.Count > 0)
            { 
                Page.ClientScript.RegisterStartupScript(this.GetType(), "exportInserted", "window.open('" + attendance_sheet.PAGE_NAME + "?" + attendance_sheet.BATCH_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value)
                    + "&" + attendance_sheet.MODE_QUERY + "=I', '_blank', 'menubar=no,location=no');", true);
            }
        }

    }
}