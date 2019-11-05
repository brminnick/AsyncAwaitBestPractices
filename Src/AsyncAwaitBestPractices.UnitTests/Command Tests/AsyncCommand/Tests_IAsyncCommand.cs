using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    class Tests_IAsyncCommand : BaseAsyncCommandTest
    {
        [TestCase(500)]
        [TestCase(default)]
        public async Task AsyncCommand_ExecuteAsync_IntParameter_Test(int parameter)
        {
            //Arrange
            IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [TestCase("Hello")]
        [TestCase(default)]
        public async Task AsyncCommand_ExecuteAsync_StringParameter_Test(string parameter)
        {
            //Arrange
            IAsyncCommand<string> command = new AsyncCommand<string>(StringParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [Test]
        public void IAsyncCommand_Parameter_CanExecuteTrue_Test()
        {
            //Arrange
            IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void IAsyncCommand_Parameter_CanExecuteFalse_Test()
        {
            //Arrange
            IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }

        [Test]
        public void IAsyncCommand_NoParameter_CanExecuteTrue_Test()
        {
            //Arrange
            IAsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void IAsyncCommand_NoParameter_CanExecuteFalse_Test()
        {
            //Arrange
            IAsyncCommand command = new AsyncCommand(NoParameterTask, CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }
    }
}
