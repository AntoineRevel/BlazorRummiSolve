using System.Diagnostics;
using RummiSolve.Results;
using RummiSolve.Solver.Genetic;
using RummiSolve.Solver.Interfaces;

namespace RummiSolve.Strategies;

/// <summary>
///     Stratégie simple utilisant uniquement le ParallelGeneticSolver
///     Sans hybridation avec d'autres solvers
/// </summary>
public class PureGeneticStrategy : ISolverStrategy
{
    private readonly GeneticConfiguration _config;
    private readonly bool _enableLogging;

    /// <summary>
    ///     Crée une stratégie génétique pure avec configuration personnalisée
    /// </summary>
    /// <param name="config">Configuration du solver génétique (null = configuration par défaut)</param>
    /// <param name="enableLogging">Active les logs de performance</param>
    public PureGeneticStrategy(GeneticConfiguration? config = null, bool enableLogging = false)
    {
        _config = config ?? GeneticConfiguration.Default;
        _enableLogging = enableLogging;
    }

    public async Task<SolverResult> GetSolverResult(
        Set board,
        Set rack,
        bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // Crée le solver génétique approprié
        ISolver geneticSolver;

        if (!hasPlayed)
        {
            // Premier coup : configuration optimisée pour trouver 30+ points
            var firstPlayConfig = new GeneticConfiguration
            {
                PopulationSize = _config.PopulationSize / 2, // Moins de population car problème plus simple
                PopulationCount = Math.Max(1, _config.PopulationCount / 2),
                MaxGenerations = _config.MaxGenerations,
                MutationRate = _config.MutationRate * 1.5, // Plus de mutations pour explorer
                EliteSize = Math.Max(3, _config.EliteSize / 2),
                EnableLogging = _config.EnableLogging
            };

            geneticSolver = ParallelGeneticSolver.Create(new Set(), rack, true, firstPlayConfig);
        }
        else
        {
            // Coups suivants : configuration normale
            geneticSolver = ParallelGeneticSolver.Create(new Set(board), rack, false, _config);
        }

        // Exécute le solver de manière asynchrone
        var result = await Task.Run(() => geneticSolver.SearchSolution(cancellationToken), cancellationToken);

        stopwatch.Stop();

        if (_enableLogging) LogResult(result, stopwatch.ElapsedMilliseconds, hasPlayed);

        return result;
    }

    /// <summary>
    ///     Crée une stratégie avec un profil prédéfini
    /// </summary>
    public static PureGeneticStrategy CreateWithProfile(GeneticProfile profile)
    {
        var config = profile switch
        {
            GeneticProfile.Fast => GeneticConfiguration.Fast,
            GeneticProfile.Balanced => GeneticConfiguration.Default,
            GeneticProfile.Thorough => GeneticConfiguration.Aggressive,
            GeneticProfile.UltraFast => new GeneticConfiguration
            {
                PopulationSize = 25,
                PopulationCount = 1,
                MaxGenerations = 50,
                MutationRate = 0.2,
                EliteSize = 5
            },
            _ => GeneticConfiguration.Default
        };

        return new PureGeneticStrategy(config, false);
    }

    private void LogResult(SolverResult result, long elapsedMs, bool hasPlayed)
    {
        var phase = hasPlayed ? "Subsequent play" : "First play";
        Console.WriteLine($"[PureGeneticStrategy - {phase}] Completed in {elapsedMs}ms");

        if (result.Found)
        {
            Console.WriteLine("  ✓ Solution found");
            Console.WriteLine($"  - Tiles to play: {result.TilesToPlay.Count()}");
            Console.WriteLine($"  - Score: {result.Score}");
            Console.WriteLine($"  - Won: {result.Won}");
        }
        else
        {
            Console.WriteLine("  ✗ No solution found");
        }
    }
}

/// <summary>
///     Profils prédéfinis pour la stratégie génétique
/// </summary>
public enum GeneticProfile
{
    /// <summary>
    ///     Ultra rapide - pour les décisions en temps réel (< 100ms typiquement)
    /// </summary>
    UltraFast,

    /// <summary>
    ///     Rapide - bon compromis vitesse/qualité (< 500ms)
    /// </summary>
    Fast,

    /// <summary>
    ///     Équilibré - configuration par défaut (< 2s)
    /// </summary>
    Balanced,

    /// <summary>
    ///     Approfondi - recherche plus exhaustive (< 10s)
    /// </summary>
    Thorough
}

/// <summary>
///     Version simplifiée avec configuration automatique
/// </summary>
public class SimpleGeneticStrategy : ISolverStrategy
{
    public Task<SolverResult> GetSolverResult(
        Set board,
        Set rack,
        bool hasPlayed,
        CancellationToken cancellationToken = default)
    {
        // Utilise une configuration adaptative simple basée sur la complexité
        var tileCount = board.Tiles.Count + rack.Tiles.Count;

        var profile = tileCount switch
        {
            < 20 => GeneticProfile.UltraFast,
            < 40 => GeneticProfile.Fast,
            < 60 => GeneticProfile.Balanced,
            _ => GeneticProfile.Thorough
        };

        var strategy = PureGeneticStrategy.CreateWithProfile(profile);
        return strategy.GetSolverResult(board, rack, hasPlayed, cancellationToken);
    }
}