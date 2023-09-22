using System.Net;
using System.Net.Mail;
using MempoolBot.Models;
using Newtonsoft.Json;

namespace MempoolBot.Notifications
{
	public class EmailNotifier: INotifier
	{
        Settings _Settings;
        private bool disposedValue;

        public EmailNotifier(Settings settings)
		{
            _Settings = settings;
        }

        public async Task SendAsync(RecommendedFees currentFees, RecommendedFees previousFees)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient(_Settings.SmtpServer);

                // set smtp-client with basicAuthentication
                var basicAuthenticationInfo = new NetworkCredential(_Settings.SmtpUser, _Settings.SmtpPass);
                mySmtpClient.UseDefaultCredentials = false;
                mySmtpClient.Port = 587;
                mySmtpClient.Credentials = basicAuthenticationInfo;
                mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mySmtpClient.EnableSsl = true;

                // add from,to mailaddresses
                var from = new MailAddress(_Settings.FromEmail, "Mempool Bot");
                var to = new MailAddress(_Settings.ToEmail);
                var myMail = new MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = "Mempool Fee Bot";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
                var body = MakeEmailBody(currentFees, previousFees);
                myMail.Body = body;
                myMail.BodyEncoding = System.Text.Encoding.UTF8;

                // text or html
                myMail.IsBodyHtml = true;

                await mySmtpClient.SendMailAsync(myMail);
            }
            catch (SmtpException ex)
            {
                throw new ApplicationException("SmtpException has occured: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        private string MakeEmailBody(RecommendedFees currentFees, RecommendedFees previousFees)
        {
            var body = $"<b>Current \"economy\" fee:</b> {currentFees.EconomyFee} sats/vbyte<br>";
            if (previousFees != null) body += $"<b>Previous \"economy\" fee:</b> {previousFees?.EconomyFee} sats/vbyte<br>";
            body += $"<b>Notification Threshold:</b> {_Settings.EconomyRateThreshold} sats/vbyte";

            return body;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~EmailNotifier()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}

