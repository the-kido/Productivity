using Godot;

public partial class AIMessage : PanelContainer
{
	public enum MessageType
	{
		User,
		AI
	}

	[Export]
	public Label text;
	[Export]
	public Button delete;
	TestCMD testCMD;

    public override void _Process(double delta)
    {
		if (GetGlobalRect().HasPoint(GetGlobalMousePosition()) && Input.IsActionJustPressed("Click"))
		{
			DisplayServer.ClipboardSet(text.Text);
		}
    }

    public void Init(string userInputtedText, TestCMD testCMD)
	{
		Init(testCMD);
		Visible = true;
		SizeFlagsHorizontal = SizeFlags.ShrinkEnd;
		text.Text = userInputtedText;
	}
	
	public void Init(TestCMD testCMD)
	{	
		ProcessMode = ProcessModeEnum.Inherit;
		delete.Pressed += () =>{
			testCMD.RemoveMessage(text.Text);
			QueueFree();
		};
	}

	public void Init(AIMessage userMessage, TestCMD testCMD)
	{
		Init(testCMD);
		FocusMode = FocusModeEnum.All;
		SizeFlagsHorizontal = SizeFlags.ShrinkBegin;

		delete.Visible = false; // Do not let the message be deleted until the message is done

		testCMD.SendChatMessage(this, userMessage.text.Text);
	}
}
