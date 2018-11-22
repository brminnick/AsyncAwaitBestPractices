using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

using HackerNewsExtensions;

namespace HackerNews
{
    public class NewsViewModel_BadAsyncAwaitPractices : BaseViewModel
    {
        #region Fields
        bool _isListRefreshing;
        ICommand _refreshCommand;
        List<StoryModel> _topStoryList;
        #endregion

        #region Constructors
        public NewsViewModel_BadAsyncAwaitPractices()
        {
            //ToDo Refactor
            ExecuteRefreshCommand();
        }
        #endregion

        #region Properties
        //ToDo Refactor
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
            //ToDo Refactor
            SetIsRefreshing(true).Wait();

            try
            {
                TopStoryList = await GetTopStories(20);
            }
            finally
            {
                //ToDo Refactor
                SetIsRefreshing(false).Wait();
            }
        }

        async Task<List<StoryModel>> GetTopStories(int numberOfStories)
        {
            List<StoryModel> topStoryList = new List<StoryModel>();

            //ToDo Refactor
            var topStoryIds = await GetTopStoryIDs();

            for (int i = 0; i < numberOfStories; i++)
            {
                //ToDo Refactor
                var story = await GetStory(topStoryIds[i]);
                topStoryList.Add(story);
            }

            return topStoryList.Where(x => x != null).OrderByDescending(x => x.Score).ToList();
        }

        //ToDo Refactor
        async Task<List<string>> GetTopStoryIDs()
        {
            return await GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
        }

        //ToDo Refactor
        async Task<StoryModel> GetStory(string storyId)
        {
            if (string.IsNullOrWhiteSpace(storyId))
                return null;

            try
            {
                return await GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            }
            catch (System.Exception e)
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
        #endregion
    }
}
