using System;
using System.Threading.Tasks;
using Godot;
using WindowsInput;

public partial class TestCopyable : MarginContainer
{
	[Export]
	Panel bar;
	[Export]
	Label label;
	TestSideMenu testSideMenu;
	static InputSimulator inputSimulator = new();

	public void Init(TestSideMenu testSideMenu, string text)
	{
		this.testSideMenu = testSideMenu;
		label.Text = text;

		CallDeferred("SetPivot");
		ProcessMode = ProcessModeEnum.Inherit;
	}

	public void SetPivot()
	{
		label.PivotOffset = new(label.Size.X / 2, label.Size.Y / 2);
	}

	const float SELECTION_TIME = 0.5f;

	double timer = 0;
	public override void _Process(double delta)
	{
		Vector2 mousePosition = GetGlobalMousePosition();
		if (testSideMenu.isMenuOpened && GetGlobalRect().HasPoint(mousePosition))
		{
			timer += delta / SELECTION_TIME;
		}
		else 
		{
			timer = Math.Max(timer - delta / SELECTION_TIME * 2, 0);
		}

		if (timer >= 1) 
		{
			DisplayServer.ClipboardSet(label.Text); 
			testSideMenu.ToggleMenu(mousePosition);

			inputSimulator.Keyboard.ModifiedKeyStroke(WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.VK_V);
		}
		else {
			bar.Scale = new((float) (timer*timer*timer*timer), 1);
			getRotation();
		}
	}

	private void getRotation()
	{
        double amp;
        if (timer < 0.8) amp = timer;
		else amp = 0.8 - 20 * Math.Pow(timer - 0.8, 2);
		amp *= 20;

		double rotation = Math.Sin(timer * Math.PI * 5) * amp;
		label.RotationDegrees = (float) rotation;
	}
}






