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
    /// <summary>
    /// Controller to manage the Hotel
    /// </summary>
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
        /// <summary>
        /// Generate the greeting message to the guest.
        /// </summary>
        /// <param name="hotel">Name of the Hotel.</param>
        /// <param name="guestname">Guest Name.</param>
        /// <returns>Greet to the Guest.</returns>
        [HttpGet("welcome/{hotel}/{guestname}")]
        public async Task<IActionResult> WelcomeGuest([FromRoute]string hotel, [FromRoute] string guestname)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotel>(hotel);
            var greeting = await hotelGrain.WelcomeGreeting(guestname);
            return Ok(greeting);
        }
    }
}
