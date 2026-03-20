namespace Common;

using Godot;
using System;
public sealed partial class DebugMenu : Control
{
    public event Action OnDebugPoints;
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonPoints;
    public override void _Ready()
    {
        Visible = false;
        _buttonPoints.Pressed += () => OnDebugPoints?.Invoke();
        _buttonCancel.Pressed += () => Visible = false;
    }
}