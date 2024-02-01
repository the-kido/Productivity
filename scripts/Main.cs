using Godot;

public partial class Main : Control {

	Main() => Instance = this;
	
	public static Main Instance {get; private set;}

	[Export]
	Button minimize;


	[Export]
	AnimationPlayer animationPlayer;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetTree().AutoAcceptQuit = false;
		minimize.Pressed += () =>  animationPlayer.Play("Minimize");

		FindChild("Main background").GetChild<AnimationPlayer>(0).Play("spin");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Escape")) animationPlayer.Play("Minimize");
	}

	public static void Minimize() {
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Minimized);
	}

	private void temp() {
		if (GetTree().Root.HasFocus()) animationPlayer.Play("Open");
	}
	
	public override void _Notification(int what) {
        if (what == MainLoop.NotificationApplicationFocusIn) {
			CallDeferred("temp");
		}
		if (what == MainLoop.NotificationApplicationFocusOut) {

		}
    }
}
