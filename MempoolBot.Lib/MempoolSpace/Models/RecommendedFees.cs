namespace MempoolBot.Lib.MempoolSpace.Models
{
    using Newtonsoft.Json;

    public partial class RecommendedFees
    {
        [JsonProperty("fastestFee")]
        public long FastestFee { get; set; }

        [JsonProperty("halfHourFee")]
        public long HalfHourFee { get; set; }

        [JsonProperty("hourFee")]
        public long HourFee { get; set; }

        [JsonProperty("economyFee")]
        public long EconomyFee { get; set; }

        [JsonProperty("minimumFee")]
        public long MinimumFee { get; set; }
    }
}
