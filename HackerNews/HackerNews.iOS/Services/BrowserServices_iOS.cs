using System.Threading.Tasks;

using Foundation;
using CoreFoundation;
using SafariServices;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using HackerNews.iOS;

[assembly: Dependency(typeof(BrowserServices_iOS))]
namespace HackerNews.iOS
{
    public class BrowserServices_iOS : IBrowserServices
    {
        public async Task OpenBrowser(string url)
        {
            var safariViewController = new SFSafariViewController(new NSUrl(url), true)
            {
                PreferredBarTintColor = ColorConstants.BrowserNavigationBarBackgroundColor.ToUIColor(),
                PreferredControlTintColor = ColorConstants.BrowserNavigationBarTextColor.ToUIColor()
            };

            var visibleViewController = await HelperMethods.GetVisibleViewController();

            var taskCompletionSource = new TaskCompletionSource<bool>();
            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                visibleViewController.PresentViewControllerAsync(safariViewController, true);
                taskCompletionSource.SetResult(true);
            });

            await taskCompletionSource.Task;
        }
    }
}

