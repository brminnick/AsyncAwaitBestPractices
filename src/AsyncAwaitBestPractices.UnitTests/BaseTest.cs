using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests;

[ExcludeFromCodeCoverage]
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
	protected WeakEventManager TestWeakEventManager { get; } = new();
	protected WeakEventManager<string> TestStringWeakEventManager { get; } = new();

	protected static Task NoParameterTask() => Task.Delay(Delay);
	protected static Task IntParameterTask(int delay) => Task.Delay(delay);
	protected static Task NullableIntParameterTask(int? delay) => Task.Delay(delay ?? Delay);
	protected static Task StringParameterTask(string? text) => Task.Delay(Delay);
	protected static Task NoParameterImmediateNullReferenceExceptionTask() => throw new NullReferenceException();
	protected static Task ParameterImmediateNullReferenceExceptionTask(int delay) => throw new NullReferenceException();

	protected static async Task NoParameterDelayedNullReferenceExceptionTask()
	{
		await Task.Delay(Delay);
		throw new NullReferenceException();
	}

	protected static async Task IntParameterDelayedNullReferenceExceptionTask(int delay)
	{
		await Task.Delay(delay);
		throw new NullReferenceException();
	}

	protected static async ValueTask NoParameterValueTask() => await Task.Delay(Delay);
	protected static async ValueTask IntParameterValueTask(int delay) => await Task.Delay(delay);
	protected static async ValueTask NullableIntParameterValueTask(int? delay) => await Task.Delay(delay ?? Delay);
	protected static async ValueTask StringParameterValueTask(string? text) => await Task.Delay(Delay);
	protected static ValueTask NoParameterImmediateNullReferenceExceptionValueTask() => throw new NullReferenceException();
	protected static ValueTask ParameterImmediateNullReferenceExceptionValueTask(int delay) => throw new NullReferenceException();

	protected static async ValueTask NoParameterDelayedNullReferenceExceptionValueTask()
	{
		await Task.Delay(Delay);
		throw new NullReferenceException();
	}

	protected static async ValueTask IntParameterDelayedNullReferenceExceptionValueTask(int delay)
	{
		await Task.Delay(delay);
		throw new NullReferenceException();
	}

	protected static bool CanExecuteTrue(bool parameter) => true;
	protected static bool CanExecuteTrue(int parameter) => true;
	protected static bool CanExecuteTrue(string? parameter) => true;
	protected static bool CanExecuteTrue(object? parameter) => true;

	protected static bool CanExecuteFalse(bool parameter) => false;
	protected static bool CanExecuteFalse(int parameter) => false;
	protected static bool CanExecuteFalse(string? parameter) => false;
	protected static bool CanExecuteFalse(object? parameter) => false;

	protected static bool CanExecuteDynamic(object? booleanParameter)
	{
		if (booleanParameter is bool parameter)
			return parameter;

		throw new InvalidCastException();
	}
}