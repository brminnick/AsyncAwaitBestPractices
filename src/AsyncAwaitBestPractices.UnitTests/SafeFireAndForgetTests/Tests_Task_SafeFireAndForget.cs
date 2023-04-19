using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_SafeFireAndForget : BaseTest
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

	[Test]
	public async Task SafeFireAndForget_ThreadTest()
	{
		//Arrange
		Thread? initialThread, workingThread, finalThread;
		var threadTCS = new TaskCompletionSource<Thread>();

		//Act
		initialThread = Thread.CurrentThread;

		BlockingThreadMethod().SafeFireAndForget();

		finalThread = Thread.CurrentThread;

		workingThread = await threadTCS.Task;

		//Assert
		Assert.IsNotNull(initialThread);
		Assert.IsNotNull(workingThread);
		Assert.IsNotNull(finalThread);

		Assert.AreEqual(initialThread, finalThread);
		Assert.AreNotEqual(initialThread, workingThread);
		Assert.AreNotEqual(finalThread, workingThread);

		async Task BlockingThreadMethod()
		{
			await Task.Delay(100);
			threadTCS.SetResult(Thread.CurrentThread);
		}
	}

	[Test]
	public async Task SafeFireAndForget_NonAsyncMethodThreadTest()
	{
		//Arrange
		Thread initialThread, workingThread, finalThread;
		var threadTCS = new TaskCompletionSource<Thread>();

		//Act
		initialThread = Thread.CurrentThread;

		NonAsyncMethod().SafeFireAndForget();

		finalThread = Thread.CurrentThread;

		workingThread = await threadTCS.Task;

		//Assert
		Assert.IsNotNull(initialThread);
		Assert.IsNotNull(workingThread);
		Assert.IsNotNull(finalThread);

		Assert.AreEqual(initialThread, finalThread);
		Assert.AreEqual(initialThread, workingThread);
		Assert.AreEqual(finalThread, workingThread);

		Task NonAsyncMethod()
		{
			threadTCS.SetResult(Thread.CurrentThread);
			return Task.FromResult(true);
		}
	}

	[Test]
	public async Task SafeFireAndForget_Task_Succeeds()
	{
		//Arrange
		bool wasActionExecuted = false;

		//Act
		var task = Task.Run(() => Task.Delay(500));
		task.SafeFireAndForget(ex => wasActionExecuted = true);

		await task;

		//Assert
		Assert.IsFalse(wasActionExecuted);
	}

#if NETCOREAPP1_0_OR_GREATER
	[Test]
	public void SafeFireAndForget_Task_ThrowsException_ReThrown()
	{
		//Arrange
		bool wasActionExecuted = false;

		//Act
		Assert.ThrowsAsync<Exception>(async () =>
		{
			var task = Task.FromException(new Exception());
			task.SafeFireAndForget();

			await task;
		});

		//Assert
		Assert.IsFalse(wasActionExecuted);
	}

	[Test]
	public async Task SafeFireAndForget_Task_ThrowsException_ActionExecuted()
	{
		//Arrange
		bool wasActionExecuted = false;

		//Act
		var task = Task.FromException(new Exception());
		task.SafeFireAndForget(ex => wasActionExecuted = true);

		await task;

		//Assert
		Assert.IsTrue(wasActionExecuted);
	}
#endif
}