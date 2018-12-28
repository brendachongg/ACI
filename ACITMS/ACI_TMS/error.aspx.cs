using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session[BasePage.ERROR_DESC_KEY] != null)
                lblErr.Text = Session[BasePage.ERROR_DESC_KEY].ToString();

            if (Session[BasePage.ERROR_TITLE_KEY] != null)
                lblTitle.Text = Session[BasePage.ERROR_TITLE_KEY].ToString();

            btnOk.Visible = (Session[BasePage.ERROR_GOTO_KEY] != null);
        }

        protected void btnOk_Click(object sender, EventArgs e)
        {
            if (Session[BasePage.ERROR_GOTO_KEY] != null)
                Response.Redirect(Session[BasePage.ERROR_GOTO_KEY].ToString());
        }
    }
}