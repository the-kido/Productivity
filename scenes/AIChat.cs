using Godot;

public partial class AIChat : Control
{

	[Export]
	TextEdit userInput;
	[Export]
	Button sendMessage;

	[Export]
	PanelContainer panelTemplate;
	[Export]
	ScrollContainer scrollContainer;

	[Export]
	TestCMD testCMD;
	[Export] 
	BoxContainer container;

	[Export]
	Control buffer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		scrollContainer.GetVScrollBar().Changed += () => {
			scrollContainer.ScrollVertical = (int) scrollContainer.GetVScrollBar().MaxValue;
		};

		sendMessage.Pressed += CreateNewMessage;
	}

	private void CreateNewMessage()
	{
		var userDup = panelTemplate.Duplicate() as AIMessage;
		container.AddChild(userDup);
		userDup.Init(userInput.Text, testCMD);

		var msgDup = panelTemplate.Duplicate() as AIMessage;
		container.AddChild(msgDup);
		msgDup.Init(userDup, testCMD);

		// Move the buffer to the end again
		container.MoveChild(buffer, container.GetChildCount() - 1);

		userInput.Text = ""; // Clear it after sending
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Message()
	{
		panelTemplate.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
	}
}
