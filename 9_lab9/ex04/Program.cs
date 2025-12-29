using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.Json;

namespace ex04
{
    internal class Program
    {
        // Cozi thread-safe pentru comunicarea între thread-uri
        static BlockingCollection<string> urlQueue = new();
        static BlockingCollection<string> downloadedImages = new();

        static HttpClient client = new HttpClient();
        static bool running = true;

        static void Main(string[] args)
        {
            // Creăm folderul de output dacă nu există
            Directory.CreateDirectory("images");

            Thread requester = new Thread(RequesterThread);
            Thread downloader = new Thread(DownloaderThread);
            Thread processer = new Thread(ProcesserThread);

            requester.Start();
            downloader.Start();
            processer.Start();

            Console.WriteLine("Aplicația rulează. Apăsați ENTER pentru a opri...");
            Console.ReadLine();

            running = false;
            urlQueue.CompleteAdding();
            downloadedImages.CompleteAdding();
        }

        // ================= 1. REQUESTER (cu Exponential Backoff) =================
        static void RequesterThread()
        {
            int delaySeconds = 1;

            while (running)
            {
                try
                {
                    var response = client.GetAsync("http://localhost:5000/image").Result;
                    string jsonText = response.Content.ReadAsStringAsync().Result;

                    if (string.IsNullOrWhiteSpace(jsonText)) throw new Exception("Răspuns vid de la server");

                    using (JsonDocument doc = JsonDocument.Parse(jsonText))
                    {
                        string status = doc.RootElement.GetProperty("status").GetString();

                        if (status == "SUCCESS")
                        {
                            string url = doc.RootElement.GetProperty("url").GetString();
                            urlQueue.Add(url);
                            Console.WriteLine($"[Requester] Succes: {url}");
                            delaySeconds = 1; // Resetăm la succes
                        }
                        else if (status == "RETRY-LATER")
                        {
                            Console.WriteLine($"[Requester] RETRY-LATER. Aștept {delaySeconds}s...");
                            Thread.Sleep(delaySeconds * 1000);
                            delaySeconds *= 2; // Dublăm timpul (Exponential Backoff)
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Requester] Eroare/Retry: {ex.Message}. Aștept {delaySeconds}s...");
                    Thread.Sleep(delaySeconds * 1000);
                    delaySeconds *= 2;
                }
            }
        }

        // ================= 2. DOWNLOADER (Fiecare imagine o singură dată) =================
        static void DownloaderThread()
        {
            HashSet<string> seenUrls = new();

            // Consumă URL-urile din coadă pe măsură ce apar
            foreach (var url in urlQueue.GetConsumingEnumerable())
            {
                if (!seenUrls.Add(url)) continue; // Dacă există deja în HashSet, dă skip

                try
                {
                    byte[] data = client.GetByteArrayAsync(url).Result;
                    string fileName = Path.GetFileName(new Uri(url).LocalPath);
                    string fullPath = Path.Combine("images", fileName);

                    File.WriteAllBytes(fullPath, data);
                    downloadedImages.Add(fullPath);
                    Console.WriteLine($"[Downloader] Salvat: {fileName}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Downloader] Eroare la {url}: {ex.Message}");
                }
            }
        }

        // ================= 3. PROCESSER (Watermark) =================
        static void ProcesserThread()
        {
            foreach (var path in downloadedImages.GetConsumingEnumerable())
            {
                try
                {
                    // Așteptăm puțin să ne asigurăm că fișierul nu e blocat de sistem
                    Thread.Sleep(200);

                    using (Image img = Image.FromFile(path))
                    using (Graphics g = Graphics.FromImage(img))
                    {
                        using (Font font = new Font("Arial", 30, FontStyle.Bold))
                        using (Brush brush = new SolidBrush(Color.FromArgb(128, Color.White)))
                        {
                            g.DrawString("WATERMARK", font, brush, new PointF(20, 20));
                        }

                        string extension = Path.GetExtension(path);
                        string newPath = path.Replace(extension, ".watermarked" + extension);

                        img.Save(newPath, ImageFormat.Jpeg);
                        Console.WriteLine($"[Processer] Procesat: {Path.GetFileName(newPath)}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Processer] Eroare procesare {path}: {ex.Message}");
                }
            }
        }
    }
}