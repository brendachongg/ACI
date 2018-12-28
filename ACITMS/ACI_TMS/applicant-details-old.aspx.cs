using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ACI_TMS
{
    public partial class applicant_details_old : BasePage
    {
        public const string PAGE_NAME = "applicant-details.aspx";

        public const string APPLICANT_QUERY = "a";

        public applicant_details_old()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_EDIT, applicant.PAGE_NAME)
        {

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


        DataTable dtApplicantDetails = new DataTable();
        DataTable dtEmploymentDetails = new DataTable();
        DataTable dtInterviewDetails = new DataTable();
        DataTable dtPaymentHistory = new DataTable();

        DataTable dtCourse = new DataTable();
        DataTable dtProgrammeBatchInfo = new DataTable();

        DataTable exemptedModule = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string applicantId = Request.QueryString[APPLICANT_QUERY];

                    if (!applicantId.Equals(""))
                    {
                        //loadCourseBatchInformation();
                        loadInterviewer();
                        loadAllProgrammeBatchInfo();
                        loadSponsorshipType();
                        loadAllChannelInfo();
                        loadAllEducationReferenceInfo();
                        //loadAllLanguageProficiencyInfo();
                        loadStep5Fields();
                        loadAllTypeOfIdRefereceInfo();
                        loadAllNationalityRefereceInfo();
                        loadAllRaceRefereceInfo();
                        //loadOccupationCodeReferenceInfo();
                        //loadAllEmploymentStatusInfo();
                        loadLanguageProficiency(ddlEngPro);
                        loadLanguageProficiency(ddlChnPro);
                        loadLanguageProficiency(ddlOtherLangPro);
                        loadLanguageProficiency(ddlWChiPro);
                        loadLanguageProficiency(ddlWEngPro);
                        loadLanguageProficiency(ddlWOtherLangPro);
                        loadOtherLanguage();


                        loadApplicationDetails(applicantId);

                        btnISNotYetDone.CommandName = InterviewStatus.NYD.ToString();
                        btnISPending.CommandName = InterviewStatus.PD.ToString();
                        btnISPass.CommandName = InterviewStatus.PASSED.ToString();
                        btnISFail.CommandName = InterviewStatus.FAILED.ToString();
                        btnNotRequired.CommandName = InterviewStatus.NREQ.ToString();
                    }
                }
                else
                {
                    panelError.Visible = false;
                    panelSuccess.Visible = false;
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving applicant details.");
            }
        }

        private void getTraineeDetails(string nric)
        {
            Trainee_Management tm = new Trainee_Management();
            DataTable dtTraineeDetails = tm.getTraineeDetailsByTraineeNRIC(nric);

            if (dtTraineeDetails != null)
            {
                tbFullNameValue.Text = dtTraineeDetails.Rows[0]["fullName"].ToString();

                tbIdentificationValue.Text = dtTraineeDetails.Rows[0]["idNumber"].ToString();

                ddlIdentificationType.SelectedValue = dtTraineeDetails.Rows[0]["idType"].ToString();

                ddlNationalityValue.SelectedValue = dtTraineeDetails.Rows[0]["nationality"].ToString();

                ddlGenderValue.SelectedValue = dtTraineeDetails.Rows[0]["gender"].ToString();

                ddlRaceValue.SelectedValue = dtTraineeDetails.Rows[0]["race"].ToString();

                tbContact1Value.Text = dtTraineeDetails.Rows[0]["contactNumber1"].ToString();

                tbContact2Value.Text = dtTraineeDetails.Rows[0]["contactNumber2"].ToString();

                tbEmailAddressValue.Text = dtTraineeDetails.Rows[0]["emailAddress"].ToString();

                tbBirthDateValue.Text = dtTraineeDetails.Rows[0]["birthDateDisplay"].ToString();

                tbAddressLine1Value.Text = dtTraineeDetails.Rows[0]["addressLine"].ToString();

                tbPostalCodeValue.Text = dtTraineeDetails.Rows[0]["postalCode"].ToString();

                ddlHighestEducationValue.SelectedValue = dtTraineeDetails.Rows[0]["highestEducation"].ToString();
                tbHighestEducationRemarkValue.Text = dtTraineeDetails.Rows[0]["highestEduRemarks"].ToString();

                string writtenLanguage = dtTraineeDetails.Rows[0]["writtenLanguage"].ToString();

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

                        //foreach (RepeaterItem item in rptWrittenLanguage.Items)
                        //{
                        //    //Label lb = (Label)item.FindControl("lbSLanguages");
                        //    HiddenField hdf = (HiddenField)item.FindControl("hdfWLangCodeValue");
                        //    DropDownList ddl = (DropDownList)item.FindControl("ddlWLanguagesPro");
                        //    if (hdf.Value.Equals(wLangPro[0].ToString()))
                        //    {
                        //        ddl.SelectedValue = wLangPro[1].ToString();
                        //    }
                        //}
                    }
                }

                //Spoken Language
                string spokenLanguage = dtTraineeDetails.Rows[0]["spokenLanguage"].ToString();


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

                string channel = dtApplicantDetails.Rows[0]["getToKnowChannel"].ToString();
                if (!channel.Equals("") || !channel.Equals("Empty"))
                {
                    string[] knowChannel = channel.Replace(" ", "").Split(',');
                    foreach (string kc in knowChannel)
                    {
                        foreach (ListItem i in cblGetToKnowChannel.Items)
                        {
                            if (i.Value.Equals(kc))
                            {
                                i.Selected = true;
                            }
                        }
                    }
                }

                //string writtenLanguage = dtTraineeDetails.Rows[0]["writtenLanguage"].ToString();

                //if (!writtenLanguage.Equals("") && !writtenLanguage.Equals("Empty"))
                //{
                //    string[] wLangSet = writtenLanguage.Split(';');
                //    foreach (string set in wLangSet)
                //    {
                //        string[] wLangPro = set.Split(':');

                //        //foreach (RepeaterItem item in rptWrittenLanguage.Items)
                //        //{
                //        //    //Label lb = (Label)item.FindControl("lbSLanguages");
                //        //    HiddenField hdf = (HiddenField)item.FindControl("hdfWLangCodeValue");
                //        //    DropDownList ddl = (DropDownList)item.FindControl("ddlWLanguagesPro");
                //        //    if (hdf.Value.Equals(wLangPro[0].ToString()))
                //        //    {
                //        //        ddl.SelectedValue = wLangPro[1].ToString();
                //        //    }
                //        //}
                //    }
                //}

                ////Spoken Language
                //string spokenLanguage = dtTraineeDetails.Rows[0]["spokenLanguage"].ToString();


                //if (!spokenLanguage.Equals("") && !spokenLanguage.Equals("Empty"))
                //{
                //    string[] sLangSet = spokenLanguage.Split(';');
                //    foreach (string set in sLangSet)
                //    {
                //        string[] sLangPro = set.Split(':');

                //        foreach (RepeaterItem item in rptSpokenLanguage.Items)
                //        {
                //            //Label lb = (Label)item.FindControl("lbSLanguages");
                //            HiddenField hdf = (HiddenField)item.FindControl("hdfSLangCodeValue");
                //            DropDownList ddl = (DropDownList)item.FindControl("ddlSLanguagesPro");
                //            if (hdf.Value.Equals(sLangPro[0].ToString()))
                //            {
                //                ddl.SelectedValue = sLangPro[1].ToString();
                //            }
                //        }
                //    }
                //}

                //string channel = dtTraineeDetails.Rows[0]["getToKnowChannel"].ToString();
                //if (!channel.Equals("") || !channel.Equals("Empty"))
                //{
                //    string[] knowChannel = channel.Replace(" ", "").Split(',');
                //    foreach (string kc in knowChannel)
                //    {
                //        foreach (ListItem i in cblGetToKnowChannel.Items)
                //        {
                //            if (i.Value.Equals(kc))
                //            {
                //                i.Selected = true;
                //            }
                //        }
                //    }


                //}

            }
        }



        //private void loadOccupationCodeReferenceInfo()
        //{
        //    Applicant_Management am = new Applicant_Management();
        //    DataTable dtOccupationCode = am.getOccupationCodeReference();

        //    ddlOccupationCode.DataSource = dtOccupationCode;
        //    ddlOccupationCode.DataTextField = "codeValueDisplay";
        //    ddlOccupationCode.DataValueField = "codeValue";
        //    ddlOccupationCode.DataBind();
        //    ddlOccupationCode.Items.Insert(0, new ListItem("--Select--", ""));


        //}


        ////Kareen: Added || Get Employment Status From Code Reference 

        //private void loadAllEmploymentStatusInfo()
        //{
        //    Applicant_Management am = new Applicant_Management();
        //    DataTable dtEmpStatus = am.getEmploymentStatus();

        //    ddlEmploymentStatus.DataSource = dtEmpStatus;
        //    ddlEmploymentStatus.DataTextField = "codeValueDisplay";
        //    ddlEmploymentStatus.DataValueField = "codeValue";
        //    ddlEmploymentStatus.DataBind();
        //    ddlEmploymentStatus.Items.Insert(0, new ListItem("--Select--", ""));


        //}

        private void loadAllRaceRefereceInfo()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtIdtype = am.getRaceCodeReference();
            ddlRaceValue.DataSource = dtIdtype;
            ddlRaceValue.DataTextField = "codeValueDisplay";
            ddlRaceValue.DataValueField = "codeValue";
            ddlRaceValue.DataBind();
            ddlRaceValue.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadAllNationalityRefereceInfo()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtIdtype = am.getIdentificationTypeCodeReference();
            ddlIdentificationType.DataSource = dtIdtype;
            ddlIdentificationType.DataBind();

        }

        private void loadAllTypeOfIdRefereceInfo()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtIdtype = am.getNationalityCodeReference();

            ddlNationalityValue.DataSource = dtIdtype;
            ddlNationalityValue.DataBind();
            ddlNationalityValue.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadInterviewer()
        {
            try
            {
                //ddlInterviewer
                ACI_Staff_User staff = new ACI_Staff_User();
                DataTable dtStaffUser = staff.getInterviewer();

                if (dtStaffUser.Rows.Count > 0)
                {
                    ddlInterviewer.DataTextField = "userName";
                    ddlInterviewer.DataValueField = "userId";

                    ddlInterviewer.DataSource = dtStaffUser;
                    ddlInterviewer.DataBind();
                    ddlInterviewer.Items.Insert(0, "Select an interviewer");
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error loading interviewer details.");
            }
        }

        //For display regardless of defuct batches
        private void loadAllProgrammeBatchInfo()
        {
            Batch_Session_Management cbm = new Batch_Session_Management();
            ViewState["dtProgrammeBatchInfo"] = dtProgrammeBatchInfo = cbm.getAllBatchForDisplay();

            ddlCourseAppliedValue.DataTextField = "ProgrammeCode";
            ddlCourseAppliedValue.DataValueField = "programmeBatchId";

            ddlCourseAppliedValue.DataSource = dtProgrammeBatchInfo;
            ddlCourseAppliedValue.DataBind();

            ddlCourseAppliedValue.Items.Insert(0, new ListItem("--Select--", ""));

            //ddlProjectCodeValue.DataTextField = "projectCode";
            //ddlProjectCodeValue.DataValueField = "programmeBatchId";

            //ddlProjectCodeValue.DataSource = dtProgrammeBatchInfo;
            //ddlProjectCodeValue.DataBind();

        }

        private void loadSponsorshipType()
        {
            //ddlSponsorship.Items.Add(new ListItem("Self-Sponsored", Sponsorship.SELF.ToString()));
            //ddlSponsorship.Items.Add(new ListItem("Company-Sponsored", Sponsorship.COMP.ToString()));
            Applicant_Management am = new Applicant_Management();
            //Sponsorship Code
            DataTable dtSponsorshipCode = am.getSponsorship();
            ddlSponsorship.DataSource = dtSponsorshipCode;
            ddlSponsorship.DataBind();
        }

        private void loadOtherLanguage()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtOtherLang = am.getOtherLanguageCodeReference();


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

        private void loadAllEducationReferenceInfo()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dt = am.getAllEducationCodeReference();

            ViewState["dtEducationReferenceInfo"] = dt;

            ddlHighestEducationValue.DataTextField = "codeValueDisplay";
            ddlHighestEducationValue.DataValueField = "codeValue";

            ddlHighestEducationValue.DataSource = dt;
            ddlHighestEducationValue.DataBind();

            ddlHighestEducationValue.Items.Insert(0, new ListItem("--Select--", ""));


        }


        //private void loadAllLanguageProficiencyInfo()
        //{
        //    Applicant_Management am = new Applicant_Management();

        //    DataTable dtLang = am.getAllLanguageCodeReference();
        //    rptSpokenLanguage.DataSource = dtLang;
        //    rptSpokenLanguage.DataBind();

        //    //rptWrittenLanguage.DataSource = dtLang;
        //    //rptWrittenLanguage.DataBind();



        //}

        private void loadAllChannelInfo()
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtChannel = am.getAllGetToKnowChannelCodeReference();

            cblGetToKnowChannel.DataTextField = "codeValueDisplay";
            cblGetToKnowChannel.DataValueField = "codeValue";

            cblGetToKnowChannel.DataSource = dtChannel;
            cblGetToKnowChannel.DataBind();
        }

        //protected void rptSpokenLanguage_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    Applicant_Management am = new Applicant_Management();
        //    DataTable dtLangfPr = am.getAllLanguageProficiencyCodeReference();
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        DropDownList ddlSLangPr = (DropDownList)e.Item.FindControl("ddlSLanguagesPro");
        //        ddlSLangPr.DataTextField = "codeValueDisplay";
        //        ddlSLangPr.DataValueField = "codeValue";

        //        ddlSLangPr.DataSource = dtLangfPr;
        //        ddlSLangPr.DataBind();

        //        ddlSLangPr.Items.Insert(0, "Select");
        //    }
        //}

        private void loadLanguageProficiency(object sender)
        {
            Applicant_Management am = new Applicant_Management();
            DropDownList ddl = (DropDownList)sender;
            DataTable dtLangfPr = am.getAllLanguageProficiencyCodeReference();
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

        //protected void rptWrittenLanguage_ItemDataBound(object sender, RepeaterItemEventArgs e)
        //{
        //    Applicant_Management am = new Applicant_Management();
        //    DataTable dtLangfPr = am.getAllLanguageProficiencyCodeReference();
        //    if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
        //    {
        //        DropDownList ddlWLangPr = (DropDownList)e.Item.FindControl("ddlWLanguagesPro");
        //        ddlWLangPr.DataTextField = "codeValueDisplay";
        //        ddlWLangPr.DataValueField = "codeValue";

        //        ddlWLangPr.DataSource = dtLangfPr;
        //        ddlWLangPr.DataBind();

        //        ddlWLangPr.Items.Insert(0, "Select");
        //    }
        //}

        protected void cblGetToKnowChannel_DataBound(object sender, EventArgs e)
        {

        }

        //private string setApplicantAppliedProgramme(string programmeBatchId)
        //{

        //    DataTable dt = ViewState["dtProgrammeBatchInfo"] as DataTable;
        //    DataRow[] result = dt.Select("programmeBatchId = '" + programmeBatchId + "'");

        //    ddlCourseAppliedValue.SelectedValue = programmeBatchId;
        //    //ddlProjectCodeValue.SelectedValue = programmeBatchId;
        //    tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();
        //    tbCourseStartDate.Text = result[0]["programmeStartDateDisplay"].ToString();

        //    if (DateTime.Parse(result[0]["programmeStartDateDisplay"].ToString()) < DateTime.Now.Date)
        //    {
        //        btnEditCourseDetails.Disabled = true;
        //        lkbtnEnrollApplicantTop.Enabled = false;
        //        lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info disabled";
        //        lkbtnEnrollApplicantBottom.Enabled = false;
        //        lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info disabled";
        //        panelError.Visible = true;
        //        lblErrorMsg.Text = "The enrollment for the selected programme have closed";
        //    }
        //    else
        //    {
        //        //Load the batches that is open for registration and enrollment
        //        Batch_Session_Management cbm = new Batch_Session_Management();
        //        ViewState["dtProgrammeBatchInfo"] = dtProgrammeBatchInfo = cbm.getAllBatchForRegistration();

        //        ddlCourseAppliedValue.DataTextField = "programmeTitle";
        //        ddlCourseAppliedValue.DataValueField = "programmeBatchId";

        //        ddlCourseAppliedValue.DataSource = dtProgrammeBatchInfo;
        //        ddlCourseAppliedValue.DataBind();

        //        //ddlProjectCodeValue.DataTextField = "projectCode";
        //        //ddlProjectCodeValue.DataValueField = "programmeBatchId";

        //        //ddlProjectCodeValue.DataSource = dtProgrammeBatchInfo;
        //        //ddlProjectCodeValue.DataBind();
        //    }

        //    return result[0]["programmeType"].ToString();

        //}

        //Dropdownlist on selected index changed for course applied
        protected void ddlCourseAppliedValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddlProgramme = (DropDownList)sender;

            string selectedCourseCode = ddlProgramme.SelectedItem.ToString();
            string selectProgrammeBatchId = ddlProgramme.SelectedValue;

            DataTable dt = ViewState["dtProgrammeBatchInfo"] as DataTable;


            DataRow[] result = dt.Select("programmeBatchId = '" + selectProgrammeBatchId + "'");

            //tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();

            //ddlProjectCodeValue.SelectedValue = selectProgrammeBatchId;

            //tbCourseStartDate.Text = result[0]["programmeStartDateDisplay"].ToString();

            if (result.Length > 0)
            {

                tbCourseStartDate.Text = result[0]["programmeStartDateDisplay"].ToString();

                tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();
            }
            else
            {
                dt = ViewState["dtPastCourseProgram"] as DataTable;
                result = dt.Select("programmeBatchId = '" + selectProgrammeBatchId + "'");
                tbCourseStartDate.Text = Convert.ToDateTime(result[0]["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");




                tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();

            }
            //loadProjectCode(selectedCourseCode);


        }

        //Dropdownlist on selected index changed for project code
        protected void ddlProjectCodeValue_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbCourseStartDate.Text = "";

            DropDownList ddlProjectCode = (DropDownList)sender;

            string selectedProjectCode = ddlProjectCode.SelectedItem.ToString();
            string selectProgrammeBatchId = ddlProjectCode.SelectedValue;

            DataTable dt = ViewState["dtProgrammeBatchInfo"] as DataTable;

            ddlCourseAppliedValue.SelectedValue = selectProgrammeBatchId;

            DataRow[] result = dt.Select("projectCode = '" + selectedProjectCode + "'");

            if (result.Length > 0)
            {

                tbCourseStartDate.Text = result[0]["programmeStartDateDisplay"].ToString();

                tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();
            }
            else
            {
                dt = ViewState["dtPastCourseProgram"] as DataTable;
                result = dt.Select("projectCode = '" + selectedProjectCode + "'");
                tbCourseStartDate.Text = Convert.ToDateTime(result[0]["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");


                tbCourseCodeValue.Text = result[0]["SSGRefNum"].ToString();

            }
        }

        public void loadApplicationDetails(string applicantId)
        {
            Applicant_Management am = new Applicant_Management();

            Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> applicantTuple = am.getApplicationDetailsByApplicantId(applicantId);
            ViewState["dtApplicantDetails"] = dtApplicantDetails = applicantTuple.Item1;
            ViewState["dtEmploymentDetails"] = dtEmploymentDetails = applicantTuple.Item2;
            ViewState["dtInterviewDetails"] = dtInterviewDetails = applicantTuple.Item3;
            ViewState["dtPaymentHistory"] = dtPaymentHistory = applicantTuple.Item4;
            ViewState["listExemptedModule"] = exemptedModule = applicantTuple.Item5;

            //string registrationFeePaymentId = "";
            //decimal coursePayableAmount = 0;
            //int enableEnrolCounter = 0;
            string programmeType = "";

            if (dtApplicantDetails.Rows.Count > 0)
            {

                //Setting application details

                lbApplicantId.Text = applicantId;

                lbApplicationSubmittedDate.Text = dtApplicantDetails.Rows[0]["applicationDate"].ToString();

                //ddlCourseAppliedValue.SelectedItem.Text = dtApplicantDetails.Rows[0]["programmeTitle"].ToString();

                //tbCourseCodeValue.Text = dtApplicantDetails.Rows[0]["courseCode"].ToString();

                //loadProjectCode(dtApplicantDetails.Rows[0]["projectCode"].ToString());

                //ddlProjectCodeValue.SelectedValue = dtApplicantDetails.Rows[0]["projectCode"].ToString();

                //tbCourseStartDate.Text = dtApplicantDetails.Rows[0]["programmeStartDateDisplay"].ToString();

                //Kareen: Check if the applicant applied course is within registraiton date

                DataTable dt = ViewState["dtProgrammeBatchInfo"] as DataTable;
                string applicantAppliedID = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();
                bool result = false;

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["programmeBatchId"].ToString().Equals(applicantAppliedID))
                    {
                        result = true;
                        break;
                    }
                }

                if (result == false)
                {
                    Batch_Session_Management cbm = new Batch_Session_Management();
                   // DataTable programDT = cbm.getProgrammeBatchByProgrammeBatchId(applicantAppliedID);

                //    if (programDT.Rows.Count > 0)
                //    {

                //        ViewState["dtPastCourseProgram"] = programDT;

                //        ddlCourseAppliedValue.Items.Insert(ddlCourseAppliedValue.Items.Count, new ListItem(programDT.Rows[0]["ProgrammeCode"].ToString(), programDT.Rows[0]["programmeBatchId"].ToString()));
                //        ddlCourseAppliedValue.SelectedValue = applicantAppliedID;


                //        //ddlProjectCodeValue.DataTextField = "projectCode";
                //        //ddlProjectCodeValue.DataValueField = "programmeBatchId";

                //        //ddlProjectCodeValue.DataSource = programDT;
                //        //ddlProjectCodeValue.DataBind();

                //        // ddlProjectCodeValue.Items.Insert(0, new ListItem(programDT.Rows[0]["projectCode"].ToString(), programDT.Rows[0]["programmeBatchId"].ToString()));

                //        tbCourseCodeValue.Text = programDT.Rows[0]["SSGRefNum"].ToString();
                //        tbCourseStartDate.Text = Convert.ToDateTime(programDT.Rows[0]["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");



                //        DataRow[] rs = programDT.Select("programmeBatchId = '" + dtApplicantDetails.Rows[0]["programmeBatchId"].ToString() + "'");
                //        programmeType = rs[0]["programmeType"].ToString();
                //        ViewState["programmeBatchId"] = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();


                //        dt.Merge(programDT);
                //        ViewState["dtProgrammeBatchInfo"] = dt;
                //    }
                //    else
                //    {
                //        ddlCourseAppliedValue.SelectedValue = "";

                //        tbCourseCodeValue.Text = "";
                //        tbCourseStartDate.Text = "";


                //        lbCourseError.Text = "Applicant's selected programme had started. Please select another programe for applicant.";
                //        lbCourseError.Visible = true;

                //    }


                //}
                //else
                //{
                //    //programmeType = setApplicantAppliedProgramme(dtApplicantDetails.Rows[0]["programmeBatchId"].ToString());
                //    ViewState["programmeBatchId"] = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();
                //    dt = ViewState["dtProgrammeBatchInfo"] as DataTable;
                //    DataRow[] rs;
                //    rs = dt.Select("programmeBatchId = '" + dtApplicantDetails.Rows[0]["programmeBatchId"].ToString() + "'");

                //    ddlCourseAppliedValue.SelectedValue = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();
                //    //ddlProjectCodeValue.SelectedValue = programmeBatchId;
                //    tbCourseCodeValue.Text = rs[0]["SSGRefNum"].ToString();
                //    tbCourseStartDate.Text = rs[0]["programmeStartDateDisplay"].ToString();
                //    programmeType = rs[0]["programmeType"].ToString();

                //    // programmeType = setApplicantAppliedProgramme(dtApplicantDetails.Rows[0]["programmeBatchId"].ToString());

                //}

                //Kareen: Commented

                //programmeType = setApplicantAppliedProgramme(dtApplicantDetails.Rows[0]["programmeBatchId"].ToString());

                //ViewState["programmeBatchId"] = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();

                //Kareen: End Commented
                //Setting applicant's particular

                tbFullNameValue.Text = dtApplicantDetails.Rows[0]["fullName"].ToString();

                tbIdentificationValue.Text = dtApplicantDetails.Rows[0]["idNumber"].ToString();

                ddlIdentificationType.SelectedValue = dtApplicantDetails.Rows[0]["idType"].ToString();

                ddlNationalityValue.SelectedValue = dtApplicantDetails.Rows[0]["nationality"].ToString();

                ddlGenderValue.SelectedValue = dtApplicantDetails.Rows[0]["gender"].ToString();

                ddlRaceValue.SelectedValue = dtApplicantDetails.Rows[0]["race"].ToString();

                tbContact1Value.Text = dtApplicantDetails.Rows[0]["contactNumber1"].ToString();

                tbContact2Value.Text = dtApplicantDetails.Rows[0]["contactNumber2"].ToString();

                tbEmailAddressValue.Text = dtApplicantDetails.Rows[0]["emailAddress"].ToString();

                tbBirthDateValue.Text = dtApplicantDetails.Rows[0]["birthDateDisplay"].ToString();

                tbAddressLine1Value.Text = dtApplicantDetails.Rows[0]["addressLine"].ToString();

                tbPostalCodeValue.Text = dtApplicantDetails.Rows[0]["postalCode"].ToString();


                ddlSponsorship.SelectedValue = dtApplicantDetails.Rows[0]["selfSponsored"].ToString();



                if (dtApplicantDetails.Rows[0]["rejectStatus"].ToString().Equals(General_Constance.STATUS_YES))
                {
                    btnEditApplicant.Disabled = true;
                    btnEditCourseDetails.Disabled = true;
                    btnEditInterviewDetails.Disabled = true;
                    btnExemptedModule.Disabled = true;
                    //btnAddEmployment.Enabled = false;
                    btnPayRegistrationFee.Enabled = false;
                    btnPayCourseFee.Enabled = false;
                    lkbtnEnrollApplicantTop.Enabled = false;
                    lkbtnEnrollApplicantBottom.Enabled = false;
                    btnRejectAppcalicantTop.Enabled = false;
                    btnRejectAppcalicantBottom.Enabled = false;
                    btnSaveRemarks.Enabled = false;
                }

                ddlHighestEducationValue.SelectedValue = dtApplicantDetails.Rows[0]["highestEducation"].ToString();


                tbHighestEducationRemarkValue.Text = dtApplicantDetails.Rows[0]["highestEduRemarks"].ToString();

                //Written Language
                string writtenLanguage = dtApplicantDetails.Rows[0]["writtenLanguage"].ToString();

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

                        //foreach (RepeaterItem item in rptWrittenLanguage.Items)
                        //{
                        //    //Label lb = (Label)item.FindControl("lbSLanguages");
                        //    HiddenField hdf = (HiddenField)item.FindControl("hdfWLangCodeValue");
                        //    DropDownList ddl = (DropDownList)item.FindControl("ddlWLanguagesPro");
                        //    if (hdf.Value.Equals(wLangPro[0].ToString()))
                        //    {
                        //        ddl.SelectedValue = wLangPro[1].ToString();
                        //    }
                        //}
                    }
                }

                //Spoken Language
                string spokenLanguage = dtApplicantDetails.Rows[0]["spokenLanguage"].ToString();


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

                string channel = dtApplicantDetails.Rows[0]["getToKnowChannel"].ToString();
                if (!channel.Equals("") || !channel.Equals("Empty"))
                {
                    string[] knowChannel = channel.Replace(" ", "").Split(',');
                    foreach (string kc in knowChannel)
                    {
                        foreach (ListItem i in cblGetToKnowChannel.Items)
                        {
                            if (i.Value.Equals(kc))
                            {
                                i.Selected = true;
                            }
                        }
                    }


                }

                lbLastModifyDateTimeValue.Text = dtApplicantDetails.Rows[0]["lastModifiedDate"].ToString();

                tbApplicantRemarksValue.Text = dtApplicantDetails.Rows[0]["applicantRemarks"].ToString();

                //Interview status
                //if (dtApplicantDetails.Rows[0]["shortlistStatus"].ToString().Equals(General_Constance.STATUS_YES))
                //{
                //    cbShortlisted.Checked = true;
                //}
                //else
                //{
                //    enableEnrolCounter++;
                //}

                lbInterviewStatusValue.Text = dtApplicantDetails.Rows[0]["interviewStatus"].ToString();

                //Exempted module
                rptExemptedModule.DataSource = null;
                rptExemptedModule.DataBind();


                if (exemptedModule.Rows.Count > 0)
                {
                    rptExemptedModule.DataSource = exemptedModule;
                    rptExemptedModule.DataBind();

                    lbNoExemptedModuleMsg.Visible = false;
                }
                else
                {
                    lbNoExemptedModuleMsg.Visible = true;
                }

                int enableExemptionAccess = 0;

                if (dtPaymentHistory.Rows.Count > 0)
                {
                    foreach (DataRow drPayment in dtPaymentHistory.Rows)
                    {
                        string payType = drPayment["paymentType"].ToString();
                        string payStatus = drPayment["paymentStatus"].ToString();

                        //Check if payment status is not void
                        if (!payStatus.Equals(PaymentStatus.VOID.ToString()) && (payType.Equals(PaymentType.PROG.ToString()) || payType.Equals(PaymentType.BOTH.ToString())))
                        {
                            enableExemptionAccess++;
                        }
                        //else if (payStatus.Equals(PaymentStatus.VOID.ToString()) && !payType.Equals(PaymentType.REG.ToString()))
                        //{
                        //    
                        //}

                    }
                }
                else
                {
                    enableExemptionAccess = 0;
                }

                if (enableExemptionAccess == 0)
                {
                    btnExemptedModule.Disabled = false;
                }
                else
                {
                    btnExemptedModule.Disabled = true;
                }

                //Registration fee payment
                //if (dtApplicantDetails.Rows[0]["registrationFeePaymentId"].ToString().Equals(null) || dtApplicantDetails.Rows[0]["registrationFeePaymentId"].ToString().Equals(""))
                //{
                //    btnViewRegistationFeeReceipt.Visible = false;
                //    btnPayRegistrationFee.Visible = true;

                //    lbRegistrationFeeStatus.Text = "Incomplete";
                //    enableEnrolCounter++;                    
                //}
                //else
                //{
                //    btnViewRegistationFeeReceipt.Visible = true;
                //    btnPayRegistrationFee.Visible = false;

                //    registrationFeePaymentId = dtApplicantDetails.Rows[0]["registrationFeePaymentId"].ToString();
                //    hdfRegistrationPaymentId.Value = dtApplicantDetails.Rows[0]["registrationFeePaymentId"].ToString();

                //    lbRegistrationFeeStatus.Text = "Completed";

                //}

                //initialize with MultiView1.ActiveViewIndex = 0
                //view 1 is for FQ, separate payment of reg fee and course fee, MultiView1.ActiveViewIndex = 0;

                //view 2 is for short course, only course fee, MultiView1.ActiveViewIndex = 1;

                //view 3 is for FQ, combine payment of both, MultiView1.ActiveViewIndex = 2;

                panelError.Visible = false;
                lblErrorMsg.Text = "";

                bool SponsorRecord = false;
                if (ddlSponsorship.SelectedValue == Sponsorship.COMP.ToString())
                {
                    if (dtEmploymentDetails.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtEmploymentDetails.Rows)
                        {
                            if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                            {
                                tbSponsorshipCompany.Text = dr["companyName"].ToString();
                                SponsorRecord = true;
                                break;
                            }
                        }
                    }

                    if (SponsorRecord == false)
                    {
                        panelError.Visible = true;
                        lblErrorMsg.Text = "Your have selected the sponsorship under your company, please ensure that you have enter your employment record before enrol." + "<br>";
                    }
                }
                else if (ddlSponsorship.SelectedValue == Sponsorship.SELF.ToString())
                {
                    SponsorRecord = true;
                }


                if (programmeType == ProgrammeType.FQ.ToString())
                {
                    decimal totalAmountPaid = 0;
                    decimal regFeesPaidSoFar = 0;
                    decimal programmeFeePaidSoFar = 0;

                    if (dtPaymentHistory.Rows.Count > 0)
                    {
                        foreach (DataRow drPayment in dtPaymentHistory.Rows)
                        {
                            string payType = drPayment["paymentType"].ToString();
                            string payStatus = drPayment["paymentStatus"].ToString();

                            //If the payment status is not void.
                            if (!payStatus.Equals(PaymentStatus.VOID.ToString()))
                            {
                                if (payType.Equals(PaymentType.BOTH.ToString()))
                                {
                                    MultiView1.ActiveViewIndex = 2;

                                    btnRnCPayment.Visible = true;
                                    //lblRnCStatus.Text = "Completed";
                                    cb_combinepayment.Visible = false;

                                    totalAmountPaid += Convert.ToDecimal(drPayment["paymentAmount"].ToString());
                                    ViewState["PaymentStatus"] = PaymentType.BOTH.ToString();
                                }
                                else
                                {
                                    ViewState["PaymentStatus"] = PaymentType.PROG.ToString() + "&" + PaymentType.REG.ToString();

                                    if (payType.Equals(PaymentType.REG.ToString()))
                                    {
                                        MultiView1.ActiveViewIndex = 0;

                                        btnPayRegistrationFee.Enabled = true;
                                        btnPayCourseFee.Enabled = true;
                                        totalAmountPaid += Convert.ToDecimal(drPayment["paymentAmount"].ToString());
                                        regFeesPaidSoFar += Convert.ToDecimal(drPayment["paymentAmount"].ToString());

                                    }

                                    if (payType.Equals(PaymentType.PROG.ToString()))
                                    {
                                        MultiView1.ActiveViewIndex = 0;

                                        btnPayRegistrationFee.Enabled = true;
                                        btnPayCourseFee.Enabled = true;
                                        totalAmountPaid += Convert.ToDecimal(drPayment["paymentAmount"].ToString());
                                        programmeFeePaidSoFar += Convert.ToDecimal(drPayment["paymentAmount"].ToString());
                                    }

                                }

                            }
                        } //end of For Each Loop

                        if (ViewState["PaymentStatus"].ToString() == PaymentType.PROG.ToString() + "&" + PaymentType.REG.ToString())
                        {
                            decimal payableCourseFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["programmePayableAmount"].ToString());
                            decimal regGst = 0;
                            if (!dtApplicantDetails.Rows[0]["registrationFee"].ToString().Equals(""))
                            {
                                regGst = Math.Round(decimal.Parse(dtApplicantDetails.Rows[0]["registrationFee"].ToString()) * General_Constance.GST_RATE, 2);
                            }
                            decimal gstFee = 0;
                            decimal regFees = 0;

                            if (!dtApplicantDetails.Rows[0]["registrationFee"].ToString().Equals(""))
                            {
                                regFees = Convert.ToDecimal(dtApplicantDetails.Rows[0]["registrationFee"].ToString());
                            }
                            if (!dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString().Equals(""))
                            {
                                gstFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString());
                            }

                            decimal amoutSubsidised = 0;

                            if (!dtApplicantDetails.Rows[0]["subsidyAmt"].ToString().Equals(""))
                            {
                                amoutSubsidised = Convert.ToDecimal(dtApplicantDetails.Rows[0]["subsidyAmt"].ToString());
                            }

                            //decimal regFeePayable = Convert.ToDecimal(dtApplicantDetails.Rows[0]["registrationFee"].ToString());

                            decimal outstandingBalance = (payableCourseFee - amoutSubsidised + gstFee + regGst + regFees) - totalAmountPaid;

                            lbTotalAmountPaidValue.Text = totalAmountPaid.ToString("C");
                            lbOutstandingAmountValue.Text = outstandingBalance.ToString("C");

                            decimal outstandingRegFees = (regFees + regGst - regFeesPaidSoFar);

                            if (outstandingRegFees <= 0)
                            {
                                lbRegistrationFeeStatus.Text = "Completed";
                            }
                            else
                            {
                                lbRegistrationFeeStatus.Text = "Incomplete";
                            }

                            decimal outstandingProgrammeFees = (payableCourseFee - amoutSubsidised + gstFee) - programmeFeePaidSoFar;

                            if (outstandingProgrammeFees <= 0)
                            {
                                lbCourseFeeStaus.Text = "Completed";
                            }
                            else
                            {
                                lbCourseFeeStaus.Text = "Incomplete";
                            }

                            //Cleared all payment , enable enrolment button
                            if (outstandingBalance <= 0)
                            {
                                //lbOutstandingAmountValue.Text = "0.00";

                                lkbtnEnrollApplicantTop.Enabled = true;
                                lkbtnEnrollApplicantBottom.Enabled = true;
                                lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info";
                                lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info";
                            }
                            else
                            {


                                lkbtnEnrollApplicantTop.Enabled = false;
                                lkbtnEnrollApplicantBottom.Enabled = false;
                                lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info disabled";
                                lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info disabled";
                            }

                        }
                        else if (ViewState["PaymentStatus"].ToString() == PaymentType.BOTH.ToString())
                        {
                            decimal payableCourseFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["programmePayableAmount"].ToString());
                            decimal regFees = 0;

                            if (!dtApplicantDetails.Rows[0]["registrationFee"].ToString().Equals(""))
                            {

                                regFees = Convert.ToDecimal(dtApplicantDetails.Rows[0]["registrationFee"].ToString());
                            }

                            decimal gstFee = 0;
                            if (!dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString().Equals(""))
                            {
                                gstFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString());
                            }

                            decimal amoutSubsidised = 0;

                            if (!dtApplicantDetails.Rows[0]["subsidyAmt"].ToString().Equals(""))
                            {
                                amoutSubsidised = Convert.ToDecimal(dtApplicantDetails.Rows[0]["subsidyAmt"].ToString());
                            }

                            //decimal regFeePayable = Convert.ToDecimal(dtApplicantDetails.Rows[0]["registrationFee"].ToString());

                            decimal outstandingBalance = (payableCourseFee - amoutSubsidised + gstFee + regFees) - totalAmountPaid;

                            lbTotalAmountPaidValue.Text = totalAmountPaid.ToString("C");
                            lbOutstandingAmountValue.Text = outstandingBalance.ToString("C");


                            //Cleared all payment , enable enrolment button
                            if (outstandingBalance <= 0)
                            {
                                //lbOutstandingAmountValue.Text = "0.00";
                                lblRnCStatus.Text = "Completed";

                                lkbtnEnrollApplicantTop.Enabled = true;
                                lkbtnEnrollApplicantBottom.Enabled = true;
                                lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info";
                                lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info";
                            }
                            else
                            {
                                lblRnCStatus.Text = "Incomplete";

                                lkbtnEnrollApplicantTop.Enabled = false;
                                lkbtnEnrollApplicantBottom.Enabled = false;
                                lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info disabled";
                                lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info disabled";
                            }

                        }

                    }
                    else  //No Payment Records Found
                    {
                        MultiView1.ActiveViewIndex = 0;

                        cb_combinepayment.Visible = true;

                        btnPayRegistrationFee.Enabled = true;
                        btnPayCourseFee.Enabled = true;
                        lbCourseFeeStaus.Text = "Incomplete";
                        lbRegistrationFeeStatus.Text = "Incomplete";
                    }
                }
                else if (programmeType == ProgrammeType.SCNWSQ.ToString() || programmeType == ProgrammeType.SCWSQ.ToString())
                {
                    MultiView1.ActiveViewIndex = 1;
                    cb_combinepayment.Visible = false;

                    decimal totalAmountPaid = 0;

                    if (dtPaymentHistory.Rows.Count > 0)
                    {
                        foreach (DataRow drPayment in dtPaymentHistory.Rows)
                        {
                            string payType = drPayment["paymentType"].ToString();
                            string payStatus = drPayment["paymentStatus"].ToString();

                            //Check if payment status is not void
                            if (!payStatus.Equals(PaymentStatus.VOID.ToString()))
                            {
                                if (payType.Equals(PaymentType.PROG.ToString()))
                                {
                                    //btnCourseFeeOnly.Enabled = true;

                                    //lblCourseFeeOnlyStatus.Text = "Completed";

                                    totalAmountPaid += Convert.ToDecimal(drPayment["paymentAmount"].ToString());
                                }
                            }
                            //else
                            //{
                            //    if (payType.Equals(PaymentType.PROG.ToString()))
                            //    {
                            //        btnCourseFeeOnly.Enabled = true;
                            //        lblCourseFeeOnlyStatus.Text = "Incomplete";

                            //        if (SponsorRecord == false)
                            //        {
                            //            btnCourseFeeOnly.Enabled = false;
                            //        }
                            //    }
                            //}
                        }
                    }
                    //No record of payment
                    else
                    {
                        btnCourseFeeOnly.Enabled = true;
                        lblCourseFeeOnlyStatus.Text = "Incomplete";

                        if (SponsorRecord == false)
                        {
                            btnCourseFeeOnly.Enabled = false;
                        }
                    }

                    decimal payableCourseFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["programmePayableAmount"].ToString());
                    decimal gstFee = 0;
                    if (!dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString().Equals(""))
                    {
                        gstFee = Convert.ToDecimal(dtApplicantDetails.Rows[0]["GSTPayableAmount"].ToString());
                    }

                    decimal amoutSubsidised = 0;

                    if (!dtApplicantDetails.Rows[0]["subsidyAmt"].ToString().Equals(""))
                    {
                        amoutSubsidised = Convert.ToDecimal(dtApplicantDetails.Rows[0]["subsidyAmt"].ToString());
                    }

                    decimal outstandingBalance = (payableCourseFee - amoutSubsidised + gstFee) - totalAmountPaid;

                    lbTotalAmountPaidValue.Text = totalAmountPaid.ToString("C");
                    lbOutstandingAmountValue.Text = outstandingBalance.ToString("C");


                    //Cleared all payment , enable enrolment button
                    if (outstandingBalance <= 0)
                    {
                        lbOutstandingAmountValue.Text = "0.00";
                        lblCourseFeeOnlyStatus.Text = "Completed";
                        lkbtnEnrollApplicantTop.Enabled = true;
                        lkbtnEnrollApplicantBottom.Enabled = true;
                        lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info";
                        lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info";
                    }
                    else
                    {
                        //lbOutstandingAmountValue.Text = outstandingBalance.ToString();
                        lblCourseFeeOnlyStatus.Text = "Incomplete";
                        lkbtnEnrollApplicantTop.Enabled = false;
                        lkbtnEnrollApplicantBottom.Enabled = false;
                        lkbtnEnrollApplicantTop.CssClass = "btn btn-sm btn-info disabled";
                        lkbtnEnrollApplicantBottom.CssClass = "btn btn-sm btn-info disabled";
                    }

                }

            }

            if (dtApplicantDetails.Rows[0]["selfSponsored"].ToString() == Sponsorship.COMP.ToString())
            {
                if (dtEmploymentDetails.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtEmploymentDetails.Rows)
                    {
                        if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                        {

                            tbSponsorshipCompany.Text = dr["companyName"].ToString();
                            break;
                        }
                    }
                }
            }

            //Employment details
            //if (dtEmploymentDetails.Rows.Count > 0)
            //{
            //    rptEmploymentDetails.DataSource = dtEmploymentDetails;
            //    rptEmploymentDetails.DataBind();

            //    lbLastModifyEmpDateTimeValue.Text = dtEmploymentDetails.Rows[0]["lastModifiedDate"].ToString();
            //    if (dtEmploymentDetails.Rows.Count < 2)
            //    {
            //        btnAddEmployment.Visible = true;
            //    }
            //    else
            //    {
            //        btnAddEmployment.Visible = false;
            //    }
            //}
            //else
            //{
            //    lbNoEmploymentHistoryMsg.Visible = true;
            //    btnAddEmployment.Visible = true;
            //}


            //Interview details
            if (dtInterviewDetails.Rows.Count > 0)
            {
                if (!dtApplicantDetails.Rows[0]["interviewStatus"].ToString().Equals(InterviewStatus.NREQ.ToString()))
                {
                    tbInterviewDateValue.Text = dtInterviewDetails.Rows[0]["interviewDateDisplay"].ToString();
                    tbInterviewRemarksValue.Text = dtInterviewDetails.Rows[0]["interviewRemarks"].ToString();
                    ddlInterviewer.SelectedValue = dtInterviewDetails.Rows[0]["interviewerId"].ToString();

                    if (dtApplicantDetails.Rows[0]["shortlistStatus"].ToString() == General_Constance.STATUS_YES)
                    {
                        cbShortlisted.Checked = true;
                    }

                }

            }

            if (dtApplicantDetails.Rows[0]["contactNumber1"].ToString().Equals("") && dtApplicantDetails.Rows[0]["emailAddress"].ToString().Equals("")
                && dtApplicantDetails.Rows[0]["addressLine"].ToString().Equals("") && dtApplicantDetails.Rows[0]["postalCode"].ToString().Equals(""))
            {
                getTraineeDetails(tbIdentificationValue.Text);
            }


            if (dtEmploymentDetails.Rows.Count > 0)
            {
                foreach (DataRow dr in dtEmploymentDetails.Rows)
                {
                    if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                    {
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


                        //divPrevEmpl.Attributes.CssStyle.Add("display", "block");
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
            }
        }

        protected void btnReceipt_Click(object sender, EventArgs e)
        {
        }

        protected void btnEditApplicant_ServerClick(object sender, EventArgs e)
        {
            btnEditApplicant.Visible = false;

            btnCancelApplicant.Visible = true;
            panelUpdateApplicantDetails.Visible = true;

            panelParticular.Enabled = true;
        }

        protected void btnCancelApplicant_ServerClick(object sender, EventArgs e)
        {
            btnCancelApplicant.Visible = false;

            btnEditApplicant.Visible = true;

            panelUpdateApplicantDetails.Visible = false;

            panelParticular.Enabled = false;

            lblErrorMsg.Text = "";

            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        protected void btnEditCourseDetails_ServerClick(object sender, EventArgs e)
        {
            panelCourseApplied.Enabled = true;

            btnCancelCourseDetails.Visible = true;

            btnEditCourseDetails.Visible = false;

            panelUpdateCourseDetails.Visible = true;
        }

        protected void btnCancelCourseDetails_ServerClick(object sender, EventArgs e)
        {
            panelCourseApplied.Enabled = false;

            btnCancelCourseDetails.Visible = false;

            btnEditCourseDetails.Visible = true;

            panelUpdateCourseDetails.Visible = false;

            string applicantId = Request.QueryString[APPLICANT_QUERY];

            loadApplicationDetails(applicantId);
        }


        protected void btnEditInterviewDetails_ServerClick(object sender, EventArgs e)
        {


            btnCancelInterviewDetails.Visible = true;

            btnEditInterviewDetails.Visible = false;

            panelInterviewDetails.Enabled = true;

            panelUpdateInterviewDetails.Visible = true;
        }

        protected void btnCancelInterviewDetails_ServerClick(object sender, EventArgs e)
        {


            btnCancelInterviewDetails.Visible = false;

            btnEditInterviewDetails.Visible = true;

            panelInterviewDetails.Enabled = false;

            panelUpdateInterviewDetails.Visible = false;

            string applicantId = Request.QueryString[APPLICANT_QUERY];

            loadApplicationDetails(applicantId);


        }

        protected void btnUpdateInterview_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName.Equals(InterviewStatus.PASSED.ToString()))
            {
                lbInterviewStatusValue.Text = InterviewStatus.PASSED.ToString();
            }

            if (e.CommandName.Equals(InterviewStatus.FAILED.ToString()))
            {
                lbInterviewStatusValue.Text = InterviewStatus.FAILED.ToString();
            }

            if (e.CommandName.Equals(InterviewStatus.NYD.ToString()))
            {
                lbInterviewStatusValue.Text = InterviewStatus.NYD.ToString();
            }

            if (e.CommandName.Equals(InterviewStatus.PD.ToString()))
            {
                lbInterviewStatusValue.Text = InterviewStatus.PD.ToString();
            }

            if (e.CommandName.Equals(InterviewStatus.NREQ.ToString()))
            {
                lbInterviewStatusValue.Text = InterviewStatus.NREQ.ToString();
            }
        }

        protected void rptEmploymentDetails_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item)
            {
                HiddenField hdfCurrentc = (HiddenField)e.Item.FindControl("hdfCurrent");

                Label lbTo = (Label)e.Item.FindControl("lbTo");
                if (hdfCurrentc.Value.Equals(General_Constance.STATUS_YES))
                {
                    lbTo.Text = "current";
                }
            }
        }

        //View selected employment details
        //protected void rptEmploymentDetails_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    HiddenField hdfCompany = (HiddenField)e.Item.FindControl("hdfCompany");
        //    HiddenField hdfPosition = (HiddenField)e.Item.FindControl("hdfPosition");
        //    HiddenField hdfSalary = (HiddenField)e.Item.FindControl("hdfSalary");

        //    HiddenField hdfStartDate = (HiddenField)e.Item.FindControl("hdfStartDate");

        //    HiddenField hdfEndDate = (HiddenField)e.Item.FindControl("hdfEndDate");

        //    HiddenField hdfEmploymentStatus = (HiddenField)e.Item.FindControl("hdfEmploymentStatus");


        //    HiddenField hdfOccupationCode = (HiddenField)e.Item.FindControl("hdfOccupationCode");

        //    HiddenField hdfOccupationRemarks = (HiddenField)e.Item.FindControl("hdfOccupationRemarks");
        //    HiddenField hdfCurrentEmp = (HiddenField)e.Item.FindControl("hdfCurrentEmp");


        //    DateTime sDate = DateTime.Parse(hdfStartDate.Value).Date;
        //    DateTime eDate = DateTime.Parse(hdfEndDate.Value).Date;


        //    if (rptEmploymentDetails.Items.Count > 0)
        //    {
        //        //cbSetCurrentEmployment.Visible = true;
        //        //if (e.CommandName.Equals(General_Constance.STATUS_YES))
        //        //{
        //        //    cbSetCurrentEmployment.Checked = true;
        //        //}
        //        //else
        //        //{                    
        //        //    cbSetCurrentEmployment.Checked = false;
        //        //}

        //        if (hdfCurrentEmp.Value.Equals(General_Constance.STATUS_YES))
        //        {
        //            cbSetCurrentEmployment.Checked = true;
        //            cbSetCurrentEmployment.Enabled = false;
        //            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), "text", "hide()", true);
        //        }
        //        else
        //        {
        //            cbSetCurrentEmployment.Checked = false;
        //            cbSetCurrentEmployment.Enabled = false;
        //            cbSetCurrentEmployment.Visible = false;
        //            ScriptManager.RegisterClientScriptBlock(this.Page, typeof(Page), "text", "hide()", true);
        //        }

        //        tbCompanyNameValue.Text = hdfCompany.Value;
        //        tbPositionValue.Text = hdfPosition.Value;
        //        tbSalaryValue.Text = hdfSalary.Value;
        //        tbStartDateValue.Text = sDate.ToString("dd MMM yyyy");
        //        if (e.CommandName.Equals(General_Constance.STATUS_YES))
        //        {
        //            tbEndDateValue.Text = "";
        //        }
        //        else
        //        {
        //            tbEndDateValue.Text = eDate.ToString("dd MMM yyyy");
        //        }
        //        hdfEmploymentHistoryId.Value = e.CommandArgument.ToString();
        //        ddlEmploymentStatus.SelectedValue = hdfEmploymentStatus.Value;
        //        ddlOccupationCode.SelectedValue = hdfOccupationCode.Value;
        //        tbOccupationRemarks.Text = hdfOccupationRemarks.Value;


        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModel", "openModel()", true);
        //    }

        //}

        private char generateCheckCode(char letter, string num)
        {

            // Extract the digits
            int digit7 = Convert.ToInt32(num.Substring(6, 1));
            int digit6 = Convert.ToInt32(num.Substring(5, 1));
            int digit5 = Convert.ToInt32(num.Substring(4, 1));
            int digit4 = Convert.ToInt32(num.Substring(3, 1));
            int digit3 = Convert.ToInt32(num.Substring(2, 1));
            int digit2 = Convert.ToInt32(num.Substring(1, 1));
            int digit1 = Convert.ToInt32(num.Substring(0, 1));

            int step1 = 0;

            if (letter == 'T' || letter == 'G')
            {
                step1 = digit1 * 2 + digit2 * 7 + digit3 * 6 + digit4 * 5 +
                       digit5 * 4 + digit6 * 3 + digit7 * 2 + 4;
            }
            else
            {
                step1 = digit1 * 2 + digit2 * 7 + digit3 * 6 + digit4 * 5 +
                       digit5 * 4 + digit6 * 3 + digit7 * 2;
            }


            int step2 = step1 % 11;
            int step3 = 11 - step2;
            char code = ' ';

            if (letter == 'S' || letter == 'T')
            {


                switch (step3)
                {
                    case 1: code = 'A'; break;
                    case 2: code = 'B'; break;
                    case 3: code = 'C'; break;
                    case 4: code = 'D'; break;
                    case 5: code = 'E'; break;
                    case 6: code = 'F'; break;
                    case 7: code = 'G'; break;
                    case 8: code = 'H'; break;
                    case 9: code = 'I'; break;
                    case 10: code = 'Z'; break;
                    case 11: code = 'J'; break;
                }


                return code;

            }
            else
            {

                switch (step3)
                {
                    case 1: code = 'K'; break;
                    case 2: code = 'L'; break;
                    case 3: code = 'M'; break;
                    case 4: code = 'N'; break;
                    case 5: code = 'P'; break;
                    case 6: code = 'Q'; break;
                    case 7: code = 'R'; break;
                    case 8: code = 'T'; break;
                    case 9: code = 'U'; break;
                    case 10: code = 'W'; break;
                    case 11: code = 'X'; break;
                }

                return code;

            }// end generateCheckCode 

        }


        // Extract the digits


        //Update applicant's details
        protected void btnUpdateApplicantDetails_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;

            string fullName = tbFullNameValue.Text;
            string idNumber = tbIdentificationValue.Text;
            string idType = ddlIdentificationType.SelectedValue;
            string nationality = ddlNationalityValue.SelectedValue;
            string gender = ddlGenderValue.SelectedValue;
            string race = ddlRaceValue.SelectedValue;
            string contactNumber1 = tbContact1Value.Text;
            string contactNumber2 = tbContact2Value.Text;
            string emailAddress = tbEmailAddressValue.Text;
            DateTime birthDate = Convert.ToDateTime(null);
            string address = tbAddressLine1Value.Text;
            string postalCode = tbPostalCodeValue.Text;
            string highestEdu = ddlHighestEducationValue.SelectedValue;
            string highestEduRemarks = tbHighestEducationRemarkValue.Text;
            string sponsorship = ddlSponsorship.SelectedValue;

            string errorMsg = "";
            int failedCount = 0;
            //check names
            //Kareen: Remove the check of names
            //var regexName = new Regex(@"^[A-Za-z ]+$");
            //if (fullName.Length == 0 || !regexName.IsMatch(fullName))
            //{
            //    failedCount++;
            //    errorMsg += "Invalid name" + "<br>";
            //}

            if (tbBirthDateValue.Text.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's birthd date. <br>";

            }
            else
            {
                birthDate = DateTime.Parse(tbBirthDateValue.Text);
            }

            if (fullName.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's Name." + "<br>";
            }

            if (idNumber.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's Identification." + "<br>";
            }
            else
            {
                if (ddlIdentificationType.SelectedValue == ((int)IDType.NRIC).ToString() || ddlIdentificationType.SelectedValue == ((int)IDType.FIN).ToString())
                {
                    if (!idNumber.Trim().Equals("") && idNumber.Length == 9)
                    {
                        char fLetter = ' ';
                        char lLetter = ' ';
                        string seven_digits_nric = "";

                        if (idNumber[0].ToString() == "S" || idNumber[0].ToString() == "T" || idNumber[0].ToString() == "G" || idNumber[0].ToString() == "F")
                        {
                            fLetter = idNumber[0];
                        }

                        if (idNumber[0].ToString().All(Char.IsLetter))
                        {
                            lLetter = idNumber[8];
                        }

                        if (idNumber.Substring(1, 7).All(Char.IsDigit))
                        {
                            seven_digits_nric = idNumber.Substring(1, 7);
                        }

                        if (fLetter != ' ' && lLetter != ' ' && seven_digits_nric != "")
                        {
                            char last_char_check = generateCheckCode(fLetter, seven_digits_nric);

                            if (lLetter != last_char_check)
                            {
                                failedCount++;
                                errorMsg += "Invalid NRIC." + "<br>";
                            }
                        }
                        else
                        {
                            failedCount++;
                            errorMsg += "Invalid NRIC." + "<br>";
                        }

                    }
                }
            }
            //else if (idNumber.Length != 9)
            //{
            //    failedCount++;
            //    errorMsg += "Invalid NRIC." + "<br>";
            //}
            //check nric
            //else 

            //check nationality
            if (nationality.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's Nationality." + "<br>";
            }

            //check contact 1 - must have
            //if (contactNumber1.Length == 0 || !contactNumber1.All(Char.IsDigit))
            //{
            //    failedCount++;
            //    errorMsg += "Invalid contact number" + "<br>";
            //}

            if (contactNumber1.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's Contact Number 1." + "<br>";
            }
            else
            {
                if (!contactNumber1.All(Char.IsDigit))
                {
                    failedCount++;
                    errorMsg += "Invalid Contact Number 1." + "<br>";
                }
            }

            //check contact 2 if field not empty
            if (!contactNumber2.Trim().Equals(""))
            {
                if (!contactNumber2.All(Char.IsDigit))
                {
                    failedCount++;
                    errorMsg += "Invalid Contact Number 2." + "<br>";
                }
            }

            //remove the mandatory field for email 

            if (!tbEmailAddressValue.Text.Trim().Equals(""))
            {
                Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                if (!regexEmail.IsMatch(tbEmailAddressValue.Text))
                {
                    failedCount++;
                    errorMsg += "Invalid email address" + "<br>";
                }
            }


            //if (tbEmailAddressValue.Text.Trim().Equals(""))
            //{
            //    failedCount++;
            //    errorMsg += "Please enter applicant's email." + "<br>";
            //}
            //else
            //{
            //    Regex regexEmail = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            //    if (!regexEmail.IsMatch(tbEmailAddressValue.Text))
            //    {
            //        failedCount++;
            //        errorMsg += "Invalid email address" + "<br>";
            //    }
            //}

            if (race.Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's race." + "<br>";
            }

            //check address
            if (address.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's address" + "<br>";
            }

            //check postalCode
            if (postalCode.Trim().Equals(""))
            {
                failedCount++;
                errorMsg += "Please enter applicant's postal code." + "<br>";
            }

            //check education
            if (highestEdu.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's highest education" + "<br>";
            }

            string slangScore = "";
            if (ddlEngPro.SelectedValue.Equals(""))
            {

                failedCount++;
                errorMsg += "Please select applicant's spoken English proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                slangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";
            }

            if (ddlChnPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken Chinese proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                slangScore += General_Constance.CHN + ":" + ddlChnPro.SelectedValue + ";";
            }

            if (!ddlOtherLanguage.SelectedValue.Equals("") && ddlOtherLangPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken " + ddlOtherLanguage.SelectedItem.Text + " proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                slangScore += ddlOtherLanguage.SelectedValue + ":" + ddlOtherLangPro.SelectedValue + ";";
            }


            string wlangScore = "";
            if (ddlWEngPro.SelectedValue.Equals(""))
            {

                failedCount++;
                errorMsg += "Please select applicant's written English proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                wlangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";
            }

            if (ddlWChiPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's written Chinese proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                wlangScore += General_Constance.CHN + ":" + ddlWChiPro.SelectedValue + ";";
            }

            if (!ddlWOtherLanguage.SelectedValue.Equals("") && ddlWOtherLangPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken " + ddlOtherLanguage.SelectedItem.Text + " proficiency. Select 'None' if not applicable." + "<br>";
            }
            else
            {
                wlangScore += ddlWOtherLanguage.SelectedValue + ":" + ddlWOtherLangPro.SelectedValue + ";";
            }


            //spoken language
            // string slangScore = "";
            //foreach (RepeaterItem item in rptSpokenLanguage.Items)
            //{
            //    Label lb = (Label)item.FindControl("lbSLanguages");
            //    HiddenField hdf = (HiddenField)item.FindControl("hdfSLangCodeValue");
            //    DropDownList ddl = (DropDownList)item.FindControl("ddlSLanguagesPro");
            //    if (ddl.SelectedValue != "Select")
            //    {
            //        slangScore += hdf.Value + ":" + ddl.SelectedValue + ";";
            //    }
            //    else
            //    {
            //        failedCount++;
            //        errorMsg += "For spoken language " + lb.Text + ", please select none if not applicable" + "<br>";
            //    }
            //}

            //Wrttien language
            //string wlangScore = "";
            //foreach (RepeaterItem item in rptWrittenLanguage.Items)
            //{
            //    Label lb = (Label)item.FindControl("lbWLanguages");
            //    HiddenField hdf = (HiddenField)item.FindControl("hdfWLangCodeValue");
            //    DropDownList ddl = (DropDownList)item.FindControl("ddlWLanguagesPro");
            //    if (ddl.SelectedValue != "Select")
            //    {
            //        wlangScore += hdf.Value + ":" + ddl.SelectedValue + ";";
            //    }
            //    else
            //    {
            //        failedCount++;
            //        errorMsg += "For written language " + lb.Text + ", please select none if not applicable" + "<br>";
            //    }

            //}

            //channel
            string channelChecked = "";
            foreach (ListItem i in cblGetToKnowChannel.Items)
            {

                if (i.Selected)
                {
                    channelChecked += i.Value + ", ";
                }
            }

            //if (channelChecked.Equals(""))
            //{
            //    failedCount++;
            //    errorMsg += "Please select at least one channel" + "<br>";
            //}

            //if (ddlSponsorship.SelectedValue == Sponsorship.COMP.ToString())
            //{
            //    if (tbSponsorshipCompany.Text == "")
            //    {
            //        failedCount++;
            //        errorMsg += "Applicant has selected the Company Sponsorship, please ensure that applicant's employment record is entered before enrol." + "<br>";
            //    }
            //}

            if (failedCount == 0)
            {
                Applicant_Management am = new Applicant_Management();
                //bool success = am.updateApplicantDetails(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2,
                //    emailAddress, race, birthDate, address, postalCode, highestEdu, highestEduRemarks, slangScore, wlangScore, channelChecked, sponsorship, LoginID);

                if (true)
                {
                    panelUpdateApplicantDetails.Visible = false;
                    btnCancelApplicant.Visible = false;
                    btnEditApplicant.Visible = true;
                    panelParticular.Enabled = false;
                    panelSuccess.Visible = true;
                    lblErrorMsg.Text = "";
                    loadApplicationDetails(applicantId);
                }
            }
            else
            {
                panelError.Visible = true;
                lblErrorMsg.Text = errorMsg;
            }
        }

        //Update empolyment details
        //protected void btnUpdateEmploymentDetails_Click(object sender, EventArgs e)
        //{
        //    int employmentId = 0;
        //    string errorMsg = "";
        //    int failedCount = 0;
        //    string currentEmployment = "";
        //    string empStatus = "";

        //    string companyname = "";
        //    string position = "";
        //    string salary = "";
        //    string tbstartDate = tbStartDateValue.Text.Trim();
        //    string tbendDate = tbEndDateValue.Text.Trim();
        //    DateTime startDate = DateTime.Now.Date;
        //    DateTime endDate = DateTime.Now.Date;

        //    string occupationcode = "";
        //    string occupationremarks = "";

        //    occupationremarks = tbOccupationRemarks.Text;

        //    DataTable dtEmpDetails = ViewState["dtEmploymentDetails"] as DataTable;

        //    string applicantId = lbApplicantId.Text;
        //    //if (hdfEmploymentHistoryId.Value != null || hdfEmploymentHistoryId.Value != "")
        //    //{
        //    //    employmentId = int.Parse(hdfEmploymentHistoryId.Value);
        //    //}

        //    try
        //    {
        //        employmentId = int.Parse(hdfEmploymentHistoryId.Value);
        //    }
        //    catch
        //    {
        //        employmentId = 0;
        //    }



        //    if (tbstartDate.Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please enter a start date <br>";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            startDate = DateTime.Parse(tbstartDate).Date;
        //        }
        //        catch
        //        {
        //            failedCount++;
        //            errorMsg += "Please enter a valid start date <br>";
        //        }
        //    }

        //    //if (currentEmployment.Equals(General_Constance.STATUS_NO))
        //    //{
        //    //    if (tbendDate.Equals(""))
        //    //    {
        //    //        failedCount++;
        //    //        errorMsg += "Please enter an end date";
        //    //    }
        //    //    else
        //    //    {
        //    //        try
        //    //        {
        //    //            endDate = DateTime.Parse(tbendDate).Date;
        //    //        }
        //    //        catch
        //    //        {
        //    //            failedCount++;
        //    //            errorMsg += "Please enter a valid end date <br>";
        //    //        }
        //    //    }
        //    //}

        //    //Kareen: Check if it is current employment 

        //    bool current_emp_exist = false;
        //    DataRow[] foundRows = null;

        //    if (dtEmpDetails.Rows.Count > 0)
        //    {
        //        string expression = "currentEmployment = '" + General_Constance.STATUS_YES + "'";

        //        // Use the Select method to find all rows matching the filter.
        //        foundRows = dtEmpDetails.Select(expression);

        //        if (foundRows.Length > 0)
        //        {
        //            current_emp_exist = true;
        //        }
        //    }



        //    if (cbSetCurrentEmployment.Checked)
        //    {
        //        currentEmployment = General_Constance.STATUS_YES;

        //        if (current_emp_exist == true)
        //        {
        //            failedCount++;
        //            errorMsg += "Please enter only one current empoloyment record. <br>";
        //        }

        //    }
        //    else
        //    {
        //        currentEmployment = General_Constance.STATUS_NO;
        //        if (current_emp_exist == false && dtEmpDetails.Rows.Count > 0)
        //        {
        //            failedCount++;
        //            errorMsg += "Please enter a current employment record. <br>";
        //        }
        //        else if (current_emp_exist == true)
        //        {
        //            if (tbendDate.Equals(""))
        //            {
        //                failedCount++;
        //                errorMsg += "Please enter an end date";
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    endDate = DateTime.Parse(tbendDate).Date;
        //                }
        //                catch
        //                {
        //                    failedCount++;
        //                    errorMsg += "Please enter a valid end date <br>";
        //                }
        //            }

        //            DateTime currEmpStartDate = DateTime.Parse(foundRows[0]["employmentStartDate"].ToString());
        //            //Within the current employment start/end date
        //            if (currEmpStartDate <= startDate && currEmpStartDate >= endDate)
        //            {
        //                failedCount++;
        //                errorMsg += "Non-current Employment start / end date should not be within the Current Employment Period.<br>";
        //            }
        //            else
        //            {

        //                if (startDate > endDate)
        //                {
        //                    failedCount++;
        //                    errorMsg += "Employment's Start Date should not be later than Employment's End Date<br>";
        //                }

        //                if (endDate > currEmpStartDate)
        //                {
        //                    failedCount++;
        //                    errorMsg += "Employment's End Date should not be later than Current Employment's Start Date<br>";
        //                }

        //            }
        //        }

        //    }

        //    //Kareen: End Check


        //    if (tbCompanyNameValue.Text.Trim().Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please enter applicant's company name" + "<br>";
        //    }
        //    else
        //    {
        //        companyname = tbCompanyNameValue.Text;
        //    }

        //    if (ddlEmploymentStatus.SelectedValue.Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please select applicant's employment status <br>";
        //    }
        //    else
        //    {
        //        empStatus = ddlEmploymentStatus.SelectedValue;
        //    }

        //    if (ddlOccupationCode.SelectedValue.Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please enter applicant's occupation code" + "<br>";
        //    }
        //    else
        //    {
        //        occupationcode = ddlOccupationCode.SelectedValue;
        //    }
        //    if (tbPositionValue.Text.Trim().Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please enter applicant's position" + "<br>";
        //    }
        //    else
        //    {
        //        position = tbPositionValue.Text;
        //    }

        //    if (tbSalaryValue.Text.Trim().Equals(""))
        //    {
        //        failedCount++;
        //        errorMsg += "Please enter applicant's salary" + "<br>";
        //    }
        //    else
        //    {
        //        salary = tbSalaryValue.Text;
        //        try
        //        {
        //            decimal dSalary = decimal.Parse(salary);
        //        }
        //        catch
        //        {
        //            failedCount++;
        //            errorMsg += "Please enter only numerics for salary <br>";
        //        }
        //    }


        //    //Kareen: Commented
        //    //try
        //    //{
        //    //    startDate = DateTime.Parse(tbStartDateValue.Text).Date;


        //    //}
        //    //catch
        //    //{
        //    //    startDate = DateTime.Now.Date;
        //    //    //endDate = DateTime.Now.Date;
        //    //    failedCount++;
        //    //    errorMsg += "Please enter a valid date for start date" + "<br>";
        //    //}

        //    //if (tbEndDateValue.Text != "")
        //    //{
        //    //    try
        //    //    {
        //    //        endDate = DateTime.Parse(tbEndDateValue.Text).Date;
        //    //        if (endDate.Date > DateTime.Today.Date)
        //    //        {
        //    //            failedCount++;
        //    //            errorMsg += "End date cannot be greater than today" + "<br>";
        //    //        }
        //    //    }
        //    //    catch
        //    //    {
        //    //        endDate = DateTime.Now.Date;
        //    //        failedCount++;
        //    //        errorMsg += "Please enter a valid date for end date" + "<br>";
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    currentEmployment = General_Constance.STATUS_YES;

        //    //}

        //    //Kareen: End Commented





        //    if (failedCount == 0)
        //    {
        //        Applicant_Management am = new Applicant_Management();

        //        //bool success = am.updateEmploymentDetails(applicantId, employmentId, companyname, position, startDate, endDate, salary, currentEmployment, empStatus, LoginID, occupationcode);

        //        //if (success)
        //        //{
        //        //    loadApplicationDetails(applicantId);
        //        //    panelSuccessModal.Visible = true;
        //        //    lblErrorMsgModal.Text = "";
        //        //    errorMsg = "";
        //        //    loadApplicationDetails(applicantId);
        //        //    Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //        //}

        //    }
        //    else
        //    {
        //        panelErrorModal.Visible = true;
        //        lblErrorMsgModal.Text = errorMsg;
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeModel", "closeModel()", true);

        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModel", "openModel()", true);
        //    }

        //}

        protected void btnUpdateCourseDetails_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            //string selectedCourseCode = tbCourseCodeValue.Text;
            //string selectedProjectCode = ddlProjectCodeValue.SelectedValue;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue;//ddlProjectCodeValue.SelectedValue;

            Applicant_Management am = new Applicant_Management();
            am.updateApplicantCourseProjectCode(applicantId, programmeBatchId);
            Page.Response.Redirect(Page.Request.Url.ToString(), true);
        }

        protected void btnSaveRemarks_Click(object sender, EventArgs e)
        {
            string remarks = tbApplicantRemarksValue.Text;
            string applicantId = lbApplicantId.Text;

            Applicant_Management am = new Applicant_Management();
            int userId = LoginID;
            am.updateApplicantRemarks(applicantId, remarks, userId);
            Page.Response.Redirect(Page.Request.Url.ToString(), true);

        }

        //Update Interview details
        protected void btnUpdateInterviewDetails_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            DateTime interviewDate = Convert.ToDateTime("1/1/1753");
            string selectedInterviewStatus = lbInterviewStatusValue.Text;
            string interviewRemarks = tbInterviewRemarksValue.Text;
            string shortlistStatus = General_Constance.STATUS_NO;
            string errorMsg = "";

            string interviewerId = ddlInterviewer.SelectedValue;
            if (!selectedInterviewStatus.Equals(InterviewStatus.NREQ.ToString()))
            {
                if (interviewerId.Equals("Select an interviewer"))
                {
                    errorMsg += "Please select an interviewer" + "<br>";
                }

                if (!string.IsNullOrEmpty(tbInterviewDateValue.Text))
                {
                    interviewDate = DateTime.Parse(tbInterviewDateValue.Text).Date;
                }
                else
                {
                    errorMsg += "Please enter a valid date" + "<br>";
                }

                if (selectedInterviewStatus.Equals(InterviewStatus.PASSED.ToString()) || selectedInterviewStatus.Equals(InterviewStatus.FAILED.ToString()))
                {
                    if (cbShortlisted.Checked == true)
                    {
                        shortlistStatus = General_Constance.STATUS_YES;
                    }
                }
            }
            else
            {
                interviewerId = "0";
            }
            int userId = LoginID;

            if (errorMsg == "")
            {
                Applicant_Management am = new Applicant_Management();
                //bool success = am.updateApplicantInterviewDetails(applicantId, interviewerId, selectedInterviewStatus, shortlistStatus, interviewDate, interviewRemarks, userId);

                //if (success)
                //{
                //    btnCancelInterviewDetails.Visible = false;

                //    btnEditInterviewDetails.Visible = true;

                //    panelInterviewDetails.Enabled = false;

                //    panelUpdateInterviewDetails.Visible = false;

                //    panelSuccess.Visible = true;
                //    lblSuccess.Visible = true;
                //    lblErrorMsg.Text = "";
                //    loadApplicationDetails(applicantId);
                //}

            }
            else
            {
                panelError.Visible = true;
                lblErrorMsg.Text = errorMsg;
                lblErrorMsg.Visible = true;
            }
        }


        //Edit exempted modules
        protected void btnExemptedModule_ServerClick(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue;//ddlProjectCodeValue.SelectedValue;

            Response.Redirect(applicant_module_exemption.PAGE_NAME + "?a=" + applicantId + "&pb=" + programmeBatchId);
        }

        protected void btnPayCourseFee_Click(object sender, EventArgs e)
        {

            //DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
            //DataTable dtEmpDetails = ViewState["dtEmploymentDetails"] as DataTable;

            //if (dtApplicantDetails.Rows[0]["selfSponsored"].Equals("COMP") && dtEmpDetails.Rows.Count == 0)
            //{
            //    string errorMsg = "Please enter employment details for Company Sponsored Applicants before making payment.";
            //    panelError.Visible = true;
            //    lblErrorMsg.Text = errorMsg;

            //}
            //else
            //{
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue;//ddlProjectCodeValue.SelectedValue;

            //to retrieve the voided registration receipt number by applicantId
            Finance_Management a = new Finance_Management();
            string PreviousVoidedReceiptNum = a.getReptNumberCSE(applicantId);
            string pageDirective = "linked from applicant details aspx";

            string paymentId = hdf_selctedPaymentIdForCourseFee.Value;
            //DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;

            //Kareen: Ask Victoria
            //string applicantRemarks = "Course Payment Voided";
            //if (applicantRemarks == dtApplicantDetails.Rows[0]["applicantRemarks"].ToString())
            //{
            //    //Response.Redirect(General_Constance.PAGE_CREATE_NEW_PAYMENT_FOR_COURSE_FEE + "?a=" + applicantId + "&pb=" + programmeBatchId + "&r=" + PreviousVoidedReceiptNum + "&pageLinkedFrom=" + pageDirective);
            //    Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
            //        + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}
            //else
            //{
            //Response.Redirect(General_Constance.PAGE_APPLICANT_COURSE_PAYMENT + "?a=" + applicantId + "&pb=" + programmeBatchId + "&rpid=" + paymentId);
            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
                + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}

            //Response.Redirect(General_Constance.PAGE_APPLICANT_COURSE_PAYMENT + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode);
            //}
        }

        protected void btnPayRegistrationFee_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;

            //DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
            ////retrieve latest registration fee.the fee will be 51745.00 this value is special as it will determine which aspx to link to.
            //string regisFee = dtApplicantDetails.Rows[0]["registrationFee"].ToString();

            //51745 can be changed to other value in the data layer
            //if (regisFee == "51745.00")
            //{
            //    //to retrieve the voided registration receipt number by applicantId
            //    Finance_Management a = new Finance_Management();
            //    string PreviousVoidedReceiptNum = a.getReptNumberREG(applicantId);
            //    string pageDirective = "linked from applicant details aspx";
            //    if (PreviousVoidedReceiptNum != null)
            //    {
            //        //Response.Redirect(General_Constance.PAGE_CREATE_NEW_PAYMENT_FOR_REGISTRATION_FEE + "?a=" + applicantId + "&c=" + courseCode + "&pb=" + programmeBatchId + "&r=" + PreviousVoidedReceiptNum + "&pageLinkedFrom=" + pageDirective);
            //        Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.REG.ToString()
            //            + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //    }
            //}
            //else
            //{
                //Response.Redirect(General_Constance.PAGE_APPLICANT_REGISTRATION_PAYMENT + "?a=" + applicantId + "&c=" + courseCode + "&pb=" + programmeBatchId);
                Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.REG.ToString()
                    + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}

            //Response.Redirect(General_Constance.PAGE_APPLICANT_REGISTRATION_PAYMENT + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode);
        }

        protected void btnViewRegistationFeeReceipt_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            //string courseCode = tbCourseCodeValue.Text;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;

            string registrationPaymentId = hdfRegistrationPaymentId.Value;

            //Response.Redirect(General_Constance.PAGE_APPLICANT_REGISTRATION_RECEIPT + "?a=" + applicantId + "&pb=" + programmeBatchId + "&rpid=" + registrationPaymentId);
            Response.Redirect(applicant_programme_receipt.PAGE_NAME + "?" + applicant_programme_receipt.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_receipt.TYPE_QUERY + "=" + PaymentType.REG.ToString());
        }

        protected void lkbtnEnrollApplicant_Click(object sender, EventArgs e)
        {
            string errorMsg = "";
            int failedCount = 0;

            panelError.Visible = false;
            lblErrorMsg.Text = "";

            if (tbFullNameValue.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's name. <br>";
                failedCount++;
            }

            if (tbIdentificationValue.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's identification number. <br>";
                failedCount++;
            }

            if (ddlNationalityValue.SelectedValue.Equals(""))
            {
                errorMsg += "Please select applicant's nationality. <br>";
                failedCount++;
            }

            if (ddlGenderValue.SelectedValue.Equals(""))
            {
                errorMsg += "Please select applicant's gender. <br>";
                failedCount++;
            }

            if (ddlRaceValue.SelectedValue.Equals(""))
            {
                errorMsg += "Please select applicant's race. <br>";
                failedCount++;
            }

            if (tbContact1Value.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's contact number. <br>";
                failedCount++;
            }

            //if (tbEmailAddressValue.Text.Equals(""))
            //{
            //    errorMsg += "Please enter applicant's email address. <br>";
            //    failedCount++;
            //}

            if (tbBirthDateValue.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's DOB. <br>";
                failedCount++;
            }

            if (tbAddressLine1Value.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's address. <br>";
                failedCount++;
            }

            if (tbPostalCodeValue.Text.Equals(""))
            {
                errorMsg += "Please enter applicant's postal code. <br>";
                failedCount++;
            }

            if (ddlHighestEducationValue.SelectedValue.Equals(""))
            {
                errorMsg += "Please select applicant's highest education. <br>";
                failedCount++;
            }

            if (ddlEngPro.SelectedValue.Equals(""))
            {

                failedCount++;
                errorMsg += "Please select applicant's spoken English proficiency. Select 'None' if not applicable." + "<br>";
            }
            if (ddlChnPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken Chinese proficiency. Select 'None' if not applicable." + "<br>";
            }


            if (!ddlOtherLanguage.SelectedValue.Equals("") && ddlOtherLangPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken " + ddlOtherLanguage.SelectedItem.Text + " proficiency. Select 'None' if not applicable." + "<br>";
            }

            if (ddlWEngPro.SelectedValue.Equals(""))
            {

                failedCount++;
                errorMsg += "Please select applicant's written English proficiency. Select 'None' if not applicable." + "<br>";
            }

            if (ddlWChiPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's written Chinese proficiency. Select 'None' if not applicable." + "<br>";
            }

            if (!ddlWOtherLanguage.SelectedValue.Equals("") && ddlWOtherLangPro.SelectedValue.Equals(""))
            {
                failedCount++;
                errorMsg += "Please select applicant's spoken " + ddlOtherLanguage.SelectedItem.Text + " proficiency. Select 'None' if not applicable." + "<br>";
            }

            //foreach (RepeaterItem item in rptSpokenLanguage.Items)
            //{
            //    Label lb = (Label)item.FindControl("lbSLanguages");
            //    HiddenField hdf = (HiddenField)item.FindControl("hdfSLangCodeValue");
            //    DropDownList ddl = (DropDownList)item.FindControl("ddlSLanguagesPro");
            //    if (ddl.SelectedValue != "Select")
            //    {
            //        slangScore += hdf.Value + ":" + ddl.SelectedValue + ";";
            //    }
            //    else
            //    {
            //        failedCount++;
            //        errorMsg += "For spoken language " + lb.Text + ", please select none if not applicable" + "<br>";
            //    }
            //}

            //Wrttien language
            // string wlangScore = "";
            //foreach (RepeaterItem item in rptWrittenLanguage.Items)
            //{
            //    Label lb = (Label)item.FindControl("lbWLanguages");
            //    HiddenField hdf = (HiddenField)item.FindControl("hdfWLangCodeValue");
            //    DropDownList ddl = (DropDownList)item.FindControl("ddlWLanguagesPro");
            //    if (ddl.SelectedValue != "Select")
            //    {
            //        wlangScore += hdf.Value + ":" + ddl.SelectedValue + ";";
            //    }
            //    else
            //    {
            //        failedCount++;
            //        errorMsg += "For written language " + lb.Text + ", please select none if not applicable" + "<br>";
            //    }

            //}

            //channel
            string channelChecked = "";
            foreach (ListItem i in cblGetToKnowChannel.Items)
            {

                if (i.Selected)
                {
                    channelChecked += i.Value + ", ";
                }
            }

            //if (channelChecked.Equals(""))
            //{
            //    failedCount++;
            //    errorMsg += "Please select at least one channel" + "<br>";
            //}

            if (ddlSponsorship.SelectedValue == Sponsorship.COMP.ToString())
            {
                DataTable dtEmp = ViewState["dtEmploymentDetails"] as DataTable;
                if (dtEmp.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtEmp.Rows)
                    {
                        if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                        {
                            tbSponsorshipCompany.Text = dr["companyName"].ToString();
                            break;
                        }
                    }
                }

                if (tbSponsorshipCompany.Text == "")
                {
                    failedCount++;
                    errorMsg += "Your have selected the sponsorship under your company, please ensure that you have enter your employment record before enrol." + "<br>";
                }
            }

            Applicant_Management am = new Applicant_Management();

            DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
            string applicantId = dtApplicantDetails.Rows[0]["applicantId"].ToString();



            if (failedCount == 0)
            {

                if (!am.checkIfApplicantDetailsIsUpdated(applicantId))
                {
                    applicantId = lbApplicantId.Text;

                    string fullName = tbFullNameValue.Text;
                    string idNumber = tbIdentificationValue.Text;
                    string idType = ddlIdentificationType.SelectedValue;
                    string nationality = ddlNationalityValue.SelectedValue;
                    string gender = ddlGenderValue.SelectedValue;
                    string race = ddlRaceValue.SelectedValue;
                    string contactNumber1 = tbContact1Value.Text;
                    string contactNumber2 = tbContact2Value.Text;
                    string emailAddress = tbEmailAddressValue.Text;
                    DateTime birthDate = Convert.ToDateTime(null);
                    string address = tbAddressLine1Value.Text;
                    string postalCode = tbPostalCodeValue.Text;
                    string highestEdu = ddlHighestEducationValue.SelectedValue;
                    string highestEduRemarks = tbHighestEducationRemarkValue.Text;
                    string sponsorship = ddlSponsorship.SelectedValue;

                    birthDate = DateTime.Parse(tbBirthDateValue.Text);


                    string slangScore = "";

                    slangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";



                    slangScore += General_Constance.CHN + ":" + ddlChnPro.SelectedValue + ";";

                    if (!ddlOtherLanguage.SelectedValue.Equals("") && !ddlOtherLangPro.SelectedValue.Equals(""))
                    {
                        slangScore += ddlOtherLanguage.SelectedValue + ":" + ddlOtherLangPro.SelectedValue + ";";
                    }




                    string wlangScore = "";
                    wlangScore += General_Constance.ENG + ":" + ddlEngPro.SelectedValue + ";";
                    wlangScore += General_Constance.CHN + ":" + ddlWChiPro.SelectedValue + ";";
                    if (!ddlWOtherLanguage.SelectedValue.Equals("") && !ddlWOtherLangPro.SelectedValue.Equals(""))
                    {
                        wlangScore += ddlWOtherLanguage.SelectedValue + ":" + ddlWOtherLangPro.SelectedValue + ";";
                    }

                    //bool success = am.updateApplicantDetails(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2,
                    //    emailAddress, race, birthDate, address, postalCode, highestEdu, highestEduRemarks, slangScore, wlangScore, channelChecked, sponsorship, LoginID);

                }
                //DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
                //string applicantId = dtApplicantDetails.Rows[0]["applicantId"].ToString();

                //DataTable dt = ViewState["dtProgrammeBatchInfo"] as DataTable;

                //DataTable dtProgrammeBatchInfo = new DataTable();
                //dtProgrammeBatchInfo.Columns.Add("programmeBatchId");
                //dtProgrammeBatchInfo.Columns.Add("programmeId");

                //string programmeBatchId = ddlProjectCodeValue.SelectedValue;

                //DataRow[] result = dt.Select("programmeBatchId = '" + programmeBatchId + "'");

                //dtProgrammeBatchInfo.Rows.Add(result[0]["programmeBatchId"].ToString(), result[0]["programmeId"].ToString());

                Trainee_Management tm = new Trainee_Management();
                Tuple<int, string, string> tupleResult = tm.enrollApplicant(applicantId, LoginID);

                if (tupleResult.Item1 == -1)
                {
                    panelError.Visible = true;
                    lblErrorMsg.Text = tupleResult.Item3;
                }
                else
                {
                    Response.Redirect(trainee_details.PAGE_NAME + "?" + trainee_details.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(tupleResult.Item2) + "&" + trainee_details.MSG_QUERY + "=" + tupleResult.Item1);
                }
            }
            else
            {
                panelError.Visible = true;
                lblErrorMsg.Text = errorMsg;
            }


        }

        //btnCourseFee. this btn redirects user to payment page (View 2,for Short programme)
        protected void btnCourseFeeOnly_Click(object sender, EventArgs e)
        {

            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            // string projectCode = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;

            //to retrieve the voided receipt number by applicantId
            Finance_Management a = new Finance_Management();
            string PreviousVoidedReceiptNum = a.getReptNumberCSE(applicantId);
            string pageDirective = "linked from applicant details aspx";


            // DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
            //string applicantRemarks = "ShortCourse Payment Voided";
            //if (applicantRemarks == dtApplicantDetails.Rows[0]["applicantRemarks"].ToString())
            //{
            //    //Response.Redirect(General_Constance.PAGE_CREATE_NEW_PAYMENT_FOR_SHORTCOURSE_FEE + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode + "&r=" + PreviousVoidedReceiptNum + "&pageLinkedFrom=" + pageDirective);
            //    Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
            //        + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}
            //else
            //{
            //Response.Redirect(General_Constance.PAGE_APPLICANT_SHORTCOURSE_PAYMENT + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode);
            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
                + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}
        }

        //view payment recipt (view 2)
        protected void btnCourseFeeReceiptOnly_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            //string projectCode = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;
            string registrationPaymentId = HiddenField1.Value.ToString();
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;

            //Response.Redirect(General_Constance.PAGE_APPLICANT_SHORTCOURSE_RECEIPT + "?a=" + applicantId + "&pb=" + programmeBatchId + "&rpid=" + registrationPaymentId);
            Response.Redirect(applicant_programme_receipt.PAGE_NAME + "?" + applicant_programme_receipt.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_receipt.TYPE_QUERY + "=" + PaymentType.PROG.ToString());
        }


        //multiView > view 3. (for combined payments)
        protected void btnRnCPayment_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            //string projectCode = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;
            //to retrieve the voided receipt number by applicantId
            Finance_Management a = new Finance_Management();
            string PreviousVoidedReceiptNum = a.getReptNumberFULLCSEPayment(applicantId);
            //string pageDirective = "linked from applicant details aspx";
            //DataTable dtApplicantDetails = ViewState["dtApplicantDetails"] as DataTable;
            //string applicantRemarks = "Full Payment Voided";
            //if (applicantRemarks == dtApplicantDetails.Rows[0]["applicantRemarks"].ToString())
            //{

            //    //Response.Redirect(General_Constance.PAGE_CREATE_NEW_PAYMENT_FOR_COMBINED_FEES + "?a=" + applicantId + "&c=" + courseCode + "&pb=" + programmeBatchId + "&r=" + PreviousVoidedReceiptNum + "&pageLinkedFrom=" + pageDirective);
            //    Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&"
            //        + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.BOTH.ToString()
            //        + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}
            //else
            //{

            //Response.Redirect(General_Constance.PAGE_APPLICANT_REGISTRATION_AND_COURSE_PAYMENT + "?a=" + applicantId + "&c=" + courseCode + "&pb=" + programmeBatchId);
            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&"
                + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.BOTH.ToString()
                + "&" + applicant_programme_payment.PREV_QUERY + "=A");
            //}
        }

        //for course fee
        protected void btnCourseFeeReceipt_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            //string projectCode = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;
            string registrationPaymentId = hdf_selctedPaymentIdForCourseFee.Value;
            string programmeBatchId = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;

            //Response.Redirect(General_Constance.PAGE_APPLICANT_COURSE_RECEIPT + "?a=" + applicantId + "&pb=" + programmeBatchId + "&rpid=" + registrationPaymentId);
            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
                    + "&" + applicant_programme_payment.PREV_QUERY + "=A");
        }


        //link to receipt page, for full payment
        protected void btnRnCReceipt_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            // string projectCode = ddlCourseAppliedValue.SelectedValue; //ddlProjectCodeValue.SelectedValue;
            string registrationPaymentId = hdfRegistrationPaymentId.Value;//paymentId
            //Response.Redirect(General_Constance.PAGE_APPLICANT_REGISTRATION_AND_COURSE_RECIEPT + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode + "&rpid=" + registrationPaymentId);
            Response.Redirect(applicant_programme_receipt.PAGE_NAME + "?" + applicant_programme_receipt.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_receipt.TYPE_QUERY + "=" + PaymentType.BOTH.ToString());
        }


        //link back to applicant page
        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(applicant.PAGE_NAME);

        }

        protected void cb_combinepayment_CheckedChanged(object sender, EventArgs e)
        {

            if (cb_combinepayment.Checked)
            {
                MultiView1.ActiveViewIndex = 2;
                cb_combinepayment.Visible = true;
                //cb_seperatePayment.Checked = false;
                //cb_seperatePayment.Visible = true;
            }
            else
            {
                MultiView1.ActiveViewIndex = 0;
                cb_combinepayment.Visible = true;
            }
        }


        protected void btn_print_Click(object sender, EventArgs e)
        {
            string applicantId = Request.QueryString[APPLICANT_QUERY];

            Applicant_Management am = new Applicant_Management();
            Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> applicantTuple = am.getApplicationDetailsByApplicantId(applicantId);

            //ViewState["dtApplicantDetails"] = dtApplicantDetails = applicantTuple.Item1;
            //ViewState["dtEmploymentDetails"] = dtEmploymentDetails = applicantTuple.Item2;


            DataTable DataTable_ApplicantDetails = new DataTable();
            DataTable DataTable_EmploymentDetails = new DataTable();

            DataTable Nationality_N_Race = new DataTable();
            Nationality_N_Race.Columns.Add("race", typeof(String));
            Nationality_N_Race.Columns.Add("nationality", typeof(String));
            Nationality_N_Race.Columns.Add("gender", typeof(String));

            DataRow row = Nationality_N_Race.NewRow();

            row["race"] = ddlNationalityValue.SelectedItem.Text;
            row["nationality"] = ddlRaceValue.SelectedItem.Text;
            row["gender"] = ddlGenderValue.SelectedItem.Text;

            Nationality_N_Race.Rows.Add(row);

            //DataTable_ApplicantDetails = dtApplicantDetails;
            DataTable_ApplicantDetails = applicantTuple.Item1;

            //DataTable_EmploymentDetails = dtEmploymentDetails;
            DataTable_EmploymentDetails = applicantTuple.Item2;

            HttpContext.Current.Session["dtNationality_N_Race"] = Nationality_N_Race;
            HttpContext.Current.Session["dtApplicantDetails"] = DataTable_ApplicantDetails;
            HttpContext.Current.Session["dtEmploymentDetails"] = DataTable_EmploymentDetails;

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('applicant-details-print.aspx'); ", true);
        }

        //protected void btnAddEmployment_Click(object sender, EventArgs e)
        //{
        //    cbSetCurrentEmployment.Enabled = true;
        //    cbSetCurrentEmployment.Checked = false;

        //    DataTable dtEmpDetails = ViewState["dtEmploymentDetails"] as DataTable;
        //    bool current_emp_exist = false;

        //    DataRow[] foundRows = null;

        //    if (dtEmpDetails.Rows.Count > 0)
        //    {
        //        string expression = "currentEmployment = '" + General_Constance.STATUS_YES + "'";

        //        // Use the Select method to find all rows matching the filter.
        //        foundRows = dtEmpDetails.Select(expression);

        //        if (foundRows.Length > 0)
        //        {
        //            current_emp_exist = true;
        //        }
        //    }

        //    if (current_emp_exist == true)
        //    {
        //        cbSetCurrentEmployment.Visible = false;
        //        cbSetCurrentEmployment.Checked = false;

        //    }
        //    else
        //    {
        //        cbSetCurrentEmployment.Enabled = true;
        //        cbSetCurrentEmployment.Checked = false;

        //    }



        //    panelSuccessModal.Visible = false;
        //    panelErrorModal.Visible = false;
        //    lblErrorMsgModal.Text = "";

        //    tbCompanyNameValue.Text = "";
        //    tbPositionValue.Text = "";
        //    tbSalaryValue.Text = "";
        //    tbStartDateValue.Text = "";
        //    tbEndDateValue.Text = "";
        //    hdfEmploymentHistoryId.Value = "";
        //    ddlEmploymentStatus.SelectedValue = "";

        //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModel", "openModel()", true);
        //}

        protected void btnRejectAppcalicant_Click(object sender, EventArgs e)
        {
            int userId = LoginID;
            // DataTable dt = ViewState["dtApplicantDetails"] as DataTable;

            string applicantId = lbApplicantId.Text;//dt.Rows[0]["applicantId"].ToString();
            string remarks = tbApplicantRemarksValue.Text;

            Applicant_Management am = new Applicant_Management();
            bool success = am.updateApplicantStatusReject(applicantId, userId);
            if (success)
            {
                panelSuccess.Visible = true;
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }


        }

        protected void ddlSponsorship_SelectedIndexChanged(object sender, EventArgs e)
        {
            bool sponsorCompany = false;
            if (ddlSponsorship.SelectedValue == Sponsorship.COMP.ToString())
            {
                DataTable dtEmp = ViewState["dtEmploymentDetails"] as DataTable;
                if (dtEmp.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtEmp.Rows)
                    {
                        if (dr["currentEmployment"].ToString() == General_Constance.STATUS_YES)
                        {
                            tbSponsorshipCompany.Text = dr["companyName"].ToString();
                            sponsorCompany = true;
                            break;
                        }
                    }
                }

            }
            else
            {
                tbSponsorshipCompany.Text = "";
            }
        }

        private void loadStep5Fields()
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

        protected void btn_SaveEmployment_Click(object sender, EventArgs e)
        {
            List<EmploymentRecord> empl = new List<EmploymentRecord>();
            if (cbCurrEmpl.Checked)
            {

                string companyName = tbCurrCoName.Text;
                string dept = tbCurrEmplDept.Text;
                string designation = tbCurrEmplDesignation.Text;
                string status = ddlCurrEmplStatus.SelectedValue;
                string occupationType = ddlCurrEmplOccupation.SelectedValue;
                decimal salary = decimal.Parse(tbCurrEmplSalary.Text);
                DateTime dtStart = DateTime.ParseExact(tbCurrEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnd = DateTime.MaxValue;
                int empHistId = 0;

                
                if (!hdfCurrEmpHisId.Value.Equals(""))
                    empHistId = int.Parse(hdfCurrEmpHisId.Value);

                string applicantId = lbApplicantId.Text;
                Applicant_Management am = new Applicant_Management();
                bool success = am.updateEmploymentDetails(applicantId, empHistId, companyName, designation, dtStart, dtEnd, salary, General_Constance.STATUS_YES, status, LoginID, occupationType, dept);
            }
            if (cbPrevEmpl.Checked)
            {

                string companyName = tbPrevCoName.Text;
                string dept = tbPrevEmplDept.Text;
                string designation = tbPrevEmplDesignation.Text;
                string status = ddlPrevEmplStatus.SelectedValue;
                string occupationType = ddlPrevEmplOccupation.SelectedValue;
                decimal salary = decimal.Parse(tbPrevEmplSalary.Text);
                DateTime dtStart = DateTime.ParseExact(tbPrevEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
                DateTime dtEnd = DateTime.ParseExact(tbPrevEmplEndDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

                int empHistId = 0;


                if (!hdfPrevEmpHist.Value.Equals(""))
                    empHistId = int.Parse(hdfPrevEmpHist.Value);

                string applicantId = lbApplicantId.Text;
                Applicant_Management am = new Applicant_Management();
                bool success = am.updateEmploymentDetails(applicantId, empHistId, companyName, designation, dtStart, dtEnd, salary, General_Constance.STATUS_NO, status, LoginID, occupationType, dept);
            }

            loadApplicationDetails(lbApplicantId.Text);
        }

        //protected void cbSetCurrentEmployment_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (cbSetCurrentEmployment.Checked)
        //    {
        //        tbEndDateValue.Enabled = false;
        //    }
        //    else
        //    {
        //        tbEndDateValue.Enabled = true;
        //    }
        //}


        //protected void rptReceipt_ItemCommand(object source, RepeaterCommandEventArgs e)
        //{
        //    string applicantId = lbApplicantId.Text;
        //    string courseCode = tbCourseCodeValue.Text;
        //    string projectCode = ddlProjectCodeValue.SelectedValue;

        //    LinkButton lkbtnReceipt = (LinkButton)e.Item.FindControl("btnReceipt");

        //    string PaymentId = lkbtnReceipt.CommandArgument.ToString();

        //    Response.Redirect(General_Constance.PAGE_APPLICANT_COURSE_RECEIPT + "?a=" + applicantId + "&c=" + courseCode + "&p=" + projectCode + "&rpid=" + PaymentId);

        //}

    }
}