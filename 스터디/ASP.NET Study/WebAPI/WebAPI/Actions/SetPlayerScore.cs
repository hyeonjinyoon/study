using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Caching.Memory;
using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using WebAPI.Models;
namespace WebAPI.Actions;

public class SetPlayerScore : IApiService
{
    private readonly ScoreContext _scoreContext;
    private readonly StackExchange.Redis.IDatabase _redis;

    public SetPlayerScore(ScoreContext scoreContext, StackExchange.Redis.IDatabase redis)
    {
        _scoreContext = scoreContext;
        _redis = redis;
    }

    public Task<JObject> ProcessAsync(JObject param, string uid)
    {
        var score = param.Value<int>("score");

        if (score < 0 || score > 10000)
        {
            return Task.FromResult(new JObject
            {
                ["status"] = "INVALID_REQUEST"
            });
        }


        const string redisKey = $"requestLock";
        var successLock = _redis.SetAdd(redisKey, uid);

        if (successLock)
        {
            var scoreItem = _scoreContext.Scores.FirstOrDefault(item => item.user == uid);

            if (scoreItem != null)
            {
                scoreItem.id = score;
            }
            else
            {
                _scoreContext.Scores.Add(new ScoreItem()
                {
                    user = uid,
                    id = score,
                });
            }

            _scoreContext.SaveChanges();
            _redis.SetRemove(redisKey, uid, CommandFlags.FireAndForget);

            return Task.FromResult(new JObject
            {
            });
        }
        else
        {
            return Task.FromResult(new JObject
            {
                ["status"] = "REQUEST_LOCK"
            });
        }
    }
}
