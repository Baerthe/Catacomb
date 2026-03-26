namespace FrogGame;

using Common;
using Godot;
/// <summary>
/// The base class for nodes in the Frog Game controlled by AI. Setting "Ridable" to false (by default) sets collision_layer to 4 (Objects) otherwise to 8 (Platforms)
/// </summary>
[GlobalClass]
public sealed partial class FrogMovable : AnimatableBody2D
{
    [Export] public bool Ridable { get; private set;} = false;
    [Export] public Marker2D SpawnPoint { get; private set; }
    [Export] public FrogSpeed Speed { get; private set;} = FrogSpeed.normal;
    [Export] private AnimatedSprite2D _sprite;
    [Export] public Direction MovingDirection { get; private set; }
    public bool InBounds { get; set; } = false;
    public Vector2 MovementVector { get; set; }
    public override void _Ready()
    {
        if (Ridable)
            CollisionLayer = 4;
        else
            CollisionLayer = 8;
    }
    public override void _PhysicsProcess(double delta)
    {
        var collision = MoveAndCollide(MovementVector);
        if (collision?.GetCollider() is FrogCharacter frog)
            if (!Ridable)
                frog.Death();
            else
                return;
    }
}