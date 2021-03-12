using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            AndroidApp androidApp => (bool)(androidApp.Query(x => x.Class("RefreshViewRenderer").Invoke("isRefreshing")).FirstOrDefault() ?? false),
            iOSApp iosApp => iosApp.Query(x => x.Class("UIRefreshControl")).Any(),
            _ => throw new NotSupportedException(),
        };

        public bool IsBrowserOpen => App switch
        {
            iOSApp iOSApp => iOSApp.Query(x => x.Class("SFSafariView")).Any(),
            _ => throw new NotSupportedException("Browser Can Only Be Verified on iOS")
        };

        public override async Task WaitForPageToLoad()
        {
            await base.WaitForPageToLoad().ConfigureAwait(false);

            try
            {
                await WaitForActivityIndicator(5).ConfigureAwait(false);
            }
            catch
            {
            }

            await WaitForNoActivityIndicator().ConfigureAwait(false);
        }

        public async Task WaitForActivityIndicator(int timeoutInSeconds = 25)
        {
            int counter = 0;
            while (!IsRefreshActivityIndicatorDisplayed && counter < timeoutInSeconds)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                counter++;

                if (counter >= timeoutInSeconds)
                    throw new Exception($"Loading the list took longer than {timeoutInSeconds}");
            }
        }

        public async Task WaitForNoActivityIndicator(int timeoutInSeconds = 25)
        {
            int counter = 0;
            while (IsRefreshActivityIndicatorDisplayed && counter < timeoutInSeconds)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
                counter++;

                if (counter >= timeoutInSeconds)
                    throw new Exception($"Loading the list took longer than {timeoutInSeconds}");
            }
        }

        public IReadOnlyList<StoryModel> GetStoryList() =>
            App.InvokeBackdoorMethod<IReadOnlyList<StoryModel>>(BackdoorMethodConstants.GetStoriesAsBase64String);
    }
}
