using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_SafeFireAndForgetT : BaseTest
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
		NullReferenceException? exception = null;

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget<NullReferenceException>(ex => exception = ex);
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.That(exception, Is.Not.Null);
	}

	[Test]
	public async Task SafeFireAndForgetT_SetDefaultExceptionHandling_NoParams()
	{
		//Arrange
		Exception? exception = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget();
		await NoParameterTask();
		await NoParameterTask();

		//Assert
		Assert.That(exception, Is.Not.Null);
	}

	[Test]
	public async Task SafeFireAndForgetT_SetDefaultExceptionHandling_WithParams()
	{
		//Arrange
		Exception? exception1 = null;
		NullReferenceException? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionTask().SafeFireAndForget<NullReferenceException>(ex => exception2 = ex);
		await NoParameterTask();
		await NoParameterTask();

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(exception1, Is.Not.Null);
			Assert.That(exception2, Is.Not.Null);
		});
	}

	[Test]
	public void SafeFireAndForget_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		AsyncCommand command = new AsyncCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(() => command.ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void SafeFireAndForget_ExecuteAsync_ExceptionHandlingWithParameter_Test()
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(ParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(() => command.ExecuteAsync(0));

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}
}