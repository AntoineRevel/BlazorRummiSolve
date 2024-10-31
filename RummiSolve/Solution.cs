namespace RummiSolve;

public class Solution
{
    private readonly List<Run> _runs;
    private readonly List<Group> _groups;
    public bool IsValid;
    private int _jokers;
    //todo int totalTiles

    private static readonly Solution InvalidSolution = new() { IsValid = false };

    public Solution()
    {
        _runs = [];
        _groups = [];
        IsValid = true;
        _jokers = 0;
    }

    private Solution(Solution existingSolution)
    {
        ArgumentNullException.ThrowIfNull(existingSolution);

        _runs = [..existingSolution._runs];
        _groups = [..existingSolution._groups];
        IsValid = existingSolution.IsValid;
        _jokers = existingSolution._jokers;
    }

    public static Solution GetInvalidSolution()
    {
        return InvalidSolution;
    }

    public void AddSolution(Solution solution)
    {
        _groups.AddRange(solution._groups);

        _runs.AddRange(solution._runs);
    }

    public void AddRun(Run run)
    {
        _runs.Add(run);
    }


    public void AddGroup(Group group)
    {
        _groups.Add(group);
    }

    public void PrintSolution()
    {
        if (!IsValid)
        {
            Console.WriteLine("Invalid solution");
            return;
        }
        
        var hasPrintedRun = false;
        var hasPrintedGroup = false;

        foreach (var run in _runs)
        {
            if (hasPrintedRun) Console.Write("| ");

            run.PrintAllTiles();
            hasPrintedRun = true;
        }

        if (hasPrintedRun) Console.WriteLine();

        foreach (var group in _groups)
        {
            if (hasPrintedGroup) Console.Write("| ");

            group.PrintAllTiles();
            hasPrintedGroup = true;
        }

        if (hasPrintedGroup) Console.WriteLine();
        else if (!hasPrintedRun) Console.WriteLine("No tiles on the board. "); 
    }

    public bool IsValidSolution()
    {
        return _runs.All(run => run.IsValidRun()) && _groups.All(group => group.IsValidGroup());
    }

    public Set GetSet()
    {
        var result = new Set();

        foreach (var run in _runs)
        {
            result.Concat(run);
        }

        foreach (var group in _groups)
        {
            result.Concat(group);
        }

        return result;
    }

    public int Count() => _groups.Sum(g => g.Tiles.Count) + _runs.Sum(s => s.Tiles.Count); //TODO temporaire
}