using Distel.DataTier.Abstractions.Models;
using Orleans;
using System;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IUserGrain : IGrainWithStringKey
    {
        Task<TravelHistory> GetTravelHistory();
        Task SubscribeToAttractionEventsAsync(Guid displayBoardId, string nameSpace);
    }
}

