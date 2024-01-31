using Godot;

public partial class FocusTimerCreator : Control {
	// Called when the node enters the scene tree for the first time.

	[Export]
	FocusTimer focusMode;
	
	[Export]
	Button createTimerButton;

	[Export]
	SpinBox minutes;
	[Export]
	LineEdit reason;


	const string DISABLE_TEXT = "Cannot create a new timer";
	const string ENABLE_TEXT = "Create timer";

	public override void _Ready() {
		createTimerButton.Pressed += OnButtonPressed;
	}

	private void OnButtonPressed() {
		focusMode.StartTimer((int) minutes.Value, reason.Text);
		minutes.Value = 0;
		reason.Text = "";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		bool sufficientInfo = minutes.Value > 0 && !string.IsNullOrEmpty(reason.Text) && !focusMode.IsOpened;
		
		if (sufficientInfo) {
			createTimerButton.Disabled = false;
			createTimerButton.Text = ENABLE_TEXT;
		} else {
			createTimerButton.Disabled = true;
			createTimerButton.Text = DISABLE_TEXT;
		}
	}
}
