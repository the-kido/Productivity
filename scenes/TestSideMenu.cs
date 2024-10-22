using Godot;

public partial class TestSideMenu : Control
{
	[Export]
	AnimationPlayer openAnimation;
	[Export]
	Panel panel;
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

    public override void _Ready()
    {
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

		GetParent<Window>().Visible = false;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
	Vector2 lastHitPosition;
    public override void _Process(double delta)
	{
		if (GetRect().HasPoint(GetGlobalMousePosition()))
		{
			if (!mouseInside)
			{ 
				if (hits == 0) hits++;
				else if (lastHitPosition.DistanceTo(GetGlobalMousePosition()) < 100) hits++;
			}

			if (timer <= 0) {
				timer = COOLDOWN;
				lastHitPosition = GetGlobalMousePosition();
			}
			mouseInside = true;
		}
		else mouseInside = false;

		if (timer >= 0) timer -= delta;
		else hits = 0;
	
		if (hits == 2)
		{
			hits = 0;
			ToggleMenu();
		}
		GD.Print(hits);
	}
	public bool isMenuOpened = false;

	public void ToggleMenu()
	{
		// When opening, update where the menu opens from.
		if (!isMenuOpened)
		{
			float yDiff = GetGlobalMousePosition().Y - panel.Size.Y / 2;
			panel.Position = new(panel.Position.X, yDiff);
		}
		// panel.PivotOffset = new(panel.PivotOffset.X, (panel.Size.Y / 2 + yDiff) * 1.15f);

		openAnimation.Play("open", fromEnd: isMenuOpened, customSpeed: isMenuOpened ? -1 : 1);
		isMenuOpened = !isMenuOpened;
	}
}
