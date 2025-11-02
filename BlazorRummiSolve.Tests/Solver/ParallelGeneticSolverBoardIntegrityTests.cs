using RummiSolve;
using RummiSolve.Solver.Genetic;
using Xunit.Abstractions;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests pour vérifier que toutes les tuiles du plateau sont conservées dans la solution
/// </summary>
public class ParallelGeneticSolverBoardIntegrityTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public ParallelGeneticSolverBoardIntegrityTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

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
    public void SearchSolution_BoardTilesNotReorganized_NoSolutionFound()
    {
        // Arrange - Cas où le joueur ne peut pas jouer ses tuiles
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(3, TileColor.Red),
            new Tile(10),
            new Tile(11),
            new Tile(12)
        ]);

        var playerSet = new Set([
            new Tile(5, TileColor.Black) // Ne peut pas être combinée
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert - Le joueur ne peut rien jouer, donc pas de solution
        Assert.False(result.Found, "Aucune solution ne devrait être trouvée si le joueur ne peut rien jouer");
    }

    [Fact]
    public void SearchSolution_EmptyPlayer_NoSolutionFound()
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

        // Assert - Sans tuiles, le joueur ne peut rien jouer
        Assert.False(result.Found, "Aucune solution ne devrait être trouvée si le joueur n'a pas de tuiles");
    }

    [Fact]
    public void SearchSolution_BoardWith2Groups_ShouldNotUsePlayerTileInsteadOfBoardTile()
    {
        // Arrange - Reproduit le bug où le solveur joue le 2 noir alors qu'il est sur le plateau
        // Configuration exacte du jeu problématique:
        // Plateau: 2 bleu, 2 orange, 2 noir, 13 bleu, 13 rouge, 13 noir (2 groupes)
        // Rack joueur: 1 orange, 11 bleu, 3 noir, 4 bleu, 9 noir, 6 noir, 3 orange, 2 Noir
        // Résultat attendu: Aucune solution trouvée (le joueur ne peut rien jouer)

        var boardSet = new Set([
            // Groupe de 2
            new Tile(2),
            new Tile(2, TileColor.Mango),
            new Tile(2, TileColor.Black),
            // Groupe de 13
            new Tile(13),
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Mango),
            new Tile(11),
            new Tile(3, TileColor.Black),
            new Tile(4),
            new Tile(9, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(3, TileColor.Mango),
            new Tile(2, TileColor.Black)
        ]);

        var config = GeneticConfiguration.Default;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var result = solver.SearchSolution(cts.Token);

        // Assert - Le joueur ne devrait pas pouvoir jouer avec ces tuiles
        if (result.Found)
        {
            // Debug: affiche la solution trouvée pour comprendre le bug
            var solutionTiles = result.BestSolution.GetSet().Tiles;
            var playedTiles = result.TilesToPlay.ToList();

            var message = $"Une solution a été trouvée alors qu'elle ne devrait pas!\n" +
                          $"Tuiles jouées: {string.Join(", ", playedTiles.Select(t => $"{t.Value} {t.Color}"))}\n" +
                          $"Score: {result.Score}\n" +
                          $"Runs: {result.BestSolution.Runs.Count}, Groups: {result.BestSolution.Groups.Count}";

            Assert.Fail(message);
        }
    }

    [Fact]
    public void SearchSolution_ComplexBoardWithMultiple9s_ShouldPlayOptimalSolution()
    {
        // Arrange - Configuration du tour 8 du jeu 87d8849e-8739-4b5f-871b-4ee74bc90f13
        // Plateau: 11-12-13 (bleu), 2-2-2 (bleu-orange-noir), 8-8-8 (bleu-orange-noir),
        //          9-9-9 (bleu-rouge-noir), 13-13-13 (rouge-orange-noir)
        // Rack: 1 orange, 11 bleu, 3 noir, 4 bleu, 9 noir, 6 noir, 3 orange, 2 noir,
        //       7 orange, 5 bleu, 9 bleu, 5 noir, 9 orange
        // Le solveur devrait pouvoir jouer au moins le 9 orange, idéalement optimiser davantage

        var boardSet = new Set([
            // Suite 11-12-13 (bleu? à vérifier selon les couleurs du rack)
            new Tile(11, TileColor.Red),
            new Tile(12, TileColor.Red),
            new Tile(13, TileColor.Red),
            // Groupe de 2
            new Tile(2),
            new Tile(2, TileColor.Mango),
            new Tile(2, TileColor.Black),
            // Groupe de 8
            new Tile(8),
            new Tile(8, TileColor.Mango),
            new Tile(8, TileColor.Black),
            // Groupe de 9
            new Tile(9),
            new Tile(9, TileColor.Red),
            new Tile(9, TileColor.Black),
            // Groupe de 13
            new Tile(13, TileColor.Mango),
            new Tile(13),
            new Tile(13, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(1, TileColor.Mango),
            new Tile(11),
            new Tile(3, TileColor.Black),
            new Tile(4),
            new Tile(9, TileColor.Black),
            new Tile(6, TileColor.Black),
            new Tile(3, TileColor.Mango),
            new Tile(2, TileColor.Black),
            new Tile(7, TileColor.Mango),
            new Tile(5),
            new Tile(9),
            new Tile(5, TileColor.Black),
            new Tile(9, TileColor.Mango)
        ]);

        var config = GeneticConfiguration.Default;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
        var result = solver.SearchSolution(cts.Token);

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");
        Assert.Equal(3, result.TilesToPlay.Count());
    }

    [Fact]
    public void SearchSolution_Turn11_ShouldFindSolutionWithReorganization()
    {
        // Arrange - Configuration du tour 11 du jeu 87d8849e-8739-4b5f-871b-4ee74bc90f13
        // Le ParallelGeneticSolver peut avoir des difficultés à trouver une solution
        //
        // Plateau: 11-12-13 (bleu), 1-2-3 (orange),
        //          2-2-2 (bleu-rouge-noir), 8-8-8 (bleu-orange-noir),
        //          9-9-9 (bleu-rouge-noir), 9-9-9 (bleu-orange-noir), 13-13-13 (rouge-orange-noir)
        //
        // Rack BobNew: 6 noir, 4 noir, 2 noir, 13 noir, 1 noir, 2 bleu, 10 noir, 6 rouge,
        //              10 rouge, 8 noir, 11 rouge, 7 bleu, 5 noir, J (joker)
        //
        // Solutions possibles:
        // - Jouer 10 noir + 8 noir
        // - Jouer 2 rouge, 10 rouge, 11 rouge, 7 bleu (comme montré dans l'UI)

        // Teste plusieurs fois car l'algorithme génétique est aléatoire
        const int iterations = 5;
        var successCount = 0;
        var solutions = new List<string>();

        for (var i = 0; i < iterations; i++)
        {
            var boardSet = new Set([
                // Suite 11-12-13 (bleu)
                new Tile(11),
                new Tile(12),
                new Tile(13),
                // Suite 1-2-3 (orange)
                new Tile(1, TileColor.Mango),
                new Tile(2, TileColor.Mango),
                new Tile(3, TileColor.Mango),
                // Groupe de 2
                new Tile(2),
                new Tile(2, TileColor.Red),
                new Tile(2, TileColor.Black),
                // Groupe de 8
                new Tile(8),
                new Tile(8, TileColor.Mango),
                new Tile(8, TileColor.Black),
                // Premier groupe de 9
                new Tile(9),
                new Tile(9, TileColor.Red),
                new Tile(9, TileColor.Black),
                // Deuxième groupe de 9
                new Tile(9),
                new Tile(9, TileColor.Mango),
                new Tile(9, TileColor.Black),
                // Groupe de 13
                new Tile(13, TileColor.Red),
                new Tile(13, TileColor.Mango),
                new Tile(13, TileColor.Black)
            ]);

            var playerSet = new Set([
                new Tile(6, TileColor.Black),
                new Tile(4, TileColor.Black),
                new Tile(2, TileColor.Black),
                new Tile(13, TileColor.Black),
                new Tile(1, TileColor.Black),
                new Tile(2),
                new Tile(10, TileColor.Black),
                new Tile(6, TileColor.Red),
                new Tile(10, TileColor.Red),
                new Tile(8, TileColor.Black),
                new Tile(11, TileColor.Red),
                new Tile(7),
                new Tile(5, TileColor.Black),
                new Tile(true) // Joker
            ]);

            // Test avec configuration Fast (comme peut-être utilisée dans l'app?)
            var config = GeneticConfiguration.Fast;
            var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

            // Act - Timeout court comme dans l'application Blazor
            var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var result = solver.SearchSolution(cts.Token);

            if (result.Found)
            {
                successCount++;
                var playedTiles = result.TilesToPlay.ToList();
                var solutionDesc =
                    $"[{playedTiles.Count} tuiles, score {result.Score}]: {string.Join(", ", playedTiles.Select(t => t.IsJoker ? "J" : $"{t.Value}{t.Color.ToString()[0]}"))}";
                solutions.Add(solutionDesc);
            }
        }

        // Affiche les résultats
        Console.WriteLine($"=== Résultat du solveur Turn 11 sur {iterations} itérations ===");
        Console.WriteLine($"Succès: {successCount}/{iterations}");
        Console.WriteLine("Solutions trouvées:");
        foreach (var sol in solutions) Console.WriteLine($"  - {sol}");

        _testOutputHelper.WriteLine($"=== Résultat du solveur Turn 11 sur {iterations} itérations ===");
        _testOutputHelper.WriteLine($"Succès: {successCount}/{iterations}");
        _testOutputHelper.WriteLine("Solutions trouvées:");
        foreach (var sol in solutions) _testOutputHelper.WriteLine($"  - {sol}");

        // Le solveur devrait trouver une solution dans au moins une itération
        Assert.True(successCount > 0,
            $"Le solveur devrait trouver une solution dans au moins une des {iterations} itérations");
    }

    [Fact]
    public void SearchSolution_Turn7_ShouldPlayOptimalReorganization()
    {
        // Arrange - Configuration du tour 7 du jeu d8b42678-0068-4aa1-a087-17232f2b65fe

        var boardSet = new Set([
            // Groupe de 7
            new Tile(7),
            new Tile(7, TileColor.Mango),
            new Tile(7, TileColor.Black),

            // Groupe de 5
            new Tile(5),
            new Tile(5, TileColor.Red),
            new Tile(5, TileColor.Mango),

            // Groupe de 11 avec joker
            new Tile(11, TileColor.Red),
            new Tile(11, TileColor.Black),
            new Tile(true), // Joker
            // Groupe de 13
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Mango),
            new Tile(13, TileColor.Black)
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(7, TileColor.Black),
            new Tile(6, TileColor.Mango), // 6 orange - OK
            new Tile(7, TileColor.Red), // 7 rouge - OK
            new Tile(5, TileColor.Red),
            new Tile(5, TileColor.Black), // 5 noir - OK
            new Tile(1, TileColor.Black),
            new Tile(8),
            new Tile(11, TileColor.Black),
            new Tile(10, TileColor.Mango),
            new Tile(8, TileColor.Mango), // 8 orange - OK
            new Tile(13, TileColor.Red),
            new Tile(13, TileColor.Mango)
        ]);

        var config = GeneticConfiguration.UltraAggressive;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act - Timeout plus long pour cette situation complexe
        var result = solver.SearchSolution();

        Assert.True(result.Found);
        Assert.Equal(4, result.TilesToPlay.Count());
    }
}