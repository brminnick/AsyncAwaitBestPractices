using System;
using Newtonsoft.Json;
using Xamarin.UITest;
using Xamarin.UITest.Android;
using Xamarin.UITest.iOS;

namespace HackerNews.UITests
{
    static class BackdoorServices
    {
        public static object InvokeBackdoorMethod(this IApp app, string backdoorMethodName, string parameter = "") => app switch
        {
            iOSApp iosApp => iosApp.Invoke(backdoorMethodName + ":", parameter),
            AndroidApp androidApp when string.IsNullOrWhiteSpace(parameter) => androidApp.Invoke(backdoorMethodName),
            AndroidApp androidApp => androidApp.Invoke(backdoorMethodName, parameter),
            _ => throw new NotSupportedException("Platform Not Supported"),
        };

        public static T InvokeBackdoorMethod<T>(this IApp app, string backdoorMethodName, string parameter = "")
        {
            var result = app.InvokeBackdoorMethod(backdoorMethodName, parameter).ToString();
            return JsonConvert.DeserializeObject<T>(result) ?? throw new JsonException();
        }
    }
}
