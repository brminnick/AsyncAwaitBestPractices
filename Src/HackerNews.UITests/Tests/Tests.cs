using HackerNews.Shared;
using NUnit.Framework;
using Xamarin.UITest;

namespace HackerNews.UITests
{
    public class Tests : BaseTest
    {

        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void AppLaunches()
        {

        }

        [Test]
        public void DownloadTopStories()
        {
            //Arrange
            int actualNumberOfStories;
            const int expectedNumberOfStories = StoriesConstants.NumberOfStories;

            //Act
            actualNumberOfStories = NewsPage.GetStoryList().Count;

            //Assert
            Assert.AreEqual(expectedNumberOfStories, actualNumberOfStories);
        }
    }
}
