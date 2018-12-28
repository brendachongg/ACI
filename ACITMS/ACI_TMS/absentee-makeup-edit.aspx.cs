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
    public partial class absentee_makeup_edit : BasePage
    {
        public const string PAGE_NAME = "absentee-makeup-edit.aspx";
        public const string TRAINEE_QUERY = "tid";
        public const string SESSION_QUERY = "sid";

        public absentee_makeup_edit()
            : base(PAGE_NAME, AccessRight_Constance.MAKEUP_EDIT, absentee_makeup_view.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[TRAINEE_QUERY] == null || Request.QueryString[TRAINEE_QUERY] == "" || Request.QueryString[SESSION_QUERY] == null || Request.QueryString[SESSION_QUERY] == "")
                {
                    redirectToErrorPg("Missing information.", absentee_management.PAGE_NAME);
                    return;
                }

                string traineeId = HttpUtility.UrlDecode(Request.QueryString[TRAINEE_QUERY]);
                string sessionId = HttpUtility.UrlDecode(Request.QueryString[SESSION_QUERY]);

                PrevPage += "?" + absentee_makeup_view.TRAINEE_QUERY + "=" + Request.QueryString[TRAINEE_QUERY] + "&" + absentee_makeup_view.SESSION_QUERY + "=" + Request.QueryString[SESSION_QUERY];

                Tuple<DataTable, DataTable> details = (new Attendance_Management()).getAbsenceDetails(traineeId, int.Parse(sessionId));
                if (details == null)
                {
                    redirectToErrorPg("Error retrieving information.");
                    return;
                }

                loadAbsenceDetails(details.Item1);
                if (details.Item2 != null && details.Item2.Rows.Count > 0) loadMakeupDetails(details.Item2);
                else //if no makeup info yet, auto default the module
                    selectModule(int.Parse(hfModuleId.Value), lbModule.Text);

                //check if should show the make up panel
                if (cbValid.Checked || hfAbsPayment.Value == "Y") panelMakeup.Visible = true;                
                else lbMakeupMsg.Visible = true;
            }
            else
            {
                PrevPage += "?" + absentee_makeup_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(lbTraineeId.Text) + "&" + absentee_makeup_view.SESSION_QUERY + "=" + HttpUtility.UrlEncode(hfSessionId.Value);
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
            
            modulesearch.selectModule += new module_search.SelectModule(selectModule);
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

            hfModuleId.Value = dr["moduleId"].ToString();
            lbModule.Text = dr["moduleTitle"].ToString();
            lbModuleCode.Text = dr["moduleCode"].ToString();
            lbSession.Text = dr["sessionDateDisp"].ToString() + " " + dr["sessionPeriodDisp"].ToString();
            hfSessionDt.Value = dr["sessionDateDisp"].ToString();
            hfSessionPeriod.Value = dr["sessionPeriod"].ToString();
            lbVenue.Text = dr["venueLocation"].ToString();

            tbReason.Text = dr["AbsentRemarks"] == DBNull.Value ? "" : dr["AbsentRemarks"].ToString();
            if (dr["isAbsentValid"] != DBNull.Value && dr["isAbsentValid"].ToString() == "Y") cbValid.Checked = true;

            hfAbsPayment.Value = (bool)dr["hasPayment"] == false ? "N" : "Y";
        }

        private void loadMakeupDetails(DataTable dt)
        {
            DataRow dr = dt.Rows[0];

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

            hfOriMakeupSessionId.Value = dr["insertedSessionId"].ToString();
        }

        protected void cbValid_CheckedChanged(object sender, EventArgs e)
        {
            panelMakeup.Visible = (cbValid.Checked || hfAbsPayment.Value == "Y");
            lbMakeupMsg.Visible = !panelMakeup.Visible;
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
            //get selected makeup session
            int selSessionId = -1, selBatchId = -1;
            if (panelMakeup.Visible) { 
                foreach (GridViewRow gr in gvNewSessions.Rows)
                {
                    if(((RadioButton)gr.Cells[3].FindControl("rbSession")).Checked){
                        //check if session date is before absent date
                        DateTime dt = DateTime.ParseExact(hfSessionDt.Value, "dd MMM yyyy", CultureInfo.InvariantCulture);
                        DateTime dtNew = DateTime.ParseExact(gr.Cells[5].Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                        bool isValid = true;
                        if (dt.CompareTo(dtNew) > 0) isValid = false;
                        else if (dt.CompareTo(dtNew) == 0)
                        {
                            if ((hfSessionPeriod.Value == DayPeriod.AM.ToString() && gr.Cells[2].Text == DayPeriod.AM.ToString()) ||
                                (hfSessionPeriod.Value == DayPeriod.PM.ToString() && gr.Cells[2].Text == DayPeriod.AM.ToString()) ||
                                (hfSessionPeriod.Value == DayPeriod.EVE.ToString() && (gr.Cells[2].Text == DayPeriod.AM.ToString() || gr.Cells[2].Text == DayPeriod.PM.ToString()))) 
                                isValid = false;
                        }
                        if (!isValid)
                        {
                            lblError.Text = "Selected make-up session must be after absent session.";
                            panelError.Visible = true;
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
                            return;
                        }

                        selSessionId = int.Parse(gr.Cells[0].Text);
                        selBatchId = int.Parse(gr.Cells[1].Text);
                        break;
                    }
                }
            }

            if (selSessionId.ToString() == hfOriMakeupSessionId.Value)
            {
                lblSuccess.Text = "No change in make-up details, no update is required.";
                panelSuccess.Visible = true;
                return;
            }

            Tuple<bool, string> status = (new Attendance_Management()).updateMakeup(lbTraineeId.Text, int.Parse(hfSessionId.Value), cbValid.Checked, 
                tbReason.Text == "" ? null : tbReason.Text, selBatchId, selSessionId, LoginID);

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
            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

    }
}