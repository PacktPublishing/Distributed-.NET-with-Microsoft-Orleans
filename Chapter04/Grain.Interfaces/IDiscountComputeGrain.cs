using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IDiscountComputeGrain : IGrainWithIntegerKey
    {
        Task<decimal> ComputeDiscount(decimal price);
    }
}
