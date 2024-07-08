using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_ValueTask_SafeFireAndForgetT : BaseAsyncValueCommandTest
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
		NoParameterDelayedNullReferenceExceptionValueTask().SafeFireAndForget<NullReferenceException>(ex => exception = ex);
		await BaseTest.NoParameterTask();
		await BaseTest.NoParameterTask();

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
		NoParameterDelayedNullReferenceExceptionValueTask().SafeFireAndForget();
		await BaseTest.NoParameterTask();
		await BaseTest.NoParameterTask();

		//Assert
		Assert.That(exception, Is.Not.Null);
	}

	[Test]
	public async Task SafeFireAndForgetT_SetDefaultExceptionHandling_WithParam()
	{
		//Arrange
		Exception? exception1 = null;
		NullReferenceException? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionValueTask().SafeFireAndForget<NullReferenceException>(ex => exception2 = ex);
		await BaseTest.NoParameterTask();
		await BaseTest.NoParameterTask();

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(exception1, Is.Not.Null);
			Assert.That(exception2, Is.Not.Null);
		});
	}

	[Test]
	public async Task SafeFireAndForgetT_SetDefaultExceptionHandling_WithParams()
	{
		//Arrange
		Exception? exception1 = null;
		NullReferenceException? exception2 = null;
		SafeFireAndForgetExtensions.SetDefaultExceptionHandling(ex => exception1 = ex);

		//Act
		NoParameterDelayedNullReferenceExceptionValueTaskWithReturn().SafeFireAndForget<bool, NullReferenceException>(ex => exception2 = ex);
		await BaseTest.NoParameterTask();
		await BaseTest.NoParameterTask();

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(exception1, Is.Not.Null);
			Assert.That(exception2, Is.Not.Null);
		});
	}
}