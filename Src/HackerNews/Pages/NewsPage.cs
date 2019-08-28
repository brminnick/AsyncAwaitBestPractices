using Xamarin.Forms;
using HackerNews.Shared;
using Xamarin.Essentials;

namespace HackerNews
{
    class NewsPage : BaseContentPage<NewsViewModel_GoodAsyncAwaitPractices>
    {
        public NewsPage() : base(PageTitleConstants.NewsPageTitle)
        {
            ViewModel.ErrorOcurred += HandleErrorOcurred;

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

        void HandleErrorOcurred(object sender, string e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", e, "OK"));

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView && e?.Item is StoryModel storyTapped)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    listView.SelectedItem = null;

                    var browserOptions = new BrowserLaunchOptions
                    {
                        PreferredControlColor = ColorConstants.BrowserNavigationBarTextColor,
                        PreferredToolbarColor = ColorConstants.BrowserNavigationBarBackgroundColor
                    };

                    await Browser.OpenAsync(storyTapped.Url, browserOptions);
                });
            }
        }
    }
}
