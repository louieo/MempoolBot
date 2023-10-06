using MempoolBot.Lib;
using MempoolBot.Lib.Common;
using MempoolBot.Lib.Notifications;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace MempoolBot.Web.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _Logger;
    private IOptions<Settings> _Settings;
    private IOptions<TelegramSettings> _TelegramSettings;
    private FeeChecker _FeeChecker;

    public IndexModel(ILogger<IndexModel> logger,
        IOptions<Settings> settings,
        IOptions<TelegramSettings> telegramSettings,
        FeeChecker feechecker)
    {
        _Logger = logger;
        _Settings = settings;
        _TelegramSettings = telegramSettings;
        _FeeChecker = feechecker;
    }

    public void OnGet()
    {
    }

    public void OnPost(string mempoolApiUrl,
        int economyRateThreshold,
        int notifyRepeatFrequencyMinutes,
        string telegramBotToken)
    {
        _Logger.LogInformation("Saving settings...");

        _Settings.Value.MempoolApiUrl = mempoolApiUrl;
        _Settings.Value.EconomyRateThreshold = economyRateThreshold;
        _Settings.Value.NotifyRepeatFrequencyMinutes = notifyRepeatFrequencyMinutes;
        _TelegramSettings.Value.TelegramBotToken = telegramBotToken;
        _FeeChecker.Notifier.ApplySettings();
    }
}

