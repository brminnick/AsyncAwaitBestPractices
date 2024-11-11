using System;
using System.Reflection;

namespace AsyncAwaitBestPractices;

readonly struct Subscription(in WeakReference? subscriber, in MethodInfo handler)
{
	public WeakReference? Subscriber { get; } = subscriber;
	public MethodInfo Handler { get; } = handler ?? throw new ArgumentNullException(nameof(handler));
}