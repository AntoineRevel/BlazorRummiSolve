using System.Diagnostics;
using static System.Console;

namespace RummiSolve;

public class Game(Guid id)
{
    public readonly List<Player> Players = [];

    private int _noPlay;

    public int Turn;

    public Guid Id= id;
    private Solution BoardSolution { get; set; } = new();
    private Solution NextPlayerSolution { get; set; } = new();
    public Solution? SolutionToShow = new();

    private Queue<Tile>? _tilePool;

    public int CurrentPlayerIndex { get; private set; }
    public bool IsGameOver { get; private set; }
    public Player? Winner { get; private set; }


    public Game() : this(Guid.NewGuid())
    {
    }

    public void InitializeGame(List<string> playerNames)
    {
        var guidBytes = Id.ToByteArray();
        var seed = BitConverter.ToInt32(guidBytes, 0);
        var tiles = new List<Tile>();
        foreach (var color in Enum.GetValues<TileColor>())
        {
            for (var i = 1; i <= 13; i++)
            {
                tiles.Add(new Tile(i, color));
                tiles.Add(new Tile(i, color));
            }
        }

        tiles.Add(new Tile(true));
        tiles.Add(new Tile(true));

        var rng = new Random(seed);

        var shuffledTiles = tiles.OrderBy(_ => rng.Next()).ToList(); //TODO One iteration ?


        Players.AddRange(playerNames.Select((name, index) =>
        {
            var playerTiles = shuffledTiles.Skip(index * 14).Take(14).ToList();
            return new Player(name, playerTiles);
        }));
        
        
        _tilePool = new Queue<Tile>(shuffledTiles.Skip(playerNames.Count * 14));
    }

    private Tile DrawTile()
    {
        return _tilePool!.Dequeue(); //TODO no !
    }

    public void Start()
    {
        while (!IsGameOver)
        {
            var gameStopwatch = Stopwatch.StartNew();
            PlayCurrentPlayerTurn();
            ShowSolution();
            gameStopwatch.Stop();
            WriteLine($"Turn : {gameStopwatch.Elapsed.TotalSeconds} seconds");
        }
    }


    public void PlayCurrentPlayerTurn()
    {
        if (IsGameOver) return;

        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        if (CurrentPlayerIndex == 0) Turn++;

        var player = Players[CurrentPlayerIndex]; //TODO amélioré
        WriteLine(Turn + " => ___   " + player.Name + "'s turn   ___");


        if (NextPlayerSolution.IsValid) BoardSolution = NextPlayerSolution;

        NextPlayerSolution = _noPlay < Players.Count
            ? player.Solve(BoardSolution)
            : player.Solve(BoardSolution, false);
        if (NextPlayerSolution.IsValid)
        {
            player.RemoveTilePlayed();
            _noPlay = 0;
        }
        else
        {
            WriteLine(player.Name + " can't play.");
            var drawTile = DrawTile();
            player.SetLastDrewTile(drawTile);
            Write("Drew tile: ");
            drawTile.PrintTile();
            WriteLine();
            player.AddTileToRack(drawTile);
            _noPlay++;
        }
    }

    public void ShowSolution()
    {
        var player = Players[CurrentPlayerIndex];
        if (NextPlayerSolution.IsValid)
        {
            SolutionToShow = NextPlayerSolution;
        }

        player.ShowRemovedTile();

        Print(player);

        if (!player.Won) return;
        IsGameOver = true;
        Winner = player;
    }

    public void BackSolution()
    {
        SolutionToShow = BoardSolution;
    }

    private void Print(Player player)
    {
        player.PrintRackTiles();
        WriteLine("");
        BoardSolution.PrintSolution();
        WriteLine("");
    }
}