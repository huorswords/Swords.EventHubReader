using System.ComponentModel;
using System.Configuration;
using System.Linq;

namespace Swords.EventHubReader.Configuration
{
    internal sealed class AppConfigurationProvider : IConfigurationProvider
    {
        public TValue GetValue<TValue>(string key, TValue defaultValue)
        {
            var configurationValue = defaultValue;
            if (ConfigurationManager.AppSettings.Keys.Cast<string>().Any(x => x.Equals(key, System.StringComparison.InvariantCultureIgnoreCase)))
            {
                var converter = TypeDescriptor.GetConverter(typeof(TValue));
                configurationValue = (TValue)(converter.ConvertFromInvariantString(ConfigurationManager.AppSettings[key]));
            }

            return configurationValue;
        }
    }
}