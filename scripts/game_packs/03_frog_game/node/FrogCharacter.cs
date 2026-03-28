namespace FrogGame;

using Common;
using Godot;
using System;
public sealed partial class FrogCharacter : CharacterBody2D
{
    public event Action OnDeath;
    public event Action OnWin;
    [Export] private RayCast2D _deathCast;
    [Export] private RayCast2D _floorCast;
    [Export] private RayCast2D _rayCast;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private AudioEvent _sfxMove;
    private Timer _coolDown;
    private bool _coolDownLock = false;
    private Vector2 _spawnLocation;
    private const float _gridSnapEpsilon = 0.001f;
    private const byte _gridSize = 16;
    public override void _Ready()
    {
        _spawnLocation = Position;
        _coolDown = this.AddNode<Timer>();
        _coolDown.Timeout += () => _coolDownLock = !_coolDownLock;
        _coolDown.OneShot = true;
        _coolDown.WaitTime = 0.2f;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (_floorCast.IsColliding() && _floorCast.GetCollider() is FrogMovable moveable)
        {
            MoveAndCollide(moveable.MovementVector);
            return;
        }
        else if (_floorCast.GetCollider() is TileMapLayer)
            OnWin?.Invoke();
        else if (_deathCast.IsColliding() && _deathCast.GetCollider() is TileMapLayer)
            Death();
        Velocity = Vector2.Zero;
        MoveAndSlide();
    }
    public void Death()
    {
        _sprite.Play("death");
        ResetPosition();
        OnDeath?.Invoke();
    }
    public void ResetPosition() => Position = _spawnLocation;
    public void Move(Direction direction)
    {
        _sprite.Play("walk");
        if (_coolDownLock)
            return;
        SnapToGrid();
        Vector2 moveDirection = direction switch
        {
            Direction.Up => Vector2.Up,
            Direction.Left => Vector2.Left,
            Direction.Down => Vector2.Down,
            Direction.Right => Vector2.Right,
            Direction.None => Vector2.Zero,
            _ => throw new InvalidOperationException(),
        };
        _sprite.FlipH = moveDirection == Vector2.Left || moveDirection == Vector2.Up;
        moveDirection *= _gridSize;
        _rayCast.TargetPosition = moveDirection;
        _rayCast.ForceRaycastUpdate();
        if (!_rayCast.IsColliding())
        {
            Position += moveDirection;
            SnapToGrid();
            GameManagers.Instance.Audio.PlayAudioClip(_sfxMove);
            _coolDownLock = true;
            _coolDown.Start();
        }
    }
    private void SnapToGrid()
    {
        float grid = _gridSize;
        Vector2 offset = Position - _spawnLocation;
        Vector2 snappedOffset = new(
            Mathf.Round(offset.X / grid) * grid,
            Mathf.Round(offset.Y / grid) * grid
        );
        Vector2 snappedPosition = _spawnLocation + snappedOffset;
        if (Position.DistanceTo(snappedPosition) > _gridSnapEpsilon)
            Position = snappedPosition;
    }
}