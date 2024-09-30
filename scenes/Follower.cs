using System;
using Godot;

public partial class Follower : Window
{
	[Export]
	Window followingWindow;
	[Export]
	AnimatedSprite2D arrow;

	public override void _Ready()
	{
		followingWindow.VisibilityChanged += () => Visible = followingWindow.Visible;
	}

	[Export]
	int followDistance = 500, baseSpeed = 20;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 currentPosition = Position + Size / 2;
		Vector2 otherPosition = followingWindow.Position + followingWindow.Size / 2; 
		Vector2 direction = (otherPosition - currentPosition).Normalized();
		
		Move(currentPosition, otherPosition, direction, delta);
		
		Rotate(direction);
	}

	private void Move(Vector2 currentPosition, Vector2 otherPosition, Vector2 direction, double delta)
	{
		float distance = currentPosition.DistanceTo(otherPosition);

		int sign = distance > followDistance ? 1 : -1;

		double speedMultiplier = baseSpeed * Math.Pow(sign * (distance - followDistance), 2.0 / 3.0);

		if (double.IsNaN(speedMultiplier)) speedMultiplier = 0;

		Position += (Vector2I) (sign * direction * (float) (delta * speedMultiplier));
	}

	private void Rotate(Vector2 direction)
	{
		// Tween.InterpolateValue()
		arrow.Rotation = direction.Angle() - Mathf.Pi / 2;
	}
}
