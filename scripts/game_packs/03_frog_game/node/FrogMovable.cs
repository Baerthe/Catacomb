namespace FrogGame;

using Common;
using Godot;
[GlobalClass]
public sealed partial class FrogMovable : CharacterBody2D
{
    [Export] public FrogSpeed Speed { get; private set;} = FrogSpeed.normal;
    [Export] public Direction MovingDirection { get; private set; }
    [Export] private AnimatedSprite2D _sprite;
    public bool OnScreen { get; private set; }
    public Vector2 InitPosition { get; private set; }
    private VisibleOnScreenNotifier2D _onScreenNotification;
    public override void _Ready()
    {
        _onScreenNotification = this.AddNode<VisibleOnScreenNotifier2D>();
        _onScreenNotification.ScreenExited += () => OnScreen = false;
        _onScreenNotification.ScreenEntered += () => OnScreen = true;
    }
}