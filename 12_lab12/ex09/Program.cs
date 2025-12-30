using ex09.Services;
using System.ServiceModel.Syndication;

namespace ex09
{
    internal class Program
    {
        static void Main(string[] args)
        {
            IEnumerable<SyndicationItem> items = RSSFeedService.GetFeedItems("https://www.wired.com/feed/rss");

            if(items?.Count() > 0)
            {
                foreach(SyndicationItem item in items)
                {
                    Console.WriteLine(item.Categories?.FirstOrDefault()?.Name);
                }
            }
        }
    }
}