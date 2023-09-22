using MempoolBot.Models;

namespace MempoolBot.Notifications
{
	public interface INotifier: IDisposable
    {
        public void INotifier(Settings settings) { }
        public async Task SendAsync(RecommendedFees currentFees, RecommendedFees previousFees) { }
    }
}

