using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace RummiSolve;

public class GlobalCache
{
    private static readonly Lazy<GlobalCache> instance = new Lazy<GlobalCache>(() => new GlobalCache());
    private ConcurrentDictionary<string, string> cache;

    private static readonly string Path =
        "/Users/antoinerevel/Documents/Projet perso/RummiSolve/RummiSolve/RummiSolve/cache.json";

    private GlobalCache()
    {
        cache = new ConcurrentDictionary<string, string>();
        LoadCacheFromFile();
    }

    public static GlobalCache Instance
    {
        get { return instance.Value; }
    }

    public bool TryGetSolution(string key, out string solutionKey)
    {
        return cache.TryGetValue(key, out solutionKey);
    }

    public void SetSolution(string key, string solutionKey)
    {
        cache[key] = solutionKey;
    }

    public void SaveCacheToFile()
    {
        var cacheData = JsonConvert.SerializeObject(cache);
        File.WriteAllText(Path, cacheData);
    }

    private void LoadCacheFromFile()
    {
        if (File.Exists(Path))
        {
            var cacheData = File.ReadAllText(Path);
            cache = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(cacheData);
        }
    }
}