namespace MempoolBot.Lib.Common;

public class Settings
{
    public required string NotifierType { get; set; }
    public required string MempoolApiUrl { get; set; }
    public required int EconomyRateThreshold { get; set; }
    public required int NotifyRepeatFrequencyMinutes { get; set; }
}