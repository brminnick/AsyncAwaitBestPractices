using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.Input;

namespace HackerNews;

partial class NewsViewModel_BadAsyncAwaitPractices : BaseViewModel
{
	readonly IDispatcher _dispatcher;
	readonly HackerNewsAPIService _hackerNewsAPIService;

	readonly WeakEventManager _pullToRefreshEventManager = new();

	public NewsViewModel_BadAsyncAwaitPractices(IDispatcher dispatcher, HackerNewsAPIService hackerNewsAPIService)
	{
		_dispatcher = dispatcher;
		_hackerNewsAPIService = hackerNewsAPIService;

		//ToDo Refactor
		Refresh();
	}

	public event EventHandler<string> PullToRefreshFailed
	{
		add => _pullToRefreshEventManager.AddEventHandler(value);
		remove => _pullToRefreshEventManager.RemoveEventHandler(value);
	}

	public ObservableCollection<StoryModel> TopStoryCollection { get; } = new();

	[RelayCommand]
	async Task Refresh()
	{
		TopStoryCollection.Clear();

		try
		{
			var topStoriesList = await GetTopStories(StoriesConstants.NumberOfStories);

			foreach (var story in topStoriesList)
			{
				TopStoryCollection.Add(story);
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

	async Task<List<StoryModel>> GetTopStories(int numberOfStories)
	{
		List<StoryModel> topStoryList = new List<StoryModel>();

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
	async Task<StoryModel> GetStory(string storyId)
	{
		return await Hack
	}

	//ToDo Refactor
	async Task<List<string>?> GetTopStoryIDs()
	{
		try
		{
			return await GetDataFromAPI<List<string>?>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
		}
		catch (Exception e)
		{
			Debug.WriteLine(e.Message);
			return null;
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