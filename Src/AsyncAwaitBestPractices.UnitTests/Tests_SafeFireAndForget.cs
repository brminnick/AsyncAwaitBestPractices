using System;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_SafeFireAndForget : BaseTest
    {
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
