using System.Collections.Frozen;
using System.Diagnostics;
using AsyncAwaitBestPractices;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel_BadAsyncAwaitPractices : BaseViewModel
{
	readonly HackerNewsAPIService _hackerNewsAPIService;
	readonly WeakEventManager _pullToRefreshEventManager = new();
	readonly AsyncAwaitBestPractices.WeakEventManager _pullToRefreshEventManager = new();

	[ObservableProperty]
	bool _isListRefreshing;

	public NewsViewModel_BadAsyncAwaitPractices(IDispatcher dispatcher, HackerNewsAPIService hackerNewsAPIService) : base(dispatcher)
	{
		_hackerNewsAPIService = hackerNewsAPIService;

		//ToDo Refactor
		Refresh(CancellationToken.None);
	}

	public event EventHandler<string> PullToRefreshFailed
	{
		add => _pullToRefreshEventManager.AddEventHandler(value);
		remove => _pullToRefreshEventManager.RemoveEventHandler(value);
	}

	[RelayCommand]
	async Task Refresh(CancellationToken token)
	{
		// ToDo Refactor
		var minimumRefreshTimeTask = Task.Delay(TimeSpan.FromSeconds(2));

		try
		{
			// ToDo Refactor
			var topStoriesList = await GetTopStories(StoriesConstants.NumberOfStories);

			TopStoryCollection.Clear();

			foreach (var story in topStoriesList)
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
			// ToDo Refactor
			minimumRefreshTimeTask.Wait();
			IsListRefreshing = false;
		}
	}

	// ToDo Refactor
	async Task<FrozenSet<StoryModel>> GetTopStories(int storyCount = int.MaxValue)
	{
		List<StoryModel> topStoryList = new();

		//ToDo Refactor
		var topStoryIds = await GetTopStoryIDs();

		foreach (var topStoryId in topStoryIds)
		{
			var story = await GetStory(topStoryId);
			topStoryList.Add(story);

			if (topStoryList.Count >= storyCount)
				break;
		}

		return topStoryList.Where(x => x is not null).OrderByDescending(x => x.Score).ToFrozenSet();
	}

	//ToDo Refactor
	async Task<StoryModel> GetStory(long storyId)
	{
		return await _hackerNewsAPIService.GetStory(storyId, CancellationToken.None);
	}

	//ToDo Refactor
	async Task<FrozenSet<long>> GetTopStoryIDs()
	{
		if (IsDataRecent(TimeSpan.FromHours(1)))
			return TopStoryCollection.Select(x => x.Id).ToFrozenSet();

		try
		{
			return await _hackerNewsAPIService.GetTopStoryIDs(CancellationToken.None);
		}
		catch (Exception e)
		{
			Trace.WriteLine(e.Message);
			throw;
		}
	}

	bool IsDataRecent(TimeSpan timeSpan) => (DateTimeOffset.UtcNow - TopStoryCollection.Max(x => x.CreatedAt_DateTimeOffset)) > timeSpan;

	void OnPullToRefreshFailed(string message) => _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));
}