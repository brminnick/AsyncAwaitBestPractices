using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests
{
    public abstract class BaseTest
    {
        #region Constant Fields
        protected const int Delay = 500;
        protected readonly WeakEventManager _testWeakEventManager = new WeakEventManager();
        protected readonly WeakEventManager<string> _testStringWeakEventManager = new WeakEventManager<string>();
        #endregion

        #region Events
        protected event EventHandler TestEvent
        {
            add => _testWeakEventManager.AddEventHandler(value);
            remove => _testWeakEventManager.RemoveEventHandler(value);
        }

        protected event EventHandler<string> TestStringEvent
        {
            add => _testStringWeakEventManager.AddEventHandler(value);
            remove => _testStringWeakEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Methods
        protected Task NoParameterTask() => Task.Delay(Delay);
        protected Task IntParameterTask(int delay) => Task.Delay(delay);
        protected Task StringParameterTask(string text) => Task.Delay(Delay);
        protected Task NoParameterExceptionTask() => Task.Run(() => throw new Exception());
        protected Task IntParameterExceptionTask(int delay) => Task.Run(() => throw new Exception());

        protected bool CanExecuteTrue(object parameter) => true;
        protected bool CanExecuteFalse(object parameter) => false;
        #endregion
    }
}
