﻿using BenchmarkDotNet.Running;

namespace RummiSolve;

public static class Program
{
    private static void Main()
    {
        BenchmarkRunner.Run<RummiBench>();
        //RummiBench.TestMultiPlayerGame();
    }
    
}