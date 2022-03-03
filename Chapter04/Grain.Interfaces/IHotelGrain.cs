﻿using Orleans;

namespace Distel.Grains.Abstractions
{
    public interface IHotelGrain : IGrainWithStringKey
    {
        Task<string> WelcomeGreetingAsync(string guestName);
        Task<string> GetKey();

        Task OnboardFromOtherHotel(IHotelGrain fromHotel, string guestName);
        Task<decimal> ComputeDue(string guestName);
    }
}

