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
    public partial class applicant_quick_registration : BasePage
    {
        public const string PAGE_NAME = "applicant-quick-registration.aspx";

        public applicant_quick_registration()
            : base(PAGE_NAME, new string[]{AccessRight_Constance.REG_NEW, AccessRight_Constance.REG_NEW_ANY})
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadIdType();
                loadSponsorship();
                loadProgrammeCategory();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadIdType()
        {
            DataTable dt = (new Applicant_Management()).getIdentificationTypeCodeReference();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving identification types.");
                return;
            }

            ddlIdType.DataSource = dt;
            ddlIdType.DataBind();
        }

        private void loadSponsorship()
        {
            DataTable dt = (new Applicant_Management()).getSponsorship();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programme categories.");
                return;
            }

            ddlSponsorship.DataSource = dt;
            ddlSponsorship.DataBind();
            ddlSponsorship.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadProgrammeCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programme categories.");
                return;
            }

            if (dt.Rows.Count == 0)
            {
                lblError.Text = "No programme available.";
                panelError.Visible = true;
                return;
            }

            ddlProgrammeCategory.DataSource = dt;
            ddlProgrammeCategory.DataBind();
            ddlProgrammeCategory.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void ddlProgrammeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlProgramme.Items.Clear();
            ddlStartDate.Items.Clear();
            ddlStartDate.Enabled = false;
            ddlProgramme.Enabled = false;

            if (ddlProgrammeCategory.SelectedValue == "") return;

            DataTable dt = (new Programme_Management()).getAvailableProgrammeForReg(ddlProgrammeCategory.SelectedValue, checkAccessRights(AccessRight_Constance.REG_NEW_ANY));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving programmes.");
                return;
            }

            ddlProgramme.DataSource = dt;
            ddlProgramme.DataBind();
            ddlProgramme.Items.Insert(0, new ListItem("--Select--", ""));
            ddlProgramme.Enabled = true;
        }

        protected void ddlProgramme_SelectedIndexChanged(object sender, EventArgs e)
        {
            ddlStartDate.Items.Clear();
            ddlStartDate.Enabled = false;

            if (ddlProgramme.SelectedValue == "") return;

            DataTable dt = (new Programme_Management()).getAvailableProgrammeDateForReg(int.Parse(ddlProgramme.SelectedValue), checkAccessRights(AccessRight_Constance.REG_NEW_ANY));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving classes.");
                return;
            }

            ddlStartDate.DataSource = dt;
            ddlStartDate.DataBind();
            ddlStartDate.Items.Insert(0, new ListItem("--Select--", ""));
            ddlStartDate.Enabled = true;
        }

        protected void btnRegister_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            Tuple<bool, string> status = (new Applicant_Management()).registerApplicantQuick(tbFullName.Text, DateTime.ParseExact(tbBirthDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                (ddlIdType.SelectedValue == ((int)IDType.Oth).ToString()) ? tbPPIdentification.Text : ddlIdFirstLetter.SelectedValue + tbLocalIdentification.Text + ddlIdLastLetter.Text, 
                (IDType)int.Parse(ddlIdType.SelectedValue), (Sponsorship)Enum.Parse(typeof(Sponsorship), ddlSponsorship.SelectedValue), int.Parse(ddlProgramme.SelectedValue), int.Parse(ddlStartDate.SelectedValue), 
                LoginID);

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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbFullName.Text = "";
            tbBirthDate.Text = "";
            tbLocalIdentification.Text = "";
            tbPPIdentification.Text = "";
            ddlIdType.SelectedIndex = 0;
            panelLocalId.Visible = true;
            panelPP.Visible = false;
            ddlIdFirstLetter.SelectedIndex = 0;
            ddlIdLastLetter.SelectedIndex = 0;
            ddlSponsorship.SelectedIndex = 0;
            ddlProgrammeCategory.SelectedIndex = 0;

            ddlProgrammeCategory_SelectedIndexChanged(null, null);
        }

        protected void ddlIdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            panelLocalId.Visible = false;
            panelPP.Visible = false;

            if (ddlIdType.SelectedValue == ((int)IDType.Oth).ToString())
                panelPP.Visible = true;
            else
                panelLocalId.Visible = true;

        }
    }
}