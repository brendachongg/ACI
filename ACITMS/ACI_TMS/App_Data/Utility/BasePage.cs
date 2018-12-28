using GeneralLayer;
using LogicLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace ACI_TMS
{
    public class BasePage : System.Web.UI.Page
    {
        public const string ERROR_DESC_KEY = "ErrorDesc";
        public const string ERROR_TITLE_KEY = "ErrorTitle";
        public const string ERROR_GOTO_KEY = "ReturnPage";
        public const string ERROR_PAGE = "~/error.aspx";

        private int _loginId;
        private string _loginName;
        private string _pgName;
        //the dict value: -1 unknown, 0 no access, 1 has access
        private Dictionary<string, int> _access = null;
        private string _prevPg;
        private bool _toCheckLogin;
        private string _errorPgTitle = null;
        private DataTable dtStaffAccount;

        protected Log_Handler lh=new Log_Handler();

        //if want to check login but not access right, pass in null for function input parameter
        public BasePage(string name, string function, string prevPg = null)
            : base()
        {
            _pgName = name;
            _prevPg = prevPg;
            if (function != null)
            {
                _access = new Dictionary<string, int>();
                _access.Add(function, -1);
            }
            _toCheckLogin = true;
        }

        public BasePage(string name, string[] function, string prevPg = null)
            : base()
        {
            _pgName = name;
            _prevPg = prevPg;
            if (function != null)
            {
                _access = new Dictionary<string, int>();
                foreach (string f in function) _access.Add(f, -1);
            }
            _toCheckLogin = true;
        }

        //this constructor is for pages who do not need to check login and access right
        public BasePage(string name)
        {
            _pgName = name;
            _toCheckLogin = false;
        }

        protected void Page_PreInit(object sender, EventArgs e)
        {
            if (!_toCheckLogin) return;

            checkLogin();

            //access check
            if (!IsPostBack && _access != null && _access.Count != 0 && !hasAnyAccess())
            {
                redirectToErrorPg("You do not have the rights to access this page.");
                return;
            }
        }

        private bool hasAnyAccess()
        {
            foreach (string f in _access.Keys.ToList())
            {
                if (checkAccessRights(f)) return true;
            }

            return false;
        }

        public bool checkLogin()
        {
            
            dtStaffAccount = Session["dtStaffAccount"] as DataTable;

            //check if user has login, if not direct to login page
            if (dtStaffAccount == null)
            {
                Response.Redirect(aci_staff_login.PAGE_NAME);
                return false;
            }

            _loginId = int.Parse(dtStaffAccount.Rows[0]["userId"].ToString());
            _loginName = dtStaffAccount.Rows[0]["userName"].ToString();

            return true;
        }

        public string PageName
        {
            get { return _pgName; }
        }

        public string[] Function
        {
            get { return _access.Keys.ToArray(); }
        }

        public string PrevPage
        {
            get { return _prevPg; }
            set { _prevPg = value; }
        }

        public string ErrorPageTitle
        {
            get { return _errorPgTitle; }
            set { _errorPgTitle = value; }
        }

        public int LoginID
        {
            get { return _loginId; }
        }

        public string LoginName
        {
            get { return _loginName; }
        }

        protected bool checkAccessRights(string function)
        {
            try
            {
                //search through the dict to see if previously check so as to prevent accessing the database again
                if (_access.ContainsKey(function) && _access[function] != -1) return _access[function] == 1 ? true : false;
                else
                {
                    bool hasAccess = (new Access_Control_Management()).checkAccessRight(_loginId, function);
                    if (_access.ContainsKey(function)) _access[function] = hasAccess ? 1 : 0; else _access.Add(function, hasAccess ? 1 : 0);

                    return hasAccess;
                }
            }
            catch (Exception ex)
            {
                log("CheckAccessRights", "Error checking access right", ex);
                redirectToErrorPg("Error checking access.");
                return false;
            }
        }

        public void redirectToErrorPg(string desc, string nxtPg = null)
        {
            Session[ERROR_DESC_KEY] = desc;

            if (nxtPg == null || nxtPg == "")
            {
                if (_prevPg == null || _prevPg == "")
                {
                    Session[ERROR_GOTO_KEY] = dashboard.PAGE_NAME;
                }
                else Session[ERROR_GOTO_KEY] = _prevPg;
            }
            else Session[ERROR_GOTO_KEY] = nxtPg;

            if (_errorPgTitle != null) Session[ERROR_TITLE_KEY] = _errorPgTitle;

            Server.Transfer(ERROR_PAGE);
        }

        protected void log(string source, string msg, string sysMsg = "")
        {
            lh.WriteLog(sysMsg, _pgName, source, msg, _loginId);
        }

        protected void log(string source, string msg, Exception ex)
        {
            lh.WriteLog(ex, _pgName, source, msg, _loginId);
        }

        protected void lbtnBack_Click(object sender, EventArgs e)
        {
            if (_prevPg == null || _prevPg == "")
            {
                Response.Redirect(dashboard.PAGE_NAME);
                return;
            }else Response.Redirect(_prevPg);
        }

        //to catch any unhandled error
        private void Page_Error(object sender, EventArgs e)
        {
            // Get last error from the server
            Exception exc = Server.GetLastError();

            log("Page_Error", "Unhandled error caught." + exc.Message, exc);
            redirectToErrorPg("An error has occurred.");

            // Clear the error from the server
            Server.ClearError();
        }
    }
}