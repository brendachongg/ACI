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
    public partial class aci_user_view_edit : BasePage
    {
        public const string PAGE_NAME = "aci-user-view-edit.aspx";
        public const string ACI_USER_QUERY = "a";

        ACI_Staff_User aci_staff = new ACI_Staff_User();

        public aci_user_view_edit()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ACI_USER_EDIT, AccessRight_Constance.ACI_USER_VIEW }, aci_user_list.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadEmploymentType();
                loadSalutations();
                string aciStaffId = Request.QueryString[ACI_USER_QUERY];
                loadACIStaff(int.Parse(aciStaffId));
                btnUpdate.Visible = false;
                btnCancel.Visible = false;

                if (checkAccessRights(AccessRight_Constance.ACI_USER_EDIT))
                {
                    lkbtnEdit.Visible = true;
                }
                else
                {
                    lkbtnEdit.Visible = false;
                }

                if (!checkAccessRights(AccessRight_Constance.ACI_USER_VIEW))
                {
                    redirectToErrorPg("You have not access right to this module");
                }
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
                  tbLoginId.Text = "";
              }
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

        private void loadACIStaff(int staffId)
        {
            DataTable dtACIUser = aci_staff.getACIStaff(staffId);

            //select userName, userEmail, idNumber, salutation, isTrainer, userAuthorization, isAssessor, emplType, 
           //                 contactNumber1, contactNumber2, addressLine, postalCode from aci_user where u.userId=@uId";

            tbFullName.Text = dtACIUser.Rows[0]["userName"].ToString();
            tbIdentificationNo.Text = dtACIUser.Rows[0]["idNumber"].ToString();
            tbEmail.Text = dtACIUser.Rows[0]["userEmail"].ToString();
            ddlSalutations.SelectedValue = dtACIUser.Rows[0]["salutation"].ToString();
            ddlEmpType.SelectedValue = dtACIUser.Rows[0]["emplType"].ToString();
            tbContactNumber1.Text = dtACIUser.Rows[0]["contactNumber1"].ToString();
            tbContactNumber2.Text = dtACIUser.Rows[0]["contactNumber2"].ToString();
            tbAddress.Text = dtACIUser.Rows[0]["addressLine"].ToString();
            tbPostalCode.Text = dtACIUser.Rows[0]["postalCode"].ToString();


            if (ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.FT.ToString() || ddlEmpType.SelectedValue == GeneralLayer.StaffEmploymentType.ADJ.ToString())
            {
                tbLoginId.Visible = true;
                lblLoginId.Visible = true;
                tbLoginId.Text = dtACIUser.Rows[0]["loginId"].ToString();
            }

            if (dtACIUser.Rows[0]["isAssessor"].Equals(General_Constance.STATUS_YES))
            {
                cbAccessor.Checked = true;
                ViewState["isAssessor"] = true;
            }
            else
            {
                cbAccessor.Checked = false;
                ViewState["isAssessor"] = false;
            }

            if (dtACIUser.Rows[0]["userAuthorization"].Equals(General_Constance.STATUS_YES))
            {
                cbTMS.Checked = true;
                ViewState["AcccessToTMS"] = true;
            }
            else
            {
                cbTMS.Checked = false;
                ViewState["AcccessToTMS"] = false;
            }
            if (dtACIUser.Rows[0]["isTrainer"].Equals(General_Constance.STATUS_YES))
            {
                cbTrainer.Checked = true;
                ViewState["isTrainer"] = true;
            }
            else
            {
                cbTrainer.Checked = false;
                ViewState["isTrainer"] = false;
            }

            if (dtACIUser.Rows[0]["isInterviewer"].Equals(General_Constance.STATUS_YES))
            {
                cbInterviewer.Checked = true;
                ViewState["isInterviewer"] = true;
            }
            else
            {
                cbInterviewer.Checked = false;
                ViewState["isInterviewer"] = false;
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
                ViewState["isAssessor"] = true;
            }
            else
            {
                ViewState["isAssessor"] = false;
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

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            string name = tbFullName.Text;
            string email = tbEmail.Text;
            string password = "password";
            string idNum = tbIdentificationNo.Text;
            int sal = int.Parse(ddlSalutations.SelectedValue);
            bool canLogin = (bool)ViewState["AcccessToTMS"];
            bool isTrainer = (bool)ViewState["isTrainer"];
            bool isAssessor = (bool)ViewState["isAssessor"];
            bool isInterviewer = (bool)ViewState["isInterviewer"];
            string emType = ddlEmpType.SelectedValue;
            string contact1 = tbContactNumber1.Text;
            string contact2 = tbContactNumber2.Text;
            string address = tbAddress.Text;
            string postalCode = tbPostalCode.Text;
            string wsLoginId = tbLoginId.Text;
            int loginId = LoginID;
            int uId = int.Parse(Request.QueryString[ACI_USER_QUERY]);
            Tuple<Boolean, String> status = aci_staff.updateACIUser(name, email, idNum, sal, canLogin,isTrainer,isAssessor, isInterviewer,  emType,contact1, contact2, address, postalCode, uId, loginId, wsLoginId);

            if (status.Item1)
            {
                panelSuccess.Visible = true;
                panelError.Visible = false;
                lblSuccess.Text = status.Item2;
                btnUpdate.Visible = false;
                btnCancel.Visible = false;
                panelParticular.Enabled = false;
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = true;
                lblErrorMsg.Text = status.Item2;
            }

        }

        protected void lkbtnEdit_Click(object sender, EventArgs e)
        {
            btnUpdate.Visible = true;
            btnCancel.Visible = true;
            panelParticular.Enabled = true;
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

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            btnUpdate.Visible = false;
            btnCancel.Visible = false;
            panelParticular.Enabled = false;
        }
    }
}