using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;
using System.Configuration;
using System.IO;

namespace ACI_TMS
{
    public partial class soa_process : BasePage
    {
        public const string PAGE_NAME = "soa-process.aspx";

        public soa_process()
            : base(PAGE_NAME, AccessRight_Constance.SOA_PROCESS)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            //cos the search 2 textbox is hidden dyanmically at runtime, so everytime the page load need to recreate again
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSearch2", "showSearch2();", true);

            if (!IsPostBack)
            {
                loadContacts();

                bool canView, canReceive;
                canView = checkAccessRights(AccessRight_Constance.SOA_VIEW);
                canReceive = checkAccessRights(AccessRight_Constance.SOA_RECEIVE);
                if (!canView && !canReceive) panelOperations.Visible = false;
                else
                {
                    if (!canView) panelViewSOA.Visible = false;
                    if (!canReceive) panelRecevSOA.Visible = false;
                }
            }
            else panelError.Visible = false;
        }

        private void loadContacts()
        {
            DataTable dt = (new ACI_Staff_User()).getSOAContacts();
            if (dt == null)
            {
                redirectToErrorPg("Error searching trainees.");
                return;
            }

            ddlContact.DataSource = dt;
            ddlContact.DataBind();
            ddlContact.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DataTable dt = (new Assessment_Management()).searchSOATrainee(ddlSearch.SelectedValue, txtSearch.Text, txtSearch2.Text,
                new SOAStatus[] { SOAStatus.NYA, SOAStatus.PROC });
            if (dt == null)
            {
                redirectToErrorPg("Error searching trainees.");
                return;
            }

            gvTrainee.Columns[0].Visible = true;
            gvTrainee.Columns[1].Visible = true;
            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();
            gvTrainee.Columns[0].Visible = false;
            gvTrainee.Columns[1].Visible = false;

            panelProcess.Visible = dt.Rows.Count > 0;
        }

        protected void btnProcess_Click(object sender, EventArgs e)
        {
            List<Tuple<string, int>> lstSelected = new List<Tuple<string, int>>();
            foreach (GridViewRow gvr in gvTrainee.Rows)
            {
                if (!((CheckBox)gvr.FindControl("cb")).Checked) continue;

                lstSelected.Add(new Tuple<string, int>(gvr.Cells[1].Text, int.Parse(gvr.Cells[0].Text)));
            }

            if (lstSelected.Count == 0)
            {
                lblError.Text = "No trainee is selected.";
                panelError.Visible = true;
                return;
            }

            string path = Server.MapPath(ConfigurationManager.AppSettings["tempFolder"].ToString());
            //TODO: temporary measure to pass in the cert code used, should be retrieved from database
            Tuple<bool, string> status = (new Assessment_Management()).generateSOA(lstSelected, int.Parse(ddlContact.SelectedValue), ddlCertCode.SelectedValue, path, LoginID);

            if (status.Item1)
            {
                btnSearch_Click(null, null);

                Page.ClientScript.RegisterStartupScript(this.GetType(), "export", "window.open('" + soa_export.PAGE_NAME + "?" + soa_export.FILE_QUERY + "=" + HttpUtility.UrlEncode(status.Item2)
                    + "', '_blank', 'menubar=no,location=no');", true);
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void lkbtnViewSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_view.PAGE_NAME);
        }

        protected void lkbtnRecevSOA_Click(object sender, EventArgs e)
        {
            Server.Transfer(soa_receive.PAGE_NAME);
        }
    }
}