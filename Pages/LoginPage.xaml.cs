using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Start page. Lets a user log in, continue as a guest or create an account.
/// </summary>
public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
		InitializeComponent();
	}

	// Clears the fields every time this page shows (for example, after logout)
	protected override void OnAppearing()
	{
		base.OnAppearing();
		txtEmail.Text = string.Empty;
		txtPassword.Text = string.Empty;
		lblError.Text = string.Empty;
	}

	// Checks the email and password against the hard-coded users
	private async void OnLoginClicked(object sender, EventArgs e)
	{
		string email = txtEmail.Text?.Trim() ?? string.Empty;
		string password = txtPassword.Text ?? string.Empty;

		// Both fields are required
		if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
		{
			lblError.Text = "Please enter your email and password.";
			return;
		}

		User? user = SampleData.FindUser(email, password);

		if (user == null)
		{
			lblError.Text = "Login failed. Check your email and password.";
			return;
		}

		// Save the logged-in user, then open the Events page
		Session.CurrentUser = user;
		await Navigation.PushAsync(new EventsPage());
	}

	// Opens the Events page with no logged-in user
	private async void OnGuestClicked(object sender, EventArgs e)
	{
		Session.CurrentUser = null;
		await Navigation.PushAsync(new EventsPage());
	}

	// Opens the Create Account page
	private async void OnCreateAccountClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new CreateAccountPage());
	}
}