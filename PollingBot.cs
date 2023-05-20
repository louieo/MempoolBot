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
        const int ECONOMY_THRESHOLD = 5;
        const int EMAIL_REPEAT_INTERVAL_MINS = 10;

        MempoolAPI _MempoolAPI;
        int _EconomyThreshold;
        Timer _Timer = new Timer();
        RecommendedFees? _PreviousFees;
        DateTime _EmailLastSentTime = DateTime.MinValue;

        public PollingBot(string url) 
        {
			_MempoolAPI = new MempoolAPI(url);
            if (!int.TryParse(Environment.GetEnvironmentVariable("ECONOMY_THRESHOLD"), out _EconomyThreshold))
                _EconomyThreshold = ECONOMY_THRESHOLD;
            Console.WriteLine($"Economy threshold = {_EconomyThreshold}");
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
                            (DateTime.Now - _EmailLastSentTime).Minutes >= EMAIL_REPEAT_INTERVAL_MINS)
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
                    $"<b>ECONOMY_THRESHOLD:</b> {ECONOMY_THRESHOLD}";
        }

        private async Task SendEmailAsync(string body)
        {
            try
            {
                SmtpClient mySmtpClient = new SmtpClient("smtp.gmail.com");

                // set smtp-client with basicAuthentication
                mySmtpClient.UseDefaultCredentials = false;
                NetworkCredential basicAuthenticationInfo = new NetworkCredential("louieo@gmail.com", "welizcrheejrnirk");
                mySmtpClient.Port = 587;
                mySmtpClient.Credentials = basicAuthenticationInfo;
                mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network; 
                mySmtpClient.EnableSsl = true;

                // add from,to mailaddresses
                MailAddress from = new MailAddress("louieo@gmail.com", "Mempool Bot");
                MailAddress to = new MailAddress("louieo@gmail.com");
                MailMessage myMail = new MailMessage(from, to);

                // add ReplyTo
                //MailAddress replyTo = new MailAddress("louieo@gmail.com");
                //myMail.ReplyToList.Add(replyTo);

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