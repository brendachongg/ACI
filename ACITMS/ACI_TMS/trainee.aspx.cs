using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class trainee : BasePage
    {
        public const string PAGE_NAME = "trainee.aspx";

        public trainee()
            : base(PAGE_NAME, AccessRight_Constance.TRAINEE_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    loadTrainee();
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving list of trainee");
            }
        }


        //Load applicant by today or view all
        private void loadTrainee()
        {
            Trainee_Management tm = new Trainee_Management();
            gvTrainee.VirtualItemCount = tm.getTotalCount();

            DataTable dt = tm.getListOfTrainee(0);

            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();
        }

        protected void gvTrainee_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTrainee.PageIndex = e.NewPageIndex;
            Trainee_Management tm = new Trainee_Management();
            DataTable dt = tm.getListOfTrainee(e.NewPageIndex);
            gvTrainee.DataSource = dt;
            gvTrainee.DataBind();

            //gvTrainee.DataSource = ViewState["listOfTrainee"] as DataTable;
            //gvTrainee.DataBind();

        }

        protected void gvTrainee_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("viewTraineeDetail"))
            {
                string selectedTraineeID = e.CommandArgument.ToString();
                Response.Redirect(trainee_details.PAGE_NAME + "?" + trainee_details.TRAINEE_QUERY + "=" + selectedTraineeID);
            }
        }
        protected void btnSearchApplicant_Click(object sender, EventArgs e)
        {
            gvTrainee.DataSource = null;
            gvTrainee.DataBind();
            gvTrainee.AllowCustomPaging = false;
            gvTrainee.AllowPaging = false;

            if (tbSearchApplicant.Text.Trim().Equals(""))
            {
                gvTrainee.AllowCustomPaging = true;
                gvTrainee.AllowPaging = true;
                loadTrainee();
         
            }
            else
            {
                Trainee_Management tm = new Trainee_Management();
               
                DataTable dtTrainee = tm.getListOfTraineeByValue(tbSearchApplicant.Text);
              
                //ViewState["listOfTrainee"] = dtTrainee;
       
                gvTrainee.DataSource = dtTrainee;
                gvTrainee.DataBind();
            }
        }

    }
}