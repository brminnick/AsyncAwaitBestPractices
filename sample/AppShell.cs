namespace HackerNews;

partial class AppShell : Shell
{
	public AppShell(NewsPage newsPage)
	{
		Items.Add(newsPage);

#if IOS || MACCATALYST
		ShellAttachedProperties.SetPrefersLargeTitles(this, true);
#endif
	}
}