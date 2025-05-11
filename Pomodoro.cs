using System.Threading.Tasks;
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
	AnimationPlayer openCloseAnimations, shrinkExpandAnimations, fade;
	[Export]
	HSlider progress;
	[Export]
	DeleteThis deleteThis;


	const string PLAY_TEXT = "▶";
	const string PAUSE_TEXT = "||";
	const string WORK_TO_BREAK = "Switch to break", BREAK_TO_WORK = "Switch to work";

    int startTimeSeconds, endTimeSeconds;

	int workTimeMinutes, breakTimeMinutes;

	public async void Open(int workTime, int breakTime) {
		Window parentWindow = GetParent() as Window;
		
		parentWindow.RequestAttention();
		parentWindow.GrabFocus();

		progress.Value = 0f;
		
		parentWindow.Popup();
		openCloseAnimations.Play("open");
		
		workTimeMinutes = workTime;
		breakTimeMinutes = breakTime;
		
		UpdateEndTime();
		
		// One of these guys are redundant but also why should I care.
		// await Task.Delay(1000);
		// CallDeferred(nameof(ApplyAcrylic));		
	}
	
	private void ApplyAcrylic() => deleteThis.Apply();

	bool paused = false;
	bool inWorkPeriod = true; // We will start the thing during the work period.

	private void UpdateEndTime() {
		timeLeftSeconds = inWorkPeriod ? startTimeSeconds + workTimeMinutes * 60 : startTimeSeconds + breakTimeMinutes * 60;
    }

	public override void _Ready() {
		openCloseAnimations.AnimationFinished += (_) => fade.PlayBackwards("fadeIn");

		MouseEntered += () => fade.Play("fadeIn");
		MouseExited += () => fade.PlayBackwards("fadeIn");

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
		progress.Value = 0f;

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

	bool isHovering = false;
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
			float newValue = 1f - (float) timeLeftSeconds / (inWorkPeriod ? workTimeMinutes : breakTimeMinutes) / 60f;
			progress.Value = newValue;
		}

		time.Text = Utils.TimerText(0, timeLeftSeconds / 60, timeLeftSeconds % 60);
		// (GetParent() as Window).Title = Utils.TimerText(0, timeLeftSeconds / 60, timeLeftSeconds % 60);
	}
}
