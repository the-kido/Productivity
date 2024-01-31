using Godot;

public partial class TextScroller : Label {
	const int LINE_SIZE = 22;
	
	[Export]
    private double speed = 0.2;

    public static Godot.Collections.Array<string> Tasks {get; set;} 

	string text = "";
	string GetShrunkedText() {
		text = text[1..];
		return text;
	}

	static int lineIndex = 0;
	static string GetNewLine() {
		lineIndex = (lineIndex + 1) % Tasks.Count;
		return new string(' ', 10) + Tasks[lineIndex];
	}

	double time = 0;

	private void LoopText(double delta) {
		time += delta;
		
		// Once we need to add another task, increase:
		if (text.Length <= LINE_SIZE){
			text += GetNewLine();
		}

		if (time >= speed) {
			time = 0;
			Text = GetShrunkedText();
		}
	}

	public override void _Process(double delta) {
		LoopText(delta);
	}
    
}
