using RummiSolve;
using RummiSolve.Solver.Genetic;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests pour vérifier que toutes les tuiles du plateau sont conservées dans la solution
/// </summary>
public class ParallelGeneticSolverBoardIntegrityTests
{
    [Fact]
    public void SearchSolution_SimpleBoardTiles_AllBoardTilesPresent()
    {
        // Arrange - Un plateau simple avec une suite
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");
        Assert.True(result.BestSolution.IsValid, "La solution devrait être valide");

        // Vérifie que toutes les tuiles du plateau sont dans la solution
        var solutionTiles = result.BestSolution.GetSet().Tiles;

        foreach (var boardTile in boardSet.Tiles)
        {
            var found = solutionTiles.Any(t => t.Value == boardTile.Value && t.Color == boardTile.Color);
            Assert.True(found,
                $"La tuile du plateau {boardTile.Value} {boardTile.Color} devrait être dans la solution");
        }
    }

    [Fact]
    public void SearchSolution_MultipleBoardRuns_AllBoardTilesPresent()
    {
        // Arrange - Plusieurs suites sur le plateau
        var boardSet = new Set([
            // Suite rouge
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            // Suite bleue
            new Tile(5),
            new Tile(6),
            new Tile(7)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(8)
        ]);

        var config = GeneticConfiguration.Default;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        var solutionTiles = result.BestSolution.GetSet().Tiles;
        var boardTileCount = boardSet.Tiles.Count;

        // Vérifie qu'on a au moins le nombre de tuiles du plateau dans la solution
        Assert.True(solutionTiles.Count >= boardTileCount,
            $"La solution devrait contenir au moins {boardTileCount} tuiles (du plateau)");

        // Vérifie chaque tuile du plateau
        foreach (var boardTile in boardSet.Tiles)
        {
            var matchingTiles = solutionTiles.Where(t =>
                t.Value == boardTile.Value && t.Color == boardTile.Color).ToList();

            Assert.True(matchingTiles.Any(),
                $"La tuile du plateau {boardTile.Value} {boardTile.Color} est manquante dans la solution!");
        }
    }

    [Fact]
    public void SearchSolution_BoardWithGroup_AllBoardTilesPresent()
    {
        // Arrange - Un groupe sur le plateau
        var boardSet = new Set([
            new Tile(7, TileColor.Red),
            new Tile(7),
            new Tile(7, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(7, TileColor.Mango),
            new Tile(8, TileColor.Red),
            new Tile(9, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        var solutionTiles = result.BestSolution.GetSet().Tiles;

        // Compte les tuiles de valeur 7 dans le plateau et dans la solution
        var boardSevenCount = boardSet.Tiles.Count(t => t.Value == 7);
        var solutionSevenCount = solutionTiles.Count(t => t.Value == 7);

        Assert.True(solutionSevenCount >= boardSevenCount,
            $"Toutes les tuiles 7 du plateau ({boardSevenCount}) doivent être dans la solution (trouvé: {solutionSevenCount})");
    }

    [Fact]
    public void SearchSolution_ComplexBoard_CountMatchesExactly()
    {
        // Arrange
        var boardSet = new Set([
            // Run 1
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            // Group
            new Tile(10, TileColor.Red),
            new Tile(10),
            new Tile(10, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(5, TileColor.Red),
            new Tile(11, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Default;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found);

        var solutionTiles = result.BestSolution.GetSet().Tiles;
        var boardTileCount = boardSet.Tiles.Count;
        var playerTilesPlayed = result.TilesToPlay.Count();

        // Le nombre total de tuiles dans la solution devrait être exactement
        // les tuiles du plateau + les tuiles jouées par le joueur
        var expectedTotal = boardTileCount + playerTilesPlayed;

        Assert.Equal(expectedTotal, solutionTiles.Count);
    }

    [Fact]
    public void SearchSolution_WithBoardJoker_JokerStillPresent()
    {
        // Arrange - Un joker sur le plateau
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(true) // Joker
        ]);

        var playerSet = new Set([
            new Tile(10),
            new Tile(10, TileColor.Red),
            new Tile(10, TileColor.Black)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found);

        var totalJokersInSolution = result.BestSolution.Runs.Sum(r => r.Jokers) +
                                    result.BestSolution.Groups.Sum(g => g.Jokers);

        // Le joker du plateau doit être présent dans la solution
        Assert.True(totalJokersInSolution >= 1,
            "Le joker du plateau doit être présent dans la solution");
    }

    [Fact]
    public void SearchSolution_LargeBoardManyTiles_AllPreserved()
    {
        // Arrange - Un grand plateau
        var boardSet = new Set([
            // Run 1
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            // Run 2
            new Tile(5),
            new Tile(6),
            new Tile(7),
            // Run 3
            new Tile(8, TileColor.Black),
            new Tile(9, TileColor.Black),
            new Tile(10, TileColor.Black),
            // Group
            new Tile(11, TileColor.Red),
            new Tile(11),
            new Tile(11, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(12, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Aggressive;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var result = solver.SearchSolution(cts.Token);

        // Assert
        Assert.True(result.Found);

        var solutionTiles = result.BestSolution.GetSet().Tiles;

        // Crée une liste pour compter les occurrences
        var boardTileList = boardSet.Tiles.ToList();
        var solutionTileList = solutionTiles.ToList();

        foreach (var boardTile in boardTileList)
        {
            var matchIndex = solutionTileList.FindIndex(t =>
                t.Value == boardTile.Value && t.Color == boardTile.Color && !t.IsJoker);

            Assert.True(matchIndex >= 0,
                $"Tuile du plateau manquante: {boardTile.Value} {boardTile.Color}");

            // Retire la tuile de la liste pour éviter les doubles comptages
            solutionTileList.RemoveAt(matchIndex);
        }
    }

    [Fact]
    public void SearchSolution_BoardTilesNotReorganized_StillAllPresent()
    {
        // Arrange - Cas où le joueur ne peut pas jouer toutes ses tuiles
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(10),
            new Tile(11),
            new Tile(12)
        ]);

        var playerSet = new Set([
            new Tile(5, TileColor.Black) // Ne peut pas être combinée facilement
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert - Même si aucune tuile du joueur n'est jouée, le plateau doit être intact
        var solutionTiles = result.BestSolution.GetSet().Tiles;

        Assert.True(solutionTiles.Count >= boardSet.Tiles.Count,
            "Le plateau doit au minimum contenir toutes ses tuiles d'origine");
    }

    [Fact]
    public void SearchSolution_EmptyPlayer_BoardUnmodified()
    {
        // Arrange - Joueur sans tuiles (cas limite)
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red)
        ]);

        var playerSet = new Set();

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        var solutionTiles = result.BestSolution.GetSet().Tiles;

        Assert.Equal(boardSet.Tiles.Count, solutionTiles.Count);

        foreach (var boardTile in boardSet.Tiles)
            Assert.Contains(solutionTiles, t =>
                t.Value == boardTile.Value && t.Color == boardTile.Color);
    }
}