using MempoolBot;
using MempoolBot.Lib;
using MempoolBot.Lib.Common;
using MempoolBot.Lib.Notifications;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();


// Build a config object, using env vars and JSON providers.
IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
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
    Console.WriteLine("Creating Fee Checker...");
    var feeChecker = new FeeChecker(settings, notifier);

    Console.WriteLine("Starting Fee Checker...");
    feeChecker.Start();

    //Thread.Sleep(Timeout.Infinite);
}

app.Run();

Console.WriteLine("Web is now shutdown");