namespace RummiSolve;

public class Solution
{
    public readonly List<Run> Runs = [];
    public readonly List<Group> Groups = [];
    public bool IsValid = true;

    private static readonly Solution InvalidSolution = new() { IsValid = false };

    public static Solution GetInvalidSolution()
    {
        return InvalidSolution;
    }

    public void AddSolution(Solution solution)
    {
        Groups.AddRange(solution.Groups);

        Runs.AddRange(solution.Runs);
    }

    public void AddRun(Run run)
    {
        Runs.Add(run);
    }


    public void AddGroup(Group group)
    {
        Groups.Add(group);
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

        foreach (var run in Runs)
        {
            if (hasPrintedRun) Console.Write("| ");

            run.PrintAllTiles();
            hasPrintedRun = true;
        }

        if (hasPrintedRun) Console.WriteLine();

        foreach (var group in Groups)
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
        return Runs.All(run => run.IsValidRun()) && Groups.All(group => group.IsValidGroup());
    }

    public Set GetSet()
    {
        var result = new Set();

        foreach (var run in Runs)
        {
            result.Concat(run);
        }

        foreach (var group in Groups)
        {
            result.Concat(group);
        }

        return result;
    }

    public int Count() => Groups.Sum(g => g.Tiles.Count) + Runs.Sum(s => s.Tiles.Count); //TODO temporaire
}