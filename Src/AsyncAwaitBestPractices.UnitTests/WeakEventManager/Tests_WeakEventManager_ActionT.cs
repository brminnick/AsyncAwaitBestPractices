using System;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    class Tests_WeakEventManager_ActionT : BaseTest
    {
        readonly WeakEventManager<string> _actionEventManager = new WeakEventManager<string>();

        public event Action<string> ActionEvent
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
            _actionEventManager.HandleEvent("Test", nameof(ActionEvent));

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
            _actionEventManager.HandleEvent("Test", nameof(TestEvent));

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
            _actionEventManager.HandleEvent("Test", nameof(ActionEvent));

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
            unassignedEventManager.HandleEvent(nameof(ActionEvent));

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
            Assert.Throws<InvalidHandleEventException>(() => _actionEventManager.HandleEvent(this, "Test", nameof(ActionEvent)));
            Assert.IsFalse(didEventFire);
            ActionEvent -= HandleDelegateTest;
        }

        [Test]
        public void WeakEventManagerActionT_AddEventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler((Action<string>?)null, nameof(ActionEvent)), "Value cannot be null.\nParameter name: action");
#pragma warning enable CS8625 //Cannot convert null literal to non-nullable reference type
        }

        [Test]
        public void WeakEventManagerActionT_AddEventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625 
        }

        [Test]
        public void WeakEventManagerActionT_AddEventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler((Action<string>?)null, string.Empty), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625 
        }

        [Test]
        public void WeakEventManagerActionT_AddEventHandler_WhitespaceEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.AddEventHandler(s => { var temp = s; }, " "), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625
        }

        [Test]
        public void WeakEventManagerActionT_RemoveventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler((Action<string>?)null), "Value cannot be null.\nParameter name: handler");
#pragma warning enable CS8625 
        }

        [Test]
        public void WeakEventManagerActionT_RemoveventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, null), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625 
        }

        [Test]
        public void WeakEventManagerActionT_RemoveventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, string.Empty), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625
        }

        [Test]
        public void WeakEventManagerActionT_RemoveventHandler_WhiteSpaceEventName()
        {
            //Arrange

            //Act

            //Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
            Assert.Throws<ArgumentNullException>(() => _actionEventManager.RemoveEventHandler(s => { var temp = s; }, " "), "Value cannot be null.\nParameter name: eventName");
#pragma warning enable CS8625
        }
    }
}
