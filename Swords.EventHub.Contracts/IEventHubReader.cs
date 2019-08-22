using System;

namespace Swords.EventHubReader.Contracts
{
    public interface IEventHubReader
    {
        event EventHandler<EventDataEventArgs> MessageReceived;

        void Read();
    }
}