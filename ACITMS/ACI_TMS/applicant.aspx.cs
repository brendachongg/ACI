using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;
using System.Drawing;
using System.Web.UI.HtmlControls;

namespace ACI_TMS
{
    public partial class applicant : BasePage
    {
        public const string PAGE_NAME = "applicant.aspx";

        public applicant()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    loadBlackList();
                    loadApplicant();                   
                    ViewState["CheckTab"] = "1";
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving list of applicant");            
            }
        }

        private void loadBlackList()
        {
            Blacklist_Management bm = new Blacklist_Management();
            ViewState["BlackList"] = bm.getSuspensionListWithinPeriod();

        }

        //Load applicant by today or view all
        private void loadApplicant()
        {
            Applicant_Management am = new Applicant_Management();
            Tuple<DataTable, DataTable, int, int> applicantTuple = am.getListOfApplicant();

            lbTodayBadge.Text = applicantTuple.Item3.ToString();
            lbOtherDatesBadge.Text = applicantTuple.Item4.ToString();

            ViewState["todayApplication"] = applicantTuple.Item2;
            ViewState["otherDayApplication"] = applicantTuple.Item1;

            gvApplicant.DataSource = applicantTuple.Item2;
            gvApplicant.DataBind();
        }

        protected void gvApplicant_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvApplicant.PageIndex = e.NewPageIndex;
            gvApplicant.DataBind();

            if (ViewState["CheckTab"].ToString() == "1")
            {
                last24();
            }

            if (ViewState["CheckTab"].ToString() == "2")
            {
                after24();
            }


        }

        protected void gvApplicant_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewApplicantDetails"))
            {
                string selectedApplicantId = e.CommandArgument.ToString();

                Response.Redirect(applicant_details.PAGE_NAME + "?" + applicant_details.APPLICANT_QUERY + "=" + selectedApplicantId);
            }
        }

        protected void btnLast24_ServerClick(object sender, EventArgs e)
        {
            gvApplicant.PageIndex = 0;

            last24();
        }

        private void last24()
        {
            ViewState["CheckTab"] = "1";

            gvApplicant.DataSource = null;
            gvApplicant.DataBind();

            btnAfter24.Attributes.Add("class", "btn btn-default");
            btnLast24.Attributes.Add("class", "btn btn-default btn-info");

            gvApplicant.DataSource = ViewState["todayApplication"] as DataTable;
            gvApplicant.DataBind();
        }

        protected void btnAfter24_ServerClick(object sender, EventArgs e)
        {
            gvApplicant.PageIndex = 0;

            after24();
        }

        private void after24()
        {
            ViewState["CheckTab"] = "2";

            gvApplicant.DataSource = null;
            gvApplicant.DataBind();

            btnAfter24.Attributes.Add("class", "btn btn-default btn-info");
            btnLast24.Attributes.Add("class", "btn btn-default");


            gvApplicant.DataSource = ViewState["otherDayApplication"] as DataTable;
            gvApplicant.DataBind();
        }

        protected void btnSearchApplicant_Click(object sender, EventArgs e)
        {
            Applicant_Management am = new Applicant_Management();
            DataTable dtApplicant = null;

            if (tbSearchApplicant.Text.Trim().Equals(""))
            {
                if (ViewState["CheckTab"].ToString() == "1")
                {
                    dtApplicant = ViewState["todayApplication"] as DataTable;
                }
                else if (ViewState["CheckTab"].ToString() == "2")
                {
                    dtApplicant = ViewState["otherDayApplication"] as DataTable;
                }

            }
            else
            {
                dtApplicant = am.getListOfApplicantByValue(tbSearchApplicant.Text);
            }


            btnAfter24.Attributes.Add("class", "btn btn-default");
            btnLast24.Attributes.Add("class", "btn btn-default");

            gvApplicant.DataSource = dtApplicant;
            gvApplicant.DataBind();
        }

        protected void gvApplicant_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                HiddenField hdfBlacklistStatus = (HiddenField)e.Row.FindControl("hdfBlacklisted");
                LinkButton lkbtnId = (LinkButton)e.Row.FindControl("lkbtnApplicantId");
                Label lbApplicantID = (Label)e.Row.FindControl("lbgvIdentity");

                DataTable dt = ViewState["BlackList"] as DataTable;

                Cryptography crypt = new Cryptography();

                DataRow[] result = dt.Select("idNumber = '" + crypt.encryptInfo(lbApplicantID.Text) + "'");

                if (result.Length > 0)
                {
                    e.Row.ForeColor = Color.Red;
                    lkbtnId.ForeColor = Color.Red;
                }

                if (hdfBlacklistStatus.Value.Equals(General_Constance.STATUS_YES))
                {
                    e.Row.ForeColor = Color.Red;
                    lkbtnId.ForeColor = Color.Red;
                }
            }
        }







    }
}