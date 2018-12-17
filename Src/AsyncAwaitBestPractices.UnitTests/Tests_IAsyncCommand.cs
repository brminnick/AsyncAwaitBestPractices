using System.Threading;
using System.Threading.Tasks;
using AsyncAwaitBestPractices.MVVM;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_IAsyncCommand : BaseTest
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

        [TestCase(false)]
        [TestCase(true)]
        public async Task IAsyncCommand_ExecuteAsync_NoParameter_ContinueOnCapturedContext_Test(bool continueOnCapturedContext)
        {
            //Arrange
            IAsyncCommand command = new AsyncCommand(NoParameterTask, continueOnCapturedContext: continueOnCapturedContext);
            var callingThreadId = Thread.CurrentThread.ManagedThreadId;

            //Act
            await command.ExecuteAsync();

            var returningThreadId = Thread.CurrentThread.ManagedThreadId;

            //Assert
            Assert.AreNotEqual(callingThreadId, returningThreadId);
        }

        [TestCase(500, false)]
        [TestCase(500, true)]
        public async Task IAsyncCommand_ExecuteAsync_Parameter_ContinueOnCapturedContext_Test(int parameter, bool continueOnCapturedContext)
        {
            //Arrange
            IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, continueOnCapturedContext: continueOnCapturedContext);
            var callingThreadId = Thread.CurrentThread.ManagedThreadId;

            //Act
            await command.ExecuteAsync(parameter);

            var returningThreadId = Thread.CurrentThread.ManagedThreadId;

            //Assert
            Assert.AreNotEqual(callingThreadId, returningThreadId);
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
            IAsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }

        [Test]
        public void IAsyncCommand_NoParameter_CanExecuteTrue_Test()
        {
            //Arrange
            IAsyncCommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteTrue);

            //Act

            //Assert
            Assert.IsTrue(command.CanExecute(null));
        }

        [Test]
        public void IAsyncCommand_NoParameter_CanExecuteFalse_Test()
        {
            //Arrange
            IAsyncCommand command = new AsyncCommand(NoParameterTask, canExecute: CanExecuteFalse);

            //Act

            //Assert
            Assert.False(command.CanExecute(null));
        }
    }
}
