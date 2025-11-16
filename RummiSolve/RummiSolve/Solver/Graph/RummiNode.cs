using RummiSolve.Solver.Abstract;

namespace RummiSolve.Solver.Graph;

public class RummiNode : BaseSolver
{
    private readonly int _boardJokers;
    private readonly int _boardTileNotPlayed;
    private readonly bool[] _isPlayerTile;
    private readonly bool _isRun;
    private readonly RummiNode? _parentNode;
    private readonly ValidSet _set;
    private readonly int _startIndex;
    private readonly int _totalJoker;

    public readonly List<RummiNode> Children = [];
    public readonly bool[] IsTileUsed;
    public readonly int PlayerTilePlayed;
    public readonly int Score;
    private string? _id;


    private RummiNode(ValidSet set, Tile[] tiles, bool[] isTileUsed, int jokers, int startIndex, int score,
        RummiNode? parentNode, bool isRun, bool[] isPlayerTile,
        int boardJokers, int totalJoker, int playerTilePlayed, int boardTileNotPlayed) :
        base(tiles, jokers)
    {
        IsTileUsed = (bool[])isTileUsed.Clone();
        Array.Copy(IsTileUsed, UsedTiles, Tiles.Length);
        _set = set;
        _startIndex = startIndex;
        Score = score;
        _parentNode = parentNode;
        _isRun = isRun;
        _isPlayerTile = isPlayerTile;
        _boardJokers = boardJokers;
        _totalJoker = totalJoker;
        PlayerTilePlayed = playerTilePlayed;
        _boardTileNotPlayed = boardTileNotPlayed;
    }

    public static RummiNode CreateRoot(Tile[] tiles, int jokers, bool[] isPlayerTile, int boardTile, int boardJokers)
    {
        return new RummiNode(new ValidSet([]), tiles, new bool[tiles.Length], jokers, 0, 0, null, false,
            isPlayerTile, boardJokers, jokers, 0, boardTile);
    }

    public bool GetChildren()
    {
        var totalCreatedNode = 0;

        for (var i = _startIndex; i < Tiles.Length; i++)
        {
            if (IsTileUsed[i]) continue;
            var createdNode = 0;
            UsedTiles[i] = true;
            var playerTilePlayed = PlayerTilePlayed;
            var boardTileNotPlayed = _boardTileNotPlayed;
            if (_isPlayerTile[i]) playerTilePlayed++;
            else boardTileNotPlayed--;
            foreach (var set in GetRuns(i))
                CreateChildNode(ref createdNode, i, set, true, playerTilePlayed, boardTileNotPlayed);
            foreach (var set in GetGroups(i))
                CreateChildNode(ref createdNode, i, set, false, playerTilePlayed, boardTileNotPlayed);

            if (createdNode == 0 && !_isPlayerTile[i]) return false;

            UsedTiles[i] = false;

            totalCreatedNode += createdNode;
        }

        var jokerPlayer = _totalJoker - Jokers - _boardJokers;
        var isLeaf = totalCreatedNode == 0 && _boardTileNotPlayed == 0 && (PlayerTilePlayed > 0 || jokerPlayer > 0);

        return isLeaf;
    }

    private void CreateChildNode(ref int createdNode, int index, ValidSet set, bool isRun, int playerTilePlayed,
        int boardTileNotPlayed)
    {
        createdNode++;
        MarkTilesAsUsed(set, index, ref playerTilePlayed, ref boardTileNotPlayed);
        var child = new RummiNode(set, Tiles, UsedTiles, Jokers, index + 1,
            Score + set.GetScore(), this, isRun, _isPlayerTile, _boardJokers, _totalJoker,
            playerTilePlayed, boardTileNotPlayed);
        MarkTilesAsUnused(set, index);
        Children.Add(child);
    }

    private void MarkTilesAsUsed(ValidSet set, int unusedIndex, ref int playerTilePlayed, ref int boardTileNotPlayed)
    {
        unusedIndex++;
        for (var tIndex = 1; tIndex < set.Tiles.Length; tIndex++)
        {
            var tile = set.Tiles[tIndex];
            if (tile.IsJoker)
            {
                Jokers -= 1;
                continue;
            }

            for (; unusedIndex < Tiles.Length; unusedIndex++)
            {
                if (UsedTiles[unusedIndex] || !Tiles[unusedIndex].Equals(tile))
                    continue; //todo regarder pour récupérer les index des tuiles du validet set directement dans getRun et getGroup

                if (_isPlayerTile[unusedIndex]) playerTilePlayed++;
                else boardTileNotPlayed--;

                UsedTiles[unusedIndex] = true;
                break;
            }
        }
    }

    private string GetId()
    {
        if (_id != null)
            return _id;

        var id = new char[Tiles.Length];

        for (var i = 0; i < _startIndex; i++) id[i] = '0';

        for (var i = _startIndex; i < Tiles.Length; i++) id[i] = UsedTiles[i] ? '1' : '0';

        // Inclure le nombre de jokers dans l'ID pour différencier les états
        _id = $"{new string(id)}_{Jokers}";
        return _id;
    }

    private void Print()
    {
        _set.Print();

        Console.Write("  Unused: [ ");
        for (var i = 0; i < Tiles.Length; i++)
            if (!IsTileUsed[i])
                Tiles[i].PrintTile();
        for (var i = 0; i < Jokers; i++) Tile.PrintJoker();
        Console.Write("]");

        Console.WriteLine(
            $" sc: {Score}, tilePlayed: {PlayerTilePlayed}, borad tile not played: {_boardTileNotPlayed}, id: {GetId()}");
    }

    public void PrintTree()
    {
        PrintTreeRecursive(0);
    }

    private void PrintTreeRecursive(int depth)
    {
        Console.Write(new string(' ', depth * 2));
        Print();
        foreach (var child in Children) child.PrintTreeRecursive(depth + 1);
    }

    public Solution GetSolution()
    {
        var solution = new Solution { IsValid = true };
        var currentNode = this;

        while (currentNode._parentNode is not null)
        {
            if (currentNode._isRun)
                solution.AddRun(currentNode._set);
            else
                solution.AddGroup(currentNode._set);

            currentNode = currentNode._parentNode;
        }

        return solution;
    }
}