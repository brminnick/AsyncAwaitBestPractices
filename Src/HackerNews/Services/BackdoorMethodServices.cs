using Newtonsoft.Json;
using Xamarin.Forms;

namespace HackerNews
{
    public static class BackdoorMethodServices
    {
        public static string GetStoriesAsBase64String()
        {
            var storyList = GetViewModel().TopStoryList;
            return JsonConvert.SerializeObject(storyList);
        }

        static NewsPage GetNewsPage()
        {
            var mainPage = Application.Current.MainPage as NavigationPage;
            return mainPage.RootPage as NewsPage;
        }

        static NewsViewModel_GoodAsyncAwaitPractices GetViewModel() => GetNewsPage().BindingContext as NewsViewModel_GoodAsyncAwaitPractices;
    }
}