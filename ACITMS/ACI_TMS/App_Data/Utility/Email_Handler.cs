using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;
using System.Configuration;

namespace GeneralLayer
{
    public class Email_Handler
    {
        public bool SendEmail(string frm, string to, string[] cc, string[] bcc, string subject, string body)
        {
            return SendEmail(frm, new string[] { to }, cc, bcc, subject, body);
        }

        public bool SendEmail(string frm, string[] to, string[] cc, string [] bcc, string subject, string body)
        {
            try
            {
                SmtpClient SmtpServer = new SmtpClient() { Port = 25, Host = ConfigurationManager.AppSettings["SMTPSvr"] };

                MailMessage mail = new MailMessage() { IsBodyHtml = true, From = new MailAddress(frm), Subject = subject, Body = body };
                foreach (string t in to) mail.To.Add(new MailAddress(t));

                if (cc != null && cc.Length > 0)
                {
                    foreach (string c in cc) mail.CC.Add(new MailAddress(c));
                }

                if (bcc != null && bcc.Length > 0)
                {
                    foreach (string bc in bcc) mail.Bcc.Add(new MailAddress(bc));
                }

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                Log_Handler lh = new Log_Handler();
                lh.WriteLog(ex, "Email_Handler.cs", "SendEmail()", ex.Message, -1);

                return false;
            }
        }
    }
}