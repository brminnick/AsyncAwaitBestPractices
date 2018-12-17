using System;
using NUnit.Framework;
namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_WeakEventManager : BaseTest
    {
        readonly WeakEventManager _testWeakEventManager = new WeakEventManager();
        readonly WeakEventManager<string> _testStringWeakEventManager = new WeakEventManager<string>();

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
    }
}
