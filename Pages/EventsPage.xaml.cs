using RSVPApp.DataAccess;
using RSVPApp.Models;
using RSVPApp.Services;

namespace RSVPApp.Pages;

/// <summary>
/// Shows events from the database. Users can filter by All, Attending or Hosting.
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

	// Reloads the list each time the page shows (for example, after adding an event)
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await LoadEventsAsync();
	}

	// Gets events from the database based on the current filter
	private async Task LoadEventsAsync()
	{
		User? user = Session.CurrentUser;
		List<RsvpEvent> events;

		if (currentFilter == "Attending" && user != null)
		{
			events = await RsvpDatabase.Instance.GetAttendingEventsAsync(user.Id);
		}
		else if (currentFilter == "Hosting" && user != null)
		{
			events = await RsvpDatabase.Instance.GetHostingEventsAsync(user.Id);
		}
		else
		{
			events = await RsvpDatabase.Instance.GetAllEventsAsync();
		}

		cvEvents.ItemsSource = events;
		lblFilter.Text = $"Showing: {currentFilter} Events";
	}

	private async void OnAllClicked(object sender, EventArgs e)
	{
		currentFilter = "All";
		await LoadEventsAsync();
	}

	private async void OnAttendingClicked(object sender, EventArgs e)
	{
		currentFilter = "Attending";
		await LoadEventsAsync();
	}

	private async void OnHostingClicked(object sender, EventArgs e)
	{
		currentFilter = "Hosting";
		await LoadEventsAsync();
	}

	// Opens the details page for the tapped event
	private async void OnEventSelected(object sender, SelectionChangedEventArgs e)
	{
		if (e.CurrentSelection.FirstOrDefault() is RsvpEvent selected)
		{
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