using Godot;

public partial class FocusModeCreator : Control {
	// Called when the node enters the scene tree for the first time.

	[Export]
	FocusMode focusMode;
	
	[Export]
	Button createTimerButton;

	[Export]
	SpinBox minutes;
	[Export]
	LineEdit reason;


	const string DisabledText = "Cannot create timer yet!";
	const string EnabledText = "Create timer";

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
		bool sufficientInfo = minutes.Value > 0 && !string.IsNullOrEmpty(reason.Text);
		
		if (sufficientInfo) {
			createTimerButton.Disabled = false;
			createTimerButton.Text = EnabledText;
		} else {
			createTimerButton.Disabled = true;
			createTimerButton.Text = DisabledText;
		}
	}
}
