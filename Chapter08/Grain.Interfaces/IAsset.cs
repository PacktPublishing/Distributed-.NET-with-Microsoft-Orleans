using Orleans;
using Orleans.Concurrency;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IAsset : IGrainWithStringKey
    {
        [AlwaysInterleave]
        Task<TimeSpan> GetTimeToService();
    }
}
