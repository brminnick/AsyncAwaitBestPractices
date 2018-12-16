using System;
using System.Collections.Generic;
using System.Reflection;

using static System.String;
using System.Runtime.CompilerServices;

namespace AsyncAwaitBestPractices
{
    /// <summary>
    /// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
    /// </summary>
    public class WeakEventManager
    {
        readonly Dictionary<string, List<Subscription>> _eventHandlers =
            new Dictionary<string, List<Subscription>>();

        /// <summary>
        /// Adds the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        /// <typeparam name="TEventArgs">EventHandler type</typeparam>
        public void AddEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
        {
            if (IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Adds the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void AddEventHandler(EventHandler handler, [CallerMemberName] string eventName = "")
        {
            if (IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            AddEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Executes the event
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="args">Arguments</param>
        /// <param name="eventName">Event name</param>
        public void HandleEvent(object sender, object args, string eventName)
        {
            var toRaise = new List<Tuple<object, MethodInfo>>();
            var toRemove = new List<Subscription>();

            if (_eventHandlers.TryGetValue(eventName, out List<Subscription> target))
            {
                for (int i = 0; i < target.Count; i++)
                {
                    Subscription subscription = target[i];
                    bool isStatic = subscription.Subscriber == null;
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
                tuple.Item2.Invoke(tuple.Item1, new[] { sender, args });
            }
        }

        /// <summary>
        /// Removes the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        /// <typeparam name="TEventArgs">EventHandler type</typeparam>
        public void RemoveEventHandler<TEventArgs>(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "")
            where TEventArgs : EventArgs
        {
            if (IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void RemoveEventHandler(EventHandler handler, [CallerMemberName] string eventName = "")
        {
            if (IsNullOrEmpty(eventName))
                throw new ArgumentNullException(nameof(eventName));

            if (handler is null)
                throw new ArgumentNullException(nameof(handler));

            RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo());
        }

        void AddEventHandler(string eventName, object handlerTarget, MethodInfo methodInfo)
        {
            var doesContainSubscriptions = _eventHandlers.TryGetValue(eventName, out List<Subscription> targets);
            if (!doesContainSubscriptions)
            {
                targets = new List<Subscription>();
                _eventHandlers.Add(eventName, targets);
            }

            if (handlerTarget == null)
            {
                // This event handler is a static method
                targets.Add(new Subscription(null, methodInfo));
                return;
            }

            targets.Add(new Subscription(new WeakReference(handlerTarget), methodInfo));
        }

        void RemoveEventHandler(string eventName, object handlerTarget, MemberInfo methodInfo)
        {
            var doesContainSubscriptions = _eventHandlers.TryGetValue(eventName, out List<Subscription> subscriptions);
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

        struct Subscription
        {
            public Subscription(WeakReference subscriber, MethodInfo handler)
            {
                Subscriber = subscriber;
                Handler = handler ?? throw new ArgumentNullException(nameof(handler));
            }

            public WeakReference Subscriber { get; }
            public MethodInfo Handler { get; }
        }
    }
}