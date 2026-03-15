namespace BlockGame;

using Common;
using Godot;
/// <summary>
/// Represents the paddle in the block_game.
/// Handles movement, resizing, and color changes.
/// </summary>
[GlobalClass]
public sealed partial class PaddleBlock : CharacterBody2D
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
        _collisionShape.Shape = new RectangleShape2D() { Size = new Vector2(Size, 24) };
        _colorRect.Size = new Vector2(Size, 24);
        _colorRect.Position = new Vector2(-Size / 2, -12);
        _initialPosition =  Position;
        _fixedFriction = 1.0f / (Friction / 15.0f);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (Velocity == Vector2.Zero)
            return;
        Velocity *= _fixedFriction;
        MoveAndCollide(Velocity * (float)delta);
    }
    /// <summary>
    /// Adjusts the color of the paddle.
    /// </summary>
    /// <param name="color"></param>
    public void AdjustColor(Color color) => _colorRect.Color = color;
    /// <summary>
    /// Changes the speed of the paddle.
    /// </summary>
    /// <param name="speed"></param>
    public void ChangeSpeed(uint speed)
    {
        if (speed < 100 || speed > 10000)
            return;
        Speed = speed;
    }
    /// <summary>
    /// Moves the paddle in the specified direction.
    /// </summary>
    /// <param name="direction"></param>
    public void Move(Direction direction)
    {
        Velocity = direction == Direction.Left || direction == Direction.Up ? new Vector2(-Speed, 0) : new Vector2(Speed, 0);
    }
    /// <summary>
    /// Resizes the paddle.
    /// </summary>
    /// <param name="size"></param>
    public void Resize(byte size)
    {
        Size = size;
        _collisionShape.Shape = new RectangleShape2D() { Size = new Vector2(size, 24) };
        _colorRect.Size = new Vector2(size, 24);
        _colorRect.Position = new Vector2(-Size / 2, -12);
    }
    /// <summary>
    /// Resets the paddle to its initial position.
    /// </summary>
    public void ResetPosition()
    {
        Velocity = Vector2.Zero;
        Position = _initialPosition;
    }
}