using Godot;
using Microsoft.VisualBasic;
using WindowsInput;

public partial class GrayscaleAtNight : Node
{
	const int HOUR = 12 + 8; // 8:30 PM is when we grayscale.
	const int MIN = 30;

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

	const string GRAYSCALED = "Did grayscale", DIDNT = "Didn't grayscale";
	const string FILE_PATH = "user://grayscaled.txt";
	
	public override void _Ready()
	{
		using var r = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Read);
		var prev = r.GetLine();
		GD.Print(prev);
		r.Close();
		if (prev == GRAYSCALED)
		{
			Toggle();
			using var w = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Write);
			w.StoreLine(DIDNT);
		}
	}

	void Toggle()
	{
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
		ProcessMode = ProcessModeEnum.Disabled;
	}

	public override void _Process(double delta)
	{
		if (DateAndTime.Now.Hour >= HOUR && DateAndTime.Now.Minute >= MIN)
		{
			Toggle();

			using var w = FileAccess.Open(FILE_PATH, FileAccess.ModeFlags.Write);
			w.StoreLine(GRAYSCALED);
			return;
		}
	}
}
