using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace GeneralLayer
{
    public class Utlity_Handler
    {
        public int compute_cents(decimal dollar_amount)
        {
            return (int)(dollar_amount * 100);
        }
    }
}