using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace HackerNews
{
    public class NewsViewModel_GoodAsyncAwaitPractices : BaseViewModel
    {
        #region Fields
        bool _isListRefreshing;
        IAsyncCommand _refreshCommand;
        List<StoryModel> _topStoryList;
        #endregion

        #region Constructors
        public NewsViewModel_GoodAsyncAwaitPractices() => RefreshCommand?.Execute(null);
        #endregion

        #region Properties
        public IAsyncCommand RefreshCommand => _refreshCommand ??
            (_refreshCommand = new AsyncCommand(ExecuteRefreshCommand, ex => Debug.WriteLine(ex.Message), false));

        public List<StoryModel> TopStoryList
        {
            get => _topStoryList;
            set => SetProperty(ref _topStoryList, value);
        }

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            IsListRefreshing = true;

            try
            {
                TopStoryList = await GetTopStories(20).ConfigureAwait(false);
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task<List<StoryModel>> GetTopStories(int numberOfStories)
        {
            var topStoryIds = await GetTopStoryIDs().ConfigureAwait(false);

            var getTopStoryTaskList = new List<ValueTask<StoryModel>>();
            for (int i = 0; i < numberOfStories; i++)
            {
                getTopStoryTaskList.Add(GetStory(topStoryIds[i]));
            }

            var topStoriesArray = await CompleteAllValueTasks(getTopStoryTaskList).ConfigureAwait(false);

            return topStoriesArray.Where(x => x != null).OrderByDescending(x => x.Score).ToList();
        }

        Task<List<string>> GetTopStoryIDs() =>
            GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");

        async ValueTask<StoryModel> GetStory(string storyId)
        {
            if (string.IsNullOrWhiteSpace(storyId))
                return null;

            try
            {
                return await GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty").ConfigureAwait(false);
            }
            catch (System.Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        static async Task<IReadOnlyCollection<T>> CompleteAllValueTasks<T>(IEnumerable<ValueTask<T>> tasks)
        {
            var results = new List<T>();
            var toAwait = new List<Task<T>>();

            foreach (var valueTask in tasks)
            {
                if (valueTask.IsCompletedSuccessfully)
                    results.Add(valueTask.Result);
                else
                    toAwait.Add(valueTask.AsTask());
            }

            results.AddRange(await Task.WhenAll(toAwait).ConfigureAwait(false));

            return results;
        }
        #endregion
    }
}
