using static System.Console;

namespace RummiSolve;

public class Player(string name)
{
    public readonly string Name = name;
    public readonly Set RackTilesSet = new();
    private bool _isFirst = true;
    private Tile _lastDrewTile;

    public void AddTileToRack(Tile tile)
    {
        RackTilesSet.AddTile(tile);
    }

    public void SetLastDrewTile(Tile tile)
    {
        _lastDrewTile = tile;
    }

    public void PrintRackTiles()
    {
        if (RackTilesSet.Tiles.Count != 0)
        {
            WriteLine(Name + " tiles : ");
            RackTilesSet.Tiles.ForEach(t => t.PrintTile());
        }
        else WriteLine(Name + " Win !!!");

        WriteLine();
    }

    public Solution Solve(Solution boardSolution, bool boardChange = true)
    {
        if (_isFirst) return SolveFirst(boardSolution);

        var boardSet = boardSolution.GetSet();
        var finalSolution = Solution.GetInvalidSolution();
        var locker = new Lock();
        Set finalRackSet = null!;

        for (var tileCount = RackTilesSet.Tiles.Count; tileCount > 0; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTilesSet.Tiles, tileCount);

            rackSetsToTry = boardChange
                ? rackSetsToTry
                : rackSetsToTry.Where(tab => tab.Tiles.Contains(_lastDrewTile));

            Parallel.ForEach(rackSetsToTry, (currentRackSet, state) =>
            {
                if (finalRackSet != null) state.Stop();

                var solution = boardSet.ConcatNew(currentRackSet).GetSolution();

                if (!solution.IsValid) return;

                lock (locker)
                {
                    if (finalRackSet != null) return;
                    finalRackSet = currentRackSet;
                    finalSolution = solution;
                    state.Stop();
                }
            });

            if (finalRackSet != null) break;
        }

        if (finalRackSet == null) return finalSolution;


        Write("Play: ");
        finalRackSet.PrintAllTiles();
        WriteLine();

        foreach (var tile in finalRackSet.Tiles)
        {
            RackTilesSet.Remove(tile);
        }

        return finalSolution;
    }

    private Solution SolveFirst(Solution boardSolution)
    {
        var finalSolution = Solution.GetInvalidSolution();

        for (var tileCount = RackTilesSet.Tiles.Count; tileCount > 3; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(RackTilesSet.Tiles, tileCount);

            rackSetsToTry = _lastDrewTile.IsNull
                ? rackSetsToTry
                : rackSetsToTry.Where(tab => tab.Tiles.Contains(_lastDrewTile));

            Parallel.ForEach(rackSetsToTry, (currentRackSet, state) =>
            {
                if (currentRackSet.GetScore() < 30) state.Break();
                var firstSol = currentRackSet.GetSolution();
                if (!firstSol.IsValid) return;
                state.Break();
                finalSolution = firstSol;
            });

            if (finalSolution.IsValid) break;
        }

        if (!finalSolution.IsValid) return finalSolution;

        boardSolution.AddSolution(finalSolution);

        var finalRackSet = finalSolution.GetSet();

        Write("Playing for the first time: ");

        finalRackSet.PrintAllTiles();
        WriteLine();

        foreach (var tile in finalRackSet.Tiles)
        {
            RackTilesSet.Remove(tile);
        }

        _isFirst = false;
        return boardSolution;
    }

    public bool HasWon()
    {
        return RackTilesSet.Tiles.Count == 0;
    }
}