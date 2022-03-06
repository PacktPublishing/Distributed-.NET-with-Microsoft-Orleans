using Distel.Grains.Interfaces;
using Orleans;
using Orleans.Concurrency;
using System;
using System.Threading.Tasks;

namespace Distel.Grains
{
    [StatelessWorker]
    public class IntermediatoryGrain : Grain, IIntermediatoryGrain
    {
        int runningTotal = 0;
        public override Task OnActivateAsync()
        {
            RegisterTimer(ReportToAggregateGrain, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            return base.OnActivateAsync();
        }

        async Task ReportToAggregateGrain(object obj)
        {
            var compaignGrain = GrainFactory.GetGrain<ICampaignGrain>("campaignKey");
            await compaignGrain.ReceiveUserEngagementUpdate(runningTotal);
            runningTotal = 0;
        }

        public Task UpdateDelta(int value)
        {
            runningTotal += value;
            return Task.CompletedTask;
        }
    }
}
