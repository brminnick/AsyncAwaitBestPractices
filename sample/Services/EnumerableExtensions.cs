using System.Collections;

namespace HackerNews;

static class EnumerableExtensions
{
	public static bool IsNullOrEmpty(this IEnumerable? enumerable)
	{
		if(enumerable is null)
			return true;
		
		var enumerator = enumerable.GetEnumerator();
		using ((IDisposable)enumerator)
		{
			return !enumerator.MoveNext();
		}
	}
}