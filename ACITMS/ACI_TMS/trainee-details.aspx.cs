using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Globalization;

namespace ACI_TMS
{
    public partial class trainee_details : BasePage
    {
        public const string PAGE_NAME = "trainee-details.aspx";

        public const string TRAINEE_QUERY = "t";
        public const string MSG_QUERY = "m";

        Trainee_Management tm = new Trainee_Management();

        public trainee_details()
            : base(PAGE_NAME, AccessRight_Constance.TRAINEE_VIEW, trainee.PAGE_NAME)
        {

        }

        protected void isContactEmailBothEmpty(object sourc, ServerValidateEventArgs args)
        {
            if (tbContactNo1.Text.Trim() == "" && tbEmailAdd.Text.Trim() == "")
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btn_print_Click(object sender, EventArgs e)
        {
            //string applicantId = Request.QueryString[APPLICANT_QUERY];
            string traineeID = Request.QueryString[TRAINEE_QUERY];
            Trainee_Management tm = new Trainee_Management();
            Tuple<DataTable, DataTable> dt = tm.getTraineeDetailsByTraineeId(traineeID);

            DataTable dtTraineeDetails = dt.Item1;

            dtTraineeDetails.Rows[0]["race"] = ddlRaceValue.SelectedItem.Text;
            dtTraineeDetails.Rows[0]["nationality"] = ddlNationalityValue.SelectedItem.Text;
            dtTraineeDetails.Rows[0]["gender"] = ddlGenderValue.SelectedItem.Text;
            dtTraineeDetails.Rows[0]["idType"] = ddlIdentificationType.SelectedItem.Text;
            dtTraineeDetails.Rows[0]["highestEducation"] = ddlHighestEducationValue.SelectedItem.Text;

            DataColumn column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "engSpoken";
            dtTraineeDetails.Columns.Add(column);
            DataRow row = dtTraineeDetails.NewRow();
            dtTraineeDetails.Rows.Add(row);
            dtTraineeDetails.Rows[0]["engSpoken"] = ddlEngPro.SelectedItem.Text;

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "chnSpoken";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();
            dtTraineeDetails.Rows.Add(row);
            dtTraineeDetails.Rows[0]["chnSpoken"] = ddlChnPro.SelectedItem.Text;

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "othSpoken";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();
            dtTraineeDetails.Rows.Add(row);
            if (ddlOtherLanguage.SelectedIndex != 0)
                dtTraineeDetails.Rows[0]["othSpoken"] = ddlOtherLangPro.SelectedItem.Text;
            else
                dtTraineeDetails.Rows[0]["othSpoken"] = "";

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "othSpokenLang";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();

            dtTraineeDetails.Rows.Add(row);

            if (ddlOtherLanguage.SelectedIndex != 0)
                dtTraineeDetails.Rows[0]["othSpokenLang"] = ddlOtherLanguage.SelectedItem.Text;
            else
                dtTraineeDetails.Rows[0]["othSpokenLang"] = "";


            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "engWritten";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();

            dtTraineeDetails.Rows.Add(row);
            dtTraineeDetails.Rows[0]["engWritten"] = ddlWEngPro.SelectedItem.Text;
            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "chnWritten";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();

            dtTraineeDetails.Rows.Add(row);
            dtTraineeDetails.Rows[0]["chnWritten"] = ddlWChiPro.SelectedItem.Text;

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "othWrittenLang";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();

            dtTraineeDetails.Rows.Add(row);

            if (ddlWOtherLanguage.SelectedIndex != 0)
                dtTraineeDetails.Rows[0]["othWrittenLang"] = ddlWOtherLanguage.SelectedItem.Text;
            else
                dtTraineeDetails.Rows[0]["othWrittenLang"] = "";

            column = new DataColumn();
            column.DataType = Type.GetType("System.String");
            column.ColumnName = "othWrittenLangPro";
            dtTraineeDetails.Columns.Add(column);
            row = dtTraineeDetails.NewRow();
            dtTraineeDetails.Rows.Add(row);

            if (ddlWOtherLangPro.SelectedIndex != 0)
                dtTraineeDetails.Rows[0]["othWrittenLangPro"] = ddlWOtherLangPro.SelectedItem.Text;
            else
                dtTraineeDetails.Rows[0]["othWrittenLangPro"] = "";

            HttpContext.Current.Session["dtTraineeDetails"] = dtTraineeDetails;
            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('" + trainee_details_print.PAGE_NAME + "'); ", true);

            List<EmploymentHistory> empl = new List<EmploymentHistory>();
            if (cbCurrEmpl.Checked)
            {
                empl.Add(new EmploymentHistory()
                {
                    companyName = tbCurrCoName.Text,
                    dept = tbCurrEmplDept.Text,
                    designation = tbCurrEmplDesignation.Text,
                    status = ddlCurrEmplStatus.SelectedItem.Text,
                    occupationType =ddlCurrEmplOccupation.SelectedItem.Text,
                    salary = decimal.Parse(tbCurrEmplSalary.Text),
                    dtStart = DateTime.ParseExact(tbCurrEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    dtEnd = DateTime.MaxValue,
                    current = General_Constance.STATUS_YES
                });
            }

            if (cbPrevEmpl.Checked)
            {
                empl.Add(new EmploymentHistory()
                    {
                        companyName = tbPrevCoName.Text,
                        dept = tbPrevEmplDept.Text,
                        designation = tbPrevEmplDesignation.Text,
                        status = ddlPrevEmplStatus.SelectedItem.Text,
                        occupationType = ddlPrevEmplOccupation.SelectedItem.Text,
                        salary = decimal.Parse(tbPrevEmplSalary.Text),
                        dtStart = DateTime.ParseExact(tbPrevEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        dtEnd = DateTime.ParseExact(tbPrevEmplEndDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                        current = General_Constance.STATUS_NO
                    });
            }

            HttpContext.Current.Session["dtEmploymentHistory"] = empl;

            DataTable dtTraineeProgrammeInfo = tm.getTraineeProgrammeInfo(traineeID);
            HttpContext.Current.Session["dtTraineeProgrammeInfo"] = dtTraineeProgrammeInfo;
        }

        protected override void OnInit(EventArgs e)
        {
            Page.SaveStateComplete += new EventHandler(RegisterHere);
            base.OnInit(e);
        }

        void RegisterHere(object sender, EventArgs e)
        {
            Page.ClientScript.RegisterOnSubmitStatement(typeof(Page), "name", "showPrev()");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    loadAllEducationReferenceInfo();
                    loadRaceReferenceInfo();
                    loadNationalityReferenceInfo();
                    loadIdentificationTypeReferenceInfo();
                    loadLanguageProficiency(ddlEngPro);
                    loadLanguageProficiency(ddlChnPro);
                    loadLanguageProficiency(ddlOtherLangPro);
                    loadLanguageProficiency(ddlWChiPro);
                    loadLanguageProficiency(ddlWEngPro);
                    loadLanguageProficiency(ddlWOtherLangPro);
                    loadOtherLanguage();

                    loadEmplStaffs();

                    string traineeID = Request.QueryString[TRAINEE_QUERY];
                    loadTrainee(traineeID);
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving trainee details");
            }
        }

        protected void btnEditTrainee_ServerClick(object sender, EventArgs e)
        {
            btnEditTrainee.Visible = false;
            panelSuccess.Visible = false;
            panelError.Visible = false;
            btnCancelTrainee.Visible = true;
            panelParticular.Enabled = true;
        }

        protected void btnCancelTrainee_ServerClick(object sender, EventArgs e)
        {
            btnEditTrainee.Visible = true;
            btnCancelTrainee.Visible = false;
            panelSuccess.Visible = false;
            panelError.Visible = false;
            panelParticular.Enabled = false;
            string traineeID = Request.QueryString[TRAINEE_QUERY];
            loadTrainee(traineeID);
        }

        protected void loadTrainee(string traineeID)
        {
            Trainee_Management tm = new Trainee_Management();
            Tuple<DataTable, DataTable> dt = tm.getTraineeDetailsByTraineeId(traineeID);

            DataTable dtTrainee = dt.Item1;
            DataTable dtTraineeEmployeeHistory = dt.Item2;

            if (dtTrainee.Rows.Count > 0)
            {
                if (dtTrainee.Rows[0]["traineeStatus"].ToString().Equals(TraineeStatus.W.ToString()))
                {
                    //redirectToErrorPg("Trainee is withdrawn.");
                    string reason = dtTrainee.Rows[0]["traineeRemarks"].ToString().Trim() == "" ? "-" : dtTrainee.Rows[0]["traineeRemarks"].ToString();
                    lblWithdrawReason.Text = "Trainee is withdrawn. Withdrawal Reason: " + reason;
                    btnCfmWithdrawTrainee.Visible = false;
                    btnCancelTrainee.Visible = false;
                    btnEditTrainee.Visible = false;
                    // panelUpdateTraineeDetails.Visible = false;
                    panelParticular.Enabled = false;

                }
                tbTraineeId.Text = traineeID;
                tbFullName.Text = dtTrainee.Rows[0]["fullName"].ToString();
                tbIdNo.Text = dtTrainee.Rows[0]["idNumber"].ToString();
                ddlIdentificationType.SelectedValue = dtTrainee.Rows[0]["idType"].ToString();
                ddlNationalityValue.SelectedValue = dtTrainee.Rows[0]["nationality"].ToString();
                ddlGenderValue.SelectedValue = dtTrainee.Rows[0]["gender"].ToString();
                tbContactNo1.Text = dtTrainee.Rows[0]["contactNumber1"].ToString();
                tbContactNo2.Text = dtTrainee.Rows[0]["contactNumber2"].ToString();
                tbEmailAdd.Text = dtTrainee.Rows[0]["emailAddress"].ToString();
                ddlRaceValue.SelectedValue = dtTrainee.Rows[0]["race"].ToString();
                tbDOB.Text = dtTrainee.Rows[0]["birthDateDisplay"].ToString();
                tbAddress.Text = dtTrainee.Rows[0]["addressLine"].ToString();
                tbPostalCode.Text = dtTrainee.Rows[0]["postalCode"].ToString();

                string TraineeEduLevel = dtTrainee.Rows[0]["highestEducation"].ToString();
                ddlHighestEducationValue.SelectedValue = TraineeEduLevel;
                tbHighestEduRemark.Text = dtTrainee.Rows[0]["highestEduRemarks"].ToString();

                string writtenLanguage = dtTrainee.Rows[0]["writtenLanguage"].ToString();
                if (!writtenLanguage.Equals(""))
                {
                    string[] wLangSet = writtenLanguage.Split(';');
                    foreach (string set in wLangSet)
                    {
                        if (!set.Equals(""))
                        {
                            string[] wLangPro = set.Split(':');


                            if (wLangPro[0].Equals(General_Constance.CHN))
                            {
                                ddlWChiPro.SelectedValue = wLangPro[1].ToString();
                            }

                            else if (wLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlWEngPro.SelectedValue = wLangPro[1].ToString();
                            }

                            else
                            {
                                ddlWOtherLanguage.SelectedValue = wLangPro[0];
                                ddlWOtherLangPro.SelectedValue = wLangPro[1];
                            }
                        }
                    }
                }

                string spokenLanguage = dtTrainee.Rows[0]["spokenLanguage"].ToString();
                if (!spokenLanguage.Equals(""))
                {
                    string[] sLangSet = spokenLanguage.Split(';');
                    foreach (string set in sLangSet)
                    {
                        if (!set.Equals(""))
                        {
                            string[] sLangPro = set.Split(':');
                            if (sLangPro[0].Equals(General_Constance.CHN))
                            {
                                ddlChnPro.SelectedValue = sLangPro[1].ToString();
                            }

                            else if (sLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlEngPro.SelectedValue = sLangPro[1].ToString();
                            }

                            else
                            {
                                ddlOtherLanguage.SelectedValue = sLangPro[0];
                                ddlOtherLangPro.SelectedValue = sLangPro[1];
                            }
                        }
                    }
                }


                if (dtTraineeEmployeeHistory.Rows.Count > 0)
                {
                    pnEmploymentHistory.Visible = true;
                    pnNoEmploymentHistory.Visible = false;

                    foreach (DataRow dr in dtTraineeEmployeeHistory.Rows)
                    {
                        if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                        {
                            cbCurrEmpl.Checked = true;
                            hdfCurrEmpHisId.Value = dr["employmentHistoryId"].ToString();
                            tbCurrCoName.Text = dr["companyName"].ToString();
                            tbCurrEmplDept.Text = dr["companyDepartment"].ToString();
                            ddlCurrEmplStatus.SelectedValue = dr["employmentStatus"].ToString();
                            ddlCurrEmplOccupation.SelectedValue = dr["occupationCode"].ToString(); 
                            tbCurrEmplSalary.Text = dr["salaryAmount"].ToString();
                            tbCurrEmplDesignation.Text = dr["position"].ToString();
                            DateTime sDate = DateTime.Parse(dr["employmentStartDate"].ToString()).Date;
                            tbCurrEmplStartDt.Text = sDate.ToString("dd MMM yyyy");


                        }
                        else
                        {

                            cbPrevEmpl.Checked = true;
                            hdfPrevEmpHist.Value = dr["employmentHistoryId"].ToString();
                            tbPrevCoName.Text = dr["companyName"].ToString();
                            tbPrevEmplDept.Text = dr["companyDepartment"].ToString();
                            ddlPrevEmplStatus.SelectedValue = dr["employmentStatus"].ToString();
                            ddlPrevEmplOccupation.SelectedValue = dr["occupationCode"].ToString();
                            tbPrevEmplSalary.Text = dr["salaryAmount"].ToString();
                            tbPrevEmplDesignation.Text = dr["position"].ToString();
                            DateTime sDate = DateTime.Parse(dr["employmentStartDate"].ToString()).Date;
                            DateTime eDate = DateTime.Parse(dr["employmentEndDate"].ToString()).Date;
                            tbPrevEmplStartDt.Text = sDate.ToString("dd MMM yyyy");
                            tbPrevEmplEndDt.Text = eDate.ToString("dd MMM yyyy");

                        }
                    }
                }
                else
                {
                    pnEmploymentHistory.Visible = false;
                    pnNoEmploymentHistory.Visible = true;
                    lbNoHistory.Text = "No Employment Records were found for this trainee.";

                }



            }

            DataTable dtTraineeProgrammeInfo = tm.getTraineeProgrammeInfo(traineeID);

            if (dtTraineeProgrammeInfo.Rows.Count > 0)
            {
                tbProjCode.Text = dtTraineeProgrammeInfo.Rows[0]["projectCode"].ToString();
                tbCourseCode.Text = dtTraineeProgrammeInfo.Rows[0]["courseCode"].ToString();
                tbProgrammeTitle.Text = dtTraineeProgrammeInfo.Rows[0]["programmeTitle"].ToString();
                tbProgrammeStartDate.Text = Convert.ToDateTime(dtTraineeProgrammeInfo.Rows[0]["programmeStartDate"].ToString()).ToString("dd MMM yyyy");
                tbProgrammeEndDate.Text = Convert.ToDateTime(dtTraineeProgrammeInfo.Rows[0]["programmeCompletionDate"].ToString()).ToString("dd MMM yyyy");
                hfBatchId.Value = dtTraineeProgrammeInfo.Rows[0]["programmeBatchId"].ToString();
                tbBatchCode.Text = dtTraineeProgrammeInfo.Rows[0]["BatchCode"].ToString();
            }
        }

        private void loadEmplStaffs()
        {

            Applicant_Management am = new Applicant_Management();
            DataTable dt = am.getEmploymentJob();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving designation type options.");
                return;
            }

            ddlCurrEmplOccupation.DataSource = dt;
            ddlCurrEmplOccupation.DataBind();
            ddlCurrEmplOccupation.Items.Insert(0, new ListItem("--Select--", ""));

            ddlPrevEmplOccupation.DataSource = dt;
            ddlPrevEmplOccupation.DataBind();
            ddlPrevEmplOccupation.Items.Insert(0, new ListItem("--Select--", ""));

            dt = am.getEmploymentStatus();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving employment type options.");
                return;
            }

            ddlCurrEmplStatus.DataSource = dt;
            ddlCurrEmplStatus.DataBind();
            ddlCurrEmplStatus.Items.Insert(0, new ListItem("--Select--", ""));

            ddlPrevEmplStatus.DataSource = dt;
            ddlPrevEmplStatus.DataBind();
            ddlPrevEmplStatus.Items.Insert(0, new ListItem("--Select--", ""));
        }


        private void loadOtherLanguage()
        {

            DataTable dtOtherLang = tm.getOtherLanguageCodeReference(General_Constance.LANG);


            ddlOtherLanguage.DataTextField = "codeValueDisplay";
            ddlOtherLanguage.DataValueField = "codeValue";

            ddlOtherLanguage.DataSource = dtOtherLang;
            ddlOtherLanguage.DataBind();
            ddlOtherLanguage.Items.Insert(0, new ListItem("--Select--", ""));

            ddlWOtherLanguage.DataTextField = "codeValueDisplay";
            ddlWOtherLanguage.DataValueField = "codeValue";

            ddlWOtherLanguage.DataSource = dtOtherLang;
            ddlWOtherLanguage.DataBind();
            ddlWOtherLanguage.Items.Insert(0, new ListItem("--Select--", ""));
        }


        private void loadLanguageProficiency(object sender)
        {

            DropDownList ddl = (DropDownList)sender;
            DataTable dtLangfPr = tm.getCodeReferenceValues(General_Constance.LANGPR);
            //if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            //{
            //DropDownList ddlWLangPr = (DropDownList)e.Item.FindControl("ddlWLanguagesPro");
            ddl.DataTextField = "codeValueDisplay";
            ddl.DataValueField = "codeValue";

            ddl.DataSource = dtLangfPr;
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("--Select--", ""));
            //}
        }

