using Godot;

public partial class Settings : Control {
	[ExportCategory("Required External Nodes")]
	[Export]
	RoutineReminder routineReminder;
	[Export]
	HourlyCheckup hourlyCheckup;
	[ExportCategory("Required Internal Nodes")]
	[Export]
	AnimationPlayer animationPlayer;
	[Export]
	Button settingsButton;

	[ExportCategory("Settings")]
	[Export]
	CheckBox showAnimations;

	bool opened = false;
	private void ToggleSettings() {
		opened = !opened;
		animationPlayer.Play("open", customSpeed: opened ? 1 : -1);
	}
	
	public override void _Ready() {
		showAnimations.Toggled += ToggleShowAnimations;
		settingsButton.Pressed += ToggleSettings;
	}

	private void ToggleShowAnimations(bool toggle) {
		routineReminder.ToggleNotifications(toggle);
		hourlyCheckup.ToggleCheckup(toggle);
	}

	public override void _Process(double delta) {
	}
}
