using Newtonsoft.Json.Linq;
namespace BlazorApp1.Actions;

public interface IApiService
{
    Task<JObject> ProcessAsync(JObject param,string uid);
}