using Xamarin.Forms;

namespace TwitterSearch
{
    public class App : Application
    {
        public App() => MainPage = new NavigationPage(new SearchPage());
    }
}
