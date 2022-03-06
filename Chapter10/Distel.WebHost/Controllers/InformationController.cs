using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Distel.WebHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InformationController : ControllerBase
    {
        private readonly ILogger<InformationController> _logger;
        private readonly IClusterClient clusterClient;

        public InformationController(ILogger<InformationController> logger, IClusterClient clusterClient)
        {
            this._logger = logger;
            this.clusterClient = clusterClient;
        }

        [HttpGet("hotels")]
        public async Task<IActionResult> GetHotelChain()
        {
            var grain = this.clusterClient.GetGrain<ICacheGrain<Hotel[]>>("hotelsChain");
            var response = await grain.GetAsync();
            if (response.Value == null)
            {
                var hotels = new Hotel[] {
                    new Hotel{ Name="Tajmahal", Location= "Agra, India" },
                    new Hotel{ Name="The Great Pyramid of Giza", Location= "Giza, Egypt" },
                    new Hotel{ Name=" Temple of Artemis", Location= "Ephesus, Turkey" },
                    new Hotel{ Name="Hanging Gardens", Location= "Al-Hillah, Iraq" },
                    new Hotel{ Name="The Lighthouse of Alexandria", Location= "Alexandria, Egypt" }
                };
                await grain.SetAsync(new Orleans.Concurrency.Immutable<Hotel[]>(hotels));
                return Ok(hotels);
            }

            return Ok(response);
        }

        [HttpGet("hotels2")]
        public async Task<IActionResult> GetHotelChainWithCacheService([FromServices] IDistributedCache distributedCache)
        {
            var response = distributedCache.Get("hotelsChain");

            if (response == null)
            {
                var hotels = new Hotel[] {
                    new Hotel{ Name="Tajmahal", Location= "Agra, India" },
                    new Hotel{ Name="The Great Pyramid of Giza", Location= "Giza, Egypt" },
                    new Hotel{ Name=" Temple of Artemis", Location= "Ephesus, Turkey" },
                    new Hotel{ Name="Hanging Gardens", Location= "Al-Hillah, Iraq" },
                    new Hotel{ Name="The Lighthouse of Alexandria", Location= "Alexandria, Egypt" }
                };
                await distributedCache.SetAsync("hotelsChain", JsonSerializer.SerializeToUtf8Bytes(hotels));
                return Ok(hotels);
            }
            else
            {
                return Ok(JsonSerializer.Deserialize<Hotel[]>(response));
            }
        }

        [HttpGet("ipaddress")]
        public IActionResult GetIPAddress()
        {
            string ip = Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_IP");
            if (string.IsNullOrWhiteSpace(ip))
                ip = "No IP";

            string ports = Environment.GetEnvironmentVariable("WEBSITE_PRIVATE_PORTS");
            if (string.IsNullOrWhiteSpace(ports))
                ports = "No ports";

            return Ok($"Connection: {ip} ||| {ports}");
        }
    }
        [Serializable]
    class Hotel
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }

}