        private void loadAllEducationReferenceInfo()
        {

            DataTable dt = tm.getEduLevelReference(General_Constance.EDU);



            ddlHighestEducationValue.DataTextField = "codeValueDisplay";
            ddlHighestEducationValue.DataValueField = "codeValue";

            ddlHighestEducationValue.DataSource = dt;
            ddlHighestEducationValue.DataBind();

            ddlHighestEducationValue.Items.Insert(0, new ListItem("--Select--", ""));


        }

        private void loadRaceReferenceInfo()
        {

            DataTable dt = tm.getEduLevelReference(General_Constance.RACE);



            ddlRaceValue.DataTextField = "codeValueDisplay";
            ddlRaceValue.DataValueField = "codeValue";

            ddlRaceValue.DataSource = dt;
            ddlRaceValue.DataBind();

            ddlRaceValue.Items.Insert(0, new ListItem("--Select--", ""));


        }

        private void loadNationalityReferenceInfo()
        {

            DataTable dt = tm.getNationalityCodeReference(General_Constance.NATION);



            ddlNationalityValue.DataTextField = "codeValueDisplay";
            ddlNationalityValue.DataValueField = "codeValue";

            ddlNationalityValue.DataSource = dt;
            ddlNationalityValue.DataBind();

            ddlNationalityValue.Items.Insert(0, new ListItem("--Select--", ""));


        }

