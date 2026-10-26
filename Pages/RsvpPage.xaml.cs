using RSVPApp.DataAccess;
using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a user RSVP for an event. Saves the RSVP to the Rsvps table.
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

	// Checks the fields, blocks double RSVPs, then saves
	private async void OnSubmitClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtName.Text) ||
			string.IsNullOrWhiteSpace(txtEmail.Text) ||
			string.IsNullOrWhiteSpace(txtPhone.Text))
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		User? user = Session.CurrentUser;
		string email = txtEmail.Text.Trim();

		bool alreadyRsvpd = await RsvpDatabase.Instance.HasRsvpAsync(rsvpEvent.Id, user?.Id, email);
		if (alreadyRsvpd)
		{
			lblError.Text = "You have already RSVP'd for this event.";
			return;
		}

		// TODO (Week 4): check the RSVP deadline and max attendees

		var rsvp = new Rsvp
		{
			EventId = rsvpEvent.Id,
			UserId = user?.Id,
			Name = txtName.Text.Trim(),
			Email = email,
			Phone = txtPhone.Text.Trim(),
			RsvpDate = DateTime.Now
		};

		await RsvpDatabase.Instance.AddRsvpAsync(rsvp);
		await DisplayAlert("RSVP Confirmed", $"You're signed up for {rsvpEvent.Name}.", "OK");
		await Navigation.PopAsync();
	}

	// Goes back without submitting
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}