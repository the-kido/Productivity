using Godot;

public partial class Main : Control {

	Main() => Instance = this;
	
	public static Main Instance {get; private set;}

	[Export]
	Button closeButton;

	[Export]
	Button minimize;


	[Export]
	AnimationPlayer animationPlayer;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready() {
		GetTree().AutoAcceptQuit = false;
		closeButton.Pressed += () => GetTree().Quit();
		minimize.Pressed += () =>  animationPlayer.Play("Minimize");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta) {
		if (Input.IsActionJustPressed("Escape")) animationPlayer.Play("Minimize");
	}

	public static void Minimize() {
		DisplayServer.WindowSetMode(DisplayServer.WindowMode.Minimized);
	}

	public override void _Notification(int what) {
        if (what == MainLoop.NotificationApplicationFocusIn) {
			animationPlayer.Play("Open");
		}
    }
}
