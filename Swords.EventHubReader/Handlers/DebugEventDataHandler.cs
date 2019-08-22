using System;
using System.Diagnostics;
using System.Text;

using Microsoft.Azure.EventHubs;

using Swords.EventHubReader.Contracts;

namespace Swords.EventHubReader.Handlers
{
    public class DebugEventDataHandler : IEventDataHandler
    {
        public void Subscribe(IEventHubReader reader)
        {
            reader.MessageReceived += Reader_MessageReceived;
        }

        public void Unsubscribe(IEventHubReader reader)
        {
            reader.MessageReceived -= Reader_MessageReceived;
        }

        private void Reader_MessageReceived(object sender, EventDataEventArgs e)
        {
            HandleEvent(e.Data);
        }

        private void HandleEvent(EventData @event)
        {
            string content = Encoding.UTF8.GetString(@event.Body.Array);
            Console.WriteLine(content);
            Debug.WriteLine(content);
        }
    }
}