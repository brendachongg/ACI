using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class programme_creation : BasePage
    {
        public const string PAGE_NAME = "programme-creation.aspx";

        private const string DATA_KEY = "dtProgramme";
        private Programme_Management pm = new Programme_Management();

        public programme_creation()
            : base(PAGE_NAME, AccessRight_Constance.PROGRAM_NEW, programme_management.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                populateDropDownList();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }

            bundlesearch.selectBundle += new bundle_search.SelectBundle(selectBundle);
        }

        private void selectBundle(int bundleId, string bundleCode)
        {
            txtBundle.Text = bundleCode;
            hfBundleId.Value = bundleId.ToString();

            Bundle_Management pm = new Bundle_Management();
            DataTable dt = pm.getBundleModule(bundleId);

            if (dt == null)
            {
                //redirect to error page
                redirectToErrorPg("Error retrieving modules.");
                return;
            }

            gvModules.DataSource = dt;
            gvModules.DataBind();
            gvModules.Visible = true;
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        private void populateDropDownList()
        {
            DataTable dtProgrammeLevel = pm.getProgrammeLevel();
            DataTable dtProgrammeCategory = pm.getProgrammeCategory();
            DataTable dtProgrammeType = pm.getProgrammeType();

            //Programme Type
            ddlProgrammeType.DataSource = dtProgrammeType;
            ddlProgrammeType.DataBind();

            //Programme Level
            ddlProgrammeLevel.DataSource = dtProgrammeLevel;
            ddlProgrammeLevel.DataBind();
            //ddlProgrammeLevel.Items.Insert(0, new ListItem("Not Applicable", "NA"));

            //Programme Category
            ddlProgrammeCategory.DataSource = dtProgrammeCategory;
            ddlProgrammeCategory.DataBind();
        }

        protected void validateProgrammeImage(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            int maxImgSize = 2097152;
            string imageName = fileUploadProgrammeImage.FileName;
            string imgExt = imageName.Substring(imageName.LastIndexOf('.') + 1).ToUpper();

            string[] acceptedImgExt = new string[3];
            acceptedImgExt[0] = ".JPG";
            acceptedImgExt[1] = ".JPEG";
            acceptedImgExt[2] = ".PNG";

            foreach (string ext in acceptedImgExt)
            {
                if (imgExt == ext)
                {
                    args.IsValid = true;
                }
                else
                {
                    args.IsValid = false;
                }
            }

            if (fileUploadProgrammeImage.FileContent.Length < (maxImgSize * 1024))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbProgrammeCode.Text = "";
            ddlProgrammeLevel.SelectedIndex = 0;
            ddlProgrammeType.SelectedIndex = 0;
            tbCourseCode.Text = "";
            tbProgrammeTitle.Text = "";
            tbProgrammeVersion.Text = "1";
            tbNumofSOA.Text = "";
            tbSSGRefNum.Text = "";
            txtBundle.Text = "";
            tbProgrammeDescription.Text = "";
            ddlProgrammeCategory.SelectedIndex = 0;
            gvModules.Visible = false;
            //fileUploadProgrammeImage.Attributes.Clear();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            byte[] imgByte = null;
            Stream s = fileUploadProgrammeImage.PostedFile.InputStream;
            BinaryReader reader = new BinaryReader(s);
            imgByte = reader.ReadBytes((int)s.Length);

            string programmeCode = tbProgrammeCode.Text;
            string programmeLevel = ddlProgrammeLevel.SelectedValue;
            string programmeType = ddlProgrammeType.SelectedValue;
            string courseCode = tbCourseCode.Text;
            string programmeTitle = tbProgrammeTitle.Text;
            int programmeVersion = int.Parse(tbProgrammeVersion.Text);
            int numOfSOA = int.Parse(tbNumofSOA.Text);
            string SSGRefNum = tbSSGRefNum.Text;
            int bundleId = int.Parse(hfBundleId.Value);
            string programmeDescription = tbProgrammeDescription.Text;
            string programmeCategory = ddlProgrammeCategory.SelectedValue;

            Tuple<bool, string> success = pm.createProgramme(programmeCode, courseCode, programmeVersion, programmeLevel, programmeCategory, programmeTitle,
            programmeDescription, numOfSOA, SSGRefNum, bundleId, programmeType, imgByte, LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
                btnClear_Click(null, null);
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);

        }

        protected void CustomValidator1_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (gvModules.Rows.Count >= Convert.ToInt32(tbNumofSOA.Text))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }

        }

    }
}