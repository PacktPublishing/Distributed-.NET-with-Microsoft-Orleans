using Distel.DataTier.Abstractions;
using Distel.DataTier.Abstractions.Models;
using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Streams;
using System;
using System.Threading.Tasks;

namespace Distel.Grains
{
    public class UserGrain : Grain, IUserGrain
    {
        private readonly ILogger<UserGrain> logger;
        private readonly IUserRepository userRepository;

        public UserGrain(ILogger<UserGrain> logger, IUserRepository userRepository)
        {
            this.logger = logger;
            this.userRepository = userRepository;
        }

        public async Task<TravelHistory> GetTravelHistory()
        {
            var userId = this.GetPrimaryKeyString();
            var history = await userRepository.GetTravelHistoryAsync(userId);
            return history;
        }

        public Task SubscribeToAttractionEventsAsync(Guid displayBoardId, string nameSpace) =>
            GetStreamProvider("attractions-stream")
            .GetStream<AttractionNotification>(displayBoardId, nameSpace)
            .SubscribeAsync(new AttractionObserver(Notifier));

        private Task Notifier(AttractionNotification notification)
        {
            this.logger.LogInformation("UserGrain: " + notification.ToString());
            return Task.CompletedTask;
        }
    }

    class AttractionObserver : IAsyncObserver<AttractionNotification>
    {
        private readonly Func<AttractionNotification, Task> action;
        public AttractionObserver(Func<AttractionNotification, Task> action) =>
            this.action = action;
        public Task OnCompletedAsync() => Task.CompletedTask;
        public Task OnErrorAsync(Exception ex) => Task.CompletedTask;
        public Task OnNextAsync(AttractionNotification item, StreamSequenceToken token = null) =>
            action(item);
    }
}
