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
    public partial class soa_view : BasePage
    {
        public const string PAGE_NAME = "soa-view.aspx";

        private const string GV_DATA = "data";

        public soa_view()
            : base(PAGE_NAME, AccessRight_Constance.SOA_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            //cos the search 2 textbox is hidden dyanmically at runtime, so everytime the page load need to recreate again
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSearch2", "showSearch2();", true);

            if (!IsPostBack)
            {
                bool canProcess, canReceive;
                canProcess = checkAccessRights(AccessRight_Constance.SOA_PROCESS);
                canReceive = checkAccessRights(AccessRight_Constance.SOA_RECEIVE);
                if (!canProcess && !canReceive) panelOperations.Visible = false;
                else
                {
                    if (!canProcess) panelProcSOA.Visible = false;
                    if (!canReceive) panelRecevSOA.Visible = false;
                }
            }

            panelError.Visible = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Assessment_Management()).searchSOATrainee(ddlSearch.SelectedValue, txtSearch.Text, txtSearch2.Text,
                new SOAStatus[] { SOAStatus.NYA, SOAStatus.RSOA, SOAStatus.PROC });
            if (dt == null)
            {
                redirectToErrorPg("Error searching trainees.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();
        }

        protected void gvTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrainee.DataSource = ViewState[GV_DATA] as DataTable;
            gvTrainee.PageIndex = e.NewPageIndex;
            gvTrainee.DataBind();
        }

        protected void lkbtnProcSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_process.PAGE_NAME);
        }

        protected void lkbtnRecevSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_receive.PAGE_NAME);
        }
    }
}