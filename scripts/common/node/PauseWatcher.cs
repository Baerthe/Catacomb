namespace Common;

using System;
using Godot;
/// <summary>
/// Watches for pause input and triggers pause events.
/// </summary>
public sealed partial class PauseWatcher : Node
{
    public event Action OnTogglePause;
    public static PauseWatcher Instance { get; private set; }
    public PauseWatcher()
    {
        GD.Print("PauseWatcher: Initialized");
        Instance = this;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("pause_game"))
            OnTogglePause?.Invoke();
    }
}