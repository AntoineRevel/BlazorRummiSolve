using static System.Console;

namespace RummiSolve;

public class Player
{
    public readonly string Name;
    private Set _lastRackTilesSet=new();
    private readonly Set _rackTilesSet = new();
    public  Set RackTileToShow;
    public List<Tile> TilesToPlay = [];
    private bool _played;
    public bool Won { get; private set; }
    private Tile _lastDrewTile;

    public Player(string name, List<Tile> tiles)
    {
        Name = name;
        _rackTilesSet = new Set(tiles); //TODO pas de copie de la liste
        _lastRackTilesSet = new Set(tiles);
        RackTileToShow = _rackTilesSet;
    }

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

            if (finalRackSet != null) break;
        }

        if (finalRackSet == null) return finalSolution;

        TilesToPlay = finalRackSet.Tiles;

        Write("Play: ");
        finalRackSet.PrintAllTiles();
        WriteLine();

        return finalSolution;
    }

    public void RemoveTilePlayed()
    {
        _lastRackTilesSet = new Set(_rackTilesSet);
        foreach (var tile in TilesToPlay) _rackTilesSet.Remove(tile);
        TilesToPlay.Clear();
        
    }

    public void ShowRemovedTile()
    {
        RackTileToShow = _rackTilesSet;
    }
    
    public void BackRemovedTile()
    {
        RackTileToShow = _lastRackTilesSet;
    }

    private Solution SolveFirst(Solution boardSolution)
    {
        var firstRackSolution = new Set(_rackTilesSet.Tiles).GetSolution();
        if (firstRackSolution.IsValid)
        {
            Won = true;
            TilesToPlay = [.._rackTilesSet.Tiles];
            return boardSolution.AddSolution(firstRackSolution);
        }

        var finalSolution = Solution.GetInvalidSolution();

        for (var tileCount = _rackTilesSet.Tiles.Count - 1; tileCount > 3; tileCount--)
        {
            var rackSetsToTry = Set.GetBestSets(_rackTilesSet.Tiles, tileCount);

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

        TilesToPlay = finalSolution.GetSet().Tiles;
        finalSolution.AddSolution(boardSolution);


        Write("Playing for the first time: ");
        foreach (var tile in TilesToPlay) tile.PrintTile();
        WriteLine();

        _played = true;
        return finalSolution;
    }
}