namespace FrogGame;

using Common;
using Godot;
[GlobalClass]
public sealed partial class FrogMovable : AnimatableBody2D
{
    [Export] public Marker2D SpawnPoint { get; private set; }
    [Export] public FrogSpeed Speed { get; private set;} = FrogSpeed.normal;
    [Export] public Direction MovingDirection { get; private set; }
    [Export] private AnimatedSprite2D _sprite;
    public bool InBounds { get; set; } = false;
    public Vector2 MovementVector { get; set; }
    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(MovementVector);
        if (collision?.GetCollider() is FrogCharacter frog)
            frog.Death();
    }
}