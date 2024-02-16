using System.Collections.Generic;
using Godot;

public partial class Photo : Control {

	[Export]
	AnimationPlayer wobble, grow;
	
	[Export]
	public Panel panel;
	
	[Export]
	public TextureRect textureRect;

	[Export]
	public int relativeSize = 1; // either 1, 2, or 3. 3 being the biggest

	public void SetPhotoSize(Vector2 size) {
		panel.Size = size;
		panel.PivotOffset = new(size.X / 2, size.Y / 2);
	}

	public override void _Ready() {
		wobble.Play("wiggle");
		wobble.SpeedScale *= relativeSize / 3.0f;
		panel.MouseEntered += () => MouseChanged(true);
		panel.MouseExited += () => MouseChanged(false);

		allPhotos.Add(this);
	}
	
	private void MouseChanged(bool mouseWithin) {
		if (mouseWithin) {
            grow.Play("grow");
			SetPriority(this);
        } else {
            grow.PlayBackwards("grow");
        }
	}

	static readonly List<Photo> allPhotos = new();
	static void SetPriority(Photo photo) {
		allPhotos.Remove(photo);
		allPhotos.Add(photo);

		for (int i = 0; i < 3; i++) allPhotos[i].ZIndex = i; 
	}
}
