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
    public partial class programme_edit : BasePage
    {
        public const string PAGE_NAME = "programme-edit.aspx";
        public const string QUERY_ID = "pId";
        public const string NEW_VER = "newVer";

        private Programme_Management pm = new Programme_Management();
        private Bundle_Management bm = new Bundle_Management();

        public programme_edit()
            : base(PAGE_NAME, AccessRight_Constance.PROGRAM_EDIT, programme_view.PAGE_NAME)
        {
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            PrevPage = PrevPage + "?" + programme_view.QUERY_ID + "=" + hfProgrammeId.Value;

            if (!IsPostBack)
            {
                if (Request.QueryString[QUERY_ID] == null || Request.QueryString[QUERY_ID] == "")
                {
                    redirectToErrorPg("Missing programme information.", programme_management.PAGE_NAME);
                    return;
                }

                hfProgrammeId.Value = Request.QueryString[QUERY_ID];
                hfEditNew.Value = Request.QueryString[NEW_VER];
                loadProgrammeDetails();
            }
            else
            {
                panelSysError.Visible = false;
                panelSuccess.Visible = false;
            }

            bundlesearch.selectBundle += new bundle_search.SelectBundle(selectBundle);
        }
        private void selectBundle(int bundleId, string bundleCode)
        {
            txtBundle.Text = bundleCode;
            hfBundleId.Value = bundleId.ToString();

            Bundle_Management pm = new Bundle_Management();
            DataTable dt = pm.getBundleModule(bundleId);

            if (dt == null)
            {
                //redirect to error page
                redirectToErrorPg("Error retrieving modules.");
                return;
            }

            gvModules.DataSource = dt;
            gvModules.DataBind();
            gvModules.Visible = true;
        }

        private void loadProgrammeDetails()
        {
            DataTable dtProgrammeStructure = pm.getProgrammeDetails(hfProgrammeId.Value);
            DataTable dtProgrammeLevel = pm.getProgrammeLevel();
            DataTable dtProgrammeCategory = pm.getProgrammeCategory();
            DataTable dtProgrammeType = pm.getProgrammeType();

            if (dtProgrammeStructure == null || dtProgrammeStructure.Rows.Count == 0)
            {
                redirectToErrorPg("Missing programme information");
                return;
            }

            //Programme Type
            ddlProgrammeType.DataSource = dtProgrammeType;
            ddlProgrammeType.DataBind();

            //Programme Level
            ddlProgrammeLevel.DataSource = dtProgrammeLevel;
            ddlProgrammeLevel.DataBind();
            //ddlProgrammeLevel.Items.Insert(0, new ListItem("Not Applicable", "NA"));

            //Programme Category
            ddlProgrammeCategory.DataSource = dtProgrammeCategory;
            ddlProgrammeCategory.DataBind();

            foreach (DataRow dr in dtProgrammeStructure.Rows)
            {
                if (hfEditNew.Value == "Y")
                {
                    lbProgrammeVersionValue.Text = dr["maxProgVer"].ToString();
                }
                else
                {
                    lbProgrammeVersionValue.Text = dr["programmeVersion"].ToString();
                }

                if (dr["programmeImage"] != DBNull.Value)
                {
                    MemoryStream ms = new MemoryStream((byte[])dr["programmeImage"]);
                    BinaryReader br = new BinaryReader(ms);
                    byte[] bytes = br.ReadBytes((Int32)ms.Length);

                    if (bytes.Length > 0)
                    {
                        string imgString = Convert.ToBase64String(bytes, 0, bytes.Length);
                        imgProgrammeImage.ImageUrl = "data:image/jpeg;base64," + imgString;
                        imgProgrammeImage.Style.Remove("display");
                        lbProgrammeImageEmpty.Style.Add("display", "none");
                    }
                    else
                    {
                        imgProgrammeImage.Style.Add("display", "none");
                        lbProgrammeImageEmpty.Style.Remove("display");
                    }
                }
                else
                {
                    imgProgrammeImage.Style.Add("display", "none");
                    lbProgrammeImageEmpty.Style.Remove("display");
                }

                ddlProgrammeCategory.SelectedValue = dr["programmeCategory"].ToString();
                lbProgrammeCodeValue.Text = dr["programmeCode"].ToString();
                ddlProgrammeLevel.SelectedValue = dr["programmeLevel"].ToString();
                lbCourseCodeValue.Text = dr["courseCode"].ToString();
                tbProgrammeTitle.Text = dr["programmeTitle"].ToString();
                ddlProgrammeType.SelectedValue = dr["programmeType"].ToString();
                tbNumofSOA.Text = dr["numberOfSOA"].ToString();
                tbSSGRefNum.Text = dr["SSGRefNum"].ToString();
                tbProgrammeDescription.Text = dr["programmeDescription"].ToString();
                txtBundle.Text = dr["bundleCode"].ToString();
                hfBundleId.Value = dr["bundleId"].ToString();
            }

            DataTable dtBatchProgramme = pm.validateProgrammeUsed(int.Parse(hfProgrammeId.Value));

            if (hfEditNew.Value == "N")
            {
                if (dtBatchProgramme.Rows.Count != 0)
                {
                    ddlProgrammeCategory.Enabled = false;
                    ddlProgrammeLevel.Enabled = false;
                    ddlProgrammeType.Enabled = false;
                    tbNumofSOA.Enabled = false;
                    iconSearchPkg.Visible = false;
                }
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

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (hfEditNew.Value == "Y")
            {
                updateNew();
            }
            else
            {
                updateCurrent();
            }
        }

        private void updateNew()
        {
            byte[] imgByte = null;
            Stream s = fileUploadProgrammeImage.PostedFile.InputStream;
            BinaryReader reader = new BinaryReader(s);
            imgByte = reader.ReadBytes((int)s.Length);

            Tuple<bool, string, int> updateProgramme = pm.updateProgrammeNewVer(lbProgrammeCodeValue.Text, lbCourseCodeValue.Text, ddlProgrammeLevel.SelectedValue, Convert.ToInt32(lbProgrammeVersionValue.Text), ddlProgrammeCategory.SelectedValue,
                tbProgrammeTitle.Text, tbProgrammeDescription.Text, Convert.ToInt32(tbNumofSOA.Text), tbSSGRefNum.Text, int.Parse(hfBundleId.Value), ddlProgrammeType.SelectedValue, imgByte, int.Parse(hfProgrammeId.Value), LoginID);

            if (updateProgramme.Item1)
            {
                hfProgrammeId.Value = updateProgramme.Item3.ToString();
                lblSuccess.Text = updateProgramme.Item2;
                panelSuccess.Visible = true;
                hfEditNew.Value = "N";
            }

            else
            {
                lblSysError.Text = updateProgramme.Item2;
                panelSysError.Visible = true;
            }
        }

        private void updateCurrent()
        {
            byte[] imgByte = null;
            Stream s = fileUploadProgrammeImage.PostedFile.InputStream;
            BinaryReader reader = new BinaryReader(s);
            imgByte = reader.ReadBytes((int)s.Length);

            Tuple<bool, string> updateProgramme;

            updateProgramme = pm.updateProgramme(Convert.ToInt32(hfProgrammeId.Value), lbProgrammeCodeValue.Text, lbCourseCodeValue.Text, Convert.ToInt32(lbProgrammeVersionValue.Text),
                ddlProgrammeLevel.SelectedValue, ddlProgrammeCategory.SelectedValue, tbProgrammeTitle.Text, tbProgrammeDescription.Text,
                Convert.ToInt32(tbNumofSOA.Text), tbSSGRefNum.Text, int.Parse(hfBundleId.Value), ddlProgrammeType.SelectedValue, imgByte, LoginID);

            if (updateProgramme.Item1)
            {
                lblSuccess.Text = updateProgramme.Item2;
                panelSuccess.Visible = true;
            }

            else
            {
                lblSysError.Text = updateProgramme.Item2;
                panelSysError.Visible = true;
            }
        }

        protected void btnClearProgramme_Click(object sender, EventArgs e)
        {
            loadProgrammeDetails();
        }
    }
}