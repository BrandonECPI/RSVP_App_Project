using RSVPApp.DataAccess;
using RSVPApp.Models;

namespace RSVPApp.Pages;

/// <summary>
/// Shows all details for one event, including attendees from the database.
/// </summary>
public partial class EventDetailsPage : ContentPage
{
	// The event being shown on this page
	private readonly RsvpEvent rsvpEvent;

	public EventDetailsPage(RsvpEvent selectedEvent)
	{
		InitializeComponent();
		rsvpEvent = selectedEvent;

		lblName.Text = rsvpEvent.Name;
		lblHost.Text = $"Host: {rsvpEvent.HostName}";
		lblAddress.Text = $"Address: {rsvpEvent.Address}";
		lblDate.Text = $"Date and Time: {rsvpEvent.DateDisplay}";
		lblDeadline.Text = $"RSVP Deadline: {rsvpEvent.RsvpDeadline:ddd, MMM d, yyyy}";
		lblMax.Text = $"Maximum Attendees: {rsvpEvent.MaxAttendees}";
	}

	// Reloads attendees each time the page shows (for example, after an RSVP)
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadAttendeesAsync();
	}

	// Gets the attendee names from the Rsvps table
	private async Task LoadAttendeesAsync()
	{
		List<string> names = await RsvpDatabase.Instance.GetAttendeeNamesAsync(rsvpEvent.Id);
		lblCount.Text = $"Currently Attending: {names.Count}";

		stackAttendees.Children.Clear();
		if (names.Count == 0)
		{
			stackAttendees.Children.Add(new Label { Text = "No one has RSVP'd yet." });
		}
		else
		{
			foreach (string name in names)
			{
				stackAttendees.Children.Add(new Label { Text = $"• {name}" });
			}
		}

		// TODO (Week 4): check the RSVP deadline and max attendees
	}

	// Opens the RSVP page for this event
	private async void OnRsvpClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new RsvpPage(rsvpEvent));
	}

	// Returns to the Events page
	private async void OnBackClicked(object sender, EventArgs e)
	{
		await Navigation.PopAsync();
	}
}