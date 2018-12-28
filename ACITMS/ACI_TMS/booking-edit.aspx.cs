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
    public partial class booking_edit : BasePage
    {
        public const string PAGE_NAME = "booking-edit.aspx";
        public const string QUERY_ID = "bId";

        private Venue_Management vm = new Venue_Management();

        public booking_edit()
            : base(PAGE_NAME, AccessRight_Constance.BOOKING_EDIT, booking_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing module information.");
                    return;
                }

                hfBookingId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);
                PrevPage = booking_view.PAGE_NAME + "?" + booking_view.QUERY_ID + "=" + HttpUtility.UrlEncode(hfBookingId.Value);
                loadBooking();
            }
            else
            {
                PrevPage = booking_view.PAGE_NAME + "?" + booking_view.QUERY_ID + "=" + HttpUtility.UrlEncode(hfBookingId.Value);
                panelSuccess.Visible = false;
                panelError.Visible = false;
            }
        }

        private void loadBooking()
        {
            DataTable bookingDT = vm.getBooking(int.Parse(hfBookingId.Value));

            if (bookingDT == null || bookingDT.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving booking record.");
                return;
            }

            DataRow dr = bookingDT.Rows[0];

            lbVenueValue.Text = dr["venueLocation"].ToString();
            lbDateValue.Text = dr["bookingDate"].ToString();
            lbPeriodValue.Text = dr["bookingPeriodDisp"].ToString();
            tbRemarkValue.Text = dr["bookingRemarks"].ToString();          
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> success = vm.updateBooking(int.Parse(hfBookingId.Value), tbRemarkValue.Text, LoginID);

            if (success.Item1)
            {
                panelSuccess.Visible = true;
                lblSuccess.Text = success.Item2;
            }
            else
            {
                panelError.Visible = true;
                lblError.Text = success.Item2;
            }
        }
    }
}