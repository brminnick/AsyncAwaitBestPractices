using System;
using Xamarin.Forms;
namespace HackerNews
{
    public class NewsPage : BaseContentPage<NewsViewModel_GoodAsyncAwaitPractices>
    {
        #region Constant Fields
        readonly ListView _storiesListView;
        #endregion

        public NewsPage() : base("Top Stories")
        {
            _storiesListView = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(StoryViewCell)),
                IsPullToRefreshEnabled = true,
            };
			_storiesListView.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.TopStoryList));
			_storiesListView.SetBinding(ListView.IsRefreshingProperty, nameof(ViewModel.IsListRefreshing));
            _storiesListView.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.RefreshCommand));

            Content = _storiesListView;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(_storiesListView.BeginRefresh);
        }

        protected override void SubscribeEventHandlers()
        {
            _storiesListView.ItemTapped += HandleItemTapped;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _storiesListView.ItemTapped -= HandleItemTapped;
        }

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;
            var storyTapped = e.Item as StoryModel;


            Device.BeginInvokeOnMainThread(() =>
            {
                listView.SelectedItem = null;
                Device.OpenUri(new Uri(storyTapped.Url));
            });
        }
    }
}
