using Foundation;
using HackerNews.Shared;

namespace HackerNews.iOS
{
    public partial class AppDelegate
    {
        public AppDelegate() => Xamarin.Calabash.Start();

        [Preserve, Export(BackdoorMethodConstants.GetStoriesAsBase64String + ":")]
        public NSString GetStoriesAsBase64String(NSString noValue) =>
            new NSString(BackdoorMethodServices.GetStoriesAsBase64String());
    }
}
