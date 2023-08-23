namespace HackerNews;

static class ServiceProvider
{
	public static T? GetService<T>() where T : class
		=> Application.Current?.Handler.MauiContext?.Services.GetService<T>();
}