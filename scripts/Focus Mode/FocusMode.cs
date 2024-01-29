using System.Security.Principal;
using Godot;

public partial class FocusMode : Control {

	[Export]
	Button pause, settings;
	
	[Export]
	Label timer, reasonLabel;

	[Export]
	Control settingsControl;
	
	[Export]
	Window window;
	[Export]
	AnimationPlayer animationPlayer;


    static string Clockify(int number) => (number <= 9 ? "0" : "") + number.ToString();
    static string TimerText(int hours, int minutes, int seconds) =>
		// Include hours if they're there 
		(hours == 0 ? "" : $"{Clockify(hours)}:") +
		// always show minutes and seconds 
		$"{Clockify(minutes)}:{Clockify(seconds)} ";

	public void StartTimer(int minutes, string reason) {
        window.Visible = true;

		seconds = minutes * 60;

		reasonLabel.Text = reason;
		
		UpdateText();
	}
	void UpdateText() {
		timer.Text = TimerText(seconds / 3600, seconds/60  % 60, seconds % 60);
	}

	int seconds = 0;

    public override void _Ready() {
		settings.Pressed += OpenSettingsPanel;
    }

	bool IsSettingsOpen => settingsControl.Visible;
	private void OpenSettingsPanel() {
		
		if (IsSettingsOpen) {
			settingsControl.Visible = false;
		}
		else {
			settingsControl.Visible = true;
		}
	}

	double time = 0;
    public override void _Process(double delta) {
		ControlSize();
		time += delta;

		if (time >= 0.99999) {
			seconds--;
			UpdateText();
			time = 0;
		}
	}

	private void ControlSize() {
		if (Input.IsActionPressed("Right Click") && !animationPlayer.IsPlaying()) {

			if (window.Size.Y != 150) {
				animationPlayer.Play("size_up");
			} else if(window.Size.Y != 110) {
				animationPlayer.Play("size_down");
			}
		}
	}

}
