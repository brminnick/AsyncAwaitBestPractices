using System;
using System.ComponentModel;
using NUnit.Framework;
namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_WeakEventManager : BaseTest, INotifyPropertyChanged
    {
        readonly WeakEventManager _testWeakEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _testStringWeakEventManager = new WeakEventManager<string>();
        readonly WeakEventManager _propertyChangedWeakEventManager = new WeakEventManager();

        event EventHandler TestEvent
        {
            add => _testWeakEventManager.AddEventHandler(value);
            remove => _testWeakEventManager.RemoveEventHandler(value);
        }

        event EventHandler<string> TestStringEvent
        {
            add => _testStringWeakEventManager.AddEventHandler(value);
            remove => _testStringWeakEventManager.RemoveEventHandler(value);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add => _propertyChangedWeakEventManager.AddEventHandler(value);
            remove => _propertyChangedWeakEventManager.RemoveEventHandler(value);
        }

        /*************************
        * WeakEventManager Tests *
        **************************/

        [Test]
        public void WeakEventManager_HandleEvent_ValidImplementation()
        {
            //Arrange
            TestEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, EventArgs e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNotNull(e);

                didEventFire = true;
                TestEvent -= HandleTestEvent;
            }

            //Act
            _testWeakEventManager?.HandleEvent(this, new EventArgs(), nameof(TestEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManager_HandleEvent_NullSender()
        {
            //Arrange
            TestEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, EventArgs e)
            {
                Assert.IsNull(sender);
                Assert.IsNotNull(e);

                didEventFire = true;
                TestEvent -= HandleTestEvent;
            }

            //Act
            _testWeakEventManager?.HandleEvent(null, new EventArgs(), nameof(TestEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManager_HandleEvent_EmptyEventArgs()
        {
            //Arrange
            TestEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, EventArgs e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNotNull(e);
                Assert.AreEqual(EventArgs.Empty, e);

                didEventFire = true;
                TestEvent -= HandleTestEvent;
            }

            //Act
            _testWeakEventManager?.HandleEvent(this, EventArgs.Empty, nameof(TestEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManager_HandleEvent_NullEventArgs()
        {
            //Arrange
            TestEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, EventArgs e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNull(e);

                didEventFire = true;
                TestEvent -= HandleTestEvent;
            }

            //Act
            _testWeakEventManager?.HandleEvent(this, null, nameof(TestEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManager_HandleEvent_InvalidHandleEvent()
        {
            //Arrange
            TestEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, EventArgs e) => didEventFire = true;

            //Act
            _testWeakEventManager?.HandleEvent(this, new EventArgs(), nameof(TestStringEvent));

            //Assert
            Assert.False(didEventFire);
            TestEvent -= HandleTestEvent;
        }

        [Test]
        public void WeakEventManager_UnassignedEvent()
        {
            //Arrange
            bool didEventFire = false;

            TestEvent += HandleTestEvent;
            TestEvent -= HandleTestEvent;
            void HandleTestEvent(object sender, EventArgs e) => didEventFire = true;

            //Act
            _testWeakEventManager.HandleEvent(null, null, nameof(TestEvent));

            //Assert
            Assert.IsFalse(didEventFire);
        }

        [Test]
        public void WeakEventManager_UnassignedEventManager()
        {
            //Arrange
            var unassignedEventManager = new WeakEventManager();
            bool didEventFire = false;

            TestEvent += HandleTestEvent;
            void HandleTestEvent(object sender, EventArgs e) => didEventFire = true;

            //Act
            unassignedEventManager.HandleEvent(null, null, nameof(TestEvent));

            //Assert
            Assert.IsFalse(didEventFire);
            TestEvent -= HandleTestEvent;
        }

        [Test]
        public void WeakEventManager_AddEventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.AddEventHandler(null), "Value cannot be null.\nParameter name: handler");
        }

        [Test]
        public void WeakEventManager_AddEventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManager_AddEventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManager_AddEventHandler_WhitespaceEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.AddEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManager_RemoveventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
        }

        [Test]
        public void WeakEventManager_RemoveventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.RemoveEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManager_RemoveventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.RemoveEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManager_RemoveventHandler_WhiteSpaceEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testWeakEventManager.RemoveEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
        }

        /*************************
        * WeakEventManager Tests *
        **************************/

        [Test]
        public void WeakEventManagerDelegate_HandleEvent_ValidImplementation()
        {
            //Arrange
            PropertyChanged += HandleDelegateTest;
            bool didEventFire = false;

            void HandleDelegateTest(object sender, PropertyChangedEventArgs e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNotNull(e);

                didEventFire = true;
                PropertyChanged -= HandleDelegateTest;
            }

            //Act
            _propertyChangedWeakEventManager?.HandleEvent(this, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManagerDelegate_HandleEvent_NullSender()
        {
            //Arrange
            PropertyChanged += HandleDelegateTest;
            bool didEventFire = false;

            void HandleDelegateTest(object sender, PropertyChangedEventArgs e)
            {
                Assert.IsNull(sender);
                Assert.IsNotNull(e);

                didEventFire = true;
                PropertyChanged -= HandleDelegateTest;
            }

            //Act
            _propertyChangedWeakEventManager?.HandleEvent(null, new PropertyChangedEventArgs("Test"), nameof(PropertyChanged));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManagerDelegate_HandleEvent_InvalidEventArgs()
        {
            //Arrange
            PropertyChanged += HandleDelegateTest;
            bool didEventFire = false;

            void HandleDelegateTest(object sender, PropertyChangedEventArgs e) => didEventFire = true;

            //Act

            //Assert
            Assert.Throws<ArgumentException>(() => _propertyChangedWeakEventManager?.HandleEvent(this, EventArgs.Empty, nameof(PropertyChanged)));
            Assert.IsFalse(didEventFire);
            PropertyChanged -= HandleDelegateTest;
        }

        [Test]
        public void WeakEventManagerDelegate_HandleEvent_NullEventArgs()
        {
            //Arrange
            PropertyChanged += HandleDelegateTest;
            bool didEventFire = false;

            void HandleDelegateTest(object sender, PropertyChangedEventArgs e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNull(e);

                didEventFire = true;
                PropertyChanged -= HandleDelegateTest;
            }

            //Act
            _propertyChangedWeakEventManager?.HandleEvent(this, null, nameof(PropertyChanged));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManagerDelegate_HandleEvent_InvalidHandleEvent()
        {
            //Arrange
            PropertyChanged += HandleDelegateTest;
            bool didEventFire = false;

            void HandleDelegateTest(object sender, PropertyChangedEventArgs e) => didEventFire = true;

            //Act
            _propertyChangedWeakEventManager?.HandleEvent(this, new PropertyChangedEventArgs("Test"), nameof(TestStringEvent));

            //Assert
            Assert.False(didEventFire);
            PropertyChanged -= HandleDelegateTest;
        }

        [Test]
        public void WeakEventManagerDelegate_UnassignedEvent()
        {
            //Arrange
            bool didEventFire = false;

            PropertyChanged += HandleDelegateTest;
            PropertyChanged -= HandleDelegateTest;
            void HandleDelegateTest(object sender, PropertyChangedEventArgs e) => didEventFire = true;

            //Act
            _propertyChangedWeakEventManager.HandleEvent(null, null, nameof(PropertyChanged));

            //Assert
            Assert.IsFalse(didEventFire);
        }

        [Test]
        public void WeakEventManagerDelegate_UnassignedEventManager()
        {
            //Arrange
            var unassignedEventManager = new WeakEventManager();
            bool didEventFire = false;

            PropertyChanged += HandleDelegateTest;
            void HandleDelegateTest(object sender, PropertyChangedEventArgs e) => didEventFire = true;

            //Act
            unassignedEventManager.HandleEvent(null, null, nameof(PropertyChanged));

            //Assert
            Assert.IsFalse(didEventFire);
            PropertyChanged -= HandleDelegateTest;
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
            Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
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
        public void WeakEventManagerDelegate_RemoveventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
        }

        [Test]
        public void WeakEventManagerDelegate_RemoveventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerDelegate_RemoveventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerDelegate_RemoveventHandler_WhiteSpaceEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _propertyChangedWeakEventManager.RemoveEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
        }


        /*****************************
         * WeakEventManager<T> Tests *
         *****************************/

        [Test]
        public void WeakEventManagerTEventArgs_HandleEvent_ValidImplementation()
        {
            //Arrange
            TestStringEvent += HandleTestEvent;

            const string stringEventArg = "Test";
            bool didEventFire = false;

            void HandleTestEvent(object sender, string e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNotNull(e);
                Assert.AreEqual(stringEventArg, e);

                didEventFire = true;
                TestStringEvent -= HandleTestEvent;
            }

            //Act
            _testStringWeakEventManager?.HandleEvent(this, stringEventArg, nameof(TestStringEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManageTEventArgs_HandleEvent_NullSender()
        {
            //Arrange
            TestStringEvent += HandleTestEvent;

            const string stringEventArg = "Test";

            bool didEventFire = false;

            void HandleTestEvent(object sender, string e)
            {
                Assert.IsNull(sender);

                Assert.IsNotNull(e);
                Assert.AreEqual(stringEventArg, e);

                didEventFire = true;
                TestStringEvent -= HandleTestEvent;
            }

            //Act
            _testStringWeakEventManager?.HandleEvent(null, stringEventArg, nameof(TestStringEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManagerTEventArgs_HandleEvent_NullEventArgs()
        {
            //Arrange
            TestStringEvent += HandleTestEvent;
            bool didEventFire = false;

            void HandleTestEvent(object sender, string e)
            {
                Assert.IsNotNull(sender);
                Assert.AreEqual(this.GetType(), sender.GetType());

                Assert.IsNull(e);

                didEventFire = true;
                TestStringEvent -= HandleTestEvent;
            }

            //Act
            _testStringWeakEventManager?.HandleEvent(this, null, nameof(TestStringEvent));

            //Assert
            Assert.IsTrue(didEventFire);
        }

        [Test]
        public void WeakEventManagerTEventArgs_HandleEvent_InvalidHandleEvent()
        {
            //Arrange
            TestStringEvent += HandleTestEvent;

            bool didEventFire = false;

            void HandleTestEvent(object sender, string e) => didEventFire = true;

            //Act
            _testStringWeakEventManager?.HandleEvent(this, "Test", nameof(TestEvent));

            //Assert
            Assert.False(didEventFire);
            TestStringEvent -= HandleTestEvent;
        }

        [Test]
        public void WeakEventManager_NullEventManager()
        {
            //Arrange
            WeakEventManager unassignedEventManager = null;

            //Act

            //Assert
            Assert.Throws<NullReferenceException>(() => unassignedEventManager.HandleEvent(null, null, nameof(TestEvent)));
        }

        [Test]
        public void WeakEventManagerTEventArgs_UnassignedEventManager()
        {
            //Arrange
            var unassignedEventManager = new WeakEventManager<string>();
            bool didEventFire = false;

            TestStringEvent += HandleTestEvent;
            void HandleTestEvent(object sender, string e) => didEventFire = true;

            //Act
            unassignedEventManager.HandleEvent(null, null, nameof(TestStringEvent));

            //Assert
            Assert.IsFalse(didEventFire);
            TestStringEvent -= HandleTestEvent;
        }

        [Test]
        public void WeakEventManagerTEventArgs_UnassignedEvent()
        {
            //Arrange
            bool didEventFire = false;

            TestStringEvent += HandleTestEvent;
            TestStringEvent -= HandleTestEvent;
            void HandleTestEvent(object sender, string e) => didEventFire = true;

            //Act
            _testStringWeakEventManager.HandleEvent(this, "Test", nameof(TestStringEvent));

            //Assert
            Assert.IsFalse(didEventFire);
        }

        [Test]
        public void WeakEventManagerT_AddEventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null), "Value cannot be null.\nParameter name: handler");
        }

        [Test]
        public void WeakEventManagerT_AddEventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerT_AddEventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerT_AddEventHandler_WhiteSpaceEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, " "), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerT_RemoveventHandler_NullHandler()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.RemoveEventHandler(null), "Value cannot be null.\nParameter name: handler");
        }


        [Test]
        public void WeakEventManagerT_RemoveventHandler_NullEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, null), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerT_RemoveventHandler_EmptyEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }

        [Test]
        public void WeakEventManagerT_RemoveventHandler_WhiteSpaceEventName()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => _testStringWeakEventManager.AddEventHandler(null, string.Empty), "Value cannot be null.\nParameter name: eventName");
        }
    }
}
