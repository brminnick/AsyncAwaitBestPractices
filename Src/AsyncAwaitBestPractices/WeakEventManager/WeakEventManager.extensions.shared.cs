using System;
using System.Runtime.CompilerServices;

namespace AsyncAwaitBestPractices
{
    public partial class WeakEventManager<TEventArgs>
    {
        /// <summary>
        /// Adds the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void AddEventHandler(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "") => AddEventHandler(in handler, in eventName);

        /// <summary>
        /// Adds the event handler
        /// </summary>
        /// <param name="action">Handler</param>
        /// <param name="eventName">Event name</param>
        public void AddEventHandler(Action<TEventArgs> action, [CallerMemberName] string eventName = "") => AddEventHandler(in action, in eventName);

        /// <summary>
        /// Removes the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void RemoveEventHandler(EventHandler<TEventArgs> handler, [CallerMemberName] string eventName = "") => RemoveEventHandler(in handler, in eventName);

        /// <summary>
        /// Removes the event handler
        /// </summary>
        /// <param name="action">Handler</param>
        /// <param name="eventName">Event name</param>
        public void RemoveEventHandler(Action<TEventArgs> action, [CallerMemberName] string eventName = "") => RemoveEventHandler(in action, in eventName);

        /// <summary>
        /// Invokes the event EventHandler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="eventArgs">Event arguments</param>
        /// <param name="eventName">Event name</param>
        public void RaiseEvent(object? sender, TEventArgs eventArgs, string eventName) => RaiseEvent(in sender, in eventArgs, in eventName);

        /// <summary>
        /// Invokes the event Action
        /// </summary>
        /// <param name="eventArgs">Event arguments</param>
        /// <param name="eventName">Event name</param>
        public void RaiseEvent(TEventArgs eventArgs, string eventName) => RaiseEvent(in eventArgs, in eventName);
    }

    /// <summary>
    /// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
    /// </summary>
    public partial class WeakEventManager
    {
        /// <summary>
        /// Adds the event handler
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void AddEventHandler(Delegate handler, [CallerMemberName] string eventName = "") => AddEventHandler(in handler, in eventName);

        /// <summary>
        /// Removes the event handler.
        /// </summary>
        /// <param name="handler">Handler</param>
        /// <param name="eventName">Event name</param>
        public void RemoveEventHandler(Delegate handler, [CallerMemberName] string eventName = "") => RemoveEventHandler(in handler, in eventName);

        /// <summary>
        /// Invokes the event EventHandler
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="eventArgs">Event arguments</param>
        /// <param name="eventName">Event name</param>
        public void RaiseEvent(object? sender, object? eventArgs, string eventName) => RaiseEvent(in sender, in eventArgs, in eventName);

        /// <summary>
        /// Invokes the event Action
        /// </summary>
        /// <param name="eventName">Event name</param>
        public void RaiseEvent(string eventName) => RaiseEvent(in eventName);
    }
}