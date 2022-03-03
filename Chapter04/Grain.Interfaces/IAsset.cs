﻿using Orleans;
using Orleans.Concurrency;

namespace Distel.Grains.Abstractions
{
    public interface IAsset : IGrainWithStringKey
    {
        [AlwaysInterleave]
        Task<TimeSpan> GetTimeToService();
    }
}
