using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Web.UI.HtmlControls;
using System.Globalization;
using GeneralLayer;

namespace ACI_TMS
{
    public partial class bundle_module_new_session : System.Web.UI.UserControl
    {
        private DataTable dtPeriods;

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public void loadModule(int programmeId, int programmeBatchId, int moduleId, DateTime batchStartDt, int numSessions, string type)
        {
            Batch_Session_Management bsm=new Batch_Session_Management();

            dtPeriods = (new Venue_Management()).getPeriods();
            hfbatchStartDt.Value = batchStartDt.ToString("dd MMM yyyy");

            if (type == "NEW") {
                ACI_Staff_User su = new ACI_Staff_User();
                DataTable dtTrainers = su.getTrainers();
                DataTable dtAssessors = su.getAssessors();

                ddlTrainer1.DataSource = dtTrainers;
                ddlTrainer1.DataBind();
                ddlTrainer1.Items.Insert(0, new ListItem("--Select--", ""));

                ddlTrainer2.DataSource = dtTrainers;
                ddlTrainer2.DataBind();
                ddlTrainer2.Items.Insert(0, new ListItem("--Select--", ""));

                ddlAssessor.DataSource = dtAssessors;
                ddlAssessor.DataBind();
                ddlAssessor.Items.Insert(0, new ListItem("--Select--", ""));

                panelNewModule.Visible = true;
                lbtnGenSession.Attributes.Add("onClick", "return checkModuleDate('" + tbModDtFrm.ClientID + "', '" + tbDay.ClientID + "', '" + batchStartDt.ToString("dd MMM yyyy") + "', '" + GenSession.UniqueID + "');");
                lbtnSelDay.Attributes.Add("onClick", "showDayDialog("+moduleId+", "+programmeId+", "+programmeBatchId+");");
            }
            else
            {
                panelNewModule.Visible = false;
                lbtnGenSession.Visible = false;
                tbModDtFrm.Enabled = false;
                lbtnSelDay.Visible = false;

                DataTable dtInfo = bsm.getBatchModuleDates(programmeBatchId, moduleId);
                if (dtInfo == null || dtInfo.Rows.Count == 0)
                {
                    ((BasePage)this.Page).redirectToErrorPg("Error retrieving batch info");
                    return;
                }

                DataRow drInfo = dtInfo.Rows[0];
                hfDay.Value = drInfo["Day"].ToString();
                tbDay.Text = bsm.formatDayStr(drInfo["Day"].ToString(), dtPeriods);
                tbModDtFrm.Text = drInfo["startDateDisp"].ToString();
                tbModDtTo.Text = drInfo["endDateDisp"].ToString();
            }

            loadSessions(programmeId, programmeBatchId, moduleId, numSessions);

            //if new module disable the fields in session first until user select the date and day
            if (type == "NEW")
            {
                foreach(RepeaterItem r in rpNewSessions.Items)
                {
                    ((TextBox)r.FindControl("tbNewSessionDt")).Enabled = false;
                    ((DropDownList)r.FindControl("ddlNewSessionPeriod")).Enabled = false;
                }
            }
        }

        private void loadSessions(int programmeId, int programmeBatchId, int moduleId, int extraSessions)
        {
            lblNumSession.Text = extraSessions.ToString();

            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("sessionNo", typeof(int)));
            dt.Columns.Add(new DataColumn("moduleId", typeof(int)));
            dt.Columns.Add(new DataColumn("programmeId", typeof(int)));
            dt.Columns.Add(new DataColumn("programmeBatchId", typeof(int)));

            DataRow dr;
            for (int i = 1; i <= extraSessions; i++)
            {
                dr = dt.NewRow();
                dr["sessionNo"] = i;
                dr["moduleId"] = moduleId;
                dr["programmeId"] = programmeId;
                dr["programmeBatchId"] = programmeBatchId;
                dt.Rows.Add(dr);
            }

            rpNewSessions.DataSource = dt;
            rpNewSessions.DataBind();
        }

