namespace FrogGame;

using System;
using Common;
using Godot;
public sealed partial class FrogCharacter : CharacterBody2D
{
    public event Action OnDeath;
    [Export] private RayCast2D _rayCast;
    [Export] private AnimatedSprite2D _sprite;
    [Export] private AudioEvent _sfxMove;
    public byte GridSize { get; set; }
    private Timer _coolDown;
    private bool _coolDownLock = false;
    private Vector2 _spawnLocation;
    public override void _Ready()
    {
        _spawnLocation = Position;
        _coolDown = new Timer();
        _coolDown.Timeout += () => _coolDownLock = !_coolDownLock;
        _coolDown.OneShot = true;
        _coolDown.WaitTime = 0.2f;
        AddChild(_coolDown);
    }
    public void Move(Direction direction)
    {
        if (_coolDownLock)
            return;
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
        moveDirection *= GridSize;
        _rayCast.TargetPosition = moveDirection;
        _rayCast.ForceRaycastUpdate();
        if (!_rayCast.IsColliding())
        {
            Position += moveDirection;
            GameManagers.Instance.Audio.PlayAudioClip(_sfxMove);
            _coolDownLock = true;
            _coolDown.Start();
        }
        MoveAndSlide();
    }
    private void Death()
    {
        
        OnDeath?.Invoke();
    }
}