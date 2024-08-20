using System.Collections.Generic;
using Godot;

public partial class DoRoutine : Control
{
	[Export]
	HSlider progress;
	[Export]
	BoxContainer checkboxParent;

	readonly List<CheckBox> checkBoxes = new();

	public override void _Ready()
	{
		foreach (var child in checkboxParent.GetChildren())
		{
			if (child is CheckBox checkBox) 
			{
				checkBox.Toggled += Toggled; 
				checkBoxes.Add(checkBox);
			}
		}

		progress.MaxValue = checkBoxes.Count;
	}
	
	int totalToggled = 0;

	private void Toggled(bool toggle)
	{
		if (toggle) progress.Value++;
		else progress.Value--;
 
		if (progress.Ratio >= 1)
		{
			GetParent<Window>().Visible = false;
		}
	}
}
