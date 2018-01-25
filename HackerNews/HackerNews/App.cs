using Xamarin.Forms;

namespace HackerNews
{
    public class App : Application
    {
        public App()
        {
            MainPage = new NavigationPage(new NewsPage())
            {
                BarBackgroundColor = Color.FromHex("FF6601"),
                BarTextColor = Color.Black
            };
        }
    }
}
