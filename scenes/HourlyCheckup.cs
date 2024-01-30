using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

public partial class HourlyCheckup : Control {
	[Export]
	Button button;

	Window window; 
	[Export]
	AnimationPlayer animationPlayer;

	List<string> possibleMessages = new(
		new string[] {"Make sure I am using my time effectively",
		"Don't be a silly goober",
		"Hopefully you are actually doing something USEFUL right now, right!?",
		"I can count on you to be responsible with your time :)"}
	);

	string[] currentWords;
	int getRandomIndex(int range) => (int) (new Random().NextSingle() * range);
	private string[] GetNewWords() {
		
		List<string> duplicateArray = possibleMessages.ToList();
		string string1, string2;
		
		// Get item 1
		int index1 = getRandomIndex(duplicateArray.Count);
		string1 = duplicateArray[index1];
		duplicateArray.RemoveAt(index1);
		
		// Get item 2
		int index2 = getRandomIndex(duplicateArray.Count);
		string2 = duplicateArray[index2];

		return string1.Split(" ").Concat(string2.Split(" ")).ToArray();
	} 
	
	public override void _Ready() {
		window = GetParent<Window>();
		button.Pressed += UpdateButton;
		ShowEveryHour();
	}

	private void Start() {
		currentWords = GetNewWords();
		window.Visible = true;
		UpdateButton();
		animationPlayer.Play("Play");

		window.Position = new((int) PossibleShift(1920, Size.X), (int) PossibleShift(1080, Size.Y));
	}

	async void ShowEveryHour() {
		await Task.Delay(1000 * 60 * 60);
		Start();
		ShowEveryHour();
	}

	int wordIndex = 0;
	private void Close() {
		window.Visible = false;
		wordIndex = 0;
		animationPlayer.Play("Quiet");
	}

	private void UpdateButton() {
		if (wordIndex >= currentWords.Length) {
			Close();
			return;
		}

		button.Text = currentWords[wordIndex];
		wordIndex++;
		CallDeferred("UpdateButton2");

	}
	// I don't know a better way of doing this.
	private static float PossibleShift(int max, float axisSize) => new Random().NextSingle() * (max - axisSize);

	private void UpdateButton2() {
		button.Position = new(PossibleShift(300, button.Size.X), PossibleShift(300, button.Size.Y));
	}

    public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Escape") && Input.IsActionPressed("Shift")) Close();
    }
}
