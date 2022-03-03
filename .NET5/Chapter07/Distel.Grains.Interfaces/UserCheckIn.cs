using System;
using System.Collections.Generic;
using System.Text;

namespace Distel.Grains.Interfaces
{
    public class UserCheckIn
    {
        public string UserId { get; set; }
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NUmberOfGuests { get; set; }
        public string RoomNumber { get; set; }
    }
}
