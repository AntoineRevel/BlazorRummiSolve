using static System.Console;

namespace RummiSolve;

public class Player
{
    public string Name;
    private Set _rackTilesSet;
    private bool isFirst;

    public Player(string name)
    {
        Name = name;
        _rackTilesSet = new Set();
        isFirst = true;
    }

    public void AddTileToRack(Tile tile)
    {
        _rackTilesSet.AddTile(tile);
    }

    public void PrintRackTiles()
    {
        if (_rackTilesSet.Tiles.Count != 0)
        {
            WriteLine(Name + " tiles : ");
            _rackTilesSet.Tiles.ForEach(t => t.PrintTile());
        }
        else WriteLine(Name + "Win !!!");

        WriteLine();
    }


    public Solution Solve(Solution boardSolution)
    {
        var boardSet = boardSolution.GetSet();
        var finalSolution = Solution.GetInvalidSolution();
        var locker = new object();
        Set finalRackSet = null!;

        for (var tileCount = _rackTilesSet.Tiles.Count; tileCount > (boardSet.Tiles.Count == 0 ? 3 : 0); tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(_rackTilesSet.Tiles, tileCount);

            Parallel.ForEach(rackSetsToTry, (currentRackSet, state) =>
            {
                if (finalRackSet != null) state.Stop();

                if (isFirst && currentRackSet.GetScore() < 30) state.Stop();

                //TODO try currentRackSet.GetSolution();

                var solution = boardSet.ConcatNew(currentRackSet).GetSolution();

                if (!solution.IsValid) return;

                lock (locker)
                {
                    if (finalRackSet == null)
                    {
                        finalRackSet = currentRackSet;
                        finalSolution = solution;
                        state.Stop();
                    }
                }
            });

            if (finalRackSet != null) break;
        }

        if (finalRackSet == null) return finalSolution;

        isFirst = false;

        Write("Play : ");
        finalRackSet.PrintAllTiles();
        WriteLine();
        foreach (var tile in finalRackSet.Tiles)
        {
            _rackTilesSet.Remove(tile);
        }

        return finalSolution;
    }

    public bool HasWon()
    {
        return _rackTilesSet.Tiles.Count == 0;
    }
}