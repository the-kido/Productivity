using Godot;

public partial class Routine : Window {
	[Export]
	Button open, close;
	[Export]
	AnimationPlayer animationPlayer;

	public override void _Ready() {
		open.Pressed += () => {
			Visible = true;
			animationPlayer.Play("Open");
		};
		close.Pressed += () => animationPlayer.Play("Minimize");
	}

	public void Minimize() {
		Mode = ModeEnum.Minimized;
		Visible = false;
	}

	public override void _Process(double delta) {
		PressToPan();
	}

	Vector2 lastMousePosition;
	Vector2 MousePosition => Main.Instance.GetGlobalMousePosition();
	private void PressToPan() {
		if (Input.IsActionJustPressed("Click")) lastMousePosition = MousePosition;

		if (Input.IsMouseButtonPressed(MouseButton.Left)) {
			Vector2 currentMousePosition = MousePosition;

			if (lastMousePosition != currentMousePosition) {
				Vector2 difference = currentMousePosition - lastMousePosition;

				Position = new((int) (Position.X + difference.X), (int) (Position.Y + difference.Y));
			}

			lastMousePosition = currentMousePosition;
		}
	}
}
