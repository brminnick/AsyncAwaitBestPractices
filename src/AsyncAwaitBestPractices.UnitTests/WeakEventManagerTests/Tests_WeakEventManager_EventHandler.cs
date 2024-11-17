using System;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_EventHandler : BaseTest
{
	[Test]
	public void WeakEventManager_HandleEvent_ValidImplementation()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(this, new EventArgs(), nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, EventArgs e)
		{
			if (sender is null)
				throw new ArgumentNullException(nameof(sender));

			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			TestEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManager_HandleEvent_NullSender()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(null, new EventArgs(), nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, EventArgs e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Null);
				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			TestEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManager_HandleEvent_EmptyEventArgs()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, EventArgs e)
		{
			if (sender is null)
				throw new ArgumentNullException(nameof(sender));

			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
				Assert.That(e, Is.EqualTo(EventArgs.Empty));
			});


			didEventFire = true;
			TestEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManager_HandleEvent_NullEventArgs()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(this, null, nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, EventArgs e)
		{
			if (sender is null)
				throw new ArgumentNullException(nameof(sender));

			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Null);
			});

			didEventFire = true;
			TestEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManager_HandleEvent_InvalidHandleEventName()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(this, new EventArgs(), nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		TestEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, EventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManager_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		TestEvent += HandleTestEvent;
		TestEvent -= HandleTestEvent;

		//Act
		TestWeakEventManager.RaiseEvent(null, null, nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.False);

		void HandleTestEvent(object? sender, EventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManager_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager();
		bool didEventFire = false;

		TestEvent += HandleTestEvent;

		//Act
		unassignedEventManager.RaiseEvent(null, null, nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		TestEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, EventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManager_AddEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.AddEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManager_AddEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManager_AddEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManager_AddEventHandler_WhitespaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.AddEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.RemoveEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.RemoveEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManager_RemoveEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestWeakEventManager.RemoveEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManager_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act

		//Assert
		Assert.Multiple(() =>
		{
			Assert.Throws<InvalidHandleEventException>(() => TestWeakEventManager.RaiseEvent(nameof(TestEvent)));
			Assert.That(didEventFire, Is.False);
		});

		TestEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, EventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManager_RaiseEvent_WithParameters()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestWeakEventManager.RaiseEvent(this, new EventArgs(), nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, EventArgs e)
		{
			if (sender is null)
				throw new ArgumentNullException(nameof(sender));

			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			TestEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManager_ExceptionHandling()
	{
		//Arrange
		TestEvent += HandleTestEvent;
		Exception? caughtException = null;

		//Act
		try
		{
			TestWeakEventManager.RaiseEvent(this, new EventArgs(), nameof(TestEvent));
		}
		catch (Exception ex)
		{
			caughtException = ex;
		}

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		TestEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, EventArgs e) => throw new NullReferenceException();
	}
}