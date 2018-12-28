using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class case_log_view : BasePage
    {
        public const string PAGE_NAME = "case-log-view.aspx";
        public const string CASELOG_QUERY = "caseLogId";

        private const string GV_DATA = "CASELOGFOLLOWUP";
        private const string CAN_EDIT_CASELOG = "canEdit";

        private CaseLog_Management clm = new CaseLog_Management();

        public case_log_view()
            : base(PAGE_NAME, AccessRight_Constance.CASELOG_VIEW, case_log_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (HttpUtility.UrlDecode(Request.QueryString[CASELOG_QUERY]) == "")
                {
                    redirectToErrorPg("Missing Case Log Information.");
                    return;
                }

                hdfCaseLogId.Value = Request.QueryString[CASELOG_QUERY];

                loadCaseLogDetails();
                loadFollowUpRecords();

                bool canEdit = checkAccessRights(AccessRight_Constance.CASELOG_EDIT);
                if (!canEdit)
                {
                    btnReply.Visible = false;
                    btnConfirmDel.Visible = false;
                    btnReopen.Visible = false;
                }
                else
                {
                    string status = hdfStatus.Value;
                    if (status == CaseLogStatus.C.ToString())
                    {
                        btnReply.Visible = false;
                        btnConfirmDel.Visible = false;
                        btnReopen.Visible = true;
                    }
                    else
                    {
                        btnReply.Visible = true;
                        btnConfirmDel.Visible = true;
                        btnReopen.Visible = false;
                    }

                }

            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadCaseLogDetails()
        {
            DataTable dtCaseLogDetails = clm.getCaseLogDetails(hdfCaseLogId.Value);
            if (dtCaseLogDetails == null || dtCaseLogDetails.Rows.Count == 0)
            {
                redirectToErrorPg("Missing Case Log Information.");
                return;
            }
            DataRow dr = dtCaseLogDetails.Rows[0];
            hdfCaseLogId.Value = dr["caseLogId"].ToString();
            lbCaseLogCategoryValue.Text = dr["LGCAT"].ToString();
            lbCaseLogSubjectValue.Text = dr["subject"].ToString();
            lbCaseLogIncidentDateValue.Text = dr["dateOfIncident"].ToString();
            lbCaseLogIncidentTimeValue.Text = dr["incidentTime"].ToString();
            lbReportedByValue.Text = dr["userName"].ToString();
            tbCaseLogMessageValue.Text = dr["message"].ToString();
            lbCaseLogStatusValue.Text = dr["LGSTAT"].ToString();
            hdfStatus.Value = dr["status"].ToString();

            if (dr["attachment"] != DBNull.Value && dr["attachmentName"] != DBNull.Value && dr["attachmentType"] != DBNull.Value)
            {
                lbtnAttachment.Visible = true;
                lbCaseLogAttachmentEmpty.Visible = false;
                lbtnAttachment.Attributes["onClick"] = "showAttachment.Start('" + dr["caseLogId"].ToString() + "', '" + "CASELOG" + "'," + "this" + ");";
            }
            else
            {
                lbtnAttachment.Visible = false;
                lbCaseLogAttachmentEmpty.Visible = true;
            }
        }

        private void loadFollowUpRecords()
        {
            DataTable dt = clm.getFollowUpByCaseLogId(hdfCaseLogId.Value);

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving case log follow up records.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvFollowUp.DataSource = dt;
            gvFollowUp.DataBind();
        }

        protected void btnConfirmReply_Click(object sender, EventArgs e)
        {
            // Read the file and convert it to Byte Array
            string filePath = null, fileName = null, ext = null, attachmentType = null;
            byte[] fileByte = null;

            if (fileUploadFollowUpAttachment.HasFile)
            {             
                Stream s = fileUploadFollowUpAttachment.PostedFile.InputStream;
                BinaryReader reader = new BinaryReader(s);
                fileByte = reader.ReadBytes((int)s.Length);

                filePath = fileUploadFollowUpAttachment.PostedFile.FileName;
                fileName = Path.GetFileName(filePath);
                ext = Path.GetExtension(fileName);
                attachmentType = String.Empty;

                //Set the contenttype based on File Extension
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
            //Create New Follow Up
            Tuple<bool, string> success = clm.createFollowUp(hdfCaseLogId.Value, tbReplyBoxValue.Text, fileByte, fileName, attachmentType, LoginID);

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
            loadFollowUpRecords();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbReplyBoxValue.Text = "";
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.C.ToString();

            //Update Method
            Tuple<bool, string> success = clm.updateCaseLog(hdfCaseLogId.Value, (CaseLogStatus)Enum.Parse(typeof(CaseLogStatus), hdfStatus.Value), LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
                btnClear_Click(null, null);
                btnConfirmDel.Visible = false;
                btnReply.Visible = false;
                btnReopen.Visible = true;
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }

        protected void btnReopenCase_Click(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.RS.ToString();
            //Update Method
            Tuple<bool, string> success = clm.updateCaseLog(hdfCaseLogId.Value, (CaseLogStatus)Enum.Parse(typeof(CaseLogStatus), hdfStatus.Value), LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
                btnClear_Click(null, null);
                btnReopen.Visible = false;
                btnConfirmDel.Visible = true;
                btnReply.Visible = true;
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }


        protected void gvFollowUp_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (((DataRowView)e.Row.DataItem)["followUpId"] != DBNull.Value
                && ((DataRowView)e.Row.DataItem)["attachment"] != DBNull.Value
                && ((DataRowView)e.Row.DataItem)["attachmentName"] != DBNull.Value
                && ((DataRowView)e.Row.DataItem)["attachmentType"] != DBNull.Value)
            {
                ((Label)e.Row.FindControl("lbtnAttachment")).Attributes["onClick"] = "showAttachment.Start('" + ((DataRowView)e.Row.DataItem)["followUpId"].ToString() + "', '" + "FOLLOWUP" + "'," + "this" + ");";
                ((HiddenField)e.Row.FindControl("hdfFollowUpId")).Value = ((DataRowView)e.Row.DataItem)["followUpId"].ToString();
                e.Row.FindControl("lbFollowUpAttachmentEmpty").Visible = false;
            }
            else
            {
                e.Row.FindControl("lbtnAttachment").Visible = false;
                e.Row.FindControl("lbFollowUpAttachmentEmpty").Visible = true;
            }
        }

        protected void gvFollowUp_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvFollowUp.DataSource = ViewState[GV_DATA] as DataTable;
            gvFollowUp.PageIndex = e.NewPageIndex;
            gvFollowUp.DataBind();
        }

    }
}