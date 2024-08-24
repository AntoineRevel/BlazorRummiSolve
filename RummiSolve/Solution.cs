namespace RummiSolve;

public class Solution
{
    private readonly List<Run> _runs;
    private readonly List<Group> _groups;
    public bool IsValid;
    //TODO cheeck if needed private string? _key=null;

    private static readonly Solution InvalidSolution = new() { IsValid = false };

    public Solution()
    {
        _runs = [];
        _groups = [];
        IsValid = true;
    }
    
    public Solution(string key)
    {
        if (key == "InvalidSolution")
        {
            IsValid = false;
            _runs = [];
            _groups = [];
            return;
        }

        IsValid = true;
        
        var parts = key.Split(["||"], StringSplitOptions.None);
        if (parts.Length != 2)
        {
            throw new ArgumentException("Clé invalide pour une Solution.");
        }
        
        _runs = parts[0]
            .Split('|')
            .Where(runKey => !string.IsNullOrWhiteSpace(runKey))
            .Select(runKey => new Run(runKey))
            .ToList();
        
        _groups = parts[1]
            .Split('|')
            .Where(groupKey => !string.IsNullOrWhiteSpace(groupKey))
            .Select(groupKey => new Group(groupKey))
            .ToList();
    }
    
    public string GetKey()
    {
        if (!IsValid)
        {
            return "InvalidSolution";
        }
        
        var runKeys = _runs.Select(run => run.GetKey()).ToList();
        var groupKeys = _groups.Select(group => group.GetKey()).ToList();
        
        runKeys.Sort();
        groupKeys.Sort();
        
        var combinedRunKeys = string.Join("|", runKeys);
        var combinedGroupKeys = string.Join("|", groupKeys);
        
        return $"{combinedRunKeys}||{combinedGroupKeys}";
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

    public void AddRun(Run run)
    {
        ArgumentNullException.ThrowIfNull(run);

        _runs.Add(run);
    }
    

    public void AddGroup(Group group)
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