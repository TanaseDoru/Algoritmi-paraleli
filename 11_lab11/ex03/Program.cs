using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ex03
{
    public class Product
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public double Price { get; set; }
    }

    internal class ProductClient
    {
        private readonly HttpClient _httpClient;
        private const int PageSize = 10;
        private const string BaseUrl = "http://localhost:5000/api/products";

        public ProductClient()
        {
            _httpClient = new HttpClient();
        }

        public async Task SearchProductsByPriceAsync(double targetPrice, int k)
        {
            var cts = new CancellationTokenSource();
            int foundCount = 0;

            try
            {
                await foreach (var product in GetAllProductsAsync(cts.Token))
                {
                    if (Math.Abs(product.Price - targetPrice) < 0.001)
                    {
                        foundCount++;
                        Console.WriteLine(
                            $"[{foundCount}/{k}] Found: {product.Id} | {product.Name} | " +
                            $"Category: {product.Category} | Price: {product.Price:F2}");

                        if (foundCount >= k)
                        {
                            cts.Cancel();
                            Console.WriteLine($"Găsite toate cele {k} produse cu pretul {targetPrice:F2}. Oprire cautare.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private async IAsyncEnumerable<Product> GetAllProductsAsync(
            [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            int offset = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                string url = $"{BaseUrl}?offset={offset}&limit={PageSize}";

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    yield break;
                }

                string json = await response.Content.ReadAsStringAsync(cancellationToken);

                var products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (products == null || products.Count == 0)
                {
                    yield break;
                }

                foreach (var product in products)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    yield return product;
                }

                if (products.Count < PageSize)
                {
                    yield break;
                }

                offset += PageSize;
            }
        }
    }

    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("ex03: Cautare primele K produse cu un pret dat\n");

            var client = new ProductClient();

            double targetPrice = 39.99;
            int k = 2;

            Console.WriteLine($"Cautam primele {k} produse cu pretul {targetPrice:F2}...\n");

            await client.SearchProductsByPriceAsync(targetPrice, k);

            Console.WriteLine("\nProgram terminat.");
            Console.ReadKey();
        }
    }
}