using Distel.Grains.Interfaces;
using Orleans;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class CampaignGrain : Grain, ICampaignGrain
    {
        public Task ReceiveUserEngagementUpdate(string hotelGrianId, int hitCount)
        {
            // Do something to check the campaign effectiveness
            return Task.CompletedTask;
        }

        public Task ReceiveUserEngagementUpdate(int hitCount)
        {
            // Do something to check the campaign effectiveness
            return Task.CompletedTask;
        }
    }
}
