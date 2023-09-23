using MempoolBot;
using MempoolBot.Common;
using MempoolBot.Notifications;
using Microsoft.Extensions.Configuration;

// Build a config object, using env vars and JSON providers.
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("src/appsettings.json")
    .AddEnvironmentVariables()
    .Build();

// Get values from the config given their key and their target type.
Settings? settings = config.GetRequiredSection("Settings").Get<Settings>();

if (settings == null) throw new Exception("Unable to get config from settings file");

Console.WriteLine("---------------------------------------------");
Console.WriteLine($"NotifyMethod = {settings.NotifyMethod}");
Console.WriteLine($"MempoolApiUrl = {settings.MempoolApiUrl}");
Console.WriteLine($"EconomyRateThreshold = {settings.EconomyRateThreshold}");
Console.WriteLine($"NotifyRepeatFrequencyMinutes = {settings.NotifyRepeatFrequencyMinutes}");
Console.WriteLine($"TelegramBotToken = {settings.TelegramBotToken}");
Console.WriteLine($"SmtpServer = {settings.SmtpServer}");
Console.WriteLine($"SmtpUser = {settings.SmtpUser}");
Console.WriteLine($"SmtpPass = {settings.SmtpPass}");
Console.WriteLine($"FromEmail = {settings.FromEmail}");
Console.WriteLine($"ToEmail = {settings.ToEmail}");
Console.WriteLine("---------------------------------------------");

INotifier notifier;

switch (settings.NotifyMethod)
{
    case NotifyMethod.Email:
        notifier = new EmailNotifier(settings);
        break;
    case NotifyMethod.Telegram:
        notifier = new TelegramNotifier(settings);
        break;
    default:
        throw new Exception($"Unsupported Notification method {settings.NotifyMethod}");
}

using (notifier)
{
    Console.WriteLine("Creating API poller...");
    var apiPoller = new APIPoller(settings, notifier);

    Console.WriteLine("Starting API poller...");
    apiPoller.Start();

    Thread.Sleep(Timeout.Infinite);
}

// to run
// docker login -u louieo
// docker pull louieo/mempoolbot
// docker run -e "MEMPOOL_API_URL=http://192.168.1.114:3006/api/v1" -e "ECONOMY_THRESHOLD=5" -d louieo/mempoolbot