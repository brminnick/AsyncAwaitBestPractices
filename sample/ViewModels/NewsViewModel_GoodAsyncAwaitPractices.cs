using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel(IDispatcher dispatcher, HackerNewsAPIService hackerNewsAPIService) : BaseViewModel(dispatcher)
{
	readonly HackerNewsAPIService _hackerNewsAPIService = hackerNewsAPIService;
	readonly WeakEventManager _pullToRefreshEventManager = new();

	public event EventHandler<string> PullToRefreshFailed
	{
		add => _pullToRefreshEventManager.AddEventHandler(value);
		remove => _pullToRefreshEventManager.RemoveEventHandler(value);
	}

	[ObservableProperty]
	public partial bool IsListRefreshing { get; set; }

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
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(storyCount);

		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs(token).ConfigureAwait(false);

		var getTopStoryTaskList = topStoryIds.Select(id => _hackerNewsAPIService.GetStory(id, token)).ToList();

		await foreach (var topStoryTask in Task.WhenEach(getTopStoryTaskList).WithCancellation(token).ConfigureAwait(false))
		{
			yield return await topStoryTask.ConfigureAwait(false);

			if (--storyCount <= 0)
			{
				break;
			}
		}
	}

	void OnPullToRefreshFailed(string message) => _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));
}