using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a user RSVP for an event.
/// RSVP saving is not functional yet (Week 3).
/// </summary>
public partial class RsvpPage : ContentPage
{
	public RsvpPage(RsvpEvent rsvpEvent)
	{
		InitializeComponent();
		lblEventName.Text = rsvpEvent.Name;

		// If logged in, fill in the fields with the user's saved info.
		// Guests start with blank fields.
		User? user = Session.CurrentUser;
		if (user != null)
		{
			txtName.Text = user.Name;
			txtEmail.Text = user.Email;
			txtPhone.Text = user.Phone;
		}
	}

	// Checks that every field has data, then returns to the details page
	private async void OnSubmitClicked(object sender, EventArgs e)
	{
		bool anyBlank =
			string.IsNullOrWhiteSpace(txtName.Text) ||
			string.IsNullOrWhiteSpace(txtEmail.Text) ||
			string.IsNullOrWhiteSpace(txtPhone.Text);

		if (anyBlank)
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		// TODO (Week 3): add this person to the event's attendee list
		await Navigation.PopAsync();
	}

	// Goes back without submitting
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}