namespace Swords.EventHubReader.Contracts
{
    public interface IEventDataHandler
    {
        void Subscribe(IEventHubReader reader);

        void Unsubscribe(IEventHubReader reader);
    }
}