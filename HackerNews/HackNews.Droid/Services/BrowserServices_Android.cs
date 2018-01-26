using System;
using System.Threading.Tasks;

using Android.Content;
using Android.Support.CustomTabs;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Plugin.CurrentActivity;

using HackerNews.Droid;

[assembly: Dependency(typeof(BrowserServices_Android))]
namespace HackerNews.Droid
{
    public class BrowserServices_Android : IBrowserServices
    {
        Context CurrentContext => CrossCurrentActivity.Current.Activity;

        public Task OpenBrowser(string url)
        {
            var tabsBuilder = new CustomTabsIntent.Builder();
            tabsBuilder.SetShowTitle(true);
            tabsBuilder.SetToolbarColor(ColorConstants.BrowserNavigationBarBackgroundColor.ToAndroid());
            tabsBuilder.SetSecondaryToolbarColor(ColorConstants.BrowserNavigationBarTextColor.ToAndroid());

            var intent = tabsBuilder.Build();
            intent.LaunchUrl(CurrentContext, Android.Net.Uri.Parse(url));

            return Task.CompletedTask;
        }
    }
}

