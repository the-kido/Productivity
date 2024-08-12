using System;
using System.Threading.Tasks;
using Godot;

public partial class SleepReminder : Panel
{
	[Export]
	RichTextLabel label;
	Window parent;

	public override void _Ready()
	{
		parent = GetParent<Window>();
		Loop();		
	}
	private async void Loop()
	{
		Update();
		await Task.Delay(5000);
		Loop(); // Loop the update
	}

	const string
		PAST_9 = "[center][pulse freq=1]Get ready to sleep!", 
		PAST_9_30 = "[center][shake rate=20.0 level=5 connected=1][pulse freq=1]Get ready to sleep!",
		PAST_10 = @"[center][rainbow freq=1 sat=0.2 val=0.9][tornado radius=10.0 freq=3.0 connected=1]G o   t o 
s l e e p ! !";

	const int temp = 14;

	const int 
		MINUTES_TO_10 = (temp + 1) * 60,
		MINUTES_TO_9 = temp * 60,
		MINUTES_TO_9_30 = temp * 60 + 30;

	private void Update()
	{
		DateTime dateTime = DateTime.Now;
		int minutes = dateTime.Minute + dateTime.Hour * 60;

		// Make window visible now
		if (!parent.Visible && minutes >= MINUTES_TO_9) parent.Visible = true;

		string reminder = minutes switch
        {
            >= MINUTES_TO_10 => PAST_10,
            >= MINUTES_TO_9_30 => PAST_9_30,
            >= MINUTES_TO_9 => PAST_9,
            _ => "Why do you see this"
        };

		UpdateLabel(reminder);
    }

	private void UpdateLabel(string bodyText) => label.Text = bodyText;
}
