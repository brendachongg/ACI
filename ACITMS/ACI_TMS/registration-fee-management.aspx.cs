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
using System.Globalization;

namespace ACI_TMS
{
    public partial class registration_fee_management : BasePage
    {
        public const string PAGE_NAME = "registration-fee-management.aspx";

        private const string GV_DATA = "data";
        
        private bool canEdit = false, canDelete = false;

        public registration_fee_management()
            : base(PAGE_NAME, AccessRight_Constance.REGFEE_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) 
            {
                if (!checkAccessRights(AccessRight_Constance.REGFEE_NEW)) panelNewReg.Visible = false;

                getRegFee();
            }
            else
            {
                panelSuccess.Visible = false;
                panelError.Visible = false;
            }
        }

        private void getRegFee()
        {
            DataTable dt = (new Finance_Management()).getAllRegFee();
            if (dt == null)
            {
                redirectToErrorPg("Error retrieving registration fees.");
                return;
            }

            ViewState[GV_DATA] = dt;

            loadRegFee();
        }

        private void loadRegFee()
        {
            canEdit = checkAccessRights(AccessRight_Constance.REGFEE_EDIT);
            canDelete = checkAccessRights(AccessRight_Constance.REGFEE_DEL);

            gvReg.Columns[2].Visible = canEdit || canDelete;
            gvReg.DataSource = ViewState[GV_DATA] as DataTable;
            gvReg.DataBind();
        }

        protected void gvReg_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvReg.PageIndex = e.NewPageIndex;
            loadRegFee();
        }

        protected void gvReg_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvReg.EditIndex = e.NewEditIndex;
            loadRegFee();
        }

        protected void gvReg_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvReg.EditIndex = -1;
            loadRegFee();
        }

        protected void gvReg_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            int selectedRow = e.RowIndex;
            GridViewRow row = (GridViewRow)gvReg.Rows[selectedRow];

            decimal fee = decimal.Parse(((TextBox)row.FindControl("tbFee")).Text);
            int id = (int)gvReg.DataKeys[selectedRow].Value;

            Tuple<bool, string> status = (new Finance_Management()).updateRegFee(id, fee, LoginID);

            if (status.Item1)
            {
                DataTable dt = ViewState[GV_DATA] as DataTable;
                DataRow dr = dt.Select("feeId=" + id.ToString())[0];
                dr["registrationFee"] = fee;
                dr["registrationFeeDisp"] = fee.ToString("#,##0.00");
                gvReg.EditIndex = -1;
                loadRegFee();

                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).addRegFee(DateTime.ParseExact(tbDt.Text, "dd MMM yyyy", CultureInfo.InvariantCulture), 
                decimal.Parse(tbFee.Text), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                getRegFee();
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
            tbDt.Text = "";
            tbFee.Text = "";
        }

        protected void btnDelReg_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = (new Finance_Management()).delRegFee(int.Parse(hfSelFee.Value), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;

                getRegFee();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void gvReg_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (e.Row.FindControl("lbtnDelReg") != null)
            {
                if (canDelete) ((Label)e.Row.FindControl("lbtnDelReg")).Attributes["onClick"] = "showDelDialog(" + ((DataRowView)e.Row.DataItem)["feeId"].ToString() + ");";
                else e.Row.FindControl("lbtnDelReg").Visible = false;
            }

            if (!canEdit && e.Row.FindControl("lbtnEdit") != null)
                e.Row.FindControl("lbtnEdit").Visible = false;
        }
    }
}