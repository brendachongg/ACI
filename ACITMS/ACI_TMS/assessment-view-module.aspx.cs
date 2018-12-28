using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;
using System.Globalization;

namespace ACI_TMS
{
    public partial class assessment_view_module : BasePage
    {
        public const string PAGE_NAME = "assessment-view-module.aspx";
        public const string QUERY_MODULE = "bmid";

        public assessment_view_module()
            : base(PAGE_NAME, new string[]{AccessRight_Constance.ASSESSMENT_VIEW, AccessRight_Constance.ASSESSMENT_VIEW_ALL}, assessment_management.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_MODULE] == null || Request.QueryString[QUERY_MODULE] == "")
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                hfBatchModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_MODULE]);

                bool ownMod = (new Assessment_Management()).isOwnBatchModule(LoginID, int.Parse(hfBatchModuleId.Value));
                if (!checkAccessRights(AccessRight_Constance.ASSESSMENT_VIEW_ALL) && !ownMod)
                {
                    redirectToErrorPg("You do not have the rights to view this module assessment.");
                    return;
                }

                loadBatchModuleDetails();
                loadTraineeAssessment();

                //check if edit button can be shown
                if (DateTime.ParseExact(lbBatchStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.Today) > 0 ||
                    !(checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT_ALL) || (checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT) && ownMod)))
                    btnEdit.Visible = false;
            }
        }

        private void loadBatchModuleDetails()
        {
            DataTable dt = (new Batch_Session_Management()).getBatchModuleInfo(int.Parse(hfBatchModuleId.Value));
            if (dt == null || dt.Rows.Count==0)
            {
                redirectToErrorPg("Error retrieving module information");
                return;
            }

            DataRow dr = dt.Rows[0];

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

        private void loadTraineeAssessment()
        {
            DataTable dt = (new Assessment_Management()).getBatchModuleTraineesAssessment(int.Parse(hfBatchModuleId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving trainee assessments.");
                return;
            }

            gv1stAssessment.DataSource = dt.Select("attempt=1").CopyToDataTable();
            gv1stAssessment.DataBind();

            gv2ndAssessment.DataSource = dt.Select("attempt=2").CopyToDataTable();
            gv2ndAssessment.DataBind();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(assessment_edit_module.PAGE_NAME + "?" + assessment_edit_module.QUERY_MODULE + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value));
        }
    }
}