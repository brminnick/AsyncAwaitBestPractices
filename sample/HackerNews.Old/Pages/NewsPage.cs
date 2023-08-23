using System;
using System.Linq;
using HackerNews.Shared;
using Xamarin.CommunityToolkit.Markup;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HackerNews;

class NewsPage : BaseContentPage<NewsViewModel_GoodAsyncAwaitPractices>
{
	public NewsPage() : base(PageTitleConstants.NewsPageTitle)
	{
		ViewModel.ErrorOccurred += HandleErrorOccurred;

		Content = new RefreshView
		{
			RefreshColor = Color.Black,

			Content = new CollectionView
			{
				BackgroundColor = Color.FromHex("F6F6EF"),
				SelectionMode = SelectionMode.Single,
				ItemTemplate = new StoryDataTemplate(),

			}.Bind(CollectionView.ItemsSourceProperty, nameof(ViewModel.TopStoryList))
			 .Invoke(collectionView => collectionView.SelectionChanged += HandleSelectionChanged)

		}.Bind(RefreshView.IsRefreshingProperty, nameof(ViewModel.IsListRefreshing))
		 .Bind(RefreshView.CommandProperty, nameof(ViewModel.RefreshCommand));
	}

	async void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var listView = (CollectionView)sender;

		listView.SelectedItem = null;

		if (e.CurrentSelection.FirstOrDefault() is StoryModel storyTapped)
		{
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
		}
	}

	async void HandleErrorOccurred(object sender, string e) =>
		await MainThread.InvokeOnMainThreadAsync(() => DisplayAlert("Error", e, "OK"));

}