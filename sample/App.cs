using CommunityToolkit.Maui.Markup;
using Microsoft.Maui.Controls;

namespace HackerNews;

partial class App : Application
{
	readonly AppShell _appShell;

	public App(AppShell appshell)
	{
		Resources = new ResourceDictionary
		{
			new Style<Shell>(
				(Shell.NavBarHasShadowProperty, true),
				(Shell.TitleColorProperty, ColorConstants.NavigationBarTextColor),
				(Shell.DisabledColorProperty, ColorConstants.NavigationBarTextColor),
				(Shell.UnselectedColorProperty, ColorConstants.NavigationBarTextColor),
				(Shell.ForegroundColorProperty, ColorConstants.NavigationBarTextColor),
				(Shell.BackgroundColorProperty, ColorConstants.NavigationBarBackgroundColor)).ApplyToDerivedTypes(true),

			new Style<NavigationPage>(
				(NavigationPage.BarTextColorProperty, ColorConstants.NavigationBarTextColor),
				(NavigationPage.BarBackgroundColorProperty, ColorConstants.NavigationBarBackgroundColor)).ApplyToDerivedTypes(true)
		};

		_appShell = appshell;
	}

	protected override Window CreateWindow(IActivationState? activationState) => new(_appShell);
}