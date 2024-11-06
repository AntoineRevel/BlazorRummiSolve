namespace RummiSolve;

public class Set : ISet
{
    public List<Tile> Tiles = []; //TODO check if List is the best
    private bool _isSorted;
    public int Jokers { get; private set; }

    public void AddTile(Tile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        Tiles.Add(tile);
        if (tile.IsJoker) Jokers++;
        _isSorted = false;
    }

    public void Concat(ValidSet set)
    {
        ArgumentNullException.ThrowIfNull(set);

        Tiles.AddRange(set.Tiles);
        Jokers += set.Jokers;
        _isSorted = false;
    }

    public Set ConcatNew(Set set)
    {
        ArgumentNullException.ThrowIfNull(set);

        var newSet = new Set
        {
            Tiles = [..Tiles],
            _isSorted = _isSorted,
            Jokers = Jokers
        };

        newSet.Tiles.AddRange(set.Tiles);
        newSet.Jokers += set.Jokers;
        newSet._isSorted = false;

        return newSet;
    }


    public void Remove(Tile tile)
    {
        ArgumentNullException.ThrowIfNull(tile);

        Tiles.Remove(tile);

        if (tile.IsJoker) Jokers--;
    }

    
    public void PrintAllTiles()
    {
        foreach (var tile in Tiles)
        {
            tile.PrintTile();
        }
        
        for (var i = 0; i < Jokers; i++)
        {
            Tile.PrintJoker();
        }
    }

    public void Sort()
    {
        if (_isSorted) return;
        Tiles.Sort();
        _isSorted = true;
    }

    public int GetScore()
    {
        return Tiles.Sum(t => t.Value);
    }

    public Solution GetSolution()
    {
        Sort();

        if (Jokers > 0) Tiles.RemoveRange(Tiles.Count - Jokers, Jokers);
        var length = Tiles.Count;
        var usedTiles = new bool[length];

        if (length + Jokers <= 2) return length == 0 ? new Solution() : Solution.GetInvalidSolution();

        var solution = FindSolution(new Solution(), usedTiles, length, 0, Jokers);
        return solution;
    }

    private Solution FindSolution(Solution solution, bool[] usedTiles, int unusedTileCount, int firstUnusedTileIndex,
        int availableJokers)
    {
        firstUnusedTileIndex = GetNextUnusedTileIndex(usedTiles, firstUnusedTileIndex);

        var sets = GetRuns(firstUnusedTileIndex, usedTiles, availableJokers)
            .Concat<ValidSet>(GetGroups(firstUnusedTileIndex, usedTiles, availableJokers))
            .OrderBy(set => set.Jokers);

        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, usedTiles, ref unusedTileCount, ref availableJokers);

            var newSolution = unusedTileCount switch
            {
                0 when availableJokers == 0 => solution,
                0 => Solution.GetInvalidSolution(),
                _ => FindSolution(solution, usedTiles, unusedTileCount, firstUnusedTileIndex, availableJokers)
            };

            if (newSolution.IsValid)
            {
                switch (set)
                {
                    case Run run:
                        solution.AddRun(run);
                        break;
                    case Group group:
                        solution.AddGroup(group);
                        break;
                }

                return solution;
            }

