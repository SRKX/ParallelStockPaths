using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using System.Diagnostics;

namespace ParallelStockPaths
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initialization.");
            
            //We simulate for 10 years each with 252 days.
            int nbrPoints = 2520;
            //The number of paths will will compute
            int nbrPaths = 100000;

            //We compute the daily mean (assuming an annualized 3%)
            double mean = Math.Pow(1.03, 1.0 / 252.0) - 1.0;
            //We compute the daily volatility (assuming an annualized 10%)
            double stdDev = 0.1 / Math.Sqrt(252.0);

            //We define the creator function
            Func<int,StockPath> creator = x => new StockPath(nbrPoints, mean, stdDev);

            //We run a simulation in both modes.
            RunSimulation(nbrPoints, nbrPaths, "classic", creator, ExecutionMode.CLASSIC);
            RunSimulation(nbrPoints, nbrPaths, "parallel", creator, ExecutionMode.PARALLEL);
                        
            Console.WriteLine("End of analysis.");
            Console.ReadLine();
            
        }

        /// <summary>
        /// Represents the exectuion mode of a simulation (either classic or in parallel).
        /// </summary>
        public enum ExecutionMode
        {
            CLASSIC,
            PARALLEL
        }

        /// <summary>
        /// Runs a simulation and prints results on the console.
        /// </summary>
        /// <param name="nbrPoints">The number of points for each path to be generated.</param>
        /// <param name="nbrPaths">The number of paths to be generated.</param>
        /// <param name="simulationName">The name of the simulation</param>
        /// <param name="creator">The function used to create the <seealso cref="StockPath"/> from a given index.</param>
        /// <param name="mode">The <see cref="Program.ExecutionMode"/></param>
        public static void RunSimulation(int nbrPoints, int nbrPaths, string simulationName, Func<int,StockPath> creator, ExecutionMode mode)
        {
            Stopwatch stopWatch = new Stopwatch();
            StockPath[] paths = new StockPath[nbrPaths];
            IEnumerable<int> indices = Enumerable.Range(0, nbrPaths - 1);
            Console.WriteLine("Starting " + simulationName + " simulation.");
            stopWatch.Start();

            switch (mode)
            {
                case ExecutionMode.CLASSIC:
                    paths = indices.Select(creator).ToArray();
                    break;
                case ExecutionMode.PARALLEL:
                    paths = indices.AsParallel().Select(creator).ToArray();
                    break;
                default:
                    throw new ArgumentException("Unknown execution mode", "mode");
            }

            stopWatch.Stop();
            Console.WriteLine("End of " + simulationName + " simulation.");
            var lastPrices = paths.Select(x => x.LastPrice);
            Console.WriteLine("Min price: " + lastPrices.Min().ToString("N2"));
            Console.WriteLine("Max price: " + lastPrices.Max().ToString("N2"));
            Console.WriteLine("Computation time: " + stopWatch.Elapsed.TotalSeconds.ToString("N2") + " sec");
        }


    }
}
