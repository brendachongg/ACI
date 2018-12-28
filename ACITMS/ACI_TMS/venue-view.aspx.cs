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
    public partial class venue_view : BasePage
    {
        public const string PAGE_NAME = "venue-view.aspx";
        public const string QUERY_ID = "vId";

        private Venue_Management vm = new Venue_Management();

        public venue_view()
            : base(PAGE_NAME, AccessRight_Constance.VENUE_VIEW, venue_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null)
                {
                    redirectToErrorPg("Missing venue information.");
                    return;
                }

                hfVenueId.Value = Request.QueryString[QUERY_ID];

                loadVenueDetail();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadVenueDetail()
        {
            DataTable dtVenue = vm.getVenue(hfVenueId.Value);

            if (dtVenue == null || dtVenue.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving venue details.");
                return;
            }

            foreach (DataRow dr in dtVenue.Rows)
            {
                lbVenueIdValue.Text = dr["venueId"].ToString();
                lbLocationValue.Text = dr["venueLocation"].ToString();
                tbDescription.Text = dr["venueDesc"].ToString();
                lbCapacityValue.Text = dr["venueCapacity"].ToString();
                lbEffectiveDateValue.Text = dr["venueEffectDate"].ToString();
            }
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            Response.Redirect(venue_edit.PAGE_NAME + "?" + venue_edit.QUERY_ID + "=" + hfVenueId.Value);
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = vm.deleteVenue(hfVenueId.Value, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnConfirmDel.Visible = false;
                btnUpdate.Visible = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }
    }
}