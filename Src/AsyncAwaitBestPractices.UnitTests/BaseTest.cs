using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests
{
    abstract class BaseTest
    {
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

        protected const int Delay = 500;
        protected WeakEventManager TestWeakEventManager { get; } = new WeakEventManager();
        protected WeakEventManager<string> TestStringWeakEventManager { get; } = new WeakEventManager<string>();

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

        protected bool CanExecuteTrue(bool parameter) => true;
        protected bool CanExecuteTrue(int parameter) => true;
        protected bool CanExecuteTrue(string parameter) => true;
        protected bool CanExecuteTrue(object? parameter) => true;

        protected bool CanExecuteFalse(bool parameter) => false;
        protected bool CanExecuteFalse(int parameter) => false;
        protected bool CanExecuteFalse(string parameter) => false;
        protected bool CanExecuteFalse(object? parameter) => false;

        protected bool CanExecuteDynamic(object? booleanParameter)
        {
            if (booleanParameter is bool parameter)
                return parameter;

            throw new InvalidCastException();
        }
    }
}
