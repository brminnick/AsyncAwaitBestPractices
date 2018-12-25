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
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Constant Fields
        static readonly JsonSerializer _serializer = new JsonSerializer();
        static readonly HttpClient _client = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        readonly WeakEventManager<PropertyChangedEventArgs> _propertyChangedEventManager = new WeakEventManager<PropertyChangedEventArgs>();
        #endregion

        #region Fields
        static int _networkIndicatorCount = 0;
        #endregion

        #region Events
        event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
        {
            add => _propertyChangedEventManager.AddEventHandler(value);
            remove => _propertyChangedEventManager.RemoveEventHandler(value);
        }
        #endregion

        #region Methods
        protected void SetProperty<T>(ref T backingStore, T value, Action onChanged = null, [CallerMemberName] string propertyname = "")
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

            var httpContent = new StringContent(stringPayload, Encoding.UTF8, "application/json");

            UpdateActivityIndicatorStatus(true);

            try
            {
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
                UpdateActivityIndicatorStatus(false);
            }
        }

        void UpdateActivityIndicatorStatus(bool isActivityInidicatorRunning)
        {
            if (isActivityInidicatorRunning)
            {
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = true);
                _networkIndicatorCount++;
            }
            else if (--_networkIndicatorCount <= 0)
            {
                Device.BeginInvokeOnMainThread(() => Application.Current.MainPage.IsBusy = false);
                _networkIndicatorCount = 0;
            }
        }

        void OnPropertyChanged([CallerMemberName]string propertyName = "") =>
            _propertyChangedEventManager?.HandleEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));
        #endregion
    }
}