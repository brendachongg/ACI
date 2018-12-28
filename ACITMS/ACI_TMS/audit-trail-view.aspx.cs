using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Text;
using ClosedXML.Excel;
using System.IO;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class audit_trail_view : BasePage
    {
        public const string PAGE_NAME = "audit-trail-view.aspx";
        public audit_trail_view()
            : base(PAGE_NAME, AccessRight_Constance.AUDIT_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dt = Application[Global.AUDIT_DATA] as DataTable;
                if (dt == null)
                {
                    redirectToErrorPg("Error retrieving audit configuration.");
                    return;
                }

                DataDDL.DataSource = (new DataView(dt)).ToTable(true, new string[] { "groupName"});
                DataDDL.DataBind();
                DataDDL.Items.Insert(0, new ListItem("--select--", ""));
            }
        }


        //clearing all fields in the form
        protected void btnClear_Click(object sender, EventArgs e)
        {
            DataDDL.SelectedIndex = 0;
            actionDDL.SelectedIndex = 0;
            txtStartDate.Text = "";
            txtEndDate.Text = "";
            gvPreview.Visible = false;
            btnExport.Visible = false;
        }

        protected void btnPreview_Click(object sender, EventArgs e)
        {
            string[] tables = getTableNames();
            if (tables == null) return;

            DataTable dtPreview = (new AuditTrail_Management()).previewAuditTrail(tables,
                (txtStartDate.Text == "" ? DateTime.MinValue : DateTime.ParseExact(txtStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture)),
                (txtEndDate.Text == "" ? DateTime.MaxValue : DateTime.ParseExact(txtEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture)),
                actionDDL.SelectedValue);
            if (dtPreview == null)
            {
                redirectToErrorPg("Error retrieving log records.");
                return;
            }

            gvPreview.Visible = true;
            gvPreview.DataSource = dtPreview;
            gvPreview.DataBind();

            btnExport.Visible = true;
        }

        private string[] getTableNames()
        {
            DataTable dt = Application[Global.AUDIT_DATA] as DataTable;
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving audit configuration.");
                return null;
            }

            List<string> tables = new List<string>();
            foreach (DataRow dr in dt.Select(Global.GRP_NAME_COLUMN + "='" + DataDDL.SelectedValue + "'"))
                tables.Add(dr[Global.TBL_NAME_COLUMN].ToString());

            return tables.ToArray();
        }

        protected void btnExport_Click(object sender, EventArgs e)
        {
            string dtQuery = "";
            if (txtStartDate.Text != "") dtQuery += "&" + audit_trail_export.STARTDATE_QUERY + "=" + DateTime.ParseExact(txtStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");
            if (txtEndDate.Text != "") dtQuery += "&" + audit_trail_export.ENDDATE_QUERY + "=" + DateTime.ParseExact(txtEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).ToString("yyyyMMdd");

            Page.ClientScript.RegisterStartupScript(this.GetType(), "exportLog", "window.open('" + audit_trail_export.PAGE_NAME + "?" + audit_trail_export.DATA_QUERY + "=" + HttpUtility.UrlEncode(DataDDL.SelectedValue)
                    + "&" + audit_trail_export.ACTION_QUERY + "=" + HttpUtility.UrlEncode(actionDDL.SelectedValue) + dtQuery + "', '_blank', 'menubar=no,location=no');", true);
        }

    }
}