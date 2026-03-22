namespace FrogGame;

using Common;
using Godot;
public sealed partial class FrogCharacter : CharacterBody2D
{
    [Export] private RayCast2D _rayCast;
    [Export] private AnimatedSprite2D _sprite;
    private byte _gridSize = 16;
    public void Move(Direction direction)
    {
        Vector2 moveDirection = direction switch
        {
            Direction.Up => Vector2.Up,
            Direction.Left => Vector2.Left,
            Direction.Down => Vector2.Down,
            Direction.Right => Vector2.Right,
            Direction.None => Vector2.Zero,
            _ => throw new System.InvalidOperationException(),
        };
        _sprite.FlipH = moveDirection == Vector2.Left || moveDirection == Vector2.Up;
        moveDirection = moveDirection * _gridSize;
        _rayCast.TargetPosition = moveDirection;
        _rayCast.ForceRaycastUpdate();
        if (!_rayCast.IsColliding())
            Position += moveDirection;
        MoveAndSlide();
    }
}