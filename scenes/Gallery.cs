using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class Gallery : Control {
	
	[Export]
	TextureRect largeLeft, mediumRight, smallBottom;
	[Export]
	Panel panel;

	TextureRect[] imageRects;

	const int PADDING = 10;

	const string FOLDER_DIRECTORY = "C:/Users/alpit/OneDrive/Documents/Krish/Other Godot/images/Screenshot 2023-11-21 204551.png";

	const string other = "C:\\Users\\alpit\\OneDrive\\Documents\\Krish\\Other Godot\\images";

	// TODO: load all images in "ready" and save it as a list instead of accessing files every other second
	// Add gradient on the bottom of images to add "shadow" kinda thing?
	// have ambient bobbly animations too!?

	public override void _Ready() {
		imageRects = new TextureRect[3] {largeLeft, mediumRight, smallBottom};

    	using var file = FileAccess.Open(FOLDER_DIRECTORY, FileAccess.ModeFlags.Read);
		
		List<string> directories = System.IO.Directory.GetFiles(other).ToList();
        
		directories = directories.Select((path) => path.Replace('\\', '/')).ToList(); // for godot reasons 
		
        for (int i = 0; i < imageRects.Length; i++) {
			GD.Print(directories[i]);
			ImageTexture imageTexture = GetTexture(directories[i]);
			
			imageRects[i].Texture = imageTexture;
			imageRects[i].GetParent<Panel>().Size = GetPanelSize(imageTexture.GetSize(), 350 - i*25);
		}

		CallDeferred("Reposition");
		ResizePanel();
	}

	private void ResizePanel() {
		float x = 10 * 2 + largeLeft.Size.X + PADDING + mediumRight.Size.X;
		float y = 10 * 2 + PADDING + largeLeft.Size.Y + smallBottom.Size.Y;
		panel.Size = new Vector2(x, y);;
	}

	private void Reposition() {
		
		float x_1 = largeLeft.Size.X + PADDING;
        float y_1 = largeLeft.Size.Y - mediumRight.Size.Y;
        mediumRight.GetParent<Panel>().Position = new Vector2(x_1, y_1);


		float x_2 = (mediumRight.GetParent<Panel>().Position.X + mediumRight.Size.X - smallBottom.Size.X) / 2;
		float y_2 = largeLeft.Size.Y + PADDING;
		smallBottom.GetParent<Panel>().Position = new Vector2(x_2, y_2);

	}

	// Takes the size of an image, gets its ratio, and then 
	// returns the size of the Y for the panel for it to fit snuggly

	// just change the panel size; the txtrect will scale accordingly
	private static Vector2 GetPanelSize(Vector2 imageSize, int size) {
		// try with 400 first
		float xToyRatio = imageSize.X / imageSize.Y;
        float s = size / (1 + xToyRatio);

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

	public override void _Process(double delta) {
	}
}
