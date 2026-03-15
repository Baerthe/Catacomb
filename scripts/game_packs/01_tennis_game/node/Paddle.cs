namespace TennisGame;

using Common;
using Godot;
/// <summary>
/// Represents the paddle in the tennis_game.
/// Handles movement, resizing, and color changes.
/// </summary>
[GlobalClass]
public sealed partial class Paddle : CharacterBody2D
{
    [ExportGroup("Properties")]
    [Export(PropertyHint.Range, "1,100")] public byte Friction { get; private set; } = 25;
    [Export(PropertyHint.Range, "100,10000")] public uint Speed { get; private set; } = 2000;
    [Export(PropertyHint.Range, "1,255")] public byte Size { get; private set; } = 64;
    private CollisionShape2D _collisionShape;
    private ColorRect _colorRect;
    private float _fixedFriction;
    private Vector2 _initialPosition;
    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        _colorRect = GetNode<ColorRect>("ColorRect");
        _collisionShape.Shape = new RectangleShape2D() { Size = new Vector2(24, Size) };
        _colorRect.Size = new Vector2(24, Size);
        _colorRect.Position = new Vector2(-12, -Size / 2);
        _initialPosition = Position;
        _fixedFriction = 1.0f / (Friction / 15.0f);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Velocity == Vector2.Zero)
            return;
        Velocity *= _fixedFriction;
        MoveAndCollide(Velocity * (float)delta);
    }
    public void AdjustColor(Color color)
    {
        _colorRect.Color = color;
    }
    public void ChangeSpeed(uint speed)
    {
        if (speed < 100 || speed > 10000)
            return;
        Speed = speed;
    }
    public void Move(Direction direction)
    {
        Velocity = direction == Direction.Up || direction == Direction.Left ? new Vector2(0, -Speed) : new Vector2(0, Speed);
    }
    public void Resize(byte size)
    {
        Size = size;
        _collisionShape.Shape = new RectangleShape2D() { Size = new Vector2(24, Size) };
        _colorRect.Size = new Vector2(24, Size);
        _colorRect.Position = new Vector2(-12, -Size / 2);
    }
    public void ResetPosition()
    {
        Velocity = Vector2.Zero;
        Position = _initialPosition;
    }
}