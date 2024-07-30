using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel(IDispatcher dispatcher, HackerNewsAPIService _hackerNewsAPIService) : BaseViewModel(dispatcher)
{
	readonly HackerNewsAPIService _hackerNewsAPIService = hackerNewsAPIService;
	readonly WeakEventManager _pullToRefreshEventManager = new();

	[ObservableProperty]
	bool _isListRefreshing;

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

	async IAsyncEnumerable<StoryModel> GetTopStories(
		int storyCount,
		[EnumeratorCancellation] CancellationToken token)
	{
		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs(token).ConfigureAwait(false);
		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs(token).ConfigureAwait(false);

		var getTopStoryTaskList = new List<Task<StoryModel>>();
		foreach(var story in topStoryIds)
		{
			getTopStoryTaskList.Add(_hackerNewsAPIService.GetStory(story, token));
		}

		while (getTopStoryTaskList.Any() && storyCount-- > 0)
		{
			Task<StoryModel> completedGetStoryTask = await Task.WhenAny(getTopStoryTaskList).ConfigureAwait(false);
			getTopStoryTaskList.Remove(completedGetStoryTask);

			var story = await completedGetStoryTask.ConfigureAwait(false);
			yield return story;
		}
	}

	void OnPullToRefreshFailed(string message) => _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));
}