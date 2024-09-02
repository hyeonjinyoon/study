using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using WebAPI.Actions;
using WebAPI.Service;
using System.Runtime.Caching;
using CacheItemPriority = Microsoft.Extensions.Caching.Memory.CacheItemPriority;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Test2Controller : ControllerBase
    {
        private const string employeeListCacheKey = "employeeList";

        private readonly ILogger<Test2Controller> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestContext _requestContext;
        private IMemoryCache _cache;

        public Test2Controller(
            ILogger<Test2Controller> logger,
            IServiceProvider serviceProvider,
            RequestContext requestContext,
            IMemoryCache memoryCache)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _requestContext = requestContext;
            _cache = memoryCache;
        }

        // GET: api/Test2
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[]
            {
                "value1", "value2"
            };
        }

        [HttpPost]
        public async Task<string> PostAsync()
        {
            var actions = _requestContext.Actions ?? Array.Empty<JObject>();
            var response = new JArray();

            foreach (var actionJson in actions)
            {
                var resultJson = await ProcessActionAsync(actionJson, _requestContext.Uid);
                response.Add(resultJson);
            }

            return response.ToString();
        }

        private async Task<JObject> ProcessActionAsync(JObject json, string uid)
        {
            var action = json.Value<string>("action");

            var apiService = _serviceProvider.GetApiService(action);
            var response = apiService == default
                ? new JObject()
                {
                    ["status"] = "INVALID_REQUEST"
                }
                : await apiService.ProcessAsync(json, uid);

            response["action"] = action;

            if (response.ContainsKey("status") == false)
                response["status"] = "success";

            return response;
        }
    }
}
