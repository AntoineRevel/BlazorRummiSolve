namespace RummiSolve;

public class Set
{
    private readonly List<Run> _runs = [];
    private readonly List<Group> _groups = [];

    public void AddRun(Run run)
    {
        _runs.Add(run);
    }

    public void AddGroup(Group group)
    {
        _groups.Add(group);
    }

    public void PrintSet()
    {
        Console.WriteLine("Runs:");
        foreach (var run in _runs)
        {
            run.PrintAllTiles();
            Console.WriteLine();
        }

        Console.WriteLine("Groups:");
        foreach (var group in _groups)
        {
            group.PrintAllTiles();
            Console.WriteLine();
        }
    }

    public bool IsValidSet()
    {
        return _runs.All(run => run.IsValidRun()) && _groups.All(group => group.IsValidGroup());
    }
}