using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazorApp1.Actions;
using BlazorApp1.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BlazorApp1
{
    [Route("api/[controller]")]
    [ApiController]
    public class APITestController : ControllerBase
    {
        private const string employeeListCacheKey = "employeeList";

        private readonly ILogger<APITestController> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RequestContext _requestContext;

        public APITestController(
            ILogger<APITestController> logger,
            IServiceProvider serviceProvider,
            RequestContext requestContext)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _requestContext = requestContext;
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
