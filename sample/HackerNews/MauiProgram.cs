using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Markup;
using Polly;
using Refit;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace HackerNews;

public class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder()
								.UseMauiApp<App>()
								.UseMauiCommunityToolkit()
								.UseMauiCommunityToolkitMarkup();

		builder.ConfigureMauiHandlers(handlers =>
		{
#if IOS || MACCATALYST
			handlers.AddHandler<Shell, ShellWithLargeTitles>();
#endif
		});

		// App
		builder.Services.AddSingleton<App>();
		builder.Services.AddSingleton<AppShell>();

		// Services
		builder.Services.AddSingleton(Browser.Default);
		builder.Services.AddSingleton<HackerNewsAPIService>();

		builder.Services.AddRefitClient<IHackerNewsAPI>()
							.ConfigureHttpClient(client => client.BaseAddress = new Uri("https://hacker-news.firebaseio.com/v0"))
							.AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(3, ExponentialBackoff));

		// Pages + View Models
		builder.Services.AddTransientWithShellRoute<NewsPage, NewsViewModel>($"//{nameof(NewsPage)}");


		return builder.Build();

		static TimeSpan ExponentialBackoff(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
	}
}