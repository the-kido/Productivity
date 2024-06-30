using Godot;

public partial class Pomodoro : Panel
{
	[Export]
	Button pause, close;
	[Export]
	Label time;
	[Export]
	Button switchBetweenPeriods;
	[Export]
	AnimationPlayer openCloseAnimations, shrinkExpandAnimations;
	[Export]
	HSlider progress;


	const string PLAY_TEXT = "▶";
	const string PAUSE_TEXT = "||";
	const string WORK_TO_BREAK = "Switch to break", BREAK_TO_WORK = "Switch to work";

    int startTimeSeconds, endTimeSeconds;

	int workTimeMinutes, breakTimeMinutes;

	public void Open(int workTime, int breakTime) {
		progress.Value = 1f;
		
		openCloseAnimations.Play("open");
		
		workTimeMinutes = workTime;
		breakTimeMinutes = breakTime;
		
		UpdateEndTime();
	}

	bool paused = false;
	bool inWorkPeriod = true; // We will start the thing during the work period.

	private void UpdateEndTime() {
		timeLeftSeconds = inWorkPeriod ? startTimeSeconds + workTimeMinutes * 60 : startTimeSeconds + breakTimeMinutes * 60;
    }

	public override void _Ready() {
		
		switchBetweenPeriods.Pressed += OnSwitchedPressed;
		close.Pressed += Close;
		pause.Pressed += Pause;
	}

	private void Pause() {
		paused = !paused;
		pause.Text = paused ? PLAY_TEXT : PAUSE_TEXT;
	}

	private void Close() {
		openCloseAnimations.Play("close");
	}

	private void OnSwitchedPressed() {
		progress.Value = 1f;

		inWorkPeriod = !inWorkPeriod;
		// Do not allow skipping during break
		if (inWorkPeriod) switchBetweenPeriods.Disabled = true;
		UpdateEndTime();

		switchBetweenPeriods.Text = inWorkPeriod ? WORK_TO_BREAK : BREAK_TO_WORK;
	}

	double loopTime = 0;
	int timeLeftSeconds = 0;

	private void Finished() {
		switchBetweenPeriods.Disabled = false;
	}

	bool isBig = true;

	public override void _Process(double delta) {
		if (Input.IsActionPressed("Shift") && Input.IsActionJustPressed("Click")) {
			if (isBig) shrinkExpandAnimations.Play("bigToSmall");
			else shrinkExpandAnimations.PlayBackwards("bigToSmall");
			isBig = !isBig;
		}

		if (!Visible || paused || timeLeftSeconds < 0) return;
		
		if (timeLeftSeconds == 0) { 
			timeLeftSeconds = -1;
			Finished();
			return;
		}

		loopTime += delta;		

		if (loopTime > 1) {
			loopTime = 0;
			timeLeftSeconds--;
			float newValue = (float) timeLeftSeconds / (inWorkPeriod ? workTimeMinutes : breakTimeMinutes) / 60f - 1f;
			progress.Value = newValue;
		}

		time.Text = Utils.TimerText(0, timeLeftSeconds / 60, timeLeftSeconds % 60);
	}
}
