using System;
using System.Threading.Tasks;
using Distel.Grains.Interfaces;
using Orleans;
using Orleans.TestingHost;
using Xunit;

namespace Distel.Tests
{
    public class HotelGrainTests
    {
        [Fact]
        public async Task WelcomeGreeting()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var hotelGrain = cluster.GrainFactory.GetGrain<IHotelGrain>("Taj");
            var greeting = await hotelGrain.WelcomeGreetingAsync("Bhupesh");

            cluster.StopAllSilos();

            Assert.Equal("Hello, World", greeting);
        }
    }
}
