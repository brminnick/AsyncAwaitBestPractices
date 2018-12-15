using NUnit.Framework;

using Xamarin.UITest;

namespace HackerNews.UITests
{
	[TestFixture(Platform.Android)]
	[TestFixture(Platform.iOS)]
	public abstract class BaseTest
	{
		#region Constant Fields
		readonly Platform _platform;
		#endregion

		#region Constructors
		protected BaseTest(Platform platform) => _platform = platform;
		#endregion

		#region Properties
		protected IApp App { get; private set; }
		protected NewsPage NewsPage { get; private set; }
		#endregion

		#region Methods
		[SetUp]
		virtual public void BeforeEachTest()
		{
			App = AppInitializer.StartApp(_platform);
			App.Screenshot("App Initialized");

			NewsPage = new NewsPage(App);

            NewsPage.WaitForPageToLoad();
		}
		#endregion
	}
}

