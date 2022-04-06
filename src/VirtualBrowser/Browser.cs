using Microsoft.Extensions.Caching.Memory;
using PuppeteerSharp;
namespace VirtualBrowser
{
    public class Browser: IBrowser
    {
        private readonly IMemoryCache _memoryCache;
        public Browser(IMemoryCache cache)
        {
            _memoryCache = cache;
        }

        public async Task<Metadata> GetMetadata(string url)
        {
            return await _memoryCache.GetOrCreateAsync<Metadata>(url, async entry =>
            {
                var options = new LaunchOptions
                {
                    Headless = true
                };
                entry.SetAbsoluteExpiration(TimeSpan.FromDays(1));
                Console.WriteLine("Downloading chromium");
                using var browserFetcher = new BrowserFetcher();
                var res = await browserFetcher.DownloadAsync();
                options.ExecutablePath = Path.GetRelativePath(new FileInfo(" typeof(Browser).Assembly.Location").DirectoryName, res.ExecutablePath);

                using (var browser = await Puppeteer.LaunchAsync(options))
                using (var page = await browser.NewPageAsync())
                {
                    await page.GoToAsync(url);
                    var content = await page.GetContentAsync();
                    var graph = OpenGraphNet.OpenGraph.ParseHtml(content);
                    return new Metadata
                    {
                        Image = graph.Image,
                        Title = graph.Title,
                        Url = graph.Url ?? new Uri(url),
                        Description = graph.Metadata["og:description"] != null && graph.Metadata["og:description"].Count > 0 ? graph.Metadata["og:description"][0] : ""
                    };
                }
            });
        }
    }
}