namespace Common;

using System;
using Godot;
/// <summary>
/// Watches for debug menu input and triggers debug menu.
/// </summary>
public sealed partial class DebugWatcher : Node
{
    public event Action OnToggleDebug;
    public static DebugWatcher Instance { get; private set; }
    public DebugWatcher()
    {
        if(Instance != null)
            return;
        GD.Print("PauseWatcher: Initialized");
        Instance = this;
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("debug_menu"))
            OnToggleDebug?.Invoke();
    }
}