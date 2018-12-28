using System;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;

namespace GeneralLayer
{
    public class Session_Handler
    {
        //Set staff account onto constance session
        public void setStaffAccountSession(DataTable dtStaffAccount)
        {
            Cryptography decryptContent = new Cryptography();

            foreach (DataRow dr in dtStaffAccount.Rows)
            {
                //dr["userName"] = decryptContent.decryptInfo(dr["userName"].ToString());
                dr["userEmail"] = decryptContent.decryptInfo(dr["userEmail"].ToString());
            }

            HttpContext.Current.Session["dtStaffAccount"] = dtStaffAccount;
        }

        public void setApplicationDetails(DataTable dtDecryptedApplicantionDetails)
        {
            HttpContext.Current.Session["dtApplicationDetails"] = dtDecryptedApplicantionDetails;
        }

        //Clear off all sessions
        public void clearAllSession()
        {
            HttpContext.Current.Session.Clear();
        }
    }
}
