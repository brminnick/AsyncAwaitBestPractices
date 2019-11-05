namespace AsyncAwaitBestPractices.MVVM
{
    /// <summary>
    /// An Async implmentation of ICommand for ValueTask
    /// </summary>
    public interface IAsyncValueCommand<T> : System.Windows.Input.ICommand
    {
        /// <summary>
        /// Executes the Command as a ValueTask
        /// </summary>
        /// <returns>The ValueTask to execute</returns>
        /// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
        System.Threading.Tasks.ValueTask ExecuteAsync(T parameter);
    }

    /// <summary>
    /// An Async implmentation of ICommand for ValueTask
    /// </summary>
    public interface IAsyncValueCommand : System.Windows.Input.ICommand
    {
        /// <summary>
        /// Executes the Command as a ValueTask
        /// </summary>
        /// <returns>The ValueTask to execute</returns>
        System.Threading.Tasks.ValueTask ExecuteAsync();
    }
}
