using RSVPApp.Models;

namespace RSVPApp.Pages;

/// <summary>
/// Shows all details for one event, including who is attending.
/// </summary>
public partial class EventDetailsPage : ContentPage
{
	// The event being shown on this page
	private readonly RsvpEvent rsvpEvent;

	public EventDetailsPage(RsvpEvent selectedEvent)
	{
		InitializeComponent();
		rsvpEvent = selectedEvent;
		ShowDetails();
	}

	// Fills in the labels with the event's info
	private void ShowDetails()
	{
		lblName.Text = rsvpEvent.Name;
		lblHost.Text = $"Host: {rsvpEvent.HostName}";
		lblAddress.Text = $"Address: {rsvpEvent.Address}";
		lblDate.Text = $"Date and Time: {rsvpEvent.DateDisplay}";
		lblDeadline.Text = $"RSVP Deadline: {rsvpEvent.RsvpDeadline:ddd, MMM d, yyyy}";
		lblMax.Text = $"Maximum Attendees: {rsvpEvent.MaxAttendees}";
		lblCount.Text = $"Currently Attending: {rsvpEvent.AttendeeCount}";

		// Add one label per attendee name
		stackAttendees.Children.Clear();
		if (rsvpEvent.Attendees.Count == 0)
		{
			stackAttendees.Children.Add(new Label { Text = "No one has RSVP'd yet." });
		}
		else
		{
			foreach (string name in rsvpEvent.Attendees)
			{
				stackAttendees.Children.Add(new Label { Text = $"• {name}" });
			}
		}

		// TODO (Week 3): check the RSVP deadline, max attendees and duplicate RSVPs
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