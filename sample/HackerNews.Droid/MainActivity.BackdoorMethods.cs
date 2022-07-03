using Android.Runtime;
using HackerNews.Shared;

namespace HackerNews.Droid
{
    public partial class MainActivity
    {
        [Preserve, Java.Interop.Export(BackdoorMethodConstants.GetStoriesAsBase64String)]
        public string GetStoriesAsBase64String() => BackdoorMethodServices.GetStoriesAsBase64String();
    }
}
