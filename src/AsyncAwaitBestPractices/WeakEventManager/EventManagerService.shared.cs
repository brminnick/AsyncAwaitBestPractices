using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace AsyncAwaitBestPractices;

static class EventManagerService
{
#if NET9_0_OR_GREATER
    static readonly System.Threading.Lock _lock = new();
#else
	static readonly object _lock = new();
#endif

	internal static void AddEventHandler(in string eventName, in object? handlerTarget, in MethodInfo methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
	{
		lock (_lock)
		{
			var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out var targets);
			if (!doesContainSubscriptions || targets is null)
			{
				targets = [];
				eventHandlers.Add(eventName, targets);
			}

			if (handlerTarget is null)
				targets.Add(new Subscription(null, methodInfo));
			else
				targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
		}
	}

	internal static void RemoveEventHandler(in string eventName, in object? handlerTarget, in MemberInfo methodInfo, in Dictionary<string, List<Subscription>> eventHandlers)
	{
		lock (_lock)
		{
			var doesContainSubscriptions = eventHandlers.TryGetValue(eventName, out var subscriptions);
			if (!doesContainSubscriptions || subscriptions is null)
				return;

			for (var n = subscriptions.Count - 1; n >= 0; n--)
			{
				var current = subscriptions[n];

				if (current.Subscriber?.Target != handlerTarget
					|| current.Handler.Name != methodInfo?.Name)
				{
					continue;
				}

				subscriptions.RemoveAt(n);
				break;
			}
		}
	}

	internal static void HandleEvent(in string eventName, in object? sender, in object? eventArgs, in Dictionary<string, List<Subscription>> eventHandlers)
	{
		AddRemoveEvents(eventName, eventHandlers, out var toRaise);

		for (var i = 0; i < toRaise.Count; i++)
		{
			try
			{
				var (instance, eventHandler) = toRaise[i];

#if NET5_0_OR_GREATER
				if (eventHandler is DynamicMethod dynamicMethod)
				{
					dynamicMethod.Invoke(instance, [sender, eventArgs]);
				}
				else
				{
					eventHandler.Invoke(instance, [sender, eventArgs]);
				}
#else
				if (eventHandler.IsLightweightMethod())
				{
					var method = TryGetDynamicMethod(eventHandler);
					method?.Invoke(instance, [sender, eventArgs]);
				}
				else
				{
					eventHandler.Invoke(instance, [sender, eventArgs]);
				}
#endif
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

		for (var i = 0; i < toRaise.Count; i++)
		{
			try
			{
				var (instance, eventHandler) = toRaise[i];

#if NET5_0_OR_GREATER
				if (eventHandler is DynamicMethod dynamicMethod)
				{
					dynamicMethod.Invoke(instance, [actionEventArgs]);
				}
				else
				{
					eventHandler.Invoke(instance, [actionEventArgs]);
				}
#else
				if (eventHandler.IsLightweightMethod())
				{
					var method = TryGetDynamicMethod(eventHandler);
					method?.Invoke(instance, [actionEventArgs]);
				}
				else
				{
					eventHandler.Invoke(instance, [actionEventArgs]);
				}
#endif
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

		for (var i = 0; i < toRaise.Count; i++)
		{
			try
			{
				var (instance, eventHandler) = toRaise[i];
				if (eventHandler is DynamicMethod dynamicMethod)
				{
					dynamicMethod.Invoke(instance, null);
				}
				else
				{
					eventHandler.Invoke(instance, null);
				}
			}
			catch (TargetParameterCountException e)
			{
				throw new InvalidHandleEventException("Parameter count mismatch. If invoking an `event EventHandler` use `HandleEvent(object sender, TEventArgs eventArgs, string eventName)` or if invoking an `event Action<T>` use `HandleEvent(object eventArgs, string eventName)`instead.", e);
			}
		}
	}

	static void AddRemoveEvents(in string eventName, in Dictionary<string, List<Subscription>> eventHandlers, out List<(object? Instance, MethodInfo EventHandler)> toRaise)
	{
		var toRemove = new List<Subscription>();
		toRaise = [];

		var doesContainEventName = eventHandlers.TryGetValue(eventName, out var target);
		if (doesContainEventName && target is not null)
		{
			for (var i = 0; i < target.Count; i++)
			{
				var subscription = target[i];
				var isStatic = subscription.Subscriber is null;

				if (isStatic)
				{
					toRaise.Add((null, subscription.Handler));
					continue;
				}

				var subscriber = subscription.Subscriber?.Target;

				if (subscriber is null)
					toRemove.Add(subscription);
				else
					toRaise.Add((subscriber, subscription.Handler));
			}

			foreach (var subscription in toRemove)
			{
				target.Remove(subscription);
			}
		}
	}

#if !NET5_0_OR_GREATER
	static DynamicMethod? TryGetDynamicMethod(in MethodInfo rtDynamicMethod)
	{
		var typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType("RTDynamicMethod");
		var typeRTDynamicMethod = typeInfoRTDynamicMethod?.AsType();

		return (typeInfoRTDynamicMethod?.IsAssignableFrom(rtDynamicMethod.GetType().GetTypeInfo()) ?? false) ?
			 (DynamicMethod?)typeRTDynamicMethod?.GetRuntimeFields().First(f => f.Name is "m_owner").GetValue(rtDynamicMethod)
			: null;
	}

	static bool IsLightweightMethod(this MethodBase method)
	{
		var typeInfoRTDynamicMethod = typeof(DynamicMethod).GetTypeInfo().GetDeclaredNestedType("RTDynamicMethod");
		return method is DynamicMethod || (typeInfoRTDynamicMethod?.IsAssignableFrom(method.GetType().GetTypeInfo()) ?? false);
	}
#endif
}