using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PuppeteerSharp;
namespace VirtualBrowser
{
    public class Browser: IBrowser
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;
        public Browser(IMemoryCache cache, ILogger<Browser> logger, IConfiguration configuration)
        {
            _memoryCache = cache;
            _logger = logger;
            _configuration = configuration;
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
                
                if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH")))
                {
                    Environment.SetEnvironmentVariable("PUPPETEER_EXECUTABLE_PATH", _configuration["ChromePath"]);
                }

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