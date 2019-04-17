using System;
using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using AsyncAwaitBestPractices.MVVM;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_AsyncCommand : BaseTest
    {
        [Test]
        public void AsyncCommand_NullExecuteParameter()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => new AsyncCommand(null));
        }

        [Test]
        public void AsyncCommandT_NullExecuteParameter()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<ArgumentNullException>(() => new AsyncCommand<object>(null));
        }

        [TestCase(500)]
        [TestCase(default)]
        public async Task AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
        {
            //Arrange
            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [TestCase("Hello")]
        [TestCase(default)]
        public async Task AsyncCommand_ExecuteAsync_StringParameter_Test(string parameter)
        {
            //Arrange
            AsyncCommand<string> command = new AsyncCommand<string>(StringParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [Test]
        public void AsyncCommand_Parameter_CanExecuteTrue_Test()
        {
            //Arrange
            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void AsyncCommand_Parameter_CanExecuteFalse_Test()
        {
            //Arrange
            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }

        [Test]
        public void AsyncCommand_NoParameter_CanExecuteTrue_Test()
        {
            //Arrange
            AsyncCommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void AsyncCommand_NoParameter_CanExecuteFalse_Test()
        {
            //Arrange
            AsyncCommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }
    }
}
