using System.Diagnostics;
using static System.Console;

namespace RummiSolve;

public class Game
{
    public Set RackTilesSet { get; set; } = new();
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

        TilePool.Add(new Tile(true));
        TilePool.Add(new Tile(true));

        var rng = new Random(seed);

        TilePool = TilePool.OrderBy(_ => rng.Next()).ToList();
    }

    private void InitializeRackTiles()
    {
        for (var i = 0; i < 14; i++)
        {
            RackTilesSet.AddTile(TilePool[i]);
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
        RackTilesSet.AddTile(newTile);
        playedTiles++;
        Write("Drew tile: ");
        newTile.PrintTile();
        WriteLine();
        return newTile;
    }

    public void PlaySoloGame()
    {
        InitializeTilePool(1);
        InitializeRackTiles();

        PrintAllTiles();
        var isFirstMove = true;
        var playedTiles = 0;
        var newTiles = new List<Tile>();

        var totalStopwatch = Stopwatch.StartNew();

        while (RackTilesSet.Tiles.Count > 0)
        {
            var turnStopwatch = Stopwatch.StartNew();

            Write(playedTiles + " => ");
            var rack1 = RackTilesSet.Tiles.Count;
            var board1 = BoardSolution.Count();
            var solution = Solve(isFirstMove, newTiles);
            var rack2 = RackTilesSet.Tiles.Count;
            var board2 = BoardSolution.Count();
            
            if (rack1 + board1 != rack2 + board2) Console.WriteLine("Big souci " + rack1 + board1 + " " + rack2 + board2);
            newTiles.Clear();
            if (solution.IsValid)
            {
                isFirstMove = false;
                if (RackTilesSet.Tiles.Count > 0 && TilePool.Count > 0)
                {
                    Write("Can play but not finish : ");
                }
            }
            else Write("Can't play : ");

            newTiles.Add(DrawAndAddTileToRack(ref playedTiles));

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
        var isFirst = true;
        var newTiles = new List<Tile>();
        while (RackTilesSet.Tiles.Count > 0 || isFirst)
        {
            AddTileToBoardConsole();
            var solution = Solve(isFirst, newTiles);
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
                RackTilesSet = new Set(input, color);
            }
        }
    }

    private void AddTileToBoardConsole()
    {
        var boardSet = BoardSolution.GetSet();
        var setToAdd = new Set();

        while (true)
        {
            WriteLine("Complete board tiles:");
            foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
            {
                WriteLine($"Enter tile numbers for {color} (separated by spaces):");
                var input = ReadLine();
                if (string.IsNullOrEmpty(input)) continue;
                var newSet = new Set(input, color);
                setToAdd.Concat(newSet);
            }
            
            var boardSolution = boardSet.ConcatNew(setToAdd).GetSolution();
            if (boardSolution.IsValid)
            {
                BoardSolution = boardSolution;
            }
            else
            {
                WriteLine("Invalid board tiles. Please try again.");
                setToAdd = new Set();
                continue;
            }

            break;
        }
    }

    private void PrintAllTiles()
    {
        WriteLine("Player tiles:");
        RackTilesSet.Tiles.ForEach(t => t.PrintTile());

        WriteLine();

        WriteLine("Board tiles:");
        BoardSolution.PrintSolution();
        WriteLine();
    }

    public Solution Solve(bool isFirst, List<Tile> lastTileAdded)
    {
        if (!isFirst && lastTileAdded.Count == 0) return Solution.GetInvalidSolution();

        var boardSet = BoardSolution.GetSet();

        var finalSolution = Solution.GetInvalidSolution();
        var locker = new object();
        Set finalRackSet = null!;

        for (var tileCount = RackTilesSet.Tiles.Count; tileCount > (boardSet.Tiles.Count == 0 ? 3 : 0); tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTilesSet.Tiles, tileCount);
            rackSetsToTry = !isFirst
                ? rackSetsToTry.Where(tab => tab.Tiles.Intersect(lastTileAdded).Any())
                : rackSetsToTry;

            Parallel.ForEach(rackSetsToTry, (currentRackSet, state) =>
            {
                if (finalRackSet != null) state.Stop();

                if (isFirst && currentRackSet.GetScore() < 30) state.Stop();

                //TODO try currentRackSet.GetSolution();
                
                var solution = boardSet.ConcatNew(currentRackSet).GetSolution();

                if (!solution.IsValid) return;

                lock (locker)
                {
                    if (finalRackSet == null)
                    {
                        finalRackSet = currentRackSet;
                        finalSolution = solution;
                        state.Stop();
                    }
                }
            });

            if (finalRackSet != null) break;
        }

        if (finalRackSet == null) return finalSolution;

        foreach (var tile in finalRackSet.Tiles)
        {
            RackTilesSet.Remove(tile);
        }

        BoardSolution = finalSolution;

        return finalSolution;
    }
}