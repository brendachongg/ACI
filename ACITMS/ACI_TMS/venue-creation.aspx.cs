using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class venue_creation : BasePage
    {
        public const string PAGE_NAME = "venue-creation.aspx";

        private const string DATA_KEY = "dtProgramme";
        private Venue_Management vm = new Venue_Management();

        public venue_creation()
            : base(PAGE_NAME, AccessRight_Constance.PROGRAM_NEW, venue_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {

            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbVenueID.Text = "";
            tbLocation.Text = "";
            tbDescription.Text = "";
            tbCapacity.Text = "";
            tbEffectiveDate.Text = "";
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            string venueID = tbVenueID.Text;
            string venueLocation = tbLocation.Text;
            string venueDesc = tbDescription.Text;
            int venueCapacity = Int32.Parse(tbCapacity.Text);
            DateTime venueEffectDate = Convert.ToDateTime(tbEffectiveDate.Text);

            Tuple<bool, string> success = vm.createVenue(venueID, venueLocation, venueDesc, venueCapacity, venueEffectDate, LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
                btnClear_Click(null, null);
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "displayStatus", "scrollTopPage();", true);
        }


    }
}