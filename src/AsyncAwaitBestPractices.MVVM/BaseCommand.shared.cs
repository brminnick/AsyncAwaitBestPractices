using System;
using System.ComponentModel;
using System.Reflection;

namespace AsyncAwaitBestPractices.MVVM;

/// <summary>
/// Abstract Base Class for AsyncCommand and AsyncValueCommand
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class BaseCommand<TCanExecute>
{
	readonly Func<TCanExecute?, bool> _canExecute;
	readonly WeakEventManager _weakEventManager = new();

	/// <summary>
	/// Initializes BaseCommand
	/// </summary>
	/// <param name="canExecute"></param>
	private protected BaseCommand(Func<TCanExecute?, bool>? canExecute) => _canExecute = canExecute ?? (_ => true);

	/// <summary>
	/// Occurs when changes occur that affect whether the command should execute
	/// </summary>
	public event EventHandler? CanExecuteChanged
	{
		add => _weakEventManager.AddEventHandler(value);
		remove => _weakEventManager.RemoveEventHandler(value);
	}

	/// <summary>
	/// Determines whether the command can execute in its current state
	/// </summary>
	/// <returns><c>true</c>, if this command can be executed; otherwise, <c>false</c>.</returns>
	/// <param name="parameter">Data used by the command. If the command does not require data to be passed, this object can be set to null.</param>
	public bool CanExecute(TCanExecute? parameter) => _canExecute(parameter);

	/// <summary>
	/// Raises the CanExecuteChanged event.
	/// </summary>
	public void RaiseCanExecuteChanged() => _weakEventManager.RaiseEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));

	/// <summary>
	/// Determine if T is Nullable
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	private protected static bool IsNullable<T>()
	{
		var type = typeof(T);

		if (!type.GetTypeInfo().IsValueType)
			return true; // ref-type

		if (Nullable.GetUnderlyingType(type) != null)
			return true; // Nullable<T>

		return false; // value-type
	}
}