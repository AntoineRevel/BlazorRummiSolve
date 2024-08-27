using System.Collections.Concurrent;

namespace RummiSolve;

public class GlobalCache
{
    private static readonly Lazy<GlobalCache> Inst = new(() => new GlobalCache());

    private readonly ConcurrentDictionary<string, Solution?> _cache;

    private GlobalCache()
    {
        _cache = new ConcurrentDictionary<string, Solution?>();
    }

    public static GlobalCache Instance => Inst.Value;

    public bool TryGetSolution(string key, out Solution? solution)
    {
        return _cache.TryGetValue(key, out solution);
    }

    public void SetSolution(string key, Solution? solution)
    {
        _cache[key] = solution;
    }
}