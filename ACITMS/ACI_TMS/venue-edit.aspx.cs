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
    public partial class venue_edit : BasePage
    {
        public const string PAGE_NAME = "venue-edit.aspx";
        public const string QUERY_ID = "vId";
        public const string GV_DATA = "VENUE";

        private Venue_Management vm = new Venue_Management();
        public venue_edit()
            : base(PAGE_NAME, AccessRight_Constance.VENUE_EDIT, venue_view.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PrevPage = PrevPage + "?" + venue_view.QUERY_ID + "=" + hfVenueId.Value;

            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing venue information.", venue_management.PAGE_NAME);
                    return;
                }

                hfVenueId.Value = Request.QueryString[QUERY_ID];
                loadVenueDetail();
            }
            else
            {
                panelSysError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadVenueDetail()
        {
            DataTable dtVenue = vm.getVenue(hfVenueId.Value);
            ViewState[GV_DATA] = dtVenue;

            if (dtVenue == null || dtVenue.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving venue details.");
                return;
            }

            foreach (DataRow dr in dtVenue.Rows)
            {
                lbVenueIDValue.Text = dr["venueId"].ToString();
                tbLocation.Text = dr["venueLocation"].ToString();
                tbDescription.Text = dr["venueDesc"].ToString();
                tbCapacity.Text = dr["venueCapacity"].ToString();
                tbEffectiveDate.Text = dr["venueEffectDate"].ToString();
            }
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            string venueId = hfVenueId.Value;
            string venueLocation = tbLocation.Text;
            string venueDesc = tbDescription.Text;
            int venueCapacity = Int32.Parse(tbCapacity.Text);
            DateTime venueEffectDate = Convert.ToDateTime(tbEffectiveDate.Text);
            Tuple<bool, string> updateVenue = vm.updateVenue(venueId, venueLocation, venueDesc, venueCapacity, venueEffectDate, LoginID);

            if (updateVenue.Item1)
            {
                lblSuccess.Text = updateVenue.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblSysError.Text = updateVenue.Item2;
                panelSysError.Visible = true;
            }
        }

        protected void btnClearVenue_Click(object sender, EventArgs e)
        {
            loadVenueDetail();
        }
    }
}