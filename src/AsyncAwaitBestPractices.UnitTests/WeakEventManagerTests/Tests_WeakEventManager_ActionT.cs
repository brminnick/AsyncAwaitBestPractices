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

		void HandleDelegateTest(string message)
		{
			Assert.IsNotNull(message);
			Assert.IsNotEmpty(message);

			didEventFire = true;
			ActionEvent -= HandleDelegateTest;
		}

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));

		//Assert
		Assert.IsTrue(didEventFire);
	}

	[Test]
	public void WeakEventManagerActionT_HandleEvent_InvalidHandleEventEventName()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		void HandleDelegateTest(string message)
		{
			Assert.IsNotNull(message);
			Assert.IsNotEmpty(message);

			didEventFire = true;
		}

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(TestEvent));

		//Assert
		Assert.False(didEventFire);
		ActionEvent -= HandleDelegateTest;
	}

	[Test]
	public void WeakEventManagerActionT_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;
		ActionEvent -= HandleDelegateTest;
		void HandleDelegateTest(string message)
		{
			Assert.IsNotNull(message);
			Assert.IsNotEmpty(message);

			didEventFire = true;
		}

		//Act
		_actionEventManager.RaiseEvent("Test", nameof(ActionEvent));

		//Assert
		Assert.IsFalse(didEventFire);
	}

	[Test]
	public void WeakEventManagerActionT_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager();
		bool didEventFire = false;

		ActionEvent += HandleDelegateTest;
		void HandleDelegateTest(string message)
		{
			Assert.IsNotNull(message);
			Assert.IsNotEmpty(message);

			didEventFire = true;
		}

		//Act
		unassignedEventManager.RaiseEvent(nameof(ActionEvent));

		//Assert
		Assert.IsFalse(didEventFire);
		ActionEvent -= HandleDelegateTest;
	}

	[Test]
	public void WeakEventManagerActionT_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		ActionEvent += HandleDelegateTest;
		bool didEventFire = false;

		void HandleDelegateTest(string message)
		{
			Assert.IsNotNull(message);
			Assert.IsNotEmpty(message);

			didEventFire = true;
		}

		//Act

		//Assert
		Assert.Throws<InvalidHandleEventException>(() => _actionEventManager.RaiseEvent(this, "Test", nameof(ActionEvent)));
		Assert.IsFalse(didEventFire);
		ActionEvent -= HandleDelegateTest;
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
		Assert.IsNull(_action);
		Assert.IsNotNull(addEventResult);
		Assert.IsNotNull(removeEventResult);

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