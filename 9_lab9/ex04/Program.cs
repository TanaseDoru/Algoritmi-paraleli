using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using SixLabors.ImageSharp.PixelFormats;

namespace ex04
{
    public class ApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }

    public static class Constants
    {
        public const string SUCCESS = "SUCCESS";
        public const string RETRY_LATER = "RETRY-LATER";
    }

    internal class Program
    {
        private static readonly BlockingCollection<string> urlQueue = new();
        private static readonly BlockingCollection<string> pendingWatermark = new();
        private static readonly HttpClient httpClient = new();
        private static volatile bool isRunning = true;

        static async Task Main(string[] args)
        {
            Directory.CreateDirectory("images");

            var requesterTask = Task.Run(RequesterThread);
            var downloaderTask = Task.Run(DownloaderThread);
            var processerTask = Task.Run(ProcesserThread);

            Console.WriteLine("Sistem pornit. Apăsați ENTER pentru a opri...");
            Console.ReadLine();

            isRunning = false;
            urlQueue.CompleteAdding();
            pendingWatermark.CompleteAdding();

            await Task.WhenAll(requesterTask, downloaderTask, processerTask);
            Console.WriteLine("Program terminat.");
        }

        // ================= THREAD 1: REQUESTER =================
        static async Task RequesterThread()
        {
            string apiUrl = "http://localhost:5000/api/image";
            int delaySeconds = 1;

            while (isRunning)
            {
                try
                {
                    string responseString = await httpClient.GetStringAsync(apiUrl);
                    var apiResponse = JsonSerializer.Deserialize<ApiResponse>(responseString,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    if (apiResponse?.Status == Constants.SUCCESS && !string.IsNullOrEmpty(apiResponse.Url))
                    {
                        Console.WriteLine($"[Requester] URL primit: {apiResponse.Url}");
                        urlQueue.Add(apiResponse.Url);
                        delaySeconds = 1;
                    }
                    else
                    {
                        Console.WriteLine($"[Requester] Server ocupat (RETRY-LATER). Retry în {delaySeconds}s...");
                        await Task.Delay(delaySeconds * 1000);
                        delaySeconds *= 2;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Requester] Eroare: {ex.Message}. Retry în {delaySeconds}s...");
                    await Task.Delay(delaySeconds * 1000);
                    delaySeconds *= 2;
                }
            }
        }

        // ================= THREAD 2: DOWNLOADER =================
        static async Task DownloaderThread()
        {
            var downloadedUrls = new HashSet<string>();

            foreach (var url in urlQueue.GetConsumingEnumerable())
            {
                if (!downloadedUrls.Add(url)) continue;

                try
                {
                    byte[] imageBytes = await httpClient.GetByteArrayAsync(url);
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    string filePath = Path.Combine("images", fileName);

                    await File.WriteAllBytesAsync(filePath, imageBytes);
                    Console.WriteLine($"[Downloader] Salvat: {fileName}");

                    pendingWatermark.Add(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Downloader] Eroare la {url}: {ex.Message}");
                }
            }
        }

        // ================= THREAD 3: PROCESSER =================
        static void ProcesserThread()
        {
            foreach (var filePath in pendingWatermark.GetConsumingEnumerable())
            {
                try
                {
                    using var image = Image.Load<Rgba32>(filePath);

                    // Încercăm să găsim Arial prin metoda statică corectă
                    FontFamily family;
                    if (SystemFonts.TryGet("Arial", out family))
                    {
                        // Arial găsit
                    }
                    else
                    {
                        // Fallback: primul font disponibil din sistem
                        family = SystemFonts.Families.First();
                        Console.WriteLine("[Processer] Arial nu a fost găsit. Folosim fallback font: " + family.Name);
                    }

                    var font = family.CreateFont(60, FontStyle.Bold);

                    // Culoare text: alb cu ~70% opacitate
                    var textColor = Color.FromRgba(255, 255, 255, 180);

                    // Contur negru pentru contrast excelent
                    var outlinePen = Pens.Solid(Color.Black, 5);

                    var textOptions = new RichTextOptions(font)
                    {
                        Origin = new PointF(20, 20),
                        HorizontalAlignment = HorizontalAlignment.Left,
                        VerticalAlignment = VerticalAlignment.Top
                    };

                    // Aplicăm watermark-ul (contur întâi pentru efect de umbră)
                    image.Mutate(ctx =>
                    {
                        ctx.DrawText(textOptions, "WATERMARK", outlinePen);
                        ctx.DrawText(textOptions, "WATERMARK", textColor);
                    });

                    // Construim noul nume fișier
                    string directory = Path.GetDirectoryName(filePath)!;
                    string nameWithoutExt = Path.GetFileNameWithoutExtension(filePath);
                    string extension = Path.GetExtension(filePath);
                    string newPath = Path.Combine(directory, $"{nameWithoutExt}.watermarked{extension}");

                    image.Save(newPath);

                    Console.WriteLine($"[Processer] Watermark aplicat: {Path.GetFileName(newPath)}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processer] Eroare la {filePath}: {ex.Message}");
                }
            }
        }
    }
}