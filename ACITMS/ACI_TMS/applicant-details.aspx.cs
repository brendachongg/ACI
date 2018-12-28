﻿using System;
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
using System.Configuration;

namespace ACI_TMS
{
    public partial class applicant_details : BasePage
    {
        public const string PAGE_NAME = "applicant-details.aspx";
        public const string APPLICANT_QUERY = "a";
     

        Applicant_Management am = new Applicant_Management();
        Batch_Session_Management cbm = new Batch_Session_Management();
        ACI_Staff_User staff = new ACI_Staff_User();
        Trainee_Management tm = new Trainee_Management();

        public applicant_details()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_EDIT, applicant.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string applicantId = Request.QueryString[APPLICANT_QUERY];

                if (!applicantId.Equals(""))
                {
                    loadAllProgrammeBatchInfo();
                    loadSponsorshipType();
                    loadAllChannelInfo();
                    loadAllEducationReferenceInfo();
                    loadEmplFields();
                    loadNationality();
                    loadIdType();
                    loadRace();
                    loadPaymentMode();

                    loadLanguageProficiency(ddlEngSpoken);
                    loadLanguageProficiency(ddlEngWritten);
                    loadLanguageProficiency(ddlChnSpoken);
                    loadLanguageProficiency(ddlChnWritten);
                    loadLanguageProficiency(ddlOtherLanguageProSpoken);
                    loadLanguageProficiency(ddlOtherLanguageProWritten);
                    loadOtherLanguage();

                    loadInterviewStatus();
                    loadInterviewer();



                    loadApplicationDetails(applicantId);
                }
            }
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

                bool success = am.updateEmploymentDetails(applicantId, empHistId, companyName, designation, dtStart, dtEnd, salary, General_Constance.STATUS_YES, status, LoginID, occupationType, dept);

