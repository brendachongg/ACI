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
    public partial class booking_view : BasePage
    {
        public const string PAGE_NAME = "booking-view.aspx";
        public const string QUERY_ID = "bId";
        public const string GV_DATA = "BOOKING";

        private Venue_Management vm = new Venue_Management();

        public booking_view()
            : base(PAGE_NAME, AccessRight_Constance.BOOKING_VIEW, booking_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID]=="")
                {
                    redirectToErrorPg("Missing module information.");
                    return;
                }

                hfBookingId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);
                loadBooking();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
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

            if (dr["bookingRemarks"] != DBNull.Value && dr["bookingRemarks"].ToString() != "")
            {
                panelRemark.Visible = true;
                panelSession.Visible = false;

                lbRemarkValue.Text = dr["bookingRemarks"].ToString();
            }
            else if (dr["sessionId"] != DBNull.Value)
            {
                panelRemark.Visible = false;
                panelSession.Visible = true;

                lbClassCodeValue.Text = dr["batchCode"].ToString();
                lbModuleValue.Text = dr["moduleTitle"].ToString();
                lbModuleCodeValue.Text = dr["moduleCode"].ToString();

                btnConfirmDel.Visible = false;
                btnEdit.Visible = false;
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Response.Redirect(booking_edit.PAGE_NAME + "?" + booking_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfBookingId.Value));
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = vm.deleteBooking(int.Parse(hfBookingId.Value), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnConfirmDel.Visible = false;
                btnEdit.Visible = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

    }
}