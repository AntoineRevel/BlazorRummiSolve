namespace RummiSolve;

public class Solution
{
    private static readonly Solution InvalidSolution = new() { IsValid = false };
    public readonly List<Group> Groups = [];
    public readonly List<Run> Runs = [];
    public bool IsValid = true;

    public static Solution GetInvalidSolution()
    {
        return InvalidSolution;
    }

    public Solution AddSolution(Solution solution)
    {
        Groups.AddRange(solution.Groups);

        Runs.AddRange(solution.Runs);
        return this;
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

            foreach (var runTile in run.Tiles) runTile.PrintTile();
            hasPrintedRun = true;
        }

        if (hasPrintedRun) Console.WriteLine();

        foreach (var group in Groups)
        {
            if (hasPrintedGroup) Console.Write("| ");

            foreach (var groupTile in group.Tiles) groupTile.PrintTile();

            hasPrintedGroup = true;
        }

        if (hasPrintedGroup) Console.WriteLine();
        else if (!hasPrintedRun) Console.WriteLine("No tiles on the board. ");
    }

    public Set GetSet()
    {
        var result = new Set();

        foreach (var run in Runs) result.Concat(run);

        foreach (var group in Groups) result.Concat(group);

        return result;
    }
}