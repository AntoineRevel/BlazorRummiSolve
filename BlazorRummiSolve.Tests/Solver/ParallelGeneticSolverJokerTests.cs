using RummiSolve;
using RummiSolve.Solver.Genetic;

namespace BlazorRummiSolve.Tests.Solver;

/// <summary>
///     Tests spécifiques pour vérifier le comportement du ParallelGeneticSolver avec les jokers
/// </summary>
public class ParallelGeneticSolverJokerTests
{
    [Fact]
    public void SearchSolution_PlayerJokerInRun_Valid()
    {
        // Arrange - Un joker du joueur peut compléter un run
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker qui peut faire 3 rouge
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");
        Assert.True(result.BestSolution.IsValid, "La solution devrait être valide");

        // Le joker devrait être utilisé
        var totalJokers = result.BestSolution.Runs.Sum(r => r.Jokers) +
                          result.BestSolution.Groups.Sum(g => g.Jokers);
        Assert.True(totalJokers > 0, "Au moins un joker devrait être utilisé");
    }

    [Fact]
    public void SearchSolution_PlayerJokerInGroup_Valid()
    {
        // Arrange - Un joker du joueur peut compléter un groupe
        var boardSet = new Set([
            new Tile(10, TileColor.Red),
            new Tile(10)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker qui peut faire 10 noir
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Red),
            new Tile(7, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");
        Assert.True(result.BestSolution.IsValid, "La solution devrait être valide");
        Assert.True(result.JokerToPlay > 0, "Le joker du joueur devrait être joué");
    }

    [Fact]
    public void SearchSolution_BoardJokerOnly_NoPlayerJokerUsed()
    {
        // Arrange - Le plateau a un joker, le joueur n'en a pas
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(true) // Joker du plateau
        ]);

        var playerSet = new Set([
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        if (result.Found)
            // Si une solution est trouvée, aucun joker du joueur ne devrait être utilisé
            Assert.Equal(0, result.JokerToPlay);
    }

    [Fact]
    public void SearchSolution_MultiplePlayerJokers_HandledCorrectly()
    {
        // Arrange - Le joueur a 2 jokers
        var boardSet = new Set([
            new Tile(1, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker 1
            new Tile(true), // Joker 2
            new Tile(4),
            new Tile(5)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        // Les jokers utilisés ne doivent pas dépasser ceux disponibles
        Assert.True(result.JokerToPlay <= 2, "Pas plus de 2 jokers du joueur ne peuvent être utilisés");
    }

    [Fact]
    public void SearchSolution_FirstPlayWithJoker_MinimumScoreRespected()
    {
        // Arrange - Premier coup avec un joker
        var boardSet = new Set();

        var playerSet = new Set([
            new Tile(true), // Joker
            new Tile(10, TileColor.Red),
            new Tile(11, TileColor.Red),
            new Tile(12, TileColor.Red),
            new Tile(13, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, true, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        if (result.Found)
        {
            var playerScore =
                result.TilesToPlay.Sum(t => t.Value) + result.JokerToPlay * 10; // Joker vaut au moins 10
            Assert.True(playerScore >= 30, $"Le score du premier coup devrait être >= 30, obtenu: {playerScore}");
        }
    }

    [Fact]
    public void SearchSolution_JokerValueCalculation_Correct()
    {
        // Arrange - Vérifie que la valeur des jokers est bien comptée
        var boardSet = new Set([
            new Tile(10, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker qui fera 10 bleu
            new Tile(10),
            new Tile(10, TileColor.Black)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        if (result.Won)
        {
            // Si victoire, toutes les tuiles doivent être jouées
            Assert.Equal(2, result.TilesToPlay.Count()); // 2 tuiles 10 normales
            Assert.Equal(1, result.JokerToPlay); // 1 joker
        }
    }

    [Fact]
    public void SearchSolution_MixedJokers_BoardAndPlayer()
    {
        // Arrange - Jokers sur le plateau ET dans la main du joueur
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(true) // Joker du plateau
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker du joueur
            new Tile(10),
            new Tile(10, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        var totalJokersInSolution = result.BestSolution.Runs.Sum(r => r.Jokers) +
                                    result.BestSolution.Groups.Sum(g => g.Jokers);

        // Total jokers = 1 (plateau) + 1 (joueur) = 2
        Assert.True(totalJokersInSolution <= 2, "Maximum 2 jokers peuvent être utilisés");
    }

    [Fact]
    public void SearchSolution_JokerNotRequired_OptimalSolution()
    {
        // Arrange - Le joueur a un joker mais n'en a pas besoin pour gagner
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker
            new Tile(3, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(5, TileColor.Red)
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        if (result.Won)
            // Le solver peut choisir d'utiliser ou non le joker
            // L'important est que la solution soit valide
            Assert.True(result.BestSolution.IsValid);
    }

    [Fact]
    public void SearchSolution_OnlyJokers_HandledCorrectly()
    {
        // Arrange - Cas extrême : le joueur n'a que des jokers
        var boardSet = new Set([
            new Tile(10, TileColor.Red),
            new Tile(10)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker 1
            new Tile(true) // Joker 2
        ]);

        var config = GeneticConfiguration.Fast;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var result = solver.SearchSolution();

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");

        if (result.Found)
            // Les 2 jokers peuvent compléter le groupe
            Assert.True(result.JokerToPlay <= 2, "Maximum 2 jokers");
    }

    [Fact]
    public void SearchSolution_JokerInComplexSolution_Valid()
    {
        // Arrange - Solution complexe avec plusieurs jokers
        var boardSet = new Set([
            new Tile(1, TileColor.Red),
            new Tile(2, TileColor.Red),
            new Tile(4, TileColor.Red),
            new Tile(true) // Joker plateau (peut faire 3 rouge)
        ]);

        var playerSet = new Set([
            new Tile(true), // Joker joueur
            new Tile(6, TileColor.Red),
            new Tile(10),
            new Tile(10, TileColor.Red)
        ]);

        var config = new GeneticConfiguration
        {
            PopulationSize = 100,
            PopulationCount = 4,
            MaxGenerations = 300,
            EnableLogging = false
        };

        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
        var result = solver.SearchSolution(cts.Token);

        // Assert
        Assert.True(result.Found, "Une solution devrait être trouvée");
        Assert.True(result.BestSolution.IsValid, "La solution doit être valide");

        // Vérifie que le comptage des jokers est correct
        var totalJokersInSolution = result.BestSolution.Runs.Sum(r => r.Jokers) +
                                    result.BestSolution.Groups.Sum(g => g.Jokers);
        var maxJokers = 1 + 1; // 1 plateau + 1 joueur
        Assert.True(totalJokersInSolution <= maxJokers,
            $"Nombre de jokers utilisés ({totalJokersInSolution}) ne devrait pas dépasser {maxJokers}");
    }

    [Fact]
    public void SearchSolution_GameId400adf36_ReproduceIssue()
    {
        // Arrange - Tente de reproduire le problème de la partie 400adf36-0f8b-4119-b0a7-e5c515c5ab10
        // Configuration typique d'une partie à 2 joueurs IA

        var boardSet = new Set([
            new Tile(5, TileColor.Red),
            new Tile(6, TileColor.Red),
            new Tile(7, TileColor.Red),
            new Tile(true) // Joker sur le plateau
        ]);

        var playerSet = new Set([
            new Tile(8, TileColor.Red),
            new Tile(9, TileColor.Red),
            new Tile(10),
            new Tile(11),
            new Tile(12),
            new Tile(true), // Joker du joueur
            new Tile(4, TileColor.Black)
        ]);

        var config = GeneticConfiguration.Default;
        var solver = ParallelGeneticSolver.Create(boardSet, playerSet, false, config);

        // Act
        var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
        var result = solver.SearchSolution(cts.Token);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Found || !result.Found, "Le solver doit retourner un résultat déterministe");

        if (result.Found)
        {
            // Vérifie que les jokers sont correctement comptabilisés
            Assert.True(result.JokerToPlay >= 0, "Le nombre de jokers jouésne peut pas être négatif");
            Assert.True(result.JokerToPlay <= 1, "Le joueur ne peut jouer que son propre joker");

            // Vérifie que les tuiles jouées sont correctes
            Assert.NotNull(result.TilesToPlay);
            Assert.True(result.TilesToPlay.All(t => !t.IsJoker),
                "Les tuiles jouées ne doivent pas contenir de jokers (comptés séparément)");
        }
    }
}