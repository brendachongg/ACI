using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeneralLayer;
using LogicLayer;
using System.IO;

namespace ACI_TMS
{
    public partial class applicant_details_print : BasePage
    {
        public const string PAGE_NAME = "Forms/ApplicationForm.aspx";
        Applicant_Management am = new Applicant_Management();

        public applicant_details_print()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_VIEW, applicant.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

            //load details
            loadApplicantDetails();

        }
        private void loadApplicantDetails()
        {
            DataTable DataTable_ApplicantDetails = new DataTable();
            DataTable DataTable_EmploymentDetails = new DataTable();
            DataTable DataTable_details = new DataTable();

            if (HttpContext.Current.Session["dtApplicantDetails"] != null)
            {
                DataTable_ApplicantDetails = HttpContext.Current.Session["dtApplicantDetails"] as DataTable;
                DataTable_EmploymentDetails = HttpContext.Current.Session["dtEmploymentDetails"] as DataTable;
                DataTable_details = HttpContext.Current.Session["dtNationality_N_Race"] as DataTable;

                if (DataTable_ApplicantDetails.Rows.Count != 0)
                {


                    lbApplicantId.Text = DataTable_ApplicantDetails.Rows[0]["applicantId"].ToString();

                    Tuple<string, byte[]> rs = (new Applicant_Management()).getApplicantSignature(DataTable_ApplicantDetails.Rows[0]["applicantId"].ToString());


                    if (rs.Item1 != "")
                    {
                        MemoryStream ms = new MemoryStream((byte[])rs.Item2);
                        BinaryReader br = new BinaryReader(ms);
                        byte[] bytes = br.ReadBytes((Int32)ms.Length);

                        if (bytes.Length > 0)
                        {
                            string imgString = Convert.ToBase64String(bytes, 0, bytes.Length);
                            imgSig.ImageUrl = "data:image/jpeg;base64," + imgString;
                            imgSig.Visible = true;
                        }
                        else
                            Label6.Visible = true;

                    }

                    tbProjCode.Text = DataTable_ApplicantDetails.Rows[0]["projectCode"].ToString();
                    tbCourseCode.Text = DataTable_ApplicantDetails.Rows[0]["courseCode"].ToString();
                    tbProgrammeTitle.Text = DataTable_ApplicantDetails.Rows[0]["programmeTitle"].ToString();
                    tbProgrammeStartDate.Text = Convert.ToDateTime(DataTable_ApplicantDetails.Rows[0]["programmeStartDate"].ToString()).ToString("dd MMM yyyy");
                    tbProgrammeEndDate.Text = Convert.ToDateTime(DataTable_ApplicantDetails.Rows[0]["programmeCompletionDate"].ToString()).ToString("dd MMM yyyy");
                    tbClassCode.Text = DataTable_ApplicantDetails.Rows[0]["batchcode"].ToString();

                    lblName.Text = DataTable_ApplicantDetails.Rows[0]["fullName"].ToString();

                    lblNric.Text = DataTable_ApplicantDetails.Rows[0]["idNumber"].ToString();

                    DataTable dtType = am.getCodeValueDisplay(DataTable_ApplicantDetails.Rows[0]["idType"].ToString(), "IDTYPE");
                    if (dtType.Rows.Count > 0)
                        lbltype.Text = dtType.Rows[0]["codeValueDisplay"].ToString();
                    else
                        lbltype.Text = "-";

                    DataTable dtNationality = am.getCodeValueDisplay(DataTable_ApplicantDetails.Rows[0]["nationality"].ToString(), "NATION");
                    if (dtNationality.Rows.Count > 0)
                        lbNationalityText.Text = dtNationality.Rows[0]["codeValueDisplay"].ToString();
                    else
                        lbNationalityText.Text = "-";

                    lbGenderText.Text = DataTable_details.Rows[0]["gender"].ToString();

                    DataTable dtRace = am.getCodeValueDisplay(DataTable_ApplicantDetails.Rows[0]["race"].ToString(), "RACE");
                    if (dtRace.Rows.Count > 0)
                        lbRaceText.Text = dtRace.Rows[0]["codeValueDisplay"].ToString();
                    else
                        lbRaceText.Text = "-";

                    lbContactNo1Text.Text = DataTable_ApplicantDetails.Rows[0]["contactNumber1"].ToString();

                    lbContactNo2Text.Text = DataTable_ApplicantDetails.Rows[0]["contactNumber2"].ToString() == "" ? "-" : DataTable_ApplicantDetails.Rows[0]["contactNumber2"].ToString();

                    lbEmailAddText.Text = DataTable_ApplicantDetails.Rows[0]["emailAddress"].ToString() == "" ? "-" : DataTable_ApplicantDetails.Rows[0]["emailAddress"].ToString();

                    lbDOBText.Text = Convert.ToDateTime(DataTable_ApplicantDetails.Rows[0]["birthDateDisplay"]).ToString("dd MMM yyyy");

                    lbAddressText.Text = DataTable_ApplicantDetails.Rows[0]["addressLine"].ToString();

                    lbPostalCodeText.Text = DataTable_ApplicantDetails.Rows[0]["postalCode"].ToString();

                    DataTable highestEdu = am.getCodeValueDisplay(DataTable_ApplicantDetails.Rows[0]["highestEducation"].ToString(), "EDU");

                    lbHighestEduText.Text = highestEdu.Rows.Count > 0 ? highestEdu.Rows[0]["codeValueDisplay"].ToString() : "-";

                    lbHighestEduRemarkText.Text = DataTable_ApplicantDetails.Rows[0]["highestEduRemarks"].ToString() == "" ? "-" : DataTable_ApplicantDetails.Rows[0]["highestEduRemarks"].ToString();

                    string spokenLang = DataTable_ApplicantDetails.Rows[0]["spokenLanguage"].ToString();
                    string[] sSpokenLang = spokenLang.Split(';');
                    foreach (string sLang in sSpokenLang)
                    {


                        string[] sLangAndEff = sLang.Split(':');

                        if (sLangAndEff.Length > 0)
                        {
                            if (sLangAndEff[0] != "" && sLangAndEff[1] != "")
                                lblSL.Text += am.getCodeValueDisplay(sLangAndEff[0], "LANG").Rows[0]["codeValueDisplay"].ToString() + ": " + am.getCodeValueDisplay(sLangAndEff[1], "LANGPR").Rows[0]["codeValueDisplay"].ToString() + " ";
                        }
                    }

                }


                string writtenLang = DataTable_ApplicantDetails.Rows[0]["writtenLanguage"].ToString();
                string[] sWrittenLang = writtenLang.Split(';');
                foreach (string wLang in sWrittenLang)
                {

                    if (wLang.Trim().Length != 0)
                    {
                        string[] wLangAndEff = wLang.Split(':');
                        if (wLangAndEff.Length > 0)
                        {
                            if (wLangAndEff[0] != "" && wLangAndEff[1] != "")
                                lblWL.Text += am.getCodeValueDisplay(wLangAndEff[0], "LANG").Rows[0]["codeValueDisplay"].ToString() + ": " + am.getCodeValueDisplay(wLangAndEff[1], "LANGPR").Rows[0]["codeValueDisplay"].ToString() + " ";

                        }
                    }
                }

                string getToKnow = DataTable_ApplicantDetails.Rows[0]["getToKnowChannel"].ToString();

                string[] channels = getToKnow.Split(',');
                string getChannels = "";
                foreach (string c in channels)
                {
                    if (c.Trim().Length != 0)
                        getChannels = am.getCodeValueDisplay(c.Trim(), "KNOWN").Rows[0]["codeValueDisplay"].ToString() + ",";
                }


                lblGTKL.Text = getChannels == "" ? "-" : getChannels.Substring(0, getChannels.Length - 1);

                DataTable dtSpon = am.getCodeValueDisplay(DataTable_ApplicantDetails.Rows[0]["selfSponsored"].ToString(), "SPON");
                lbSpon.Text = dtSpon.Rows.Count > 0 ? dtSpon.Rows[0]["codeValueDisplay"].ToString() : "-";

            }


            if (HttpContext.Current.Session["dtEmploymentDetails"] != null)
            {

                if (DataTable_EmploymentDetails.Rows.Count <= 0)
                {
                    pnCurrEmploymentHistory.Visible = false;
                    //pnPrevEmp.Visible = false;
                }
                else
                {
                    foreach (DataRow row in DataTable_EmploymentDetails.Rows)
                    {
                        if (row["currentEmployment"].ToString() == General_Constance.STATUS_YES.ToString())
                        {
                            pnCurrEmploymentHistory.Visible = true;
                            lblCE.Text = DataTable_EmploymentDetails.Rows[0]["position"].ToString();

                            lbCurrCompanyName.Text = DataTable_EmploymentDetails.Rows[0]["companyName"].ToString();

                            lbCurrStartDate.Text = Convert.ToDateTime(DataTable_EmploymentDetails.Rows[0]["employmentStartDate"]).ToString("dd MMM yyyy").Substring(0, 11);
                            lbCurrDepartment.Text = DataTable_EmploymentDetails.Rows[0]["companyDepartment"].ToString();
                        }

                    }
                }

            }
        }
    }
}