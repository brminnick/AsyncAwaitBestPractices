using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AsyncAwaitBestPractices.MVVM;

/// <summary>
/// Abstract Base Class used by AsyncValueCommand
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class BaseAsyncCommand<TExecute, TCanExecute> : BaseCommand<TCanExecute>, ICommand
{
	readonly Func<TExecute?, Task> _execute;
	readonly Action<Exception>? _onException;
	readonly bool _continueOnCapturedContext;

	/// <summary>
	/// Initializes a new instance of BaseAsyncCommand
	/// </summary>
	/// <param name="execute">The Function executed when Execute or ExecuteAsync is called. This does not check canExecute before executing and will execute even if canExecute is false</param>
	/// <param name="canExecute">The Function that verifies whether AsyncCommand should execute.</param>
	/// <param name="onException">If an exception is thrown in the Task, <c>onException</c> will execute. If onException is null, the exception will be re-thrown</param>
	/// <param name="continueOnCapturedContext">If set to <c>true</c> continue on captured context; this will ensure that the Synchronization Context returns to the calling thread. If set to <c>false</c> continue on a different context; this will allow the Synchronization Context to continue on a different thread</param>
	private protected BaseAsyncCommand(Func<TExecute?, Task>? execute,
										Func<TCanExecute?, bool>? canExecute,
										Action<Exception>? onException,
										bool continueOnCapturedContext) : base(canExecute)
	{
		_execute = execute ?? throw new ArgumentNullException(nameof(execute), $"{nameof(execute)} cannot be null");
		_onException = onException;
		_continueOnCapturedContext = continueOnCapturedContext;
	}

	/// <summary>
	/// Executes the Command as a Task
	/// </summary>
	/// <returns>The executed Task</returns>
	/// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
	private protected Task ExecuteAsync(TExecute? parameter) => _execute(parameter);

	bool ICommand.CanExecute(object? parameter) => parameter switch
	{
		TCanExecute validParameter => CanExecute(validParameter),
		null when IsNullable<TCanExecute>() => CanExecute((TCanExecute?)parameter),
		null => throw new InvalidCommandParameterException(typeof(TCanExecute)),
		_ => throw new InvalidCommandParameterException(typeof(TCanExecute), parameter.GetType()),
	};

	void ICommand.Execute(object? parameter)
	{
		switch (parameter)
		{
			case TExecute validParameter:
				ExecuteAsync(validParameter).SafeFireAndForget(_onException, _continueOnCapturedContext);
				break;

			case null when IsNullable<TExecute>():
				ExecuteAsync((TExecute?)parameter).SafeFireAndForget(_onException, _continueOnCapturedContext);
				break;

			case null:
				throw new InvalidCommandParameterException(typeof(TExecute));

			default:
				throw new InvalidCommandParameterException(typeof(TExecute), parameter.GetType());
		}
	}
}