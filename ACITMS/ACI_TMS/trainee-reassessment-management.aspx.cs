﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;

namespace ACI_TMS
{
    public partial class trainee_reassessment_management : BasePage
    {
        public const string PAGE_NAME = "trainee-reassessment-management.aspx";

        private const string GV_DATA = "data";

        public trainee_reassessment_management()
            : base(PAGE_NAME, AccessRight_Constance.REPEATMOD_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                //load everyting
                searchReassessment();
            }
        }

        private void searchReassessment()
        {
            DataTable dt = (new Assessment_Management()).searchReassessmentTrainees(ddlSearch.SelectedValue == "" ? null : ddlSearch.SelectedValue, txtSearch.Text);

            ViewState[GV_DATA] = dt;
            gvReAssessment.DataSource = dt;
            gvReAssessment.DataBind();
        }

        protected void gvReAssessment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReAssessment.DataSource = ViewState[GV_DATA] as DataTable;
            gvReAssessment.PageIndex = e.NewPageIndex;
            gvReAssessment.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchReassessment();
        }

        protected void btnListAll_Click(object sender, EventArgs e)
        {
            ddlSearch.SelectedIndex = 0;
            txtSearch.Text = "";
            searchReassessment();
        }


        protected void gvReAssessment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_DATA] as DataTable;

                //string traineeId = ((HiddenField)gvReAssessment.Rows[index].Cells[0].FindControl("hfTrainee")).Value;
                //string batchModuleId = ((HiddenField)gvReAssessment.Rows[index].Cells[0].FindControl("hfBatchModule")).Value;

                Server.Transfer(trainee_reassessment_view.PAGE_NAME + "?" + trainee_reassessment_view.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["traineeId"].ToString()) + "&"
                    + trainee_reassessment_view.BATCH_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["batchModuleId"].ToString()));
            }
        }

        
    }
}