            MarkTilesAsUsed(set, false, usedTiles, ref unusedTileCount, ref availableJokers);
        }

        return Solution.GetInvalidSolution();
    }

    private static int GetNextUnusedTileIndex(bool[] usedTiles, int startIndex)
    {
        return Array.FindIndex(usedTiles, startIndex, used => !used);
    }

    private List<Run> GetRuns(int tileIndex, bool[] usedTiles, int availableJokers)
    {
        var runs = new List<Run>();
        var firstTile = Tiles[tileIndex];
        var color = firstTile.Color;
        var currentRun = new List<Tile> { firstTile };
        var jokersUsed = 0;
        var i = tileIndex + 1;

        while (true)
        {
            for (; i < Tiles.Count; i++)
            {
                if (Tiles[i].Color != color)
                {
                    i = Tiles.Count;
                    break;
                }

                if (usedTiles[i] || Tiles[i].Value == currentRun[^1].Value) continue;

                if (Tiles[i].Value != currentRun[^1].Value + 1) break;

                currentRun.Add(Tiles[i]);

                if (currentRun.Count >= 3)
                    runs.Add(new Run
                    {
                        Tiles = [..currentRun],
                        Jokers = jokersUsed
                    });
            }

            if (availableJokers <= 0) return runs;

            if (currentRun[^1].Value != 13) currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));
            else if (currentRun[0].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));
            else jokersUsed--; //TODO insert 2 Run 123J 4...

            availableJokers -= 1;
            jokersUsed++;

            if (currentRun.Count >= 3)
            {
                runs.Add(new Run
                {
                    Tiles = [..currentRun],
                    Jokers = jokersUsed
                });
            }
        }
    }

    private Group[] GetGroups(int firstTileIndex, bool[] usedTiles, int availableJokers)
    {
        if (Tiles is []) return [];

        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Value;
        var color = firstTile.Color;

        var sameNumberTiles = Tiles
            .Where((tile, index) => !usedTiles[index] && tile.Value == number && tile.Color != color)
            .Distinct()
            .ToArray();

        var size = sameNumberTiles.Length;

        return availableJokers switch
        {
            0 => size switch
            {
                < 2 => [],
                2 => [new Group { Tiles = [firstTile, ..sameNumberTiles] }],
                3 =>
                [
                    new Group { Tiles = [firstTile, ..sameNumberTiles] },
                    new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[1]] },
                    new Group { Tiles = [firstTile, sameNumberTiles[1], sameNumberTiles[2]] },
                    new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[2]] }
                ],
                _ => []
            },
            > 0 => size switch
            {
                0 => [],
                1 =>
                [
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[0], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    }
                ],
                2 =>
                [
                    new Group { Tiles = [firstTile, ..sameNumberTiles] },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[0], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[1], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, ..sameNumberTiles, new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                ],
                3 =>
                [
                    new Group { Tiles = [firstTile, ..sameNumberTiles] },
                    new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[1]] },
                    new Group { Tiles = [firstTile, sameNumberTiles[1], sameNumberTiles[2]] },
                    new Group { Tiles = [firstTile, sameNumberTiles[0], sameNumberTiles[2]] },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[0], sameNumberTiles[1],
                            new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[1], sameNumberTiles[2],
                            new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[0], sameNumberTiles[2],
                            new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[0], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[1], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                    new Group
                    {
                        Tiles =
                        [
                            firstTile, sameNumberTiles[2], new Tile(firstTile.Value, TileColor.Black, true)
                        ],
                        Jokers = 1
                    },
                ],
                _ => []
            },
            _ => []
        };
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed, bool[] usedTiles, ref int unusedTile, ref int availableJokers)
    {
        foreach (var tile in set.Tiles)
        {
            if (tile.IsJoker)
            {
                availableJokers += isUsed ? -1 : 1;
                continue;
            }

            for (var i = 0; i < Tiles.Count; i++)
            {
                if (usedTiles[i] == isUsed || !Tiles[i].Equals(tile)) continue;

                usedTiles[i] = isUsed;
                unusedTile += isUsed ? -1 : 1;
                break;
            }
        }
    }

    public Set ShuffleTiles()
    {
        var random = new Random();
        var n = Tiles.Count;

        for (var i = n - 1; i > 0; i--)
        {
            var j = random.Next(0, i + 1);
            (Tiles[i], Tiles[j]) = (Tiles[j], Tiles[i]);
        }

        return this;
    }

    public static IEnumerable<Set> GetBestSets(List<Tile> tiles, int n)
    {
        var combinations = GetCombinations(tiles, n);
        return combinations
            .Select(combination =>
            {
                var set = new Set
                {
                    Tiles = combination,
                    Jokers = combination.Count(tile => tile.IsJoker)
                };
                return set;
            })
            .OrderByDescending(t => t.GetScore());
    }

    private static IEnumerable<List<Tile>> GetCombinations(List<Tile> list, int length)
    {
        if (length == 0) yield return [];
        else
        {
            HashSet<string> seenCombinations = [];
            for (var i = 0; i < list.Count; i++)
            {
                var element = list[i];
                var remainingList = list.Skip(i + 1).ToList();
                foreach (var combination in GetCombinations(remainingList, length - 1))
                {
                    combination.Insert(0, element);
                    combination.Sort();
                    var combinationKey = GetKey(combination);
                    if (seenCombinations.Add(combinationKey)) yield return combination;
                }
            }
        }
    }

    private static string GetKey(IEnumerable<Tile> sortedTiles)
    {
        return string.Join("-", sortedTiles);
    }

    
}