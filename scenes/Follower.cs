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
		followingWindow.VisibilityChanged += () => 
		{
			Visible = followingWindow.Visible;
		};
	}

	[Export]
	int followDistance = 500, baseSpeed = 20;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector2 currentPosition = Position + Size / 2;
		Vector2 otherPosition = followingWindow.Position + followingWindow.Size / 2; 
		float distance = currentPosition.DistanceTo(otherPosition);

		int sign = distance > followDistance ? 1 : -1;

		double speedMultiplier = baseSpeed * Math.Pow(sign * (distance - followDistance), 2.0 / 3.0);

		if (double.IsNaN(speedMultiplier)) speedMultiplier = 0;

		Vector2 direction = (otherPosition - currentPosition).Normalized();

		Position += (Vector2I) (sign * direction * (float) (delta * speedMultiplier));




		arrow.LookAt(otherPosition - currentPosition);
		arrow.Rotate((float) Math.PI * 3.0f / 2.0f);
		// GD.Print(arrow.Rotation);
	}
}
