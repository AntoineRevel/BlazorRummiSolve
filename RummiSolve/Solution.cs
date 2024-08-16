namespace RummiSolve;

public class Solution
{
    private readonly List<Run> _runs;
    private readonly List<Group> _groups;
    public bool IsValid;

    public Solution()
    {
        _runs = [];
        _groups = [];
        IsValid = true;
    }

    private Solution(Solution existingSolution)
    {
        ArgumentNullException.ThrowIfNull(existingSolution);

        _runs = [..existingSolution._runs];
        _groups = [..existingSolution._groups];
        IsValid = existingSolution.IsValid;
    }
    
    public Solution GetSolutionWithAddedRun(Run run)
    {
        ArgumentNullException.ThrowIfNull(run);

        var newSolution = new Solution(this);
        newSolution.AddRun(run);
        return newSolution;
    }

    private void AddRun(Run run)
    {
        ArgumentNullException.ThrowIfNull(run);

        _runs.Add(run);
    }
    
    public Solution GetSolutionWithAddedGroup(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        var newSolution = new Solution(this);
        newSolution.AddGroup(group);
        return newSolution;
    }

    private void AddGroup(Group group)
    {
        ArgumentNullException.ThrowIfNull(group);

        _groups.Add(group);
    }

    public void PrintSolution()
    {
        if (!IsValid)
        {
            Console.WriteLine("Invalid solution");
            return;
        }
        foreach (var run in _runs)
        {
            run.PrintAllTiles();
        }

        Console.WriteLine();
        foreach (var group in _groups)
        {
            group.PrintAllTiles();
        }
    }
    
    public Set GetSet()
    {
        var tiles = _runs.SelectMany(run => run.Tiles).Concat(_groups.SelectMany(group => group.Tiles)).ToList();
        return new Set { Tiles = tiles };
    }

    public bool IsValidSolution()
    {
        return _runs.All(run => run.IsValidRun()) && _groups.All(group => group.IsValidGroup());
    }

    public static Solution GetInvalidSolution()
    {
        return new Solution { IsValid = false };
    }
}