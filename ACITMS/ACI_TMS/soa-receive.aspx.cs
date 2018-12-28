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
    public partial class soa_receive : BasePage
    {
        public const string PAGE_NAME = "soa-receive.aspx";

        public soa_receive()
            : base(PAGE_NAME, AccessRight_Constance.SOA_RECEIVE)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            //cos the search 2 textbox is hidden dyanmically at runtime, so everytime the page load need to recreate again
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSearch2", "showSearch2();", true);

            if (!IsPostBack)
            {
                bool canView, canProcess;
                canView = checkAccessRights(AccessRight_Constance.SOA_VIEW);
                canProcess = checkAccessRights(AccessRight_Constance.SOA_PROCESS);
                if (!canView && !canProcess) panelOperations.Visible = false;
                else
                {
                    if (!canView) panelViewSOA.Visible = false;
                    if (!canProcess) panelProcSOA.Visible = false;
                }
            }

            panelError.Visible = false;
            panelSuccess.Visible = false;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Assessment_Management()).searchSOATrainee(ddlSearch.SelectedValue, txtSearch.Text, txtSearch2.Text,
                new SOAStatus[] { SOAStatus.PROC });
            if (dt == null)
            {
                redirectToErrorPg("Error searching trainees.");
                return;
            }

            gvTrainee.Columns[0].Visible = true;
            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();
            gvTrainee.Columns[0].Visible = false;

            panelProcess.Visible = dt.Rows.Count > 0;
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            List<Tuple<string, int>> lstSelected = new List<Tuple<string, int>>();
            foreach (GridViewRow gvr in gvTrainee.Rows)
            {
                if (!((CheckBox)gvr.FindControl("cb")).Checked) continue;

                lstSelected.Add(new Tuple<string, int>(gvr.Cells[2].Text, int.Parse(gvr.Cells[0].Text)));
            }

            if (lstSelected.Count == 0)
            {
                lblError.Text = "No trainee is selected.";
                panelError.Visible = true;
                return;
            }

            Tuple<bool, string> status = (new Assessment_Management()).receiveSOA(lstSelected, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                btnSearch_Click(null, null);
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void lkbtnProcSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_process.PAGE_NAME);
        }

        protected void lkbtnViewSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_view.PAGE_NAME);
        }
    }
}