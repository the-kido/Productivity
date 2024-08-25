using Godot;

public partial class Notification : Control {
	[Export]
	AnimationPlayer animationPlayer;
    public override void _Ready() {
        animationPlayer.AnimationFinished += (a) => GetParent<Window>().Visible = false;
    }

    public void Play() {
		GetParent<Window>().Visible = true;
		animationPlayer.Play("Show");
	}
}
