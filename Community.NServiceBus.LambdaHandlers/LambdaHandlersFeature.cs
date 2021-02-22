using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Unicast;

namespace Community.NServiceBus.LambdaHandlers
{
    using Microsoft.Extensions.DependencyInjection;

    public class LambdaHandlersFeature : Feature
    {
        public LambdaHandlersFeature()
        {
            EnableByDefault();
            DependsOn("NServiceBus.Features.RegisterHandlersInOrder");
            // register a registry so it will be reused by the RegisterHandlersInOrder feature in core
            Defaults(s => s.Set(new MessageHandlerRegistry()));
        }

        protected override void Setup(FeatureConfigurationContext context)
        {
            var registry = context.Settings.Get<MessageHandlerRegistry>();
            var lambdaHandlers = context.Settings.Get<List<LambdaHandler>>();

            // Dictionary<Type, List<DelegateHolder>>
            var handlersField = registry.GetType().GetField("handlerAndMessagesHandledByHandlerCache", BindingFlags.NonPublic | BindingFlags.Instance);
            var handlers = handlersField.GetValue(registry);
            var collectionType = handlers.GetType();
            
            // List<DelegateHolder>
            var delegateHolderCollectionType = collectionType.GenericTypeArguments[1];
            var delegateHolderCollection = Activator.CreateInstance(delegateHolderCollectionType);


            // DelegateHolder (private)
            var delegateHolderType = delegateHolderCollectionType.GenericTypeArguments[0];

            foreach (var handler in lambdaHandlers)
            {
                var delegateHolderInstance = Activator.CreateInstance(delegateHolderType);

                delegateHolderType.GetField("MessageType").SetValue(delegateHolderInstance, handler.MessageType);
                delegateHolderType.GetField("MethodDelegate").SetValue(delegateHolderInstance, handler.Delegate);

                delegateHolderCollectionType.GetMethod("Add").Invoke(delegateHolderCollection, new[] { delegateHolderInstance });
            }
            
            collectionType.GetMethod("Add").Invoke(handlers, new[] { typeof(LambdaFunctionsDummyInstance), delegateHolderCollection });

            // Register instance of dummy type which will be resolved as part of the handler invocation pipeline
            context.Services.AddSingleton(new LambdaFunctionsDummyInstance());
        }

        class LambdaFunctionsDummyInstance
        {
        }

        internal class LambdaHandler
        {
            public Type MessageType { get; protected set; }
            public Func<object, object, IMessageHandlerContext, Task> Delegate { get; protected set; }
        }
    }
}