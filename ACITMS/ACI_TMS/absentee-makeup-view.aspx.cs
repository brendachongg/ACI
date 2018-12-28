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
    public partial class absentee_makeup_view : BasePage
    {
        public const string PAGE_NAME = "absentee-makeup-view.aspx";
        public const string TRAINEE_QUERY = "tid";
        public const string SESSION_QUERY = "sid";
        
        public absentee_makeup_view()
            : base(PAGE_NAME, AccessRight_Constance.MAKEUP_VIEW, absentee_management.PAGE_NAME)
        {
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "" || Request.QueryString[SESSION_QUERY] == null || Request.QueryString[SESSION_QUERY] == "")
                {
                    redirectToErrorPg("Missing information.");
                    return;
                }

                string traineeId = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                string sessionId = HttpUtility.UrlDecode(Request.QueryString[SESSION_QUERY]);

                Tuple<DataTable, DataTable> details = (new Attendance_Management()).getAbsenceDetails(traineeId, int.Parse(sessionId));
                if (details == null)
                {
                    redirectToErrorPg("Error retrieving information.");
                    return;
                }

                loadAbsenceDetails(details.Item1);
                if (details.Item2 != null && details.Item2.Rows.Count > 0) loadMakeupDetails(details.Item2);

                bool hasAccess = checkAccessRights(AccessRight_Constance.MAKEUP_EDIT);
                if (!hasAccess) btnEdit.Visible = false;
            }
        }

        private void loadAbsenceDetails(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

            lbTraineeId.Text = dr["traineeId"].ToString();
            lbTraineeName.Text = dr["fullName"].ToString();
            hfSessionId.Value = dr["sessionId"].ToString();

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
            lbSession.Text = dr["sessionDateDisp"].ToString() + " " + dr["sessionPeriodDisp"].ToString();
            lbVenue.Text = dr["venueLocation"].ToString();

            tbReason.Text = dr["AbsentRemarks"] == DBNull.Value ? "" : dr["AbsentRemarks"].ToString();
            if (dr["isAbsentValid"] != DBNull.Value && dr["isAbsentValid"].ToString() == "Y") cbValid.Checked = true;
        }

        private void loadMakeupDetails(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

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
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(absentee_makeup_edit.PAGE_NAME + "?" + absentee_makeup_edit.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + absentee_makeup_edit.SESSION_QUERY + "=" + HttpUtility.UrlEncode(hfSessionId.Value));
        }
    }
}