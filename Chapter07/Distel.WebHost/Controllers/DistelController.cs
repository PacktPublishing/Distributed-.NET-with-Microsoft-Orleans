using Distel.Grains.Interfaces;
using Distel.Grains.Interfaces.Models;
using Microsoft.AspNetCore.Mvc;
using Orleans;

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
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var greeting = await hotelGrain.WelcomeGreetingAsync(guestname);
            _logger.LogWarning("Warning: Welcome Guest successful for guest " + guestname + "for hotel " + hotel);
            _logger.LogError("Error: Welcome Guest successful for guest " + guestname + "for hotel " + hotel);

            return Ok(greeting);
        }

        /// <summary>
        /// Checkin Guest to Distel
        /// </summary>
        /// <param name="hotel">Hotel</param>
        /// <param name="userCheckInDetails">user check-in details.</param>
        /// <returns>Allotted room</returns>
        [HttpPost("checkin/{hotel}/")]
        public async Task<IActionResult> CheckIn([FromRoute] string hotel, [FromBody] UserCheckIn userCheckInDetails)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var alottedRoom = await hotelGrain.CheckInGuest(userCheckInDetails);
            return Ok(alottedRoom);
        }

        /// <summary>
        /// Checkout the Guest from Distel
        /// </summary>
        /// <param name="hotel">Hotel</param>
        /// <param name="userCheckInDetails">user check-in details.</param>
        /// <returns>Allotted room</returns>
        [HttpPost("checkout/{hotel}/")]
        public async Task<IActionResult> CheckOut([FromRoute] string hotel, [FromBody] UserCheckIn userCheckInDetails)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            var alottedRoom = await hotelGrain.CheckOutGuest(userCheckInDetails);
            return Ok(alottedRoom);
        }

        /// <summary>
        /// Checkout the Guest from Distel
        /// </summary>
        /// <param name="hotel">Hotel</param>
        /// <param name="userCheckInDetails">user check-in details.</param>
        /// <returns>Allotted room</returns>
        [HttpPost("partner/{hotel}/onboard")]
        public async Task<IActionResult> OnboardPartner([FromRoute] string hotel, [FromBody] Partner partner)
        {
            var hotelGrain = this.clusterClient.GetGrain<IHotelGrain>(hotel);
            await hotelGrain.AssociatePartner(partner);
            return Ok();
        }
    }
}
