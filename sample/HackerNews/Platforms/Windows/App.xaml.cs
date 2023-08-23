using Microsoft.Maui;
using Microsoft.Maui.Hosting;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace HackerNews.WinUI;

public partial class App : MauiWinUIApplication
{
	public App() => InitializeComponent();

	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}