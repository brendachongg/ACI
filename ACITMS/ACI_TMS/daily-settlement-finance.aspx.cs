using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GeneralLayer;

namespace ACI_TMS
{
    public partial class daily_settlement_finance : BasePage
    {
         public const string PAGE_NAME = "daily-settlement-finance.aspx";
         public daily_settlement_finance()
            : base(PAGE_NAME, AccessRight_Constance.DAILY_PAYMENT)
        {

        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}