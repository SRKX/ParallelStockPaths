using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;

namespace ParallelStockPaths
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting simulation");
            int nbrPoints = 2500;
            int nbrPaths = 100000;
            StockPath[] paths = new StockPath[nbrPaths];
            DateTime start = DateTime.Now;
            //for (int i = 0; i < nbrPaths; i++)
            //{
            //    paths[i] = new StockPath(nbrPoints, 0.00008, 0.0063);
            //}

            var indices = Enumerable.Range(0, nbrPaths - 1);
            paths = indices.AsParallel().Select(x => new StockPath(nbrPoints, 0.00008, 0.0063)).ToArray();

            DateTime end = DateTime.Now;
            Console.WriteLine("Simulation done");
            //StockPath path = new StockPath(nbrPoints, 0.00008, 0.0063);
            //Console.WriteLine("Last price: " + path.LastPrice);

            var lastPrices = paths.Select(x=>x.LastPrice);

            Console.WriteLine("Min price: " + lastPrices.Min());
            Console.WriteLine("Max price: " + lastPrices.Max());
            Console.WriteLine("Time Elapsed: " + end.Subtract(start).TotalSeconds + " sec.");
            Console.WriteLine("Program terminated");
            Console.ReadLine();
            
        }
    }
}
