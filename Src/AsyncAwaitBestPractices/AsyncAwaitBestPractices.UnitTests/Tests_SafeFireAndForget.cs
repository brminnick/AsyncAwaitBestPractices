using System;
using System.Threading.Tasks;
using AsyncAwaitBestPractices;
using NUnit.Framework;

namespace AsyncAwaitBestPractices.UnitTests
{
    public class Tests_SafeFireAndForget : BaseTest
    {
        [Test]
        public async Task SafeFireAndForget()
        {
            //Arrange
            Exception exception = null;

            //Act
            try
            {
                NoParameterTask().SafeFireAndForget();
                await NoParameterTask();
                await NoParameterTask();
            }
            catch (Exception e)
            {
                exception = e;
            }

            //Assert
            Assert.IsNull(exception);
        }

        [Test]
        public async Task SafeFireAndForget_UnhandledException()
        {
            //Arrange
            Exception exception = null;

            //Act
            try
            {
                NoParameterExceptionTask().SafeFireAndForget();
                await NoParameterTask();
                await NoParameterTask();
            }
            catch(Exception e)
            {
                exception = e;
            }

            //Assert
            Assert.IsNotNull(exception);
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
