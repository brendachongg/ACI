using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeneralLayer;
using LogicLayer;
using System.Data;

namespace ACI_TMS
{
    public partial class aci_dashboard : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                loadUsername();
                loadDashboardMenu();
            }
        }

        //Display username
        protected void loadUsername()
        {
            try
            {
                if (!IsPostBack)
                {
                    if (HttpContext.Current.Session["dtStaffAccount"] != null)
                    {
                        DataTable dtStaffAccount = HttpContext.Current.Session["dtStaffAccount"] as DataTable;

                        if (dtStaffAccount.Rows.Count > 0)
                        {
                            lbUserName.Text = dtStaffAccount.Rows[0]["userName"].ToString();
                        }
                    }
                    else
                    {
                        Response.Redirect(aci_staff_login.PAGE_NAME);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Response.Redirect(aci_staff_login.PAGE_NAME);
            }
            
        }

        private void loadDashboardMenu()
        {
            List<string> access = (new Access_Control_Management()).getUserAccessFunctName(int.Parse(((DataTable)Session["dtStaffAccount"]).Rows[0]["userId"].ToString()));

            List<Menu_Link_Model> lmList = new List<Menu_Link_Model>();

            lmList.Add(new Menu_Link_Model(dashboard.PAGE_NAME, "fa fa-dashboard fa-fw", "Dashboard"));
            if (access.Contains(AccessRight_Constance.APPLN_VIEW)) lmList.Add(new Menu_Link_Model(applicant.PAGE_NAME, "user", "Applicants"));

            if (access.Contains(AccessRight_Constance.ENROLLLETTER_GEN) || access.Contains(AccessRight_Constance.ENROLLLETTER_VIEW) || access.Contains(AccessRight_Constance.ENROLLTEMPL_VIEW))
                lmList.Add(new Menu_Link_Model(enrollment_letter_managment.PAGE_NAME, "envelope", "Enrollment Letter"));

            if (access.Contains(AccessRight_Constance.SUSPEND_VIEW)) lmList.Add(new Menu_Link_Model(aci_suspended_list.PAGE_NAME, "bolt", "Suspend"));
            if (access.Contains(AccessRight_Constance.TRAINEE_VIEW)) lmList.Add(new Menu_Link_Model(trainee.PAGE_NAME, "users", "Trainee"));
            if (access.Contains(AccessRight_Constance.REG_NEW)) lmList.Add(new Menu_Link_Model(applicant_quick_registration.PAGE_NAME, "edit", "Registration"));
            if (access.Contains(AccessRight_Constance.PAYMT_VIEW)) lmList.Add(new Menu_Link_Model(payment_management.PAGE_NAME, "euro", "Payment"));
            if (access.Contains(AccessRight_Constance.REGFEE_VIEW)) lmList.Add(new Menu_Link_Model(registration_fee_management.PAGE_NAME, "cog", "Registration Fee"));
            if (access.Contains(AccessRight_Constance.SUBSIDY_VIEW)) lmList.Add(new Menu_Link_Model(subsidy_management.PAGE_NAME, "cog", "Subsidy"));         
            if (access.Contains(AccessRight_Constance.MODULE_VIEW)) lmList.Add(new Menu_Link_Model(module_management.PAGE_NAME, "book", "Module"));
            if (access.Contains(AccessRight_Constance.BUNDLE_VIEW)) lmList.Add(new Menu_Link_Model(bundle_management.PAGE_NAME, "th-large", "Bundle"));
            if (access.Contains(AccessRight_Constance.PROGRAM_VIEW)) lmList.Add(new Menu_Link_Model(programme_management.PAGE_NAME, "university", "Programme"));
            if (access.Contains(AccessRight_Constance.BATCH_VIEW)) lmList.Add(new Menu_Link_Model(batch_management.PAGE_NAME, "graduation-cap", "Class"));
            if (access.Contains(AccessRight_Constance.SESSION_VIEW)) lmList.Add(new Menu_Link_Model(session_management.PAGE_NAME, "th-list", "Session"));
            if (access.Contains(AccessRight_Constance.ATTENDANCE_VIEW) || access.Contains(AccessRight_Constance.ATTENDANCE_VIEW_ALL)) lmList.Add(new Menu_Link_Model(attendance_management.PAGE_NAME, "tags", "Attendance"));
            if (access.Contains(AccessRight_Constance.MAKEUP_VIEW)) lmList.Add(new Menu_Link_Model(absentee_management.PAGE_NAME, "calendar", "Make-up"));
            if (access.Contains(AccessRight_Constance.ASSESSMENT_VIEW)) lmList.Add(new Menu_Link_Model(assessment_management.PAGE_NAME, "book", "Assessment"));
            if (access.Contains(AccessRight_Constance.REASSESSMENT_VIEW)) lmList.Add(new Menu_Link_Model(trainee_reassessment_management.PAGE_NAME, "repeat", "Re-Assessment"));
            if (access.Contains(AccessRight_Constance.REPEATMOD_VIEW)) lmList.Add(new Menu_Link_Model(trainee_repeat_module_management.PAGE_NAME, "refresh", "Repeat Module"));
            if (access.Contains(AccessRight_Constance.SITIN_VIEW)) lmList.Add(new Menu_Link_Model(sitin_management.PAGE_NAME, "paperclip", "No SOA"));
            if (access.Contains(AccessRight_Constance.SOA_VIEW)) lmList.Add(new Menu_Link_Model(soa_view.PAGE_NAME, "inbox", "SOA"));
            if (access.Contains(AccessRight_Constance.VENUE_VIEW)) lmList.Add(new Menu_Link_Model(venue_management.PAGE_NAME, "home", "Venue"));
            if (access.Contains(AccessRight_Constance.BOOKING_VIEW)) lmList.Add(new Menu_Link_Model(booking_management.PAGE_NAME, "map-marker", "Booking"));
            if (access.Contains(AccessRight_Constance.REPORT_ACIMTHLY) || access.Contains(AccessRight_Constance.REPORT_SSGKPI) || access.Contains(AccessRight_Constance.REPORT_FEEGRANT)
                || access.Contains(AccessRight_Constance.REPORT_FULLQUALQT) || access.Contains(AccessRight_Constance.REPORT_WTSDISB) || access.Contains(AccessRight_Constance.REPORT_SFCDISB)
                || access.Contains(AccessRight_Constance.REPORT_ALLS) || access.Contains(AccessRight_Constance.REPORT_QPO) || access.Contains(AccessRight_Constance.REPORT_FEECOLLECT)
                || access.Contains(AccessRight_Constance.REPORT_FEETALLY) || access.Contains(AccessRight_Constance.REPORT_NETS) || access.Contains(AccessRight_Constance.REPORT_TRAINEE)) 
                lmList.Add(new Menu_Link_Model(report_management.PAGE_NAME, "files-o", "Reports"));
            if (access.Contains(AccessRight_Constance.ACI_USER_VIEW)) lmList.Add(new Menu_Link_Model(aci_user_list.PAGE_NAME, "users", "ACI Users"));
            if (access.Contains(AccessRight_Constance.ACCESS_CONTROL)) lmList.Add(new Menu_Link_Model(access_control.PAGE_NAME, "lock", "Access Control"));
            if (access.Contains(AccessRight_Constance.ACCESS_FUNCT)) lmList.Add(new Menu_Link_Model(access_functions.PAGE_NAME, "newspaper-o", "Access Function"));
            if (access.Contains(AccessRight_Constance.CASELOG_VIEW)) lmList.Add(new Menu_Link_Model(case_log_management.PAGE_NAME, "wrench", "Case Log"));
            if (access.Contains(AccessRight_Constance.AUDIT_VIEW)) lmList.Add(new Menu_Link_Model(audit_trail_view.PAGE_NAME, "database", "Audit Trail"));
            if (access.Contains(AccessRight_Constance.DATA_MIGRATION)) lmList.Add(new Menu_Link_Model(data_migration.PAGE_NAME, "database", "Data Migration"));
            if (access.Contains(AccessRight_Constance.DATA_ANALYTICS)) lmList.Add(new Menu_Link_Model(data_analytics.PAGE_NAME, "database", "Generate Data For Tableau"));
            if (access.Contains(AccessRight_Constance.DAILY_PAYMENT)) lmList.Add(new Menu_Link_Model(daily_payment.PAGE_NAME, "money", "Daily Payment"));      
            repeaterDashboardMenu.DataSource = lmList;
            repeaterDashboardMenu.DataBind();
        }

        protected void lkBtnLogout_Click(object sender, EventArgs e)
        {
            Session_Handler sh = new Session_Handler();
            sh.clearAllSession();

            Response.Redirect(aci_staff_login.PAGE_NAME);
        }
    }
}