# Community.NServiceBus.LambdaHandlers

This community package provides support for delegate based message handlers for NServiceBus.

## Usage

use the `Handle<TMessageType>` extension method on the `EndpointConfiguration` class to register the delegate to be invoked for a specific message type.

Note that currently, messages need to implement the NServiceBus marker interfaces (`IMessage`, `ICommand` or `IEvent`).

```csharp
endpointConfiguration.Handle<MyMessage>((message, context) =>
{
    Console.WriteLine("Hello from lambda");
    return Task.CompletedTask;
});
```

See the demo project in this repository for more examples.

The endpoint will automatically subscribe to events if there is a delegate registered for an event type (unless auto-subscribe has been explicitly disabled).