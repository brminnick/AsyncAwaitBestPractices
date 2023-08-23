using System;
using System.Threading.Tasks;

namespace AsyncAwaitBestPractices.UnitTests;

abstract class BaseAsyncValueCommandTest : BaseTest
{
	protected new ValueTask NoParameterTask() => BaseAsyncValueCommandTest.ValueTaskDelay(Delay);
	protected new ValueTask IntParameterTask(int delay) => BaseAsyncValueCommandTest.ValueTaskDelay(delay);
	protected new ValueTask NullableIntParameterTask(int? delay) => BaseAsyncValueCommandTest.ValueTaskDelay(delay ?? Delay);
	protected new ValueTask StringParameterTask(string? text) => BaseAsyncValueCommandTest.ValueTaskDelay(Delay);
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