// See https://aka.ms/new-console-template for more information
using MempoolBot;

const string DEFAULT_MEMPOOL_API_URL = "https://mempool.space:3006/api/v1";
const int DEFAULT_ECONOMY_RATE_THRESHOLD = 5;
const int DEFAULT_ALERT_REPEAT_FREQUENCY_MINUTES = 10;
const string DEFAULT_SMTP_SERVER = "smtp.example.com";
const string DEFAULT_SMTP_USER = "example_user@example.com";
const string DEFAULT_SMTP_PASS = "example";
const string DEFAULT_FROM_EMAIL = "example_from@example.com";
const string DEFAULT_TO_EMAIL = "example_to@example.com";

var mempoolApiUrl = Environment.GetEnvironmentVariable("MEMPOOL_API_URL") ?? DEFAULT_MEMPOOL_API_URL;
if (!int.TryParse(Environment.GetEnvironmentVariable("ECONOMY_RATE_THRESHOLD"), out var economyRateThreshold))
    economyRateThreshold = DEFAULT_ECONOMY_RATE_THRESHOLD;
if (!int.TryParse(Environment.GetEnvironmentVariable("ALERT_REPEAT_FREQUENCY_MINUTES"), out var alertRepeatFrequencyMinutes))
    alertRepeatFrequencyMinutes = DEFAULT_ALERT_REPEAT_FREQUENCY_MINUTES;
var smptServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? DEFAULT_SMTP_SERVER;
var smptUser = Environment.GetEnvironmentVariable("SMTP_USER") ?? DEFAULT_SMTP_USER;
var smptPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? DEFAULT_SMTP_PASS;
var fromEmail = Environment.GetEnvironmentVariable("FROM_EMAIL") ?? DEFAULT_FROM_EMAIL;
var toEmail = Environment.GetEnvironmentVariable("TO_EMAIL") ?? DEFAULT_TO_EMAIL;

Console.WriteLine("Creating polling bot...");
var pollingBot = new PollingBot(mempoolApiUrl,
    economyRateThreshold,
    alertRepeatFrequencyMinutes,
    smptServer,
    smptUser,
    smptPass,
    fromEmail,
    toEmail);

Console.WriteLine("Starting polling bot...");
pollingBot.Start();

// Hang around
Thread.Sleep(Timeout.Infinite);

// to run
// docker login -u louieo
// docker pull louieo/mempoolbot
// docker run -e "MEMPOOL_API_URL=http://192.168.1.114:3006/api/v1" -e "ECONOMY_THRESHOLD=5" -d louieo/mempoolbot