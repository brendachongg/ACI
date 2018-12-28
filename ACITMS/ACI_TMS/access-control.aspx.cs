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
    public partial class access_control : BasePage
    {
        public const string PAGE_NAME = "access-control.aspx";

        private Access_Control_Management acm = new Access_Control_Management();
        private ACI_Staff_User staffUser = new ACI_Staff_User();

        public access_control()
            : base(PAGE_NAME, AccessRight_Constance.ACCESS_CONTROL)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                populateStaff();

                loadAccessFunctions();
            }

            successMessage.Visible = false;
            failMessage.Visible = false;
        }

        private void populateStaff()
        {
            DataTable dt = staffUser.getStaff();
            if (dt == null) 
            {
                redirectToErrorPg("Error retrieving staff.");
                return;
            }

            ddlStaff.DataSource = dt;
            ddlStaff.DataBind();
            ddlStaff.Items.Insert(0, new ListItem("--- Select ---", ""));
            ddlStaff.SelectedIndex = 0;
        }

        private void loadAccessFunctions()
        {
            DataTable dt = acm.getAllFunctionGrps(); 
            rptGroup.DataSource = dt; 
            rptGroup.DataBind();
        }
       
        protected void btnGrant_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var item = (RepeaterItem)btn.NamingContainer;
            var cbl = (CheckBoxList)item.FindControl("CheckBoxListFunctions");
            foreach (ListItem li in cbl.Items)
            {
                li.Selected = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "expand", "showSection(" + ((HiddenField)item.FindControl("hfIndex")).Value + ");", true);
        }

        protected void btnRevoke_Click(object sender, EventArgs e)
        {
            var btn = (Button)sender;
            var item = (RepeaterItem)btn.NamingContainer;
            var cbl = (CheckBoxList)item.FindControl("CheckBoxListFunctions");
            foreach (ListItem li in cbl.Items)
            {
                li.Selected = false;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "expand", "showSection(" + ((HiddenField)item.FindControl("hfIndex")).Value + ");", true);
        }


        protected void rptGroup_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            DataTable dt = acm.getFunctionByGroup(((HiddenField)e.Item.FindControl("hfGetGroupValue")).Value);

            CheckBoxList cbl = (CheckBoxList)e.Item.FindControl("CheckBoxListFunctions");

            cbl.DataSource = dt;
            cbl.DataBind();
        }

        protected void saveBtn_Click(object sender, EventArgs e)
        {
            List<int> selFunct = new List<int>();
            List<int> toExpand = new List<int>();
            int i = 0;
            bool hasAccess = false;

            foreach (RepeaterItem item in rptGroup.Items)
            {
                hasAccess = false;

                CheckBoxList chkItem = (CheckBoxList)item.FindControl("CheckBoxListFunctions");
                foreach (ListItem li in chkItem.Items)
                {
                    if (li.Selected == true)
                    {
                        selFunct.Add(int.Parse(li.Value));
                        hasAccess = true;
                    }
                }

                if (hasAccess) toExpand.Add(i);

                i++;
            }

            if (acm.assignAccessRights(selFunct, int.Parse(ddlStaff.SelectedValue), LoginID))
            {
                successMessage.Visible = true;
                lblSuccess.Text = "Access rights saved successfully.";
            }
            else
            {
                failMessage.Visible = true;
                lblFailure.Text = "Error saving access rights.";
            }

            string js = "";
            foreach (int index in toExpand)
            {
                js += "showSection(" + index + ");";
            }
            if (js != "") Page.ClientScript.RegisterStartupScript(this.GetType(), "expand", js, true);
        }

        protected void ddlStaff_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlStaff.SelectedValue == "")
            {
                foreach (RepeaterItem r in rptGroup.Items)
                {
                    CheckBoxList cbl = (CheckBoxList)r.FindControl("CheckBoxListFunctions");
                    foreach (ListItem li in cbl.Items) li.Selected = false;
                }

                return;
            }

            DataTable dt = acm.getUserAccess(int.Parse(ddlStaff.SelectedValue));
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving user access.");
                return;
            }

            List<int> access = new List<int>();
            List<int> toExpand = new List<int>();
            foreach (DataRow dr in dt.Rows) access.Add((int)dr["FunctionId"]);

            int i = 0;
            bool hasAccess = false;
            foreach (RepeaterItem r in rptGroup.Items)
            {
                hasAccess = false;
                CheckBoxList cbl = (CheckBoxList)r.FindControl("CheckBoxListFunctions");
                foreach (ListItem li in cbl.Items)
                {
                    if (access.Exists(x => x == int.Parse(li.Value))) 
                    { 
                        li.Selected = true;
                        hasAccess = true;
                    }
                    else li.Selected = false;
                }

                if (hasAccess) toExpand.Add(i);

                i++;
            }

            string js = "";
            foreach(int index in toExpand)
            {
                js += "showSection(" + index + ");";
            }
            if (js != "") Page.ClientScript.RegisterStartupScript(this.GetType(), "expand", js, true);
        }


    }
}