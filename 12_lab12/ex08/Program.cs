using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.ServiceModel.Syndication;
using ex08.Services;
using ex08.Models;

namespace ex08
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            // 1. Buffer comun pentru toate articolele
            var buffer = new BufferBlock<Post>();

            // 2. ActionBlock care procesează articolele
            var displayBlock = new ActionBlock<Post>(post =>
            {
                Console.WriteLine($"[{post.Date}] {post.Title}");
                if (post.Categories.Count > 0)
                {
                    Console.WriteLine($"   Categories: {string.Join(", ", post.Categories)}");
                }
                Console.WriteLine();
            });

            // Link cu propagare automată a finalizării
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };
            buffer.LinkTo(displayBlock, linkOptions);

            // 3. Lista cu URL-urile RSS
            var feedUrls = new[]
            {
                "https://devblogs.microsoft.com/dotnet/feed/",
                "https://feeds.feedburner.com/TechCrunch/",
                "https://www.microsoft.com/microsoft-365/blog/feed/",
                "https://www.wired.com/feed/rss"
            };

            // 4. Sarcini pentru citirea paralelă a feed-urilor
            var feedTasks = new List<Task>();

            foreach (var url in feedUrls)
            {
                feedTasks.Add(Task.Run(async () =>
                {
                    try
                    {
                        var items = RSSFeedService.GetFeedItems(url);
                        foreach (var item in items)
                        {
                            var post = new Post
                            {
                                Title = item.Title.Text,
                                Date = item.PublishDate != DateTimeOffset.MinValue
                                    ? item.PublishDate.ToString("yyyy-MM-dd HH:mm")
                                    : item.LastUpdatedTime.ToString("yyyy-MM-dd HH:mm"),
                                Categories = item.Categories?.Select(c => c.Name).ToList() ?? new List<string>(),
                                Content = item.Summary?.Text
                            };

                            await buffer.SendAsync(post);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Eroare la citirea feed-ului {url}: {ex.Message}");
                    }
                }));
            }

            // 5. Așteptăm toate feed-urile să termine
            await Task.WhenAll(feedTasks);

            // 6. Semnalăm că nu mai vin date
            buffer.Complete();

            // 7. Așteptăm procesarea finală
            await displayBlock.Completion;

            Console.WriteLine("Toate articolele au fost procesate.");
            Console.ReadKey();
        }
    }
}