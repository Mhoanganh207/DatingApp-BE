using System.Text.Json;
using StackExchange.Redis;

namespace DatingApp.Services;

public class CacheService : ICacheService {
    
    private readonly IDatabase _cache;

    public CacheService(IDatabase cache)
    {
        var redis = ConnectionMultiplexer.Connect("localhost:6379");
        _cache = redis.GetDatabase();
    }
    
    public T GetData<T>(string key)
    {
        var data = _cache.StringGet(key);
        if (!string.IsNullOrEmpty(data))
        {
            return JsonSerializer.Deserialize<T>(data);
        }

        return default;
    }

    public bool SetData<T>(string key, T data, DateTimeOffset expirationTime)
    {
        var expiration = expirationTime.Subtract(DateTimeOffset.Now);
        return _cache.StringSet(key, JsonSerializer.Serialize(data), expiration);
    }

    public object RemoveData(string key)
    {
        var isExist = _cache.KeyExists(key);
        if (isExist)
        {
            return _cache.KeyDelete(key);
        }

        return false;
    }
}