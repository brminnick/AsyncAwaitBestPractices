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
		IAsyncValueCommand<int, bool> command = new AsyncValueCommand<int, bool>(IntParameterTask, BaseTest.CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncValueCommand<string, bool> command = new AsyncValueCommand<string, bool>(StringParameterTask, BaseTest.CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute(true));
	}

	[Test]
	public void IAsyncCommand_Execute_InvalidReferenceParameter()
	{
		// Arrange
		IAsyncValueCommand<int, bool> command = new AsyncValueCommand<int, bool>(IntParameterTask, BaseTest.CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.Execute("Hello World"));
	}

	[Test]
	public void IAsyncCommand_CanExecute_InvalidValueTypeParameter()
	{
		// Arrange
		IAsyncValueCommand<int, string> command = new AsyncValueCommand<int, string>(IntParameterTask, BaseTest.CanExecuteTrue);

		// Act

		// Assert
		Assert.Throws<InvalidCommandParameterException>(() => command.CanExecute(true));
	}

	[TestCase(500)]
	[TestCase(default)]
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
	public async Task AsyncValueCommand_ExecuteAsync_StringParameter_Test(string parameter)
	{
		//Arrange
		IAsyncValueCommand<string> command = new AsyncValueCommand<string>(StringParameterTask);
		IAsyncValueCommand<string, string> command2 = new AsyncValueCommand<string, string>(StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[Test]
	public void IAsyncValueCommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncValueCommand<int?> command = new AsyncValueCommand<int?>(NullableIntParameterTask, BaseTest.CanExecuteTrue);
		IAsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, BaseTest.CanExecuteTrue);

		//Act

		//Assert
		Assert.IsTrue(command.CanExecute(null));
		Assert.IsTrue(command2.CanExecute(0));
	}

	[Test]
	public void IAsyncValueCommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncValueCommand<int?> command = new AsyncValueCommand<int?>(NullableIntParameterTask, BaseTest.CanExecuteFalse);
		IAsyncValueCommand<int, int> command2 = new AsyncValueCommand<int, int>(IntParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
		Assert.False(command2.CanExecute(0));
	}

	[Test]
	public void IAsyncValueCommand_NoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, BaseTest.CanExecuteTrue);

		//Act

		//Assert
		Assert.IsTrue(command.CanExecute(null));
	}

	[Test]
	public void IAsyncValueCommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
	}
}