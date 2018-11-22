using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests
{
    public abstract class BaseTest
    {
        protected const int Delay = 500;

        protected Task NoParameterTask() => Task.Delay(Delay);
        protected Task IntParameterTask(int delay) => Task.Delay(delay);
        protected Task StringParameterTask(string text) => Task.Delay(Delay);
        protected Task NoParameterExceptionTask() => Task.Run(() => throw new Exception());
        protected Task IntParameterExceptionTask(int delay) => Task.Run(() => throw new Exception());

        protected bool CanExecuteTrue(object parameter) => true;
        protected bool CanExecuteFalse(object parameter) => false;
    }
}
