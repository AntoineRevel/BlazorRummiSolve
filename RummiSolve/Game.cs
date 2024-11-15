using static System.Console;

namespace RummiSolve;

public class Game
{
    public readonly List<Player> Players = [];
    public Solution BoardSolution { get; set; } = new();
    private List<Tile> TilePool { get; set; } = [];
    
    public int CurrentPlayerIndex { get; private set; }
    public bool IsGameOver { get; private set; }
    public Player? Winner { get; private set; }

    public int Turn;
    
    private int _noPlay;
    

    public void AddPlayer(string playerName)
    {
        Players.Add(new Player(playerName));
    }
    
    public void InitializeGame()
    {
        InitializeTilePool(2);
        foreach (var player in Players)
        {
            InitializeRackTilesForPlayer(player);
            player.PrintRackTiles();
        }
    }

    public void PlayCurrentPlayerTurn()
    {
        if (IsGameOver) return;

        var player = Players[CurrentPlayerIndex];
        WriteLine(Turn + " => ___   " + player.Name + "'s turn   ___");
                
        var playerSolution = _noPlay < Players.Count
            ? player.Solve(BoardSolution)
            : player.Solve(BoardSolution, false);
                
        if (playerSolution.IsValid)
        {
            BoardSolution = playerSolution;
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

        Print(player);

        if (player.HasWon())
        {
            IsGameOver = true;
            Winner = player;
            return;
        }

        // Passer au joueur suivant
        CurrentPlayerIndex = (CurrentPlayerIndex + 1) % Players.Count;
        if (CurrentPlayerIndex == 0) Turn++;
        
    }
    
    

    public void Start()
    {
        InitializeTilePool(1);
        foreach (var player in Players)
        {
            InitializeRackTilesForPlayer(player);
            player.PrintRackTiles();
        }

        WriteLine();

        var playerWin = false;
        var noPlay = 0;

        while (!playerWin)
        {
            foreach (var player in Players)
            {
                WriteLine(Turn + " => ___   " + player.Name + "'s turn   ___");
                
                var playerSolution = noPlay < Players.Count
                    ? player.Solve(BoardSolution)
                    : player.Solve(BoardSolution, false);
                
                if (playerSolution.IsValid)
                {
                    BoardSolution = playerSolution;
                    noPlay = 0;
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
                    noPlay++;
                }

                Print(player);

                if (!player.HasWon()) continue;
                playerWin = true;
                break;
            }

            Turn++;
        }
    }

    private void Print(Player player)
    {
        player.PrintRackTiles();
        WriteLine("");
        BoardSolution.PrintSolution();
        WriteLine("");
    }

    private void InitializeTilePool(int seed)
    {
        foreach (TileColor color in Enum.GetValues<TileColor>())
        {
            for (var i = 1; i <= 13; i++)
            {
                TilePool.Add(new Tile(i, color));
                TilePool.Add(new Tile(i, color));
            }
        }

        TilePool.Add(new Tile(true));
        TilePool.Add(new Tile(true));

        var rng = new Random(seed);

        TilePool = TilePool.OrderBy(_ => rng.Next()).ToList();
    }


    private void InitializeRackTilesForPlayer(Player player)
    {
        for (var i = 0; i < 14; i++)
        {
            player.AddTileToRack(DrawTile());
        }
    }

    private Tile DrawTile()
    {
        if (TilePool is []) throw new InvalidOperationException("No tiles left in the pool.");
        
        var drawnTile = TilePool[0];
        TilePool.RemoveAt(0);
        return drawnTile;
    }
}