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
    public partial class session_view : BasePage
    {
        public const string PAGE_NAME = "session-view.aspx";
        public const string QUERY_ID = "id";

        public session_view()
            : base(PAGE_NAME, AccessRight_Constance.SESSION_VIEW, session_management.PAGE_NAME)
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
                loadData();

                //if session has already pass cannot change
                if (!checkAccessRights(AccessRight_Constance.SESSION_EDIT) || 
                    DateTime.ParseExact(lbSessionDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.Now) < 0)
                    btnEdit.Visible = false;
            }
        }

        private void loadData()
        {
            Batch_Session_Management bsm = new Batch_Session_Management();
            DataTable dt = bsm.getSessionDetails(int.Parse(hfSessionId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving session details");
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
            lbDay.Text = bsm.formatDayStr(dr["day"].ToString());
            lbModDtFrm.Text = hfModDtFrm.Value = dr["startDateDisp"].ToString();
            lbModDtTo.Text = hfModDtTo.Value = dr["endDateDisp"].ToString();
            lbTrainer1.Text = dr["trainerUserName1"].ToString();
            lbTrainer2.Text = dr["trainerUserName2"] == DBNull.Value ? "" : dr["trainerUserName2"].ToString();
            lbAssessor.Text = dr["assessorUserName"].ToString();

            lbSessionDate.Text = dr["sessionDateDisp"].ToString();
            lbSessionPeriod.Text = dr["sessionPeriodDisp"].ToString();
            lbVenue.Text = dr["venueLocation"].ToString();

        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(session_edit.PAGE_NAME + "?" + session_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfSessionId.Value));
        }
    }
}