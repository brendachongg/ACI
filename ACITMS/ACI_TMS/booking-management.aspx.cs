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
    public partial class booking_management : BasePage
    {
        public const string PAGE_NAME = "booking-management.aspx";
        public const string GV_DATA = "BOOKING";

        private Venue_Management vm = new Venue_Management();

        public booking_management()
            : base(PAGE_NAME, AccessRight_Constance.BOOKING_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                if (!checkAccessRights(AccessRight_Constance.BOOKING_NEW)) panelNewBooking.Visible = false;
            }

            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
        }

        private void selectVenue(string venueId, string venueLocation)
        {
            hfVenueId.Value = venueId;
            tbVenue.Text = venueLocation;
        }

        protected void lkbtnCreateBooking_Click(object sender, EventArgs e)
        {
            Response.Redirect(booking_creation.PAGE_NAME);
        }

        protected void gvBooking_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvBooking.DataSource = ViewState[GV_DATA] as DataTable;
            gvBooking.PageIndex = e.NewPageIndex;
            gvBooking.DataBind();
        }

        protected void gvBooking_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (((DataRowView)e.Row.DataItem)["sessionId"] != DBNull.Value)
            {
                Label lbBookingClassCode = (Label)e.Row.FindControl("lbBookingClassCode");
                Label lbBookingModuleId = (Label)e.Row.FindControl("lbBookingModuleId");

                lbBookingClassCode.Text = "Class: " + ((DataRowView)e.Row.DataItem)["batchCode"].ToString();
                lbBookingModuleId.Text = "/ Module : " + ((DataRowView)e.Row.DataItem)["moduleTitle"].ToString() + " (" + ((DataRowView)e.Row.DataItem)["moduleCode"].ToString() + ")";
                lbBookingClassCode.Visible = true;
                lbBookingModuleId.Visible = true;
            }
            else
            {
                Label lbBookingDetail = (Label)e.Row.FindControl("lbBookingDetail");
                lbBookingDetail.Text = ((DataRowView)e.Row.DataItem)["bookingRemarks"].ToString();
                lbBookingDetail.Visible = true;
            }

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime dtStart = DateTime.MinValue, dtEnd = DateTime.MaxValue;
            if (tbStartDate.Text != "") dtStart = DateTime.ParseExact(tbStartDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            if (tbEndDate.Text != "") dtEnd = DateTime.ParseExact(tbEndDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

            DataTable dtBooking = vm.getBooking(hfVenueId.Value,dtStart, dtEnd);
            if (dtBooking == null)
            {
                redirectToErrorPg("Error retrieving booking records.");
                return;
            }

            ViewState[GV_DATA] = dtBooking;
            gvBooking.Columns[2].Visible = true;
            gvBooking.DataSource = dtBooking;
            gvBooking.DataBind();
            if (tbVenue.Text != "") gvBooking.Columns[2].Visible = false;
        }

        protected void gvBooking_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewBooking"))
            {
                Response.Redirect(booking_view.PAGE_NAME + "?" + booking_view.QUERY_ID + "=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            tbVenue.Text = "";
            hfVenueId.Value = "";
            tbStartDate.Text = "";
            tbEndDate.Text = "";
        }
    }
}