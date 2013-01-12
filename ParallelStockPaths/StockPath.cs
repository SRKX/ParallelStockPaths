using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Distributions;

namespace ParallelStockPaths
{
    class StockPath
    {

        private double _lastPrice;
        public double LastPrice
        {
            get
            {
                return _lastPrice;
            }

        }

        public StockPath(int nbrSteps, double mean, double std)
        {
            double dt = 1;
            Normal normal = Normal.WithMeanStdDev(0,1);
            IEnumerable<double> returns = normal.Samples().Take(nbrSteps);
            _lastPrice = returns.Aggregate(100.0, (st, wt) => (Math.Max(st + st * mean * dt + std * st * wt,0.0)));
        }

    }
}
