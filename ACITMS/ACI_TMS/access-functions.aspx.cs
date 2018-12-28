using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace ACI_TMS
{
    public partial class access_functions : BasePage
    {
        public const string PAGE_NAME = "access-functions.aspx";

        private const string GV_DATA = "data";
        private const string GRP_DATA = "grp";

        private Access_Control_Management acm=new Access_Control_Management();

        public access_functions()
            : base(PAGE_NAME, AccessRight_Constance.ACCESS_FUNCT)
        {
            
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataTable dt = acm.getAllFunctionGrps();
                if (dt == null)
                {
                    redirectToErrorPg("Error retrieving function groups.");
                    return;
                }
                ViewState[GRP_DATA] = dt;

                loadFunctions(); 
                loadFunctionGrps();  
            }

            successDiv.Visible = false;
            failureDiv.Visible = false;
        }
        
        protected void loadFunctionGrps() 
        {
            ddlGrp.DataSource = ViewState[GRP_DATA] as DataTable;
            ddlGrp.DataBind();
            ddlGrp.Items.Insert(0, new ListItem("--Select--", ""));
        }

        protected void loadFunctions() 
        {
            DataTable dt = acm.getFunctions(); 
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving functions.");
                return;
            }
            ViewState[GV_DATA] = dt;
            FunctionsGridView.DataSource = dt;
            FunctionsGridView.DataBind();
        }

        //clearing all the fields
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ddlGrp.SelectedIndex = 0;
            tbNewFunction.Text = "";
            tbNewDesc.Text = "";
        }

        protected void FunctionsGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FunctionsGridView.PageIndex = e.NewPageIndex;
            FunctionsGridView.DataSource = ViewState[GV_DATA] as DataTable;
            FunctionsGridView.DataBind();
        }

        protected void FunctionsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            FunctionsGridView.EditIndex = e.NewEditIndex;
            FunctionsGridView.DataSource = ViewState[GV_DATA] as DataTable;
            FunctionsGridView.DataBind();
        }

        //when update is clicked
        protected void FunctionsGridView_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int selectedRow = e.RowIndex;
            GridViewRow row = (GridViewRow)FunctionsGridView.Rows[selectedRow];

            int functionId = (int)FunctionsGridView.DataKeys[selectedRow].Value;          
            string functionGrp = ((DropDownList)row.FindControl("ddlEdit")).SelectedValue; //get the codeValue of the row of data
            string desc = ((TextBox)row.FindControl("tbDescription")).Text;

            if (acm.updateFunction(functionGrp, functionId, desc, LoginID))
            {
                lblSuccessMsg.Text = "Function saved successfully.";
                successDiv.Visible = true;
                FunctionsGridView.EditIndex = -1;
                loadFunctions();
            }
            else
            {
                lblError.Text = "Error saving function.";
                failureDiv.Visible = true;
            }
        }

        protected void FunctionsGridView_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            FunctionsGridView.EditIndex = -1;
            FunctionsGridView.DataSource = ViewState[GV_DATA] as DataTable;
            FunctionsGridView.DataBind();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = acm.addFunction(ddlGrp.SelectedValue, tbNewFunction.Text, tbNewDesc.Text, LoginID);

            if (status.Item1)
            {
                lblSuccessMsg.Text = status.Item2;
                successDiv.Visible = true;

                ddlGrp.SelectedIndex = 0;
                tbNewDesc.Text = "";
                tbNewFunction.Text = "";

                loadFunctions();
            }
            else
            {
                lblError.Text = status.Item2;
                failureDiv.Visible = true;
            }
        }


        protected void FunctionsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList ddl = (e.Row.FindControl("ddlEdit") as DropDownList);
                ddl.DataSource = ViewState[GRP_DATA] as DataTable;
                ddl.DataBind();

                DataRowView dr = e.Row.DataItem as DataRowView;
                ddl.SelectedValue = dr["codeValue"].ToString();
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            if (acm.deleteFunction(int.Parse(HiddenFunctionId.Value), LoginID))
            {
                successDiv.Visible = true;
                lblSuccessMsg.Text = "Function removed successfully.";
                loadFunctions();
            }
            else
            {
                failureDiv.Visible = true;
                lblError.Text = "Error removing function.";
            }
        }

    }
}