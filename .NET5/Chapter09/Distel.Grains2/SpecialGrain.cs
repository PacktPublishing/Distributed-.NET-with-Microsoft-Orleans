using Distel.Grains.Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains2
{
    public class SpecialGrain : Grain, ISpecialGrain
    {
        public Task<string> SpecialAction()
        {
            return Task.FromResult("From special grain");
        }
    }
}
