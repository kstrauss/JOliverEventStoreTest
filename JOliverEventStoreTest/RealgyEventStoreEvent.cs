using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventStore;

namespace JOliverEventStoreTest
{
    [Serializable]
    class RealgyEventStoreEvent : EventMessage
    {
        public RealgyEventStoreEvent()
        {
            this.Headers.Add("Processed", DateTime.Now);
        }
    }
}
