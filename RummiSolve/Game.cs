using static System.Console;

namespace RummiSolve;

public class Game
{
    public List<Tile> RackTiles { get; set; } = [];
    public Solution BoardSolution { get; set; } = new();
    

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

    public void PrintAllTiles()
    {
        WriteLine("Player tiles:");
        RackTiles.ForEach(t => t.PrintTile());

        WriteLine();
        
        WriteLine("Board tiles:");
        BoardSolution.PrintSolution();
    }

    public Solution Solve(bool isFirst)
    {
        var boardTiles = BoardSolution.GetAllTiles();
        for (var tileCount = RackTiles.Count; tileCount > 0; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTiles, tileCount);

            foreach (var rackSetToTry in rackSetsToTry)
            {
                foreach (var tile in rackSetToTry)
                {
                    tile.PrintTile();
                }
                WriteLine(rackSetToTry.Sum(tile => tile.Number));
                if (isFirst && rackSetToTry.Sum(tile => tile.Number) < 30) break;
                var setToTry = Set.ConcatTiles(rackSetToTry, boardTiles);
                var solution = setToTry.GetSolution();
                if (!solution.IsValid) continue;
                Write("Play : ");
                foreach (var tile in rackSetToTry)
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