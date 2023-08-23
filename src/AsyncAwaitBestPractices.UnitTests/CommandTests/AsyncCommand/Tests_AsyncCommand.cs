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
	[TestCase(default)]
	public async Task AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(BaseTest.IntParameterTask);
		AsyncCommand<int, int> command2 = new AsyncCommand<int, int>(BaseTest.IntParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[TestCase("Hello")]
	[TestCase(default)]
	public async Task AsyncCommand_ExecuteAsync_StringParameter_Test(string parameter)
	{
		//Arrange
		AsyncCommand<string> command = new AsyncCommand<string>(BaseTest.StringParameterTask);
		AsyncCommand<string, string> command2 = new AsyncCommand<string, string>(BaseTest.StringParameterTask);

		//Act
		await command.ExecuteAsync(parameter);
		await command2.ExecuteAsync(parameter);

		//Assert

	}

	[Test]
	public void AsyncCommand_Parameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(BaseTest.IntParameterTask, BaseTest.CanExecuteTrue);
		AsyncCommand<int, bool> command2 = new AsyncCommand<int, bool>(BaseTest.IntParameterTask, BaseTest.CanExecuteTrue);

		//Act

		//Assert
		Assert.IsTrue(command.CanExecute(null));
		Assert.IsTrue(command2.CanExecute(true));
	}

	[Test]
	public void AsyncCommand_Parameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncCommand<int> command = new AsyncCommand<int>(BaseTest.IntParameterTask, BaseTest.CanExecuteFalse);
		AsyncCommand<int, int> command2 = new AsyncCommand<int, int>(BaseTest.IntParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert

		Assert.False(command.CanExecute(null));
		Assert.False(command2.CanExecute(0));
	}

	[Test]
	public void AsyncCommand_NoParameter_CanExecuteTrue_Test()
	{
		//Arrange
		AsyncCommand command = new AsyncCommand(BaseTest.NoParameterTask, BaseTest.CanExecuteTrue);

		//Act

		//Assert

		Assert.IsTrue(command.CanExecute(null));
	}

	[Test]
	public void AsyncCommand_NoParameter_CanExecuteFalse_Test()
	{
		//Arrange
		AsyncCommand command = new AsyncCommand(BaseTest.NoParameterTask, BaseTest.CanExecuteFalse);

		//Act

		//Assert
		Assert.False(command.CanExecute(null));
	}


	[Test]
	public void AsyncCommand_CanExecuteChanged_Test()
	{
		//Arrange
		bool canCommandExecute = false;
		bool didCanExecuteChangeFire = false;

		AsyncCommand command = new AsyncCommand(BaseTest.NoParameterTask, commandCanExecute);
		command.CanExecuteChanged += handleCanExecuteChanged;

		void handleCanExecuteChanged(object? sender, EventArgs e) => didCanExecuteChangeFire = true;
		bool commandCanExecute(object? parameter) => canCommandExecute;


		Assert.False(command.CanExecute(null));


		//Act
		canCommandExecute = true;

		//Assert

		Assert.True(command.CanExecute(null));

		Assert.False(didCanExecuteChangeFire);

		//Act
		command.RaiseCanExecuteChanged();

		//Assert
		Assert.True(didCanExecuteChangeFire);
		Assert.True(command.CanExecute(null));
	}
}