using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IMessageDispatcherGrain : IGrainWithIntegerKey
    {
        Task Send(List<string> messages);
    }
}
