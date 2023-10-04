using System.Net;
using System.Net.Mail;
using MempoolBot.Lib.Common;
using MempoolBot.Lib.MempoolSpace.Models;
using Microsoft.Extensions.Options;

namespace MempoolBot.Lib.Notifications;

public class EmailNotifier : INotifier
{
    IOptions<Settings> _GeneralSettings;
    IOptions<EmailSettings> _EmailSettings;
    private bool disposedValue;

    public RecommendedFees? LatestFees { get; set; }
    public DateTime LastNotificationTime { get; internal set; }

    public bool IsStarted { get; internal set; }

    public EmailNotifier(IOptions<Settings> settings, IOptions<EmailSettings> emailSettings)
    {
        _GeneralSettings = settings;
        _EmailSettings = emailSettings;

        ApplySettings();
    }

    public async Task SendFeesAsync(RecommendedFees currentFees, bool isRepeatNotification)
    {
        try
        {
            SmtpClient mySmtpClient = new SmtpClient(_EmailSettings.Value.SmtpServer);

            // set smtp-client with basicAuthentication
            var basicAuthenticationInfo = new NetworkCredential(_EmailSettings.Value.SmtpUser, _EmailSettings.Value.SmtpPass);
            mySmtpClient.UseDefaultCredentials = false;
            mySmtpClient.Port = 587;
            mySmtpClient.Credentials = basicAuthenticationInfo;
            mySmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mySmtpClient.EnableSsl = true;

            // add from,to mailaddresses
            var from = new MailAddress(_EmailSettings.Value.FromEmail, "Mempool Bot");
            var to = new MailAddress(_EmailSettings.Value.ToEmail);
            var myMail = new MailMessage(from, to);

            // set subject and encoding
            myMail.Subject = "Mempool Fee Bot";
            myMail.SubjectEncoding = System.Text.Encoding.UTF8;

            // set body-message and encoding
            var body = MakeEmailBody(currentFees);
            myMail.Body = body;
            myMail.BodyEncoding = System.Text.Encoding.UTF8;

            // text or html
            myMail.IsBodyHtml = true;

            await mySmtpClient.SendMailAsync(myMail);

            LastNotificationTime = DateTime.UtcNow;
        }
        catch (SmtpException ex)
        {
            Console.WriteLine($"SmtpException has occured: {ex}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception has occured sending email: {ex}");
        }
    }

    private string MakeEmailBody(RecommendedFees currentFees)
    {
        var body = $"<b>Current \"economy\" fee:</b> {currentFees.EconomyFee} sats/vbyte<br>";
        if (LatestFees != null) body += $"<b>Previous \"economy\" fee:</b> {LatestFees?.EconomyFee} sats/vbyte<br>";
        body += $"<b>Notification Threshold:</b> {_GeneralSettings.Value.EconomyRateThreshold} sats/vbyte";

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

    public void ApplySettings()
    {
        IsStarted = true;
    }
}

