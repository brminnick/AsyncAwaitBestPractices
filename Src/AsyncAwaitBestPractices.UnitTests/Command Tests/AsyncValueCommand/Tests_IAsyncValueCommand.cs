using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    class Tests_IAsyncValueCommand : BaseAsyncValueCommandTest
    {
        [TestCase(500)]
        [TestCase(default)]
        public async Task AsyncValueCommand_ExecuteAsync_IntParameter_Test(int parameter)
        {
            //Arrange
            IAsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [TestCase("Hello")]
        [TestCase(default)]
        public async Task AsyncValueCommand_ExecuteAsync_StringParameter_Test(string parameter)
        {
            //Arrange
            IAsyncValueCommand<string> command = new AsyncValueCommand<string>(StringParameterTask);

            //Act
            await command.ExecuteAsync(parameter);

            //Assert

        }

        [Test]
        public void IAsyncValueCommand_Parameter_CanExecuteTrue_Test()
        {
            //Arrange
            IAsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void IAsyncValueCommand_Parameter_CanExecuteFalse_Test()
        {
            //Arrange
            IAsyncValueCommand<int> command = new AsyncValueCommand<int>(IntParameterTask, CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }

        [Test]
        public void IAsyncValueCommand_NoParameter_CanExecuteTrue_Test()
        {
            //Arrange
            IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void IAsyncValueCommand_NoParameter_CanExecuteFalse_Test()
        {
            //Arrange
            IAsyncValueCommand command = new AsyncValueCommand(NoParameterTask, CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }
    }
}
