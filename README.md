ParallelStockPaths
==================

ParallelStockPaths is a small C# project aiming to the advantage and the simplicity of using .Net parallelism mechanisms.

In particular, it will explore the performance of a simple LINQ query compared to its parallelized counterpart PLINQ.

The example chose is the generation of multiple prices paths of a stock, assuming a log-normal distribution (Geometric Brownian Motion).

This project uses the Math.net packages (using NuGet).