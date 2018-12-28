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
    public partial class session_edit : BasePage
    {
        public const string PAGE_NAME = "session-edit.aspx";
        public const string QUERY_ID = "id";

        public session_edit()
            : base(PAGE_NAME, AccessRight_Constance.SESSION_EDIT, session_view.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing session information.");
                    return;
                }

                PrevPage += "?" + session_view.QUERY_ID + "=" + Request.QueryString[QUERY_ID];
                hfSessionId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);
                loadData();

                //if session already exceed current date cannot edit
                if (DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture).CompareTo(DateTime.Now) < 0)
                    btnSave.Visible = false;
            }
            else
            {
                PrevPage += "?" + session_view.QUERY_ID + "=" + HttpUtility.UrlEncode(hfSessionId.Value);
                panelSuccess.Visible = false;
                panelSysError.Visible = false;
            }

            venuesearch.type = venue_search.RecentType.SESSION;
            venuesearch.selectVenue += new venue_search.SelectVenue(selectVenue);
        }

        private void loadData()
        {
            Batch_Session_Management bsm = new Batch_Session_Management();
            DataTable dtPeriods = (new Venue_Management()).getPeriods();
            DataTable dt = bsm.getSessionDetails(int.Parse(hfSessionId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving session details");
                return;
            }

            DataRow dr = dt.Rows[0];

            lbProgrammeCategory.Text = dr["programmeCategoryDisp"].ToString();
            lbProgrammeLevel.Text = dr["programmeLevelDisp"].ToString();
            lbProgramme.Text = dr["programmeTitle"].ToString();
            lbProgrammeVersion.Text = dr["programmeVersion"].ToString();
            lbProgrammeCode.Text = dr["programmeCode"].ToString();
            lbProgrammeType.Text = dr["programmeTypeDisp"].ToString();

            lbBatchCode.Text = dr["batchCode"].ToString();
            lbBatchType.Text = dr["batchTypeDisp"].ToString();
            lbProjCode.Text = dr["projectCode"].ToString();
            lbRegStartDate.Text = dr["programmeRegStartDateDisp"].ToString();
            lbRegEndDate.Text = dr["programmeRegEndDateDisp"].ToString();
            lbBatchStartDate.Text = dr["programmeStartDateDisp"].ToString();
            lbBatchEndDate.Text = dr["programmeCompletionDateDisp"].ToString();
            lbCapacity.Text = dr["batchCapacity"].ToString();
            lbClassMode.Text = dr["classModeDisp"].ToString();

            lbModule.Text = dr["moduleTitle"].ToString();
            lbModuleCode.Text = dr["moduleCode"].ToString();
            lbDay.Text = bsm.formatDayStr(dr["day"].ToString(), dtPeriods);
            lbModDtFrm.Text = hfModDtFrm.Value = dr["startDateDisp"].ToString();
            lbModDtTo.Text = hfModDtTo.Value = dr["endDateDisp"].ToString();
            lbTrainer1.Text = dr["trainerUserName1"].ToString();
            lbTrainer2.Text = dr["trainerUserName2"] == DBNull.Value ? "" : dr["trainerUserName2"].ToString();
            lbAssessor.Text = dr["assessorUserName"].ToString();

            tbDate.Text = dr["sessionDateDisp"].ToString();
            ddlPeriod.DataSource = dtPeriods;
            ddlPeriod.DataBind();
            ddlPeriod.Items.Insert(0, new ListItem("--Select--", ""));
            ddlPeriod.SelectedValue = dr["sessionPeriod"].ToString();
            tbVenue.Text = dr["venueLocation"].ToString();
            hfVenueId.Value = dr["venueId"].ToString();

        }


        private void selectVenue(string id, string location)
        {
            tbVenue.Text = location;
            hfVenueId.Value = id;

            //check venue available
            if ((new Venue_Management()).checkVenueAvailable(DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                (DayPeriod)Enum.Parse(typeof(DayPeriod), ddlPeriod.SelectedValue), id, int.Parse(lbCapacity.Text)))
            {
                lbVenueAva.Text = "(Available)";
                lbVenueAva.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                lbVenueAva.Text = "(Not Available)";
                lbVenueAva.ForeColor = System.Drawing.Color.Red;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Batch_Session_Management()).updateSession(int.Parse(hfSessionId.Value),
                DateTime.ParseExact(tbDate.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), (DayPeriod)Enum.Parse(typeof(DayPeriod), ddlPeriod.SelectedValue), 
                hfVenueId.Value, int.Parse(lbCapacity.Text), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblSysError.Text = status.Item2;
                panelSysError.Visible = true;
            }
        }
    }
}