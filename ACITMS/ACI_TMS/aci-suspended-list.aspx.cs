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

namespace ACI_TMS
{
    public partial class aci_suspended_list : BasePage
    {
        public const string PAGE_NAME = "aci-suspended-list.aspx";

        public aci_suspended_list()
            : base(PAGE_NAME, AccessRight_Constance.SUSPEND_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {                    
                    loadSuspendedList();
                }
                
                
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving suspended list.");
            }
        }

        //Load suspended applicant from WDA and ACI
        private void loadSuspendedList()
        {
            gvSuspended.DataSource = null;
            gvSuspended.DataBind();

            Blacklist_Management bm = new Blacklist_Management();
            DataTable dtSuspendedList = bm.getSuspendedList();
            ViewState["dtSuspendedList"] = dtSuspendedList;
     
            gvSuspended.DataSource = dtSuspendedList;
            gvSuspended.DataBind();
        }

        //Search with NRIC or Name
        protected void btnSearchApplicant_Click(object sender, EventArgs e)
        {
            gvSuspended.DataSource = null;
            gvSuspended.DataBind();

            Blacklist_Management bm = new Blacklist_Management();
            if (tbSearchApplicant.Text.Trim().Equals(""))
            {
                DataTable dtSuspendedList = bm.getSuspendedList();
                ViewState["dtSuspendedList"] = dtSuspendedList;

                gvSuspended.DataSource = dtSuspendedList;
                gvSuspended.DataBind();
            } else{
                DataTable dtSuspendedList = bm.getSuspendedListByValue(tbSearchApplicant.Text);
                ViewState["dtSuspendedList"] = dtSuspendedList;

                gvSuspended.DataSource = dtSuspendedList;
                gvSuspended.DataBind();
            }
          
        }

        protected void gvSuspended_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSuspended.PageIndex = e.NewPageIndex;
            gvSuspended.DataBind();

            gvSuspended.DataSource = ViewState["dtSuspendedList"] as DataTable;
            gvSuspended.DataBind();
        }

        protected void lnkDelete(Object sender, CommandEventArgs e)
        {
            string suspendedId = e.CommandArgument.ToString();
            Blacklist_Management bm = new Blacklist_Management();

            bool success = bm.removeFromSuspension(suspendedId);

            if (success)
            {
                loadSuspendedList();
            }
        }

        //protected void gvSuspended_RowCommand(object sender, GridViewCommandEventArgs e)
        //{
        //    if (e.CommandName.Equals("Unsuspend"))
        //    {
        //       // LinkButton btn = (LinkButton)(sender);
        //        string suspendedId = e.CommandArgument.ToString();
        //        Blacklist_Management bm = new Blacklist_Management();

        //        bool success = bm.removeFromSuspension(suspendedId);

        //        if (success)
        //        {
        //            loadSuspendedList();
        //        }
        //    }
            //if (e.CommandName.Equals("changeDetails"))
            //{
            //    int rowIndex = Convert.ToInt32(e.CommandArgument);
            //    HiddenField hRemarks = (HiddenField)gvSuspended.Rows[rowIndex].FindControl("hdfRemarks");
            //    Label lIdentityNumber = (Label)gvSuspended.Rows[rowIndex].FindControl("lbgvIdentity");
            //    Label lName = (Label)gvSuspended.Rows[rowIndex].FindControl("lbgvFullName");

            //    tbNRICValue.Text = lIdentityNumber.Text;
            //    tbNameValue.Text = lName.Text;
            //    tbRemarksValue.Text = hRemarks.Value;
                
            //    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "openModel", "openModel()", true);
            //}

            //if (e.CommandName.Equals("remove"))
            //{
            //    int rowIndex = Convert.ToInt32(e.CommandArgument);
            //    Label lIdentityNumber = (Label)gvSuspended.Rows[rowIndex].FindControl("lbgvIdentity");


            //}
        //}

        //protected void gvSuspended_RowDataBound(object sender, GridViewRowEventArgs e)
        //{

        //}

        //protected void btnRemoveSuspend_Click(object sender, EventArgs e)
        //{
        //    string selectedIdNumber = tbNRICValue.Text;
        //    Blacklist_Management bm = new Blacklist_Management();

        //    bool success = bm.removeFromSuspension(selectedIdNumber);

        //    if (success)
        //    {
        //        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "closeModel", "closeModel()", true);
        //        Page.Response.Redirect(Page.Request.Url.ToString(), true);
        //        loadSuspendedList();
        //    }
            
        //}

        protected void btnAddSuspend_Click(object sender, EventArgs e)
        {
            Response.Redirect(aci_suspension.PAGE_NAME);
        }
    }
}