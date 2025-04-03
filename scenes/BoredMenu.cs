using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class BoredMenu : Node
{
	[Export] Godot.Collections.Array<string> ideas;

	PackedScene copyableIdea = ResourceLoader.Load<PackedScene>("res://scenes/bored menu/copyable_idea.tscn");
	[Export] Panel boredMenu;
	[Export] AnimationPlayer menuAnimations;
	[Export] Control menuHitbox;
	[Export] Container container;
	[Export] Window menuWindow;

	static readonly List<Control> copiedItems = new();

	double enoughDistanceCoefficient;

	Vector2 defaultSize;

    public override void _Ready()
    {
		enoughDistanceCoefficient = 6.0 * menuHitbox.Size.Y / DisplayServer.ScreenGetRefreshRate();

        for (int i = 0; i < ideas.Count; i++)
		{
            string copypaste = ideas[i];
            if (copypaste[0] == '*')
			{
				continue;
			}

			CopyableIdea dupeCopyable = copyableIdea.Instantiate<CopyableIdea>();
			dupeCopyable.Init(copypaste);
			copiedItems.Add(dupeCopyable);

			container.AddChild(dupeCopyable);
		}

		// Vector2 newMenuSize = new(boredMenu.Size.X, copiedItems.Count * 70 + 10);
		CallDeferred("UpdateSizes");
	}

	private void UpdateSizes()
	{
		Vector2 newMenuSize = new(boredMenu.Size.X, container.Size.Y + 20);
		menuAnimations.GetAnimation("Open").TrackSetKeyValue(2, 1, newMenuSize);

        for (int i = 0; i < ideas.Count; i++) if (copiedItems[i] is CopyableIdea copyableIdea) copyableIdea.UpdatePivot();


		menuWindow.Visible = false;
	}

	double time = 2.5;
	bool playingAnimation = false;

	private void Open()
	{
		playingAnimation = true;
		time = 2.5;
		shown = 0;

		menuAnimations.Play("Open");

		foreach (var copy in copiedItems)
		{
			copy.Visible = false;
		}
	}

	int shown = 0;

	private int GetCurrentContainerSize()
	{
		return shown * 70 + 20;
	}

	private void PlayOpeningAnimation(double delta)
	{
		if (!playingAnimation) return;
		time -= delta;
		if (time < 0) playingAnimation = false;

		float sizeDiff = boredMenu.Size.Y - GetCurrentContainerSize();
		if (sizeDiff >= 500) return; // ignore the first frame where the window hasn't updated in size

		if (sizeDiff >= 60)
		{
			var toShow = copiedItems[shown++];
			if (toShow is CopyableIdea copyableIdea && copyableIdea.Visible == false) copyableIdea.Open();
		}
	}

	private /*async*/ void Close()
	{
		menuAnimations.Play("Close");
		// await Task.Delay(1000);

		// foreach(var copyedItem in copiedItems)
		// {
		// 	copyedItem.Visible = false;
		// }
	}

	double timer = 0;
	int clicks = 0;

	Vector2 mousePos, previousPos;


    public override void _Process(double delta)
    {
		// for the animation stuff
		PlayOpeningAnimation(delta);

		mousePos = menuHitbox.GetGlobalMousePosition();
		
		if (timer <= 0 && menuHitbox.GetRect().HasPoint(mousePos))
		{
			if (-enoughDistanceCoefficient >= previousPos.Y - mousePos.Y || previousPos.Y - mousePos.Y >= enoughDistanceCoefficient)
			{
				if (!menuWindow.Visible) Open(); else Close();
				// start buffer timer
				timer = 2;
			}
		}
		timer -= delta;

		previousPos = mousePos;
    }
}
