using Distel.Grains.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Distel.WebHost.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DistelController : ControllerBase
    {
        private readonly ILogger<DistelController> _logger;
        private readonly IClusterClient clusterClient;

        public DistelController(ILogger<DistelController> logger, IClusterClient clusterClient)
        {
            this._logger = logger;
            this.clusterClient = clusterClient;
        }
        [HttpGet("welcome/{hotel}/{guestname}")]
        public async Task<IActionResult> WelcomeGuest([FromRoute] string hotel, [FromRoute] string guestname)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var greeting = await hotelGrain.WelcomeGreetingAsync(guestname);           
            _logger.LogWarning("Warning: Welcome Guest successful for guest " + guestname + "for hotel " + hotel);
            _logger.LogError("Error: Welcome Guest successful for guest " + guestname + "for hotel " + hotel);
            return Ok(greeting);
            
        }


        [HttpPost("checkin/{hotel}/")]
        public async Task<IActionResult> CheckIn([FromRoute] string hotel, [FromBody] UserCheckIn userCheckInDetails)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var alottedRoom = await hotelGrain.CheckInGuest(userCheckInDetails);
            return Ok(alottedRoom);
        }


        [HttpPost("checkout/{hotel}/")]
        public async Task<IActionResult> CheckOut([FromRoute] string hotel, [FromBody] UserCheckOut userCheckOutDetails)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var alottedRoom = await hotelGrain.CheckOutGuest(userCheckOutDetails);
            return Ok(alottedRoom);
        }

    }

}
