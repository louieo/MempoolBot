using MempoolBot.Models;
using MempoolBot.Notifications;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MempoolBot
{
    internal class ApiPoller
    {
        const int POLLING_INTERVAL_SECONDS = 5;

        Settings _Settings;
        INotifier _Notifier;
        MempoolAPI _MempoolAPI;

        Timer _Timer = new Timer();
        RecommendedFees? _PreviousFees;
        DateTime _LastNotificationTime = DateTime.MinValue;

        public ApiPoller(Settings settings, INotifier notifier)
        {
            _Settings = settings;
            _Notifier = notifier;
            _MempoolAPI = new MempoolAPI(_Settings.MempoolApiUrl);
        }

        ~ApiPoller()
        {
            Console.WriteLine("ApiPoller shutting down...");
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
                var currentFees = await _MempoolAPI.GetRecommendedFees();

                if (currentFees != null)
                {
                    //Console.WriteLine(JsonConvert.SerializeObject(fees));

                    if (currentFees.EconomyFee <= _Settings.EconomyRateThreshold)
                    {
                        if (_PreviousFees == null ||
                            _PreviousFees.EconomyFee > _Settings.EconomyRateThreshold ||
                            (DateTime.Now - _LastNotificationTime).Minutes >= _Settings.NotifyRepeatFrequencyMinutes)
                        {
                            Console.WriteLine($"Sending notification! EconomyFee = {currentFees.EconomyFee}, EconomyRateThreshold = {_Settings.EconomyRateThreshold}");
                            _LastNotificationTime = DateTime.Now;

                            await _Notifier.SendAsync(currentFees, _PreviousFees);
                        }
                    }

                    _PreviousFees = currentFees;
                }
            }
            finally
            {
                _Timer.Start();
            }
        }
    }
}