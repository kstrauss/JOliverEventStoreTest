using System;
using JOliverEventStoreTest;
using NServiceBus.Testing;
using NUnit.Framework;

namespace HandlerUnitTests
{
    [TestFixture]
    public class UnitTest1
    {

        [Test]
        public void DoesHandlerDoWork()
        {
            Test.Initialize();

            var handler = Test.Handler(bus => new TestSubscriber(bus))
                .ExpectReply<MessageProcessedMessage>(msg => (msg.Result == MessageProcessedMessage.Status.Success));
            handler.ExpectNotPublish<MessageProcessedMessage>(msg => false);
            handler.OnMessage<TestDomainMessage>(msg=> Console.WriteLine("hello world"));
            //Test.Bus.Publish(new TestDomainMessage("helo bus"));
        }
    }
}
