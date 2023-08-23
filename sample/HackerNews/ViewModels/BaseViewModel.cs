using System.Collections;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace HackerNews;

abstract partial class BaseViewModel : ObservableObject
{
	protected BaseViewModel(IDispatcher dispatcher)
	{
		Dispatcher = dispatcher;
		BindingBase.EnableCollectionSynchronization(TopStoryCollection, null, ObservableCollectionCallback);
	}

	public ObservableCollection<StoryModel> TopStoryCollection { get; } = new();

	protected IDispatcher Dispatcher { get; }

	//Ensure Observable Collection is thread-safe https://codetraveler.io/2019/09/11/using-observablecollection-in-a-multi-threaded-xamarin-forms-application/
	void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
	{
		Dispatcher.Dispatch(accessMethod);
	}
}