using Godot;
using System;

public partial class TestingScriptDeleteThis : Window
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		if (Input.IsActionPressed("Windows") && Input.IsActionJustPressed("C"))
		{
			GD.Print("PRessed");
		}
	}
}
