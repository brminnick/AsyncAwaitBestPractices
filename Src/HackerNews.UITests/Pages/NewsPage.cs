using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HackerNews.Shared;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace HackerNews.UITests
{
    public class NewsPage : BasePage
    {
        public NewsPage(IApp app) : base(app, PageTitleConstants.NewsPageTitle)
        {
        }

        public bool IsRefreshActivityIndicatorDisplayed => App switch
        {
            AndroidApp androidApp => (bool)(androidApp.Query(x => x.Class("ListViewRenderer_SwipeRefreshLayoutWithFixedNestedScrolling").Invoke("isRefreshing")).FirstOrDefault() ?? false),
            iOSApp iosApp => iosApp.Query(x => x.Class("UIRefreshControl")).Any(),

            _ => throw new NotSupportedException(),
        };

        public bool IsBrowserOpen => App switch
        {
            iOSApp iOSApp => iOSApp.Query(x => x.Class("SFSafariView")).Any(),
            _ => throw new NotSupportedException("Browser Can Only Be Verified on iOS")
        };

        public override void WaitForPageToLoad()
        {
            base.WaitForPageToLoad();

            try
            {
                WaitForActivityIndicator(5);
            }
            catch
            {
            }


            WaitForNoActivityIndicator();
        }

        public void WaitForActivityIndicator(int timeoutInSeconds = 25)
        {
            int counter = 0;
            while (!IsRefreshActivityIndicatorDisplayed && counter < timeoutInSeconds)
            {
                Thread.Sleep(1000);
                counter++;

                if (counter >= timeoutInSeconds)
                    throw new Exception($"Loading the list took longer than {timeoutInSeconds}");
            }
        }

        public void WaitForNoActivityIndicator(int timeoutInSeconds = 25)
        {
            int counter = 0;
            while (IsRefreshActivityIndicatorDisplayed && counter < timeoutInSeconds)
            {
                Thread.Sleep(1000);
                counter++;

                if (counter >= timeoutInSeconds)
                    throw new Exception($"Loading the list took longer than {timeoutInSeconds}");
            }
        }

        public List<StoryModel> GetStoryList()
        {
            var storyListAsBase64String = App switch
            {
                iOSApp iosApp => iosApp.Invoke("getStoriesAsBase64String:", "").ToString(),
                AndroidApp androidApp => androidApp.Invoke("GetStoriesAsBase64String").ToString(),
                _ => throw new Exception("Platform Not Supported"),
            };

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoryModel>>(storyListAsBase64String);
        }
    }
}
