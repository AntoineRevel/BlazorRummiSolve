using static System.Console;

namespace RummiSolve;

public class Game
{
    public List<Tile> RackTiles { get; set; } = [];
    public Solution BoardSolution { get; set; } = new();
    private List<Tile> TilePool { get; set; } = [];

    private void InitializeTilePool()
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

        TilePool = TilePool.OrderBy(t => Guid.NewGuid()).ToList();
    }

    private void InitializeRackTiles()
    {
        for (var i = 0; i < 14; i++)
        {
            RackTiles.Add(DrawTile());
        }
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
        var rackContainNewTile = RackTiles.Contains(newTile); //and board don't change
        RackTiles.Add(newTile);
        playedTiles++;
        Write("Drew tile: ");
        newTile.PrintTile();
        WriteLine();
        if (rackContainNewTile) newTile = DrawAndAddTileToRack(ref playedTiles);
        return newTile;
    }

    public void PlaySoloGame()
    {
        InitializeTilePool();
        InitializeRackTiles();

        PrintAllTiles();
        var isFirstMove = true;
        var playedTiles = 0;
        Tile? newTile = null;

        while (RackTiles.Count > 0)
        {
            Write(playedTiles + " => ");
            var solution = Solve(isFirstMove, false, newTile);

            if (solution.IsValid)
            {
                isFirstMove = false;
                if (RackTiles.Count > 0 && TilePool.Count > 0)
                {
                    Write("Can play but not finish : ");
                    newTile = DrawAndAddTileToRack(ref playedTiles);
                }
            }
            else
            {
                Write("Can't play : ");
                newTile = DrawAndAddTileToRack(ref playedTiles);
            }

            PrintAllTiles();
        }

        WriteLine("Congratulations, you have played all your tiles!");
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
        Console.WriteLine();
    }

    public Solution Solve(bool isFirst, bool isBoardChanged = false, Tile? newTile = null)
    {
        var boardTiles = BoardSolution.GetAllTiles();
        var lockObj = new object(); // Un objet pour synchroniser l'accès à la solution trouvée
        var finalSolution = Solution.GetInvalidSolution(); // La solution finale


        for (var tileCount = RackTiles.Count; tileCount > (boardTiles.Length == 0 ? 3 : 0); tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTiles, tileCount);
            var rackSetsToTryWhithNewTile = rackSetsToTry.Where(tab => tab.Contains(newTile));

            Parallel.ForEach(rackSetsToTryWhithNewTile, (rackSetToTry, state) =>
            {
                if (isFirst && rackSetToTry.Sum(tile => tile.Number) < 30) return;

                var setToTry = Set.ConcatTiles(rackSetToTry, boardTiles);
                var solution = setToTry.GetSolution();

                if (!solution.IsValid) return;

                lock (lockObj)
                {
                    if (finalSolution.IsValid)
                    {
                        return;
                    }

                    foreach (var tile in rackSetToTry)
                    {
                        RackTiles.Remove(tile);
                    }

                    BoardSolution = solution;
                    finalSolution = solution;
                    state.Stop(); // Arrête le parallélisme si une solution valide est trouvée
                }
            });

            if (finalSolution.IsValid)
            {
                return finalSolution;
            }
        }

        return Solution.GetInvalidSolution();
    }
}