using static System.Console;

namespace RummiSolve;

public class Player(string name)
{
    public readonly string Name = name;
    private readonly Set _rackTilesSet = new();
    private bool _isFirst = true;
    private Tile? _lastDrewTile = null;

    public void AddTileToRack(Tile tile)
    {
        _rackTilesSet.AddTile(tile);
    }

    public void SetLastDrewTile(Tile tile)
    {
        _lastDrewTile = tile;
    }

    public void PrintRackTiles()
    {
        if (_rackTilesSet.Tiles.Count != 0)
        {
            WriteLine(Name + " tiles : ");
            _rackTilesSet.Tiles.ForEach(t => t.PrintTile());
        }
        else WriteLine(Name + " Win !!!");

        WriteLine();
    }

    public Solution Solve(Solution boardSolution, bool boardChange = true)
    {
        var boardSet = boardSolution.GetSet();
        var finalSolution = Solution.GetInvalidSolution();
        var locker = new object();
        Set finalRackSet = null!;

        for (var tileCount = _rackTilesSet.Tiles.Count; tileCount > (boardSet.Tiles.Count == 0 ? 3 : 0); tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(_rackTilesSet.Tiles, tileCount);

            rackSetsToTry = _isFirst || boardChange
                ? rackSetsToTry
                : rackSetsToTry.Where(tab => tab.Tiles.Contains(_lastDrewTile!));

            Parallel.ForEach(rackSetsToTry, (currentRackSet, state) =>
            {
                if (finalRackSet != null) state.Stop();

                if (_isFirst)
                {
                    if (currentRackSet.GetScore() < 30) return;
                    var firstSol = currentRackSet.GetSolution();
                    if (!firstSol.IsValid) return;
                }

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

        

        Write(_isFirst ? "Playing for the first time: " : "Play: ");
        finalRackSet.PrintAllTiles();
        WriteLine();
        foreach (var tile in finalRackSet.Tiles)
        {
            _rackTilesSet.Remove(tile);
        }
        
        _isFirst = false;
        return finalSolution;
    }

    public bool HasWon()
    {
        return _rackTilesSet.Tiles.Count == 0;
    }
}