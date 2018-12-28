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
    public partial class sitin_creation : BasePage
    {
        public const string PAGE_NAME = "sitin-creation.aspx";

        public sitin_creation()
            : base(PAGE_NAME, AccessRight_Constance.SITIN_VIEW, sitin_management.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            modulesearch.selectModule += new module_search.SelectModule(selectModule);
            traineesearch.selectTrainee += new trainee_search.SelectTrainee(selectTrainee);
        }

        private void selectModule(int moduleId, string title)
        {
            DataTable dt = (new Module_Management()).getModule(moduleId);
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving module details.");
                return;
            }

            hfModuleId.Value = moduleId.ToString();
            tbModule.Text = title;
            lbModuleCode.Text = dt.Rows[0]["moduleCode"].ToString();

            gvSessions.Visible = false;
            loadBatches();
        }

        private void loadBatches()
        {
            DataTable dt = (new Batch_Session_Management()).getBatchesByModule(int.Parse(hfModuleId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ddlBatch.Items.Clear();
            ddlBatch.Items.Add(new ListItem("--Select--", ""));
            //filter with batches that has not been ended
            foreach (DataRow dr in dt.Select("moduleEndDate >= #" + DateTime.Today.ToString("MM/dd/yyyy") + "#"))
            {
                ddlBatch.Items.Add(new ListItem(dr["batchCode"].ToString() + " / " + dr["programmeTitle"].ToString() + " [" + dr["programmeStartDateDisp"].ToString() + " to " + dr["programmeCompletionDateDisp"].ToString() + "]",
                    dr["programmeBatchId"].ToString()));
            }
        }

        private void selectTrainee(string id, string name)
        {
            tbTraineeId.Text = id;
            lbTraineeName.Text = name;
        }

        protected void ddlBatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlBatch.SelectedValue == "")
            {
                gvSessions.Visible = false;
                return;
            }

            DataTable dt = (new Batch_Session_Management()).getBatchModuleSessions(int.Parse(ddlBatch.SelectedValue), int.Parse(hfModuleId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving sessions.");
                return;
            }

            gvSessions.Visible = true;
            gvSessions.DataSource = dt;
            gvSessions.Columns[0].Visible = true;
            gvSessions.Columns[1].Visible = true;
            gvSessions.Columns[2].Visible = true;
            gvSessions.DataBind();
            gvSessions.Columns[0].Visible = false;
            gvSessions.Columns[1].Visible = false;
            gvSessions.Columns[2].Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbTraineeId.Text = "";
            tbModule.Text = "";
            hfModuleId.Value = "";
            lbTraineeName.Text = "";
            lbModuleCode.Text = "";
            ddlBatch.Items.Clear();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Trainee_Management()).addSitIn(tbTraineeId.Text, int.Parse(ddlBatch.SelectedValue), int.Parse(hfModuleId.Value), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnClear_Click(null, null);
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }
        
    }
}