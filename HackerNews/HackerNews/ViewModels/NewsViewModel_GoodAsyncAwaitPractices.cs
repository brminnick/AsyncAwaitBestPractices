using System;
using System.Linq;
using System.Net.Http;
using System.Windows.Input;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using Xamarin.Forms;
using System.Text;
using System.IO;

namespace HackerNews
{
    public class NewsViewModel_GoodAsyncAwaitPractices : BaseViewModel
    {
        #region Constant Fields
        static readonly JsonSerializer _serializer = new JsonSerializer();
        static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
        #endregion

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
                TopStoryList = await GetTopStories(20);
            }
            finally
            {
                IsListRefreshing = false;
            }
        }

        async Task<List<StoryModel>> GetTopStories(int numberOfStories)
        {
            var topStoryIds = await GetTopStoryIDs();

            var getTop20StoriesTaskList = new List<Task<StoryModel>>();
            for (int i = 0; i < numberOfStories; i++)
                getTop20StoriesTaskList.Add(GetStory(topStoryIds[i]));

            await Task.WhenAll(getTop20StoriesTaskList);

            var topStoryList = new List<StoryModel>();
            foreach (var getStoryTask in getTop20StoriesTaskList)
                topStoryList.Add(await getStoryTask);

            return topStoryList;
        }

        Task<List<string>> GetTopStoryIDs() =>
           GetDataObjectFromAPI<List<string>>("https://hacker-news.firebaseio.com/v0/topstories.json?print=pretty");

        Task<StoryModel> GetStory(string storyId) =>
            GetDataObjectFromAPI<StoryModel>($"https://hacker-news.firebaseio.com/v0/item/{storyId}.json?print=pretty");

        async Task<TDataObject> GetDataObjectFromAPI<TDataObject>(string apiUrl)
        {
            var stringPayload = string.Empty;

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            try
            {
                UpdateActivityIndicatorStatus(true);

                using (var stream = await _client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    if (json == null)
                        return default;

                    return await Task.Run(() => _serializer.Deserialize<TDataObject>(json)).ConfigureAwait(false);
                }
            }
            catch (Exception)
            {
                return default;
            }
            finally
            {
                UpdateActivityIndicatorStatus(false);
            }
        }
        #endregion
    }
}
