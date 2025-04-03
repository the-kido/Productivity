using System;
using Godot;

public partial class TestSideMenu : Node
{
	[Export]
	AnimationPlayer openAnimation;
	[Export]
	Panel panel;
	[Export]
	Window window;
	[Export]
	Control menuHitbox;
	[Export]
	TestCopyable template;
	[Export]
	HFlowContainer container;
	[Export]
	HSeparator hSeparatorTemplate;

	[Export]
	Godot.Collections.Array<string> copypastes;

	double timer;
	bool mouseInside = false;
	int hits = 0;
	const float COOLDOWN = 0.3f;

	Vector2I windowSize;

    public override void _Ready()
    {
		windowSize = window.Size;
		foreach (string copypaste in copypastes)
		{
			if (copypaste == "sep")
			{
				HSeparator dupSepeator = hSeparatorTemplate.Duplicate() as HSeparator;
				container.AddChild(dupSepeator);
				dupSepeator.Visible = true;
				continue;
			}
			TestCopyable dupCopyable = template.Duplicate() as TestCopyable;
			container.AddChild(dupCopyable);
			dupCopyable.Init(this, copypaste);
			dupCopyable.Visible = true;
		}
		openAnimation.Play("reset_manually");

		CallDeferred("test");
    }
	private void test()
	{
		window.Visible = false;

	}

    // Called every frame. 'delta' is the elapsed time since the previous frame.
	Vector2 lastHitPosition;
    public override void _Process(double delta)
	{
		Vector2 mousePosition = menuHitbox.GetGlobalMousePosition();
		
		if (Input.IsActionPressed("Windows") && Input.IsActionJustPressed("C"))
		{
			ToggleMenu(mousePosition);
		}

		if (menuHitbox.GetRect().HasPoint(mousePosition))
		{
			if (!mouseInside)
			{ 
				if (hits == 0) hits++;
				else if (lastHitPosition.DistanceTo(mousePosition) < 100) hits++;
			}

			if (timer <= 0) {
				timer = COOLDOWN;
				lastHitPosition = mousePosition;
			}
			mouseInside = true;
		}
		else mouseInside = false;

		if (timer >= 0) timer -= delta;
		else hits = 0;
	
		if (hits == 2)
		{
			hits = 0;
			ToggleMenu(mousePosition);
		}
	}
	public bool isMenuOpened = false;

	public void ToggleMenu(Vector2 mousePosition)
	{
		// When opening, update where the menu opens from.
		if (!isMenuOpened)
		{
			int yDiff = (int)(mousePosition.Y - windowSize.Y / 2.0f);
			yDiff = Mathf.Clamp(yDiff, 0, GetTree().Root.Size.Y - windowSize.Y);
			window.Position = new(window.Position.X, yDiff);
		}
		// panel.PivotOffset = new(panel.PivotOffset.X, (panel.Size.Y / 2 + yDiff) * 1.15f);

		openAnimation.Play("open", fromEnd: isMenuOpened, customSpeed: isMenuOpened ? -1 : 1);
		isMenuOpened = !isMenuOpened;
	}
}
