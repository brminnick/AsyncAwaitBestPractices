using System;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_EventHandlerT : BaseTest
{
	[Test]
	public void WeakEventManagerTEventArgs_HandleEvent_ValidImplementation()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;

		const string stringEventArg = "Test";
		bool didEventFire = false;

		//Act
		TestStringWeakEventManager.RaiseEvent(this, stringEventArg, nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, string? e)
		{
			if (sender is null || e is null)
				throw new ArgumentNullException(nameof(sender));

			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
				Assert.That(e, Is.EqualTo(stringEventArg));
			});

			didEventFire = true;
			TestStringEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManageTEventArgs_HandleEvent_NullSender()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;

		const string stringEventArg = "Test";

		bool didEventFire = false;

		//Act
		TestStringWeakEventManager.RaiseEvent(null, stringEventArg, nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, string e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Null);

				Assert.That(e, Is.Not.Null);
				Assert.That(e, Is.EqualTo(stringEventArg));
			});

			didEventFire = true;
			TestStringEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManagerTEventArgs_HandleEvent_NullEventArgs()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;
		bool didEventFire = false;

		void HandleTestEvent(object? sender, string e)
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
			TestStringEvent -= HandleTestEvent;
		}

		//Act
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		TestStringWeakEventManager.RaiseEvent(this, null, nameof(TestStringEvent));
#pragma warning restore CS8625

		//Assert
		Assert.That(didEventFire, Is.True);
	}

	[Test]
	public void WeakEventManagerTEventArgs_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;

		bool didEventFire = false;

		//Act
		TestStringWeakEventManager.RaiseEvent(this, "Test", nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		TestStringEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, string e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerTEventArgs_RaiseEvent_WithParameters()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;
		bool didEventFire = false;

		//Act
		TestStringWeakEventManager.RaiseEvent(this, "Test", nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleTestEvent(object? sender, string e)
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
			TestStringEvent -= HandleTestEvent;
		}
	}

	[Test]
	public void WeakEventManagerTEventArgs_ExceptionHandling()
	{
		//Arrange
		TestStringEvent += HandleTestEvent;
		Exception? caughtException = null;

		//Act
		try
		{
			TestStringWeakEventManager.RaiseEvent(this, "Test", nameof(TestStringEvent));
		}
		catch (Exception ex)
		{
			caughtException = ex;
		}

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		TestStringEvent -= HandleTestEvent;

		void HandleTestEvent(object? sender, string e) => throw new NullReferenceException();
	}

	[Test]
	public void WeakEventManager_NullEventManager()
	{
		//Arrange
		WeakEventManager? unassignedEventManager = null;

		//Act

		//Assert
#pragma warning disable CS8602 //Dereference of a possible null reference
		Assert.Throws<NullReferenceException>(() => unassignedEventManager.RaiseEvent(null, null, nameof(TestEvent)));
#pragma warning restore CS8602
	}

	[Test]
	public void WeakEventManagerTEventArgs_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager<string>();
		bool didEventFire = false;

		TestStringEvent += HandleTestEvent;
		void HandleTestEvent(object? sender, string e) => didEventFire = true;

		//Act
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		unassignedEventManager.RaiseEvent(null, null, nameof(TestStringEvent));
#pragma warning restore CS8625

		//Assert
		Assert.That(didEventFire, Is.False);
		TestStringEvent -= HandleTestEvent;
	}

	[Test]
	public void WeakEventManagerTEventArgs_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		TestStringEvent += HandleTestEvent;
		TestStringEvent -= HandleTestEvent;

		//Act
		TestStringWeakEventManager.RaiseEvent(this, "Test", nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.False);

		void HandleTestEvent(object? sender, string e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerT_AddEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler((EventHandler<string>?)null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerT_AddEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerT_AddEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerT_AddEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerT_RemoveEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.RemoveEventHandler((EventHandler<string>?)null), "Value cannot be null.\nParameter name: handler");
	}


	[Test]
	public void WeakEventManagerT_RemoveEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference
	}

	[Test]
	public void WeakEventManagerT_RemoveEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerT_RemoveEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => TestStringWeakEventManager.AddEventHandler(s => { var temp = s; }, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerT_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		TestStringEvent += HandleTestStringEvent;
		bool didEventFire = false;

		//Act

		//Assert
		Assert.Multiple(() =>
		{
			Assert.Throws<InvalidHandleEventException>(() => TestStringWeakEventManager.RaiseEvent("", nameof(TestStringEvent)));
			Assert.That(didEventFire, Is.False);
		});

		TestStringEvent -= HandleTestStringEvent;

		void HandleTestStringEvent(object? sender, string e) => didEventFire = true;
	}
}