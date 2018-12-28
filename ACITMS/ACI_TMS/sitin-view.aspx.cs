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
    public partial class sitin_view : BasePage
    {
        public const string PAGE_NAME = "sitin-view.aspx";

        public const string TRAINEE_QUERY = "tid";
        public const string BATCH_QUERY = "bid";

        public sitin_view()
            : base(PAGE_NAME, AccessRight_Constance.SITIN_VIEW, sitin_management.PAGE_NAME)
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

                loadSitInDetails();

                if (!checkAccessRights(AccessRight_Constance.SITIN_DEL)) btnConfRem.Visible = false;
            }
            else
            {
                panelError.Visible = false;
            }
        }

        private void loadSitInDetails()
        {
            DataTable dt = (new Trainee_Management()).getSitInDetails(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving sit-in details.");
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

            DataTable dtSessions = (new Batch_Session_Management()).getBatchModuleSessions((int)dr["programmeBatchId"], (int)dr["moduleId"]);
            if (dtSessions == null)
            {
                redirectToErrorPg("Error retrieving sit-in module session details.");
                return;
            }
            gvNewSessions.DataSource = dtSessions;
            gvNewSessions.DataBind();
        }

        protected void btnRem_Click(object sender, EventArgs e)
        {
            if ((new Trainee_Management()).removeSitIn(lbTraineeId.Text, int.Parse(hfBatchModuleId.Value), LoginID))
            {
                Server.Transfer(sitin_management.PAGE_NAME + "?" + sitin_management.REMOVE_QUERY + "=Y");
            }
            else
            {
                lblError.Text = "Error removing sit-in.";
                panelError.Visible = true;
            }
        }
    }
}