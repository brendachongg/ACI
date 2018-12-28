using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using GeneralLayer;
using LogicLayer;
using System.Globalization;

namespace ACI_TMS
{
    public partial class subsidy_management : BasePage
    {
        public const string PAGE_NAME = "subsidy-management.aspx";

        private const string GV_DATA = "dt";
        private const string SUBTYPE_DATA = "subType";

        private bool canEdit = false, canDelete = false;

        public subsidy_management()
            : base(PAGE_NAME, AccessRight_Constance.SUBSIDY_VIEW)
        {
            
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!checkAccessRights(AccessRight_Constance.SUBSIDY_NEW)) panelNewSub.Visible = false;

                getSubsidy();
                loadSubsidyTypes();
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = false;
            }

            programme_search.selectProgramme += new programme_search.SelectProgramme(selectProgramme);
            programme_search.programmeRefresh += new programme_search.ProgrammeRefresh(programmeRefresh);
        }

        private void loadSubsidyTypes()
        {
            DataTable dt = (new Finance_Management()).getSubsidyTypes();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving subsidy types.");
                return;
            }

            ViewState[SUBTYPE_DATA] = dt;

            ddlType.DataSource = dt;
            ddlType.DataBind();
            ddlType.Items.Insert(0, new ListItem("--Select--", ""));
        }


        private void getSubsidy()
        {
            DataTable dt = (new Finance_Management()).getAllSubsidy();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving subsidies.");
                return;
            }

            //format programme code
            foreach (DataRow dr in dt.Rows)
                if (dr["programmeCode"] != DBNull.Value) dr["programmeCode"] = "(" + dr["programmeCode"].ToString() + ")";

            ViewState[GV_DATA] = dt;

            loadSubsidy();
        }

        private void loadSubsidy()
        {
            canEdit = checkAccessRights(AccessRight_Constance.SUBSIDY_EDIT);
            canDelete = checkAccessRights(AccessRight_Constance.SUBSIDY_DEL);

            gvSub.Columns[5].Visible = canEdit || canDelete;
            gvSub.DataSource = ViewState[GV_DATA] as DataTable;
            gvSub.DataBind();
        }

        private void selectProgramme(int id, string code, string title)
        {
            tbProgCode.Text = code;
            tbProgTitle.Text = title;
            hfProgId.Value = id.ToString();
        }

        private void programmeRefresh()
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showNew", "showNew();", true);
        }

        protected void btnDelSub_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).delSubsidy(int.Parse(hfSelSub.Value), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                getSubsidy();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void gvSub_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvSub.EditIndex = e.NewEditIndex;
            loadSubsidy();
        }

        protected void gvSub_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvSub.EditIndex = -1;
            loadSubsidy();
        }

        protected void gvSub_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int selectedRow = e.RowIndex;
            GridViewRow row = (GridViewRow)gvSub.Rows[selectedRow];

            int id = (int)gvSub.DataKeys[selectedRow].Value;
            DateTime dtEff = DateTime.ParseExact(((TextBox)row.FindControl("tbDt")).Text, "dd MMM yyyy", CultureInfo.InvariantCulture);
            DropDownList ddl = ((DropDownList)row.FindControl("ddlType"));
            decimal value = decimal.Parse(((TextBox)row.FindControl("tbValue")).Text);

            Tuple<bool, string> status = (new Finance_Management()).updateSubsidy(id, ddl.SelectedValue, value, dtEff, LoginID);

            if (status.Item1)
            {
                DataTable dt = ViewState[GV_DATA] as DataTable;
                DataRow dr = dt.Select("subsidyId=" + id.ToString())[0];
                dr["effectiveDate"] = dtEff;
                dr["effectiveDateDisp"] = dtEff.ToString("dd MMM yyyy");
                dr["subsidyType"] = ddl.SelectedValue;
                dr["subsidyTypeDisp"] = ddl.SelectedItem.Text;
                dr["subsidyValue"] = value;
                dr["subsidyValueDisp"] = value.ToString("#,##0.00");
                gvSub.EditIndex = -1;
                loadSubsidy();

                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void gvSub_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if ((e.Row.RowState & DataControlRowState.Edit) > 0)
            {
                DropDownList ddl = (e.Row.FindControl("ddlType") as DropDownList);
                ddl.DataSource = ViewState[SUBTYPE_DATA] as DataTable;
                ddl.DataBind();
                
                DataRowView dr = e.Row.DataItem as DataRowView;
                ddl.SelectedValue = dr["subsidyType"].ToString();
            }
            else
            {
                if (e.Row.FindControl("lbtnDelSub") != null)
                {
                    if (canDelete) ((Label)e.Row.FindControl("lbtnDelSub")).Attributes["onClick"] = "showDelDialog(" + ((DataRowView)e.Row.DataItem)["subsidyId"].ToString() + ");";
                    else e.Row.FindControl("lbtnDelSub").Visible = false;
                }

                if (!canEdit && e.Row.FindControl("lbtnEdit") != null)
                    e.Row.FindControl("lbtnEdit").Visible = false;

                //if scheme is in use, do not allow to edit
                if ((int)((DataRowView)e.Row.DataItem)["isUsed"] > 0) e.Row.FindControl("lbtnEdit").Visible = false;
            }
        }

        protected void gvSub_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSub.PageIndex = e.NewPageIndex;
            loadSubsidy();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).addSubsidy(tbScheme.Text, hfProgId.Value == "" ? -1 : int.Parse(hfProgId.Value), ddlType.SelectedValue, decimal.Parse(tbValue.Text),
                DateTime.ParseExact(tbDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                getSubsidy();
                btnClear_Click(null, null);
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
                Page.ClientScript.RegisterStartupScript(this.GetType(), "showNew", "showNew();", true);
            }
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            hfProgId.Value = "";
            tbProgCode.Text = "";
            tbProgTitle.Text = "";
            tbDt.Text = "";
            tbScheme.Text = "";
            tbValue.Text = "";
            ddlType.SelectedIndex = 0;
        }
    }
}