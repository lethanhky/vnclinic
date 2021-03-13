using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Web;

namespace VNCLNIC.Common.Tools
{
    public class MailHelper
    {
        public void DoSend(String server, int port, int timeout, Boolean SSL, String From, String To, String Subject, String Content, String smtpuser, String smtppass)
        {
            SmtpClient client = new SmtpClient();
            client.Port = port;
            client.Host = server;
            client.EnableSsl = SSL;
            //client.Timeout = timeout;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
            MailMessage mm = new MailMessage(From, To, Subject, Content);
            mm.IsBodyHtml = true;
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.Send(mm);
        }
        public static void SendEmail(String server, int port, int timeout, Boolean SSL, String From, String To, String Subject, String Content, String smtpuser, String smtppass, string cc, string bcc)
        {
            try
            {

                Thread email = new Thread(delegate ()
                {
                    SendAsyncEmail(server, port, timeout, SSL, From, To, Subject, Content, smtpuser, smtppass, cc, bcc);
                });
                email.IsBackground = true;
                email.Start();
            }
            catch
            {

            }
        }

        static void NEVER_EAT_POISON_Disable_CertificateValidation()
        {
            // Disabling certificate validation can expose you to a man-in-the-middle attack
            // which may allow your encrypted message to be read by an attacker
            // https://stackoverflow.com/a/14907718/740639
            ServicePointManager.ServerCertificateValidationCallback =
                delegate (
                    object s,
                    X509Certificate certificate,
                    X509Chain chain,
                    SslPolicyErrors sslPolicyErrors
                ) {
                    return true;
                };
        }
        public static void SendAsyncEmail(String server, int port, int timeout, Boolean SSL, String From, String To, String Subject, String Content, String smtpuser, String smtppass, string cc, string bcc)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = port;
                client.Host = server;
                client.EnableSsl = SSL;
                //client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
                MailMessage mm = new MailMessage(From, To, Subject, Content);

                if (cc != null && cc != "")
                {
                    string[] tempcc = cc.Replace(",", ";").Split(';');
                    foreach (string s in tempcc)
                    {
                        if (s != "")
                            mm.CC.Add(new MailAddress(s));
                    }

                }
                if (bcc != null && bcc != "")
                {
                    string[] tempcc = bcc.Replace(",", ";").Split(';');
                    foreach (string s in tempcc)
                    {
                        if (s != "")
                            mm.Bcc.Add(new MailAddress(s));
                    }

                }
                mm.IsBodyHtml = true;
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                NEVER_EAT_POISON_Disable_CertificateValidation();
                client.Send(mm);
            }
            catch (Exception ex)
            {
                string s = ex.Message;
            }
        }

        public void DoSend(String server, int port, int timeout, Boolean SSL, String From, String To, String Subject, String Content, String smtpuser, String smtppass, string cc, string bcc)
        {
            try
            {
                SmtpClient client = new SmtpClient();
                client.Port = port;
                client.Host = server;
                client.EnableSsl = SSL;
                client.Timeout = 10000;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
                MailMessage mm = new MailMessage(From, To, Subject, Content);

                if (cc != null && cc != "")
                {
                    string[] tempcc = cc.Replace(",", ";").Split(';');
                    foreach (string s in tempcc)
                    {
                        if (s != "")
                            mm.CC.Add(new MailAddress(s));
                    }

                }
                if (bcc != null && bcc != "")
                {
                    string[] tempcc = bcc.Replace(",", ";").Split(';');
                    foreach (string s in tempcc)
                    {
                        if (s != "")
                            mm.Bcc.Add(new MailAddress(s));
                    }

                }
                mm.IsBodyHtml = true;
                mm.BodyEncoding = UTF8Encoding.UTF8;
                mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                client.Send(mm);
            }
            catch
            {
            }
        }
    }
}