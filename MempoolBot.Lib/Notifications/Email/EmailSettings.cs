namespace MempoolBot.Lib.Notifications;

public sealed class EmailSettings
{
    public required string SmtpServer { get; set; }
    public required string SmtpUser { get; set; }
    public required string SmtpPass { get; set; }
    public required string FromEmail { get; set; }
    public required string ToEmail { get; set; }
}