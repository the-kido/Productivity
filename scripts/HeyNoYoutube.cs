using Godot;
using System.Linq;
using System.Threading.Tasks;

public partial class HeyNoYoutube : Control
{
	[Export]
	HSlider slider;
	[Export]
	Label time;

	[Export]
	Button commitTime;
	[Export]
	SpinBox customCommitedTime;
	[Export]
	Button btnUp, btnDown, btnLeft, btnRight;
	[Export]
	AnimationPlayer animationPlayer, angryAnimation, cycleThroughSuggestions;
	[Export]
	Label suggestionText;

	Window parent;

	int commitedTimeSeconds = (int) (1.5 * 3600); // Default is 1.5 hours


	Vector2I left = new(30, 930), right = new(3510, 930), up = new(1770, 30), down = new(1770, 1830);


	ChromeTabDetector detector;
	
	private void Associate(Button button, Vector2I direction) {
		button.Pressed += () => {
			parent.Position = direction;
			// idk why this isn't working :( 

			// var anim = animationPlayer.GetAnimation("move");
			
			// anim.TrackSetKeyValue(0, 0, parent.Position);
			// anim.TrackSetKeyValue(0, 1, direction);
			// anim.TrackSetKeyValue(0, 2, direction);
			
			// GD.Print("After");
			// GD.Print(anim.TrackGetKeyValue(0, 0));
			// GD.Print(anim.TrackGetKeyValue(0, 1));
			// GD.Print(anim.TrackGetKeyValue(0, 2));
			// GD.Print(anim.ValueTrackGetUpdateMode(0));

			// GD.Print("huhuh", button.Name);

			// CallDeferred("PlayAnimation");
		};
	}

	private void PlayAnimation() {
		animationPlayer.Play("move");
	}

	public override async void _Ready() {
		ChooseNewText();
		
		parent = GetParent<Window>();
		
		Associate(btnLeft, left);
		Associate(btnRight, right);
		Associate(btnUp, up);
		Associate(btnDown, down);

		detector = new();
		ProcessMode = ProcessModeEnum.Disabled;
		await Task.Delay(10);
		ProcessMode = ProcessModeEnum.Always;

		commitTime.Pressed += () => {
			commitedTimeSeconds = (int) (customCommitedTime.Value * 3600);
			commitTime.Disabled = true;
			customCommitedTime.Editable = false;

			// In case we change the time when it goes into danger zone
			angryIsPlaying = false; 
			angryAnimation.Stop();
			warnIsPlaying = false;
		};
	}

	double timeSpent = 0;
	bool showingWindow = false;
	public override void _Process(double delta)
	{
		if (detector.OnYoutube) {
			if (!parent.Visible) animationPlayer.Play("Open");
			timeSpent += delta;
		} else
			if (parent.Visible) animationPlayer.PlayBackwards("Open");

		int timeSpentSeconds = (int) timeSpent;

		time.Text = Utils.TimerText(timeSpentSeconds / 3600, timeSpentSeconds % 3600 / 60, timeSpentSeconds % 60);
		slider.Value = 1.0 * timeSpent / commitedTimeSeconds;
		
		// Switch text every 10 seconds
		if (timeSpentSeconds % 10 == 0)
		{
			cycleThroughSuggestions.Play("suggest");
		}

		if (slider.Value >= 0.85 && !warnIsPlaying)
		{
			warnIsPlaying = true;
			angryAnimation.Play("warn");
		}
		
		if (timeSpentSeconds >= commitedTimeSeconds && !angryIsPlaying) 
		{
			angryIsPlaying = true;
			angryAnimation.Play("pulse");
		}
	}
	bool angryIsPlaying = false, warnIsPlaying = false;

	static readonly string[] thingsToDo = {
		"Go through your QMB",
		"Get some rest instead",
		"Go outside",
		"Did you finish everything in your QMB?",
		"Progress your career",
		"Develop your experience",
	};
	int index = 0;

	public void ChooseNewText()
	{
		suggestionText.Text = $"ⓘ {thingsToDo[index]}";
		index = (index + 1) % thingsToDo.Length;
	}
}

