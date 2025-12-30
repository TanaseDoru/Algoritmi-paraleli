using ex08.Services;
using System.ServiceModel.Syndication;

namespace ex08
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<SyndicationItem> items_1 = RSSFeedService.GetFeedItems("https://devblogs.microsoft.com/dotnet/feed/");
            IEnumerable<SyndicationItem> items_2 = RSSFeedService.GetFeedItems("https://www.microsoft.com/microsoft-365/blog/feed/");
            IEnumerable<SyndicationItem> items_3 = RSSFeedService.GetFeedItems("https://feeds.feedburner.com/TechCrunch/");
            IEnumerable<SyndicationItem> items_4 = RSSFeedService.GetFeedItems("https://www.wired.com/feed/rss");

            if(items_1?.Count() > 0)
            {
                foreach(SyndicationItem item in items_1)
                {
                    Console.WriteLine(item.Title.Text.ToString());
                }
            }
            if (items_2?.Count() > 0)
            {
                foreach (SyndicationItem item in items_2)
                {
                    Console.WriteLine(item.Title.Text.ToString());
                }
            }
            if (items_3?.Count() > 0)
            {
                foreach (SyndicationItem item in items_3)
                {
                    Console.WriteLine(item.Title.Text.ToString());
                }
            }
            if (items_4?.Count() > 0)
            {
                foreach (SyndicationItem item in items_4)
                {
                    Console.WriteLine(item.Title.Text.ToString());
                }
            }
        }
    }
}