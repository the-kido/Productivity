using Godot;
using System.Threading.Tasks;

public partial class TakeABreak : Window
{
	[Export]
	AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
		Remind();
	}

	async void Remind()
	{
		await Task.Delay(1000 * 60 * 60); // each hour
		animationPlayer.Play("fly");
		Visible = true;	
		Remind();
	}
    public override void _Process(double delta)
    {
		if (animationPlayer.IsPlaying() && Input.IsActionJustPressed("Escape")) animationPlayer.Stop();
    }
}
