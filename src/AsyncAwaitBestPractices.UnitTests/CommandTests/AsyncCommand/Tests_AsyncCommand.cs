using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_AsyncCommand : BaseAsyncCommandTest
{
	[Test]
	public void AsyncCommand_NullExecuteParameter()
	{
		//Arrange

		//Act

		//Assert
#pragma warning disable CS8625 //Cannot convert null literal to non-nullable reference type
		Assert.Throws<ArgumentNullException>(() => new AsyncCommand(null));
		Assert.Throws<ArgumentNullException>(() => new AsyncCommand<string>(null));
		Assert.Throws<ArgumentNullException>(() => new AsyncCommand<string, string?>(null));
#pragma warning restore CS8625
	}

	[TestCase(500)]
	[TestCase(0)]
	public async Task AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask);
		AsyncCommand<int, int> command2 = new AsyncCommand<int, int>(IntParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(null)]
	public async Task AsyncCommand_ExecuteAsync_StringParameter_Test(string? parameter)
	{
		//Arrange
		AsyncCommand<string?> command = new(StringParameterTask);
		AsyncCommand<string?, string> command2 = new(StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[Test]
	public void AsyncCommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, CanExecuteTrue);
		AsyncCommand<int, bool> command2 = new AsyncCommand<int, bool>(IntParameterTask, CanExecuteTrue);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(command2.CanExecute(true), Is.True);
		});
	}

	[Test]
	public void AsyncCommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, CanExecuteFalse);
		AsyncCommand<int, int> command2 = new AsyncCommand<int, int>(IntParameterTask, CanExecuteFalse);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(null), Is.False);
			Assert.That(command2.CanExecute(0), Is.False);
		});
	}

	[Test]
	public void AsyncCommand_NoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteTrue);

		//Act

		//Assert

		Assert.That(command.CanExecute(null), Is.True);
	}

	[Test]
	public void AsyncCommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.False);
	}


	[Test]
	public void AsyncCommand_CanExecuteChanged_Test()
	{
		//Arrange
		bool canCommandExecute = false;
		bool didCanExecuteChangeFire = false;

		AsyncCommand command = new AsyncCommand(NoParameterTask, commandCanExecute);
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
	public void AsyncCommand_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		var command = new AsyncCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(() => command.ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void AsyncCommand_ExecuteAsync_ExceptionHandlingWithParameter_Test()
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