using MempoolBot.Lib.MempoolSpace.Models;

namespace MempoolBot.Lib.Notifications
{
	public interface INotifier: IDisposable
    {
        public RecommendedFees? LatestFees { get; set; }
        public DateTime LastNotificationTime { get; }
        public bool IsStarted { get; }

        public void ApplySettings();
        public Task SendFeesAsync(RecommendedFees currentFees, bool isRepeatNotification);
    }
}

