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
    public partial class assessment_edit_module : BasePage
    {
        public const string PAGE_NAME = "assessment-edit-module.aspx";
        public const string QUERY_MODULE = "bmid";

        private DataTable dtAssessor;
        private Attendance_Management am = new Attendance_Management();
        private int assessorId;

        public assessment_edit_module()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ASSESSMENT_EDIT, AccessRight_Constance.ASSESSMENT_EDIT_ALL }, assessment_view_module.PAGE_NAME)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {          
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_MODULE] == null || Request.QueryString[QUERY_MODULE] == "")
                {
                    PrevPage = assessment_management.PAGE_NAME;
                    redirectToErrorPg("Missing information.");
                    return;
                }

                string batchModuleId = HttpUtility.UrlDecode(Request.QueryString[QUERY_MODULE]);
                hfBatchModuleId.Value = batchModuleId;
                PrevPage += "?" + assessment_view_module.QUERY_MODULE + "=" + Request.QueryString[QUERY_MODULE];

                if (!checkAccessRights(AccessRight_Constance.ASSESSMENT_EDIT_ALL) && !am.isOwnBatchModule(LoginID, int.Parse(hfBatchModuleId.Value)))
                {
                    redirectToErrorPg("You are not authorized to edit the assessment of this module.");
                    return;
                }

                loadBatchModuleDetails();
                loadTraineeAssessment();
            }
            else
            {
                panelSysError.Visible = false;
                panelSuccess.Visible = false;
                PrevPage += "?" + assessment_view_module.QUERY_MODULE + "=" + HttpUtility.UrlEncode(hfBatchModuleId.Value);
            }
        }

        private void loadBatchModuleDetails()
        {
            DataTable dt = (new Batch_Session_Management()).getBatchModuleInfo(int.Parse(hfBatchModuleId.Value));
            if (dt == null || dt.Rows.Count == 0)
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

            assessorId = (dr["assessorUserId"] == DBNull.Value ? -1 : (int)dr["assessorUserId"]);
        }

        private void loadTraineeAssessment()
        {
            DataTable dt = (new Assessment_Management()).getBatchModuleTraineesAssessment(int.Parse(hfBatchModuleId.Value));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving trainee assessments.");
                return;
            }

            dtAssessor = (new ACI_Staff_User()).getAssessors();

            gv1stAssessment.DataSource = dt.Select("attempt=1").CopyToDataTable();
            gv1stAssessment.DataBind();

            gv2ndAssessment.DataSource = dt.Select("attempt=2").CopyToDataTable();
            gv2ndAssessment.DataBind();

            hfNoOfTrainees.Text = gv1stAssessment.Rows.Count.ToString();
        }

        protected void gvAssessment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            DropDownList ddl = e.Row.Cells[3].FindControl("ddlAssessor") as DropDownList;
            ddl.DataSource = dtAssessor;
            ddl.DataBind();
            //if assessor can be found, default selection to assessor, else insert blank option
            if(ddl.Items.FindByValue(assessorId.ToString())==null) ddl.Items.Insert(0, new ListItem("--Select--", ""));
            else ddl.SelectedValue = assessorId.ToString();

            DataRowView r = (DataRowView)e.Row.DataItem;
            if (r["assessorId"] != DBNull.Value) ddl.SelectedValue = r["assessorId"].ToString();
            if (r["assessmentDateDisp"] != DBNull.Value) ((TextBox)e.Row.Cells[2].FindControl("txtAssessDt")).Text = r["assessmentDateDisp"].ToString();
            if (r["moduleResult"] != DBNull.Value) ((DropDownList)e.Row.Cells[3].FindControl("ddlResult")).SelectedValue = r["moduleResult"].ToString();

            if (!am.isTraineeMeetModuleAttendance(r["traineeId"].ToString(), int.Parse(hfBatchModuleId.Value)))
            {
                ddl.Enabled = false;
                ((TextBox)e.Row.Cells[2].FindControl("txtAssessDt")).Enabled = false;
                ((DropDownList)e.Row.Cells[3].FindControl("ddlResult")).Enabled = false;

                e.Row.BackColor = System.Drawing.Color.LightPink;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("traineeId", typeof(string)));
            dt.Columns.Add(new DataColumn("assessmentDate1", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("assessorId1", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleResult1", typeof(string)));
            dt.Columns.Add(new DataColumn("assessmentDate2", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("assessorId2", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleResult2", typeof(string)));

            lblSysError.Text = "";

            GridViewRow gvr1, gvr2;
            DataRow dr;
            TextBox tb1, tb2;
            for (int i = 0; i < gv1stAssessment.Rows.Count; i++)
            {
                gvr1 = gv1stAssessment.Rows[i];
                gvr2 = gv2ndAssessment.Rows[i];

                tb1 = (TextBox)gvr1.Cells[2].FindControl("txtAssessDt");
                tb2 = (TextBox)gvr2.Cells[2].FindControl("txtAssessDt");

                //check if the 2nd assessment is filled in the first assessment has to filled in too
                if (tb2.Text != "" && tb1.Text == "")
                    lblSysError.Text += "<li>1st assessment cannot be empty when 2nd assessment is filled for trainee " + gvr1.Cells[1].Text + ".</li>";
                
                //if both assessment are filled, then:
                //1) 2nd assessment date cannot be earlier than 1st assesssment date
                //2) 1st assessment result has to be not competent
                if (tb1.Text != "" && tb2.Text != "")
                {
                    if( DateTime.ParseExact(tb1.Text, "dd MMM yyyy", CultureInfo.InvariantCulture)
                        .CompareTo(DateTime.ParseExact(tb2.Text, "dd MMM yyyy", CultureInfo.InvariantCulture)) > 0)
                        lblSysError.Text += "<li>2nd assessment date cannot be earlier than 1st assessment date for trainee " + gvr1.Cells[1].Text + ".</li>";

                    if (((DropDownList)gvr1.Cells[4].FindControl("ddlResult")).SelectedValue == ModuleResult.C.ToString())
                        lblSysError.Text += "<li>2nd assemssent has been entered for trainee " + gvr1.Cells[1].Text + ", therefore result for 1st assessment cannot be competent.</li>";
                }

                    dr = dt.NewRow();
                    dr["traineeId"] = gvr1.Cells[0].Text;
                    dr["assessmentDate1"] = (tb1.Text == "" ? (object)DBNull.Value : DateTime.ParseExact(tb1.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                dr["assessorId1"] = (((DropDownList)gvr1.Cells[3].FindControl("ddlAssessor")).SelectedIndex == 0 || tb1.Text == "" ? (object)DBNull.Value : ((DropDownList)gvr1.Cells[3].FindControl("ddlAssessor")).SelectedValue);
                dr["moduleResult1"] = (((DropDownList)gvr1.Cells[4].FindControl("ddlResult")).SelectedIndex == 0 || tb1.Text == "" ? (object)DBNull.Value : ((DropDownList)gvr1.Cells[4].FindControl("ddlResult")).SelectedValue);

                    dr["assessmentDate2"] = (tb2.Text == "" ? (object)DBNull.Value : DateTime.ParseExact(tb2.Text, "dd MMM yyyy", CultureInfo.InvariantCulture));
                dr["assessorId2"] = (((DropDownList)gvr2.Cells[3].FindControl("ddlAssessor")).SelectedIndex == 0 || tb2.Text == "" ? (object)DBNull.Value : ((DropDownList)gvr2.Cells[3].FindControl("ddlAssessor")).SelectedValue);
                dr["moduleResult2"] = (((DropDownList)gvr2.Cells[4].FindControl("ddlResult")).SelectedIndex == 0 || tb2.Text == "" ? (object)DBNull.Value : ((DropDownList)gvr2.Cells[4].FindControl("ddlResult")).SelectedValue);

                    dt.Rows.Add(dr);
            }

            if (lblSysError.Text != "")
            {
                panelSysError.Visible = true;
                return;
            }

            Tuple<bool, string> status = (new Assessment_Management()).updateBatchModuleAssessment(int.Parse(hfBatchModuleId.Value), dt, LoginID);
            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblSysError.Text = status.Item2;
                panelSysError.Visible = false;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            loadTraineeAssessment();
        }
    }
}