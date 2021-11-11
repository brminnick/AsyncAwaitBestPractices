using System.Threading.Tasks;
using Xamarin.UITest;

namespace HackerNews.UITests
{
	abstract class BasePage
	{
		protected BasePage(IApp app, string pageTitle)
		{
			App = app;
			PageTitle = pageTitle;
		}

		public string PageTitle { get; }
		protected IApp App { get; }

		public virtual Task WaitForPageToLoad()
		{
			App.WaitForElement(x => x.Marked(PageTitle));
			return Task.CompletedTask;
		}
	}
}