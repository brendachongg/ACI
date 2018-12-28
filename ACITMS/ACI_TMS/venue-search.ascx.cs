using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using LogicLayer;

namespace ACI_TMS
{
    public partial class venue_search : System.Web.UI.UserControl
    {
        public delegate void SelectVenue(string venueId, string venueLocation);
        public event SelectVenue selectVenue;

        public delegate void VenueRefresh();
        public event VenueRefresh venueRefresh;

        //this will be set by the parent page to determine what kind of recent venue to load
        public enum RecentType
        {
            SESSION,
            OTHERS
        }
        public RecentType type = RecentType.OTHERS;

        private const string DATA_KEY = "dtVenues";
        private Venue_Management vm = new Venue_Management();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                DataTable dtRecent = vm.getRecentVenues(type.ToString());
                gvRecentVenue.DataSource = dtRecent;
                gvRecentVenue.DataBind();

                loadListVenue("A", "D");
            }
        }

        private void loadListVenue(string frm, string to)
        {
            DataTable dt = vm.getListVenues(frm, to);

            if (dt != null)
            {
                ViewState[DATA_KEY] = dt;
                gvListVenue.DataSource = dt;
                gvListVenue.DataBind();
            }
            else
            {
                ((BasePage)this.Page).redirectToErrorPg("Error retrieving venue listing.");
            }
        }

        protected void btnVenue_Click(object sender, EventArgs e)
        {
            string frm = ((Button)sender).CommandArgument.Substring(0, 1);
            string to = ((Button)sender).CommandArgument.Substring(1);

            loadListVenue(frm, to);

            btnVenueAD.CssClass = "btn btn-default";
            btnVenueEH.CssClass = "btn btn-default";
            btnVenueIL.CssClass = "btn btn-default";
            btnVenueMP.CssClass = "btn btn-default";
            btnVenueQT.CssClass = "btn btn-default";
            btnVenueUX.CssClass = "btn btn-default";
            btnVenueYZ.CssClass = "btn btn-default";

            ((Button)sender).CssClass = "btn btn-primary";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showVDialog();showListVenue();", true);
            if (venueRefresh != null) venueRefresh();
        }

        protected void gvVenue_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("selectVenueRecent"))
            {
                int index = int.Parse(e.CommandArgument.ToString());

                TableCell c = ((GridView)sender).Rows[index].Cells[0];
                string id = ((HiddenField)c.FindControl("hfVenueId")).Value;
                string location = ((Label)c.FindControl("lbgvenueLocation")).Text;

                selectVenue(id, location);
            }
            else if (e.CommandName.Equals("selectVenueList"))
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());

                DataTable dt = ViewState[DATA_KEY] as DataTable;

                selectVenue(dt.Rows[index]["venueId"].ToString(), dt.Rows[index]["venueLocation"].ToString());
            }
        }

        protected void gvListVenue_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvListVenue.DataSource = ViewState[DATA_KEY] as DataTable;
            gvListVenue.PageIndex = e.NewPageIndex;
            gvListVenue.DataBind();

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showModal", "showVDialog();showListVenue();", true);
            if (venueRefresh != null) venueRefresh();
        }
    }
}