        public void selectVenue(int sessionNo, string venueId, string venueLocation)
        {
            HiddenField hf;
            Label lbl;
            //find the item within the repeater
            foreach (RepeaterItem r in rpNewSessions.Items)
            {
                hf = r.FindControl("hfSessionNo") as HiddenField;

                if (hf == null || hf.Value != sessionNo.ToString()) continue;

                ((TextBox)r.FindControl("tbNewSessionVenue")).Text = venueLocation;
                ((HiddenField)r.FindControl("hfNewSessionVenueId")).Value = venueId;

                lbl = (Label)r.FindControl("lbVenueAva");

                if ((new Venue_Management()).checkVenueAvailableForBatch(DateTime.ParseExact(((TextBox)r.FindControl("tbNewSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    (DayPeriod)Enum.Parse(typeof(DayPeriod), ((DropDownList)r.FindControl("ddlNewSessionPeriod")).SelectedValue), venueId, int.Parse(((HiddenField)r.FindControl("hfProgBatch")).Value)))
                {
                    lbl.Text = "(Available)";
                    lbl.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lbl.Text = "(Not Available)";
                    lbl.ForeColor = System.Drawing.Color.Red;
                }
                lbl.Font.Italic = true;

                break;
            }
        }

        public void refreshVenueAva(int sessionNo)
        {
            HiddenField hf;
            Label lbl;
            //find the item within the repeater
            foreach (RepeaterItem r in rpNewSessions.Items)
            {
                hf = r.FindControl("hfSessionNo") as HiddenField;

                if (hf == null || hf.Value != sessionNo.ToString()) continue;

                lbl = (Label)r.FindControl("lbVenueAva");
                if ((new Venue_Management()).checkVenueAvailableForBatch(DateTime.ParseExact(((TextBox)r.FindControl("tbNewSessionDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture),
                    (DayPeriod)Enum.Parse(typeof(DayPeriod), ((DropDownList)r.FindControl("ddlNewSessionPeriod")).SelectedValue), ((HiddenField)r.FindControl("hfNewSessionVenueId")).Value, 
                    int.Parse(((HiddenField)r.FindControl("hfProgBatch")).Value)))
                {
                    lbl.Text = "(Available)";
                    lbl.ForeColor = System.Drawing.Color.Green;
                }
                else
                {
                    lbl.Text = "(Not Available)";
                    lbl.ForeColor = System.Drawing.Color.Red;
                }
                lbl.Font.Italic = true;

                break;
            }
        }

        protected void rpNewSessions_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DropDownList ddl = e.Item.FindControl("ddlNewSessionPeriod") as DropDownList;
            ddl.DataValueField = "codeValue";
            ddl.DataTextField = "codeValueDisplay";
            ddl.DataSource = dtPeriods;
            ddl.DataBind();
            ddl.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void lbtnGenSession_Click(object sender, EventArgs e)
        {
            List<DateTime> dt = new List<DateTime>();
            foreach (RepeaterItem r in rpNewSessions.Items)
            {
                TextBox tb = r.FindControl("tbNewSessionDt") as TextBox;
                if (tb.Text != "")
                {
                    DateTime d;
                    if (!DateTime.TryParseExact(tb.Text, "dd MMM yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out d)) continue;
                    dt.Add(d);
                }
            }
            if (dt.Count == rpNewSessions.Items.Count)
            {
                //user has fill in all the dates, just find the start and end date
                tbModDtTo.Text = dt.Max().ToString("dd MMM yyyy");
                tbModDtFrm.Text = dt.Min().ToString("dd MMM yyyy");
                ((bundle_edit_sessions)this.Page).showWarning("Session date(s) are filled, only module start and end date are updated.");
                ((bundle_edit_sessions)this.Page).showCurrentTabsNPills();
                return;
            }
            else if (dt.Count > 0)
            {
                //user has fill in some but not all the dates, cannot fill in end date nor can generate sessions (as will overwrite current)
                ((bundle_edit_sessions)this.Page).showWarning("Only some session date(s) are filled, unable to update module end date.");
                ((bundle_edit_sessions)this.Page).showCurrentTabsNPills();
                return;
            }

            //get the day from the selected start date
            DateTime dtStart = DateTime.ParseExact(tbModDtFrm.Text, "dd MMM yyyy", CultureInfo.InvariantCulture);

            string[,] sessionDates = (new Batch_Session_Management()).genSessions(hfDay.Value, dtStart, DayPeriod.AM, int.Parse(lblNumSession.Text));
            for (int i = 0; i < int.Parse(lblNumSession.Text); i++)
            {
                ((TextBox)rpNewSessions.Items[i].FindControl("tbNewSessionDt")).Text = sessionDates[i, 0];
                DropDownList ddl = rpNewSessions.Items[i].FindControl("ddlNewSessionPeriod") as DropDownList;
                ddl.SelectedIndex = ddl.Items.IndexOf(ddl.Items.FindByValue(sessionDates[i, 1]));
            }

            //tbModDtFrm.Text = sessionDates[0, 0];
            tbModDtTo.Text = sessionDates[int.Parse(lblNumSession.Text)-1, 0];

            //enable the session fields
            foreach (RepeaterItem r in rpNewSessions.Items)
            {
                ((TextBox)r.FindControl("tbNewSessionDt")).Enabled = true;
                ((DropDownList)r.FindControl("ddlNewSessionPeriod")).Enabled = true;
            }

            ((bundle_edit_sessions)this.Page).showCurrentTabsNPills();
        }

        
    }
}