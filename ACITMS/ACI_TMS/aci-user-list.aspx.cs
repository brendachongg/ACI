using System;
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
    public partial class aci_user_list : BasePage
    {
        public const string PAGE_NAME = "aci-user-list.aspx";
        ACI_Staff_User aciStaffUser = new ACI_Staff_User();
        private bool canDelete = false;
        public aci_user_list()
            : base(PAGE_NAME, new string[] { AccessRight_Constance.ACI_USER_NEW, AccessRight_Constance.ACI_USER_VIEW, AccessRight_Constance.ACI_USER_DELETE })
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadStaffs();

                if (checkAccessRights(AccessRight_Constance.ACI_USER_NEW))
                {
                    panelNewUser.Visible = true;
                }
                else
                {
                    panelNewUser.Visible = false;
                }

                if (!checkAccessRights(AccessRight_Constance.ACI_USER_VIEW))
                {
                    redirectToErrorPg("You have not access right to this module");
                }

            }

        }

        private void loadStaffs()
        {
            DataTable dtStaffs = aciStaffUser.getAllStaff();
            ViewState["AllStaffs"] = dtStaffs;
            gvStaffs.DataSource = dtStaffs;
            gvStaffs.DataBind();
        }

        protected void gvStaffs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.ToString().Equals("viewStaffDetails"))
            {
                string selectedStaffId = e.CommandArgument.ToString();
                Response.Redirect(aci_user_view_edit.PAGE_NAME + "?" + aci_user_view_edit.ACI_USER_QUERY + "=" + selectedStaffId);
            }
        }

        protected void gvStaffs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (checkAccessRights(AccessRight_Constance.ACI_USER_DELETE))
            {
                if (e.Row.RowType == DataControlRowType.DataRow)
                {
                    if (e.Row.FindControl("lbtnDelSub") != null)
                    {

                        ((Label)e.Row.FindControl("lbtnDelSub")).Attributes["onClick"] = "showDelDialog(" + ((DataRowView)e.Row.DataItem)["userid"].ToString() + ");";

                    }
                }
            }
            else
            {
                if (e.Row.RowType == DataControlRowType.DataRow || e.Row.RowType == DataControlRowType.Header)
                {
                    e.Row.Cells[e.Row.Cells.Count - 1].Visible = false;
                }
            }
        }

        protected void btnDelSub_Click(object sender, EventArgs e)
        {
            if ((int.Parse(hfSelSub.Value) == LoginID))
            {
                lblError.Text = "You cannot delete your own account.";
                panelError.Visible = true;
            }
            else
            {
                Tuple<bool, string> status = aciStaffUser.deleteACIStaff(int.Parse(hfSelSub.Value), LoginID);

                if (status.Item1)
                {
                    lblSuccess.Text = status.Item2;
                    panelSuccess.Visible = true;

                    loadStaffs();
                }
                else
                {
                    lblError.Text = status.Item2;
                    panelError.Visible = true;
                }
            }
        }

        private void findStaffs(string value)
        {
            DataTable dtStaffs = aciStaffUser.findACIStaffs(value);
            gvStaffs.DataSource = dtStaffs;
            gvStaffs.DataBind();
        }

        protected void btnSearchStaff_Click(object sender, EventArgs e)
        {
            if (tbSearchStaff.Text.Trim().Equals(""))
            {
                loadStaffs();
            }
            else
            {
                findStaffs(tbSearchStaff.Text.Trim());
            }
        }

        protected void lkbAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(aci_user_new.PAGE_NAME);
        }

        protected void gvStaffs_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvStaffs.PageIndex = e.NewPageIndex;
            gvStaffs.DataBind();

            gvStaffs.DataSource = ViewState["AllStaffs"] as DataTable;
            gvStaffs.DataBind();
        }

        protected void gvStaffs_RowCreated(object sender, GridViewRowEventArgs e)
        {
            //if (!checkAccessRights(AccessRight_Constance.ACI_USER_DELETE))
            //{
            //    e.Row.Cells[5].Visible = false;
            //}
        }


    }
}