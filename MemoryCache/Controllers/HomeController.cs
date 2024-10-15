using MemoryCache.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace MemoryCache.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IMemoryCache _memoryCache;
        public HomeController(ILogger<HomeController> logger,IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index()
        {
            var cachedValue = await _memoryCache.GetOrCreateAsync<string>("catalog", cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10);// yenileme süresi 10 saniye
                return Task.FromResult($"DateTime:{DateTime.Now}");
            });
            ViewBag.CacheValue = cachedValue;

            var catalogList = new List<Catalog>();
            var catalogsCached = await _memoryCache.GetOrCreateAsync<List<Catalog>>("catalogList", cacheEntry =>
            {
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30);
                for (int i = 0; i < 1000; i++)
                {
                    catalogList.Add(new Catalog
                    {
                        Id = i + 1,
                        Name = $"Name {i + 1}",
                        Description = $"Description for item {i + 1}",
                        Price = (decimal)(i + 1) * 10, 
                        CreatedAt = DateTime.Now,
                        IsActive = true,
                        Tags = new List<string> { "Tag1", "Tag2" }
                    });
                }

                return Task.FromResult(catalogList);
            });
            ViewBag.CatalogList = catalogsCached;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
