using System.Reflection;
namespace WebAPI.Actions;

public static class ApiServiceExtensions
{
    private static readonly Dictionary<string, Type> _table;

    static ApiServiceExtensions()
    {
        var apiType = typeof(IApiService);

        _table = Assembly
            .GetExecutingAssembly()
            .GetTypes()
            .Where(_ => apiType.IsAssignableFrom(_) && _.IsClass && !_.IsAbstract)
            .ToDictionary(_ => _.Name.ToSnakeCase(), _ => _);
    }

    public static IServiceCollection AddApiService(this IServiceCollection serviceCollection)
    {
        foreach (var type in _table.Values)
            serviceCollection.AddScoped(type);
        return serviceCollection;
    }

    public static IApiService? GetApiService(this IServiceProvider serviceProvider, string? name)
    {
        if (string.IsNullOrEmpty(name)) { return default; }
        if (!_table.TryGetValue(name, out var type)) { return default; }
        return (IApiService)serviceProvider.GetRequiredService(type);
    }
}
