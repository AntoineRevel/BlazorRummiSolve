using static System.Console;

namespace RummiSolve;

public class Game
{
    private List<Tile> _rackSet = [];
    private Solution _boardSolution = new();

    public void Start()
    {
        var isFirst = false;
        AddTileToRack();
        while (_rackSet.Count > 0)
        {
            AddTileToBoard();
            var solution = Solve(isFirst);
            PrintAllTiles();
            if (!solution.IsValid)
            {
                AddTileToRack();
            }
            else isFirst = false;
        }
    }

    private void AddTileToRack()
    {
        WriteLine("Complete player tiles:");
        foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
        {
            WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                _rackSet = Set.GetTilesFromInput(input, color);
            }
        }
    }

    private void AddTileToBoard()
    {
        var boardTiles = _boardSolution.GetAllTiles();
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
                _boardSolution = boardSolution;
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
        _rackSet.ForEach(t=>t.PrintTile());

        WriteLine("Board tiles:");
        _boardSolution.PrintSolution();
    }

    private Solution Solve(bool isFirst)
    {
        var boardTiles = _boardSolution.GetAllTiles();
        for (var tileCount = _rackSet.Count; tileCount > 0; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(_rackSet, tileCount);

            foreach (var rackSetToTry in rackSetsToTry)
            {
                if (isFirst && rackSetToTry.Sum(tile => tile.Number) < 30) break;
                var setToTry = Set.ConcatTiles(rackSetToTry, boardTiles);
                var solution = setToTry.GetSolution();
                if (!solution.IsValid) continue;
                Write("Play : ");
                foreach (var tile in rackSetToTry)
                {
                    _rackSet.Remove(tile);
                }
                _boardSolution = solution;
                return solution;
            }
        }

        return Solution.GetInvalidSolution();
    }
}