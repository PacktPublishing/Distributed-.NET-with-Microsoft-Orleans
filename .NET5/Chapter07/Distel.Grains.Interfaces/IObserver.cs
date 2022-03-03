using System;
using System.Collections.Generic;
using System.Text;
using Orleans;

namespace Distel.Grains.Interfaces
{
    public interface IObserver : IGrainObserver
    {
        void ReceiveMessage(string message);
    }
}
