namespace Common;

using Godot;
/// <summary>
/// Handles the inital boot up animation.
/// </summary>
public partial class IntroManager : Control
{
	[Export] private PackedScene _main;
	[Export] private Control _emblem;
	[Export] private TextureRect _godot;
	[Export] private Label _loadingText;
	[Export] private ColorRect _fade;
	private Material _emblemMat;
	private string _loadingString;
	private Timer _incremental;
	private bool _switch = false;
	private bool _switch2 = false;
	private bool _switch3 = false;
public override void _Ready()
{
    var fadePlayer = _fade.GetNode<AnimationPlayer>("AnimationPlayer");
    fadePlayer.Stop();
    _fade.Modulate = new Color(1, 1, 1, 0);
    _emblemMat = _emblem.Material;
    _incremental = new()
    {
        Autostart = true,
        OneShot = true,
        WaitTime = 2.0f
    };
    _loadingString = _loadingText.Text;
    AddChild(_incremental);
}
	public override void _Process(double delta)
	{
		if (!_switch)
		{
			double step = 1.0f - (_incremental.TimeLeft / _incremental.WaitTime % 1.0f);
			byte byteStep = (byte)(255 / step);
			_loadingText.Text = _loadingString.Truncate((float)step);
			_emblemMat.Set("shader_parameter/progress", step);
			_emblemMat.Set("shader_parameter/particle_amount", byteStep);
			if (_incremental.IsStopped())
			{
				_switch = true;
				_incremental.Start();
			}
			return;
		}
		if (_incremental.IsStopped() && !_switch2)
		{
			_switch2 = true;
			_emblem.Visible = false;
			_loadingText.Visible = false;
			_godot.Visible = true;
			_incremental.Start();
			return;
		}
		if (_incremental.IsStopped() && !_switch3)
		{
			_switch3 = true;
			var player = _fade.GetNode<AnimationPlayer>("AnimationPlayer");
			player.AnimationFinished += (animationName) => {
				if (animationName == "fadeout") {
					GetTree().ChangeSceneToPacked(_main);
				}
			};
			player.Stop();
			player.Play("fadeout");
		}
	}
}
