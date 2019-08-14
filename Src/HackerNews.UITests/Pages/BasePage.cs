using Xamarin.UITest;

namespace HackerNews.UITests
{
	public abstract class BasePage
	{
		protected BasePage(IApp app, string pageTitle)
		{
			App = app;
			PageTitle = pageTitle;
		}

		public string PageTitle { get; }
		protected IApp App { get; }

		public virtual void WaitForPageToLoad() => App.WaitForElement(x => x.Marked(PageTitle));
	}
}

