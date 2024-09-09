using Godot;

public partial class DoRoutine : Control
{
	[Export]
	HSlider progress;
	[Export]
	BoxContainer checkboxParent;
	[Export]
	Label progressText;

	[Export]
	AnimationPlayer barAnimation, textAnimation;
	[Export]
	Button doWorkoutInMorning;

	[Export]
	Godot.Collections.Array<CheckBox> checkBoxes = new();

	private string GetProgressText(int progress) => $"{progress}/{checkBoxes.Count}";
	public override void _Ready()
	{
		int totalToggled = 0;

		void Toggled(bool toggle)
		{
			if (toggle) SetUpBarAnimation(++totalToggled);
			else SetUpBarAnimation(--totalToggled);

			SetUpTextAnimation(toggle);
			progressText.Text = GetProgressText(totalToggled);
			
			CallDeferred("PlayAnimations");
		}
		
		// play animation if progress is completed
		barAnimation.AnimationFinished += (_) => 
		{
			if (progress.Ratio >= 1) GetParent<Window>().Visible = false;
		};

		foreach (var checkBox in checkBoxes)
		{
			checkBox.Toggled += Toggled; 	
		}

		progress.MaxValue = checkBoxes.Count;

		doWorkoutInMorning.Pressed += () => {
			checkBoxes[1].Disabled = !checkBoxes[1].Disabled; 
			checkBoxes[1].ButtonPressed = true;
		};
	}

	Color increase = new("c8ff88"), decrease = new("eda4b4");
	private void SetUpTextAnimation(bool increased)
	{
		var anim = textAnimation.GetAnimation("increase");
		anim.TrackSetKeyValue(0, 1, increased ? increase : decrease);
	}
	
	private void SetUpBarAnimation(int newTotal)
	{
		var anim = barAnimation.GetAnimation("slide");
		// Set the new start to the old end.
		anim.TrackSetKeyValue(0, 0, progress.Value);
		anim.TrackSetKeyValue(0, 1, (double) newTotal);
	}

	private void PlayAnimations()
	{
		barAnimation.Stop();
		barAnimation.Play("slide");

		textAnimation.Stop();
		textAnimation.Play("increase");
	}
}
