using Godot;
using Microsoft.VisualBasic;
using System.Linq;
using System.Threading.Tasks;

public partial class SleepReminderBetter : Panel
{
	[Export] Button buttonGood, buttonGreat, buttonBad;
	[Export] TextEdit reason;
	[Export] Button finish;

	// Called when the node enters the scene tree for the first time.
	Color gray = new("ffffff30");
	public override void _Ready()
	{
		Button[] arr = { buttonGood, buttonGreat, buttonBad };
		foreach (Button button in arr)
		{
			button.Pressed += () =>
			{
				arr.Where((item) => item != button).ToList().ForEach((item) => item.Modulate = gray);
				foreach (Button _button in arr) _button.Disabled = true;
				reason.Visible = true;
			};
		}

		reason.TextChanged += () =>
		{
			finish.Visible = true;
		};

		finish.Pressed += () =>
		{
			(GetParent() as Window).Visible = false;
		};

        _ = Loop();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.

	bool stopEverything = false;
	private async Task Loop()
	{
		if (stopEverything) return;

		Update();
		await Task.Delay(5000);
		_ = Loop();
	}

	const int REMINDER_MINUTE = (12 + 8) * 60 + 30;
	private void Update()
	{
		if (DateAndTime.Now.Hour * 60 + DateAndTime.Now.Minute > REMINDER_MINUTE)
		{
			(GetParent() as Window).Visible = true;
			stopEverything = true;
		}
	}
}
