namespace RSVPApp.Pages;

/// <summary>
/// Lets a new user enter account info.
/// Saving is not functional yet (Week 3).
/// </summary>
public partial class CreateAccountPage : ContentPage
{
	public CreateAccountPage()
	{
		InitializeComponent();
	}

	// Checks that every field has data, then returns to Login
	private async void OnAddClicked(object sender, EventArgs e)
	{
		bool anyBlank =
			string.IsNullOrWhiteSpace(txtName.Text) ||
			string.IsNullOrWhiteSpace(txtEmail.Text) ||
			string.IsNullOrWhiteSpace(txtPassword.Text) ||
			string.IsNullOrWhiteSpace(txtPhone.Text);

		if (anyBlank)
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		// TODO (Week 3): save the new user to local storage
		await Navigation.PopAsync();
	}

	// Goes back to Login without saving
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}