using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_ValueTask_SafeFireAndForget : BaseAsyncValueCommandTest
{
	[SetUp]
	public void BeforeEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[TearDown]
	public void AfterEachTest()
	{
		SafeFireAndForgetExtensions.Initialize(false);
		SafeFireAndForgetExtensions.RemoveDefaultExceptionHandling();
	}

	[Test]
	public async Task SafeFireAndForget_ValueTask_Succeeds()
	{
		//Arrange
		bool wasActionExecuted = false;
		var valueTask = new ValueTask(Task.Delay(500));

		// Act
		valueTask.SafeFireAndForget(onException: ex => wasActionExecuted = true);
		await valueTask;

		//Assert
		Assert.IsFalse(wasActionExecuted);
	}

#if NETCOREAPP1_0_OR_GREATER
	[Test]
	public void SafeFireAndForget_ValueTask_ThrowsException_ReThrown()
	{
		//Arrange
		bool wasActionExecuted = false;

		// Act
		Assert.ThrowsAsync<Exception>(async () =>
		{
			var valueTask = new ValueTask(Task.FromException(new Exception()));
			valueTask.SafeFireAndForget();

			await valueTask;
		});

		//Assert
		Assert.IsFalse(wasActionExecuted);
	}


	[Test]
	public async Task SafeFireAndForget_ValueTask_ThrowsException_ActionExecuted()
	{
		//Arrange
		bool wasActionExecuted = false;
		var valueTask = new ValueTask(Task.FromException(new Exception()));

		//Act
		valueTask.SafeFireAndForget(ex => wasActionExecuted = true);
		await valueTask;

		//Assert
		Assert.IsTrue(wasActionExecuted);
	}
#endif

	[Test]
	public async Task SafeFireAndForget_HandledException()
	{
		//Arrange
		Exception? exception = null;

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ex => exception = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception);
	}

	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_NoParams()
	{
		//Arrange
		Exception? exception = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget();
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception);
	}

	[Test]
	public async Task SafeFireAndForget_SetDefaultExceptionHandling_WithParams()
	{
		//Arrange
		Exception? exception1 = null;
		Exception? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget(ex => exception2 = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.IsNotNull(exception1);
		Assert.IsNotNull(exception2);
	}
}