using System;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_ActionT : BaseTest
{
	readonly WeakEventManager<string> _actionEventManager = new();

#if NETCOREAPP3_1_OR_GREATER
	EventHandler<string>? _action;
#endif

	event Action<string> ActionEvent
	{
		add => _actionEventManager.AddEventHandler(value);
		remove => _actionEventManager.RemoveEventHandler(value);
	}

	[Test]
	public void WeakEventManagerActionT_HandleEvent_ValidImplementation()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
			ActionEvent -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerActionT_HandleEvent_InvalidHandleEventEventName()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(TestEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		ActionEvent -= HandleDelegateTest;

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
		}
	}

	[Test]
	public void WeakEventManagerActionT_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;
		ActionEvent -= HandleDelegateTest;

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.False);

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
		}
	}

	[Test]
	public void WeakEventManagerActionT_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager();
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;

		//Act
		unassignedEventManager.RaiseEvent(nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		ActionEvent -= HandleDelegateTest;

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
		}
	}

	[Test]
	public void WeakEventManagerActionT_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		//Act

		//Assert
		Assert.Multiple(() =>
		{
			Assert.Throws<InvalidHandleEventException>(() => _actionEventManager.RaiseEvent(this, "Test", nameof(ActionEvent)));
			Assert.That(didEventFire, Is.False);
		});

		ActionEvent -= HandleDelegateTest;

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
		}
	}

	[Test]
	public void WeakEventManagerActionT_AddEventHandler_NullHandler()
	{
		//Arrange
		Action<string>? nullAction = null;

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(nullAction, nameof(ActionEvent)), "Value cannot be null.\nParameter name: action");
	}

	[Test]
	public void WeakEventManagerActionT_AddEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerActionT_AddEventHandler_EmptyEventName()
	{
		//Arrange
		Action<string>? nullAction = null;

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(nullAction, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerActionT_AddEventHandler_WhitespaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(s => { var temp = s; }, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerActionT_RemoveEventHandler_NullHandler()
	{
		//Arrange
		Action<string>? nullAction = null;

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(nullAction), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerActionT_RemoveEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerActionT_RemoveEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerActionT_RemoveEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerActionT_RaiseEvent_WithParameters()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(string message)
		{
			Assert.Multiple(() =>
			{
				Assert.That(message, Is.Not.Null);
				Assert.That(message, Is.Not.Empty);
			});

			didEventFire = true;
			ActionEvent -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerActionT_ExceptionHandling()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		Exception? caughtException = null;

		//Act
		try
		{
			_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));
		}
		catch (Exception ex)
		{
			caughtException = ex;
		}

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		ActionEvent -= HandleDelegateTest;

		void HandleDelegateTest(string message) => throw new NullReferenceException();
	}

#if NETCOREAPP3_1_OR_GREATER
	[Test]
	public void WeakEventManagerActionT_AddRemoveEventHandler_VerifyNotNullAttribute()
	{
		//Arrange
		EventHandler<string> addEventResult, removeEventResult;
		string actionName = nameof(_action);

		//Act
		assignEvent();

		_actionEventManager.AddEventHandler(_action, actionName);
		addEventResult = _action;

		_action = null;
		assignEvent();

		_actionEventManager.RemoveEventHandler(_action, actionName);
		removeEventResult = _action;

		_action = null;

		//Assert
		Assert.Multiple(() =>
		{
			Assert.That(_action, Is.Null);
			Assert.That(addEventResult, Is.Not.Null);
			Assert.That(removeEventResult, Is.Not.Null);
		});

		void assignEvent()
		{
			_action = new EventHandler<string>(handleEvent);

			void handleEvent(object? sender, string e)
			{

			}
		}
	}
#endif
}