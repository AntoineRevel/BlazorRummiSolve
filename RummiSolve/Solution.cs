namespace RummiSolve;

public class Solution
{
    private readonly List<Run> _runs;
    private readonly List<Group> _groups;
    public bool IsValid;

    private static readonly Solution InvalidSolution = new() { IsValid = false };

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

    public static Solution GetInvalidSolution()
    {
        return InvalidSolution;
    }

    public void AddSolution(Solution solution)
    {
        _groups.AddRange(solution._groups);

        _runs.AddRange(solution._runs);
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
    }

    public bool IsValidSolution()
    {
        return _runs.All(run => run.IsValidRun()) && _groups.All(group => group.IsValidGroup());
    }

    public Tile[] GetAllTiles()
    {
        var totalSize = _runs.Sum(run => run.Tiles.Length) + _groups.Sum(group => group.Tiles.Length);

        var allTiles = new Tile[totalSize];

        var currentIndex = 0;

        foreach (var run in _runs)
        {
            Array.Copy(run.Tiles, 0, allTiles, currentIndex, run.Tiles.Length);
            currentIndex += run.Tiles.Length;
        }

        foreach (var group in _groups)
        {
            Array.Copy(group.Tiles, 0, allTiles, currentIndex, group.Tiles.Length);
            currentIndex += group.Tiles.Length;
        }

        return allTiles;
    }
}