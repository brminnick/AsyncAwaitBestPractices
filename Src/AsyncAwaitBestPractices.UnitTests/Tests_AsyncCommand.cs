using System.Threading;
using System.Threading.Tasks;

using NUnit.Framework;

using AsyncAwaitBestPractices.MVVM;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_AsyncCommand : BaseTest
    {
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
        public async Task AsyncCommand_ExecuteAsync_InvalidValueTypeParameter_Test()
        {
            //Arrange
            InvalidCommandParameterException actualInvalidCommandParameterException = null;
            InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(string), typeof(int));

            AsyncCommand<string> command = new AsyncCommand<string>(StringParameterTask);

            //Act
            try
            {
                await command.ExecuteAsync(500);
            }
            catch(InvalidCommandParameterException e)
            {
                actualInvalidCommandParameterException = e;
            }

            //Assert
            Assert.IsNotNull(actualInvalidCommandParameterException);
            Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException.Message);
        }

        [Test]
        public async Task AsyncCommand_ExecuteAsync_InvalidReferenceTypeParameter_Test()
        {
            //Arrange
            InvalidCommandParameterException actualInvalidCommandParameterException = null;
            InvalidCommandParameterException expectedInvalidCommandParameterException = new InvalidCommandParameterException(typeof(int), typeof(string));

            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask);

            //Act
            try
            {
                await command.ExecuteAsync("Hello World");
            }
            catch (InvalidCommandParameterException e)
            {
                actualInvalidCommandParameterException = e;
            }

            //Assert
            Assert.IsNotNull(actualInvalidCommandParameterException);
            Assert.AreEqual(expectedInvalidCommandParameterException.Message, actualInvalidCommandParameterException.Message);
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task AsyncCommand_ExecuteAsync_NoParameter_ContinueOnCapturedContext_Test(bool continueOnCapturedContext)
        {
            //Arrange
            AsyncCommand command = new AsyncCommand(NoParameterTask, continueOnCapturedContext);
            var callingThreadId = Thread.CurrentThread.ManagedThreadId;

            //Act
            await command.ExecuteAsync(null);

            var returningThreadId = Thread.CurrentThread.ManagedThreadId;

            //Assert
            Assert.AreNotEqual(callingThreadId, returningThreadId);
        }

        [TestCase(500, false)]
        [TestCase(500, true)]
        public async Task AsyncCommand_ExecuteAsync_Parameter_ContinueOnCapturedContext_Test(int parameter, bool continueOnCapturedContext)
        {
            //Arrange
            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, continueOnCapturedContext);
            var callingThreadId = Thread.CurrentThread.ManagedThreadId;

            //Act
            await command.ExecuteAsync(parameter);

            var returningThreadId = Thread.CurrentThread.ManagedThreadId;

            //Assert
            Assert.AreNotEqual(callingThreadId, returningThreadId);
        }

        [Test]
        public void AsyncCommand_Parameter_CanExecuteTrue_Test()
        {
            //Arrange
            AsyncCommand<int> command = new AsyncCommand<int>(IntParameterTask, canExecute: CanExecuteTrue);

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
