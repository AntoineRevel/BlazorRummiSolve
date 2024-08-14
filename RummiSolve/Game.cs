using static System.Console;

namespace RummiSolve;

public class Game
{
    private readonly Set _rackSet = new() { Tiles = [] };
    private Solution _boardSolution = new();

    public void Start()
    {
        AddTileToRack();
        
        while (_rackSet.Tiles.Count > 0)
        {
            AddTileToBoard();
            var solution = Solve();
            PrintAllTiles();
            if (!solution.IsValid)
            {
                AddTileToRack();
            }
        }
    }
    
    private void AddTileToRack()
    {
        WriteLine("Player tiles:");
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                _rackSet.AddTilesFromInput(input, color);
            }
        }
    }

    private void AddTileToBoard()
    {
        var boardSet = _boardSolution.GetSet();
        while (true)
        {
            WriteLine("Board tiles:");
            foreach (Color color in Enum.GetValues(typeof(Color)))
            {
                WriteLine($"Enter tile numbers for {color} (separated by spaces):");
                var input = ReadLine();
                if (string.IsNullOrEmpty(input)) continue;
                boardSet.AddTilesFromInput(input, color);
            }

            var boardSolution = boardSet.GetSolution();
            if (boardSolution.IsValid)
            {
                _boardSolution = boardSolution;
            }
            else
            {
                WriteLine("Invalid board tiles. Please try again.");
                boardSet = _boardSolution.GetSet();
                continue;
            }

            break;
        }
    }


    private void PrintAllTiles()
    {
        WriteLine("Player tiles:");
        _rackSet.PrintAllTiles();
        
        WriteLine("Board tiles:");
        _boardSolution.PrintSolution();
    }


    private Solution Solve()
    {
        for (var tileCount = _rackSet.Tiles.Count; tileCount > 0; tileCount--)
        {
            var setsToTry = _rackSet.GetBestSets(tileCount);

            foreach (var set in setsToTry)
            {
                var setToRemove = new List<Tile>(set.Tiles);
                set.PrintAllTiles(); //TODO test ordering
                //TODO for the first if set.score <30 continue
                set.AddTiles(_boardSolution.GetSet());
                var solution = set.GetSolution();

                if (!solution.IsValid) continue;

                solution.PrintSolution();
                _rackSet.Tiles.RemoveAll(tile => setToRemove.Contains(tile));
                _boardSolution = solution;
                return solution;
            }
        }

        return Solution.GetInvalidSolution();
    }
}