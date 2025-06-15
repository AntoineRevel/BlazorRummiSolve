using static System.Console;

namespace RummiSolve;

public class SimpleGame(Guid id)
{
    private readonly Queue<Tile> _tilePool = new();

    public SimpleGame() : this(Guid.NewGuid())
    {
    }

    public List<SimplePlayer> Players { get; } = new();

    public int Turn { get; private set; }
    public int PlayerIndex { get; private set; }
    public Solution BoardSolution { get; set; } = new();

    public void InitializeGame(List<string> playerNames)
    {
        if (playerNames == null || playerNames.Count == 0)
            throw new ArgumentException("Player names cannot be null or empty", nameof(playerNames));

        WriteLine($"GameId: {id}");

        var tiles = GenerateTiles();

        Shuffle(tiles, new Random(id.GetHashCode()));

        DistributeTiles(tiles, playerNames);
    }

    public void Play()
    {
        var player = Players[PlayerIndex];
        var playerSolution = player.Solve(BoardSolution);

        if (playerSolution.IsValid)
        {
            BoardSolution = playerSolution;
            player.Play();
        }
        else
        {
            player.Drew(_tilePool.Dequeue());
        }

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

    private void DistributeTiles(Tile[] tiles, List<string> playerNames)
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
            Players.Add(new SimplePlayer(playerNames[i], playerTiles.ToList()));
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
}