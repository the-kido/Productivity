using Godot;

public partial class PomodoroBuilder : Panel
{
	[Export]
	Button start;
	[Export]
	SpinBox workTime, breakTime;
	[Export]
	Pomodoro pomodoro;
	
	string startPomodoro = "Start Pomodoro", alreadyOpened = "(Started)";

	public override void _Ready() {
		start.Pressed += ButtonPressed;
	}

	private void ButtonPressed() {
		pomodoro.Open((int) workTime.Value, (int) breakTime.Value);
	}

	public override void _Process(double delta) {
		start.Disabled = pomodoro.Visible;	
		start.Text = pomodoro.Visible ? alreadyOpened : startPomodoro;	
	}
}
