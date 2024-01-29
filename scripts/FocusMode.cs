using Godot;

public partial class FocusMode : Window {
	[Export]
	Control gui;

	// Called when the node enters the scene tree for the first time.

	bool mouseWithinArea = false;
	public override void _Ready() {
		gui.MouseEntered += () => mouseWithinArea = true;
		gui.MouseExited += () => mouseWithinArea = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	Vector2 lastMousePosition;
	Vector2 MousePosition => Main.Instance.GetGlobalMousePosition();

	// Required to eliminate issue with moving mouse too fast.
	bool mouseWithinAreaCache = false;
	public override void _Process(double delta){


		GD.Print( Engine.GetFramesPerSecond());

		if (Input.IsActionJustPressed("Click")) {
			lastMousePosition = MousePosition;
			mouseWithinAreaCache = mouseWithinArea;
		}
		if (Input.IsActionJustReleased("Click")) {
			mouseWithinAreaCache = false;
		}

		if (mouseWithinAreaCache && Input.IsMouseButtonPressed(MouseButton.Left)) {
			Vector2 currentMousePosition = MousePosition;

			if (lastMousePosition != currentMousePosition) {
				Vector2 difference = currentMousePosition - lastMousePosition;

				Position = new((int) (Position.X + difference.X), (int) (Position.Y + difference.Y));
			}

			lastMousePosition = currentMousePosition;
		}
	}
}
