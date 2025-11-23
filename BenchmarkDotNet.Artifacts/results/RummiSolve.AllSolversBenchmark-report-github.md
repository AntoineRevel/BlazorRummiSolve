```

BenchmarkDotNet v0.15.6, macOS 26.1 (25B78) [Darwin 25.1.0]
Apple M3, 1 CPU, 8 logical and 8 physical cores
.NET SDK 10.0.100
  [Host]     : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a
  Job-KDXSYM : .NET 10.0.0 (10.0.0, 10.0.25.52411), Arm64 RyuJIT armv8.0-a

IterationCount=3  WarmupCount=2  

```
| Method                                  | Mean           | Error           | StdDev        | Ratio | RatioSD | Rank | Gen0        | Gen1        | Gen2      | Allocated  | Alloc Ratio |
|---------------------------------------- |---------------:|----------------:|--------------:|------:|--------:|-----:|------------:|------------:|----------:|-----------:|------------:|
| CombinationsSolver_Test                 |    34,811.1 μs |       787.52 μs |      43.17 μs | 0.006 |    0.00 |    3 |  11000.0000 |    133.3333 |         - |   88.15 MB |       0.019 |
| ParallelCombinationsSolver_Test         |    11,920.8 μs |     4,998.00 μs |     273.96 μs | 0.002 |    0.00 |    2 |  11093.7500 |   1000.0000 |  171.8750 |   88.26 MB |       0.019 |
| BestScoreComplexSolver_Test             |       542.5 μs |       123.59 μs |       6.77 μs | 0.000 |    0.00 |    1 |    189.4531 |      1.9531 |         - |    1.51 MB |       0.000 |
| IncrementalComplexSolver_Test           |       446.6 μs |        48.56 μs |       2.66 μs | 0.000 |    0.00 |    1 |    159.6680 |      2.4414 |         - |    1.28 MB |       0.000 |
| IncrementalScoreFieldComplexSolver_Test |       449.5 μs |        12.47 μs |       0.68 μs | 0.000 |    0.00 |    1 |    159.6680 |      2.4414 |         - |    1.28 MB |       0.000 |
| IncrementalComplexSolverTileAndSc_Test  |       470.4 μs |        12.91 μs |       0.71 μs | 0.000 |    0.00 |    1 |    159.6680 |      1.9531 |         - |    1.28 MB |       0.000 |
| GraphSolver_Test                        | 6,152,208.7 μs | 6,953,502.61 μs | 381,144.94 μs | 1.003 |    0.08 |    5 | 572000.0000 | 192000.0000 | 2000.0000 | 4608.54 MB |       1.000 |
| SequentialGraphSolver_Test              | 4,199,174.3 μs | 3,241,675.86 μs | 177,687.19 μs | 0.684 |    0.05 |    4 | 568000.0000 | 205000.0000 | 6000.0000 | 4589.24 MB |       0.996 |
