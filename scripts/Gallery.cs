using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Gallery : Control {
	
	[Export]
	Photo largeLeft, mediumRight, smallBottom;
	[Export]
	Panel panel;

	Photo[] photos;

	const int PADDING = 10;
	
	const int LOOP_TIME = 10; // In minutes
	const string FOLDER_DIRECTORY = "C:\\Users\\alpit\\OneDrive\\Documents\\Krish\\Other Godot\\images";

    readonly string[] directories;
	readonly ImageTexture[] images;

	Gallery() {
		return;
		directories = System.IO.Directory
			.GetFiles(FOLDER_DIRECTORY)
			.Select((path) => path.Replace('\\', '/'))
			.ToArray();
			
		images = directories.Select((directory) => GetTexture(directory)).ToArray();
	}

	public override void _Ready() {
		photos = new Photo[3] {largeLeft, mediumRight, smallBottom};
		
		UpdateImages(0);
	}

	private async void UpdateImages(int location) {
		ResizePhotos(ref location);
		CallDeferred("Reposition");
		CallDeferred("ResizePanel");
		
		await Task.Delay(1000 * LOOP_TIME);
		UpdateImages(location + 3);
	}
	
	private void ResizePhotos(ref int location) {
		location %= images.Length;

		for (int i = 0; i < photos.Length; i++) {
			// The index is shifted so that every image is looped over
			int index = (i + location) % images.Length;
			ImageTexture imageTexture = images[index];

			photos[i].textureRect.Texture = imageTexture;
			photos[i].SetPhotoSize(GetPanelSize(imageTexture.GetSize(), 275 + photos[i].relativeSize * 25)); 
		}
	}

	private void ResizePanel() {
		float x = -10 * 2 + largeLeft.panel.Size.X + PADDING + mediumRight.panel.Size.X;
		float y = -10 * 2 + PADDING + largeLeft.panel.Size.Y + smallBottom.panel.Size.Y;
		panel.Size = new Vector2(x, y);;
	}

	private void Reposition() {
		
		float x_1 = largeLeft.panel.Size.X + PADDING;
        float y_1 = largeLeft.panel.Size.Y - mediumRight.panel.Size.Y;
        mediumRight.Position = new Vector2(x_1, y_1);


		float x_2 = (mediumRight.Position.X + mediumRight.panel.Size.X - smallBottom.panel.Size.X) / 2;
		float y_2 = largeLeft.panel.Size.Y + PADDING;
		smallBottom.Position = new Vector2(x_2, y_2);

	}

	private static Vector2 GetPanelSize(Vector2 imageDimensions, int scale) {
		// try with 400 first
		float xToyRatio = imageDimensions.X / imageDimensions.Y;
        float s = scale / (1 + xToyRatio);

		return new (xToyRatio * s, s);

	}

	private static ImageTexture GetTexture(string path) {
    	using var textureFile = FileAccess.Open(path, FileAccess.ModeFlags.Read);
		
		var bytes = textureFile.GetBuffer((long) textureFile.GetLength());
		
		var image = new Image();
		image.LoadPngFromBuffer(bytes);
		
		var imageTexture = new ImageTexture();
		imageTexture.SetImage(image);
		return imageTexture;
	}
}
