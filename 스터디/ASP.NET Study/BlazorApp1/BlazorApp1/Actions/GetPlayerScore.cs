using System.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using BlazorApp1.Models;
namespace BlazorApp1.Actions;

public class GetPlayerScore : IApiService
{
    private readonly ScoreContext _scoreContext;
    private readonly IDatabase _redis;

    public GetPlayerScore(ScoreContext scoreContext, IDatabase redis)
    {
        _scoreContext = scoreContext;
        _redis = redis;
    }

    public Task<JObject> ProcessAsync(JObject param, string uid)
    {
        var scoreItem = _scoreContext.Scores.FirstOrDefault(item => item.user == uid);
        // .Include(p => p.id);

        // foreach (var scoreItem in scoreContextBook)
        // {
        //     var data = new StringBuilder();
        //     data.AppendLine($"id: {scoreItem.id}");
        //     data.AppendLine($"user: {scoreItem.user}");
        //     Console.WriteLine(data.ToString());
        // }
        
        return Task.FromResult(new JObject
        {
            ["score"] = scoreItem != null ? scoreItem.id : 0,
        });
    }
}
