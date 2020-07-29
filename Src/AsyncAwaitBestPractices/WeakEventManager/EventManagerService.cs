using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AsyncAwaitBestPractices
{
    static class EventManagerService
    {
        internal static void AddEventHandler(in string eventName, in object? handlerTarget, in MethodInfo? methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out List<Subscription>? targets);
            if (!doesContainSubscriptions || targets is null)
            {
                targets = new List<Subscription>();
                eventHandlers.Add(eventName, targets);
            }

            if (handlerTarget is null)
                targets.Add(new Subscription(null, methodInfo));
            else
                targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
        }

        internal static void RemoveEventHandler(in string eventName, in object? handlerTarget, in MemberInfo? methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out List<Subscription>? subscriptions);
            if (!doesContainSubscriptions || subscriptions is null)
                return;

            for (int n = subscriptions.Count; n > 0; n--)
            {
                Subscription current = subscriptions[n - 1];

                if (current.Subscriber?.Target != handlerTarget
                    || current.Handler.Name != methodInfo?.Name)
                {
                    continue;
                }

                subscriptions.Remove(current);
                break;
            }
        }

        internal static void HandleEvent(in string eventName, in object? sender, in object? eventArgs, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            AddRemoveEvents(eventName, eventHandlers, out var toRaise);

            for (int i = 0; i < toRaise.Count; i++)
            {
                try
                {
                    Tuple<object?, MethodInfo> tuple = toRaise[i];
                    if (tuple.Item2.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(tuple.Item2);
                        method?.Invoke(tuple.Item1, new[] { sender, eventArgs });
                    }
                    else
                    {
                        tuple.Item2.Invoke(tuple.Item1, new[] { sender, eventArgs });
                    }
                }
                catch (TargetParameterCountException e)
                {
                    throw new InvalidHandleEventException("Parameter count mismatch. If invoking an `event Action` use `HandleEvent(string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.", e);
                }
            }
        }

        internal static void HandleEvent(in string eventName, in object? actionEventArgs, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            AddRemoveEvents(eventName, eventHandlers, out var toRaise);

            for (int i = 0; i < toRaise.Count; i++)
            {
                try
                {
                    Tuple<object?, MethodInfo> tuple = toRaise[i];
                    if (tuple.Item2.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(tuple.Item2);
                        method?.Invoke(tuple.Item1, new[] { actionEventArgs });
                    }
                    else
                    {
                        tuple.Item2.Invoke(tuple.Item1, new[] { actionEventArgs });
                    }
                }
                catch (TargetParameterCountException e)
                {
                    throw new InvalidHandleEventException("Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action` use `HandleEvent(string eventName)`instead.", e);
                }
            }
        }

        internal static void HandleEvent(in string eventName, in Dictionary<string, List<Subscription>> eventHandlers)
        {
            AddRemoveEvents(eventName, eventHandlers, out var toRaise);

            for (int i = 0; i < toRaise.Count; i++)
            {
                try
                {
                    Tuple<object?, MethodInfo> tuple = toRaise[i];
                    if (tuple.Item2.IsLightweightMethod())
                    {
                        var method = TryGetDynamicMethod(tuple.Item2);
                        method?.Invoke(tuple.Item1, null);
                    }
                    else
                    {
                        tuple.Item2.Invoke(tuple.Item1, null);
                    }
                }
                catch (TargetParameterCountException e)
                {
                    throw new InvalidHandleEventException("Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.", e);
                }
            }
        }

        static void AddRemoveEvents(in string eventName, in Dictionary<string, List<Subscription>> eventHandlers, out List<Tuple<object?, MethodInfo>> toRaise)
        {
            var toRemove = new List<Subscription>();
            toRaise = new List<Tuple<object?, MethodInfo>>();

            var doesContainEventName = eventHandlers.TryGetValue(eventName, out List<Subscription>? target);
            if (doesContainEventName && target != null)
            {
                for (int i = 0; i < target.Count; i++)
                {
                    Subscription subscription = target[i];
                    bool isStatic = subscription.Subscriber is null;
                    if (isStatic)
                    {
                        toRaise.Add(Tuple.Create<object?, MethodInfo>(null, subscription.Handler));
                        continue;
                    }

                    object? subscriber = subscription.Subscriber?.Target;

                    if (subscriber is null)
                        toRemove.Add(subscription);
                    else
                        toRaise.Add(Tuple.Create<object?, MethodInfo>(subscriber, subscription.Handler));
                }

                for (int i = 0; i < toRemove.Count; i++)
                {
                    Subscription subscription = toRemove[i];
                    target.Remove(subscription);
                }
            }
        }

        static DynamicMethod? TryGetDynamicMethod(MethodInfo rtDynamicMethod)
        {
            var typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType("RTDynamicMethod");
            var typeRTDynamicMethod = typeInfoRTDynamicMethod.AsType();

            return typeInfoRTDynamicMethod.IsAssignableFrom(rtDynamicMethod.GetType().GetTypeInfo())
                ? (DynamicMethod)typeRTDynamicMethod.GetRuntimeFields().First(f => f.Name is "m_owner").GetValue(rtDynamicMethod)
                : null;
        }

        static bool IsLightweightMethod(this MethodBase method)
        {
            var typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType("RTDynamicMethod");
            return method is DynamicMethod || typeInfoRTDynamicMethod.IsAssignableFrom(method.GetType().GetTypeInfo());
        }
    }
}
