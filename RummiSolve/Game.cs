using System.Diagnostics;
using static System.Console;

namespace RummiSolve;

public class Game
{
    private readonly List<Player> _players = [];
    public Solution BoardSolution { get; set; } = new();
    private List<Tile> TilePool { get; set; } = [];

    public void AddPlayer(string playerName)
    {
        _players.Add(new Player(playerName));
    }

    public void Start()
    {
        InitializeTilePool(2);
        foreach (var player in _players)
        {
            InitializeRackTilesForPlayer(player);
            player.PrintRackTiles();
        }

        WriteLine();

        var playerWin = false;
        var turn = 0;
        var noPlay = 0;

        while (!playerWin)
        {
            foreach (var player in _players)
            {
                WriteLine(turn + " => ___   " + player.Name + "'s turn   ___");
                
                var playerSolution = noPlay < _players.Count
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

            turn++;
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
        foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
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

        TilePool.RemoveRange(0, 14);
    }

    private Tile DrawTile()
    {
        if (TilePool.Count == 0)
        {
            throw new InvalidOperationException("No tiles left in the pool.");
        }

        var drawnTile = TilePool[0];
        TilePool.RemoveAt(0);
        return drawnTile;
    }
}