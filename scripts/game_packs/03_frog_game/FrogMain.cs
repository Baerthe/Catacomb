namespace FrogGame;

using Godot;
using Common;
using System.Linq;
public sealed partial class FrogMain : PackBase
{
    [ExportGroup("References")]
    [Export] private FrogCharacter _character;
    [Export] private FrogMenu _menu;
    [Export] private Node2D _map;
    [Export] private Area2D _mapBounds;
    [Export] private Label _scoreLabel;
    private FrogPlayer _controller;
    private byte _playerLives = 3;
    private byte _startLives = 3;
    private FrogAI _frogAI;
    private Timer _rewardTimer;
    private FrogMovable[] _movables;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _menu.OnGameStart += (lives, color) =>
        {
            _startLives = lives;
            _playerLives = lives;
            _character.Modulate = color;
            RequestGameState(GameState.Playing);
        };
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
        _map.Modulate = new Color(0.1f, 0.1f, 0.1f, 1f);
        _scoreLabel.Visible = false;
        _controller = new FrogPlayer(_character);
        _character.OnDeath += () => _playerLives -= 1;
        _character.OnWin += () =>
        {
            GameOverReason = GameOverReason.Player1Won;
            RequestGameState(GameState.GameOver);
        };
        _movables = GetTree().GetNodesInGroup("moveable").OfType<FrogMovable>().ToArray();
        _frogAI = new FrogAI(_movables, _mapBounds);
        _rewardTimer = this.AddNode<Timer>();
        Score1 = new Score(_scoreLabel);
        Score1.AddPoints(10000);
    }
    public override void Tick()
    {
        if (_playerLives <= 0)
        {
            GameOverReason = GameOverReason.Player1Lost;
            RequestGameState(GameState.GameOver);
        }
        _controller.Update();
    }
    public override void InfrequentTick()
    {
        if (_playerLives <= 0)
            return;
        Score1.RemovePoints(1);
        _frogAI.Update();
    }
    // *-> Game Methods
    protected override void GameReset()
    {
        Score1.Reset();
        Score1.AddPoints(10000);
        _playerLives = _startLives;
        _character.ResetPosition();
        _rewardTimer.Stop();
        _rewardTimer.Start(999999f);
    }
    // *-> Game State Functions
    protected override void StateGameOver(GameOverReason reason)
    {
        if (reason == GameOverReason.Player1Won)
            SubmitScore(1, Score1);
        GameReset();
        RequestGameState(GameState.Paused);
    }
    protected override void StatePaused()
    {
        _scoreLabel.Visible = false;
        _map.Modulate = new Color(0.1f, 0.1f, 0.1f, 1f);
        _menu.Visible = true;
        _character.SetPhysicsProcess(false);
        _rewardTimer.Paused = true;
        _frogAI.Freeze();
    }
    protected override void StatePlaying()
    {
        _scoreLabel.Visible = true;
        _map.Modulate = new Color(1f, 1f, 1f, 1f);
        _menu.Visible = false;
        _character.SetPhysicsProcess(true);
        _rewardTimer.Paused = false;
        _frogAI.Unfreeze();
    }
}