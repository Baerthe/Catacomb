namespace FrogGame;

using Common;
using Godot;
[GlobalClass]
public sealed partial class FrogMovable : CharacterBody2D
{
    [Export] public FrogSpeed Speed { get; private set;} = FrogSpeed.normal;
    [Export] public Direction MovingDirection { get; private set; }
    [Export] private AnimatedSprite2D _sprite;
    public bool InBounds { get; set; }
    public Vector2 InitPosition { get; private set; }
    public override void _PhysicsProcess(double delta) => MoveAndSlide();
    public void Init() => InitPosition = Position;
}