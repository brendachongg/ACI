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
    public partial class dashboard : BasePage
    {
        public const string PAGE_NAME = "dashboard.aspx";

        private const string GV_ATTENDANCE_DATA = "attendance";
        private const string GV_APPLNTR_DATA = "applnTrainee";
        private const string GV_CLASS_DATA = "classes";

        private bool viewApplicant = false;
        private bool viewTrainee = false;

        public dashboard()
            : base(PAGE_NAME, new string[]{})
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            loadStats();

            if (!Page.IsPostBack)
            {
                bool viewAll = checkAccessRights(AccessRight_Constance.ATTENDANCE_VIEW_ALL);
                if (checkAccessRights(AccessRight_Constance.ATTENDANCE_VIEW) || viewAll) displayTodayAttendance(viewAll);
                else divAttendance.Visible = false;

                if (checkAccessRights(AccessRight_Constance.BATCH_VIEW))
                {
                    loadProgrammeTypes();
                    loadProgrammeCategory();
                    displayClasses();
                }
                else divClasses.Visible = false;

                viewApplicant = checkAccessRights(AccessRight_Constance.APPLN_VIEW);
                viewTrainee = checkAccessRights(AccessRight_Constance.TRAINEE_VIEW);
                if (viewApplicant || viewTrainee)
                {
                    if (!viewApplicant) ddlSearchApplicantTraineeType.Items.Remove(ddlSearchApplicantTraineeType.Items.FindByValue("a"));
                    if (!viewTrainee) ddlSearchApplicantTraineeType.Items.Remove(ddlSearchApplicantTraineeType.Items.FindByValue("t"));
                }
                else divApplnTrainee.Visible = false;
            }
        }

        private void loadStats()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("link", typeof(string)));
            dt.Columns.Add(new DataColumn("cnt", typeof(string)));
            dt.Columns.Add(new DataColumn("label", typeof(string)));
            dt.Columns.Add(new DataColumn("icon", typeof(string)));

            DataRow dr;
            int cnt;
            if (checkAccessRights(AccessRight_Constance.APPLN_VIEW))
            {
                cnt = (new Applicant_Management()).getUnprocessedApplicationCount();

                dr = dt.NewRow();
                dr["link"] = applicant.PAGE_NAME;
                dr["cnt"] = cnt == -1 ? "<Error>" : cnt.ToString();
                dr["label"] = "Applicants";
                dr["icon"] = "glyphicon glyphicon-user";
                dt.Rows.Add(dr);
            }

            if (checkAccessRights(AccessRight_Constance.PAYMT_VIEW))
            {
                cnt = (new Finance_Management()).getNoOfOutstandingClassPayments();

                dr = dt.NewRow();
                dr["link"] = payment_management.PAGE_NAME + "?OUT=1";
                dr["cnt"] = cnt == -1 ? "<Error>" : cnt.ToString();
                dr["label"] = "Incomplete Payment";
                dr["icon"] = "fa fa-dollar";
                dt.Rows.Add(dr);
            }

            if (checkAccessRights(AccessRight_Constance.BATCH_VIEW))
            {
                cnt = (new Batch_Session_Management()).getNoOfAvaBatch();

                dr = dt.NewRow();
                dr["link"] = batch_management.PAGE_NAME + "?AVA=1";
                dr["cnt"] = cnt == -1 ? "<Error>" : cnt.ToString();
                dr["label"] = "Classes";
                dr["icon"] = "fa fa-fw fa-graduation-cap";
                dt.Rows.Add(dr);
            }

            if (checkAccessRights(AccessRight_Constance.MAKEUP_VIEW))
            {
                cnt = (new Attendance_Management()).getNoOfAbsentee();

                dr = dt.NewRow();
                dr["link"] = absentee_management.PAGE_NAME + "?AVA=1";
                dr["cnt"] = cnt == -1 ? "<Error>" : cnt.ToString();
                dr["label"] = "Due for insertion";
                dr["icon"] = "glyphicon glyphicon-calendar";
                dt.Rows.Add(dr);
            }

            if (checkAccessRights(AccessRight_Constance.SOA_PROCESS))
            {
                cnt = (new Assessment_Management()).getNoOfUnprocessedSOA();

                dr = dt.NewRow();
                dr["link"] = soa_process.PAGE_NAME;
                dr["cnt"] = cnt == -1 ? "<Error>" : cnt.ToString();
                dr["label"] = "Available for SkillsConnect";
                dr["icon"] = "glyphicon glyphicon-inbox";
                dt.Rows.Add(dr);
            }

            lvStats.DataSource = dt;
            lvStats.DataBind();
        }

        private void displayTodayAttendance(bool viewAll)
        {
            DataTable dt = (new Attendance_Management()).getAttendanceSessions("D", DateTime.Now.ToString("dd MMM yyyy"), LoginID, viewAll);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving sessions.");
                return;
            }

            ViewState[GV_ATTENDANCE_DATA] = dt;
            gvAttendance.DataSource = dt;
            gvAttendance.DataBind();
        }

        protected void gvAttendance_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvAttendance.PageIndex = e.NewPageIndex;
            gvAttendance.DataSource = (DataTable)ViewState[GV_ATTENDANCE_DATA];
            gvAttendance.DataBind();
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

        protected void btnSearchApplicantTrainee_Click(object sender, EventArgs e)
        {
            gvSearchApplicantTrainee.DataSource = null;
            gvSearchApplicantTrainee.DataBind();

            if (!tbSearchApplicantTrainee.Text.Equals(""))
            {
                DataTable dt = null;

                //search for applicant 
                if (ddlSearchApplicantTraineeType.SelectedValue.Equals("a"))
                {
                    dt = (new Applicant_Management()).searchApplicantByValue(tbSearchApplicantTrainee.Text);

                }
                //search for trainee
                else if (ddlSearchApplicantTraineeType.SelectedValue.Equals("t"))
                {
                    dt = (new Trainee_Management()).searchTraineeByValue(tbSearchApplicantTrainee.Text);

                }

                //mask out the id number
                foreach (DataRow dr in dt.Rows)
                {
                    dr["idNumber"] = dr["idNumber"].ToString().Substring(0, 1) + "".PadLeft(dr["idNumber"].ToString().Length-5, '*')
                        + dr["idNumber"].ToString().Substring(dr["idNumber"].ToString().Length - 4, 4); ;
                }

                ViewState[GV_APPLNTR_DATA] = dt;
                gvSearchApplicantTrainee.DataSource = dt;
                gvSearchApplicantTrainee.DataBind();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showApplnTrSearch", "showApplnTrSearch();", true);
        }

        protected void gvSearchApplicantTrainee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewDetails"))
            {
                if (ddlSearchApplicantTraineeType.SelectedValue.Equals("a"))
                {
                    string selectedApplicantId = e.CommandArgument.ToString();
                    Response.Redirect(applicant_details.PAGE_NAME + "?" + applicant_details.APPLICANT_QUERY + "=" + selectedApplicantId);
                }

                else if (ddlSearchApplicantTraineeType.SelectedValue.Equals("t"))
                {
                    string selectedTraineeId = e.CommandArgument.ToString();
                    Response.Redirect(trainee_details.PAGE_NAME + "?" + trainee_details.TRAINEE_QUERY + "=" + selectedTraineeId);
                }
            }
        }

        protected void gvSearchApplicantTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSearchApplicantTrainee.PageIndex = e.NewPageIndex;
            gvSearchApplicantTrainee.DataSource = (DataTable)ViewState[GV_APPLNTR_DATA];
            gvSearchApplicantTrainee.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showApplnTrSearch", "showApplnTrSearch();", true);
        }

        private void loadProgrammeCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving programme category.");
                return;
            }

            ddlProgCategory.DataSource = dt;
            ddlProgCategory.DataBind();
            ddlProgCategory.SelectedIndex = 0;
        }

        private void loadProgrammeTypes()
        {
            DataTable dt = (new Programme_Management()).getProgrammeType();
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving programme types.");
                return;
            }

            ddlProgType.DataSource = dt;
            ddlProgType.DataBind();
            ddlProgType.SelectedIndex = 0;
        }

        private void displayClasses()
        {
            DataTable dt = (new Batch_Session_Management()).filterBatches(ddlProgType.SelectedValue, ddlProgCategory.SelectedValue, ddlProgStatus.SelectedValue);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ViewState[GV_CLASS_DATA] = dt;
            gvBatch.DataSource = dt;
            gvBatch.DataBind();
        }

        protected void class_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayClasses();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showApplnTrSearch", "showClasses();", true);
        }

        protected void gvBatch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBatch.PageIndex = e.NewPageIndex;
            gvBatch.DataSource = (DataTable)ViewState[GV_CLASS_DATA];
            gvBatch.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showApplnTrSearch", "showClasses();", true);
        }

        protected void gvBatch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "selectBatch")
            {
                Server.Transfer(batch_view.PAGE_NAME + "?" + batch_view.QUERY_ID + "=" + HttpUtility.UrlEncode(e.CommandArgument.ToString()));
            }
        }
    }
}