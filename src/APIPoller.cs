using MempoolBot.Notifications;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MempoolBot
{
    internal class APIPoller
    {
        const int POLLING_INTERVAL_SECONDS = 5;

        Settings _Settings;
        INotifier _Notifier;
        MempoolAPI _MempoolAPI;

        Timer _Timer = new Timer();
        DateTime _LastNotificationTime = DateTime.MinValue;

        public APIPoller(Settings settings, INotifier notifier)
        {
            _Settings = settings;
            _Notifier = notifier;
            _MempoolAPI = new MempoolAPI(_Settings.MempoolApiUrl);
        }

        ~APIPoller()
        {
            Console.WriteLine("APIPoller shutting down...");
        }

        public void Start()
        {
            _Timer.Interval = TimeSpan.FromSeconds(POLLING_INTERVAL_SECONDS).TotalMilliseconds;
            _Timer.Elapsed += Timer_Elapsed;
            _Timer.Start();
        }

        private async void Timer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            _Timer.Stop();
            try
            {
                Console.WriteLine($"Getting fees from {_Settings.MempoolApiUrl}...");
                var currentFees = await _MempoolAPI.GetRecommendedFees();

                if (currentFees != null)
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(fees));

                    if (currentFees.EconomyFee <= _Settings.EconomyRateThreshold)
                    {
                        if (_Notifier.LatestFees == null ||
                            _Notifier.LatestFees.EconomyFee > _Settings.EconomyRateThreshold ||
                            (DateTime.Now - _LastNotificationTime).Minutes >= _Settings.NotifyRepeatFrequencyMinutes)
                        {
                            Console.WriteLine($"Sending notification! EconomyFee = {currentFees.EconomyFee}, EconomyRateThreshold = {_Settings.EconomyRateThreshold}");
                            _LastNotificationTime = DateTime.Now;

                            await _Notifier.SendFeesAsync(currentFees);
                        }
                    }

                    _Notifier.LatestFees = currentFees;
                }
            }
            finally
            {
                _Timer.Start();
            }
        }
    }
}