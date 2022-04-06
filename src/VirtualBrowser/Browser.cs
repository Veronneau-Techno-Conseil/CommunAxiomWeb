using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
namespace VirtualBrowser
{
    public class Browser: IBrowser
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;
        public Browser(IMemoryCache cache, ILogger<Browser> logger)
        {
            _memoryCache = cache;
            _logger = logger;
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
                _logger.LogInformation("Downloading chromium");
                using var browserFetcher = new BrowserFetcher();
                var res = await browserFetcher.DownloadAsync();
                _logger.LogInformation("Downloading complete");
                _logger.LogInformation(Newtonsoft.Json.JsonConvert.SerializeObject(res));
                var directory = Environment.CurrentDirectory;
                _logger.LogInformation($"Resolving path to chromium relative to {directory}");
                options.ExecutablePath = Path.GetRelativePath(directory, res.ExecutablePath);
                _logger.LogInformation($"Using path {options.ExecutablePath}");

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