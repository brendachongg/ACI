using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using LogicLayer;

namespace ACI_TMS
{
    public partial class programme_search : System.Web.UI.UserControl
    {
        private const string DATA_KEY = "dtProg";

        public delegate void SelectProgramme(int id, string code, string title);
        public event SelectProgramme selectProgramme;

        public delegate void ProgrammeRefresh();
        public event ProgrammeRefresh programmeRefresh;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbtnSearchProg_Click(object sender, EventArgs e)
        {
            string type = null;
            if (rbCode.Checked) type = "CODE";
            else if (rbTitle.Checked) type = "TITLE";
            else if (rbCseCode.Checked) type = "CSECODE";

            DataTable dt = (new Programme_Management()).searchProgramme(type, tbSearchProg.Text);
            if (dt == null)
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving programmes.");
                return;
            }

            ViewState[DATA_KEY] = dt;
            gvProg.DataSource = dt;
            gvProg.Columns[0].Visible = true;
            gvProg.DataBind();
            gvProg.Columns[0].Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showProgDialog();", true);
        }

        protected void gvProg_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectProg"))
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[DATA_KEY] as DataTable;

                selectProgramme((int)dt.Rows[index]["programmeId"], dt.Rows[index]["programmeCode"].ToString(), dt.Rows[index]["programmeTitle"].ToString());
                if (programmeRefresh != null) programmeRefresh();
            }
        }

        protected void gvProg_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProg.DataSource = ViewState[DATA_KEY] as DataTable;
            gvProg.PageIndex = e.NewPageIndex;
            gvProg.Columns[0].Visible = true;
            gvProg.DataBind();
            gvProg.Columns[0].Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showProgDialog();", true);
            if (programmeRefresh != null) programmeRefresh();
        }
    }
}