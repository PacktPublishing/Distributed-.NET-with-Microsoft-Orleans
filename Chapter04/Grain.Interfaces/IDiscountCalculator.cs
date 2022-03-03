﻿using Orleans;

namespace Distel.Grains.Abstractions
{
    public interface IDiscountCalculator : IGrainWithIntegerKey
    {
        Task<decimal> ComputeDiscount(decimal price);
    }
}
