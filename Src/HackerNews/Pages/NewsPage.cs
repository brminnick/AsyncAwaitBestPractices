using Xamarin.Forms;
using HackerNews.Shared;

namespace HackerNews
{
    public class NewsPage : BaseContentPage<NewsViewModel_GoodAsyncAwaitPractices>
    {
        #region Constant Fields
        readonly ListView _storiesListView;
        #endregion

        public NewsPage() : base(PageTitleConstants.NewsPageTitle)
        {
            ViewModel.ErrorOcurred += HandleErrorOcurred;

            _storiesListView = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(StoryTextCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = Color.FromHex("F6F6EF"),
                SeparatorVisibility = SeparatorVisibility.None
            };
            _storiesListView.ItemTapped += HandleItemTapped;
            _storiesListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.TopStoryList));
            _storiesListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsListRefreshing));
            _storiesListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            Content = _storiesListView;
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
                    await DependencyService.Get<IBrowserServices>()?.OpenBrowser(storyTapped?.Url);
                });
            }
        }
    }
}
