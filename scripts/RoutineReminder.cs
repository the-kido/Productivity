using System;
using System.Threading.Tasks;
using Godot;

public partial class RoutineReminder : Panel {
	const string ROUTINE_APP_LOCATION = "C:\\Users\\alpit\\OneDrive\\Documents\\Krish\\Other Godot\\POC\\Exports\\export.exe";
	[Export]
	bool disableApp = false;

	[Export]
	int afternoonHour;

	[Export]
	CheckBox routineLabel;

	// In minutes
	const int CHECKUP_PERIOD = 30;
	const string SAVE_FILE_LOCATION = "user://routine.json";

	// Required for timing when to give the next prompt.
	readonly int minuteOpened;
	RoutineReminder() => minuteOpened = DateTime.Now.TimeOfDay.Minutes;

	public override void _Ready() {
		if (!routineLabel.ButtonPressed) Update();
		if (DateTime.Now.TimeOfDay.Hours >= afternoonHour) UpdateToAfternoon();

		routineLabel.Pressed += OnPressed;
		// Do last in case we update to afternoon.
		routineLabel.ButtonPressed = LoadIfPressed();
		// it's gotta be pressed!
		GD.Print(routineLabel.ButtonPressed);
	} 
	private bool LoadIfPressed() {
		using FileAccess saveFile = FileAccess.Open(SAVE_FILE_LOCATION, FileAccess.ModeFlags.Read);
		if (saveFile is null) return false;

        return saveFile.GetLine() == "true";
	}

	private void Save() {
		using FileAccess saveFile = FileAccess.Open(SAVE_FILE_LOCATION, FileAccess.ModeFlags.Write);
        saveFile.StoreLine(Json.Stringify(routineLabel.ButtonPressed));
	}

	int currentHour = 0;

	bool HourChanged => currentHour != DateTime.Now.TimeOfDay.Hours;

	async void Update() {
		if (HourChanged && DateTime.Now.TimeOfDay.Hours == afternoonHour) UpdateToAfternoon();

		if ((DateTime.Now.TimeOfDay.Minutes + minuteOpened) % CHECKUP_PERIOD == 0) OpenApp();

		currentHour = DateTime.Now.TimeOfDay.Hours;
		
		// Waits 1 minute to loop for performance gains
		await Task.Delay(1000*60);
		Update();
	}

	private void UpdateToAfternoon() {
		routineLabel.Text = "Finished Afternoon Routine";
		routineLabel.ButtonPressed = false;
	}

	// Open app if button isn't pressed. Button will automagically be reset after 
	// the given time in "afternoonHour" is passed
	private void OpenApp() {
		if (disableApp) return;
		if (!routineLabel.ButtonPressed) System.Diagnostics.Process.Start(ROUTINE_APP_LOCATION);
	}

	Color defaultColor = new(1, 1, 1);
	Color pressedColor = new(1.0f, 1.15f, 1.2f);
	private void OnPressed() {
		Save();
		ChangeColor();
	}

	private void ChangeColor() {
		if (routineLabel.ButtonPressed) Modulate = pressedColor;
        else Modulate = defaultColor;
    }
}
