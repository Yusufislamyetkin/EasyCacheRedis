using EasyCacheRedis.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EasyCacheRedis.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IDistributedCache _distributedCache;
        public ProductsController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index()
        {
            Product p = new Product {  ID = 1 , Name="Pen", Price=25};
            string jsonproduct = JsonConvert.SerializeObject(p);

            DistributedCacheEntryOptions cacheEntryOptions = new DistributedCacheEntryOptions();
            cacheEntryOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(100);


            await _distributedCache.SetStringAsync("product:1", jsonproduct);

            return View();
        }


        public async Task<IActionResult> Show()
        {
            string jsonproduct = await _distributedCache.GetStringAsync("product:1");
            Product p = JsonConvert.DeserializeObject<Product>(jsonproduct);
       
            ViewBag.value = p;
            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name2");

            return View();
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/1.jpg");
            byte[] imageByte = System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("resim", imageByte);
            return View();
        }

        public IActionResult ImageUrl()
        {
           var value = _distributedCache.Get("resim");
            return File(value, "image/jpg");
        }
    }
}
