using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using EventStore;
using EventStore.Dispatcher;
using EventStore.Persistence.SqlPersistence.SqlDialects;
using NServiceBus;
using NServiceBus.Hosting.Configuration;

namespace JOliverEventStoreTest
{
    public class Program
    {
        public Program()
        {
            
        }
        static void Main(string[] args)
        {
            var bus = Configure.With().Log4Net()
                .DefaultBuilder()
                //.InMemorySubscriptionStorage()
                
                //
                .MsmqTransport()
                .RavenPersistence("NServiceBus.Persistence", "NServiceBusKarlTest")
                .RavenSubscriptionStorage()
                //.DontUseTransactions()
                //.MessageForwardingInCaseOfFault()
                //.MsmqSubscriptionStorage()
                .UnicastBus()
                    /* if you do not do this there will be no messages placed in the
                     * subscription queue to allow MsmqSubscriptionStorage to work
                     * Interestingly enough if you use InMemorySubscriptionStorage 
                     * subscriptions will work. If you remove LoadMessageHandlers 
                     * the subscriptions are already in the queue, so it knows to forward
                     * the messages to the existing handlers.
                     * 
                     * If i had more energy I would submit a patch for nservicebus, but
                     * really??
                     */
                    .LoadMessageHandlers()
                .AllowSubscribeToSelf()
                .CreateBus()
                .Start();
            var myListener = new TestSubscriber(bus);
            bus.Subscribe(typeof(TestDomainMessage));
            

            IStoreEvents store = Wireup.Init()
                .LogToConsoleWindow()
                //.UsingInMemoryPersistence()
                
                .UsingSqlPersistence("Test")
                    .WithDialect(new MsSqlDialect())
                    .InitializeStorageEngine()
                    .UsingBinarySerialization()
                .UsingAsynchronousDispatchScheduler(new MyDispatcher(bus))
                .Build();
            try
            {
                PopulateStore(store, bus);
            }
            catch (Exception)
            {
                
                Console.Error.WriteLine("Done.");
            }
            
            Console.ReadKey();
        }
        static void PopulateStore(IStoreEvents store, IBus bus)
        {
            var agID = Guid.NewGuid();
            var stream = store.CreateStream(agID);
            var loE = new List<EventMessage>();
            AddTextMessage(loE, "CreateEntity");
            AddTextMessage(loE, "SupplyHedgedForPeriod");
            AddTextMessage(loE, "SupplyHedgedForPeriod");
            AddTextMessage(loE, "Usage");

            loE.ForEach(stream.Add);
            
            using (var scope = new TransactionScope())
            {
                Console.WriteLine("In transaction {0}", Transaction.Current != null);
                stream.CommitChanges(Guid.NewGuid());
                throw new Exception("Bad stuff here");
                bus.Publish(new TestDomainMessage("hello"));
                scope.Complete();
            }
        }

        //static void 
        static void AddTextMessage(List<EventMessage> list, String body)
        {
            list.Add(new RealgyEventStoreEvent()
            {
                Body = body
            });
        }


        /// <summary>
        /// this is where we should send to the bus...
        /// </summary>
        /// <param name="commit"></param>
        private static void DispatchCommit(Commit commit)
        {
            // This is where we'd hook into our messaging infrastructure, such as NServiceBus,
            // MassTransit, WCF, or some other communications infrastructure.
            // This can be a class as well--just implement IDispatchCommits.
            try
            {
                foreach (var @event in commit.Events)
                {
                    Console.WriteLine("Dispatching: {0}", @event.Body.ToString());
                }
                
        }
            catch (Exception)
            {
                Console.WriteLine("unable to dispatch");
            }
        }

        public class MyDispatcher : IDispatchCommits
        {
            private readonly IBus mybus;
            public MyDispatcher(IBus bus)
            {
                mybus = bus;
            }
            public void Dispose()
            {
                throw new NotImplementedException();
            }

            public void Dispatch(Commit commit)
            {
                try
                {
                    foreach (var @event in commit.Events)
                    {
                        Console.WriteLine("Dispatching: {0}", @event.Body.ToString());
                        var msg = new TestDomainMessage(@event.Body.ToString());
                        //TODO: this should be a publish not a send to myself...
                        mybus.Publish<TestDomainMessage>(msg);
                        //mybus.Send("jolivereventstoretest", msg);
                    }
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("unable to dispatch");
                    throw e;
                }
            }
        }

    }
}
