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
        foreach (var tile in Tiles) tile.PrintTile();
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

        var solRun = TrySet(GetRuns(firstUnusedTileIndex, usedTiles, availableJokers),
            solution, usedTiles, unusedTileCount, firstUnusedTileIndex, availableJokers);

        if (solRun.IsValid) return solRun;

        var solGroup = TrySet(GetGroups(firstUnusedTileIndex, usedTiles, availableJokers),
            solution, usedTiles, unusedTileCount, firstUnusedTileIndex, availableJokers);

        return solGroup.IsValid ? solGroup : Solution.GetInvalidSolution();
    }

    private Solution TrySet<TS>(IEnumerable<TS> sets, Solution solution, bool[] usedTiles, int unusedTileCount,
        int firstUnusedTileIndex, int availableJokers)
        where TS : ValidSet
    {
        foreach (var set in sets)
        {
            MarkTilesAsUsed(set, true, usedTiles, ref unusedTileCount, ref availableJokers);

            var newSolution = unusedTileCount switch
            {
                0 when availableJokers == 0 => solution,
                < 2 => Solution.GetInvalidSolution(),
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

    public IEnumerable<Run> GetRuns(int tileIndex, bool[] usedTiles, int availableJokers)
    {
        var color = Tiles[tileIndex].Color;
        var currentRun = new List<Tile> { Tiles[tileIndex] };
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
                    yield return new Run
                    {
                        Tiles = [..currentRun],
                        Jokers = jokersUsed
                    };
            }

            if (availableJokers <= 0) yield break;

            if (currentRun[^1].Value != 13) currentRun.Add(new Tile(currentRun[^1].Value + 1, color, true));
            else if (currentRun[0].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));
            else jokersUsed--; //TODO insert 2 Run 123J 4...

            availableJokers -= 1;
            jokersUsed++;

            if (currentRun.Count >= 3)
            {
                yield return new Run
                {
                    Tiles = [..currentRun],
                    Jokers = jokersUsed
                };
            }
        }
    }

    public List<Run> GetRunsSpan(int tileIndex, bool[] usedTiles, int availableJokers)
    {
        var runs = new List<Run>();
        var color = Tiles[tileIndex].Color;
        Span<Tile> currentRunSpan = stackalloc Tile[usedTiles.Length - tileIndex];
        currentRunSpan[0] = Tiles[tileIndex];
        var spanIndex = 0;
        var jokersUsed = 0;
        tileIndex += 1;

        while (true)
        {
            for (; tileIndex < Tiles.Count; tileIndex++)
            {
                if (Tiles[tileIndex].Color != color)
                {
                    break;
                }

                if (usedTiles[tileIndex] || Tiles[tileIndex].Value == currentRunSpan[spanIndex].Value) continue;

                if (Tiles[tileIndex].Value != currentRunSpan[spanIndex].Value + 1) break;

                spanIndex++;
                currentRunSpan[spanIndex] = Tiles[tileIndex];

                if (spanIndex >= 2)
                    runs.Add(new Run
                    {
                        Tiles = currentRunSpan[..(spanIndex + 1)].ToArray(),
                        Jokers = jokersUsed
                    });
            }

            if (availableJokers <= 0) return runs;

            if (currentRunSpan[spanIndex].Value != 13)
            {
                spanIndex++;
                currentRunSpan[spanIndex] = new Tile(currentRunSpan[spanIndex - 1].Value + 1, color, true);
            }
            //else if (currentRunSpan[spanIndex].Value != 1) currentRun.Insert(0, new Tile(currentRun[0].Value - 1, color, true));
            else jokersUsed--; //TODO insert 2 Run 123J 4...

            availableJokers -= 1;
            jokersUsed++;

            if (spanIndex >= 2)
            {
                runs.Add(new Run
                {
                    Tiles = currentRunSpan[..(spanIndex + 1)].ToArray(),
                    Jokers = jokersUsed
                });
            }
        }
    }

    public IEnumerable<Group> GetGroups(int firstTileIndex, bool[] usedTiles, int availableJokers)
    {
        var firstTile = Tiles[firstTileIndex];
        var number = firstTile.Value;
        var color = firstTile.Color;

        var sameNumberTiles = Tiles
            .Select((tile, index) => new { Tile = tile, Index = index })
            .Where(t => !usedTiles[t.Index] && t.Tile.Value == number && t.Tile.Color != color)
            .Select(t => t.Tile)
            .Distinct()
            .ToList();

        var size = sameNumberTiles.Count;

        if (availableJokers == 0)
        {
            if (size < 2) yield break;

            var groupTiles = new List<Tile> { firstTile };
            groupTiles.AddRange(sameNumberTiles);

            yield return new Group { Tiles = groupTiles.ToArray() };

            if (groupTiles.Count != 4) yield break;

            for (var i = 0; i < sameNumberTiles.Count; i++)
            {
                for (var j = i + 1; j < sameNumberTiles.Count; j++)
                {
                    yield return new Group
                    {
                        Tiles = [firstTile, sameNumberTiles[i], sameNumberTiles[j]]
                    };
                }
            }
        }
        else
        {
            for (var jokersUsed = 0; jokersUsed <= availableJokers; jokersUsed++)
            {
                for (var tilesUsed = 2; tilesUsed <= size + 1; tilesUsed++)
                {
                    if (tilesUsed + jokersUsed < 3 || tilesUsed + jokersUsed > 4)
                        continue;

                    var combinations = GetCombinationsGroup(sameNumberTiles, tilesUsed - 1);

                    foreach (var combo in combinations)
                    {
                        var totalTiles = tilesUsed + jokersUsed;
                        var groupTiles = new Tile[totalTiles];

                        groupTiles[0] = firstTile;

                        Array.Copy(combo.ToArray(), 0, groupTiles, 1, tilesUsed - 1);

                        for (var k = 0; k < jokersUsed; k++)
                        {
                            groupTiles[tilesUsed + k] = new Tile(firstTile.Value, TileColor.Black, true);
                        }

                        yield return new Group
                        {
                            Tiles = groupTiles,
                            Jokers = jokersUsed
                        };
                    }
                }
            }
        }
    }

    private void MarkTilesAsUsed(ValidSet set, bool isUsed, bool[] usedTiles, ref int unusedTile,
        ref int availableJokers)
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
            var seenCombinations = new HashSet<List<Tile>>(new TileListComparer());
            for (var i = 0; i < list.Count; i++)
            {
                var element = list[i];
                var remainingList = list.Skip(i + 1).ToList();
                foreach (var combination in GetCombinations(remainingList, length - 1))
                {
                    var newCombination = new List<Tile> { element };
                    newCombination.AddRange(combination);
                    newCombination.Sort();
                    if (seenCombinations.Add(newCombination))
                    {
                        yield return newCombination;
                    }
                }
            }
        }
    }

    private static IEnumerable<IEnumerable<Tile>> GetCombinationsGroup(List<Tile> tiles, int length)
    {
        if (length == 0) yield return new List<Tile>();
        else
        {
            for (var i = 0; i < tiles.Count; i++)
            {
                var remaining = tiles.Skip(i + 1).ToList();
                foreach (var combination in GetCombinations(remaining, length - 1))
                {
                    yield return new List<Tile> { tiles[i] }.Concat(combination);
                }
            }
        }
    }


}