using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using AsyncAwaitBestPractices;
using AsyncAwaitBestPractices.MVVM;

using HackerNews.Shared;

namespace HackerNews
{
    class NewsViewModel_GoodAsyncAwaitPractices : BaseViewModel
    {
        readonly WeakEventManager<string> _errorOccurredEventManager = new WeakEventManager<string>();

        bool _isListRefreshing;
        IAsyncCommand? _refreshCommand;
        IReadOnlyList<StoryModel> _topStoryList = new List<StoryModel>().ToList();

        public NewsViewModel_GoodAsyncAwaitPractices()
        {
            ExecuteRefreshCommand().SafeFireAndForget(onException:  ex => Debug.WriteLine(ex));
        }

        public event EventHandler<string> ErrorOccurred
        {
            add => _errorOccurredEventManager.AddEventHandler(value);
            remove => _errorOccurredEventManager.RemoveEventHandler(value);
        }

        public IAsyncCommand RefreshCommand => _refreshCommand ??= new AsyncCommand(ExecuteRefreshCommand);

        public IReadOnlyList<StoryModel> TopStoryList
        {
            get => _topStoryList;
            set => SetProperty(ref _topStoryList, value);
        }

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        async Task ExecuteRefreshCommand()
        {
            IsListRefreshing = true;

            try
            {
                TopStoryList = await GetTopStories(StoriesConstants.NumberOfStories).ConfigureAwait(false);
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task<List<StoryModel>> GetTopStories(int numberOfStories)
        {
            var topStoryIds = await GetTopStoryIDs().ConfigureAwait(false);

            var getTopStoryTaskList = new List<Task<StoryModel>>();
            for (int i = 0; i < Math.Min(topStoryIds.Count, numberOfStories); i++)
            {
                getTopStoryTaskList.Add(GetStory(topStoryIds[i]));
            }

            var topStoriesArray = await Task.WhenAll(getTopStoryTaskList).ConfigureAwait(false);

            return topStoriesArray.Where(x => x != null).OrderByDescending(x => x.Score).ToList();
        }

        Task<StoryModel> GetStory(string storyId) => GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");

        async ValueTask<List<string>> GetTopStoryIDs()
        {
            if (TopStoryList.Any())
                return TopStoryList.Select(x => x.Id.ToString()).ToList();

            try
            {
                return await GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty").ConfigureAwait(false);
            }
            catch (Exception e)
            {
                OnErrorOccurred(e.Message);
                return Enumerable.Empty<string>().ToList();
            }
        }

        void OnErrorOccurred(string message) => _errorOccurredEventManager.HandleEvent(this, message, nameof(ErrorOccurred));
    }
}
