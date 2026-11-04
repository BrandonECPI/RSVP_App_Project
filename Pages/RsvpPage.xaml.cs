using RSVPApp.DataAccess;
using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a user RSVP for an event. Sends the RSVP to the web API,
/// which checks the deadline, capacity plus duplicates.
/// </summary>
public partial class RsvpPage : ContentPage
{
	// The event the user is RSVPing for
	private readonly RsvpEvent rsvpEvent;

	public RsvpPage(RsvpEvent selectedEvent)
	{
		InitializeComponent();
		rsvpEvent = selectedEvent;
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

	// Checks the fields, then sends the RSVP to the web API
	private async void OnSubmitClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtName.Text) ||
			string.IsNullOrWhiteSpace(txtEmail.Text) ||
			string.IsNullOrWhiteSpace(txtPhone.Text))
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		var rsvp = new Rsvp
		{
			EventId = rsvpEvent.Id,
			UserId = Session.CurrentUser?.Id,
			Name = txtName.Text.Trim(),
			Email = txtEmail.Text.Trim(),
			Phone = txtPhone.Text.Trim(),
			RsvpDate = DateTime.Now
		};

		try
		{
			// Null means it saved; anything else is the reason it didn't
			string? error = await RsvpDatabase.Instance.AddRsvpAsync(rsvp);
			if (error != null)
			{
				lblError.Text = error;
				return;
			}

			await DisplayAlert("RSVP Confirmed", $"You're signed up for {rsvpEvent.Name}.", "OK");
			await Navigation.PopAsync();
		}
		catch (HttpRequestException)
		{
			lblError.Text = "Could not reach the server. Make sure the web API is running.";
		}
	}

	// Goes back without submitting
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}