using MempoolBot.Lib;
using MempoolBot.Lib.Common;
using MempoolBot.Lib.Notifications;
using MempoolBot.Web.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<Settings>(builder.Configuration.GetSection("Settings"));
builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("Settings").GetSection("Telegram"));

builder.Services.AddDbContext<MempoolBotContext>(options =>
 {
     options.UseNpgsql(builder.Configuration.GetSection("DatabaseConfig")["PostgresSQL"]);
 });

// Get values from the config given their key and their target type.
Settings? settings = builder.Configuration.GetRequiredSection("Settings").Get<Settings>();
if (settings == null) throw new Exception("Unable to get config from settings file");

Console.WriteLine("---------------------------------------------");
Console.WriteLine($"NotifierType = {settings.NotifierType}");
Console.WriteLine($"MempoolApiUrl = {settings.MempoolApiUrl}");
Console.WriteLine($"EconomyRateThreshold = {settings.EconomyRateThreshold}");
Console.WriteLine($"NotifyRepeatFrequencyMinutes = {settings.NotifyRepeatFrequencyMinutes}");
//Console.WriteLine($"TelegramBotToken = {settings.TelegramBotToken}");
//Console.WriteLine($"SmtpServer = {settings.SmtpServer}");
//Console.WriteLine($"SmtpUser = {settings.SmtpUser}");
//Console.WriteLine($"SmtpPass = {settings.SmtpPass}");
//Console.WriteLine($"FromEmail = {settings.FromEmail}");
//Console.WriteLine($"ToEmail = {settings.ToEmail}");
Console.WriteLine("---------------------------------------------");

Type? notifierType = Type.GetType($"MempoolBot.Lib.Notifications.{settings.NotifierType}, MempoolBot.Lib");

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<FeeChecker>();
builder.Services.AddSingleton(typeof(INotifier), notifierType);

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

Console.WriteLine("Creating Fee Checker...");

var feeChecker = app.Services.GetRequiredService<FeeChecker>();

Console.WriteLine("Starting Fee Checker...");
feeChecker.Start();

app.Run();

Console.WriteLine("Web is now shutdown");