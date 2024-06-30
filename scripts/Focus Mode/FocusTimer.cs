using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class FocusTimer : Control {

	[Export]
	Button pause, settings, close;
	
	[Export]
	Label timer, reasonLabel;

	[Export]
	Control settingsControl;

	[Export]
	AnimationPlayer audioPlayer;
	
	[ExportGroup("Settings requirements")]
	[Export]
	CheckBox toggleOpaqueMode;
	[Export]
	Godot.Collections.Array<Control> opaqueControls;

	[ExportGroup("For focus mode window scene")]
	[Export] Window window;

	[Export]
	AnimationPlayer animationPlayer;

	public bool IsOpened => window.Visible;



	private void Open() {
		ProcessMode = ProcessModeEnum.Always;
        window.Visible = true;
		animationPlayer.Play("open");
		TogglePause(true);
	}

	private void Close() {
		animationPlayer.AnimationFinished += HideOnAnimationFinished;
		animationPlayer.PlayBackwards("open");
		
				
		void HideOnAnimationFinished(StringName _) {
			animationPlayer.AnimationFinished -= HideOnAnimationFinished;
			settingsControl.Visible = false;
			window.Visible = false;
			ProcessMode = ProcessModeEnum.Disabled;
		}
	}
	
	public void StartTimer(int minutes, string reason) {
		Open();
		seconds = minutes * 60;
		reasonLabel.Text = FormatText(reason);
		UpdateTimer();
	}

    private const int LINE_LEAK_SIZE = 24;

    private string FormatText(string @string) {
		List<string> array = @string.Split(" ").ToList();
		int size = 0;

        for (int i = 0; i < array.Count; i++) {
            string item = array[i];

            if (size + item.Length >= LINE_LEAK_SIZE) {
				array.Insert(i, "\n");
				size = -1000;
			}
			size += item.Length + 1; // adding 1 for the space  
		}
		string combined = "";
		foreach (var item in array) combined += item + " ";
		return combined;
	}

	void UpdateTimer() {
		timer.Text = Utils.TimerText(seconds / 3600, seconds/60  % 60, seconds % 60);
	}

	int seconds = 0;

    public override void _Ready() {
		settings.Pressed += OpenSettingsPanel;
		toggleOpaqueMode.Toggled += ToggleOpaqueMode;
		close.Pressed += Close;
		pause.Pressed += () => TogglePause(!timerPaused);
    }

	
	private bool timerPaused = false;
	const string PLAY_TEXT = "▶";
	const string PAUSE_TEXT = "||";
	private void TogglePause(bool paused) {
		timerPaused = paused;
		pause.Text = timerPaused ? PLAY_TEXT : PAUSE_TEXT;
	}

	bool IsSettingsOpen => settingsControl.Visible;
	private void OpenSettingsPanel() {
		
		if (IsSettingsOpen) {
			settingsControl.Visible = false;
		}
		else {
			settingsControl.Visible = true;
		}
	}

	private void CountDown(double delta) {
		if (timerPaused || seconds <= 0) return;
		time += delta;

		if (time >= 0.99999) {
			seconds--;
			PlaySounds(); 
			UpdateTimer();
			time = 0;
		}
	}

	private void PlaySounds() {
		// Every 5 minut
		if (seconds % (5*60) == 0) audioPlayer.Play("reminder");
		if (seconds == 0) audioPlayer.Play("alarm");
	}

	double time = 0;
    public override void _Process(double delta) {
		ControlSize();
		CountDown(delta);
	}

	private void ControlSize() {
		if (Input.IsActionPressed("Right Click") && !animationPlayer.IsPlaying()) {

			if (window.Size.Y != 150) {
				animationPlayer.Play("size_up");
			} else if(window.Size.Y != 110) {
				animationPlayer.Play("size_down");
			}
		}
	}

	readonly Color normal = new(1,1,1,1), opaque = new(1,1,1,0.6f);
	
	private void ToggleOpaqueMode(bool toggle){
		foreach (Control item in opaqueControls) {item.SelfModulate = toggle ? opaque : normal; }
	}

}
