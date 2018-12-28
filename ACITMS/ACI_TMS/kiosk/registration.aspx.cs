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

namespace ACI_TMS.kiosk
{
    public partial class registration : BasePage
    {
        public const string PAGE_NAME = "registration.aspx";
        private const string APPID_KEY = "appId";

        //TODO: to create a user in the db for kiosk user
        public const int KIOSK_ID = 0;

        public string ApplicantId{
            get { return (string)ViewState[APPID_KEY]; }
        }

        public registration()
            : base(PAGE_NAME)
        {
            PrevPage = PAGE_NAME;
            ErrorPageTitle = "ACI Registration Kiosk";
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadStep1Fields();
                loadStep2Fields();
                loadStep4Fields();
                loadStep5Fields();
                loadStep6Fields();
            }
            else
            {
                panelSysError.Visible = false;
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

        private void loadStep4Fields()
        {
            Applicant_Management am = new Applicant_Management();

            DataTable dt = am.getAllLanguageProficiencyCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving language profiency options.");
                return;
            }

            ddlEngSpoken.DataSource = dt;
            ddlEngSpoken.DataBind();
            ddlEngSpoken.Items.Insert(0, new ListItem("--Select--", ""));

            ddlEngWritten.DataSource = dt;
            ddlEngWritten.DataBind();
            ddlEngWritten.Items.Insert(0, new ListItem("--Select--", ""));

            ddlChnSpoken.DataSource = dt;
            ddlChnSpoken.DataBind();
            ddlChnSpoken.Items.Insert(0, new ListItem("--Select--", ""));

            ddlChnWritten.DataSource = dt;
            ddlChnWritten.DataBind();
            ddlChnWritten.Items.Insert(0, new ListItem("--Select--", ""));

            ddlOthSpoken.DataSource = dt;
            ddlOthSpoken.DataBind();
            ddlOthSpoken.Items.Insert(0, new ListItem("--Select--", ""));

            ddlOthWritten.DataSource = dt;
            ddlOthWritten.DataBind();
            ddlOthWritten.Items.Insert(0, new ListItem("--Select--", ""));

            dt = am.getAllEducationCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving education options.");
                return;
            }

            ddlHighEdu.DataSource = dt;
            ddlHighEdu.DataBind();
            ddlHighEdu.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadStep6Fields()
        {
            DataTable dt = (new Applicant_Management()).getAllGetToKnowChannelCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving get to known channel options.");
                return;
            }

            cbKnowLst.DataSource = dt;
            cbKnowLst.DataBind();
        }

        private void loadStep2Fields()
        {
            Applicant_Management am = new Applicant_Management();

            DataTable dt = am.getNationalityCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving nationality options.");
                return;
            }

            ddlNationality.DataSource = dt;
            ddlNationality.DataBind();
            ddlNationality.Items.Insert(0, new ListItem("--Select--", ""));

