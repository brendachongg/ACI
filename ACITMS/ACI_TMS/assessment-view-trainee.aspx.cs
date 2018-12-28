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
    public partial class assessment_view_trainee : BasePage
    {
        public const string PAGE_NAME = "assessment-view-trainee.aspx";
        public const string TRAINEE_QUERY = "tid";
        public const string BATCH_MODULE_QUERY = "bmid";

        private Assessment_Management am = new Assessment_Management();

        public assessment_view_trainee()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ASSESSMENT_VIEW, AccessRight_Constance.ASSESSMENT_VIEW_ALL }, assessment_management.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "" || Request.QueryString[BATCH_MODULE_QUERY] == null || Request.QueryString[BATCH_MODULE_QUERY] == "")
                {
                    redirectToErrorPg("Missing assessment information.");
                    return;
                }

                lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                hfBatchModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[BATCH_MODULE_QUERY]);

                bool ownMod = (new Assessment_Management()).isOwnBatchModule(LoginID, int.Parse(hfBatchModuleId.Value));
                if (!checkAccessRights(AccessRight_Constance.ASSESSMENT_VIEW_ALL) && !ownMod)
                {
                    redirectToErrorPg("You do not have the rights to view this trainee assessment.");
                    return;
                }

                populateTraineeModuleDetails();
                populateAssessment();
                if (!(new Attendance_Management()).isTraineeMeetModuleAttendance(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value)))
                {
                    lblError.Text = "Trainee has not meet attendance, unable to input assessment results.";
                    panelError.Visible = true;
                    btnEdit.Visible = false;
                }
                else if (DateTime.ParseExact(lbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.Today) > 0 ||
                    !(checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT_ALL) || (checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT) && ownMod)))
                {
                    btnEdit.Visible = false;
                }
            }
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

            lb1stAssessDt.Text = dr["firstAssessmentDate"] == DBNull.Value ? "" : dr["firstAssessmentDateDisp"].ToString();
            lb2ndAssessDt.Text = dr["finalAssessmentDate"] == DBNull.Value ? "" : dr["finalAssessmentDateDisp"].ToString();
            lb1stAssessor.Text = dr["firstAssessorName"] == DBNull.Value ? "" : dr["firstAssessorName"].ToString();
            lb2ndAssessor.Text = dr["finalAssessorName"] == DBNull.Value ? "" : dr["finalAssessorName"].ToString();

            if (dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                lb1stResult.Text = "Not Yet Competent";
            else if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.C.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "N")
                lb1stResult.Text = "Competent";

            if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.C.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                lb2ndResult.Text = "Competent";
            else if (dr["moduleResult"] != DBNull.Value && dr["moduleResult"].ToString() == ModuleResult.NYC.ToString() && dr["reAssessment"] != DBNull.Value && dr["reAssessment"].ToString() == "Y")
                lb2ndResult.Text = "Not Yet Competent";
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(assessment_edit_trainee.PAGE_NAME + "?" + assessment_edit_trainee.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + assessment_edit_trainee.BATCH_MODULE_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value));
        }
    }
}