using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;

namespace ACI_TMS
{
    public partial class bundle_module_rem_session : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public void loadSessions(int programmeBatchId, int moduleId, int remSessions)
        {
            DataTable dt = (new Batch_Session_Management()).getBatchModuleSessions(programmeBatchId, moduleId);
            if (dt == null)
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving session for module.");
                return;
            }

            gvRemovalSession.DataSource = dt;
            gvRemovalSession.Columns[0].Visible = true;
            gvRemovalSession.Columns[1].Visible = true;
            gvRemovalSession.DataBind();
            gvRemovalSession.Columns[0].Visible = false;
            gvRemovalSession.Columns[1].Visible = false;

            hfTotalSession.Value = dt.Rows.Count.ToString();

            if (remSessions == 0)
            {
                //to remove all sessions
                CheckBox cb;
                foreach (GridViewRow r in gvRemovalSession.Rows)
                {
                    cb = r.Cells[1].FindControl("cb") as CheckBox;
                    cb.Checked = true;
                    cb.Enabled = false;
                }

                lblNumSession.Text = gvRemovalSession.Rows.Count.ToString();
            }
            else lblNumSession.Text = remSessions.ToString();
        }
    }
}