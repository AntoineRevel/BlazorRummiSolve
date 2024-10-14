using System.Diagnostics;
using static System.Console;

namespace RummiSolve;

public class Game
{
    public List<Tile> RackTiles { get; set; } = [];
    public Solution BoardSolution { get; set; } = new();
    private List<Tile> TilePool { get; set; } = [];

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

        //TODO Ajouter les jokers

        var rng = new Random(seed);

        TilePool = TilePool.OrderBy(_ => rng.Next()).ToList();
    }

    private void InitializeRackTiles()
    {
        for (var i = 0; i < 14; i++)
        {
            RackTiles.Add(TilePool[i]);
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

    private Tile DrawAndAddTileToRack(ref int playedTiles)
    {
        var newTile = DrawTile();
        var rackContainNewTile = RackTiles.Contains(newTile);
        RackTiles.Add(newTile);
        playedTiles++;
        Write("Drew tile: ");
        newTile.PrintTile();
        WriteLine();
        if (rackContainNewTile) newTile = null;
        return newTile;
    }

    public void PlaySoloGame()
    {
        InitializeTilePool(1);
        InitializeRackTiles();

        PrintAllTiles();
        var isFirstMove = true;
        var playedTiles = 0;
        Tile? newTile = null;

        var totalStopwatch = Stopwatch.StartNew();

        while (RackTiles.Count > 0)
        {
            var turnStopwatch = Stopwatch.StartNew();

            Write(playedTiles + " => ");
            var solution = Solve(isFirstMove, newTile);

            if (solution.IsValid)
            {
                isFirstMove = false;
                if (RackTiles.Count > 0 && TilePool.Count > 0)
                {
                    Write("Can play but not finish : ");
                }
            }
            else Write("Can't play : ");

            newTile = DrawAndAddTileToRack(ref playedTiles);

            turnStopwatch.Stop();
            WriteLine($"Time for this turn: {turnStopwatch.ElapsedMilliseconds} ms");
            WriteLine($"Total time since start: {totalStopwatch.ElapsedMilliseconds} ms");
            PrintAllTiles();
            
        }

        WriteLine("Congratulations, you have played all your tiles!");
        totalStopwatch.Stop();
    }

    public void StartConsole()
    {
        var isFirst = false;
        AddTileToRackConsole();
        while (RackTiles.Count > 0)
        {
            AddTileToBoardConsole();
            var solution = Solve(isFirst);
            PrintAllTiles();
            if (!solution.IsValid)
            {
                AddTileToRackConsole();
            }
            else isFirst = false;
        }
    }

    private void AddTileToRackConsole()
    {
        WriteLine("Complete player tiles:");
        foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
        {
            WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                RackTiles = Set.GetTilesFromInput(input, color);
            }
        }
    }

    private void AddTileToBoardConsole()
    {
        var boardTiles = BoardSolution.GetAllTiles();
        var tilesToAdd = new List<Tile>();

        while (true)
        {
            WriteLine("Complete board tiles:");
            foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
            {
                WriteLine($"Enter tile numbers for {color} (separated by spaces):");
                var input = ReadLine();
                if (string.IsNullOrEmpty(input)) continue;
                tilesToAdd.AddRange(Set.GetTilesFromInput(input, color));
            }

            var allTiles = Set.ConcatTiles(boardTiles, tilesToAdd.ToArray());
            var boardSolution = allTiles.GetSolution();
            if (boardSolution.IsValid)
            {
                BoardSolution = boardSolution;
            }
            else
            {
                WriteLine("Invalid board tiles. Please try again.");
                tilesToAdd.Clear();
                continue;
            }

            break;
        }
    }

    private void PrintAllTiles()
    {
        WriteLine("Player tiles:");
        RackTiles.ForEach(t => t.PrintTile());

        WriteLine();

        WriteLine("Board tiles:");
        BoardSolution.PrintSolution();
        WriteLine();
    }

    public Solution Solve(bool isFirst, Tile? lastTileDrawn = null, List<Tile>? lastBoardTileAdded = null)
    {
        if (!isFirst && lastTileDrawn == null && lastBoardTileAdded == null) return Solution.GetInvalidSolution();
        var boardTiles = BoardSolution.GetAllTiles();
        for (var tileCount = RackTiles.Count; tileCount > (boardTiles.Length == 0 ? 3 : 0); tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTiles, tileCount);
            var rackSetsToTryWithNewTile =
                !isFirst ? rackSetsToTry.Where(tab => tab.Tiles.Contains(lastTileDrawn)) : rackSetsToTry;
            
            foreach (var rackSetToTry in rackSetsToTryWithNewTile)
            {
                var solution = Solution.GetInvalidSolution();
                if (isFirst && rackSetToTry.GetScore() < 30) return solution;
                
                var rackSolutionIsValid = false;
                if (rackSetToTry.Tiles.Length % 3 == 0)
                {
                    var rackSolution = rackSetToTry.GetSolution();
                    rackSolutionIsValid = rackSolution.IsValid;
                    if (rackSolutionIsValid)
                    {
                        solution = BoardSolution;
                        solution.AddSolution(rackSolution);
                    }
                }

                if (!rackSolutionIsValid)
                {
                    var setToTry = Set.ConcatTiles(rackSetToTry.Tiles, boardTiles);
                    solution = setToTry.GetSolution();
                }
                
                if (!solution.IsValid) continue;
                
                foreach (var tile in rackSetToTry.Tiles)
                {
                    RackTiles.Remove(tile);
                }

                BoardSolution = solution;
                return solution;
            }
        }
        
        return Solution.GetInvalidSolution();
    }
}