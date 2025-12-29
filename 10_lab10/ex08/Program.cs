namespace ex08
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string symbol = "AAPL";

            using var cts = new CancellationTokenSource();

            var alphaClient = new AlphaVantageClient();
            var fmpClient = new FMPClient();

            Task<string> taskAlpha = alphaClient.GetStockQuoteAsync(symbol, cts.Token);
            Task<string> taskFMP = fmpClient.GetStockQuoteAsync(symbol, cts.Token);

            Task<string> completedTask = await Task.WhenAny(taskAlpha, taskFMP);

            cts.Cancel();

            try
            {
                string result = await completedTask;
                Console.WriteLine($"Primul raspuns succes: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare in task completat: {ex.Message}");
            }

            try
            {
                await Task.WhenAll(taskAlpha, taskFMP);
            }
            catch { }
        }
    }

    public class AlphaVantageClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetStockQuoteAsync(string symbol, CancellationToken ct)
        {
            string apiKey = "4DZE1GS3B2HWKDTT"; 
            string url = $"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={symbol}&apikey={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }
    }

    public class FMPClient
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public async Task<string> GetStockQuoteAsync(string symbol, CancellationToken ct)
        {
            string apiKey = "XSsHkulSIzd6AAWxq7V3GxlAVoybtJtx"; 
            string url = $"https://financialmodelingprep.com/stable/search-name?query={symbol}&apikey={apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }
    }
}