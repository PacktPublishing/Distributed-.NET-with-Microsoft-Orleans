using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distel.Grains.Interfaces
{
    public interface IObserver : IGrainObserver
    {
        void ReceiveMessage(string message);
    }

}
