using MempoolBot.Models;
using RestSharp;

namespace MempoolBot
{
    internal class MempoolAPI: IDisposable
    {
        readonly RestClient _RestClient;

        public MempoolAPI(string url)
        {
            _RestClient = new RestClient(url);
        }

        public async Task<RecommendedFees?> GetRecommendedFees()
        {
            try
            {
                return await _RestClient.GetJsonAsync<RecommendedFees>("fees/recommended");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting fees from Mempool API: {ex.Message}");
                return null;
            }
        }

        public void Dispose()
        {
            _RestClient?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
