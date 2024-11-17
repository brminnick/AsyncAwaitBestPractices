using System;
using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_ICommand_AsyncCommand : BaseAsyncCommandTest
{
	[TestCase(500)]
	[TestCase(0)]
	public async Task ICommand_Execute_IntParameter_Test(int parameter)
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(IntParameterTask);
		ICommand command2 = new AsyncCommand<int, int>(IntParameterTask);

		//Act
		command.Execute(parameter);
		await IntParameterTask(parameter);

		command2.Execute(parameter);
		await IntParameterTask(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(null)]
	public async Task ICommand_Execute_StringParameter_Test(string? parameter)
	{
		//Arrange
		ICommand command = new AsyncCommand<string>(StringParameterTask);
		ICommand command2 = new AsyncCommand<string, string>(StringParameterTask);

		//Act
		command.Execute(parameter);
		await StringParameterTask(parameter);

		command2.Execute(parameter);
		await StringParameterTask(parameter);
		//Assert

	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new(typeof(string), typeof(int));

		ICommand command = new AsyncCommand<string, string>(StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

		ICommand command = new AsyncCommand<string>(StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncCommand<int, int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncCommand<int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncCommand<int, int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncCommand<int>(IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int?>(NullableIntParameterTask, CanExecuteTrue);
		ICommand command2 = new AsyncCommand<int, int>(IntParameterTask, CanExecuteTrue);

		Assert.Multiple(() =>
		{
			//Act

			//Assert
			Assert.That(command.CanExecute(null), Is.True);
			Assert.That(command2.CanExecute(0), Is.True);
		});
	}

	[Test]
	public void ICommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int?>(NullableIntParameterTask, CanExecuteFalse);
		ICommand command2 = new AsyncCommand<int, int>(IntParameterTask, CanExecuteFalse);

		Assert.Multiple(() =>
		{
			//Act

			//Assert
			Assert.That(command.CanExecute(null), Is.False);
			Assert.That(command2.CanExecute(0), Is.False);
		});
	}

	[Test]
	public void ICommand_Parameter_CanExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncCommand<int, int>(IntParameterTask, CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(null));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_Parameter_ExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncCommand<int, int>(IntParameterTask, CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.That(actualInvalidCommandParameterException, Is.Not.Null);
		Assert.That(actualInvalidCommandParameterException?.Message, Is.EqualTo(expectedInvalidCommandParameterException.Message));
	}

	[Test]
	public void ICommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand(NoParameterTask, CanExecuteFalse);

		//Act

		//Assert
		Assert.That(command.CanExecute(null), Is.False);
	}

	[Test]
	public void ICommand_Parameter_CanExecuteDynamic_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(IntParameterTask, CanExecuteDynamic);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(true), Is.True);
			Assert.That(command.CanExecute(false), Is.False);
		});
	}

	[Test]
	public void ICommand_Parameter_CanExecuteChanged_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(IntParameterTask, CanExecuteDynamic);

		Assert.Multiple(() =>
		{
			//Assert
			Assert.That(command.CanExecute(true), Is.True);
			Assert.That(command.CanExecute(false), Is.False);
		});
	}

	[Test]
	public void ICommand_ExecuteAsync_ExceptionHandling_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand(NoParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await ((AsyncCommand)command).ExecuteAsync());

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}

	[Test]
	public void ICommand_ExecuteAsync_ExceptionHandlingWithParameter_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(ParameterImmediateNullReferenceExceptionTask, onException: HandleException);
		Exception? caughtException = null;

		//Act
		caughtException = Assert.ThrowsAsync<NullReferenceException>(async () => await ((AsyncCommand<int>)command).ExecuteAsync(0));

		//Assert
		Assert.That(caughtException, Is.Not.Null);

		void HandleException(Exception ex) => caughtException = ex;
	}
}