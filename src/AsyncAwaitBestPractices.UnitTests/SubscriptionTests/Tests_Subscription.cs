using NUnit.Framework;
using System;
using System.Reflection;

namespace AsyncAwaitBestPractices.UnitTests;

public class SubscriptionTests
{
	[Test]
	public void Subscription_SubscriberAndHandlerNotNull()
	{
		// Arrange
		var weakReference = new WeakReference(new object());
		var methodInfo = typeof(SubscriptionTests).GetMethod(nameof(SampleMethod));

		// Act
		var subscription = new Subscription(weakReference, methodInfo);

		// Assert
		Assert.IsNotNull(subscription.Subscriber);
		Assert.IsNotNull(subscription.Handler);
	}

	[Test]
	public void Subscription_HandlerNotNull_ThrowsArgumentNullException()
	{
		// Arrange
		var weakReference = new WeakReference(new object());
		MethodInfo? methodInfo = null;

		// Act & Assert
#pragma warning disable CS8604 // Possible null reference argument.
		Assert.Throws<ArgumentNullException>(() => new Subscription(weakReference, methodInfo));
#pragma warning restore CS8604 // Possible null reference argument.
	}

	private void SampleMethod()
	{
		// Sample method used for testing purposes
	}
}
