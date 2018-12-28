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
    public partial class session_management : BasePage
    {
        public const string PAGE_NAME = "session-management.aspx";

        private const string GV_DATA = "SESSION";

        public session_management()
            : base(PAGE_NAME, AccessRight_Constance.SESSION_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadProgCategory();
                loadProgLevel();
                hfSelSearchType.Value = "BASIC";
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showBasicSearch", "showBasicSearch();", true);
            }
            else panelError.Visible = false;

            modulesearch.selectModule += new module_search.SelectModule(selectModule);
        }

        private void loadProgCategory()
        {
            DataTable dt = (new Programme_Management()).getProgrammeCategory();

            ddlProgrammeCategory.DataSource = dt;
            ddlProgrammeCategory.DataBind();
            ddlProgrammeCategory.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void loadProgLevel()
        {
            DataTable dt = (new Programme_Management()).getProgrammeLevel();

            ddlProgrammeLevel.DataSource = dt;
            ddlProgrammeLevel.DataBind();
            ddlProgrammeLevel.Items.Insert(0, new ListItem("--Select--", ""));
        }

        private void selectModule(int moduleId, string title)
        {
            hfModuleId.Value = moduleId.ToString();
            tbModule.Text = title;

            hfSelSearchType.Value = "ADV";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAdvSearch", "showAdvSearch();", true);
        }

        protected void gvSession_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSession.DataSource = ViewState[GV_DATA] as DataTable;
            gvSession.PageIndex = e.NewPageIndex;
            gvSession.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSearch", hfSelSearchType.Value == "BASIC" ? "showBasicSearch();" : "showAdvSearch();", true);
        }

        protected void ddlProgrammeCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateProgramme();
        }

        protected void ddlProgrammeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            populateProgramme();
        }

        private void populateProgramme()
        {
            ddlProgrammeTitle.Items.Clear();

            hfSelSearchType.Value = "ADV";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAdvSearch", "showAdvSearch();", true);

            if (ddlProgrammeCategory.SelectedValue == "" && ddlProgrammeLevel.SelectedValue == "") return;

            DataTable dt = (new Programme_Management()).getAvaProgrammeTitle(ddlProgrammeCategory.SelectedValue, ddlProgrammeLevel.SelectedValue);

            ddlProgrammeTitle.DataSource = dt;
            ddlProgrammeTitle.DataBind();
            ddlProgrammeTitle.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void btnAdvSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Batch_Session_Management()).searchSessions(ddlProgrammeCategory.SelectedValue, ddlProgrammeLevel.SelectedValue, tbBatchCode.Text, ddlProgrammeTitle.SelectedValue,
                hfModuleId.Value == "" ? 0 : int.Parse(hfModuleId.Value), null, null);
            
            ViewState[GV_DATA] = dt;
            gvSession.DataSource = dt;
            gvSession.DataBind();

            hfSelSearchType.Value = "ADV";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAdvSearch", "showAdvSearch();", true);
        }

        protected void btnAdvClear_Click(object sender, EventArgs e)
        {
            ddlProgrammeCategory.SelectedIndex = 0;
            ddlProgrammeLevel.SelectedIndex = 0;
            ddlProgrammeTitle.Items.Clear();
            tbBatchCode.Text = "";
            tbModule.Text = "";
            hfModuleId.Value = "";

            hfSelSearchType.Value = "ADV";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showAdvSearch", "showAdvSearch();", true);
        }

        protected void btnBasicSearch_Click(object sender, EventArgs e)
        {
            string clsCode = null, progCode = null, pjCode = null;

            if (ddlBasicSearchType.SelectedValue == "BC") clsCode = tbBasicSearch.Text;
            else if (ddlBasicSearchType.SelectedValue == "PC") progCode = tbBasicSearch.Text;
            else if (ddlBasicSearchType.SelectedValue == "PJC") pjCode = tbBasicSearch.Text;
            else return;

            DataTable dt = (new Batch_Session_Management()).searchSessions(null, null, clsCode, null, 0, progCode, pjCode);

            ViewState[GV_DATA] = dt;
            gvSession.DataSource = dt;
            gvSession.DataBind();

            hfSelSearchType.Value = "BASIC";
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showBasicSearch", "showBasicSearch();", true);
        }

        protected void gvSession_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "selectSession")
            {
                Server.Transfer(session_view.PAGE_NAME + "?" + session_view.QUERY_ID + "=" + HttpUtility.UrlEncode(e.CommandArgument.ToString()));
            }
        }


    }
}