using System;

namespace Distel.Grains.Interfaces.Models
{
    public class UserCheckIn
    {
        public string UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
