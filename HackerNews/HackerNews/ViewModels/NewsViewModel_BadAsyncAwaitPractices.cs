using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Forms;

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
            Task.Run(async () => await ExecuteRefreshCommand());
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
            //ToDo Refactor
            SetIsRefreshing(true).Wait();

            try
            {
                //ToDo Refactor
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
            //ToDo Refactor
            var topStoryIds = await GetTopStoryIDs();

            var getTop20StoriesTaskList = new List<Task<StoryModel>>();
            for (int i = 0; i < numberOfStories; i++)
            {
                getTop20StoriesTaskList.Add(GetStory(topStoryIds[i]));
            }

            //ToDo Refactor
            await Task.WhenAll(getTop20StoriesTaskList);

            var topStoryList = new List<StoryModel>();
            foreach (var getStoryTask in getTop20StoriesTaskList)
            {
                //ToDo Refactor
                topStoryList.Add(getStoryTask.Result);
            }

            return topStoryList.OrderByDescending(x => x.Score).ToList();
        }

        //ToDo Refactor
        async Task SetIsRefreshing(bool isRefreshing)
        {
            IsListRefreshing = isRefreshing;
        }

        //ToDo Refactor
        async Task<List<string>> GetTopStoryIDs()
        {
            return await GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");
        }

        //ToDo Refactor
        async Task<StoryModel> GetStory(string storyId)
        {
            try
            {
                return await GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");
            }
            catch(System.Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }
        #endregion
    }
}
