namespace MempoolBot.Web.Data.Models;

public class Settings
{
    public required string NotifyMethod { get; set; }
    public required string MempoolApiUrl { get; set; }
    public required int EconomyRateThreshold { get; set; }
    public required int NotifyRepeatFrequencyMinutes { get; set; }
}