using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a logged-in user enter a new event.
/// Saving is not functional yet (Week 3).
/// </summary>
public partial class AddEventPage : ContentPage
{
	public AddEventPage()
	{
		InitializeComponent();

		// Fill in the host name with the logged-in user's name
		txtHost.Text = Session.CurrentUser?.Name ?? string.Empty;
	}

	// Checks that every field has data, then returns to the Events page
	private async void OnSaveClicked(object sender, EventArgs e)
	{
		bool anyBlank =
			string.IsNullOrWhiteSpace(txtHost.Text) ||
			string.IsNullOrWhiteSpace(txtEventName.Text) ||
			string.IsNullOrWhiteSpace(txtAddress.Text) ||
			string.IsNullOrWhiteSpace(txtMaxAttendees.Text);

		if (anyBlank)
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		// TODO (Week 3): save the new event to local storage
		await Navigation.PopAsync();
	}

	// Goes back without saving
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}