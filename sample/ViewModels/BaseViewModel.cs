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

	public ObservableCollection<StoryModel> TopStoryCollection { get; } = [];

	protected IDispatcher Dispatcher { get; }

	protected static void InsertIntoSortedCollection<T>(ObservableCollection<T> collection, Comparison<T> comparison, T modelToInsert)
	{
		if (collection.Count is 0)
		{
			collection.Add(modelToInsert);
		}
		else
		{
			int index = 0;
			foreach (var model in collection)
			{
				if (comparison(model, modelToInsert) >= 0)
				{
					collection.Insert(index, modelToInsert);
					return;
				}

				index++;
			}

			collection.Insert(index, modelToInsert);
		}
	}

	//Ensure Observable Collection is thread-safe https://codetraveler.io/2019/09/11/using-observablecollection-in-a-multi-threaded-xamarin-forms-application/
	void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
	{
		Dispatcher.Dispatch(accessMethod);
	}
}