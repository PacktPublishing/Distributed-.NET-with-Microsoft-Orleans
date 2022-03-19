using Distel.Grains.Interfaces;
using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleClientApp
{

    class HotelClient : IObserver
    {
        public void ReceiveMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
