using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class batch_view : BasePage
    {
        public const string PAGE_NAME = "batch-view.aspx";
        public const string QUERY_ID = "id";

        private DataTable dtSession;
        private Batch_Session_Management bsm = new Batch_Session_Management();

        public batch_view()
            : base(PAGE_NAME, AccessRight_Constance.BATCH_VIEW, batch_management.PAGE_NAME)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null)
                {
                    redirectToErrorPg("Missing class information.");
                    return;
                }

                hfBatchId.Value = HttpUtility.UrlDecode(Request.QueryString[QUERY_ID]);

                loadProgrammeDetails();
                loadModuleSessionDetails();

                //decide if edit and/or delete button should be available
                bool isStarted=bsm.isBatchStarted(int.Parse(hfBatchId.Value), BatchDateType.COMMENCEMENT);
                if (!checkAccessRights(AccessRight_Constance.BATCH_EDIT) || isStarted) btnEdit.Visible = false;
                if (!checkAccessRights(AccessRight_Constance.BATCH_DEL) || isStarted) btnConfirmDel.Visible = false;
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadProgrammeDetails()
        {
            DataTable dt = bsm.getBatchDetails(int.Parse(hfBatchId.Value));
            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving class details.");
                return;
            }

            DataRow dr = dt.Rows[0];
            lbProgrammeCategory.Text = dr["programmeCategoryDisp"].ToString();
            lbProgrammeLevel.Text = dr["programmeLevelDisp"].ToString();
            lbProgramme.Text = dr["programmeTitle"].ToString();
            lbProgrammeVersion.Text = dr["programmeVersion"].ToString();
            lbProgrammeCode.Text = dr["programmeCode"].ToString();
            lbProgrammeType.Text = dr["programmeTypeDisp"].ToString();

            lbBatchCode.Text = dr["batchCode"].ToString();
            lbClsType.Text = dr["batchTypeDisp"].ToString();
            lbProjCode.Text = dr["projectCode"].ToString();
            lbRegStartDate.Text = dr["programmeRegStartDateDisp"].ToString();
            lbRegEndDate.Text = dr["programmeRegEndDateDisp"].ToString();
            lbBatchStartDate.Text = dr["programmeStartDateDisp"].ToString();
            lbBatchEndDate.Text = dr["programmeCompletionDateDisp"].ToString();
            lbCapacity.Text = dr["batchCapacity"].ToString();
            lbMode.Text = dr["classModeDisp"].ToString();
        }

        private void loadModuleSessionDetails()
        {
            Tuple<DataTable, DataTable> t = bsm.getBatchModulesNSessions(int.Parse(hfBatchId.Value));

            if (t.Item1 == null || t.Item1.Rows.Count == 0 || t.Item2 == null || t.Item2.Rows.Count == 0)
            {
                redirectToErrorPg("Error retrieving class's module and session details.");
                return;
            }

            rpModuleTabs.DataSource = t.Item1;
            rpModuleTabs.DataBind();

            dtSession = t.Item2;
            rpModuleContent.DataSource = t.Item1;
            rpModuleContent.DataBind();

            //set the first module tab to show
            ((HtmlGenericControl)rpModuleTabs.Items[0].FindControl("tabMod")).Attributes.Add("class", "active");
            hfSelModule.Value = t.Item1.Rows[0]["moduleId"].ToString();
            Page.ClientScript.RegisterStartupScript(this.GetType(), "showFirstModule", "showModule('" + hfSelModule.Value + "');", true);
        }

        protected void rpModuleContent_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            GridView gv = e.Item.FindControl("gvSessions") as GridView;
            gv.DataSource = dtSession.Select("batchModuleId=" + ((DataRowView)e.Item.DataItem)["batchModuleId"].ToString()).CopyToDataTable();
            gv.DataBind();
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            Server.Transfer(batch_edit.PAGE_NAME + "?" + batch_edit.QUERY_ID + "=" + HttpUtility.UrlEncode(hfBatchId.Value));
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = bsm.deleteBatch(int.Parse(hfBatchId.Value), LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnConfirmDel.Visible = false;
                btnEdit.Visible = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "showSelModule", "showModule('" + hfSelModule.Value + "');", true);
        }

    }
}