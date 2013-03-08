using System;
using NServiceBus;
using NServiceBus.Scheduling.Messages;

namespace JOliverEventStoreTest
{
    public class TestSubscriber : IHandleMessages<TestDomainMessage>, 
        IHandleMessages<NServiceBus.Scheduling.Messages.ScheduledTask>
    {
        public IBus Bus { get; set; }
        public TestSubscriber(IBus b)
        {
            Bus = b;
        }

        public void Handle(TestDomainMessage message)
        {
            Console.WriteLine("I Got a message and could now do something useful with it.");
            Bus.Publish(new MessageProcessedMessage(MessageProcessedMessage.Status.Success));
        }


        public void Handle(ScheduledTask message)
        {
            Console.WriteLine("Got a scheduled Task");
        }
    }
}