using Foundation;
using HackerNews.Shared;

namespace HackerNews.iOS
{
    public partial class AppDelegate
    {
        [Preserve, Export(BackdoorMethodConstants.GetStoriesAsBase64String + ":")]
        public NSString GetStoriesAsBase64String(NSString noValue) =>
            new NSString(BackdoorMethodServices.GetStoriesAsBase64String());
    }
}
