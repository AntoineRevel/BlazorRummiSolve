using static System.Console;

namespace RummiSolve;

public class Player
{
    private readonly Set _rackTilesSet;
    public readonly string Name;
    private Tile _lastDrewTile;
    private Set _lastRackTilesSet;
    public bool _played;
    public bool PlayedToShow;
    public Set RackTileToShow;
    public List<Tile> TilesToPlay = [];

    public Player(string name, List<Tile> tiles)
    {
        Name = name;
        _rackTilesSet = new Set(tiles); //pas de copie de la liste TODO
        _lastRackTilesSet = new Set(tiles);
        RackTileToShow = _lastRackTilesSet;
        PlayedToShow = false;
    }

    public bool Won { get; private set; }

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
        if (!Won)
        {
            WriteLine(Name + " tiles : ");
            _rackTilesSet.Tiles.ForEach(t => t.PrintTile());
        }
        else
        {
            WriteLine(Name + " Win !!!");
        }

        WriteLine();
    }

    public Solution Solve(Solution boardSolution, bool boardChange = true)
    {
        TilesToPlay.Clear();
        if (!_played) return SolveFirst(boardSolution);
        var boardSet = boardSolution.GetSet();

        var firstRackSolution = boardSet.ConcatNew(new Set(_rackTilesSet.Tiles)).GetSolution();
        if (firstRackSolution.IsValid)
        {
            Won = true;
            TilesToPlay = [.._rackTilesSet.Tiles];
            return firstRackSolution;
        }

        var finalSolution = Solution.GetInvalidSolution();
        var locker = new Lock();
        Set finalRackSet = null!;

        for (var tileCount = _rackTilesSet.Tiles.Count - 1; tileCount > 0; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(_rackTilesSet.Tiles, tileCount);

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
            
            // foreach (var currentRackSet in rackSetsToTry)
            // {
            //     var solution = boardSet.ConcatNew(currentRackSet).GetSolution();
            //     if (!solution.IsValid) continue;
            //     finalRackSet = currentRackSet;
            //     finalSolution = solution;
            //     break;
            // }

            if (finalRackSet != null) break;
        }

        if (finalRackSet == null) return finalSolution;

        TilesToPlay = finalRackSet.Tiles;

        Write("Play: ");
        finalRackSet.PrintAllTiles();
        WriteLine();

        return finalSolution;
    }

    private Solution SolveFirst(Solution boardSolution)
    {
        var finalSolution = new Set(_rackTilesSet.Tiles).GetFirstSolution(); //TONO nocopy Tiles et joker readonly
        if (!finalSolution.IsValid) return finalSolution;

        TilesToPlay = finalSolution.GetSet().Tiles;
        finalSolution.AddSolution(boardSolution);

        Write("Playing for the first time: ");
        foreach (var tile in TilesToPlay) tile.PrintTile();
        WriteLine();

        _played = true;
        return finalSolution;
    }

    public void SaveRack()
    {
        _lastRackTilesSet = new Set(_rackTilesSet);
        RackTileToShow = _lastRackTilesSet;
    }

    public void RemoveTilePlayed()
    {
        foreach (var tile in TilesToPlay) _rackTilesSet.Remove(tile);
    }

    public void ShowRemovedTile()
    {
        RackTileToShow = _rackTilesSet;
        PlayedToShow = _played;
    }

    public void ShowLastTile()
    {
        RackTileToShow = _lastRackTilesSet;
    }
}