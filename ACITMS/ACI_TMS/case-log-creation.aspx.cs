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
using System.Globalization;

namespace ACI_TMS
{
    public partial class case_log_creation : BasePage
    {
        public const string PAGE_NAME = "case-log-creation.aspx";
        private CaseLog_Management clm = new CaseLog_Management();

        public case_log_creation()
            : base(PAGE_NAME, AccessRight_Constance.CASELOG_NEW, case_log_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                populateTime();
                loadCategory();
            }
        }

        private void populateTime()
        {
            for (int i = 0; i <= 23; i++)
            {
                ddlIncidentHour.Items.Add(i.ToString("D2"));

            }

            for (int i = 0; i <= 59; i++)
            {
                ddlIncidentMinutes.Items.Add(i.ToString("D2"));
            }
        }

        private void loadCategory()
        {
            DataTable dt = clm.getCaseLogCategory();

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving case logs.");
                return;
            }
            else if (dt.Rows.Count > 0)
            {
                ddlCaseLogCategory.DataSource = dt;
                ddlCaseLogCategory.DataTextField = "codeValueDisplay";
                ddlCaseLogCategory.DataValueField = "codeValue";
                ddlCaseLogCategory.DataBind();
                ddlCaseLogCategory.Items.Insert(0, new ListItem("--Select--", ""));
                ddlCaseLogCategory.SelectedIndex = 0;
            }

        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            
            byte[] fileByte = null;
            string filePath = null, attachmentName = null, ext = null, attachmentType = null;
            if (fileUploadCaseLogAttachment.HasFile)
            {
                Stream s = fileUploadCaseLogAttachment.PostedFile.InputStream;
                BinaryReader reader = new BinaryReader(s);
                fileByte = reader.ReadBytes((int)s.Length);
            

                // Read the file and convert it to Byte Array
                filePath = fileUploadCaseLogAttachment.PostedFile.FileName;
                attachmentName = Path.GetFileName(filePath);
                ext = Path.GetExtension(attachmentName);

                //Set the attachmentType based on File Extension
                switch (ext.ToUpper())
                {
                    case ".JPG":
                        attachmentType = "image/jpg";
                        break;
                    case ".JPEG":
                        attachmentType = "image/jpeg";
                        break;
                    case ".PNG":
                        attachmentType = "image/png";
                        break;
                    case ".PDF":
                        attachmentType = "application/pdf";
                        break;
                    case ".DOC":
                    case ".DOCX":
                        attachmentType = "application/vnd.ms-word";
                        break;
                    case ".XLS":
                    case ".XLSX":
                    case ".CSV":
                        attachmentType = "application/vnd.ms-excel";
                        break;
                }
            }

            DateTime incidentDateTime = DateTime.ParseExact((tbIncidentDate.Text + " " + ddlIncidentHour.SelectedValue + ":" + ddlIncidentMinutes.SelectedValue), "dd MMM yyyy HH:mm", System.Globalization.CultureInfo.InvariantCulture);

            Tuple<bool, string> success = clm.createCaseLog((CaseLogCategory)Enum.Parse(typeof(CaseLogCategory), ddlCaseLogCategory.SelectedValue), incidentDateTime, tbSubject.Text, tbCaseLogMessageValue.Text, fileByte, attachmentName, attachmentType, LoginID);

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

        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlCaseLogCategory.SelectedIndex = 0;
            tbSubject.Text = "";
            tbIncidentDate.Text = "";
            ddlIncidentHour.SelectedIndex = 0;
            ddlIncidentMinutes.SelectedIndex = 0;
            tbCaseLogMessageValue.Text = "";
        }


    }
}