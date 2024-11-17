using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_IAsyncCommand : BaseAsyncCommandTest
{
	[Test]
	public void IAsyncCommand_CanExecute_InvalidReferenceParameter()
	{
		// Arrange
		IAsyncCommand<int, bool> command = new AsyncCommand<int, bool>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncCommand<string, bool> command = new AsyncCommand<string, bool>(StringParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute(true));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidReferenceParameter()
	{
		// Arrange
		IAsyncCommand<int, bool> command = new AsyncCommand<int, bool>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_CanExecute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncCommand<int, string> command = new AsyncCommand<int, string>(IntParameterTask, CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(true));
	}

	[TestCase(500)]
	[TestCase(0)]
	public async Task AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
	{
		//Arrange
		IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask);
		IAsyncCommand<int> command2 = new AsyncCommand<int, int>(IntParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task AsyncCommand_ExecuteAsync_StringParameter_Test(string? parameter)
	{
		//Arrange
		IAsyncCommand<string?> command = new AsyncCommand<string?>(StringParameterTask);
		IAsyncCommand<string?> command2 = new AsyncCommand<string?, string>(StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[Test]
	public void IAsyncCommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncCommand<int?> command = new AsyncCommand<int?>(NullableIntParameterTask, CanExecuteTrue);
		IAsyncCommand<int, int> command2 = new AsyncCommand<int, int>(IntParameterTask, CanExecuteTrue);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(command2.CanExecute(0), Is.True);
		});
	}

	[Test]
	public void IAsyncCommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncCommand<int?> command = new AsyncCommand<int?>(NullableIntParameterTask, CanExecuteFalse);
		IAsyncCommand<int, int> command2 = new AsyncCommand<int, int>(IntParameterTask, CanExecuteFalse);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.False);
			Assert.That(command2.CanExecute(0), Is.False);
		});
	}

	[Test]
	public void IAsyncCommand_NoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteTrue);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.True);
	}

	[Test]
	public void IAsyncCommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.False);
	}

	[Test]
	public void IAsyncCommand_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		IAsyncCommand command = new AsyncCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void IAsyncCommand_ExecuteAsync_ExceptionHandlingWithParameter_Test()
	{
		//Arrange
		IAsyncCommand<int> command = new AsyncCommand<int>(ParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync(0));

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}
}