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

        public bool IsRefreshActivityIndicatorDisplayed
        {
            get
            {
                switch (App)
                {
                    case AndroidApp androidApp:
                        return (bool)App.Query(x => x.Class("SwipeRefreshLayout").Invoke("isRefreshing")).FirstOrDefault();

                    case iOSApp iosApp:
                        return App.Query(x => x.Class("UIRefreshControl")).Any();

                    default:
                        throw new NotSupportedException();
                }
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

        public override void WaitForPageToLoad()
        {
            base.WaitForPageToLoad();

            WaitForNoActivityIndicator();
        }

        public List<StoryModel> GetStoryList()
        {
            string storyListAsBase64String;

            switch (App)
            {
                case iOSApp iosApp:
                    storyListAsBase64String = iosApp.Invoke("getStoriesAsBase64String:", "").ToString();
                    break;
                case AndroidApp androidApp:
                    storyListAsBase64String = androidApp.Invoke("GetStoriesAsBase64String").ToString();
                    break;
                default:
                    throw new Exception("Platform Not Supported");
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<StoryModel>>(storyListAsBase64String);
        }
    }
}
