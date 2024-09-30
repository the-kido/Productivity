using Godot;
using System;

public partial class AIWindow : Window
{
	[Export]
	Button openAI;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		openAI.Pressed += () => Visible = true;
		CloseRequested += () => Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
