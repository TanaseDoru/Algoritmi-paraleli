namespace ex02
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string apiUrl = "http://localhost:5000/api/image";

            var requester = new Requester(apiUrl);
            var downloader = new Downloader();

            Console.WriteLine("Încep colectarea URL-urilor de imagini...\n");

            await requester.CollectImageUrlsAsync(Constants.DESIRED_IMAGE_COUNT);

            int collected = requester.ImageUrls.Count;
            Console.WriteLine($"\nAm colectat {collected} URL-uri din {Constants.DESIRED_IMAGE_COUNT} dorite.");

            if (collected == 0)
            {
                Console.WriteLine("Nicio imagine de descărcat.");
                return;
            }

            Console.WriteLine("\nÎncep descărcarea imaginilor în paralel...");

            var downloadTasks = requester.ImageUrls.Select((url, index) =>
                downloader.DownloadAndSaveAsync(url, $"image_{index + 1}.jpg"));

            await Task.WhenAll(downloadTasks);

            Console.WriteLine("\nToate imaginile au fost procesate.");
        }
    }
}