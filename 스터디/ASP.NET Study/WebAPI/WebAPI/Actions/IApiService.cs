using Newtonsoft.Json.Linq;
namespace WebAPI.Actions;

public interface IApiService
{
    Task<JObject> ProcessAsync(JObject param,string uid);
}