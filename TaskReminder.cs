using Godot;

public partial class TaskReminder : Control {
	[Export]
	AnimationPlayer animationPlayer;
	
	[Export]
	public Godot.Collections.Array<string> tasks; 

	public override void _Ready() {
		animationPlayer.Play("spin");
		TextScroller.Tasks = tasks;
	}

	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Click")) animationPlayer.PlayBackwards("spin");
		if (Input.IsActionJustReleased("Click")) animationPlayer.Play("spin");
	}
}
