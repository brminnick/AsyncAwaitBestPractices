using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel : BaseViewModel
{
	readonly HackerNewsAPIService _hackerNewsAPIService;
	readonly WeakEventManager _pullToRefreshEventManager = new();

	[ObservableProperty]
	bool _isListRefreshing;

	public NewsViewModel(IDispatcher dispatcher, HackerNewsAPIService hackerNewsAPIService) : base(dispatcher)
	{
		_hackerNewsAPIService = hackerNewsAPIService;
	}

	public event EventHandler<string> PullToRefreshFailed
	{
		add => _pullToRefreshEventManager.AddEventHandler(value);
		remove => _pullToRefreshEventManager.RemoveEventHandler(value);
	}

	[RelayCommand]
	async Task Refresh(CancellationToken token)
	{
		TopStoryCollection.Clear();

		var minimumRefreshTimeTask = Task.Delay(TimeSpan.FromSeconds(2), token);

		try
		{
			await foreach (var story in GetTopStories(StoriesConstants.NumberOfStories, token).ConfigureAwait(false))
			{
				if (!TopStoryCollection.Any(x => x.Title.Equals(story.Title, StringComparison.Ordinal)))
					InsertIntoSortedCollection(TopStoryCollection, (a, b) => b.Score.CompareTo(a.Score), story);
			}
		}
		catch (Exception e)
		{
			OnPullToRefreshFailed(e.ToString());
		}
		finally
		{
			await minimumRefreshTimeTask.ConfigureAwait(false);
			IsListRefreshing = false;
		}
	}

	async IAsyncEnumerable<StoryModel> GetTopStories(int storyCount, [EnumeratorCancellation] CancellationToken token)
	{
		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs(token).ConfigureAwait(false);
		var getTopStoryTaskList = topStoryIds.Select(id => _hackerNewsAPIService.GetStory(id, token)).ToList();

		while (getTopStoryTaskList.Any() && storyCount-- > 0)
		{
			var completedGetStoryTask = await Task.WhenAny(getTopStoryTaskList).ConfigureAwait(false);
			getTopStoryTaskList.Remove(completedGetStoryTask);

			var story = await completedGetStoryTask.ConfigureAwait(false);
			yield return story;
		}
	}

	void OnPullToRefreshFailed(string message) => _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));
}