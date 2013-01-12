using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace ParallelStockPaths
{

    /// <summary>
    /// Represents a path of the prices of a given stock given a mean and a standard deviation.
    /// </summary>
    /// <remarks>
    /// In order not to waste memory, the complete path is not actually stored in this implementation, we only keep the last price to see where the path ended up.
    /// </remarks>
    class StockPath
    {
        private readonly double _timeInterval;
        /// <summary>
        /// The time interval between two evaluation points.
        /// </summary>
        public double TimeInterval
        {
            get { return _timeInterval; }
        }

        private readonly double _mean;
        //The mean of the returns of the underlying model.
        public double Mean
        {
            get { return _mean; }
        }

        
        private readonly double _standardDeviation;
        
        /// <summary>
        /// The standard deviation of the returns using the underlying process.
        /// </summary>
        public double StandardDeviation
        {
            get { return _standardDeviation; }
        }


        private double _lastPrice;
        /// <summary>
        /// The last price of the simulated path.
        /// </summary>
        public double LastPrice
        {
            get
            {
                return _lastPrice;
            }

        }

        public StockPath(int nbrSteps, double mean, double std)
            :this(nbrSteps,mean,std,1.0,100.0)
        { }

        public StockPath(int nbrSteps, double mean, double std, double timeInterval, double initialPoint)
        {
            //Assigns internal setup variables
            _timeInterval = timeInterval;
            _mean = mean;
            _standardDeviation = std;

            //Using Math.Net library to compute the required random noise.
            Normal normal = Normal.WithMeanStdDev(0, 1);
            IEnumerable<double> returns = normal.Samples().Take(nbrSteps);
            
            //Direct lambda implementation for aggregation; this version is slightly quicker than the explicit one.
            //_lastPrice = returns.Aggregate(100.0, (st, wt) => (Math.Max(st + st * mean * _timeInterval + std * st * wt, 0.0)));
            
            //Explicit implementation of aggregation mechanism.
            _lastPrice = returns.Aggregate(initialPoint, _computeNextStockPrice);
        }
        
        /// <summary>
        /// Computes the change in price (delta S) between two points in the paths.
        /// </summary>
        /// <param name="currentStockPrice">The price of the stack at the first (known) point.</param>
        /// <param name="randomNoise">The random noise used to estimate the change.</param>
        /// <returns>The absolute change in price between two points according to the model.</returns>
        /// <remarks>The underlying model assumes a geometric brownian motion.</remarks>
        private double _computeStockPriceChange(double currentStockPrice, double randomNoise)
        {
            return currentStockPrice * _mean * _timeInterval + _standardDeviation * currentStockPrice * randomNoise;
        }

        /// <summary>
        /// Computes the next stock price given a <paramref name="currentStockPrice"/>.
        /// </summary>
        /// <param name="currentStockPrice">The price of the stack at the first (known) point.</param>
        /// <param name="randomNoise">The random noise used to estimate the change.</param>
        /// <returns>The next stock price according to the model. This value will never be less than 0.0.</returns>
        /// <remarks>
        /// The model makes sure that a price cannot actually go below 0.0.
        /// The underlying model assumes a geometric brownian motion.
        /// </remarks>
        private double _computeNextStockPrice(double currentStockPrice, double randomNoise)
        {
            return Math.Max(currentStockPrice + _computeStockPriceChange(currentStockPrice, randomNoise), 0.0);
        }



        private static IList<T> _aggregate<T>(IEnumerable<T> collection, T acc, Func<T, T, T> aggregator)
        {
            return _aggregate(collection, new List<T>() { acc }, acc, aggregator);
        }

        private static IList<T> _aggregate<T>(IEnumerable<T> collection, IList<T> accCollection, T acc, Func<T,T,T> aggregator)
        {
            if (collection.Count() == 0)
                return accCollection;
            else if (collection.Count() == 1)
            {
                accCollection.Add(aggregator(collection.First(), acc));
                return accCollection;
            }
            else
            {
                T nextAcc = aggregator(collection.First(), acc);
                accCollection.Add(nextAcc);
                return _aggregate(collection.Skip(1), accCollection, nextAcc, aggregator);
            }
        }

       

    }
}
