using Godot;

public partial class Temp : Control {
	[Export]
	TextureRect _1, _2;

	int timeForLoop = 5;
	double time = 0;
	const int X_LENGTH = 1920;

	public override void _Process(double delta) {
		time += delta;

		Vector2 shift = Vector2.Right * (X_LENGTH / timeForLoop) * (float) time;
		_1.Position = shift; 
		_2.Position = shift - Vector2.Right * X_LENGTH;

		if (time >= timeForLoop) time = 0;
	}
}
