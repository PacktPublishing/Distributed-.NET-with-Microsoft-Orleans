using Distel.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Distel.Grains
{
    [StatelessWorker(maxLocalWorkers: 1)]
    public class DiscountComputeGrain : Grain, IDiscountComputeGrain
    {
        public async Task<decimal> ComputeDiscount(decimal price)
        {
            var discount = price switch
            {
                > 100 => price * 0.1M,
                > 50 => price * 0.05M,
                _ => 0
            };
            await Task.Delay(2000);
            return discount;
        }
    }
}
