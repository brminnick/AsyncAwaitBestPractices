using System;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_SafeFireAndForget : BaseTest
    {
        [Test]
        public void SafeFireAndForget_UnhandledException()
        {
            //Arrange

            //Act

            //Assert
            Assert.Throws<Exception>(() =>
            {
                NoParameterExceptionTask().SafeFireAndForget();
                Thread.Sleep(Delay * 2);
            });
        }


        [Test]
        public async Task SafeFireAndForget_HandledException()
        {
            //Arrange
            Exception exception = null;

            //Act
            NoParameterExceptionTask().SafeFireAndForget(onException: ex => exception = ex);
            await NoParameterTask();
            await NoParameterTask();

            //Assert
            Assert.IsNotNull(exception);
        }
    }
}
