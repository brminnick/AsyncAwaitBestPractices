using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

using AsyncAwaitBestPractices;

using Newtonsoft.Json;

using Xamarin.Forms;

namespace HackerNews
{
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        static readonly JsonSerializer _serializer = new JsonSerializer();
        static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        readonly WeakEventManager _propertyChangedEventManager = new WeakEventManager();

        static int _networkIndicatorCount = 0;

        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _propertyChangedEventManager.AddEventHandler(value);
            remove => _propertyChangedEventManager.RemoveEventHandler(value);
        }

        protected void SetProperty<T>(ref T backingStore, in T value, in Action onChanged = null, [CallerMemberName] in string propertyname = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;

            backingStore = value;

            onChanged?.Invoke();

            OnPropertyChanged(propertyname);
        }

        protected async Task<TDataObject> GetDataObjectFromAPI<TDataObject>(string apiUrl)
        {
            var stringPayload = string.Empty;


            await UpdateActivityIndicatorStatus(true).ConfigureAwait(false);

            try
            {
                using (var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json"))
                using (var stream = await _client.GetStreamAsync(apiUrl).ConfigureAwait(false))
                using (var reader = new StreamReader(stream))
                using (var json = new JsonTextReader(reader))
                {
                    if (json is null)
                        return default;

                    return await Task.Run(() => _serializer.Deserialize<TDataObject>(json)).ConfigureAwait(false);
                }
            }
            finally
            {
                await UpdateActivityIndicatorStatus(false).ConfigureAwait(false);
            }
        }

        async Task UpdateActivityIndicatorStatus(bool isActivityInidicatorRunning)
        {
            if (isActivityInidicatorRunning)
            {
                await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = true).ConfigureAwait(false);
                _networkIndicatorCount++;
            }
            else if (--_networkIndicatorCount <= 0)
            {
                await Device.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = false).ConfigureAwait(false);
                _networkIndicatorCount = 0;
            }
        }

        void OnPropertyChanged([CallerMemberName]in string propertyName = "") =>
            _propertyChangedEventManager.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));
    }
}