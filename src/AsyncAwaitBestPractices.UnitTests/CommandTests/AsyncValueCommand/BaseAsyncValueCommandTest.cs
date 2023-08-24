using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests;

abstract class BaseAsyncValueCommandTest : BaseTest
{
	protected new static ValueTask NoParameterTask() => ValueTaskDelay(Delay);
	protected new static ValueTask IntParameterTask(int delay) => ValueTaskDelay(delay);
	protected new static ValueTask NullableIntParameterTask(int? delay) => ValueTaskDelay(delay ?? Delay);
	protected new static ValueTask StringParameterTask(string? text) => ValueTaskDelay(Delay);
	protected new static ValueTask NoParameterImmediateNullReferenceExceptionTask() => throw new NullReferenceException();
	protected new static ValueTask ParameterImmediateNullReferenceExceptionTask(int delay) => throw new NullReferenceException();

	protected new static async ValueTask NoParameterDelayedNullReferenceExceptionTask()
	{
		await Task.Delay(Delay);
		throw new NullReferenceException();
	}

	protected new static async ValueTask IntParameterDelayedNullReferenceExceptionTask(int delay)
	{
		await Task.Delay(delay);
		throw new NullReferenceException();
	}

	static ValueTask ValueTaskDelay(int delay) => new(Task.Delay(delay));
}