using System.Threading.Tasks;
using System.Windows.Input;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests;

class Tests_ICommand_AsyncCommand : BaseAsyncCommandTest
{
	[TestCase(500)]
	[TestCase(default)]
	public async Task ICommand_Execute_IntParameter_Test(int parameter)
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(BaseTest.IntParameterTask);
		ICommand command2 = new AsyncCommand<int, int>(BaseTest.IntParameterTask);

		//Act
		command.Execute(parameter);
		await BaseTest.IntParameterTask(parameter);

		command2.Execute(parameter);
		await BaseTest.IntParameterTask(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task ICommand_Execute_StringParameter_Test(string parameter)
	{
		//Arrange
		ICommand command = new AsyncCommand<string>(BaseTest.StringParameterTask);
		ICommand command2 = new AsyncCommand<string, string>(BaseTest.StringParameterTask);

		//Act
		command.Execute(parameter);
		await BaseTest.StringParameterTask(parameter);

		command2.Execute(parameter);
		await BaseTest.StringParameterTask(parameter);
		//Assert

	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

		ICommand command = new AsyncCommand<string, string>(BaseTest.StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

		ICommand command = new AsyncCommand<string>(BaseTest.StringParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(Delay));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncCommand<int, int>(BaseTest.IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_InvalidReferenceTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));


		ICommand command = new AsyncCommand<int>(BaseTest.IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_TwoParameters_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncCommand<int, int>(BaseTest.IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_ExecuteAsync_ValueTypeParameter_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));


		ICommand command = new AsyncCommand<int>(BaseTest.IntParameterTask);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int?>(BaseTest.NullableIntParameterTask, BaseTest.CanExecuteTrue);
		ICommand command2 = new AsyncCommand<int, int>(BaseTest.IntParameterTask, BaseTest.CanExecuteTrue);

		//Act

		//Assert
		Assert.True(command.CanExecute(null));
		Assert.True(command2.CanExecute(0));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int?>(BaseTest.NullableIntParameterTask, BaseTest.CanExecuteFalse);
		ICommand command2 = new AsyncCommand<int, int>(BaseTest.IntParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
		Assert.False(command2.CanExecute(0));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncCommand<int, int>(BaseTest.IntParameterTask, BaseTest.CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_Parameter_ExecuteNull_Test()
	{
		//Arrange
		InvalidCommandParameterException? actualInvalidCommandParameterException = null;
		InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int));

		ICommand command = new AsyncCommand<int, int>(BaseTest.IntParameterTask, BaseTest.CanExecuteFalse);

		//Act
		actualInvalidCommandParameterException = Assert.Throws<InvalidCommandParameterException>(() => command.Execute(null));

		//Assert
		Assert.IsNotNull(actualInvalidCommandParameterException);
		Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException?.Message);
	}

	[Test]
	public void ICommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand(BaseTest.NoParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteDynamic_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(BaseTest.IntParameterTask, BaseTest.CanExecuteDynamic);

		//Act

		//Assert
		Assert.True(command.CanExecute(true));
		Assert.False(command.CanExecute(false));
	}

	[Test]
	public void ICommand_Parameter_CanExecuteChanged_Test()
	{
		//Arrange
		ICommand command = new AsyncCommand<int>(BaseTest.IntParameterTask, BaseTest.CanExecuteDynamic);

		//Act

		//Assert
		Assert.True(command.CanExecute(true));
		Assert.False(command.CanExecute(false));
	}
}