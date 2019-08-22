using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Azure.EventHubs;

using Swords.EventHubReader.Contracts;

namespace Swords.EventHubReader
{
    internal class EventHubReader : IEventHubReader
    {
        public event EventHandler<EventDataEventArgs> MessageReceived;

        private readonly string _connectionString;
        private readonly string _entityPath;
        private readonly string _consumerGroup;

        public EventHubReader(string connectionString, string entityPath, string consumerGroup)
        {
            _connectionString = connectionString;
            _entityPath = entityPath;
            _consumerGroup = consumerGroup;
        }

        public void Read()
        {
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(_connectionString)
            {
                EntityPath = _entityPath
            };

            var eventHubClient = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());
            var messageCount = 0;
            var receiver = eventHubClient.CreateReceiver(_consumerGroup, "0", EventPosition.FromEnd());
            var receiver2 = eventHubClient.CreateReceiver(_consumerGroup, "1", EventPosition.FromEnd());
            var retries = 5;
            var currentRetries = 0;

            Console.WriteLine("Reading from event hub...");
            do
            {
                var tasks = new List<Task<IEnumerable<EventData>>>
                {
                    receiver.ReceiveAsync(200),
                    receiver2.ReceiveAsync(200)
                };

                var messages = Task.WhenAll(tasks)
                                   .GetAwaiter()
                                   .GetResult();

                if (messages != null)
                {
                    foreach (var item in messages.Where(x => x != null).SelectMany(x => x))
                    {
                        OnMessageReceived(item);
                        messageCount++;
                    }
                }

                Console.WriteLine($"MessageCount : {messageCount}");
                currentRetries = messages.Any(x => x != null) ? 0 : currentRetries + 1;
            }
            while (retries > currentRetries);

            receiver.Close();
            receiver2.Close();
        }

        private void OnMessageReceived(EventData item)
        {
            MessageReceived?.Invoke(this, new EventDataEventArgs(item));
        }
    }
}