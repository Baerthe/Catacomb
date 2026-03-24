namespace FrogGame;

using Common;
using Godot;
using System;
public sealed partial class FrogCharacter : CharacterBody2D
{
    public event Action OnDeath;
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
        _coolDown = new Timer();
        _coolDown.Timeout += () => _coolDownLock = !_coolDownLock;
        _coolDown.OneShot = true;
        _coolDown.WaitTime = 0.2f;
        AddChild(_coolDown);
    }
    public override void _PhysicsProcess(double delta) => SnapToGrid();
    public void Active() => _sprite.Play("walk");
    public void Death()
    {
        _sprite.Play("death");
        Position = _spawnLocation;
        OnDeath?.Invoke();
    }
    public void Move(Direction direction)
    {
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