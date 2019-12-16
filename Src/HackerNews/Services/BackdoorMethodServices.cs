#if DEBUG
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
			var mainPage = (NavigationPage)Application.Current.MainPage;
			return (NewsPage)mainPage.RootPage;
		}

		static NewsViewModel_GoodAsyncAwaitPractices GetViewModel() => (NewsViewModel_GoodAsyncAwaitPractices)GetNewsPage().BindingContext;
	}
}
#endif