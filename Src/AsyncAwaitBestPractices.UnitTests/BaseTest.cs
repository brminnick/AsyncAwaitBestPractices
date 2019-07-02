using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests
{
    public abstract class BaseTest
    {
        #region Events
        protected event EventHandler TestEvent
        {
            add => TestWeakEventManager.AddEventHandler(value);
            remove => TestWeakEventManager.RemoveEventHandler(value);
        }

        protected event EventHandler<string> TestStringEvent
        {
            add => TestStringWeakEventManager.AddEventHandler(value);
            remove => TestStringWeakEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Properties
        protected const int Delay = 500;
        protected WeakEventManager TestWeakEventManager { get; } = new WeakEventManager();
        protected WeakEventManager<string> TestStringWeakEventManager { get; } = new WeakEventManager<string>();
        #endregion

        #region Methods
        protected Task NoParameterTask() => Task.Delay(Delay);
        protected Task IntParameterTask(int delay) => Task.Delay(delay);
        protected Task StringParameterTask(string text) => Task.Delay(Delay);
        protected Task NoParameterImmediateNullReferenceExceptionTask() => throw new NullReferenceException();
        protected Task ParameterImmediateNullReferenceExceptionTask(int delay) => throw new NullReferenceException();

        protected async Task NoParameterDelayedNullReferenceExceptionTask()
        {
            await Task.Delay(Delay);
            throw new NullReferenceException();
        }

        protected async Task IntParameterDelayedNullReferenceExceptionTask(int delay)
        {
            await Task.Delay(delay);
            throw new NullReferenceException();
        }

        protected bool CanExecuteTrue(object parameter) => true;
        protected bool CanExecuteFalse(object parameter) => false;
        #endregion
    }
}
