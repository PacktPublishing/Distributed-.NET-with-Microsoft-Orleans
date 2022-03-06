using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IIntermediatoryGrain : IGrainWithIntegerKey
    {
        Task UpdateDelta(int value);
    }
}
