using System;
using System.ComponentModel;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_WeakEventManager_Delegate : BaseTest, INotifyPropertyChanged
{
	readonly WeakEventManager _propertyChangedWeakEventManager = new();

#if NETCOREAPP3_1_OR_GREATER
	PropertyChangedEventHandler? _nullablePropertyChangedEventHandler;
#endif

	public event PropertyChangedEventHandler? PropertyChanged
	{
		add => _propertyChangedWeakEventManager.AddEventHandler(value);
		remove => _propertyChangedWeakEventManager.RemoveEventHandler(value);
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_ValidImplementation()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(this, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender?.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			PropertyChanged -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_NullSender()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(null, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Null);
				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			PropertyChanged -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_InvalidEventArgs()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act

		Assert.Multiple(() =>
		{
			//Assert
			Assert.Throws<ArgumentException>(() => _propertyChangedWeakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(PropertyChanged)));
			Assert.That(didEventFire, Is.False);
		});

		PropertyChanged -= HandleDelegateTest;

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_NullEventArgs()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(this, null, nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender?.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Null);
			});

			didEventFire = true;
			PropertyChanged -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_InvalidHandleEventEventName()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(this, new PropertyChangedEventArgs("Test"), nameof(TestStringEvent));

		//Assert
		Assert.That(didEventFire, Is.False);
		PropertyChanged -= HandleDelegateTest;

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_DynamicMethod_ValidImplementation()
	{
		//Arrange
		var dynamicMethod = new System.Reflection.Emit.DynamicMethod(string.Empty, typeof(void), [typeof(object), typeof(PropertyChangedEventArgs)]);
		var ilGenerator = dynamicMethod.GetILGenerator();
		ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

		var handler = (PropertyChangedEventHandler)dynamicMethod.CreateDelegate(typeof(PropertyChangedEventHandler));
		PropertyChanged += handler;

		//Act

		//Assert
		Assert.DoesNotThrow(() => _propertyChangedWeakEventManager.RaiseEvent(this, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged)));
		PropertyChanged -= handler;
	}

	[Test]
	public void WeakEventManagerDelegate_UnassignedEvent()
	{
		//Arrange
		bool didEventFire = false;

		PropertyChanged += HandleDelegateTest;
		PropertyChanged -= HandleDelegateTest;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(null, null, nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.False);

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerDelegate_UnassignedEventManager()
	{
		//Arrange
		var unassignedEventManager = new WeakEventManager();
		bool didEventFire = false;

		PropertyChanged += HandleDelegateTest;

		//Act
		unassignedEventManager.RaiseEvent(null, null, nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.False);
		PropertyChanged -= HandleDelegateTest;

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerDelegate_HandleEvent_InvalidHandleEvent()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act

		//Assert
		Assert.Throws<InvalidHandleEventException>(() => _propertyChangedWeakEventManager.RaiseEvent(nameof(PropertyChanged)));
		Assert.That(didEventFire, Is.False);
		PropertyChanged -= HandleDelegateTest;

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => didEventFire = true;
	}

	[Test]
	public void WeakEventManagerDelegate_AddEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.AddEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerDelegate_AddEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerDelegate_AddEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerDelegate_AddEventHandler_WhitespaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.AddEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerDelegate_RemoveEventHandler_NullHandler()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
	}

	[Test]
	public void WeakEventManagerDelegate_RemoveEventHandler_NullEventName()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning restore CS8625
	}

	[Test]
	public void WeakEventManagerDelegate_RemoveEventHandler_EmptyEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerDelegate_RemoveEventHandler_WhiteSpaceEventName()
	{
		//Arrange

		//Act

		//Assert
		Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
	}

	[Test]
	public void WeakEventManagerDelegate_RemoveEventHandler_NoArgumentOutOfRangeException()
	{
		//Arrange


		//Act
		_propertyChangedWeakEventManager.AddEventHandler(sampleDelegate, nameof(sampleDelegate));

		//Assert
		Assert.DoesNotThrow(() =>
		{
			Parallel.For(0, 10, count => _propertyChangedWeakEventManager.RemoveEventHandler(sampleDelegate, nameof(sampleDelegate)));
		});

		static void sampleDelegate(object? sender, EventArgs e)
		{

		}
	}

#if NETCOREAPP3_1_OR_GREATER
	[Test]
	public void WeakEventManagerDelegate_AddRemoveEventHandler_VerifyNotNullAttribute()
	{
		//Arrange
		PropertyChangedEventHandler addEventHandlerResult, removeEventHandlerResult;
		string eventHandlerName = nameof(_nullablePropertyChangedEventHandler);

		//Act
		assignEventHandler();

		_propertyChangedWeakEventManager.AddEventHandler(_nullablePropertyChangedEventHandler, eventHandlerName);
		addEventHandlerResult = _nullablePropertyChangedEventHandler;

		_nullablePropertyChangedEventHandler = null;
		assignEventHandler();

		_propertyChangedWeakEventManager.RemoveEventHandler(_nullablePropertyChangedEventHandler, eventHandlerName);
		removeEventHandlerResult = _nullablePropertyChangedEventHandler;

		_nullablePropertyChangedEventHandler = null;

		//Assert
		Assert.Multiple(() =>
		{
			Assert.That(_nullablePropertyChangedEventHandler, Is.Null);
			Assert.That(addEventHandlerResult, Is.Not.Null);
			Assert.That(removeEventHandlerResult, Is.Not.Null);
		});

		void assignEventHandler()
		{
			_nullablePropertyChangedEventHandler = new PropertyChangedEventHandler(handlePropertyChanged);

			void handlePropertyChanged(object? sender, PropertyChangedEventArgs e)
			{

			}
		}
	}
#endif

	[Test]
	public void WeakEventManagerDelegate_RaiseEvent_WithParameters()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		bool didEventFire = false;

		//Act
		_propertyChangedWeakEventManager.RaiseEvent(this, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));

		//Assert
		Assert.That(didEventFire, Is.True);

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e)
		{
			Assert.Multiple(() =>
			{
				Assert.That(sender, Is.Not.Null);
				Assert.That(sender?.GetType(), Is.EqualTo(this.GetType()));

				Assert.That(e, Is.Not.Null);
			});

			didEventFire = true;
			PropertyChanged -= HandleDelegateTest;
		}
	}

	[Test]
	public void WeakEventManagerDelegate_ExceptionHandling()
	{
		//Arrange
		PropertyChanged += HandleDelegateTest;
		Exception? caughtException = null;

		//Act
		try
		{
			_propertyChangedWeakEventManager.RaiseEvent(this, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));
		}
		catch (Exception ex)
		{
			caughtException = ex;
		}

		//Assert
		Assert.That(caughtException, Is.Not.Null);
		PropertyChanged -= HandleDelegateTest;

		void HandleDelegateTest(object? sender, PropertyChangedEventArgs e) => throw new NullReferenceException();
	}
}