using System;
using System.Threading.Tasks;
using Community.NServiceBus.LambdaHandlers;
using NServiceBus;

namespace LamdaHandlers.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpointConfiguration = new EndpointConfiguration("lambdahandler");

            endpointConfiguration.UseTransport<LearningTransport>();

            endpointConfiguration.Handle<MyMessage>((message, context) =>
            {
                Console.WriteLine("Hello from lambda1");
                return Task.CompletedTask;
            });

            endpointConfiguration.Handle<MyMessage>((message, context) =>
            {
                Console.WriteLine("Hello from lambda2");
                return Task.CompletedTask;
            });

            endpointConfiguration.Handle<MyEvent>((message, context) =>
            {
                Console.WriteLine("Hello from event handler");
                return Task.CompletedTask;
            });

            var endpoint = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("Press [1] to send a command, [2] to publish an event or [Esc] to exit.");
            while (true)
            {
                var key = Console.ReadKey();
                Console.WriteLine();
                if (key.Key == ConsoleKey.Escape)
                {
                    await endpoint.Stop();
                    return;
                }

                if (key.KeyChar == '1')
                {
                    await endpoint.SendLocal(new MyMessage());
                }

                if (key.KeyChar == '2')
                {
                    await endpoint.Publish(new MyEvent());
                }
            }
        }
    }

    class MyMessage : ICommand
    {
    }

    class MyEvent : IEvent
    {
    }
}
