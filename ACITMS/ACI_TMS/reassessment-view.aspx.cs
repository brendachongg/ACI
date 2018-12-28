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
    public partial class reassessment_view : BasePage
    {
        public const string PAGE_NAME = "reassessment-view.aspx";

        public const string TRAINEE_QUERY = "tid";
        public const string BATCH_QUERY = "bid";

        private bool isReassessmentSet = false;

        public reassessment_view()
            : base(PAGE_NAME, AccessRight_Constance.REASSESSMENT_VIEW, reassessment_management.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "" || Request.QueryString[BATCH_QUERY] == null || Request.QueryString[BATCH_QUERY] == "")
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                hfBatchModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[BATCH_QUERY]);

                loadReassessmentDetails();

                if (!CheckAccessRights(AccessRight_Constance.REASSESSMENT_EDIT)) btnEdit.Visible = false;
                if (!CheckAccessRights(AccessRight_Constance.REASSESSMENT_DEL) || !isReassessmentSet) btnConfRem.Visible = false;
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadReassessmentDetails()
        {
            Tuple<DataTable, DataTable> details = (new Assessment_Management()).getReassessmentDetails(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value));
            if (details == null)
            {
                redirectToErrorPg("Error retrieving re-assessment details.");
                return;
            }

            DataRow dr = details.Item1.Rows[0];

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

            if (details.Item2 == null) return;

            dr = details.Item2.Rows[0];

            lbNewProgrammeCategory.Text = dr["programmeCategoryDisp"].ToString();
            lbNewProgrammeLevel.Text = dr["programmeLevelDisp"].ToString();
            lbNewProgramme.Text = dr["programmeTitle"].ToString();
            lbNewProgrammeVersion.Text = dr["programmeVersion"].ToString();
            lbNewProgrammeCode.Text = dr["programmeCode"].ToString();
            lbNewProgrammeType.Text = dr["programmeTypeDisp"].ToString();

            lbNewBatchCode.Text = dr["batchCode"].ToString();
            lbNewBatchType.Text = dr["batchTypeDisp"].ToString();
            lbNewProjCode.Text = dr["projectCode"].ToString();

            lbNewModule.Text = dr["moduleTitle"].ToString();
            lbNewModuleCode.Text = dr["moduleCode"].ToString();
            lbNewSession.Text = dr["sessionDateDisp"].ToString() + " " + dr["sessionPeriodDisp"].ToString();
            lbNewVenue.Text = dr["venueLocation"].ToString();

            isReassessmentSet = true;
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(reassessment_edit.PAGE_NAME + "?" + reassessment_edit.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + reassessment_edit.BATCH_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value));
        }

        protected void btnRem_Click(object sender, EventArgs e)
        {
            if((new Assessment_Management()).removeReassessment(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value), LoginID))
            {
                lblSuccess.Text = "Re-assessment session removed successfully.";
                panelSuccess.Visible = true;
                
                //clear the re-assessment session details
                lbNewBatchCode.Text = "";
                lbNewBatchType.Text = "";
                lbNewModule.Text = "";
                lbNewModuleCode.Text = "";
                lbNewProgramme.Text = "";
                lbNewProgrammeCategory.Text = "";
                lbNewProgrammeCode.Text = "";
                lbNewProgrammeLevel.Text = "";
                lbNewProgrammeType.Text = "";
                lbNewProgrammeVersion.Text = "";
                lbNewProjCode.Text = "";
                lbNewSession.Text = "";
                lbNewVenue.Text = "";
            }
            else
            {
                lblError.Text = "Error removing re-assessment session.";
                panelError.Visible = true;
            }
        }
    }
}