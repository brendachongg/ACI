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
    public partial class case_log_view_attachment : BasePage
    {
        public const string PAGE_NAME = "case-log-view-attachment.aspx";

        public const string CASELOG_QUERY = "caseLogId";
        public const string FOLLOWUP_QUERY = "followUpId";
        public const string TYPE_QUERY = "type";

        private CaseLog_Management clm = new CaseLog_Management();

        public case_log_view_attachment()
            : base(PAGE_NAME, AccessRight_Constance.CASELOG_VIEW, case_log_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if ((Request.QueryString[CASELOG_QUERY] == "") && (Request.QueryString[FOLLOWUP_QUERY] == ""))
                {
                    redirectToErrorPg("Missing case log/followup information.");
                    return;
                }

                if (HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == "CASELOG")
                {
                    loadCaseLogAttachment(HttpUtility.UrlDecode(Request.QueryString[CASELOG_QUERY]));
                }
                else if (HttpUtility.UrlDecode(Request.QueryString[TYPE_QUERY]) == "FOLLOWUP")
                {
                    loadFollowUpAttachment(int.Parse(HttpUtility.UrlDecode(Request.QueryString[FOLLOWUP_QUERY])));
                }
                else
                {
                    redirectToErrorPg("Missing attachment information.");
                    return;
                }
            }
        }

        private void showAttachment(DataRow dr)
        {
            if (dr["attachment"] != DBNull.Value && dr["attachmentName"] != DBNull.Value && (dr["attachmentType"].ToString() == "image/jpg" || dr["attachmentType"].ToString() == "image/jpeg" || dr["attachmentType"].ToString() == "image/png"))
            {
                MemoryStream ms = new MemoryStream((byte[])dr["attachment"]);
                BinaryReader br = new BinaryReader(ms);
                byte[] bytes = br.ReadBytes((Int32)ms.Length);

                if (bytes.Length > 0)
                {
                    string imgString = Convert.ToBase64String(bytes, 0, bytes.Length);
                    lbtnAttachment.ImageUrl = "data:image/jpeg;base64," + imgString;
                }
            }
            else
            {
                string attachmentName = dr["attachmentName"].ToString();
                byte[] fileContent = (byte[])(dr["attachment"]);
                string contentType = dr["attachmentType"].ToString();

                Response.ClearContent();
                Response.Buffer = true;
                Response.Charset = "";
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.ContentType = contentType;
                Response.AddHeader("Content-Disposition", "attachment; fileName=" + attachmentName);
                Response.AddHeader("Content-Length", fileContent.Length.ToString());
                Response.BinaryWrite(fileContent);

                Response.Flush();
                Response.End();
            }
        }

        private void loadCaseLogAttachment(string caseLogId)
        {
            DataTable dtCaseLogImg = clm.getCaseLogDetails(caseLogId);
            if (dtCaseLogImg == null)
            {
                redirectToErrorPg("Error retrieving case log details.");
                return;
            }

            DataRow dr = dtCaseLogImg.Rows[0];
            showAttachment(dr);

        }

        private void loadFollowUpAttachment(int followUpId)
        {
            DataTable dtFollowUpImg = clm.getFollowUpAttachment(followUpId);
            if (dtFollowUpImg == null)
            {
                redirectToErrorPg("Error retrieving follow up details.");
                return;
            }

            DataRow dr = dtFollowUpImg.Rows[0];
            showAttachment(dr);

        }
    }



}