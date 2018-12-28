using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS.kiosk
{
    public partial class registration_success : System.Web.UI.Page
    {
        public const string PAGE_NAME = "registration-success.aspx";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PreviousPage != null && PreviousPage is registration) lbAppId.Text = ((registration)PreviousPage).ApplicantId;
            }
        }
    }
}