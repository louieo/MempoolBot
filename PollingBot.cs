using MempoolBot.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MempoolBot
{
    internal class PollingBot
    {
        MempoolAPI _MempoolAPI;
        int _EconomyThreshold;
        int _AlertFrequencyRepeatMins;
        string _SmtpServer;
        string _SmtpUser;
        string _SmtpPass;
        string _FromEmail;
        string _ToEmail;

        Timer _Timer = new Timer();
        RecommendedFees? _PreviousFees;
        DateTime _EmailLastSentTime = DateTime.MinValue;

        public PollingBot(string mempoolApiUrl,
            int economyRateThreshold,
            int alertFrequencyRepeatMins,
            string smtpServer,
            string smtpUser,
            string smtpPass,
            string fromEmail,
            string toEmail)
        {
            _MempoolAPI = new MempoolAPI(mempoolApiUrl);

            _EconomyThreshold = economyRateThreshold;
            _AlertFrequencyRepeatMins = alertFrequencyRepeatMins;
            _SmtpServer = smtpServer;
            _SmtpUser = smtpUser;
            _SmtpPass = smtpPass;
            _FromEmail = fromEmail;
            _ToEmail = toEmail;
        }

        public void Start()
        {
            _Timer.Interval = 5000;
            _Timer.Elapsed += Timer_Elapsed;
            _Timer.Start();
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            try
            {
                var fees = await _MempoolAPI.GetRecommendedFees();

                if (fees != null)
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(fees));

                    if (fees.EconomyFee <= _EconomyThreshold)
                    {
                        if (_PreviousFees == null ||
                            _PreviousFees.EconomyFee > _EconomyThreshold ||
                            (DateTime.Now - _EmailLastSentTime).Minutes >= _AlertFrequencyRepeatMins)
                        {
                            Console.WriteLine($"Sending alert! EconomyFee = {JsonConvert.SerializeObject(fees.EconomyFee)}, ECONOMY_THRESHOLD = {_EconomyThreshold}");
                            _EmailLastSentTime = DateTime.Now;

                            var body = MakeEmailBody(fees);
                            await SendEmailAsync(body);
                        }
                    }

                    _PreviousFees = fees;
                }
            }
            finally
            {
                _Timer.Start();
            }
        }

        private string MakeEmailBody(RecommendedFees fees)
        {
            return $"<b>Now:</b> {JsonConvert.SerializeObject(fees)}<br>" +
                    $"<b>Previous:</b> {JsonConvert.SerializeObject(_PreviousFees)}<br>" +
                    $"<b>ECONOMY_THRESHOLD:</b> {_EconomyThreshold}";
        }

        private async Task SendEmailAsync(string body)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient(_SmtpServer);

                // set smtp-client with basicAuthentication
                var basicAuthenticationInfo = new NetworkCredential(_SmtpUser, _SmtpPass);
                mySmtpClient.UseDefaultCredentials = false;
                mySmtpClient.Port = 587;
                mySmtpClient.Credentials = basicAuthenticationInfo;
                mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                mySmtpClient.EnableSsl = true;

                // add from,to mailaddresses
                var from = new MailAddress(_FromEmail, "Mempool Bot");
                var to = new MailAddress(_ToEmail);
                var myMail = new MailMessage(from, to);

                // set subject and encoding
                myMail.Subject = "Mempool Fee Bot";
                myMail.SubjectEncoding = System.Text.Encoding.UTF8;

                // set body-message and encoding
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
    }
}