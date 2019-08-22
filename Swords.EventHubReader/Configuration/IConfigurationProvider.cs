namespace Swords.EventHubReader.Configuration
{
    public interface IConfigurationProvider
    {
        TValue GetValue<TValue>(string key, TValue defaultValue);
    }
}
