using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Configuration.AdvancedExtensibility;
using NServiceBus.Unicast;

namespace Community.NServiceBus.LambdaHandlers
{
    public static class HandlerExtensions
    {
        public static void Handle<TMessage>(this EndpointConfiguration configuration,
            Func<TMessage, IMessageHandlerContext, Task> handler)
        {
            var handlers = configuration.GetSettings().GetOrCreate<List<LambdaHandlersFeature.LambdaHandler>>();
            handlers.Add(new TypedLambdaHandler<TMessage>(handler));
        }

        class TypedLambdaHandler<TMessage> : LambdaHandlersFeature.LambdaHandler
        {
            public TypedLambdaHandler(Func<TMessage, IMessageHandlerContext, Task> handler)
            {
                MessageType = typeof(TMessage);
                Delegate = (_, message, context) => handler((TMessage)message, context);
            }
        }
    }
}