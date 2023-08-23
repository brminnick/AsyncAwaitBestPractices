using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace HackerNews;

abstract class BaseViewModel : INotifyPropertyChanged
{
	static readonly JsonSerializer _serializer = new();
	static readonly HttpClient _client = new()
	{
		Timeout = TimeSpan.FromMinutes(1)
	};

	readonly AsyncAwaitBestPractices.WeakEventManager _propertyChangedEventManager = new();

	static volatile int _networkIndicatorCount;

	event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
	{
		add => _propertyChangedEventManager.AddEventHandler(value);
		remove => _propertyChangedEventManager.RemoveEventHandler(value);
	}

	protected void SetProperty<T>(ref T backingStore, in T value, in Action? onChanged = null, [CallerMemberName] in string propertyName = "")
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return;

		backingStore = value;

		onChanged?.Invoke();

		OnPropertyChanged(propertyName);
	}

	protected async Task<TDataObject> GetDataFromAPI<TDataObject>(string apiUrl)
	{
		await UpdateActivityIndicatorStatus(true).ConfigureAwait(false);

		try
		{
			using var stream = await _client.GetStreamAsync(apiUrl).ConfigureAwait(false);
			using var reader = new StreamReader(stream);
			using var json = new JsonTextReader(reader);

			return _serializer.Deserialize<TDataObject>(json) ?? throw new JsonException();
		}
		finally
		{
			await UpdateActivityIndicatorStatus(false).ConfigureAwait(false);
		}
	}

	async Task UpdateActivityIndicatorStatus(bool isActivityIndicatorRunning)
	{
		if (isActivityIndicatorRunning)
		{
			_networkIndicatorCount++;
			await setIsBusy(true).ConfigureAwait(false);
		}
		else if (--_networkIndicatorCount <= 0)
		{
			_networkIndicatorCount = 0;
			await setIsBusy(false).ConfigureAwait(false);
		}

		static async ValueTask setIsBusy(bool isBusy)
		{
			if (Application.Current?.MainPage != null)
				await MainThread.InvokeOnMainThreadAsync(() => Application.Current.MainPage.IsBusy = true);
		}
	}

	void OnPropertyChanged([CallerMemberName] in string propertyName = "") =>
		_propertyChangedEventManager.RaiseEvent(this, new PropertyChangedEventArgs(propertyName), nameof(INotifyPropertyChanged.PropertyChanged));
}