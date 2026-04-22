using Lab3.UnitOfWorks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class CacheController : ControllerBase
    {
        private readonly CacheService _cache;
        public CacheController(CacheService cache)
        {
            _cache = cache;
        }
    }
}
