using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace ACI_TMS
{
    public partial class case_log_management : BasePage
    {
        public const string PAGE_NAME = "case-log-management.aspx";
        private CaseLog_Management clm = new CaseLog_Management();
        private const string GV_DATA = "CASELOG";

        public case_log_management()
            : base(PAGE_NAME, AccessRight_Constance.CASELOG_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadCategory();
                ddlCaseLogCategory.SelectedIndex = 0;
                ddlSearchCaseLog.SelectedIndex = 0;
                hdfStatus.Value = CaseLogStatus.NEW.ToString();
                searchCaseLog();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        //Search Case Log
        private void searchCaseLog()
        {
            DataTable dt = clm.searchCaseLog((CaseLogStatus)Enum.Parse(typeof(CaseLogStatus), hdfStatus.Value), ddlCaseLogCategory.SelectedValue, ddlSearchCaseLog.SelectedValue, tbSearchCaseLog.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving case logs.");
                return;
            }

            ViewState[GV_DATA] = dt;

            if (!checkAccessRights(AccessRight_Constance.CASELOG_DEL))
            {
                btnConfirmDel.Visible = false;
                gvCaseLog.Columns[0].Visible = false;
            }
            else
            {
                btnConfirmDel.Visible = true;
                gvCaseLog.Columns[0].Visible = true;
            }

            gvCaseLog.DataSource = dt;
            gvCaseLog.DataBind();           
        }

        //Loading of the category
        private void loadCategory()
        {
            DataTable dt = clm.getCaseLogCategory();

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving case log categories.");
                return;
            }
            else if (dt.Rows.Count > 0)
            {
                ddlCaseLogCategory.DataSource = dt;
                ddlCaseLogCategory.DataBind();
            }

        }

        protected void ddlCaseLogCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            searchCaseLog();
        }


        protected void lkbtnCreateCaseLog_Click(object sender, EventArgs e)
        {
            Response.Redirect(case_log_creation.PAGE_NAME);
        }

        protected void btnUnattendedLogs_ServerClick(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.UA.ToString();
            searchCaseLog();
            btnUnattendedLogs.Attributes.Add("class", "btn btn-default btn-info");
            btnNewLogs.Attributes.Add("class", "btn btn-default");
            btnResolvingLogs.Attributes.Add("class", "btn btn-default");
            btnClosedLogs.Attributes.Add("class", "btn btn-default");
        }

        protected void btnNewLogs_ServerClick(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.NEW.ToString();
            searchCaseLog();
            btnUnattendedLogs.Attributes.Add("class", "btn btn-default");
            btnNewLogs.Attributes.Add("class", "btn btn-default btn-info");
            btnResolvingLogs.Attributes.Add("class", "btn btn-default");
            btnClosedLogs.Attributes.Add("class", "btn btn-default");

        }

        protected void btnResolvingLogs_ServerClick(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.RS.ToString();
            searchCaseLog();
            btnUnattendedLogs.Attributes.Add("class", "btn btn-default");
            btnNewLogs.Attributes.Add("class", "btn btn-default");
            btnResolvingLogs.Attributes.Add("class", "btn btn-default  btn-info");
            btnClosedLogs.Attributes.Add("class", "btn btn-default");

        }


        protected void btnClosedLogs_ServerClick(object sender, EventArgs e)
        {
            hdfStatus.Value = CaseLogStatus.C.ToString();
            searchCaseLog(); 
            btnUnattendedLogs.Attributes.Add("class", "btn btn-default");
            btnNewLogs.Attributes.Add("class", "btn btn-default");
            btnResolvingLogs.Attributes.Add("class", "btn btn-default");
            btnClosedLogs.Attributes.Add("class", "btn btn-default  btn-info");
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearchCaseLog.SelectedIndex = 0;
            tbSearchCaseLog.Text = "";
            searchCaseLog();
        }

        protected void btnSearchCaseLog_Click(object sender, EventArgs e)
        {
            searchCaseLog();
            ddlSearchCaseLog.SelectedIndex = 0;
            tbSearchCaseLog.Text = "";
        }

        protected void gvCaseLog_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewCaseLogDetails"))
            {
                Response.Redirect(case_log_view.PAGE_NAME + "?" + case_log_view.CASELOG_QUERY + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void gvCaseLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCaseLog.DataSource = ViewState[GV_DATA] as DataTable;
            gvCaseLog.PageIndex = e.NewPageIndex;
            gvCaseLog.DataBind();
        }

        protected void btnDel_Click(object sender, EventArgs e)
        {
            List<string> lstCse = new List<string>();
            foreach (GridViewRow gvr in gvCaseLog.Rows)
            {
                if (((CheckBox)gvr.Cells[0].FindControl("cb")).Checked) lstCse.Add(((Label)gvr.Cells[1].FindControl("lbCaseLogId")).Text);
            }

            if (lstCse.Count == 0)
            {
                lblError.Text = "No case log is selected.";
                panelError.Visible = true;
                return;
            }

            Tuple<bool, string> status = clm.deleteCaseLogs(lstCse.ToArray(), LoginID);
            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                searchCaseLog();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

    }
}