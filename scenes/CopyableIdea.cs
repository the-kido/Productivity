using Godot;

public partial class CopyableIdea : Button
{
	public void Init(string text)
	{
		Text = text;
		Visible = true;
	}

	public void UpdatePivot()
	{
		PivotOffset = new(Size.X / 2, PivotOffset.Y);
	}

	public void Open()
	{
		AnimationPlayer animationPlayer = GetChild(0) as AnimationPlayer;
		animationPlayer.Play("instance");
	}
}
