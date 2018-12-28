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
    public partial class applicant_module_exemption : BasePage
    {
        public const string PAGE_NAME = "applicant-module-exemption.aspx";

        public const string STATUS_EXEMPTED = "exempted";
        public const string STATUS_TAKING = "taking";

        public applicant_module_exemption()
            : base(PAGE_NAME, AccessRight_Constance.APPLN_EDIT)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    string applicantId = Request.QueryString["a"];
                    string programmeBatchId = Request.QueryString["pb"];

                    if (!applicantId.Equals("") && !programmeBatchId.Equals(""))
                    {
                        lbApplicantId.Text = applicantId;
                        loadApplicantExemptedModule(applicantId);
                        loadCourseBatchModule(programmeBatchId);
                    }
                }
            }
            catch (Exception ex)
            {
                log("Page_Load()", ex.Message, ex);
                redirectToErrorPg("Error retrieving exemption modules");
            }
        }

        private void loadApplicantExemptedModule(string applicantId)
        {
            Applicant_Management am = new Applicant_Management();
            List<string> exemptedModule = am.getApplicantExemptedModule(applicantId);

            ViewState["listExemptedModule"] = exemptedModule;
        }

        private void loadCourseBatchModule(string programmeBatchId)
        {
            Batch_Session_Management cbm = new Batch_Session_Management();
            DataTable dt = cbm.getBatchModuleByProgrammeBatchId(programmeBatchId);

            rptModuleExamptedSelection.DataSource = dt;
            rptModuleExamptedSelection.DataBind();

        }

        protected void rptModuleExamptedSelection_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            LinkButton lkbtnExemption = (LinkButton)e.Item.FindControl("lkbtnExemptedSelection");

            string moduleId = lkbtnExemption.CommandName;


            if (ViewState["listExemptedModule"] != null)
            {
                List<string> exemptedModule = ViewState["listExemptedModule"] as List<string>;

                foreach (string item in exemptedModule)
                {
                    if (item.Equals(moduleId))
                    {
                        lkbtnExemption.CssClass = "fa fa-circle fa-2x";
                        lkbtnExemption.CommandArgument = STATUS_EXEMPTED;
                    }
                    //else
                    //{
                    //    lkbtnExemption.CssClass = "fa fa-check-circle fa-2x";
                    //    lkbtnExemption.CommandArgument = STATUS_TAKING;
                    //}
                }
            }
            else
            {
                lkbtnExemption.CssClass = "fa fa-check-circle fa-2x";
                lkbtnExemption.CommandArgument = STATUS_TAKING;
            }
        }

        protected void rptModuleExamptedSelection_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            LinkButton lkbtnExemption = (LinkButton)e.Item.FindControl("lkbtnExemptedSelection");
            string moduleStatus = lkbtnExemption.CommandArgument;
            string moduleId = lkbtnExemption.CommandName;

            //List<string> exemptedModule = new List<string>();
            List<string> exemptedModule = ViewState["listExemptedModule"] as List<string>;

            if (exemptedModule != null)
            {
                if (moduleStatus.Equals(STATUS_EXEMPTED))
                {
                    exemptedModule.Remove(moduleId);
                    lkbtnExemption.CssClass = "fa fa-check-circle fa-2x";
                    lkbtnExemption.CommandArgument = STATUS_TAKING;
                }
                else if (moduleStatus.Equals(STATUS_TAKING))
                {
                    exemptedModule.Add(moduleId);
                    lkbtnExemption.CssClass = "fa fa-circle fa-2x";
                    lkbtnExemption.CommandArgument = STATUS_EXEMPTED;
                }

                ViewState["listExemptedModule"] = exemptedModule;
            }


            if (exemptedModule == null)
            {
                List<string> newExemptedModule = new List<string>();

                if (moduleStatus.Equals(STATUS_TAKING))
                {
                    newExemptedModule.Add(moduleId);
                    lkbtnExemption.CssClass = "fa fa-circle fa-2x";
                    lkbtnExemption.CommandArgument = STATUS_EXEMPTED;
                }
                ViewState["listExemptedModule"] = newExemptedModule;
            }

        }

        protected void lkbtnSave_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            string concatedExemptedModule = "";

            List<string> exemptedModule = ViewState["listExemptedModule"] as List<string>;

            if (exemptedModule.Count > 0)
            {
                for (int i = 0; i < exemptedModule.Count; i++)
                {
                    if (i == exemptedModule.Count - 1)
                    {
                        concatedExemptedModule += exemptedModule[i].ToString();
                    }
                    else
                    {
                        concatedExemptedModule += exemptedModule[i].ToString() + ";";
                    }
                    
                }
            }

            decimal totalModuleCost = 0;

            for (int i = 0; i < rptModuleExamptedSelection.Items.Count; i++)
            {
                Label moduleCost = (Label)rptModuleExamptedSelection.Items[i].FindControl("lbModuleCost");
                LinkButton lkbtnExemption = (LinkButton)rptModuleExamptedSelection.Items[i].FindControl("lkbtnExemptedSelection");
                string moduleStatus = lkbtnExemption.CommandArgument;

                if (moduleStatus.Equals(STATUS_TAKING))
                {
                    totalModuleCost += Convert.ToDecimal(moduleCost.Text.Remove(0,1));
                }

            }

            Applicant_Management am = new Applicant_Management();
            int userId = LoginID;
            am.updateApplicantExemptedModule(applicantId, concatedExemptedModule, totalModuleCost, userId);

            Response.Redirect(applicant_details.PAGE_NAME + "?a=" + applicantId);
        }

        protected void lkbtnBack_Click(object sender, EventArgs e)
        {
            string applicantId = lbApplicantId.Text;
            Response.Redirect(applicant_details.PAGE_NAME + "?a=" + applicantId);
        }



    }
}