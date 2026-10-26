using RSVPApp.DataAccess;
using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Lets a logged-in user add a new event. Saves it to the Events table.
/// </summary>
public partial class AddEventPage : ContentPage
{
	public AddEventPage()
	{
		InitializeComponent();

		// Fill in the host name with the logged-in user's name
		txtHost.Text = Session.CurrentUser?.Name ?? string.Empty;
	}

	// Checks every field, saves the event, then returns to the Events page
	private async void OnSaveClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(txtHost.Text) ||
			string.IsNullOrWhiteSpace(txtEventName.Text) ||
			string.IsNullOrWhiteSpace(txtAddress.Text) ||
			string.IsNullOrWhiteSpace(txtMaxAttendees.Text))
		{
			lblError.Text = "Please fill in all fields.";
			return;
		}

		// Max attendees must be a whole number above 0
		if (!int.TryParse(txtMaxAttendees.Text, out int max) || max <= 0)
		{
			lblError.Text = "Maximum attendees must be a number greater than 0.";
			return;
		}

		// Read the date, time and deadline from the pickers
		if (dpEventDate.Date is not DateTime eventDay ||
			tpEventTime.Time is not TimeSpan eventTime ||
			dpDeadline.Date is not DateTime deadline)
		{
			lblError.Text = "Please pick the event date, time and RSVP deadline.";
			return;
		}

		DateTime eventDate = eventDay.Date + eventTime;

		var newEvent = new RsvpEvent
		{
			HostUserId = Session.CurrentUser?.Id ?? 0,
			HostName = txtHost.Text.Trim(),
			Name = txtEventName.Text.Trim(),
			Address = txtAddress.Text.Trim(),
			MaxAttendees = max,
			EventDate = eventDate,
			RsvpDeadline = deadline.Date
		};

		await RsvpDatabase.Instance.AddEventAsync(newEvent);
		await Navigation.PopAsync();
	}

	// Goes back without saving
	private async void OnCancelClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}