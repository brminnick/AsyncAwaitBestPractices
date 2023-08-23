using System.Collections.ObjectModel;
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

	static void InsertIntoSortedCollection<T>(ObservableCollection<T> collection, Comparison<T> comparison, T modelToInsert)
	{
		if (collection.Count is 0)
		{
			collection.Add(modelToInsert);
		}
		else
		{
			int index = 0;
			foreach (var model in collection)
			{
				if (comparison(model, modelToInsert) >= 0)
				{
					collection.Insert(index, modelToInsert);
					return;
				}

				index++;
			}

			collection.Insert(index, modelToInsert);
		}
	}

	[RelayCommand]
	async Task Refresh()
	{
		TopStoryCollection.Clear();

		try
		{
			await foreach (var story in GetTopStories(StoriesConstants.NumberOfStories).ConfigureAwait(false))
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

	async IAsyncEnumerable<StoryModel> GetTopStories(int? storyCount = int.MaxValue)
	{
		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs().ConfigureAwait(false);
		var getTopStoryTaskList = topStoryIds.Select(_hackerNewsAPIService.GetStory).ToList();

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