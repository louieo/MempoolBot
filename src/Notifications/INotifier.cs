﻿using MempoolBot.Models;

namespace MempoolBot.Notifications
{
	public interface INotifier: IDisposable
    {
        public RecommendedFees? LatestFees { get; set; }
        public void INotifier(Settings settings) { }
        public async Task SendFeesAsync(RecommendedFees currentFees) { }
    }
}

