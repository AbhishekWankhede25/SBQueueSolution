using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System.Configuration;
using System.Threading;

namespace SBReceiveQueueConApp
{
    class Program
    {
        static NamespaceManager _namespaceManager;
        static void Main(string[] args)
        {
            CollectSBDetails();
            CreateQueueToRead();
            //Console.WriteLine("Read all the Data");
            Console.ReadKey(true);
        }

        private static void CreateQueueToRead()
        {
            TokenProvider tokenProvider = _namespaceManager.Settings.TokenProvider;
            if (_namespaceManager.QueueExists("categoryqueue"))
            {
                MessagingFactory factory = MessagingFactory.Create(_namespaceManager.Address, tokenProvider);

                QueueClient catsQueueClient = factory.CreateQueueClient("categoryqueue");

                Console.WriteLine("Receiving the Messages from the Queue....");
                BrokeredMessage message;
                int ctr = 1;
                while ((message = catsQueueClient.Receive(new TimeSpan(hours: 0, minutes: 1, seconds: 5))) != null)
                {
                    Console.WriteLine($"Message Received, Sequance: {message.SequenceNumber}, MessageID: {message.MessageId},\nCat: {message.Properties[(ctr++).ToString()]}");
                    message.Complete();
                    Console.WriteLine("Processing Message (sleeping).....");
                    Thread.Sleep(1000);
                }
                factory.Close();
                catsQueueClient.Close();
                _namespaceManager.DeleteQueue("categoryqueue");
                Console.WriteLine("Finished getting all the data from the queue, Press any key to exit");
            }
        }

        private static void CollectSBDetails()
        {
            _namespaceManager = NamespaceManager.CreateFromConnectionString(ConfigurationManager.AppSettings["Microsoft.ServiceBus.ConnectionString"].ToString());
        }
    }
}
