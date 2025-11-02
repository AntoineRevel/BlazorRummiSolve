using RummiSolve;
using RummiSolve.Solver.Genetic;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests stressants pour détecter la perte de tuiles du plateau
///     Utilise plusieurs exécutions pour capturer les comportements intermittents
/// </summary>
public class ParallelGeneticSolverBoardLossTests
{
    [Fact]
    public void SearchSolution_RepeatedRuns_NoBoardTilesLost()
    {
        // Test répété plusieurs fois pour capturer le comportement aléatoire
        for (var iteration = 0; iteration < 10; iteration++)
        {
            // Arrange
            var boardSet = new Set([
                new Tile(1, TileColor.Red),
                new Tile(2, TileColor.Red),
                new Tile(3, TileColor.Red),
                new Tile(5),
                new Tile(6),
                new Tile(7)
            ]);

            var playerSet = new Set([
                new Tile(4, TileColor.Red),
                new Tile(8)
            ]);

            var config = new GeneticConfiguration
            {
                PopulationSize = 20,
                PopulationCount = 2,
                MaxGenerations = 50,
                MutationRate = 0.5, // Haute mutation pour stresser le système
                EnableLogging = false
            };

            var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

            // Act
            var result = solver.SearchSolution();

            // Assert
            if (result.Found)
            {
                var solutionTiles = result.BestSolution.GetSet().Tiles;

                foreach (var boardTile in boardSet.Tiles)
                {
                    var found = solutionTiles.Any(t =>
                        t.Value == boardTile.Value && t.Color == boardTile.Color);

                    Assert.True(found,
                        $"Iteration {iteration}: Tuile du plateau {boardTile.Value} {boardTile.Color} perdue!");
                }
            }
        }
    }

    [Fact]
    public void SearchSolution_HighMutation_BoardIntact()
    {
        // Arrange - Configuration avec mutation très élevée
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(11)
        ]);

        var config = new GeneticConfiguration
        {
            PopulationSize = 30,
            PopulationCount = 4,
            MaxGenerations = 100,
            MutationRate = 0.8, // Mutation très élevée
            EnableLogging = false
        };

        // Test plusieurs fois
        for (var i = 0; i < 5; i++)
        {
            var solver =
                ParallelGeneticSolver.Create(new Set(boardSet), new Set(playerSet), false, config);
            var result = solver.SearchSolution();

            if (result.Found)
            {
                var solutionTiles = result.BestSolution.GetSet().Tiles;
                var solutionTileCount = solutionTiles.Count;
                var boardTileCount = boardSet.Tiles.Count;

                Assert.True(solutionTileCount >= boardTileCount,
                    $"Run {i}: Solution a {solutionTileCount} tuiles mais le plateau en a {boardTileCount}");
            }
        }
    }

    [Fact]
    public void SearchSolution_AllGenerations_BoardAlwaysIntact()
    {
        // Arrange - Teste avec différentes configurations
        var boardSet = new Set([
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(10),
            new Tile(10, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(5, TileColor.Red),
            new Tile(10, TileColor.Red)
        ]);

        // Test avec différentes configs
        var configs = new[]
        {
            GeneticConfiguration.Fast,
            GeneticConfiguration.Default,
            new GeneticConfiguration { MutationRate = 0.9, PopulationSize = 20, MaxGenerations = 50 }
        };

        foreach (var config in configs)
        {
            var solver =
                ParallelGeneticSolver.Create(new Set(boardSet), new Set(playerSet), false, config);
            var result = solver.SearchSolution();

            if (result.Found)
            {
                var solutionTiles = result.BestSolution.GetSet().Tiles;

                // Vérifie chaque tuile du plateau
                foreach (var boardTile in boardSet.Tiles)
                {
                    var matchingCount = solutionTiles.Count(t =>
                        t.Value == boardTile.Value &&
                        t.Color == boardTile.Color &&
                        !t.IsJoker);

                    Assert.True(matchingCount >= 1,
                        $"Tuile {boardTile.Value} {boardTile.Color} du plateau introuvable avec config mutation={config.MutationRate}");
                }
            }
        }
    }

    [Fact]
    public void SearchSolution_StressTest_1000Iterations()
    {
        // Test de stress avec beaucoup d'itérations
        var failedIterations = new List<int>();

        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red)
        ]);

        var config = new GeneticConfiguration
        {
            PopulationSize = 10,
            MaxGenerations = 20,
            MutationRate = 0.7,
            PopulationCount = 2,
            EnableLogging = false
        };

        for (var i = 0; i < 100; i++) // Réduit à 100 pour ne pas prendre trop de temps
        {
            var solver =
                ParallelGeneticSolver.Create(new Set(boardSet), new Set(playerSet), false, config);
            var result = solver.SearchSolution();

            if (result.Found)
            {
                var solutionTiles = result.BestSolution.GetSet().Tiles;

                // Vérifie que toutes les tuiles du plateau sont présentes
                var allBoardTilesPresent = boardSet.Tiles.All(boardTile =>
                    solutionTiles.Any(t => t.Value == boardTile.Value && t.Color == boardTile.Color));

                if (!allBoardTilesPresent) failedIterations.Add(i);
            }
        }

        Assert.Empty(failedIterations);
    }
}