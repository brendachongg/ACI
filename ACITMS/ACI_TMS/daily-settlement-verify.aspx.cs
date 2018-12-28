using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LogicLayer;
using GeneralLayer;
using System.Data;

namespace ACI_TMS
{
    public partial class daily_settlement_verify : BasePage
    {
        public const string PAGE_NAME = "daily-settlement-verify.aspx";

        Daily_Settlement_Management dsm = new Daily_Settlement_Management();

        public daily_settlement_verify()
            : base(PAGE_NAME, AccessRight_Constance.DAILY_PAYMENT)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                gvPendingSettlementRecs.DataSource = dsm.getPendingSettlementRecords();
                gvPendingSettlementRecs.DataBind();
            }
        }

        protected void gvPendingSettlementRecs_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName.Equals("Select"))
            {
                int dailysettlementid = int.Parse(e.CommandArgument.ToString());
                gvConfirm.DataSource = dsm.getSettlementRecords(dailysettlementid);
                gvConfirm.DataBind();
                ViewState["dailysettlementid"] = dailysettlementid;

            }
        }

        protected void btnVerify_Click(object sender, EventArgs e)
        {
            dsm.confirmSettlement(LoginID, (int)ViewState["dailysettlementid"]);
        }

        protected void btnReject_Click(object sender, EventArgs e)
        {
            dsm.rejectSettlement(LoginID, (int)ViewState["dailysettlementid"], tbRejectRemarks.Text);
        }
    }
}