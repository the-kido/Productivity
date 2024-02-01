using Godot;

public partial class Photo : Control {
	[Export]
	AnimationPlayer wobble, grow;
	
	[Export]
	public Panel panel;
	
	[Export]
	public TextureRect textureRect;

	[Export]
	// either 1, 2, or 3. 3 being the biggest
	public int relativeSize = 1;

	public void SetPhotoSize(Vector2 size) {
		panel.Size = size;
		panel.PivotOffset = new(size.X/2, size.Y/2);
	}

	public override void _Ready() {
		wobble.Play("wiggle");
		wobble.SpeedScale *= relativeSize / 3.0f;
		panel.MouseEntered += MouseChanged;
		panel.MouseExited += MouseChanged;
	}
	
	bool mouseWithin = false;
	private void MouseChanged() {
		mouseWithin = !mouseWithin;
		
		if (mouseWithin) grow.Play("grow");
		else grow.PlayBackwards("grow");

		panel.ZIndex = mouseWithin ? 1 : 0;
	}
}
