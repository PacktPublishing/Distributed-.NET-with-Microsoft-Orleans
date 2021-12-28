using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface ISpecialGrain : IGrainWithGuidKey
    {
        Task<string> SpecialAction();
    }
}
