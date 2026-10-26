using RSVPApp.DataAccess;
using RSVPApp.Models;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a new user create an account. Saves it to the Users table.
/// </summary>
public partial class CreateAccountPage : ContentPage
{
	public CreateAccountPage()
	{
		InitializeComponent();
	}

	// Checks every field, saves the user, then returns to Login
	private async void OnAddClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtName.Text) ||
			string.IsNullOrWhiteSpace(txtEmail.Text) ||
			string.IsNullOrWhiteSpace(txtPassword.Text) ||
			string.IsNullOrWhiteSpace(txtPhone.Text))
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		var user = new User
		{
			Name = txtName.Text.Trim(),
			Email = txtEmail.Text.Trim(),
			Password = txtPassword.Text,
			Phone = txtPhone.Text.Trim()
		};

		bool added = await RsvpDatabase.Instance.AddUserAsync(user);
		if (!added)
		{
			lblError.Text = "An account with that email already exists.";
			return;
		}

		await DisplayAlert("Account Created", "Your account was created. You can now log in.", "OK");
		await Navigation.PopAsync();
	}

	// Goes back to Login without saving
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}