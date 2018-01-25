using System.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

namespace HackerNews
{
    public class NewsViewModel_GoodAsyncAwaitPractices : BaseViewModel
    {
        #region Fields
        bool _isListRefreshing;
        ICommand _refreshCommand;
        List<StoryModel> _topStoryList;
        #endregion

        #region Constructors
        public NewsViewModel_GoodAsyncAwaitPractices()
        {
            RefreshCommand?.Execute(null);
        }
        #endregion

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ?? 
            (_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

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

            var getTop20StoriesTaskList = new List<Task<StoryModel>>();
            for (int i = 0; i < numberOfStories; i++)
            {
                getTop20StoriesTaskList.Add(GetStory(topStoryIds[i]));
            }

            await Task.WhenAll(getTop20StoriesTaskList).ConfigureAwait(false);

            var topStoryList = new List<StoryModel>();
            foreach (var getStoryTask in getTop20StoriesTaskList)
            {
                topStoryList.Add(await getStoryTask.ConfigureAwait(false));
            }

            return topStoryList.OrderByDescending(x => x.Score).ToList();
        }

        Task<List<string>> GetTopStoryIDs() =>
            GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");

        Task<StoryModel> GetStory(string storyId) =>
            GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
        #endregion
    }
}
