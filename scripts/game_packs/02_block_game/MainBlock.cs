namespace BlockGame;

using Common;
using Godot;
using System;
/// <summary>
/// Main game controller for BlockGame. A breakout-style game where the player controls
/// a paddle to bounce a ball and destroy blocks. Manages game flow, scoring, lives, and level progression.
/// </summary>
public sealed partial class MainBlock : PackBase
{
    [ExportGroup("References")]
    [Export] private MenuBlock _menu;
    [Export] private BallBlock _ball;
    [Export] private BlockCollection _blockCollection;
    [Export] private Timer _gameTimer;
    [Export] private LevelData _testLevel;
    [Export] public PaddleBlock _paddle;
    [ExportGroup("Sounds")]
    [Export] private AudioEvent _sfxOutOfBounds;
    [ExportGroup("Rects")]
    [Export] private ColorRect _crossRect;
    [Export] private ColorRect _leftWallRect;
    [Export] private ColorRect _rightWallRect;
    [ExportGroup("HUD Properties")]
    [Export] private Label _scoreLabel;
    [Export] private Label _timerLabel;
    [Export] private Label _middleScreenLabel;
    [Export] private Label _livesLabel;
    // *-> Switches
    private bool _isRandomLevel = true;
    // *-> Fields
    private int _timeInSeconds = 0;
    private int _maxTimeInSeconds = 9999;
    private byte _maxScore = 255;
    private byte _lives = 3;
    private const byte DefaultLives = 3;
    // *-> Godot Overrides
    public override void _Ready()
    {
        Score1 = new Score(_scoreLabel);
        // Connect Events
        _ball.OnBlockHit += HandleBlockHit;
        _ball.OnOutOfBounds += HandleBallOutOfBounds;
        _blockCollection.OnAllBlocksDestroyed += HandleLevelCleared;
        _gameTimer.Timeout += HandleTimerUpdate;
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameStart += GameStart;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
        UpdateLivesLabel();
        if (_ball.IsEnabled)
            _ball.ToggleEnable();
    }
    // *-> Game Methods
    protected override void Tick()
    {
        // block is pretty simple and does not use per-frame logic.
    }
    protected override void GameReset()
    {
        _gameTimer.Stop();
        _paddle.ResetPosition();
        _ball.Position = _ball.InitialPosition;
        _ball.Velocity = Vector2.Zero;
        if (_ball.IsEnabled)
            _ball.ToggleEnable();
        Score1.Reset();
        _lives = DefaultLives;
        _timeInSeconds = 0;
        _blockCollection.ClearLevel();
        UpdateLivesLabel();
        RequestGameState(GameState.Paused);
    }
    /// <summary>
    /// Starts a new game with the given settings from the menu.
    /// </summary>
    private void GameStart(PlayerType paddleType, int ballSize, int paddleSize, int paddleSpeed, Color paddleColor, Color ballColor, int gameTime, int maxScore)
    {
        if (PreviousGameState == GameState.GameOver)
            GameReset();
        Player1Controller = paddleType switch
        {
            PlayerType.AI => new PaddleAI(_paddle, _ball, Score1),
            _ => new PaddlePlayer(_paddle, _ball, Score1)
        };
        _ball.AdjustSize((byte)ballSize);
        _ball.AdjustColor(ballColor);
        _paddle.Resize((byte)paddleSize);
        _paddle.ChangeSpeed((uint)paddleSpeed);
        _paddle.AdjustColor(paddleColor);
        _maxTimeInSeconds = gameTime;
        _maxScore = (byte)maxScore;
        _lives = DefaultLives;
        _timeInSeconds = 0;
        Score1.Reset();
        _blockCollection.ClearLevel();
        if (_isRandomLevel)
            _blockCollection.GenerateLevel();
        else
            _blockCollection.GenerateLevel(_testLevel);
        _middleScreenLabel.Visible = false;
        _menu.Visible = false;
        _menu.ToggleButtons();
        _paddle.ResetPosition();
        _ball.Launch();
        if (!_ball.IsEnabled)
            _ball.ToggleEnable();
        _gameTimer.WaitTime = 1.0;
        _gameTimer.Start();
        UpdateLivesLabel();
        RequestGameState(GameState.Playing);
        _timerLabel.Text = _maxTimeInSeconds.ToString("D4");
        GD.Print("BlockGame: Game started!");
    }
    // *-> Game State Functions
    protected override async void StateGameOver(GameOverReason reason)
    {
        if (CurrentGameState != GameState.GameOver)
            return;
        // ? Ideally swap these to localized strings at some point, but for now this is fine.
        string message = reason switch
        {
            GameOverReason.Player1Lost => "You Lost!",
            GameOverReason.Player1Won => "You Won!",
            GameOverReason.TimeUp => "Time's Up!",
            GameOverReason.ScoreLimitReached => "Score Limit Reached!",
            GameOverReason.LevelCleared => "Level Cleared!",
            _ => "Game Over!"
        };
        SubmitScore();
        ToggleRainbowColorEffect();
        _middleScreenLabel.Text = message;
        _middleScreenLabel.Visible = true;
        AudioManager.PlayAudioClip(SfxGameOver);
        if (_ball.IsEnabled)
            _ball.ToggleEnable();
        _gameTimer.Stop();
        await ToSignal(GetTree().CreateTimer(5.0), "timeout");
        ToggleRainbowColorEffect();
        _middleScreenLabel.Visible = false;
        _menu.Visible = true;
        _menu.ToggleButtons();
        GameReset();
    }
    protected override void StatePaused()
    {
        if (CurrentGameState != GameState.Paused)
            return;
        _gameTimer.Paused = true;
        if (_ball != null && _ball.IsEnabled)
            _ball.ToggleEnable();
        if (_menu != null)
            _menu.Visible = true;
    }
    protected override void StatePlaying()
    {
        if (CurrentGameState != GameState.Playing)
            return;
        _gameTimer.Paused = false;
        if (_ball != null && !_ball.IsEnabled)
            _ball.ToggleEnable();
        if (_menu != null)
            _menu.Visible = false;
    }
    // *-> Event Handlers
    /// <summary>
    /// Handles the block being hit by the ball.
    /// </summary>
    private void HandleBlockHit(Block block)
    {
        block.OnBlockHit();
        Score1.AddPoint();
        if (Score1.CurrentScore >= _maxScore)
        {
            GameOverReason = GameOverReason.ScoreLimitReached;
            RequestGameState(GameState.GameOver);
        }
    }
    /// <summary>
    /// Handles the ball going out of bounds (player loses a life).
    /// </summary>
    private void HandleBallOutOfBounds()
    {
        if (CurrentGameState != GameState.Playing)
            return;
        AudioManager.PlayAudioClip(_sfxOutOfBounds);
        _lives--;
        UpdateLivesLabel();
        if (_lives <= 0)
        {
            GameOverReason = GameOverReason.Player1Lost;
            RequestGameState(GameState.GameOver);
        }
        else
            _paddle.ResetPosition();
    }
    /// <summary>
    /// Handles all blocks being destroyed (level cleared).
    /// </summary>
    private void HandleLevelCleared()
    {
        GameOverReason = GameOverReason.LevelCleared;
        RequestGameState(GameState.GameOver);
    }
    /// <summary>
    /// Updates the game timer each second. Triggers GameOver when time runs out.
    /// </summary>
    private void HandleTimerUpdate()
    {
        if (_timeInSeconds < _maxTimeInSeconds)
        {
            _timeInSeconds++;
            int time = _maxTimeInSeconds - _timeInSeconds;
            _timerLabel.Text = time.ToString("D4");
        }
        else
        {
            GameOverReason = GameOverReason.TimeUp;
            RequestGameState(GameState.GameOver);
        }
    }
    /// <summary>
    /// Updates the lives display label.
    /// </summary>
    private void UpdateLivesLabel()
    {
        if (_livesLabel != null)
            _livesLabel.Text = $"x{_lives}";
    }
}