using System;
using System.Text.RegularExpressions;
using Godot;
using Microsoft.VisualBasic;
using WindowsInput;

public partial class GrayscaleAtNight : Node
{
	const int HOUR = 12 + 8; // 8:30 PM is when we grayscale.
	const int MIN = 30;

	static bool IsLate() => DateAndTime.Now.Hour >= HOUR && DateAndTime.Now.Minute >= MIN;

	static System.Diagnostics.Process StartProcess(int val)
	{
		string strCmdText = $"powershell -Command \"Set-ItemProperty -Path 'HKCU:\\Software\\Microsoft\\ColorFiltering' -Name 'HotkeyEnabled' -Value {val}\"";

		var psi = new System.Diagnostics.ProcessStartInfo("cmd.exe", "/C " + strCmdText)
		{
			UseShellExecute = false,
			// CreateNoWindow = true
		};
		return System.Diagnostics.Process.Start(psi);
	}

	const string GRAYSCALED = "Grayscaled", NOT_GRAYSCALED = "Not Grayscaled";
	const string FILE_PATH = "user://grayscaled.txt";

	bool isGrayscaled = false;
	public override void _Ready()
	{
		using var r = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Read);
		if (!FileAccess.FileExists(FILE_PATH))
		{
			using var w = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Write);
			w.StoreLine(NOT_GRAYSCALED);
		}

		var prev = r.GetLine();
		if (string.IsNullOrEmpty(prev)) prev = "Not Grayscaled";
		r.Close();

		GD.Print(prev);
		isGrayscaled = prev == GRAYSCALED;

		// Aka if it's the morning and we needa toggle it, toggle it!
		if (isGrayscaled && !IsLate()) Toggle(false);
	}

	void Toggle(bool on)
	{
		// Nothing to toggle if the state is already what we wanted!
		if (isGrayscaled == on) return;

		var a = StartProcess(1);
		a.EnableRaisingEvents = true;
		a.Exited += (_, _) =>
		{
			InputSimulator inputSimulator = new();
			inputSimulator.Keyboard.ModifiedKeyStroke(
				new[] { WindowsInput.Native.VirtualKeyCode.CONTROL, WindowsInput.Native.VirtualKeyCode.LWIN },
				WindowsInput.Native.VirtualKeyCode.VK_C
			);

			StartProcess(0);
		};

		using var w = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Write);
		w.StoreLine(on ? GRAYSCALED : NOT_GRAYSCALED);
		isGrayscaled = !isGrayscaled;
	}

	public override void _Process(double delta)
	{
		if (IsLate())
		{
			Toggle(true);
			ProcessMode = ProcessModeEnum.Disabled;
		}
	}
}
