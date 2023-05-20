// See https://aka.ms/new-console-template for more information
using MempoolBot;

//var url = "http://100.118.181.4:3006/api/v1";
//var url = "http://192.168.1.114:3006/api/v1";
var url = Environment.GetEnvironmentVariable("MEMPOOL_API_URL") ?? "http://SOME_IP:3006/api/v1";

Console.WriteLine($"Creating polling bot to url {url}...");
var pollingBot = new PollingBot(url);
Console.WriteLine("Starting polling bot...");
pollingBot.Start();
Thread.Sleep(Timeout.Infinite);

// to run
// docker login -u louieo
// docker pull louieo/mempoolbot
// docker run -e "MEMPOOL_API_URL=http://192.168.1.114:3006/api/v1" -e "ECONOMY_THRESHOLD=5" -d louieo/mempoolbot