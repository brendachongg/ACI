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
    public partial class reassessment_edit : BasePage
    {
        public const string PAGE_NAME = "reassessment-edit.aspx";

        public const string TRAINEE_QUERY = "tid";
        public const string BATCH_QUERY = "bid";

        public reassessment_edit()
            : base(PAGE_NAME, AccessRight_Constance.REASSESSMENT_EDIT, reassessment_view.PAGE_NAME)
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

                PrevPage += "?" + reassessment_view.TRAINEE_QUERY + "=" + Request.QueryString[TRAINEE_QUERY] + "&" + reassessment_view.BATCH_QUERY + "=" + Request.QueryString[BATCH_QUERY];
                lbTraineeId.Text = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                hfBatchModuleId.Value = HttpUtility.UrlDecode(Request.QueryString[BATCH_QUERY]);

                loadReassessmentDetails();
            }
            else
            {
                PrevPage += "?" + reassessment_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + reassessment_view.BATCH_QUERY + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value);
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }

            modulesearch.selectModule += new module_search.SelectModule(selectModule);
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

            hfprogrammeBatchId.Value = dr["programmeBatchId"].ToString();
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

            selectModule((int)dr["moduleId"], dr["moduleTitle"].ToString());
            if (ddlNewBatch.Items.FindByValue(dr["programmeBatchId"].ToString()) == null)
                ddlNewBatch.Items.Add(new ListItem("[OBSOLUTE] " + dr["batchCode"].ToString() + dr["batchType"].ToString() + " / "
                    + dr["programmeTitle"].ToString() + " [" + dr["programmeStartDateDisp"].ToString() + " to " + dr["programmeCompletionDateDisp"].ToString() + "]", dr["programmeBatchId"].ToString()));

            ddlNewBatch.SelectedValue = dr["programmeBatchId"].ToString();
            ddlNewBatch_SelectedIndexChanged(null, null);

            //check the session that was selected
            foreach (GridViewRow gr in gvNewSessions.Rows)
            {
                if (gr.Cells[0].Text == dr["insertedSessionId"].ToString())
                {
                    ((RadioButton)gr.Cells[1].FindControl("rbSession")).Checked = true;
                    break;
                }
            }
        }

        private void selectModule(int moduleId, string title)
        {
            DataTable dt = (new Module_Management()).getModule(moduleId);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving module details.");
                return;
            }

            hfNewModuleId.Value = moduleId.ToString();
            tbNewModule.Text = title;
            lbNewModuleCode.Text = dt.Rows[0]["moduleCode"].ToString();

            gvNewSessions.Visible = false;
            loadBatches();
        }

        private void loadBatches()
        {
            DataTable dt = (new Batch_Session_Management()).getBatchesByModule(int.Parse(hfNewModuleId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ddlNewBatch.Items.Clear();
            ddlNewBatch.Items.Add(new ListItem("--Select--", ""));
            //filter with batches that has not been ended and not the absence batch programmeBatchId
            foreach (DataRow dr in dt.Select("programmeCompletionDate >= #" + DateTime.Today.ToString("MM/dd/yyyy") + "# and programmeBatchId<>" + hfprogrammeBatchId.Value))
            {
                ddlNewBatch.Items.Add(new ListItem(dr["batchCode"].ToString() + " / " + dr["programmeTitle"].ToString() + " [" + dr["programmeStartDateDisp"].ToString() + " to " + dr["programmeCompletionDateDisp"].ToString() + "]",
                    dr["programmeBatchId"].ToString()));
            }
        }

        protected void ddlNewBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlNewBatch.SelectedValue == "")
            {
                gvNewSessions.Visible = false;
                return;
            }

            DataTable dt = (new Batch_Session_Management()).getBatchModuleSessions(int.Parse(ddlNewBatch.SelectedValue), int.Parse(hfNewModuleId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving sessions.");
                return;
            }

            gvNewSessions.Visible = true;
            gvNewSessions.DataSource = dt;
            gvNewSessions.Columns[0].Visible = true;
            gvNewSessions.Columns[1].Visible = true;
            gvNewSessions.Columns[2].Visible = true;
            gvNewSessions.DataBind();
            gvNewSessions.Columns[0].Visible = false;
            gvNewSessions.Columns[1].Visible = false;
            gvNewSessions.Columns[2].Visible = false;

            hfSessionCnt.Text = dt.Rows.Count.ToString();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}