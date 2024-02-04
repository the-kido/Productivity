using Godot;

public partial class Parallax : ParallaxBackground {
	
	public override void _Process(double delta) {
		ScrollBaseOffset += new Vector2(100, 0) * (float) delta;
	}
}
