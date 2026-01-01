using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using ex09.Services;
using System.ServiceModel.Syndication;

namespace ex09
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            string feedUrl = "https://www.wired.com/feed/rss";

            // BufferBlock pentru articolele RSS
            var bufferBlock = new BufferBlock<SyndicationItem>();

            // TransformBlock: extrage toate categoriile dintr-un item și le trimite mai departe
            var extractCategoriesBlock = new TransformManyBlock<SyndicationItem, string>(item =>
            {
                if (item.Categories == null || !item.Categories.Any())
                    return Enumerable.Empty<string>();

                return item.Categories
                    .Select(c => c.Name?.Trim())
                    .Where(name => !string.IsNullOrEmpty(name));
            });

            // Bag pentru colectarea unică a categoriilor (thread-safe)
            var uniqueCategories = new ConcurrentBag<string>();

            // ActionBlock: adaugă categoria în bag (cu conversie la majuscule)
            var collectBlock = new ActionBlock<string>(category =>
            {
                uniqueCategories.Add(category.ToUpper());
            });

            // ActionBlock final: afișează lista unică de categorii
            var displayBlock = new ActionBlock<ConcurrentBag<string>>(categories =>
            {
                var distinctCategories = categories.Distinct().OrderBy(c => c);

                Console.WriteLine("Categorii unice gasite in feed:");
                foreach (var category in distinctCategories)
                {
                    Console.WriteLine(category);
                }

                Console.WriteLine($"\nTotal categorii unice: {distinctCategories.Count()}");
            });

            // Link-uri cu propagare automată a finalizării
            var linkOptions = new DataflowLinkOptions { PropagateCompletion = true };

            bufferBlock.LinkTo(extractCategoriesBlock, linkOptions);
            extractCategoriesBlock.LinkTo(collectBlock, linkOptions);

            // După ce toate categoriile sunt colectate, trimitem bag-ul către displayBlock
            collectBlock.Completion.ContinueWith(_ =>
            {
                bufferBlock.Complete();
                extractCategoriesBlock.Complete();
                collectBlock.Complete();
                displayBlock.Post(uniqueCategories);
                displayBlock.Complete();
            });

            // Încărcăm feed-ul și trimitem articolele în buffer
            try
            {
                var items = RSSFeedService.GetFeedItems(feedUrl);
                foreach (var item in items)
                {
                    await bufferBlock.SendAsync(item);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la citirea feed-ului: {ex.Message}");
            }

            // Semnalăm finalul intrărilor
            bufferBlock.Complete();

            // Așteptăm finalizarea întregului pipeline
            await displayBlock.Completion;
        }
    }
}