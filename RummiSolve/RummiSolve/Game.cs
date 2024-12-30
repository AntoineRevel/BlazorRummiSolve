using System.Diagnostics;
using static System.Console;

namespace RummiSolve;

public class Game(Guid id)
{
    public readonly List<Player> Players = [];

    private int _noPlay;

    private Queue<Tile> _tilePool = null!;

    public Guid Id = id;
    public Solution SolutionToShow = new();

    public int Turn;


    public Game() : this(Guid.NewGuid())
    {
    }

    private Solution BoardSolution { get; set; } = new();
    private Solution NextPlayerSolution { get; set; } = new();
    public int CurrentPlayerIndex { get; private set; }
    public bool IsGameOver { get; private set; }
    public Player? Winner { get; private set; }

    public void InitializeGame(List<string> playerNames)
    {
        WriteLine("GameId : " + Id);
        var guidBytes = Id.ToByteArray();
        var seed = BitConverter.ToInt32(guidBytes, 0);
        var tiles = new List<Tile>();
        foreach (var color in Enum.GetValues<TileColor>())
            for (var i = 1; i <= 13; i++)
            {
                tiles.Add(new Tile(i, color));
                tiles.Add(new Tile(i, color));
            }

        tiles.Add(new Tile(true));
        tiles.Add(new Tile(true));

        var rng = new Random(seed);

        var shuffledTiles = tiles.OrderBy(_ => rng.Next()).ToList(); // One iteration ? RST-108 TODO


        Players.AddRange(playerNames.Select((name, index) =>
        {
            var playerTiles = shuffledTiles.Skip(index * 14).Take(14).ToList();
            return new Player(name, playerTiles);
        }));


        _tilePool = new Queue<Tile>(shuffledTiles.Skip(playerNames.Count * 14));
    }

    public void Start()
    {
        while (!IsGameOver)
        {
            var gameStopwatch = Stopwatch.StartNew();
            var player = Players[CurrentPlayerIndex];
            PlayCurrentPlayerTurn(player);
            ShowSolution(player);
            NextTurn();
            gameStopwatch.Stop();
            WriteLine($"Turn : {gameStopwatch.Elapsed.TotalSeconds} seconds");
        }
    }

    public void NextTurn()
    {
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        if (CurrentPlayerIndex == 0) Turn++;
    }
    
    
    public void PlayCurrentPlayerTurn(Player currentPlayer)
    {
        if (IsGameOver) return;

        WriteLine(Turn + " => ___   " + currentPlayer.Name + "'s turn   ___");

        if (NextPlayerSolution.IsValid) BoardSolution = NextPlayerSolution;

        NextPlayerSolution = currentPlayer.Solve(BoardSolution);

        currentPlayer.SaveRack();

        if (NextPlayerSolution.IsValid)
        {
            currentPlayer.RemoveTilePlayed();
        }
        else
        {
            WriteLine(currentPlayer.Name + " can't play.");
            var drawTile = _tilePool.Dequeue();
            currentPlayer.SetLastDrewTile(drawTile);
            Write("Drew tile: ");
            drawTile.PrintTile();
            WriteLine();
            currentPlayer.AddTileToRack(drawTile);
        }
    }

    public void ShowSolution(Player currentPlayer)
    {
        if (NextPlayerSolution.IsValid) SolutionToShow = NextPlayerSolution;

        currentPlayer.ShowRemovedTile();

        Print(currentPlayer);

        if (!currentPlayer.Won) return;
        IsGameOver = true;
        Winner = currentPlayer;
    }

    public void BackSolution()
    {
        SolutionToShow = BoardSolution;
    }

    private void Print(Player player)
    {
        player.PrintRackTiles();
        WriteLine("");
        SolutionToShow.PrintSolution();
        WriteLine("");
    }
}