using System;
using System.Threading.Tasks;
using Godot;

public partial class RoutineReminder : Panel {
	const string RoutineAppLocation = "C:\\Users\\alpit\\OneDrive\\Documents\\Krish\\Other Godot\\POC\\Exports\\export.exe";
	[Export]
	bool disableApp = false;

	[Export]
	int afternoonHour;

	[Export]
	CheckBox routineLabel;

	public override void _Ready() {
		Update();
		OpenApp();

		routineLabel.Pressed += ChangeColor;

	} 


	// Required for timing when to give the next prompt.
	readonly int minuteOpened;
	RoutineReminder() => minuteOpened = DateTime.Now.TimeOfDay.Minutes;

	int currentHour = 0;

	bool HourChanged => currentHour != DateTime.Now.TimeOfDay.Hours;

	async void Update() {
		if (HourChanged && DateTime.Now.TimeOfDay.Hours == afternoonHour) UpdateToAfternoon();

		if ((DateTime.Now.TimeOfDay.Minutes + minuteOpened) % 30 == 0) OpenApp();

		currentHour = DateTime.Now.TimeOfDay.Hours;
		
		// Waits 1 minute to loop for performance gains
		await Task.Delay(1*1000*60);
		Update();
	}

	private void UpdateToAfternoon() {
		routineLabel.Text = "Finish Afternoon Routine";
		routineLabel.ButtonPressed = false;
	}

	// Open app if button isn't pressed. Button will automagically be reset after 
	// the given time in "afternoonHour" is passed
	private void OpenApp() {
		if (disableApp) return;
		if (!routineLabel.ButtonPressed) System.Diagnostics.Process.Start(RoutineAppLocation);
	}

	
	Color defaultColor = new(1, 1, 1);
	Color pressedColor = new(1.0f, 1.15f, 1.2f);
	private void ChangeColor() {
		if (routineLabel.ButtonPressed) Modulate = pressedColor;
        else Modulate = defaultColor;
    }
}
