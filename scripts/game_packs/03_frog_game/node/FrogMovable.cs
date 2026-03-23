namespace FrogGame;

using Common;
using Godot;
[GlobalClass]
public sealed partial class FrogMovable : CharacterBody2D
{
    public byte GridSize { get; set; }
    public Direction MovingDirection { get; private set; }
    public bool OnScreen { get; private set; }
    private VisibleOnScreenNotifier2D _onScreenNotification;

    public override void _Ready()
    {
        _onScreenNotification = this.AddNode<VisibleOnScreenNotifier2D>();
        _onScreenNotification.ScreenExited += () => OnScreen = false;
        _onScreenNotification.ScreenEntered += () => OnScreen = true;
    }
}