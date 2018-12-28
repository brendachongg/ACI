using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using System.Data;
using GeneralLayer;

namespace ACI_TMS
{
    public partial class payment_management : BasePage
    {
        public const string PAGE_NAME = "payment-management.aspx";

        private const string GV_DATA = "data";

        public payment_management()
            : base(PAGE_NAME, AccessRight_Constance.PAYMT_VIEW)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Form.DefaultButton = btnSearch.UniqueID;

            if (!IsPostBack)
            {
                if (Request.QueryString["OUT"] != null) ddlSearchType.SelectedValue = "OUT"; 
                else tbSearch.Text = DateTime.Now.ToString("dd MMM yyyy");
                searchPayments();
            }

            //cos the date field is dynamically created at runtime, so everytime the page load need to recreate again
            Page.ClientScript.RegisterStartupScript(this.GetType(), "formatField", "formatSearchTxt();", true);
        }

        private void searchPayments()
        {
            DataTable dt = (new Finance_Management()).searchPayments(ddlSearchType.SelectedValue, tbSearch.Text);
            if (dt == null)
            {
                redirectToErrorPg("Error searching payments.");
                return;
            }

            ViewState[GV_DATA] = dt;           
            gvPayment.DataSource = dt;
            gvPayment.DataBind();
            //check if user has the right to delete payments, if not hide the delete column
            gvPayment.Columns[4].Visible = checkAccessRights(AccessRight_Constance.PAYMT_DEL);

            if (ddlSearchType.SelectedValue=="OUT")
                Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void gvPayment_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPayment.DataSource = ViewState[GV_DATA] as DataTable;
            gvPayment.PageIndex = e.NewPageIndex;
            gvPayment.DataBind();

            if (ddlSearchType.SelectedValue == "OUT")
                Page.ClientScript.RegisterStartupScript(this.GetType(), "disValidators", "hideError();", true);
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            searchPayments();
        }

        protected void gvPayment_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "select")
            {
                //when there is paging,  the dataitem index refers to the row index of the entire dataset that it is binded to, not the index that it appear on the gridview
                int index = int.Parse(e.CommandArgument.ToString());
                DataTable dt = ViewState[GV_DATA] as DataTable;

                string pType = dt.Rows[index]["paymentType"].ToString();
                if (pType == PaymentType.BOTH.ToString() || pType == PaymentType.REG.ToString() || pType == PaymentType.PROG.ToString())
                {
                    Server.Transfer(applicant_programme_payment.PAGE_NAME + "?" + (dt.Rows[index]["userType"].ToString() == "A" ? applicant_programme_payment.APPLICANT_QUERY : applicant_programme_payment.TRAINEE_QUERY) 
                        + "=" + HttpUtility.UrlEncode(dt.Rows[index]["userId"].ToString()) + "&" + applicant_programme_payment.TYPE_QUERY + "=" + HttpUtility.UrlEncode(pType));
                }
                else if (pType == PaymentType.MAKEUP.ToString()) 
                {
                    Server.Transfer(absentee_payment.PAGE_NAME + "?" + absentee_payment.TRAINEE_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["userId"].ToString()) + "&"
                        + absentee_payment.NAME_QUERY + "=" + HttpUtility.UrlEncode(dt.Rows[index]["fullName"].ToString()));
                }

                //TODO: handle other types of payment
            }
        }

        protected void gvPayment_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            if (!e.Row.Cells[4].Visible) return;

            string payType = ((DataRowView)e.Row.DataItem)["paymentType"].ToString();
            if (((DataRowView)e.Row.DataItem)["canDel"].ToString() == "N")
            {
                //if the record comes from trainee and is a reg or class fee payment or is outstanding payment without any payment at all, 
                //cannot allow user to delete the entire payment
                e.Row.FindControl("lbtnDelPayment").Visible = false;
            }
            else
            {
                int id = -1;
                if (((DataRowView)e.Row.DataItem)["paymentId"] != DBNull.Value) id = (int)((DataRowView)e.Row.DataItem)["paymentId"];

                ((Label)e.Row.FindControl("lbtnDelPayment")).Attributes["onClick"] = "showDelDialog(" + id + ", '" + ((DataRowView)e.Row.DataItem)["paymentType"].ToString() + "', '"
                    + ((DataRowView)e.Row.DataItem)["userId"].ToString() + "');";
            }
        }

        protected void btnDelPayment_Click(object sender, EventArgs e)
        {
            string pType = hfSelPayType.Value;
            Tuple<bool, string> status = null;
            if (pType == PaymentType.BOTH.ToString() || pType == PaymentType.REG.ToString()
                    || pType == PaymentType.PROG.ToString())
            {
                status = (new Finance_Management()).delApplnClassPayment(hfSelPayUser.Value, (PaymentType)Enum.Parse(typeof(PaymentType), pType), LoginID);   
            }
            else if (pType == PaymentType.MAKEUP.ToString())
            {
                status = (new Finance_Management()).delMakeupPayment(int.Parse(hfSelPayId.Value), LoginID);
            }

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                searchPayments();
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }
        }

        protected void lkbtnMakeupPayment_Click(object sender, EventArgs e)
        {
            Server.Transfer(absentee_payment.PAGE_NAME);
        }

        
    }
}