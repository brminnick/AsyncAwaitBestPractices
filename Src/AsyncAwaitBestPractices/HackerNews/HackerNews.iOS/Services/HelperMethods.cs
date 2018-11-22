using System.Threading.Tasks;

using UIKit;
using CoreFoundation;

namespace HackerNews.iOS
{
    public static class HelperMethods
    {
        public static Task<UIViewController> GetVisibleViewController()
        {
            var taskCompletionSource = new TaskCompletionSource<UIViewController>();

            DispatchQueue.MainQueue.DispatchAsync(() =>
            {
                var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;

                switch (rootController.PresentedViewController)
                {
                    case UINavigationController navigationController:
                        taskCompletionSource.SetResult(navigationController.TopViewController);
                        break;

                    case UITabBarController tabBarController:
                        taskCompletionSource.SetResult(tabBarController.SelectedViewController);
                        break;

                    case null:
                        taskCompletionSource.SetResult(rootController);
                        break;

                    default:
                        taskCompletionSource.SetResult(rootController.PresentedViewController);
                        break;
                }
            });

            return taskCompletionSource.Task;
        }
    }
}

