using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HackerNews
{
    public class AsyncCommand : IAsyncCommand
    {
        #region Constant Fields
        readonly Func<Task> _execute;
        readonly Func<bool> _canExecute;
        readonly Action<Exception> _onException;
        readonly bool _continueOnCapturedContext;
        #endregion

        #region Constructors
        public AsyncCommand(
            Func<Task> execute,
            Action<Exception> errorHandler = null,
            bool continueOnCapturedContext = true,
            Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
            _onException = errorHandler;
            _continueOnCapturedContext = continueOnCapturedContext;
        }
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Methods
        public bool CanExecute() => _canExecute?.Invoke() ?? true;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public async Task ExecuteAsync()
        {
            if (CanExecute())
                await _execute?.Invoke();
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

#pragma warning disable RECS0165 // Asynchronous methods should return a Task instead of void
        async void ICommand.Execute(object parameter)
#pragma warning restore RECS0165 // Asynchronous methods should return a Task instead of void
        {
            try
            {
                await ExecuteAsync().ConfigureAwait(_continueOnCapturedContext);
            }
            catch (Exception ex)
            {
                _onException?.Invoke(ex);
            }
        }
        #endregion
    }

    public interface IAsyncCommand : ICommand
    {
        Task ExecuteAsync();
        bool CanExecute();
    }
}
