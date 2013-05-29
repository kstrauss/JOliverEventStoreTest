using System;
using NServiceBus;
using NServiceBus.Scheduling.Messages;
using log4net;

namespace JOliverEventStoreTest
{
    public class TestSubscriber : IHandleMessages<TestDomainMessage>, 
        IHandleMessages<NServiceBus.Scheduling.Messages.ScheduledTask>
    {

        private static readonly ILog Logger = LogManager.GetLogger("OurDomainHandlers");
        public IBus Bus { get; set; }
        public TestSubscriber(IBus b)
        {
            Bus = b;
        }

        public void Handle(TestDomainMessage message)
        {
            String msg = "I Got a TestDomainMessage and will now publish a MessageProcessedMessage.";
            Logger.Debug(msg);
            Bus.Publish(new MessageProcessedMessage(MessageProcessedMessage.Status.Success));
        }


        public void Handle(ScheduledTask message)
        {
            Console.WriteLine("Got a scheduled Task");
        }
    }
}