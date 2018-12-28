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
    public partial class assessment_edit_trainee : BasePage
    {
        public const string PAGE_NAME = "assessment-edit-trainee.aspx";
        public const string TRAINEE_QUERY = "tid";
        public const string BATCH_MODULE_QUERY = "bmid";

        private Assessment_Management am = new Assessment_Management();

        public assessment_edit_trainee()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ASSESSMENT_EDIT, AccessRight_Constance.ASSESSMENT_EDIT_ALL }, assessment_view_trainee.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "" || Request.QueryString[BATCH_MODULE_QUERY] == null || Request.QueryString[BATCH_MODULE_QUERY] == "")
                {
                    redirectToErrorPg("Missing assessment information.", assessment_management.PAGE_NAME);
                    return;
                }

                lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                hfBatchModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[BATCH_MODULE_QUERY]);
                PrevPage += "?" + assessment_view_trainee.TRAINEE_QUERY + "=" + Request.QueryString[TRAINEE_QUERY] + "&" + assessment_view_trainee.BATCH_MODULE_QUERY + "=" + Request.QueryString[BATCH_MODULE_QUERY];

                if (!checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT_ALL) && !am.isOwnBatchModule(LoginID, int.Parse(hfBatchModuleId.Value)))
                {
                    redirectToErrorPg("You are not authorized to edit the assessment of this trainee.");
                    return;
                }

                populateAssessors();
                populateTraineeModuleDetails();
                checkAttendance();
                populateAssessment();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
                PrevPage += "?" + assessment_view_trainee.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + assessment_view_trainee.BATCH_MODULE_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value);
            }
        }

        private void populateAssessors()
        {
            DataTable dt=(new ACI_Staff_User()).getAssessors();
            ddl1stAssessor.DataSource = dt;
            ddl1stAssessor.DataBind();
            ddl1stAssessor.Items.Insert(0, new ListItem("--Select--", ""));

            ddl2ndAssessor.DataSource = dt;
            ddl2ndAssessor.DataBind();
            ddl2ndAssessor.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void populateTraineeModuleDetails()
        {
            DataTable dt = am.getTraineeModuleInfo(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving trainee and module details.");
                return;
            }

            DataRow dr = dt.Rows[0];

            lbTraineeName.Text = dr["fullName"].ToString();

            lbProgrammeCategory.Text = dr["programmeCategoryDisp"].ToString();
            lbProgrammeLevel.Text = dr["programmeLevelDisp"].ToString();
            lbProgramme.Text = dr["programmeTitle"].ToString();
            lbProgrammeVersion.Text = dr["programmeVersion"].ToString();
            lbProgrammeCode.Text = dr["programmeCode"].ToString();
            lbProgrammeType.Text = dr["programmeTypeDisp"].ToString();

            lbBatchCode.Text = dr["batchCode"].ToString();
            lbBatchType.Text = dr["batchTypeDisp"].ToString();
            lbProjCode.Text = dr["projectCode"].ToString();
            lbRegStartDate.Text = dr["programmeRegStartDateDisp"].ToString();
            lbRegEndDate.Text = dr["programmeRegEndDateDisp"].ToString();
            lbBatchStartDate.Text = dr["programmeStartDateDisp"].ToString();
            lbBatchEndDate.Text = dr["programmeCompletionDateDisp"].ToString();
            lbCapacity.Text = dr["batchCapacity"].ToString();
            lbClassMode.Text = dr["classModeDisp"].ToString();

            lbModule.Text = dr["moduleTitle"].ToString();
            lbModuleCode.Text = dr["moduleCode"].ToString();
        }

        public void populateAssessment()
        {
            DataTable dt = am.getTraineeModuleAssessment(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value));
            DataRow dr = dt.Rows[0];

            txt1stAssessDt.Text = dr["firstAssessmentDate"] == DBNull.Value ? "" : dr["firstAssessmentDateDisp"].ToString();
            txt2ndAssessDt.Text = dr["finalAssessmentDate"] == DBNull.Value ? "" : dr["finalAssessmentDateDisp"].ToString();
            ddl1stAssessor.SelectedValue = dr["firstAssessorId"] == DBNull.Value ? "" : dr["firstAssessorId"].ToString();
            ddl2ndAssessor.SelectedValue = dr["finalAssessorId"] == DBNull.Value ? "" : dr["finalAssessorId"].ToString();

            if (dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                ddl1stResult.SelectedValue = ModuleResult.NYC.ToString();
            else if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.C.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "N")
                ddl1stResult.SelectedValue = ModuleResult.C.ToString();

            if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.C.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                ddl2ndResult.SelectedValue = ModuleResult.C.ToString();
            else if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.NYC.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                ddl2ndResult.SelectedValue = ModuleResult.NYC.ToString();
        }

        private bool checkAttendance()
        {
            bool meetAttendance = (new Attendance_Management()).isTraineeMeetModuleAttendance(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value));

            txt1stAssessDt.Enabled = meetAttendance;
            txt2ndAssessDt.Enabled = meetAttendance;
            ddl1stAssessor.Enabled = meetAttendance;
            ddl2ndAssessor.Enabled = meetAttendance;
            ddl1stResult.Enabled = meetAttendance;
            ddl2ndResult.Enabled = meetAttendance;
            btnSave.Visible = meetAttendance;
            btnClear.Visible = meetAttendance;

            if (!meetAttendance) { 
                lblError.Text = "Trainee has not meet attendance, unable to input assessment results.";
                panelError.Visible = true;
            }

            return meetAttendance;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (ddl1stResult.SelectedValue == ModuleResult.C.ToString() && ddl2ndResult.SelectedValue != "")
            {
                lblError.Text = "2nd assemssent has been entered, therefore result for 1st assessment cannot be competent.";
                panelError.Visible = true;
                return;
            }

            Tuple<bool, string> status = am.updateTraineeAssessment(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value),
                txt1stAssessDt.Text == "" ? DateTime.MinValue : DateTime.ParseExact(txt1stAssessDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                txt2ndAssessDt.Text == "" ? DateTime.MinValue : DateTime.ParseExact(txt2ndAssessDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                txt1stAssessDt.Text == "" ? -1 : int.Parse(ddl1stAssessor.SelectedValue),
                txt2ndAssessDt.Text == "" ? -1 : int.Parse(ddl2ndAssessor.SelectedValue),
                ddl1stResult.SelectedValue == "" ? null : ddl1stResult.SelectedValue,
                ddl2ndResult.SelectedValue == "" ? null : ddl2ndResult.SelectedValue, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            populateAssessment();
        }
    }
}