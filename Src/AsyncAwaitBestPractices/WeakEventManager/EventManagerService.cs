using System;
using System.Collections.Generic;
using System.Reflection;

namespace AsyncAwaitBestPractices
{
    static class EventManagerService
    {
        internal static void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out List<Subscription> targets);
            if (!doesContainSubscriptions)
            {
                targets = new List<Subscription>();
                eventHandlers.Add(eventName, targets);
            }

            if (handlerTarget is null)
                targets.Add(new Subscription(null, methodInfo));
            else
                targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
        }

        internal static void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out List<Subscription> subscriptions);
            if (!doesContainSubscriptions)
                return;

            for (int n = subscriptions.Count; n > 0; n--)
            {
                Subscription current = subscriptions[n - 1];

                if (current.Subscriber?.Target != handlerTarget
                    || current.Handler.Name != methodInfo.Name)
                {
                    continue;
                }

                subscriptions.Remove(current);
                break;
            }
        }

        internal static void HandleEvent(string eventName, object sender, object eventArgs, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            var toRaise = new List<Tuple<object, MethodInfo>>();
            var toRemove = new List<Subscription>();

            if (eventHandlers.TryGetValue(eventName, out List<Subscription> target))
            {
                for (int i = 0; i < target.Count; i++)
                {
                    Subscription subscription = target[i];
                    bool isStatic = subscription.Subscriber is null;
                    if (isStatic)
                    {
                        toRaise.Add(Tuple.Create<object, MethodInfo>(null, subscription.Handler));
                        continue;
                    }

                    object subscriber = subscription.Subscriber.Target;

                    if (subscriber is null)
                        toRemove.Add(subscription);
                    else
                        toRaise.Add(Tuple.Create(subscriber, subscription.Handler));
                }

                for (int i = 0; i < toRemove.Count; i++)
                {
                    Subscription subscription = toRemove[i];
                    target.Remove(subscription);
                }
            }

            for (int i = 0; i < toRaise.Count; i++)
            {
                Tuple<object, MethodInfo> tuple = toRaise[i];
                tuple.Item2.Invoke(tuple.Item1, new[] { sender, eventArgs });
            }
        }
    }
}