                if (success)
                {
                    panelSuccess.Visible = true;
                    panelError.Visible = false;
                }
                else
                {
                    panelError.Visible = true;
                    panelSuccess.Visible = false;

                }
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
                bool success = am.updateEmploymentDetails(applicantId, empHistId, companyName, designation, dtStart, dtEnd, salary, General_Constance.STATUS_NO, status, LoginID, occupationType, dept);
                if (success)
                {
                    panelSuccess.Visible = true;
                    panelError.Visible = false;
                }
                else
                {
                    panelError.Visible = true;
                    panelSuccess.Visible = false;

                }
            }

            loadApplicationDetails(lbApplicantId.Text);
        }

        private void loadPaymentMode()
        {
            DataTable dt = (new Finance_Management()).getPaymentModes();

            cbPreferredPaymentMode.DataTextField = "codeValueDisplay";
            cbPreferredPaymentMode.DataValueField = "codeValue";

            cbPreferredPaymentMode.DataSource = dt;
            cbPreferredPaymentMode.DataBind();
        }
        private void loadInterviewer()
        {
            try
            {
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

        private void loadInterviewStatus()
        {
            DataTable dtInterviewStatus = am.getInterviewStatus();
            ddlInterviewStatus.DataSource = dtInterviewStatus;
            ddlInterviewStatus.DataTextField = "codeValueDisplay";
            ddlInterviewStatus.DataValueField = "codeValue";
            ddlInterviewStatus.DataBind();

            ddlInterviewStatus.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadOtherLanguage()
        {

            DataTable dtOtherLang = am.getOtherLanguageCodeReference();


            ddlOtherLanguageWritten.DataTextField = "codeValueDisplay";
            ddlOtherLanguageWritten.DataValueField = "codeValue";

            ddlOtherLanguageWritten.DataSource = dtOtherLang;
            ddlOtherLanguageWritten.DataBind();
            ddlOtherLanguageWritten.Items.Insert(0, new ListItem("--Select--", ""));

            ddlOtherLanguageSpoken.DataTextField = "codeValueDisplay";
            ddlOtherLanguageSpoken.DataValueField = "codeValue";

            ddlOtherLanguageSpoken.DataSource = dtOtherLang;
            ddlOtherLanguageSpoken.DataBind();
            ddlOtherLanguageSpoken.Items.Insert(0, new ListItem("--Select--", ""));
        }

        public void loadApplicationDetails(string applicantId)
        {
            Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> applicantTuple = am.getApplicationDetailsByApplicantId(applicantId);
            DataTable dtApplicantDetails = applicantTuple.Item1;
            DataTable dtEmploymentDetails = applicantTuple.Item2;
            DataTable dtInterviewDetails = applicantTuple.Item3;
            DataTable dtPaymentHistory = applicantTuple.Item4;
            DataTable exemptedModule = applicantTuple.Item5;

            string programmeType = "";

            if (dtApplicantDetails.Rows.Count > 0)
            {
                if (dtApplicantDetails.Rows[0]["rejectStatus"].ToString().Equals(General_Constance.STATUS_YES))
                {
                    pnApplicant.Enabled = false;
                    panelWarning.Visible = true;
                    lbWarningMsg.Text = "Applicant was withdrawn.";
                    lkbtnEnrollApplicantTop.Enabled = false;
                    btn_print.Enabled = false;
                    btnRejectAppcalicantTop.Enabled = false;

                }
                lbApplicantId.Text = applicantId;

                lbApplicationSubmittedDate.Text = dtApplicantDetails.Rows[0]["applicationDate"].ToString();
                string applicantAppliedID = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();

                DataTable dt = cbm.getAllBatchForDisplay();
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

                    Tuple<string, DataTable> programDT = cbm.getProgrammeBatchByProgrammeBatchId(applicantAppliedID);

                    if (programDT.Item2 != null)
                    {

                        ddlCourseApplied.Items.Insert(ddlCourseApplied.Items.Count, new ListItem(programDT.Item2.Rows[0]["ProgrammeCode"].ToString(), programDT.Item2.Rows[0]["programmeBatchId"].ToString()));
                        ddlCourseApplied.SelectedValue = applicantAppliedID;


                        tbCourseCodeValue.Text = programDT.Item2.Rows[0]["courseCode"].ToString();
                        tbCourseStartDate.Text = Convert.ToDateTime(programDT.Item2.Rows[0]["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                        DataRow[] rs = programDT.Item2.Select("programmeBatchId = '" + dtApplicantDetails.Rows[0]["programmeBatchId"].ToString() + "'");
                        programmeType = rs[0]["programmeType"].ToString();
                        dt.Merge(programDT.Item2);
                        hfCourseType.Value = programmeType;
                    }
                    else
                    {
                        ddlCourseApplied.SelectedValue = "";

                        tbCourseCodeValue.Text = "";
                        tbCourseStartDate.Text = "";


                        lbCourseError.Text = programDT.Item1.ToString();
                        lbCourseError.Visible = true;
                        cbCombinePayments.Visible = false;

                    }


                }
                else
                {

                    DataRow[] rs;
                    rs = dt.Select("programmeBatchId = '" + dtApplicantDetails.Rows[0]["programmeBatchId"].ToString() + "'");

                    ddlCourseApplied.SelectedValue = dtApplicantDetails.Rows[0]["programmeBatchId"].ToString();

                    tbCourseCodeValue.Text = rs[0]["courseCode"].ToString();
                    tbCourseStartDate.Text = rs[0]["programmeStartDateDisplay"].ToString();
                    programmeType = rs[0]["programmeType"].ToString();
                    hfCourseType.Value = programmeType;

                }



                tbFullName.Text = dtApplicantDetails.Rows[0]["fullName"].ToString();

                tbId.Text = dtApplicantDetails.Rows[0]["idNumber"].ToString();
                ddlIDType.SelectedValue = dtApplicantDetails.Rows[0]["idType"].ToString();
                ddlNationality.SelectedValue = dtApplicantDetails.Rows[0]["nationality"].ToString();
                ddlGender.SelectedValue = dtApplicantDetails.Rows[0]["gender"].ToString();
                ddlRace.SelectedValue = dtApplicantDetails.Rows[0]["race"].ToString();
                tbContact1.Text = dtApplicantDetails.Rows[0]["contactNumber1"].ToString();
                tbContact2.Text = dtApplicantDetails.Rows[0]["contactNumber2"].ToString();
                tbEmail.Text = dtApplicantDetails.Rows[0]["emailAddress"].ToString();
                tbDOB.Text = dtApplicantDetails.Rows[0]["birthDateDisplay"].ToString();
                tbAddr.Text = dtApplicantDetails.Rows[0]["addressLine"].ToString();
                tbPostalCode.Text = dtApplicantDetails.Rows[0]["postalCode"].ToString();
                ddlSponsorship.SelectedValue = dtApplicantDetails.Rows[0]["selfSponsored"].ToString();

                ddlHighEdu.SelectedValue = dtApplicantDetails.Rows[0]["highestEducation"].ToString();


                tbHighestEduRemarks.Text = dtApplicantDetails.Rows[0]["highestEduRemarks"].ToString();
                tbApplicantRemarks.Text = dtApplicantDetails.Rows[0]["applicantRemarks"].ToString();

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
                                ddlChnWritten.SelectedValue = wLangPro[1].ToString();
                            }

                            else if (wLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlEngWritten.SelectedValue = wLangPro[1].ToString();
                            }

                            else
                            {
                                ddlOtherLanguageWritten.SelectedValue = wLangPro[0];
                                ddlOtherLanguageProWritten.SelectedValue = wLangPro[1];
                            }
                        }
                    }
                }

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
                                ddlChnSpoken.SelectedValue = sLangPro[1].ToString();
                            }

                            else if (sLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlEngSpoken.SelectedValue = sLangPro[1].ToString();
                            }

                            else
                            {
                                ddlOtherLanguageWritten.SelectedValue = sLangPro[0];
                                ddlOtherLanguageProWritten.SelectedValue = sLangPro[1];
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

                if (programmeType.Equals(ProgrammeType.FQ.ToString()))
                {
                    cbCombinePayments.Visible = true;
                    lbPayCourseFees.Visible = true;
                    lbPayRegFees.Visible = true;
                }
                else if (programmeType.Equals(ProgrammeType.SCNWSQ.ToString()) || programmeType.Equals(ProgrammeType.SCWSQ.ToString()))
                {
                    cbCombinePayments.Visible = false;
                    lbPayCourseFees.Visible = true;
                    lbPayRegFees.Visible = false;
                }
            }

            if (!am.checkIfApplicantDetailsIsUpdated(applicantId))
            {

                if (getTraineeDetails(dtApplicantDetails.Rows[0]["idNumber"].ToString()))
                {
                    lbParticularsInfo.Text = "Applicant Details are retrieved from Trainee Details based on Applicant Identification No.";
                    lbParticularsInfo.Visible = true;
                }
                else
                {
                    lbParticularsInfo.Visible = false;
                }
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



            if (exemptedModule.Rows.Count > 0)
            {
                rptExemptedModule.DataSource = exemptedModule;
                rptExemptedModule.DataBind();

                lbNoExemptedModuleMsg.Visible = false;
            }
            else
            {
                rptExemptedModule.DataSource = null;
                rptExemptedModule.DataBind();
                lbNoExemptedModuleMsg.Visible = true;
            }


            if (dtInterviewDetails.Rows.Count > 0)
            {
                if (dtInterviewDetails.Rows[0]["interviewerId"].ToString() != "")
                    ddlInterviewer.SelectedValue = dtInterviewDetails.Rows[0]["interviewerId"].ToString();
                else
                    ddlInterviewer.SelectedIndex = 0;

                ddlInterviewStatus.SelectedValue = dtInterviewDetails.Rows[0]["interviewStatus"].ToString();
                if (dtInterviewDetails.Rows[0]["interviewDate"].ToString() != "")
                    tbInterviewDate.Text = Convert.ToDateTime(dtInterviewDetails.Rows[0]["interviewDate"].ToString()).Date.ToString("dd MMM yyyy");
                else
                    tbInterviewDate.Text = "";

                tbInterviewRemarks.Text = dtInterviewDetails.Rows[0]["interviewRemarks"].ToString();

                string shortlistStatus = dtInterviewDetails.Rows[0]["shortlistStatus"].ToString();

                if (shortlistStatus.Equals(General_Constance.STATUS_YES))
                    cbShortlisted.Checked = true;
                else
                    cbShortlisted.Checked = false;
            }



            if (dtPaymentHistory.Rows.Count > 0)
            {

                DataRow[] rs = dtPaymentHistory.Select("paymenttype = '" + PaymentType.BOTH.ToString() + "'");
                string paymentType = "";
                if (rs.Length > 0)
                    paymentType = rs[0]["paymenttype"].ToString();

                if (paymentType.Equals(PaymentType.BOTH.ToString()))
                {
                    cbCombinePayments.Checked = true;
                    cbCombinePayments.Visible = true;
                    cbCombinePayments.Enabled = false;
                    lbPayBoth.Visible = true;
                    lbPayRegFees.Visible = false;
                    lbPayCourseFees.Visible = false;

                }
                else
                {
                    if (programmeType.Equals(ProgrammeType.FQ.ToString()))
                    {
                        cbCombinePayments.Checked = false;
                        cbCombinePayments.Visible = false;
                        lbPayBoth.Visible = false;
                        lbPayRegFees.Visible = true;
                        lbPayCourseFees.Visible = true;
                    }
                    else if (programmeType.Equals(ProgrammeType.SCNWSQ.ToString()) || programmeType.Equals(ProgrammeType.SCWSQ.ToString()))
                    {
                        cbCombinePayments.Checked = false;
                        cbCombinePayments.Visible = false;
                        lbPayBoth.Visible = false;
                        lbPayRegFees.Visible = false;
                        lbPayCourseFees.Visible = true;
                    }
                }

                lbExemptedMsg.Text = "Payment had been made for this applicant. Unable to exempt any modules for this applicant.";
                lbNoExemptedModuleMsg.Visible = false;
                btnExemptedModule.Disabled = true;
                pnProgram.Enabled = false;
                lbProgMsg.Text = "Payment had been made for this applicant. Unable to change program.";
            }

            // load documents for online applicants. 

            if (dtApplicantDetails.Rows[0]["isOnlineApplicant"].ToString() == General_Constance.STATUS_YES.ToString())
            {
                List<Tuple<string, string>> res = am.getApplicantDocuments(applicantId);


                pnDocuments.Visible = true;
                pnPaymentMode.Visible = true;

                foreach (Tuple<string, string> result in res)
                {
                    if (result.Item2.ToString() == ApplicantDocumentType.ID.ToString())
                    {
                        hlIdentificationDocuments.Text = "Identification Document";
                        hlIdentificationDocuments.NavigateUrl = result.Item1;
                    }

                    else if (result.Item2.ToString() == ApplicantDocumentType.WTS.ToString())
                    {
                        hlWTS.Text = "WTS Letter";
                        hlWTS.NavigateUrl = result.Item1;
                    }

                    else if (result.Item2.ToString() == ApplicantDocumentType.CERT.ToString())
                    {
                        hlCerts.Text = "Certifications";
                        hlCerts.NavigateUrl = result.Item1;
                    }
                }


                if (dtApplicantDetails.Rows[0]["preferredModeOfPayment"].ToString() != "")
                {
                    string[] paymentModes = dtApplicantDetails.Rows[0]["preferredModeOfPayment"].ToString().Split(';');


                    foreach (string pm in paymentModes)
                    {
                        foreach (ListItem i in cblGetToKnowChannel.Items)
                        {
                            if (i.Value.Equals(pm))
                            {
                                i.Selected = true;
                            }
                        }
                    }
                }

                if (programmeType.Equals(GeneralLayer.ProgrammeType.FQ.ToString()))
                {
                    cbCombinePayments.Checked = true;
                    lbPayRegFees.Visible = false;
                    lbPayCourseFees.Visible = false;
                    lbPayBoth.Visible = true;
                    lbtnSendPaymentEmail.Visible = true;
                    cbCombinePayments.Enabled = false;
                    Session["PaymentType"] = PaymentType.BOTH.ToString();
                }
                else if (programmeType.Equals(GeneralLayer.ProgrammeType.SCNWSQ.ToString()) || programmeType.Equals(GeneralLayer.ProgrammeType.SCWSQ.ToString()))
                {
                    lbtnSendPaymentEmail.Visible = true;
                    Session["PaymentType"] = PaymentType.PROG.ToString();
                }

            }
            else
            {
                pnDocuments.Visible = false;
                pnPaymentMode.Visible = false;
            }
        }


        private bool getTraineeDetails(string nric)
        {
            Trainee_Management tm = new Trainee_Management();
            DataTable dtTraineeDetails = tm.getTraineeDetailsByTraineeNRIC(nric);

            if (dtTraineeDetails != null)
            {
                tbFullName.Text = dtTraineeDetails.Rows[0]["fullName"].ToString();
                tbId.Text = dtTraineeDetails.Rows[0]["idNumber"].ToString();
                ddlIDType.SelectedValue = dtTraineeDetails.Rows[0]["idType"].ToString();
                ddlNationality.SelectedValue = dtTraineeDetails.Rows[0]["nationality"].ToString();
                ddlGender.SelectedValue = dtTraineeDetails.Rows[0]["gender"].ToString();
                ddlRace.SelectedValue = dtTraineeDetails.Rows[0]["race"].ToString();
                tbContact1.Text = dtTraineeDetails.Rows[0]["contactNumber1"].ToString();
                tbContact2.Text = dtTraineeDetails.Rows[0]["contactNumber2"].ToString();
                tbEmail.Text = dtTraineeDetails.Rows[0]["emailAddress"].ToString();
                tbDOB.Text = dtTraineeDetails.Rows[0]["birthDateDisplay"].ToString();
                tbAddr.Text = dtTraineeDetails.Rows[0]["addressLine"].ToString();
                tbPostalCode.Text = dtTraineeDetails.Rows[0]["postalCode"].ToString();
                ddlHighEdu.SelectedValue = dtTraineeDetails.Rows[0]["highestEducation"].ToString();
                tbHighestEduRemarks.Text = dtTraineeDetails.Rows[0]["highestEduRemarks"].ToString();

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
                                ddlChnWritten.SelectedValue = wLangPro[1].ToString();
                            }

                            else if (wLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlEngWritten.SelectedValue = wLangPro[1].ToString();
                            }
                            else
                            {
                                ddlOtherLanguageWritten.SelectedValue = wLangPro[0];
                                ddlOtherLanguageProWritten.SelectedValue = wLangPro[1];
                            }
                        }
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
                                ddlChnSpoken.SelectedValue = sLangPro[1].ToString();
                            }

                            else if (sLangPro[0].Equals(General_Constance.ENG))
                            {
                                ddlEngSpoken.SelectedValue = sLangPro[1].ToString();
                            }
                            else
                            {
                                ddlOtherLanguageSpoken.SelectedValue = sLangPro[0];
                                ddlOtherLanguageProSpoken.SelectedValue = sLangPro[1];
                            }
                        }

                    }
                }
                return true;

            }
            else
            {
                return false;
            }
        }

        protected void lkbtnEnrollApplicant_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            panelSuccess.Visible = false;

            if (!am.checkIfApplicantDetailsIsUpdated(applicantId))
            {


                string fullName = tbFullName.Text;
                string idNumber = tbId.Text;
                string idType = ddlIDType.SelectedValue;
                string nationality = ddlNationality.SelectedValue;
                string gender = ddlGender.SelectedValue;
                string race = ddlRace.SelectedValue;
                string contactNumber1 = tbContact1.Text;
                string contactNumber2 = tbContact2.Text;
                string emailAddress = tbEmail.Text;
                DateTime birthDate = Convert.ToDateTime(tbDOB.Text);
                string address = tbAddr.Text;
                string postalCode = tbPostalCode.Text;
                string highestEdu = ddlHighEdu.SelectedValue;
                string highestEduRemarks = tbHighestEduRemarks.Text;
                string applicantRemarks = tbApplicantRemarks.Text;
                string sponsorship = ddlSponsorship.SelectedValue;

                string slangScore = "";
                slangScore += General_Constance.ENG + ":" + ddlEngSpoken.SelectedValue + ";";
                slangScore += General_Constance.CHN + ":" + ddlChnSpoken.SelectedValue + ";";
                if (!ddlOtherLanguageSpoken.SelectedValue.Equals("") && ddlOtherLanguageProSpoken.SelectedValue.Equals(""))
                {
                    slangScore += ddlOtherLanguageSpoken.SelectedValue + ":" + ddlOtherLanguageProSpoken.SelectedValue + ";";
                }

                string wlangScore = "";
                wlangScore += General_Constance.ENG + ":" + ddlEngWritten.SelectedValue + ";";

                wlangScore += General_Constance.CHN + ":" + ddlChnWritten.SelectedValue + ";";
                if (!ddlOtherLanguageSpoken.SelectedValue.Equals("") && ddlOtherLanguageProSpoken.SelectedValue.Equals(""))
                {
                    slangScore += ddlOtherLanguageWritten.SelectedValue + ":" + ddlOtherLanguageProWritten.SelectedValue + ";";
                }

                string channelChecked = "";
                foreach (ListItem i in cblGetToKnowChannel.Items)
                {

                    if (i.Selected)
                    {
                        channelChecked += i.Value + ", ";
                    }
                }


                Page.Validate();
                if (Page.IsValid)
                {
                    bool success = am.updateApplicantDetails(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2,
                            emailAddress, race, birthDate, address, postalCode, highestEdu, highestEduRemarks, slangScore, wlangScore, channelChecked, sponsorship, LoginID, applicantRemarks);
                }

            }

            Page.Validate();
            if (Page.IsValid)
            {



                Tuple<int, string, string> tupleResult = tm.enrollApplicant(lbApplicantId.Text, LoginID);

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

        }

        protected void isContactEmailBothEmpty(object sourc, ServerValidateEventArgs args)
        {
            if (tbContact1.Text.Trim() == "" && tbEmail.Text.Trim() == "")
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void isInterviewNeeded(object source, ServerValidateEventArgs args)
        {
            if (hfCourseType.Value == GeneralLayer.ProgrammeType.FQ.ToString())
            {
                if (ddlInterviewStatus.SelectedValue == GeneralLayer.InterviewStatus.NREQ.ToString())
                {
                    args.IsValid = false;
                }
                else
                {
                    args.IsValid = true;
                }
            }

        }

        protected void IsNRICValid(object source, ServerValidateEventArgs args)
        {

            if (ddlIDType.SelectedValue == ((int)IDType.Oth).ToString())
            {
                args.IsValid = true;
            }
            else
            {


                string ic = args.Value;
                if (ic.Length != 9)
                {
                    args.IsValid = false;

                }
                else
                {


                    int[] icArray = new int[7];
                    string firstChar = ic[0].ToString();
                    string lastChar = ic[8].ToString();

                    for (int i = 0; i < 8; i++)
                    {
                        try
                        {
                            icArray[i] = int.Parse(ic[i + 1].ToString());
                        }
                        catch (Exception ex)
                        {
                            args.IsValid = false;
                        }
                    }

                    icArray[0] *= 2;
                    icArray[1] *= 7;
                    icArray[2] *= 6;
                    icArray[3] *= 5;
                    icArray[4] *= 4;
                    icArray[5] *= 3;
                    icArray[6] *= 2;

                    int weight = 0;
                    for (int j = 0; j < 7; j++)
                    {
                        weight += icArray[j];
                    }


                    int offset = (firstChar == "T" || firstChar == "G") ? 4 : 0;
                    int temp = (offset + weight) % 11;
                    string[] st = new string[] { "J", "Z", "I", "H", "G", "F", "E", "D", "C", "B", "A" };
                    string[] fg = new string[] { "X", "W", "U", "T", "R", "Q", "P", "N", "M", "L", "K" };
                    string theAlpha = "";
                    if (firstChar == "S" || firstChar == "T")
                    {
                        theAlpha = st[temp];
                    }
                    else if (firstChar == "F" || firstChar == "G")
                    {
                        theAlpha = fg[temp];
                    }

                    if (lastChar != theAlpha)
                    {
                        args.IsValid = false;

                    }
                    else
                    {

                        args.IsValid = true;
                    }
                }


            }

        }

        private void loadLanguageProficiency(object sender)
        {
            DropDownList ddl = (DropDownList)sender;
            DataTable dtLangfPr = am.getAllLanguageProficiencyCodeReference();

            ddl.DataTextField = "codeValueDisplay";
            ddl.DataValueField = "codeValue";

            ddl.DataSource = dtLangfPr;
            ddl.DataBind();

            ddl.Items.Insert(0, new ListItem("--Select--", ""));

        }
        private void loadAllChannelInfo()
        {

            DataTable dtChannel = am.getAllGetToKnowChannelCodeReference();

            cblGetToKnowChannel.DataTextField = "codeValueDisplay";
            cblGetToKnowChannel.DataValueField = "codeValue";

            cblGetToKnowChannel.DataSource = dtChannel;
            cblGetToKnowChannel.DataBind();
        }


        private void loadAllProgrammeBatchInfo()
        {

            DataTable dtProgrammeBatchInfo = cbm.getAllBatchForDisplay();

            ddlCourseApplied.DataTextField = "ProgrammeCode";
            ddlCourseApplied.DataValueField = "programmeBatchId";

            ddlCourseApplied.DataSource = dtProgrammeBatchInfo;
            ddlCourseApplied.DataBind();

            ddlCourseApplied.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadSponsorshipType()
        {

            DataTable dtSponsorshipCode = am.getSponsorship();
            ddlSponsorship.DataSource = dtSponsorshipCode;
            ddlSponsorship.DataBind();
            ddlSponsorship.Items.Insert(0, new ListItem("--Select--", ""));
        }


        private void loadAllEducationReferenceInfo()
        {

            DataTable dtEducationCode = am.getAllEducationCodeReference();

            ddlHighEdu.DataTextField = "codeValueDisplay";
            ddlHighEdu.DataValueField = "codeValue";

            ddlHighEdu.DataSource = dtEducationCode;
            ddlHighEdu.DataBind();

            ddlHighEdu.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadNationality()
        {

            DataTable dtNationality = am.getNationalityCodeReference();

            ddlNationality.DataSource = dtNationality;
            ddlNationality.DataBind();
            ddlNationality.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadRace()
        {
            DataTable dtRace = am.getRaceCodeReference();
            ddlRace.DataSource = dtRace;
            ddlRace.DataTextField = "codeValueDisplay";
            ddlRace.DataValueField = "codeValue";
            ddlRace.DataBind();
            ddlRace.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadIdType()
        {

            DataTable dtIdtype = am.getIdentificationTypeCodeReference();
            ddlIDType.DataSource = dtIdtype;
            ddlIDType.DataBind();
            ddlIDType.Items.Insert(0, new ListItem("--Select--", ""));

        }

        private void loadEmplFields()
        {
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

        protected void btnUpdateCourseDetails_Click(object sender, EventArgs e)
        {

            string applicantId = lbApplicantId.Text;

            string programmeBatchId = ddlCourseApplied.SelectedValue;
            bool success = am.updateApplicantCourseProjectCode(applicantId, programmeBatchId);
            if (success)
            {
                panelSuccess.Visible = true;
                panelError.Visible = false;
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = true;
            }
            loadApplicationDetails(applicantId);
        }

        protected void ddlCourseApplied_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectProgrammeBatchId = ddlCourseApplied.SelectedValue;

            Tuple<string, DataTable> results = cbm.getProgrammeBatchByProgrammeBatchId(selectProgrammeBatchId);

            if (results.Item2 != null)
            {
                tbCourseStartDate.Text = Convert.ToDateTime(results.Item2.Rows[0]["programmeStartDate"].ToString()).Date.ToString("dd MMM yyyy");

                tbCourseCodeValue.Text = results.Item2.Rows[0]["CourseCode"].ToString();
                hfCourseType.Value = results.Item2.Rows[0]["programmeType"].ToString();

            }

        }

        protected void btnUpdateInterview_ServerClick(object sender, EventArgs e)
        {

            if (hfCourseType.Value == ProgrammeType.FQ.ToString())
            {
                string applicantId = lbApplicantId.Text;
                DateTime interviewDate = Convert.ToDateTime(null);

                if (ddlInterviewStatus.SelectedValue == GeneralLayer.InterviewStatus.FAILED.ToString() || ddlInterviewStatus.SelectedValue == GeneralLayer.InterviewStatus.PASSED.ToString())
                    interviewDate = DateTime.Parse(tbInterviewDate.Text).Date;

                string selectedInterviewStatus = ddlInterviewStatus.SelectedValue;
                string interviewRemarks = tbInterviewRemarks.Text;
                string shortlistStatus = "";
                int userId = LoginID;
                int interviewerId = ddlInterviewer.SelectedIndex;

                if (ddlInterviewer.SelectedIndex != 0)
                    interviewerId = int.Parse(ddlInterviewer.SelectedValue);

                if (cbShortlisted.Checked)
                    shortlistStatus = General_Constance.STATUS_YES;
                else
                    shortlistStatus = General_Constance.STATUS_NO;

                bool success = am.updateApplicantInterviewDetails(applicantId, interviewerId, selectedInterviewStatus, shortlistStatus, interviewDate, interviewRemarks, userId);

                if (success)
                {
                    panelError.Visible = false;
                    panelSuccess.Visible = true;
                }
                else
                {
                    panelError.Visible = true;
                    panelSuccess.Visible = false;
                }
            }
            else
            {
                string applicantId = lbApplicantId.Text;
                DateTime interviewDate = Convert.ToDateTime(null);
                string selectedInterviewStatus = ddlInterviewStatus.SelectedValue;
                string interviewRemarks = tbInterviewRemarks.Text;
                string shortlistStatus = "";
                int userId = LoginID;
                int interviewerId = ddlInterviewer.SelectedIndex;

                if (cbShortlisted.Checked)
                    shortlistStatus = General_Constance.STATUS_YES;
                else
                    shortlistStatus = General_Constance.STATUS_NO;

                bool success = am.updateApplicantInterviewDetails(applicantId, interviewerId, selectedInterviewStatus, shortlistStatus, interviewDate, interviewRemarks, userId);

                if (success)
                {
                    panelError.Visible = false;
                    panelSuccess.Visible = true;
                }
                else
                {
                    panelError.Visible = true;
                    panelSuccess.Visible = false;
                }
            }

        }

        protected void btnExemptedModule_ServerClick(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string courseCode = tbCourseCodeValue.Text;
            string programmeBatchId = ddlCourseApplied.SelectedValue;//ddlProjectCodeValue.SelectedValue;

            Response.Redirect(applicant_module_exemption.PAGE_NAME + "?a=" + applicantId + "&pb=" + programmeBatchId);
        }


        protected void btnUpdateApplicantDetails_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;

            string fullName = tbFullName.Text;
            string idNumber = tbId.Text;
            string idType = ddlIDType.SelectedValue;
            string nationality = ddlNationality.SelectedValue;
            string gender = ddlGender.SelectedValue;
            string race = ddlRace.SelectedValue;
            string contactNumber1 = tbContact1.Text;
            string contactNumber2 = tbContact2.Text;
            string emailAddress = tbEmail.Text;
            DateTime birthDate = Convert.ToDateTime(tbDOB.Text);
            string address = tbAddr.Text;
            string postalCode = tbPostalCode.Text;
            string highestEdu = ddlHighEdu.SelectedValue;
            string highestEduRemarks = tbHighestEduRemarks.Text;
            string applicantRemarks = tbApplicantRemarks.Text;
            string sponsorship = ddlSponsorship.SelectedValue;

            string slangScore = "";
            slangScore += General_Constance.ENG + ":" + ddlEngSpoken.SelectedValue + ";";
            slangScore += General_Constance.CHN + ":" + ddlChnSpoken.SelectedValue + ";";
            if (!ddlOtherLanguageSpoken.SelectedValue.Equals("") && !ddlOtherLanguageProSpoken.SelectedValue.Equals(""))
            {
                slangScore += ddlOtherLanguageSpoken.SelectedValue + ":" + ddlOtherLanguageProSpoken.SelectedValue + ";";
            }

            string wlangScore = "";
            wlangScore += General_Constance.ENG + ":" + ddlEngWritten.SelectedValue + ";";

            wlangScore += General_Constance.CHN + ":" + ddlChnWritten.SelectedValue + ";";
            if (!ddlOtherLanguageSpoken.SelectedValue.Equals("") && !ddlOtherLanguageProSpoken.SelectedValue.Equals(""))
            {
                wlangScore += ddlOtherLanguageWritten.SelectedValue + ":" + ddlOtherLanguageProWritten.SelectedValue + ";";
            }

            string channelChecked = "";
            foreach (ListItem i in cblGetToKnowChannel.Items)
            {

                if (i.Selected)
                {
                    channelChecked += i.Value + ", ";
                }
            }
            bool success = am.updateApplicantDetails(applicantId, fullName, idNumber, idType, nationality, gender, contactNumber1, contactNumber2,
                    emailAddress, race, birthDate, address, postalCode, highestEdu, highestEduRemarks, slangScore, wlangScore, channelChecked, sponsorship, LoginID, applicantRemarks);

            if (success)
            {
                panelError.Visible = false;
                panelSuccess.Visible = true;
            }
            else
            {
                panelError.Visible = true;
                panelSuccess.Visible = false;
                lbParticularsInfo.Visible = false;
            }
        }

        protected void btn_PayRegFees_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;


            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.REG.ToString()
                   + "&" + applicant_programme_payment.PREV_QUERY + "=A");
        }

        protected void btnPayCourseFee_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&" + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.PROG.ToString()
               + "&" + applicant_programme_payment.PREV_QUERY + "=A");
        }

        protected void btnCombinedPayment_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;

            Response.Redirect(applicant_programme_payment.PAGE_NAME + "?" + applicant_programme_payment.APPLICANT_QUERY + "=" + applicantId + "&"
                + applicant_programme_payment.TYPE_QUERY + "=" + PaymentType.BOTH.ToString()
                + "&" + applicant_programme_payment.PREV_QUERY + "=A");
        }

        protected void cbCombinePayments_CheckedChanged(object sender, EventArgs e)
        {
            if (cbCombinePayments.Checked)
            {
                lbPayRegFees.Visible = false;
                lbPayCourseFees.Visible = false;
                lbPayBoth.Visible = true;
            }
            else
            {
                lbPayRegFees.Visible = true;
                lbPayCourseFees.Visible = true;
                lbPayBoth.Visible = false;
            }
        }

        protected void btn_print_Click(object sender, EventArgs e)
        {
            string applicantId = Request.QueryString[APPLICANT_QUERY];

            Applicant_Management am = new Applicant_Management();
            Tuple<DataTable, DataTable, DataTable, DataTable, DataTable> applicantTuple = am.getApplicationDetailsByApplicantId(applicantId);

            DataTable DataTable_ApplicantDetails = new DataTable();
            DataTable DataTable_EmploymentDetails = new DataTable();

            DataTable Nationality_N_Race = new DataTable();
            Nationality_N_Race.Columns.Add("race", typeof(String));
            Nationality_N_Race.Columns.Add("nationality", typeof(String));
            Nationality_N_Race.Columns.Add("gender", typeof(String));

            DataRow row = Nationality_N_Race.NewRow();

            row["race"] = ddlNationality.SelectedItem.Text;
            row["nationality"] = ddlRace.SelectedItem.Text;
            row["gender"] = ddlGender.SelectedItem.Text;

            Nationality_N_Race.Rows.Add(row);
            DataTable_ApplicantDetails = applicantTuple.Item1;
            DataTable_EmploymentDetails = applicantTuple.Item2;

            HttpContext.Current.Session["dtNationality_N_Race"] = Nationality_N_Race;
            HttpContext.Current.Session["dtApplicantDetails"] = DataTable_ApplicantDetails;
            HttpContext.Current.Session["dtEmploymentDetails"] = DataTable_EmploymentDetails;

            ScriptManager.RegisterStartupScript(Page, typeof(Page), "OpenWindow", "window.open('applicant-details-print.aspx'); ", true);
        }

        protected void btnRejectAppcalicant_Click(object sender, EventArgs e)
        {
            //Tuple<string, DataTable> dtCourse = cbm.getProgrammeBatchByProgrammeBatchId(ddlCourseApplied.SelectedValue);

            //if (dtCourse.Item2 == null)
            //{
            //    panelError.Visible = true;
            //    lblErrorMsg.Text = dtCourse.Item1 + "<br> Unable to reject the applicant.";
            //}
            //else
            //{
            int userId = LoginID;
            string applicantId = lbApplicantId.Text;
            bool success = am.updateApplicantStatusReject(applicantId, userId);
            if (success)
            {
                Page.Response.Redirect(Page.Request.Url.ToString(), true);
            }
            //}


        }

        protected void lbtnSendPaymentEmail_Click(object sender, EventArgs e)
        {
            Email_Handler em = new Email_Handler();
            string fromEmail = ConfigurationManager.AppSettings["EnrollmentEmail"].ToString();
            string toEmail = tbEmail.Text.Trim();
            string bccEmail = ConfigurationManager.AppSettings["EnrollmentEmail"].ToString();

            string courseFeesAmt = "";
            string course = "";
            string dateApplied = "";
            string url = ConfigurationManager.AppSettings["ACIWPURL"].ToString();

            DataTable dtPaymentDetails = (new Applicant_Management()).getApplicantDetailsForPayment(Request.QueryString[APPLICANT_QUERY].ToString());

            decimal totalAmt = decimal.Parse(dtPaymentDetails.Rows[0]["programmePayableAmount"].ToString()) - (dtPaymentDetails.Rows[0]["subsidyAmt"].ToString() == "" ? 0 :decimal.Parse(dtPaymentDetails.Rows[0]["subsidyAmt"].ToString())) + decimal.Parse
                (dtPaymentDetails.Rows[0]["GSTPayableAmount"].ToString()) + (dtPaymentDetails.Rows[0]["registrationFee"].ToString() == "" ? 0 : decimal.Parse(dtPaymentDetails.Rows[0]["registrationFee"].ToString()));

            int totalAmtCents = (new Utlity_Handler()).compute_cents(totalAmt);
            url += "?amt=" + (new Cryptography()).EncryptURLSafe(totalAmtCents.ToString()) + "&applid=" + Request.QueryString[APPLICANT_QUERY].ToString() +"&type=" + Session["PaymentType"].ToString();
           
            string body = "Dear " + tbFullName.Text.Trim() + ", <br>";
            body += "You're required to make a payment of " + courseFeesAmt + " for the course " + course + " you have applied on " + dateApplied;
            body += "Click here <a href='" + url + "' target = _blank> for payment thru' nets</a> <br>";


            string footer = "<br><br><br>From ACI";

            string fullEmail = body + footer;


        }

    }
}