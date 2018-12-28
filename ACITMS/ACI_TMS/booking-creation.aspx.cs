using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class booking_creation : BasePage
    {
        public const string PAGE_NAME = "booking-creation.aspx";
        public const string GV_DATA = "BOOKING";

        private Venue_Management vm = new Venue_Management();

        public booking_creation()
            : base(PAGE_NAME, AccessRight_Constance.BOOKING_NEW, booking_management.PAGE_NAME)
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

            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
        }

        private void selectVenue(string venueId, string venueLocation)
        {
            hfVenueId.Value = venueId;
            tbVenue.Text = venueLocation;
        }

        protected void btnAvailable_Click(object sender, EventArgs e)
        {
            DataTable dtBooking = vm.getBooking(DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), hfVenueId.Value);

            if (dtBooking == null)
            {
                redirectToErrorPg("Error retrieving booking records.");
                return;
            }

            ViewState[GV_DATA] = dtBooking;
            panelBooking.Visible = true;
            gvNewBooking.Columns[1].Visible = true;
            gvNewBooking.DataSource = dtBooking;
            gvNewBooking.DataBind();
            gvNewBooking.Columns[1].Visible = false;
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbDate.Text = "";
            tbRemark.Text = "";
            tbVenue.Text = "";
            hfVenueId.Value = "";
            panelBooking.Visible = false;
        }

        protected void gvNewBooking_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            CheckBox cbNewBooking = (CheckBox)e.Row.FindControl("cbNewBooking");

            if (((DataRowView)e.Row.DataItem)["sessionId"] != DBNull.Value)
            {
                Label lbBookingClassCode = (Label)e.Row.FindControl("lbBookingClassCode");
                Label lbBookingModuleId = (Label)e.Row.FindControl("lbBookingModuleId");

                lbBookingClassCode.Text = "Class: " + ((DataRowView)e.Row.DataItem)["batchCode"].ToString();
                lbBookingModuleId.Text = "/ Module : " + ((DataRowView)e.Row.DataItem)["moduleTitle"].ToString() + " (" + ((DataRowView)e.Row.DataItem)["moduleCode"].ToString() + ")";
                lbBookingClassCode.Visible = true;
                lbBookingModuleId.Visible = true;
                cbNewBooking.Enabled = false;
            }
            else
            {
                Label lbBookingDetail = (Label)e.Row.FindControl("lbBookingDetail");
                if (((DataRowView)e.Row.DataItem)["bookingRemarks"] != DBNull.Value)
                {
                    lbBookingDetail.Text = ((DataRowView)e.Row.DataItem)["bookingRemarks"].ToString();
                    cbNewBooking.Enabled = false;
                }
                lbBookingDetail.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            List<DayPeriod> period = new List<DayPeriod>();
            foreach (GridViewRow gvRow in gvNewBooking.Rows)
            {
                CheckBox cbNewBooking = (CheckBox)gvRow.FindControl("cbNewBooking");

                if (cbNewBooking != null && cbNewBooking.Checked)
                    period.Add((DayPeriod)Enum.Parse(typeof(DayPeriod), gvRow.Cells[1].Text));
            }

            if (period.Count == 0)
            {
                lblError.Text = "Must select at least 1 period.";
                panelError.Visible = true;
                return;
            }

            Tuple<bool, string> success = vm.createBooking(hfVenueId.Value, DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), period.ToArray(), tbRemark.Text, LoginID);

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
        }

    }
}