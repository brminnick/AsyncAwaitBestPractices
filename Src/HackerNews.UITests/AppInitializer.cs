using System;
using Xamarin.UITest;

namespace HackerNews.UITests
{
    public static class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            switch (platform)
            {
                case Platform.Android:
                    return ConfigureApp.Android.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
                case Platform.iOS:
                    return ConfigureApp.iOS.StartApp(Xamarin.UITest.Configuration.AppDataMode.Clear);
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
