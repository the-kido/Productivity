using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class HourlyCheckup : Control {
	[Export]
	// The actual button is slightly larger so that the game isn't frustrating.
	Button visual, actual;

	Window window; 
	[Export]
	AnimationPlayer animationPlayer;

	// in milliseconds
	const int HOUR = 1000 * 60 * 60;

    readonly List<string> possibleMessages = new(new string[] {
		"Relax in real life",
		"Youtube is bad for health",
		"Is there something more healthy I could be doing?",
		"Get off of the screen",
		"No short hits of dopamine for you!",
		"What are you gaining by doing this right now!?",
	});

	string[] currentWords;

    static int GetRandomIndex(int range) => (int) (new Random().NextSingle() * range);
	private string[] GetNewWords() {
		
		List<string> duplicateArray = possibleMessages.ToList();
		string string1, string2;
		
		// Get item 1
		int index1 = GetRandomIndex(duplicateArray.Count);
		string1 = duplicateArray[index1];
		duplicateArray.RemoveAt(index1);
		
		string1 += " ..."; // Add this to seperate words b/c yes. Spacing is specific to avoid splitting wierdly

		// Get item 2
		int index2 = GetRandomIndex(duplicateArray.Count);
		string2 = duplicateArray[index2];

		return string1.Split(" ").Concat(string2.Split(" ")).ToArray();
	} 

	bool enabled = true;
	public void ToggleCheckup(bool toggle) => enabled = toggle;

	public override void _Ready() {
		window = GetParent<Window>();
		actual.Pressed += UpdateButton;
		ShowEveryHour();
	}

	private void OpenJumpscare() {
		if (!enabled) return;

		currentWords = GetNewWords();
		window.Visible = true;
		SwitchBGColor(false);
		UpdateButton();
		animationPlayer.Play("Play");

		window.Position = new((int) PossibleShift(1920, Size.X), (int) PossibleShift(1080, Size.Y));
	}
	async void ShowEveryHour() {
		await Task.Delay(HOUR);
		OpenJumpscare();
		ShowEveryHour();
	}

	int wordIndex = 0;
	private void Close() {
		window.Visible = false;
		wordIndex = 0;
		animationPlayer.Play("Quiet");
		afkTimer = 0;
	}

	readonly Color OTHER = new("d6e6ff");
	private void SwitchBGColor(bool @bool) => Modulate = @bool ? OTHER : Colors.White;

	private void UpdateButton() {
		if (wordIndex >= currentWords.Length) {
			Close();
			return;
		}
		
		if (currentWords[wordIndex] == "...") SwitchBGColor(true);
		

		visual.Text = currentWords[wordIndex];
		wordIndex++;
		CallDeferred("UpdateButton2");
	}

	private static float PossibleShift(int max, float axisSize) => new Random().NextSingle() * (max - axisSize);

	private void UpdateButton2() {
		visual.Position = new(PossibleShift(300, visual.Size.X), PossibleShift(300, visual.Size.Y));
	}

	double afkTimer = 0;
    public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Escape") && Input.IsActionPressed("Shift")) Close();

		if (window.Visible) {
			afkTimer += delta;
			if (Input.IsActionPressed("Click")) afkTimer = 0;
			if (afkTimer >= 30) Close();
		} 
    }
}
