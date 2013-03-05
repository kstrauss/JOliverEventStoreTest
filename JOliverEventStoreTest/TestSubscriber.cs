using System;
using NServiceBus;
using NServiceBus.Scheduling.Messages;

namespace JOliverEventStoreTest
{
    public class TestSubscriber : IHandleMessages<TestDomainMessage>
    {
        public IBus Bus { get; set; }
        public TestSubscriber(IBus b)
        {
            Bus = b;
            //Bus.Subscribe<TestDomainMessage>();
        }

        public void Handle(TestDomainMessage message)
        {
            Console.WriteLine("I Got a message");
        }

    }
}