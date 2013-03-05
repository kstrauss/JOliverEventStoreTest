using System;
using NServiceBus;

namespace JOliverEventStoreTest
{
    public class TestDomainMessage : IEvent
    {
        public String Value { get; private set; }
        public TestDomainMessage(String value)
        {
            Value = value;
        }
    }
}