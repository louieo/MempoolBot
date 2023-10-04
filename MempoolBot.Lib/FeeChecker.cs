using MempoolBot.Lib.Common;
using MempoolBot.Lib.MempoolSpace;
using MempoolBot.Lib.Notifications;
using Microsoft.Extensions.Options;
using System.Timers;
using Timer = System.Timers.Timer;

namespace MempoolBot.Lib;

public class FeeChecker
{
    const int POLLING_INTERVAL_SECONDS = 5;

    IOptions<Settings> _Settings;
    MempoolSpaceAPI _MempoolSpaceAPI;
    Timer _Timer = new Timer();

    public INotifier Notifier { get; internal set; }

    public FeeChecker(IOptions<Settings> settings, INotifier notifier)
    {
        _Settings = settings;
        _MempoolSpaceAPI = new MempoolSpaceAPI(_Settings.Value.MempoolApiUrl);

        Notifier = notifier;
    }

    ~FeeChecker()
    {
        Console.WriteLine("FeeChecker shutting down...");
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
            //Console.WriteLine($"Getting fees from {_Settings.MempoolApiUrl}...");
            var currentFees = await _MempoolSpaceAPI.GetRecommendedFees();

            if (currentFees != null)
            {
                //Console.WriteLine(JsonConvert.SerializeObject(fees));

                if (currentFees.EconomyFee <= _Settings.Value.EconomyRateThreshold)
                {
                    bool isRepeatNotification = (DateTime.Now - Notifier.LastNotificationTime).Minutes >= _Settings.Value.NotifyRepeatFrequencyMinutes;

                    if (Notifier.LatestFees == null || // have no previous fees recorded
                        Notifier.LatestFees.EconomyFee > _Settings.Value.EconomyRateThreshold || // has dropped since last recorded fees
                        isRepeatNotification)
                    {
                        Console.WriteLine($"Sending notification! EconomyFee = {currentFees.EconomyFee}, EconomyRateThreshold = {_Settings.Value.EconomyRateThreshold}");

                        await Notifier.SendFeesAsync(currentFees, isRepeatNotification);
                    }
                }

                Notifier.LatestFees = currentFees;
            }
        }
        finally
        {
            _Timer.Start();
        }
    }
}