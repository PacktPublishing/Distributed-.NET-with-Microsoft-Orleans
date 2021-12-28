using Distel.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Distel.Grains
{
    [StatelessWorker(maxLocalWorkers: 1)]
    public class MessageDispatcherGrain : Grain, IMessageDispatcherGrain
    {
        public Task Send(List<string> messages)
        {
            var tasks = new List<Task>();
            foreach (var message in messages)
            {
                var parts = message.Split(':');
                var grain = GrainFactory.GetGrain<IHotelGrain>(parts[0]);
                tasks.Add(grain.ReceiveMessage(parts[1]));
            }
            return Task.WhenAll(tasks);
        }
    }
}
