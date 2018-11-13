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
        public AsyncCommand(Func<Task> execute,
                            bool continueOnCapturedContext = true,
                            Action<Exception> onException = null,
                            Func<bool> canExecute = null)
        {
            _execute = execute;
            _continueOnCapturedContext = continueOnCapturedContext;
            _onException = (onException is null) ? (ex => throw ex) : onException;
            _canExecute = canExecute;
        }
        #endregion

        #region Events
        public event EventHandler CanExecuteChanged;
        #endregion

        #region Methods
        public bool CanExecute() => _canExecute?.Invoke() ?? true;
        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);

        public Task ExecuteAsync()
        {
            if (CanExecute() && _execute != null)
                return _execute.Invoke();

            return Task.CompletedTask;
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
