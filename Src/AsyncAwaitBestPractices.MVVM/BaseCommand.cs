using System;
using System.ComponentModel;

namespace AsyncAwaitBestPractices.MVVM
{
    /// <summary>
    /// Abstract Base Class for AsyncCommand and AsyncValueCommand
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public abstract class BaseCommand
    {
        readonly Func<object, bool> _canExecute;
        readonly WeakEventManager weakEventManager = new WeakEventManager();

        /// <summary>
        /// Initializes BaseCommand
        /// </summary>
        /// <param name="canExecute"></param>
        public BaseCommand(Func<object, bool>? canExecute) => _canExecute = canExecute ?? (_ => true);

        /// <summary>
        /// Occurs when changes occur that affect whether or not the command should execute
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => weakEventManager.AddEventHandler(value);
            remove => weakEventManager.RemoveEventHandler(value);
        }

        /// <summary>
        /// Determines whether the command can execute in its current state
        /// </summary>
        /// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        public bool CanExecute(object parameter) => _canExecute(parameter);

        /// <summary>
        /// Raises the CanExecuteChanged event.
        /// </summary>
        public void RaiseCanExecuteChanged() => weakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
    }
}
