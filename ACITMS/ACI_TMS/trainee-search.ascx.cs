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
    public partial class trainee_search : System.Web.UI.UserControl
    {
        private const string DATA_KEY = "dtTrainee";

        public delegate void SelectTrainee(string id, string name);
        public event SelectTrainee selectTrainee;

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbtnSearchTrainee_Click(object sender, EventArgs e)
        {
            string type = null;
            if (rbId.Checked) type = "ID";
            else if (rbName.Checked) type = "NAME";
            else if (rbNRIC.Checked) type = "NRICPIN";

            DataTable dt = (new Trainee_Management()).searchTrainee(type, tbSearchTrainee.Text);
            if (dt == null)
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving trainees.");
                return;
            }

            ViewState[DATA_KEY] = dt;
            gvTrainee.DataSource = dt;
            gvTrainee.Columns[0].Visible = true;
            gvTrainee.DataBind();
            gvTrainee.Columns[0].Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showTraineeDialog();", true);
        }

        protected void gvTrainee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectTrainee"))
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[DATA_KEY] as DataTable;

                selectTrainee(dt.Rows[index]["traineeId"].ToString(), dt.Rows[index]["fullName"].ToString());
            }
        }

        protected void gvTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrainee.DataSource = ViewState[DATA_KEY] as DataTable;
            gvTrainee.PageIndex = e.NewPageIndex;
            gvTrainee.Columns[0].Visible = true;
            gvTrainee.DataBind();
            gvTrainee.Columns[0].Visible = false;

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showTraineeDialog();", true);
        }
    }
}