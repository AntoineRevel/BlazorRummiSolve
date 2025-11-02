using RummiSolve.Results;
using RummiSolve.Solver.Genetic;
using RummiSolve.Strategies;
using static System.Console;

namespace RummiSolve;

public class Game(Guid id)
{
    private readonly Queue<Tile> _tilePool = new();

    public Game() : this(Guid.NewGuid())
    {
    }

    public Guid Id { get; } = id;
    public List<Player> Players { get; } = [];
    public int Turn { get; private set; }
    public int PlayerIndex { get; private set; }
    public Solution Board { get; private set; } = new();
    public bool IsGameOver { get; private set; }

    public void InitializeGame(List<string> playerNames, List<bool>? playerTypes = null,
        Func<Set, bool, CancellationToken, Task<SolverResult>>? humanPlayerCallback = null)
    {
        if (playerNames == null || playerNames.Count == 0)
            throw new ArgumentException("Player names cannot be null or empty", nameof(playerNames));

        WriteLine($"GameId: {Id}");

        var tiles = GenerateTiles();

        Shuffle(tiles, new Random(Id.GetHashCode()));

        DistributeTiles(tiles, playerNames, playerTypes, humanPlayerCallback);
    }

    public async Task PlayAsync(CancellationToken cancellationToken = default)
    {
        await ExecuteTurnAsync(cancellationToken);
        AdvanceToNextPlayer();
    }

    public async Task<bool> ExecuteTurnAsync(CancellationToken cancellationToken = default)
    {
        var player = Players[PlayerIndex];
        var playerSolution = await player.SolveAsync(Board, cancellationToken);

        if (playerSolution.IsValid)
        {
            Board = playerSolution;
            player.Play();
            if (player.Won) IsGameOver = true;
            return true; // Turn completed successfully
        }

        player.Drew(_tilePool.Dequeue());
        return false; // Drew a tile, waiting for next action
    }

    public void AdvanceToNextPlayer()
    {
        NextPlayer();
    }


    private static Tile[] GenerateTiles()
    {
        var tiles = new Tile[106];
        var index = 0;

        foreach (var color in Enum.GetValues<TileColor>())
            for (var i = 1; i <= 13; i++)
            {
                tiles[index++] = new Tile(i, color);
                tiles[index++] = new Tile(i, color);
            }

        tiles[index++] = new Tile(true);
        tiles[index] = new Tile(true);

        return tiles;
    }

    private void DistributeTiles(Tile[] tiles, List<string> playerNames, List<bool>? playerTypes = null,
        Func<Set, bool, CancellationToken, Task<SolverResult>>? humanPlayerCallback = null)
    {
        const int tilesPerPlayer = 14;
        var totalDistributed = playerNames.Count * tilesPerPlayer;

        Players.Clear();
        Players.Capacity = playerNames.Count;

        for (var i = 0; i < playerNames.Count; i++)
        {
            var startIndex = i * tilesPerPlayer;
            var playerTiles = new Tile[tilesPerPlayer];
            Array.Copy(tiles, startIndex, playerTiles, 0, tilesPerPlayer);

            // Determine strategy based on player types
            ISolverStrategy strategy;
            var isRealPlayer = playerTypes?[i] ?? false; // Default to AI if not specified

            if (isRealPlayer && humanPlayerCallback != null)
                strategy = new HumanPlayerStrategy(humanPlayerCallback);
            else
                // Configuration Aggressive: cherche beaucoup plus la meilleure solution
                // - PopulationSize: 200 (vs 100 Default)
                // - PopulationCount: 8 populations parallèles (vs 4 Default)
                // - MaxGenerations: 1000 (vs 500 Default)
                // - MutationRate: 0.2 (vs 0.1 Default)
                strategy = new PureGeneticStrategy(GeneticConfiguration.UltraAggressive, false);

            Players.Add(new Player(playerNames[i], playerTiles.ToList(), strategy));
        }

        _tilePool.Clear();
        for (var i = totalDistributed; i < tiles.Length; i++) _tilePool.Enqueue(tiles[i]);
    }

    private static void Shuffle(Tile[] tiles, Random rng)
    {
        for (var i = tiles.Length - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (tiles[i], tiles[j]) = (tiles[j], tiles[i]);
        }
    }

    private void NextPlayer()
    {
        PlayerIndex = (PlayerIndex + 1) % Players.Count;
        if (PlayerIndex == 0) Turn++;
    }

    public int AllTiles()
    {
        var sum = 0;

        sum += _tilePool.Count;

        Write(sum + " ");

        sum += Players.Sum(player => player.Rack.Tiles.Count);
        Write(sum + " ");

        sum += Board.GetSet().Tiles.Count;
        WriteLine(sum);

        return sum;
    }
}