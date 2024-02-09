using Godot;

public partial class Settings : Control {

	[ExportCategory("Required External Nodes")]
	[Export]
	RoutineReminder routineReminder;
	[Export]
	HourlyCheckup hourlyCheckup;
	[Export]
	Button globalMinimizeButton; 
	[ExportCategory("Required Internal Nodes")]
	[Export]
	AnimationPlayer animationPlayer;
	[Export]
	Button settingsButton;

	[ExportCategory("Settings")]
	[Export]
	CheckBox showAnimations;
	[Export]
	Button forceExit;

	bool opened = false;
	private void ToggleSettings() {
		opened = !opened;
		settingsButton.Text = opened ? "Close\nSettings" : "Open\nSettings";
		animationPlayer.Play("open", customSpeed: opened ? 1 : -1, fromEnd: !opened);

		forceExit.Text = EXIT_DEFAULT;
		strikes = 0;

	}
	
	public override void _Ready() {
		animationPlayer.Play("reset"); // in case of reasons

		showAnimations.Toggled += ToggleShowAnimations;
		ToggleShowAnimations(true); // Default them to be on.

		settingsButton.Pressed += ToggleSettings;
		// If settings is opened while minimizing, settings should close too
		globalMinimizeButton.Pressed += () => { if (opened) ToggleSettings();};

		forceExit.Pressed += ForceExitPressed;
	}

	int strikes = 0; // Counts the number of times force exit is pressed.
	const string EXIT_DEFAULT = "Force\nClose";
	const string EXIT_WARNING = "You\nSure?";
	private void ForceExitPressed() {
		strikes++;
		switch (strikes) {
			case 1:
				forceExit.Text = EXIT_WARNING;
				break;
			case 2:
				GetTree().Quit();
				return;
		}
	}

	private void ToggleShowAnimations(bool toggle) {
		routineReminder.ToggleNotifications(toggle);
		hourlyCheckup.ToggleCheckup(toggle);
	}
}
