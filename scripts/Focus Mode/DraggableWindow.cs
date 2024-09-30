using System;
using Godot;

public partial class DraggableWindow : Window {
	
	// Called when the node enters the scene tree for the first time.
	bool mouseWithinArea = false;
	Rect2 screenRect;
	public override void _Ready() {
		screenRect = GetTree().Root.GetVisibleRect();

		if (Main.Instance is null) {
			ProcessMode = ProcessModeEnum.Disabled;
			return;
		}

		MouseEntered += () => mouseWithinArea = true;
		MouseExited += () => mouseWithinArea = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	Vector2 lastMousePosition;
	Vector2 MousePosition => Main.Instance.GetGlobalMousePosition();

	// Required to eliminate issue with moving mouse too fast.
	bool mouseWithinAreaCache = false;
	
	Vector2 BottomRight => Position + Size;
	Vector2 TopLeft => Position;
	double bounceTime = 1;
	Vector2 bounceDir;
	float widthMoved, lengthMoved;

	public override void _Process(double delta)
	{
		if (Name == "Routine Do")
		{
			
		}
	
		if (bounceTime < 1)
		{
			// Position = Vector2.One; // todo
			// Position += (Vector2I) (bounceDir * (float) (delta * 10000 * Math.Pow(bounceTime, 3)));
			bounceTime += delta;
		}
		
		if (Input.IsActionJustReleased("Click")) {
			// if (!screenRect.HasPoint(TopLeft) || !screenRect.HasPoint(BottomRight))
			{
				bounceTime = 0;

				// Rect2 test = new(Vector2, TopLeft);
				Rect2 visibleRect = new(Position, Size);
				Rect2 dist = screenRect.Intersection(visibleRect);

				widthMoved = visibleRect.Size.X - dist.Size.X; 
				lengthMoved = visibleRect.Size.Y - dist.Size.Y; 
				
				// bounceDir = screenRect.GetCenter() - (Position + Size / 2); 
				// bounceDir = bounceDir.Normalized();
			}
		}

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
