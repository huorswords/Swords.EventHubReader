using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Swords.EventHubReader.Configuration;
using Swords.EventHubReader.Contracts;

namespace Swords.EventHubReader
{
    class Program
    {
        private const string ConnectionStringKey = "ConnectionString";
        private const string EntityPathKey = "EntityPath";
        private const string ConsumerGroupKey = "ConsumerGroup";
        private const string HandlersKey = "Handlers";
        private const string HandlersFolderName = "ExternalHandlers";

        static void Main()
        {
            IConfigurationProvider configurationProvider = new AppConfigurationProvider();

            var connectionString = configurationProvider.GetValue(ConnectionStringKey, string.Empty);
            var entityPath = configurationProvider.GetValue(EntityPathKey, string.Empty);
            var consumerGroup = configurationProvider.GetValue(ConsumerGroupKey, string.Empty);
            var handlers = configurationProvider.GetValue(HandlersKey, string.Empty).Split(',');
            
            IEventHubReader reader = new EventHubReader(connectionString, entityPath, consumerGroup);
            SubscribeHandlers(Assembly.GetExecutingAssembly(), reader);
            SubscribeExternalHandlers(reader);
            reader.Read();
        }

        private static void SubscribeExternalHandlers(IEventHubReader reader)
        {
            string handlersPath = Path.Combine(Directory.GetCurrentDirectory(), HandlersFolderName);
            if (Directory.Exists(handlersPath))
            {
                var files = Directory.GetFiles(handlersPath);
                foreach (var file in files)
                {
                    var assembly = Assembly.LoadFile(file);
                    SubscribeHandlers(assembly, reader);
                }
            }
        }

        private static void SubscribeHandlers(Assembly assembly, IEventHubReader reader)
        {
            var handlerTypes = assembly.GetTypes()
                                       .Where(x => typeof(IEventDataHandler).IsAssignableFrom(x) && !x.IsInterface);
            foreach (var type in handlerTypes)
            {
                SubscribeHandler(type, reader);
            }
        }

        private static void SubscribeHandler(Type type, IEventHubReader reader)
        {
            var handler = Activator.CreateInstance(type) as IEventDataHandler;
            handler.Subscribe(reader);
        }
    }
}