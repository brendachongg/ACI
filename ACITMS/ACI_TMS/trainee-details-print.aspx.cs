using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeneralLayer;
using System.Data;
using LogicLayer;

namespace ACI_TMS
{
    public partial class trainee_details_print : BasePage
    {
        public const string PAGE_NAME = "trainee-details-print.aspx";
        public trainee_details_print()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_VIEW, trainee_details.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            loadTraineeDetails();
        }

        private void loadTraineeDetails()
        {
            if (HttpContext.Current.Session["dtTraineeDetails"] != null)
            {
                DataTable dtTrainee = HttpContext.Current.Session["dtTraineeDetails"] as DataTable;

                if (dtTrainee.Rows.Count > 0)
                {
                    lbAddressText.Text = dtTrainee.Rows[0]["addressLine"].ToString();
                    lbTraineeIdText.Text = dtTrainee.Rows[0]["TraineeId"].ToString();
                    lbFullNameText.Text = dtTrainee.Rows[0]["fullName"].ToString();
                    lbIdNoText.Text = dtTrainee.Rows[0]["idNumber"].ToString();
                    lbIdTypeText.Text = dtTrainee.Rows[0]["idType"].ToString();
                    lbNationalityText.Text = dtTrainee.Rows[0]["nationality"].ToString();
                    lbContactNo1Text.Text = dtTrainee.Rows[0]["contactNumber1"].ToString();
                    lbContactNo2Text.Text = dtTrainee.Rows[0]["contactNumber2"].ToString();
                    lbRaceText.Text = dtTrainee.Rows[0]["race"].ToString();
                    lbGenderText.Text = dtTrainee.Rows[0]["gender"].ToString();
                    lbEmailAddText.Text = dtTrainee.Rows[0]["emailAddress"].ToString();
                    lbDOBText.Text = dtTrainee.Rows[0]["birthDateDisplay"].ToString();
                    lbPostalCodeText.Text = dtTrainee.Rows[0]["postalCode"].ToString();
                    lbHighestEduText.Text = dtTrainee.Rows[0]["highestEducation"].ToString();
                    lbHighestEduRemarkText.Text = dtTrainee.Rows[0]["highestEduRemarks"].ToString();
                    lbEngText.Text = dtTrainee.Rows[0]["engSpoken"].ToString();
                    lbChiText.Text = dtTrainee.Rows[0]["chnSpoken"].ToString();
                    lbOtherLangPro.Text = dtTrainee.Rows[0]["othSpokenLang"].ToString() == "" ? "" : dtTrainee.Rows[0]["othSpokenLang"].ToString();
                    lbOtherLangProText.Text = dtTrainee.Rows[0]["othSpoken"].ToString() == "" ? "" : dtTrainee.Rows[0]["othSpoken"].ToString();
                    lbWEngText.Text = dtTrainee.Rows[0]["engWritten"].ToString();
                    lbWChiText.Text = dtTrainee.Rows[0]["chnWritten"].ToString();
                    lbWItherLangPro.Text = dtTrainee.Rows[0]["othWrittenLang"].ToString() == "" ? "" : dtTrainee.Rows[0]["othWrittenLang"].ToString();
                    lbWItherLangProText.Text = dtTrainee.Rows[0]["othWrittenLangPro"].ToString() == "" ? "" : dtTrainee.Rows[0]["othWrittenLangPro"].ToString();
                }
            }

            if (HttpContext.Current.Session["dtEmploymentHistory"] != null)
            {
                List<EmploymentHistory> empl = HttpContext.Current.Session["dtEmploymentHistory"] as List<EmploymentHistory>;
                if (empl.Count <= 0)
                {
                    pnNoEmploymentHistory.Visible = true;
                    lbNoHistory.Text = "No employment history found";
                }
                else
                {
                    
                    foreach (EmploymentHistory element in empl)
                    {
                        if (element.current == General_Constance.STATUS_YES)
                        {
                            pnCurrEmploymentHistory.Visible = true;
                            lbCurrCompanyName.Text = element.companyName;
                            lbCurrDepartment.Text = element.dept;
                            lbCurrEmplStatus.Text = element.status;
                            lbCurrentDesignation.Text = element.designation;
                            lbCurrSalary.Text = element.salary.ToString();
                            lbCurrEmplOccupation.Text = element.occupationType;
                            lbCurrStartDate.Text = element.dtStart.ToString("dd MMM yyyy");
                        }
                        else
                        {
                            pnPrevEmploymentHistory.Visible = true;
                            lbPrevCompanyName.Text = element.companyName;
                            lbPrevDesignation.Text = element.designation;
                            lbPrevDept.Text = element.dept;
                            lbPrevEmplType.Text = element.status;
                            lbPrevOccupationType.Text = element.occupationType;
                            lbPrevSalary.Text = element.salary.ToString();
                            lbPrevStartDate.Text = element.dtStart.ToString("dd MMM yyyy");
                            lbPrevEndDate.Text = element.dtEnd.ToString("dd MMM yyyy");
                        }
                    }

                }

            }

            if(HttpContext.Current.Session["dtTraineeProgrammeInfo"] != null){
                     DataTable dtTraineeProgrammeInfo = HttpContext.Current.Session["dtTraineeProgrammeInfo"] as DataTable;

                if (dtTraineeProgrammeInfo.Rows.Count > 0)
                {
                    tbProjCode.Text = dtTraineeProgrammeInfo.Rows[0]["projectCode"].ToString();
                    tbCourseCode.Text = dtTraineeProgrammeInfo.Rows[0]["courseCode"].ToString();
                    tbProgrammeTitle.Text = dtTraineeProgrammeInfo.Rows[0]["programmeTitle"].ToString();
                    tbProgrammeStartDate.Text = Convert.ToDateTime(dtTraineeProgrammeInfo.Rows[0]["programmeStartDate"].ToString()).ToString("dd MMM yyyy");
                    tbProgrammeEndDate.Text = Convert.ToDateTime(dtTraineeProgrammeInfo.Rows[0]["programmeCompletionDate"].ToString()).ToString("dd MMM yyyy");
                    tbClassCode.Text = dtTraineeProgrammeInfo.Rows[0]["batchcode"].ToString();
                }
            }
        }
    }
}