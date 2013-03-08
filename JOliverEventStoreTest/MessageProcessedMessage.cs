using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace JOliverEventStoreTest
{

    public class MessageProcessedMessage : IEvent
    {
        public enum Status
        {
            Success,Failure
        }

        public Status Result { get; private set; }
        public MessageProcessedMessage(Status r)
        {
            Result = r;
        }
    }
}
