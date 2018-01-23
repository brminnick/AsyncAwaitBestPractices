using Xamarin.Forms;

namespace HackerNews
{
    public class App : Application
    {
        public App() => MainPage = new NavigationPage(new NewsPage());
    }
}