        private void loadIdentificationTypeReferenceInfo()
        {

            DataTable dt = tm.getIdentificationTypeCodeReference(General_Constance.IDTYPE);



            ddlIdentificationType.DataTextField = "codeValueDisplay";
            ddlIdentificationType.DataValueField = "codeValue";

            ddlIdentificationType.DataSource = dt;
            ddlIdentificationType.DataBind();

            ddlIdentificationType.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string traineeId = tbTraineeId.Text;
            string fullName = tbFullName.Text;
            string idNumber = tbIdNo.Text;
            string idType = ddlIdentificationType.SelectedValue;
            string nationality = ddlNationalityValue.SelectedValue;
            string gender = ddlGenderValue.SelectedValue;
            string race = ddlRaceValue.SelectedValue;
            string contactNumber1 = tbContactNo1.Text;
            string contactNumber2 = tbContactNo2.Text;
            string emailAddress = tbEmailAdd.Text;
            DateTime birthDate = Convert.ToDateTime(null);
            string dob = tbDOB.Text;
            string address = tbAddress.Text;
            string postalCode = tbPostalCode.Text;
            string highestEdu = ddlHighestEducationValue.SelectedValue;
            string highestEduRemarks = tbHighestEduRemark.Text;
            birthDate = DateTime.Parse(dob);

            string wlangScore = "";
            wlangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";
            wlangScore += General_Constance.CHN + ":" + ddlWChiPro.SelectedValue + ";";
            if (!ddlWOtherLanguage.SelectedValue.Equals("") && !ddlWOtherLangPro.SelectedValue.Equals(""))
            {
                wlangScore += ddlWOtherLanguage.SelectedValue + ":" + ddlWOtherLangPro.SelectedValue + ";";
            }

            string slangScore = "";
            slangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";
            slangScore += General_Constance.CHN + ":" + ddlChnPro.SelectedValue + ";";
            if (!ddlOtherLanguage.SelectedValue.Equals("") && !ddlOtherLangPro.SelectedValue.Equals(""))
            {
                slangScore += ddlOtherLanguage.SelectedValue + ":" + ddlOtherLangPro.SelectedValue + ";";
            }

            Trainee_Management tm = new Trainee_Management();
            Tuple<bool, string> success = tm.updateTraineeDetails(traineeId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2,
                emailAddress, race, birthDate, address, postalCode, highestEdu, highestEduRemarks, slangScore, wlangScore, LoginID);

            if (success.Item1)
            {
                btnCancelTrainee.Visible = false;
                btnEditTrainee.Visible = true;
                panelParticular.Enabled = false;
                panelSuccess.Visible = true;
                lblError.Text = "";
                lblSuccess.Text = success.Item2;
                loadTrainee(traineeId);
            }
            else
            {
                btnCancelTrainee.Visible = false;
                btnEditTrainee.Visible = true;
                panelParticular.Enabled = false;
                panelSuccess.Visible = false;
                panelError.Visible = true;
                lblError.Text = success.Item2;
                lblSuccess.Text = "";
                loadTrainee(traineeId);
            }
        }

        protected void btnWithdraw_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Trainee_Management()).withdrawTrainee(tbTraineeId.Text, LoginID, tbReason.Text);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnCfmWithdrawTrainee.Visible = false;
                btnCancelTrainee.Visible = false;
                btnEditTrainee.Visible = false;
                // panelUpdateTraineeDetails.Visible = false;
                panelParticular.Enabled = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = false;
            }
        }



    }


}