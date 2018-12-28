using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ACI_TMS
{
    public partial class programme_view : BasePage
    {
        public const string PAGE_NAME = "programme-view.aspx";
        public const string QUERY_ID = "pId";

        private Programme_Management pm = new Programme_Management();
        private Bundle_Management bm = new Bundle_Management();

        public programme_view()
            : base(PAGE_NAME, AccessRight_Constance.PROGRAM_VIEW, programme_management.PAGE_NAME)
        {

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null)
                {
                    redirectToErrorPg("Missing programme information.");
                    return;
                }

                hfProgrammeId.Value = Request.QueryString[QUERY_ID];

                loadProgrammeDetails();
            }
            else
            {
                panelError.Visible = false;
                panelSuccess.Visible = false;
            }
        }

        private void loadProgrammeDetails()
        {
            DataTable dtProgrammeStructure = pm.getProgrammeDetails(hfProgrammeId.Value);

            if (dtProgrammeStructure == null || dtProgrammeStructure.Rows.Count == 0)
            {
                redirectToErrorPg("Missing programme information");
                return;
            }

            foreach (DataRow dr in dtProgrammeStructure.Rows)
            {
                if (dr["programmeImage"] != DBNull.Value)
                {
                    MemoryStream ms = new MemoryStream((byte[])dr["programmeImage"]);
                    BinaryReader br = new BinaryReader(ms);
                    byte[] bytes = br.ReadBytes((Int32)ms.Length);

                    if (bytes.Length > 0)
                    {
                        string imgString = Convert.ToBase64String(bytes, 0, bytes.Length);
                        imgProgrammeImage.ImageUrl = "data:image/jpeg;base64," + imgString;
                        imgProgrammeImage.Visible = true;
                        lbProgrammeImageEmpty.Visible = false;
                    }
                    else
                    {
                        imgProgrammeImage.Visible = false;
                        lbProgrammeImageEmpty.Visible = true;
                    }
                }
                else
                {
                    imgProgrammeImage.Visible = false;
                    lbProgrammeImageEmpty.Visible = true;
                }

                lbProgrammeCategoryValue.Text = dr["programmeCategoryDisplay"].ToString();
                lbProgrammeCodeValue.Text = dr["programmeCode"].ToString();
                lbProgrammeLevelValue.Text = dr["programmeLevelDisplay"].ToString();
                lbCourseCodeValue.Text = dr["courseCode"].ToString();
                lbProgrammeTitleValue.Text = dr["programmeTitle"].ToString();
                lbProgrammeTypeValue.Text = dr["programmeTypeDisp"].ToString();
                lbProgrammeVersionValue.Text = dr["programmeVersion"].ToString();
                lbNumofSOAValue.Text = dr["numberOfSOA"].ToString();
                lbSSGRefNumValue.Text = dr["SSGRefNum"].ToString();
                lbProgrammeDescriptionValue.Text = dr["programmeDescription"].ToString();
                lbBundleCodeValue.Text = dr["bundleCode"].ToString();
                hfBundleId.Value = dr["bundleId"].ToString();
            }

            loadBundleDetails(int.Parse(hfBundleId.Value));
        }

        private void loadBundleDetails(int bundleId)
        {
            DataTable dt = bm.getBundleModule(bundleId, true);
            ViewState["dtModule"] = dt;

            if (dt == null || dt.Rows.Count == 0)
            {
                redirectToErrorPg("Missing bundle information");
                return;
            }

            gvModules.DataSource = ViewState["dtModule"] as DataTable;
            gvModules.DataBind();
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            lbtnBack_Click(sender, e);
        }

        protected void btnUpdateNew_Click(object sender, EventArgs e)
        {
            Response.Redirect(programme_edit.PAGE_NAME + "?" + programme_edit.QUERY_ID + "=" + hfProgrammeId.Value + "&" + programme_edit.NEW_VER + "=Y");
        }

        protected void btnUpdateCurrent_Click(object sender, EventArgs e)
        {
            Response.Redirect(programme_edit.PAGE_NAME + "?" + programme_edit.QUERY_ID + "=" + hfProgrammeId.Value + "&" + programme_edit.NEW_VER + "=N");
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            Tuple<bool, string> status = pm.deleteProgramme(int.Parse(hfProgrammeId.Value), lbProgrammeCodeValue.Text, LoginID);

            if (status.Item1)
            {
                lblSuccess.Text = status.Item2;
                panelSuccess.Visible = true;
                btnConfirmDel.Visible = false;
                btnUpdateCurrent.Visible = false;
                btnUpdateNew.Visible = false;
            }
            else
            {
                lblError.Text = status.Item2;
                panelError.Visible = true;
            }

        }
    }
}