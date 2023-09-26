
using MempoolBot.Lib.Common;

namespace MempoolBot
{
	public sealed class Settings
    {
        public required NotifyMethod NotifyMethod { get; set; }

        public required string MempoolApiUrl { get; set; }
        public required int EconomyRateThreshold { get; set; }
        public required int NotifyRepeatFrequencyMinutes { get; set; }

        public required string TelegramBotToken { get; set; }

        public required string SmtpServer { get; set; }
        public required string SmtpUser { get; set; }
        public required string SmtpPass { get; set; }
        public required string FromEmail { get; set; }
        public required string ToEmail { get; set; }
    }
}

