using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
#endif

using static System.String;

namespace AsyncAwaitBestPractices;

/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
/// <typeparam name="TEventArgs">Event args type.</typeparam>
public partial class WeakEventManager<TEventArgs>
{
	readonly Dictionary<string, List<Subscription>> _eventHandlers = new();

	/// <summary>
	/// Adds the event handler
	/// </summary>
	/// <param name="handler">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void AddEventHandler([NotNull] in EventHandler<TEventArgs>? handler, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void AddEventHandler(in EventHandler<TEventArgs>? handler, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (handler is null)
			throw new ArgumentNullException(nameof(handler));

		EventManagerService.AddEventHandler(eventName, handler.Target, handler.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Adds the event handler
	/// </summary>
	/// <param name="action">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void AddEventHandler([NotNull] in Action<TEventArgs>? action, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void AddEventHandler(in Action<TEventArgs>? action, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (action is null)
			throw new ArgumentNullException(nameof(action));

		EventManagerService.AddEventHandler(eventName, action.Target, action.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Removes the event handler
	/// </summary>
	/// <param name="handler">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void RemoveEventHandler([NotNull] in EventHandler<TEventArgs>? handler, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void RemoveEventHandler(in EventHandler<TEventArgs>? handler, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (handler is null)
			throw new ArgumentNullException(nameof(handler));

		EventManagerService.RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Removes the event handler
	/// </summary>
	/// <param name="action">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void RemoveEventHandler([NotNull] in Action<TEventArgs>? action, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void RemoveEventHandler(in Action<TEventArgs>? action, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (action is null)
			throw new ArgumentNullException(nameof(action));

		EventManagerService.RemoveEventHandler(eventName, action.Target, action.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Invokes the event EventHandler
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="eventArgs">Event arguments</param>
	/// <param name="eventName">Event name</param>
	public void RaiseEvent(in object? sender, in TEventArgs eventArgs, in string eventName) =>
		EventManagerService.HandleEvent(eventName, sender, eventArgs, _eventHandlers);

	/// <summary>
	/// Invokes the event Action
	/// </summary>
	/// <param name="eventArgs">Event arguments</param>
	/// <param name="eventName">Event name</param>
	public void RaiseEvent(in TEventArgs eventArgs, in string eventName) =>
		EventManagerService.HandleEvent(eventName, eventArgs, _eventHandlers);
}

/// <summary>
/// Weak event manager that allows for garbage collection when the EventHandler is still subscribed
/// </summary>
public partial class WeakEventManager
{
	readonly Dictionary<string, List<Subscription>> _eventHandlers = new();

	/// <summary>
	/// Adds the event handler
	/// </summary>
	/// <param name="handler">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void AddEventHandler([NotNull] in Delegate? handler, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void AddEventHandler(in Delegate? handler, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (handler is null)
			throw new ArgumentNullException(nameof(handler));

		EventManagerService.AddEventHandler(eventName, handler.Target, handler.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Removes the event handler.
	/// </summary>
	/// <param name="handler">Handler</param>
	/// <param name="eventName">Event name</param>
#if NETSTANDARD2_1_OR_GREATER || NET5_0_OR_GREATER
	public void RemoveEventHandler([NotNull] in Delegate? handler, [CallerMemberName, NotNull] in string eventName = "")
#else
	public void RemoveEventHandler(in Delegate? handler, [CallerMemberName] in string eventName = "")
#endif
	{
		if (IsNullOrWhiteSpace(eventName))
			throw new ArgumentNullException(nameof(eventName));

		if (handler is null)
			throw new ArgumentNullException(nameof(handler));

		EventManagerService.RemoveEventHandler(eventName, handler.Target, handler.GetMethodInfo(), _eventHandlers);
	}

	/// <summary>
	/// Invokes the event EventHandler
	/// </summary>
	/// <param name="sender">Sender</param>
	/// <param name="eventArgs">Event arguments</param>
	/// <param name="eventName">Event name</param>
	public void RaiseEvent(in object? sender, in object? eventArgs, in string eventName) =>
		EventManagerService.HandleEvent(eventName, sender, eventArgs, _eventHandlers);

	/// <summary>
	/// Invokes the event Action
	/// </summary>
	/// <param name="eventName">Event name</param>
	public void RaiseEvent(in string eventName) => EventManagerService.HandleEvent(eventName, _eventHandlers);
}