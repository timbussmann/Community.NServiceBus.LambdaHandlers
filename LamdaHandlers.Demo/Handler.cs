namespace LamdaHandlers.Demo
{
    using System;
    using System.Threading.Tasks;
    using NServiceBus;

    class Handler : IHandleMessages<MyMessage>
    {
        public Task Handle(MyMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine("Hello from an ordinary message handler");
            return Task.CompletedTask;
        }
    }
}