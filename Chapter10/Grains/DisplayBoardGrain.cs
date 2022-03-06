using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using Orleans.Streams.Core;
using System.Threading.Tasks;

namespace Distel.Grains
{
    [ImplicitStreamSubscription("AttractionEvents-NS")]
    public class DisplayBoardGrain : Grain, IDisplayBoardGrain, IStreamSubscriptionObserver
    {
        private readonly ILogger<DisplayBoardGrain> logger;
        private readonly AttractionObserver observer;

        public DisplayBoardGrain(ILogger<DisplayBoardGrain> logger)
        {
            this.logger = logger;
            this.observer = new AttractionObserver(this.Notifier);
        }

        // Called when a subscription is added
        public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
        {
            // Plug our AttractionObserver to the stream
            var handle = handleFactory.Create<AttractionNotification>();
            await handle.ResumeAsync(this.observer);
        }

        private Task Notifier(AttractionNotification notification)
        {
            this.logger.LogInformation("DisplayBoardGrain: " + notification.ToString());
            return Task.CompletedTask;
        }
    }
}
