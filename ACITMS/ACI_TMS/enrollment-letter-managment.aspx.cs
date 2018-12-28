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
    public partial class enrollment_letter_managment : BasePage
    {
        public const string PAGE_NAME = "enrollment-letter-managment.aspx";

        private const string GV_BATCH_DATA = "BATCH";
        private const string GV_PROG_DATA = "PROGRAMME";
        private const string GV_TR_DATA = "TRAINEE";

        public enrollment_letter_managment()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ENROLLLETTER_GEN, AccessRight_Constance.ENROLLLETTER_VIEW, AccessRight_Constance.ENROLLTEMPL_VIEW })
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!checkLogin()) return;

                bool hasGen = checkAccessRights(AccessRight_Constance.ENROLLLETTER_GEN);
                bool hasLetter = checkAccessRights(AccessRight_Constance.ENROLLLETTER_VIEW);
                bool hasTempl = checkAccessRights(AccessRight_Constance.ENROLLTEMPL_VIEW);

                //check last tab first so that if user has mutliple rights, the first available tab will be the latest to be displayed on browser
                if (hasTempl)
                {
                    loadProgrammeCategory();
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab1", "showTab('" + panelTemplate.ClientID + "');", true);
                }
                else tabTemplate.Visible = panelTemplate.Visible = false;

                if (hasLetter) Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab2", "showTab('" + panelLetter.ClientID + "');", true);
                else tabLetter.Visible = panelLetter.Visible = false;

                if (hasGen) Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab3", "showTab('" + panelGen.ClientID + "');", true);
                else tabGen.Visible = panelGen.Visible = false;

                if (!checkAccessRights(AccessRight_Constance.ENROLLLETTER_EDIT))
                {
                    btnSaveLetter.Visible = false;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "disableLetter", "$(function () {tinyMCE.get('" + tbEnrolLetter.ClientID + "').setMode('readonly');});", true);
                }

                if (!checkAccessRights(AccessRight_Constance.ENROLLTEMPL_EDIT))
                {
                    btnSaveTemplate.Visible = false;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "disableTemplate", "$(function () {tinyMCE.get('" + tbEnrolTemplate.ClientID + "').setMode('readonly');});", true);
                }
            }
            else
            {
                panelLetterSuccess.Visible = panelLetterError.Visible = false;
                panelTemplateSuccess.Visible = panelTemplateError.Visible = false;
                panelGenSuccess.Visible = panelGenError.Visible = false;
            }
        }

        private void loadProgrammeCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programme categories.");
                return;
            }

            ddlProgrammeCategory.DataSource = dt;
            ddlProgrammeCategory.DataBind();
            ddlProgrammeCategory.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void btnSearchBatch_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Batch_Session_Management()).searchBatches(ddlSearchBatch.SelectedValue, tbSearchBatch.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ViewState[GV_BATCH_DATA] = dt;

            gvBatch.DataSource = dt;
            gvBatch.DataBind();

            if (ddlSearchBatch.SelectedValue == "AVA") Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelLetter.ClientID + "');", true);
        }

        protected void gvBatch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectBatch"))
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_BATCH_DATA] as DataTable;

                hfSelId.Value = dt.Rows[index]["programmeBatchId"].ToString();
                string letter = (new Batch_Session_Management()).getEnrollmentLetter((int)dt.Rows[index]["programmeBatchId"]);
                tbEnrolLetter.Text = letter == null ? "" : letter;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelLetter.ClientID + "');", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showLetter", "showLetterDialog();", true);
            }
        }

        protected void gvBatch_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBatch.DataSource = ViewState[GV_BATCH_DATA] as DataTable;
            gvBatch.PageIndex = e.NewPageIndex;
            gvBatch.DataBind();

            if (ddlSearchBatch.SelectedValue == "AVA") Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelLetter.ClientID + "');", true);
        }

        protected void btnSaveTemplate_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Programme_Management()).updateEnrollmentTemplate(int.Parse(hfSelId.Value), tbEnrolTemplate.Text);

            if (status.Item1)
            {
                lblTemplateSuccess.Text = status.Item2;
                panelTemplateSuccess.Visible = true;
            }
            else
            {
                lblTemplateError.Text = status.Item2;
                panelTemplateError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelTemplate.ClientID + "');", true);
        }

        protected void btnSaveLetter_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Batch_Session_Management()).updateEnrollmentLetter(int.Parse(hfSelId.Value), tbEnrolLetter.Text);

            if (status.Item1)
            {
                lblLetterSuccess.Text = status.Item2;
                panelLetterSuccess.Visible = true;
            }
            else
            {
                lblLetterError.Text = status.Item2;
                panelLetterError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelLetter.ClientID + "');", true);
        }

        protected void ddlProgrammeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlProgrammeCategory.SelectedValue == "")
            {
                gvProgramme.Visible = false;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelTemplate.ClientID + "');", true);
                return;
            }

            DataTable dt = (new Programme_Management()).getProgrammeByProgrammeCategory(ddlProgrammeCategory.SelectedValue);

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programmes.");
                return;
            }

            ViewState[GV_PROG_DATA] = dt;

            gvProgramme.Visible = true;
            gvProgramme.DataSource = dt;
            gvProgramme.DataBind();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelTemplate.ClientID + "');", true);
        }

        protected void gvProgramme_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectProgramme"))
            {
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_PROG_DATA] as DataTable;

                hfSelId.Value = dt.Rows[index]["programmeId"].ToString();
                string template = (new Programme_Management()).getEnrollmentTemplate((int)dt.Rows[index]["programmeId"]);
                tbEnrolTemplate.Text = template == null ? "" : template;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelTemplate.ClientID + "');", true);
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showTemplate", "showTemplateDialog();", true);
            }
        }

        protected void gvProgramme_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProgramme.DataSource = ViewState[GV_PROG_DATA] as DataTable;
            gvProgramme.PageIndex = e.NewPageIndex;
            gvProgramme.DataBind();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelTemplate.ClientID + "');", true);
        }

        protected void btnSearchGen_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Trainee_Management()).searchTrainee(ddlSearchGen.SelectedValue, tbSearchGen.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving trainees.");
                return;
            }

            ViewState[GV_TR_DATA] = dt;

            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelGen.ClientID + "');", true);
        }

        protected void gvTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrainee.DataSource = ViewState[GV_TR_DATA] as DataTable;
            gvTrainee.PageIndex = e.NewPageIndex;
            gvTrainee.DataBind();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelGen.ClientID + "');", true);
        }

        protected void btnGen_Click(object sender, EventArgs e)
        {
            List<string> lstTr = new List<string>();
            foreach (GridViewRow gvr in gvTrainee.Rows)
            {
                if (((CheckBox)gvr.Cells[0].FindControl("cbTr")).Checked) lstTr.Add(((HiddenField)gvr.Cells[1].FindControl("hfTr")).Value);
            }

            if (lstTr.Count == 0)
            {
                lblGenError.Text = "Must select at least 1 trainee.";
                panelGenError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelGen.ClientID + "');", true);
                return;
            }

            Tuple<bool, string> status = (new Trainee_Management()).emailEnrollmentLetter(lstTr.ToArray(), LoginID);
            if (status.Item1)
            {
                lblGenSuccess.Text = status.Item2;
                panelGenSuccess.Visible = true;
                int pIndex = gvTrainee.PageIndex;
                btnSearchGen_Click(null, null);
                gvTrainee.DataSource = ViewState[GV_TR_DATA] as DataTable;
                gvTrainee.PageIndex = pIndex;
                gvTrainee.DataBind();
            }
            else
            {
                lblGenError.Text = status.Item2;
                panelGenError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showTab", "showTab('" + panelGen.ClientID + "');", true);
        }
    }
}