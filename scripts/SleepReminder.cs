using System;
using System.Threading.Tasks;
using Godot;

public partial class SleepReminder : Panel
{
	[Export]
	RichTextLabel label;
	[Export]
	AnimationPlayer bigify;
	Window parent;

    public override void _Ready()
	{
		parent = GetParent<Window>();
		Loop();		
	}

	private async void Loop()
	{
		Update();
		await Task.Delay(1000 * 60);
		Loop(); // Loop the update
	}

	const string
		REMINDER_1_TEXT = "[center][pulse freq=1]Get ready to sleep!", 
		REMINDER_2_TEXT = "[center][shake rate=20.0 level=5 connected=1][pulse freq=1]Get ready to sleep!",
		REMINDER_3_TEXT = @"[center][rainbow freq=1 sat=0.2 val=0.9][tornado radius=10.0 freq=3.0 connected=1]G o   t o 
s l e e p ! !",
		REMINDER_4_TEXT = @"[shake][center][rainbow freq=3 sat=0.9 val=3][tornado radius=10.0 freq=10.0 connected=0]G o   t o 
s l e e p ! !"; 

	// const int SLEEP_HOUR = 20, SLEEP_MIN = 30; // 8:30
	const int SLEEP_HOUR = 7, SLEEP_MIN = 14; // 8:30

	const int 
		REMINDER_1_MINS = SLEEP_HOUR * 60 + SLEEP_MIN,
		REMINDER_2_MINS = SLEEP_HOUR * 60 + 30 + SLEEP_MIN,
		REMINDER_3_MINS = SLEEP_HOUR * 60 + 50 + SLEEP_MIN,
		REMINDER_LAST_MINS = (SLEEP_HOUR + 1) * 60 + SLEEP_MIN;

	private void Update()
	{
		DateTime dateTime = DateTime.Now;
		int minutes = dateTime.Minute + dateTime.Hour * 60;

		// Make the window visible once we're 1 hour before sleeping
		if (!parent.Visible && minutes >= REMINDER_1_MINS) parent.Visible = true;

		// Enlarge the window for the biggest reminder when we hit our last reminder
		if (minutes == REMINDER_LAST_MINS) bigify.Play("bigify");

		string reminder = minutes switch
        {
            >= REMINDER_LAST_MINS => REMINDER_4_TEXT,
			>= REMINDER_3_MINS => REMINDER_3_TEXT,
            >= REMINDER_2_MINS => REMINDER_2_TEXT,
            >= REMINDER_1_MINS => REMINDER_1_TEXT,
            _ => "Why do you see this"
        };

		UpdateLabel(reminder);
    }

	private void UpdateLabel(string bodyText) => label.Text = bodyText;
}
