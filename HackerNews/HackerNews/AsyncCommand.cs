using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HackerNewsExtensions
{
    //Credit to John Thiriet, https://johnthiriet.com/mvvm-going-async-with-async-command/
    public class AsyncCommand : IAsyncCommand
    {
        #region Constant Fields
        readonly Func<Task> _execute;
        readonly Func<object, bool> _canExecute;
        readonly Action<Exception> _onException;
        readonly bool _continueOnCapturedContext;
        #endregion

        #region Constructors
        public AsyncCommand(Func<Task> execute,
                            bool continueOnCapturedContext = true,
                            Action<Exception> onException = null,
                            Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _continueOnCapturedContext = continueOnCapturedContext;
            _onException = (onException is null) ? (ex => throw ex) : onException;
            _canExecute = (canExecute is null) ? _ => true : canExecute;
        }
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Methods
        public bool CanExecute(object parameter) => _canExecute?.Invoke(parameter) ?? true;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        public Task ExecuteAsync(object parameter) => _execute?.Invoke();

        void ICommand.Execute(object parameter) => _execute?.Invoke()?.SafeFireAndForget(_continueOnCapturedContext, _onException);
        #endregion
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync(object parameter);
    }

    //Credit to John Thiriet, https://johnthiriet.com/removing-async-void/
    public static class TaskExtensions
    {
#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        public static async void SafeFireAndForget(this Task task, bool continueOnCapturedContext = true, Action<Exception> onException = null)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await task.ConfigureAwait(continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                if (onException is null)
                    throw;
                else
                    onException?.Invoke(ex);
            }
        }
    }
}
