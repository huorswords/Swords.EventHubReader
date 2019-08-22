using System;

using Microsoft.Azure.EventHubs;

namespace Swords.EventHubReader.Contracts
{
    public class EventDataEventArgs : EventArgs
    {
        public EventDataEventArgs(EventData item)
        {
            Data = item;
        }

        public EventData Data { get; }
    }
}