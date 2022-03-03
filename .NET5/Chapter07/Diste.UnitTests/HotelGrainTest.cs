using Distel.Grains.Interfaces;
using Orleans.Hosting;
using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Distel.UnitTests
{

    public class HotelGrainTest
    {
       
        [Fact]
        public async Task WelcomeGreeting()
        {
            var builder = new TestClusterBuilder();
            builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
            var cluster = builder.Build();
            cluster.Deploy();

            var hotelGrain = cluster.GrainFactory.GetGrain<IHotelGrain>("Taj");
            var greeting = await hotelGrain.WelcomeGreetingAsync("Bhupesh");

            cluster.StopAllSilos();

            Assert.Equal("Dear Bhupesh, We welcome you to Distel and hope you enjoy a comfortable stay at our hotel. ", greeting);
        }
    }
}
