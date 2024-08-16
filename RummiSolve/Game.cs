using static System.Console;

namespace RummiSolve;

public class Game
{
    private readonly Set _rackSet = new() { Tiles = [] };
    private Solution _boardSolution = new();

    public void Start()
    {
        var isFirst = false;
        AddTileToRack();
        while (_rackSet.Tiles.Count > 0)
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
                _rackSet.AddTilesFromInput(input, color);
            }
        }
    }

    private void AddTileToBoard()
    {
        var boardSet = _boardSolution.GetSet();
        while (true)
        {
            WriteLine("Complete board tiles:");
            foreach (Tile.Color color in Enum.GetValues(typeof(Tile.Color)))
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
    
    private Solution Solve(bool isFirst)
    {
        for (var tileCount = _rackSet.Tiles.Count; tileCount > 0; tileCount--)
        {
            var setsToTry = _rackSet.GetBestSets(tileCount);

            foreach (var trialSet in setsToTry)
            {
                if (isFirst && trialSet.GetScore() < 30) break;
                var trialSetCopy = trialSet.Copy();
                trialSetCopy.AddTiles(_boardSolution.GetSet());
                var solution = trialSetCopy.GetSolution();
                
                if (!solution.IsValid) continue;
                Write("Play : ");
                trialSet.PrintAllTiles();
                _rackSet.RemoveAll(trialSet);
                _boardSolution = solution;
                return solution;
            }
        }

        return Solution.GetInvalidSolution();
    }
}