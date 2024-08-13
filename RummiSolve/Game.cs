namespace RummiSolve;

public class Game
{
    private Set _rackSet = new();
    private Set _boardSet = new();

    public void Initialize()
    {
        Console.WriteLine("Player tiles:");
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            Console.WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                _rackSet.AddTilesFromInput(input, color);
            }
        }

        Console.WriteLine("Board tiles:");
        foreach (Color color in Enum.GetValues(typeof(Color)))
        {
            Console.WriteLine($"Enter tile numbers for {color} (separated by spaces):");
            var input = Console.ReadLine();
            if (!string.IsNullOrEmpty(input))
            {
                _boardSet.AddTilesFromInput(input, color);
            }
        }
    }

    public void PrintAllTiles()
    {
        Console.WriteLine("Player tiles:");
        _rackSet.PrintAllTiles();

        Console.WriteLine("Board tiles:");
        _boardSet.PrintAllTiles();
    }

    public Solution Solve()
    {
        for (var i = _rackSet.Count(); i != 0; i--)
        {
            var setsToTry = _rackSet.GetSets(i);
            setsToTry.ForEach(s => s.AddTiles(_boardSet));

            foreach (var set in setsToTry)
            {
                set.PrintAllTiles();
                var solution = set.GetSolution();
                if (!solution.IsValid) continue;
                solution.PrintSolution();
                return solution;
            }
            
            Console.WriteLine("No solution found.");

        }

        return Solution.GetInvalidSolution();
    }
}