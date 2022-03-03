using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IDiscountCalculator : IGrainWithIntegerKey
    {
        Task<decimal> ComputeDiscount(decimal price);
    }
}
