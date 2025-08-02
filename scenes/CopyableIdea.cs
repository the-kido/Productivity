using System;
using Godot;

public partial class CopyableIdea : Button
{
	AnimationPlayer animationPlayer;

	static int numIdeas = 0;
	static int activeIdeas = 0;
	public static readonly Action MenuOpened;
	public static event Action OutOfIdeas;

	static CopyableIdea()
	{
		MenuOpened += () => activeIdeas = numIdeas;
	}
	
	public void Init(string text)
	{
		numIdeas++;

		animationPlayer = GetChild(0) as AnimationPlayer;
		Text = text;
		Visible = true;

		Pressed += () => {
			animationPlayer.Play("delete");
			
			if (--activeIdeas == 0) OutOfIdeas?.Invoke();
		};
	}

	public void UpdatePivot()
	{
		PivotOffset = new(Size.X / 2, PivotOffset.Y);
	}

	public void Open()
	{
		activeIdeas = numIdeas;
		animationPlayer.Play("instance");
	}
}
