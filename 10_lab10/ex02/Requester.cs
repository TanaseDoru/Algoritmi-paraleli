using System.Text.Json;

namespace ex02
{
    public class Requester
    {
        private readonly string _targetUrl;
        private readonly HttpClient _httpClient;
        private readonly List<string> _imageUrls;

        public Requester(string targetUrl)
        {
            _targetUrl = targetUrl;
            _httpClient = new HttpClient();
            _imageUrls = new List<string>();
        }

        public IReadOnlyList<string> ImageUrls => _imageUrls;

        public async Task CollectImageUrlsAsync(int desiredCount)
        {
            while (_imageUrls.Count < desiredCount)
            {
                ApiResponse? response = await GetSingleResponseWithRetry();

                if (response == null)
                {
                    Console.WriteLine("Eșec definitiv la obținerea unui răspuns valid. Oprire colectare.");
                    break;
                }

                if (response.Status == Constants.API_RESPONSE_SUCCESS &&
                    !string.IsNullOrWhiteSpace(response.Url))
                {
                    _imageUrls.Add(response.Url);
                    Console.WriteLine($"[{_imageUrls.Count}/{desiredCount}] SUCCESS => {response.Url}");
                }
                else if (response.Status == Constants.API_RESPONSE_RETRY_LATER)
                {
                    // Backend-ul ne spune să încercăm mai târziu → aplicăm backoff suplimentar
                    int retryDelay = Constants.INITIAL_DELAY_MS * (int)Math.Pow(2, _imageUrls.Count + 1);
                    Console.WriteLine($"Răspuns invalid: Status = RETRY-LATER. Aștept {retryDelay}ms înainte de următoarea cerere...");
                    await Task.Delay(retryDelay);
                }
                else
                {
                    Console.WriteLine($"Răspuns invalid necunoscut: Status = {response.Status}");
                }
            }
        }

        private async Task<ApiResponse?> GetSingleResponseWithRetry()
        {
            int retryCount = 0;

            while (retryCount <= Constants.MAX_RETRY_PER_REQUEST)
            {
                try
                {
                    string json = await _httpClient.GetStringAsync(_targetUrl);

                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    return JsonSerializer.Deserialize<ApiResponse>(json, options);
                }
                catch (HttpRequestException ex)
                {
                    retryCount++;
                    if (retryCount > Constants.MAX_RETRY_PER_REQUEST)
                    {
                        Console.WriteLine($"Eșec definitiv după {Constants.MAX_RETRY_PER_REQUEST + 1} încercări de rețea: {ex.Message}");
                        return null;
                    }

                    int delay = Constants.INITIAL_DELAY_MS * (int)Math.Pow(2, retryCount - 1);
                    Console.WriteLine($"Eroare rețea (încercarea {retryCount}). Reîncercare în {delay}ms...");
                    await Task.Delay(delay);
                }
            }

            return null;
        }
    }
}