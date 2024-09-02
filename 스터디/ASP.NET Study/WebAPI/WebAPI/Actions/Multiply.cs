using Newtonsoft.Json.Linq;
namespace WebAPI.Actions;

public class Multiply : IApiService
{
    public Task<JObject> ProcessAsync(JObject param, string uid)
    {
        var a = param.Value<int>("a");
        var b = param.Value<int>("b");

        return Task.FromResult(new JObject
        {
            ["sum"] = a * b
        });
    }
}
