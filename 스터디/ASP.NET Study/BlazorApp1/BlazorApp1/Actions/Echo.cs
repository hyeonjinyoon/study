using Newtonsoft.Json.Linq;
namespace BlazorApp1.Actions;

public class Echo : IApiService
{
    public Task<JObject> ProcessAsync(JObject param, string uid)
    {
        var text = param.Value<string>("text");
        
        return Task.FromResult(new JObject { ["text"] = text });
    }
}