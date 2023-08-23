using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel_BadAsyncAwaitPractices : BaseViewModel
{
	readonly HackerNewsAPIService _hackerNewsAPIService;
	readonly WeakEventManager _pullToRefreshEventManager = new();

	[ObservableProperty]
	bool _isListRefreshing;

	public NewsViewModel_BadAsyncAwaitPractices(IDispatcher dispatcher, HackerNewsAPIService hackerNewsAPIService) : base(dispatcher)
	{
		_hackerNewsAPIService = hackerNewsAPIService;

		//ToDo Refactor
		Refresh();
	}

	public event EventHandler<string> PullToRefreshFailed
	{
		add => _pullToRefreshEventManager.AddEventHandler(value);
		remove => _pullToRefreshEventManager.RemoveEventHandler(value);
	}

	[RelayCommand]
	async Task Refresh()
	{
		TopStoryCollection.Clear();

		try
		{
			var topStoriesList = await GetTopStories(StoriesConstants.NumberOfStories);

			foreach (var story in topStoriesList)
			{
				if (!TopStoryCollection.Any(x => x.Title.Equals(story.Title)))
				InsertIntoSortedCollection(TopStoryCollection, (a, b) => b.Score.CompareTo(a.Score), story);
			}
		}
		catch (Exception e)
		{
			OnPullToRefreshFailed(e.ToString());
		}
		finally
		{
			IsListRefreshing = false;
		}
	}

	// ToDo Refactor
	async Task<List<StoryModel>> GetTopStories(int numberOfStories)
	{
		List<StoryModel> topStoryList = new();

		//ToDo Refactor
		var topStoryIds = await GetTopStoryIDs();

		if (topStoryIds != null)
		{
			for (int i = 0; i < numberOfStories; i++)
			{
				//ToDo Refactor
				var story = await GetStory(topStoryIds[i]);
				topStoryList.Add(story);
			}
		}

		return topStoryList.Where(x => x != null).OrderByDescending(x => x.Score).ToList();
	}

	//ToDo Refactor
	async Task<StoryModel> GetStory(long storyId)
	{
		return await _hackerNewsAPIService.GetStory(storyId);
	}

	//ToDo Refactor
	async Task<IReadOnlyList<long>?> GetTopStoryIDs()
	{
		try
		{
			return await _hackerNewsAPIService.GetTopStoryIDs();
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message);
			throw;
		}
	}

	async Task<bool> SetIsRefreshing(bool isRefreshing)
	{
		IsListRefreshing = isRefreshing;
		await Task.Delay(100);

		return isRefreshing;
	}

	void OnPullToRefreshFailed(string message) => _pullToRefreshEventManager.HandleEvent(this, message, nameof(PullToRefreshFailed));
}