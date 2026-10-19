using RSVPApp.Pages;

namespace RSVPApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		// Start on the Login page inside a NavigationPage,
		// so pages can be pushed (opened) and popped (closed)
		var navPage = new NavigationPage(new LoginPage())
		{
			BarBackgroundColor = Color.FromArgb("#1F3A5F"),
			BarTextColor = Colors.White
		};

		return new Window(navPage);
	}
}