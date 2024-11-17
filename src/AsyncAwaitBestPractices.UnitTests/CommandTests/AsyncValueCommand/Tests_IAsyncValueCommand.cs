using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_IAsyncValueCommand : BaseAsyncValueCommandTest
{
	[Test]
	public void IAsyncCommand_CanExecute_InvalidReferenceParameter()
	{
		// Arrange
		IAsyncValueCommand<int, bool> command = new AsyncValueCommand<int, bool>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncValueCommand<string, bool> command = new AsyncValueCommand<string, bool>(StringParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute(true));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidReferenceParameter()
	{
		// Arrange
		IAsyncValueCommand<int, bool> command = new AsyncValueCommand<int, bool>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_CanExecute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncValueCommand<int, string> command = new AsyncValueCommand<int, string>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(true));
	}

	[TestCase(500)]
	[TestCase(0)]
	public async Task AsyncValueCommand_ExecuteAsync_IntParameter_Test(int parameter)
	{
		//Arrange
		IAsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask);
		IAsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task AsyncValueCommand_ExecuteAsync_StringParameter_Test(string? parameter)
	{
		//Arrange
		IAsyncValueCommand<string?> command = new AsyncValueCommand<string?>(StringParameterTask);
		IAsyncValueCommand<string?, string> command2 = new AsyncValueCommand<string?, string>(StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[Test]
	public void IAsyncValueCommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncValueCommand<int?> command = new AsyncValueCommand<int?>(NullableIntParameterTask, CanExecuteTrue);
		IAsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteTrue);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(command2.CanExecute(0), Is.True);
		});
	}

	[Test]
	public void IAsyncValueCommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncValueCommand<int?> command = new AsyncValueCommand<int?>(NullableIntParameterTask, CanExecuteFalse);
		IAsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteFalse);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.False);
			Assert.That(command2.CanExecute(0), Is.False);
		});
	}

	[Test]
	public void IAsyncValueCommand_NoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteTrue);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.True);
	}

	[Test]
	public void IAsyncValueCommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.False);
	}

	[Test]
	public void IAsyncValueCommand_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		IAsyncValueCommand command = new AsyncValueCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void IAsyncValueCommand_ExecuteAsync_ExceptionHandlingWithParameter_Test()
	{
		//Arrange
		IAsyncValueCommand<int> command = new AsyncValueCommand<int>(ParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync(0));

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}
}