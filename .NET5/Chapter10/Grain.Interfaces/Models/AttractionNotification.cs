using System;

namespace Distel.Grains.Interfaces.Models
{
    public class AttractionNotification
    {
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public override string ToString() =>
            $"There is \"{Description}\" on {EventDate}";
    }
}
