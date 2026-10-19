using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Shows a list of events. Users can filter by All, Attending or Hosting.
/// Guests can only see All events.
/// </summary>
public partial class EventsPage : ContentPage
{
	// The filter currently in use: "All", "Attending" or "Hosting"
	private string currentFilter = "All";

	public EventsPage()
	{
		InitializeComponent();

		if (Session.IsGuest)
		{
			// Guests can't filter by their own events or add events
			lblWelcome.Text = "Welcome, Guest";
			btnAttending.IsEnabled = false;
			btnHosting.IsEnabled = false;
			btnAddEvent.IsVisible = false;
		}
		else
		{
			// After login, show the events the user is signed up for first
			lblWelcome.Text = $"Welcome, {Session.CurrentUser!.Name}";
			currentFilter = "Attending";
		}
	}

	// Reloads the list each time the page shows
	protected override void OnAppearing()
	{
		base.OnAppearing();
		LoadEvents();
	}

	// Fills the list based on the current filter
	private void LoadEvents()
	{
		IEnumerable<RsvpEvent> events = SampleData.Events;
		User? user = Session.CurrentUser;

		if (currentFilter == "Attending" && user != null)
		{
			events = events.Where(ev => ev.Attendees.Contains(user.Name));
		}
		else if (currentFilter == "Hosting" && user != null)
		{
			events = events.Where(ev => ev.HostName == user.Name);
		}

		cvEvents.ItemsSource = events.ToList();
		lblFilter.Text = $"Showing: {currentFilter} Events";
	}

	private void OnAllClicked(object sender, EventArgs e)
	{
		currentFilter = "All";
		LoadEvents();
	}

	private void OnAttendingClicked(object sender, EventArgs e)
	{
		currentFilter = "Attending";
		LoadEvents();
	}

	private void OnHostingClicked(object sender, EventArgs e)
	{
		currentFilter = "Hosting";
		LoadEvents();
	}

	// Opens the details page for the tapped event
	private async void OnEventSelected(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is RsvpEvent selected)
		{
			// Clear the selection so the same event can be tapped again later
			cvEvents.SelectedItem = null;
			await Navigation.PushAsync(new EventDetailsPage(selected));
		}
	}

	// Opens the Add Event page
	private async void OnAddEventClicked(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new AddEventPage());
	}

	// Logs out, then returns to the Login page
	private async void OnLogoutClicked(object sender, EventArgs e)
	{
		Session.Logout();
		await Navigation.PopToRootAsync();
	}
}