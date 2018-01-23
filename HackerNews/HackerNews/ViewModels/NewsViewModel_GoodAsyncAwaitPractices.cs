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

        #region Properties
        public ICommand RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(async () => await ExecuteRefreshCommand()));

        public bool IsListRefreshing
        {
            get => _isListRefreshing;
            set => SetProperty(ref _isListRefreshing, value);
        }

        public List<StoryModel> TopStoryList
        {
            get => _topStoryList;
            set => SetProperty(ref _topStoryList, value);
        }
        #endregion

        #region Methods
        async Task ExecuteRefreshCommand()
        {
            IsListRefreshing = true;

            try
            {
                TopStoryList = await GetTopStories();
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task<List<StoryModel>> GetTopStories()
        {
            var topStoryIds = await HackerNewsAPIService.GetTopStoryIDs();

            var getTopStoriesListTask = new List<Task<StoryModel>>();
            for (int i = 0; i < 20; i++)
                getTopStoriesListTask.Add(HackerNewsAPIService.GetStory(topStoryIds[i]));

            await Task.WhenAll(getTopStoriesListTask);

            var topStoryList = getTopStoriesListTask.Select(x => x.GetAwaiter().GetResult()).ToList();

            return topStoryList;
        }
        #endregion
    }
}
