using Xamarin.Forms;
using HackerNews.Shared;
using Xamarin.Essentials;
using System;

namespace HackerNews
{
    class NewsPage : BaseContentPage<NewsViewModel_GoodAsyncAwaitPractices>
    {
        public NewsPage() : base(PageTitleConstants.NewsPageTitle)
        {
            ViewModel.ErrorOccurred += HandleErrorOccurred;

            var storiesListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(StoryTextCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.FromHex("F6F6EF"),
                SeparatorVisibility = SeparatorVisibility.None
            };
            storiesListView.ItemTapped += HandleItemTapped;
            storiesListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.TopStoryList));
            storiesListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsListRefreshing));
            storiesListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            Content = storiesListView;
        }

        void HandleErrorOccurred(object sender, string e) =>
            MainThread.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", e, "OK"));

        void HandleItemTapped(object sender, ItemTappedEventArgs e) => MainThread.BeginInvokeOnMainThread(async () =>
        {
            var listView = (ListView)sender;
            var storyTapped = (StoryModel)e.Item;

            listView.SelectedItem = null;

            if (Uri.IsWellFormedUriString(storyTapped.Url, UriKind.Absolute))
            {
                var browserOptions = new BrowserLaunchOptions
                {
                    PreferredControlColor = ColorConstants.BrowserNavigationBarTextColor,
                    PreferredToolbarColor = ColorConstants.BrowserNavigationBarBackgroundColor
                };

                await Browser.OpenAsync(storyTapped.Url, browserOptions);
            }
            else
            {
                await DisplayAlert("No Website", "Ask HN articles do not contain a URL", "OK");
            }
        });
    }
}
