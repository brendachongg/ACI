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
    public partial class aci_user_new : BasePage
    {

        public const string PAGE_NAME = "aci-user-new.aspx";
        ACI_Staff_User aci_staff = new ACI_Staff_User();
        public aci_user_new()
            : base(PAGE_NAME, AccessRight_Constance.ACI_USER_NEW, aci_user_list.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

                if (!checkAccessRights(AccessRight_Constance.ACI_USER_NEW))
                {
                    redirectToErrorPg("You have not access right to this module");
                }

                ViewState["AcccessToTMS"] = false;
                ViewState["isAccessor"] = false;
                ViewState["isTrainer"] = false;
                ViewState["isInterviewer"] = false;

                loadEmploymentType();
                loadSalutations();


            }
        }

        private void loadEmploymentType()
        {
            DataTable dtEmpType = aci_staff.getCodeReferenceValues(General_Constance.USREMP);
            ViewState["dtEmpType"] = dtEmpType;

            ddlEmpType.DataSource = dtEmpType;
            ddlEmpType.DataTextField = "codeValueDisplay";
            ddlEmpType.DataValueField = "codeValue";
            ddlEmpType.DataBind();
            ddlEmpType.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadSalutations()
        {
            DataTable dtSalutations = aci_staff.getCodeReferenceValues(General_Constance.SAL);
            ViewState["dtSalutations"] = dtSalutations;

            ddlSalutations.DataSource = dtSalutations;
            ddlSalutations.DataTextField = "codeValueDisplay";
            ddlSalutations.DataValueField = "codeValue";
            ddlSalutations.DataBind();
            ddlSalutations.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {

            string name = tbFullName.Text.Trim();
            string email = tbEmail.Text.Trim();
            string idNum = tbIdentificationNo.Text.Trim();
            int salutation = int.Parse(ddlSalutations.SelectedValue);
            bool canLogin = (bool)ViewState["AcccessToTMS"];
            bool isTrainer = (bool)ViewState["isTrainer"];
            bool isAccessor = (bool)ViewState["isAccessor"];
            bool isInterviewer = (bool)ViewState["isInterviewer"];
            string empType = ddlEmpType.SelectedValue;
            string contact1 = tbContactNumber1.Text.Trim();
            string contact2 = tbContactNumber2.Text.Trim();
            string address = tbAddress.Text.Trim();
            string postalCode = tbPostalCode.Text.Trim();

            string wsLoginId = "";

            string password = "";

            Password_Handler ph = new Password_Handler();

            password = ph.GeneratePassword(8, true, true, true, true);

            if (ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.FT.ToString() || ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.ADJ.ToString())
            {
                wsLoginId = tbLoginId.Text;
            }

            int userId = LoginID;

            Tuple<bool, string> status = aci_staff.addACIStaff(name, email, password, idNum, salutation, canLogin, isTrainer, isAccessor, isInterviewer,
            empType, contact1, contact2, address, postalCode, userId, wsLoginId);

            if (status.Item1)
            {
                panelError.Visible = false;
                panelSuccess.Visible = true;
                lblSuccess.Text = status.Item2;

                ddlSalutations.SelectedIndex = 0;
                tbFullName.Text = "";
                tbIdentificationNo.Text = "";
                tbEmail.Text = "";
                ddlEmpType.SelectedIndex = 0;
                tbLoginId.Text = "";
                cbTMS.Checked = false;
                cbAccessor.Checked = false;
                cbTrainer.Checked = false;
                cbInterviewer.Checked = false;
                tbContactNumber1.Text = "";
                tbContactNumber2.Text = "";
                tbAddress.Text = "";
                tbPostalCode.Text = "";
                tbLoginId.Visible = false;
            }
            else
            {
                panelError.Visible = true;
                panelSuccess.Visible = false;
                lblError.Text = status.Item2;
            }
        }

        protected void cbTMS_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTMS.Checked)
            {
                ViewState["AcccessToTMS"] = true;
            }
            else
            {
                ViewState["AcccessToTMS"] = false;
            }
        }

        protected void cbAccessor_CheckedChanged(object sender, EventArgs e)
        {
            if (cbAccessor.Checked)
            {
                ViewState["isAccessor"] = true;
            }
            else
            {
                ViewState["isAccessor"] = false;
            }
        }

        protected void cbTrainer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbTrainer.Checked)
            {
                ViewState["isTrainer"] = true;
            }
            else
            {
                ViewState["isTrainer"] = false;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbFullName.Text = "";
            tbAddress.Text = "";
            tbContactNumber1.Text = "";
            tbContactNumber2.Text = "";
            tbEmail.Text = "";
            tbIdentificationNo.Text = "";
            tbPostalCode.Text = "";
            ddlEmpType.SelectedValue = "";
            ddlSalutations.SelectedValue = "";
            cbAccessor.Checked = false;
            cbTMS.Checked = false;
            cbTrainer.Checked = false;
            ViewState["isTrainer"] = false;
            ViewState["isAccessor"] = false;
            ViewState["AcccessToTMS"] = false;
            ViewState["isInterviewer"] = false;
            panelError.Visible = false;
            panelSuccess.Visible = false;
        }

        protected void cbInterviewer_CheckedChanged(object sender, EventArgs e)
        {
            if (cbInterviewer.Checked)
            {
                ViewState["isInterviewer"] = true;
            }
            else
            {
                ViewState["isInterviewer"] = false;
            }
        }

        protected void ddlEmpType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.FT.ToString() || ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.ADJ.ToString())
            {
                tbLoginId.Visible = true;
                lblLoginId.Visible = true;
            }
            else
            {
                tbLoginId.Visible = false;
                lblLoginId.Visible = false;
            }
        }
    }
}