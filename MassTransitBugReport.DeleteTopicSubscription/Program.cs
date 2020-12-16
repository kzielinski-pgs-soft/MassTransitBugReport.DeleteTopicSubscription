namespace MassTransitBugReport.DeleteTopicSubscription
{
    using Contracts;
    using MassTransit;
    using MassTransit.Context;
    using Microsoft.Azure.ServiceBus.Primitives;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Threading.Tasks;

    public class Message1Consumer : IConsumer<IMessage1>
    {
        public Task Consume(ConsumeContext<IMessage1> context)
        {
            return Task.CompletedTask;
        }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(loggingBuilder =>
            {
                loggingBuilder
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("System", LogLevel.Warning)
                    .SetMinimumLevel(LogLevel.Debug)
                    .AddConsole(options => options.TimestampFormat = "hh:mm:ss ");
            });

            LogContext.ConfigureCurrentLogContext(loggerFactory);

            var busControl = Bus.Factory.CreateUsingAzureServiceBus(configurator =>
            {
                configurator.Host(new Uri("addres"),
                    h =>
                        h.SharedAccessSignature(s =>
                        {
                            s.KeyName = "KeyName";
                            s.SharedAccessKey = "SharedAccessKey";
                            s.TokenTimeToLive = TimeSpan.FromMinutes(30);
                            s.TokenScope = TokenScope.Namespace;
                        }));

                configurator.ReceiveEndpoint(endpointConfigurator =>
                {
                    endpointConfigurator.Consumer<Message1Consumer>();
                });
            });

            await busControl.StartAsync();

            Console.WriteLine("Press any key to exit");
            Console.ReadKey();

            await busControl.StopAsync();
        }
    }
}

namespace MassTransitBugReport.DeleteTopicSubscription.Contracts
{
    public interface IMessage1
    {
    }
}