            dt = am.getIdentificationTypeCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving identification type options.");
                return;
            }

            ddlIdType.DataSource = dt;
            ddlIdType.DataBind();
            ddlIdType.Items.Insert(0, new ListItem("--Select--", ""));

            dt = am.getRaceCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving race options.");
                return;
            }

            ddlRace.DataSource = dt;
            ddlRace.DataBind();
            ddlRace.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadStep1Fields()
        {
            DataTable dt = (new Applicant_Management()).getSponsorship();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving sponsorship options.");
                return;
            }

            ddlSpon.DataSource = dt;
            ddlSpon.DataBind();
            ddlSpon.Items.Insert(0, new ListItem("--Select--", ""));

            dt = (new Programme_Management()).getProgrammeCategory();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programme categories.");
                return;
            }

            if (dt.Rows.Count == 0)
            {
                lblSysError.Text = "No programme available.";
                panelSysError.Visible = true;
                return;
            }

            ddlProgCat.DataSource = dt;
            ddlProgCat.DataBind();
            ddlProgCat.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void ddlProgCat_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlProgTitle.Items.Clear();
            ddlProgDate.Items.Clear();
            ddlProgDate.Enabled = false;
            ddlProgTitle.Enabled = false;

            if (ddlProgCat.SelectedValue == "") return;

            DataTable dt = (new Programme_Management()).getAvailableProgrammeForReg(ddlProgCat.SelectedValue);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programmes.");
                return;
            }

            ddlProgTitle.DataSource = dt;
            ddlProgTitle.DataBind();
            ddlProgTitle.Items.Insert(0, new ListItem("--Select--", ""));
            ddlProgTitle.Enabled = true;
        }

        protected void ddlProgTitle_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlProgDate.Items.Clear();
            ddlProgDate.Enabled = false;

            if (ddlProgTitle.SelectedValue == "") return;

            DataTable dt = (new Programme_Management()).getAvailableProgrammeDateForReg(int.Parse(ddlProgTitle.SelectedValue));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ddlProgDate.DataSource = dt;
            ddlProgDate.DataBind();
            ddlProgDate.Items.Insert(0, new ListItem("--Select--", ""));
            ddlProgDate.Enabled = true;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            //if user can click on register button means all info already fill, so allow going back
            lnkStep2.Attributes.Remove("disabled");
            lnkStep3.Attributes.Remove("disabled");
            lnkStep4.Attributes.Remove("disabled");
            lnkStep5.Attributes.Remove("disabled");
            lnkStep6.Attributes.Remove("disabled");

            if (!Page.IsValid) return;

            LanguageProficiency[] lang = new LanguageProficiency[3];
            lang[0] = new LanguageProficiency()
            {
                lang = "ENG",
                spoken = (Proficiency)Enum.Parse(typeof(Proficiency), ddlEngSpoken.SelectedValue),
                written = (Proficiency)Enum.Parse(typeof(Proficiency), ddlEngWritten.SelectedValue)
            };
            lang[1] = new LanguageProficiency()
            {
                lang = "CHIN",
                spoken = (Proficiency)Enum.Parse(typeof(Proficiency), ddlChnSpoken.SelectedValue),
                written = (Proficiency)Enum.Parse(typeof(Proficiency), ddlChnWritten.SelectedValue)
            };
            lang[2] = new LanguageProficiency()
            {
                lang = "OTH",
                spoken = (Proficiency)Enum.Parse(typeof(Proficiency), ddlOthSpoken.SelectedValue),
                written = (Proficiency)Enum.Parse(typeof(Proficiency), ddlOthWritten.SelectedValue)
            };

            List<EmploymentRecord> empl = new List<EmploymentRecord>();
            if (cbCurrEmpl.Checked)
            {
                empl.Add(new EmploymentRecord()
                {
                    companyName = tbCurrCoName.Text,
                    dept = tbCurrEmplDept.Text,
                    designation = tbCurrEmplDesignation.Text,
                    status = (EmplStatus)Enum.Parse(typeof(EmplStatus), ddlCurrEmplStatus.SelectedValue),
                    occupationType = (EmplOccupation)Enum.Parse(typeof(EmplOccupation), ddlCurrEmplOccupation.SelectedValue),
                    salary = decimal.Parse(tbCurrEmplSalary.Text),
                    dtStart = DateTime.ParseExact(tbCurrEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    dtEnd = DateTime.MaxValue
                });
            }
            if (cbPrevEmpl.Checked)
            {
                empl.Add(new EmploymentRecord()
                {
                    companyName = tbPrevCoName.Text,
                    dept = tbPrevEmplDept.Text,
                    designation = tbPrevEmplDesignation.Text,
                    status = (EmplStatus)Enum.Parse(typeof(EmplStatus), ddlPrevEmplStatus.SelectedValue),
                    occupationType = (EmplOccupation)Enum.Parse(typeof(EmplOccupation), ddlPrevEmplOccupation.SelectedValue),
                    salary = decimal.Parse(tbPrevEmplSalary.Text),
                    dtStart = DateTime.ParseExact(tbPrevEmplStartDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    dtEnd = DateTime.ParseExact(tbPrevEmplEndDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture)
                });
            }

            List<GetToKnowChannel> kChannels = new List<GetToKnowChannel>();
            foreach (ListItem cb in cbKnowLst.Items)
            {
                if (cb.Selected) kChannels.Add((GetToKnowChannel)Enum.Parse(typeof(GetToKnowChannel), cb.Value));
            }

            Tuple<bool, string> status=(new Applicant_Management()).registerApplicantFull(int.Parse(ddlProgTitle.SelectedValue), int.Parse(ddlProgDate.SelectedValue), (Sponsorship)Enum.Parse(typeof(Sponsorship), ddlSpon.SelectedValue)
                , tbName.Text, (IDType)int.Parse(ddlIdType.SelectedValue), tbId.Text, ddlNationality.SelectedValue, ddlRace.SelectedValue, ddlGender.SelectedValue
                , DateTime.ParseExact(tbDOB.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), tbEmail.Text, tbContact1.Text, tbContact2.Text=="" ? null : tbContact2.Text
                , tbAddr.Text, tbPostalCode.Text, lang, ddlHighEdu.SelectedValue, empl.ToArray(), kChannels.ToArray(), KIOSK_ID);

            if (status.Item1)
            {
                //TODO: redirect to success page
                ViewState[APPID_KEY] = status.Item2;
                Server.Transfer(registration_success.PAGE_NAME);
            }
            else
            {
                lblSysError.Text = status.Item2;
                panelSysError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showEmpl", "showCurrEmpl();showPrevEmpl();", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            //force refresh the page so all fields are reset
            Response.Redirect(registration.PAGE_NAME + "?" + DateTime.Now.Ticks);
        }
    }
}