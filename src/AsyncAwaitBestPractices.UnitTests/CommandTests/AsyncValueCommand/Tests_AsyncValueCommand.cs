using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_AsyncValueCommand : BaseAsyncValueCommandTest
{
	[Test]
	public void AsyncValueCommandNullExecuteParameter()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		Assert.Throws<ArgumentNullException>(() => new AsyncValueCommand(null));
		Assert.Throws<ArgumentNullException>(() => new AsyncValueCommand<int>(null));
		Assert.Throws<ArgumentNullException>(() => new AsyncValueCommand<int, int>(null));
#pragma warning restore CS8625
	}

	[TestCase(500)]
	[TestCase(0)]
	public async Task AsyncValueCommandExecuteAsync_IntParameter_Test(int parameter)
	{
		//Arrange
		AsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask);
		AsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert
	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task AsyncValueCommandExecuteAsync_StringParameter_Test(string? parameter)
	{
		//Arrange
		AsyncValueCommand<string?> command = new(StringParameterTask);
		AsyncValueCommand<string?, string> command2 = new(StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert
	}

	[Test]
	public void AsyncValueCommandParameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteTrue);
		AsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteTrue);

		Assert.Multiple(() =>
		{
			//Act

			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(command2.CanExecute(0), Is.True);
		});
	}

	[Test]
	public void AsyncValueCommandParameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteFalse);
		AsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, CanExecuteFalse);

		Assert.Multiple(() =>
		{
			//Act

			//Assert
			Assert.That(command.CanExecute(null), Is.False);
			Assert.That(command2.CanExecute(0), Is.False);
		});
	}

	[Test]
	public void AsyncValueCommandNoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteTrue);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.True);
	}

	[Test]
	public void AsyncValueCommandNoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.False);
	}


	[Test]
	public void AsyncValueCommandCanExecuteChanged_Test()
	{
		//Arrange
		bool canCommandExecute = false;
		bool didCanExecuteChangeFire = false;

		AsyncValueCommand command = new AsyncValueCommand(NoParameterTask, commandCanExecute);
		command.CanExecuteChanged += handleCanExecuteChanged;

		Assert.That(command.CanExecute(null), Is.False);

		//Act
		canCommandExecute = true;

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(didCanExecuteChangeFire, Is.False);
		});

		//Act
		command.RaiseCanExecuteChanged();

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(didCanExecuteChangeFire, Is.True);
			Assert.That(command.CanExecute(null), Is.True);
		});

		void handleCanExecuteChanged(object? sender, EventArgs e) => didCanExecuteChangeFire = true;

		bool commandCanExecute(object? parameter) => canCommandExecute;
	}

	[Test]
	public void AsyncValueCommand_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		AsyncValueCommand command = new AsyncValueCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void AsyncValueCommand_ExecuteAsync_ExceptionHandlingWithParameter_Test()
	{
		//Arrange
		AsyncValueCommand<int> command = new AsyncValueCommand<int>(ParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await command.ExecuteAsync(0));

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}
}