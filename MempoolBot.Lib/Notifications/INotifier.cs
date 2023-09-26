using MempoolBot.Lib.MempoolSpace.Models;

namespace MempoolBot.Lib.Notifications
{
	public interface INotifier: IDisposable
    {
        public RecommendedFees? LatestFees { get; set; }
        public Task SendFeesAsync(RecommendedFees currentFees, bool isRepeatNotification);
    }
}

