using Distel.Grains.Interfaces;
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
    /// Controller to manage user details
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IClusterClient clusterClient;

        public UserController(ILogger<UserController> logger, IClusterClient clusterClient)
        {
            this._logger = logger;
            this.clusterClient = clusterClient;
        }

        /// <summary>
        /// Gets user travel History
        /// </summary>
        /// <param name="userId">NUser Id.</param>
        /// <returns>Travel History of the User.</returns>
        [HttpGet("/{userId}/travelhistory")]
        public async Task<IActionResult> WelcomeGuest([FromRoute] string userId)
        {
            var userGrain = this.clusterClient.GetGrain<IUserGrain>(userId);
            var greeting = await userGrain.GetTravelHistory();
            return Ok(greeting);
        }
    }
}
