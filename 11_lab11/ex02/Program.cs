using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace ex02
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

        public async IAsyncEnumerable<Product> GetProductsAsync()
        {
            int offset = 0;

            while (true)
            {
                string url = $"{BaseUrl}?offset={offset}&limit={PageSize}";

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                {
                    yield break;
                }

                string json = await response.Content.ReadAsStringAsync();

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

            var client = new ProductClient();

            await foreach (var product in client.GetProductsAsync())
            {
                Console.WriteLine(
                    $"Processing product {product.Id}: " +
                    $"Name={product.Name}, " +
                    $"Category={product.Category}, " +
                    $"Description={product.Description}, " +
                    $"Price={product.Price:F2}");
            }

            Console.WriteLine("Toate produsele au fost procesate.");
            Console.ReadKey();
        }
    }
}