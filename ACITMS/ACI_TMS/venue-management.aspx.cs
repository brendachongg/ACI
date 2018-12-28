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
    public partial class venue_management : BasePage
    {
        public const string PAGE_NAME = "venue-management.aspx";

        private const string GV_DATA = "VENUE";
        private Venue_Management vm = new Venue_Management();

        public venue_management()
            : base(PAGE_NAME, AccessRight_Constance.VENUE_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadVenue();

                if (!checkAccessRights(AccessRight_Constance.VENUE_NEW))
                    panelNewVenue.Visible = false;
            }
        }

        private void loadVenue()
        {
            DataTable dt = vm.getAllVenue();

            if (dt == null)
            {
                redirectToErrorPg("Error retrieving venues.");
                return;
            }

            ViewState[GV_DATA] = dt;

            gvVenue.DataSource = dt;
            gvVenue.DataBind();
        }

        protected void lkbtnCreateVenue_Click(object sender, EventArgs e)
        {
            Response.Redirect(venue_creation.PAGE_NAME);
        }

        protected void gvVenue_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewVenueDetails"))
            {
                Response.Redirect(venue_view.PAGE_NAME + "?vId=" + HttpUtility.UrlEncode(Convert.ToString(e.CommandArgument)));
            }
        }

        protected void gvVenue_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvVenue.DataSource = ViewState[GV_DATA] as DataTable;
            gvVenue.PageIndex = e.NewPageIndex;
            gvVenue.DataBind();
        }
    }
}