using Godot;
using Microsoft.VisualBasic;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

public partial class SleepConsequences : Panel
{
	const int MINUTE_I_SHOULD_BE_SHUT_DOWN = (12 + 9) * 60 + 30;
	[Export] Button shutDown, wait;

	[Export] Window snoozeBarWindow;
	[Export] AnimationPlayer snoozeBarAnimation;

	int[] snoozes = {5, 3, 2, 1, 1, 1, 1, 1}; // 15 minutes in total. If there's a serious problem, I would shut down the prod app instead of waiting for these timers!
	int indexAt = 0;

	int snoozeTimeMs;

	public override void _Ready()
	{
		wait.Pressed += async () =>
		{
			(GetParent() as Window).Visible = false;
			var a = snoozeBarAnimation.GetAnimation("play");
			a.TrackSetKeyTime(0, 1, snoozeTimeMs / 1000);
			snoozeBarAnimation.Play("play");
			snoozeBarWindow.Visible = true;

			await Task.Delay(snoozeTimeMs);
			Prompt();
		};

		shutDown.Pressed += () =>
		{
			var psi = new ProcessStartInfo("shutdown", "/s /t 0")
			{
				CreateNoWindow = true,
				UseShellExecute = false
			};
			Process.Start(psi);
		};
	}

    private void Prompt()
    {
		snoozeBarAnimation.Stop();
		(GetParent() as Window).Visible = true;
		snoozeBarWindow.Visible = false;

		if (indexAt >= snoozes.Length)
		{
			wait.Disabled = true;
			return;
		}

		int snoozeMinutes = snoozes[indexAt];
        wait.Text = $"Just give me {snoozeMinutes} minute{(snoozeMinutes == 1 ? "" : "s")}";
        snoozeTimeMs = snoozeMinutes * 60 * 1000;

        indexAt++;
    }

    bool timesUp = false;
	public override void _Process(double delta)
	{
		if (timesUp) return;

		if (DateAndTime.Now.Hour * 60 + DateAndTime.Now.Minute > MINUTE_I_SHOULD_BE_SHUT_DOWN)
		{
			Prompt();
			timesUp = true;
		}
	}
}
