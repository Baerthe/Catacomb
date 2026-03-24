namespace TennisGame;

using Common;
using Godot;
using System;
/// <summary>
/// Main game controller for tennis_game. tennis_game is a pretty simple game, so we will have Main being the controller and orchestrator of the game.
/// It will manage the paddles and the ball, and handle the game logic.
/// </summary>
public sealed partial class MainTennis : PackBase
{
    public override event Action<byte, uint> OnScoreSubmission;
    [ExportGroup("References")]
    [Export] private MenuTennis _menu;
    [Export] private Timer _gameTimer;
    [Export] private Paddle _paddleP1;
    [Export] private Paddle _paddleP2;
    [Export] private BallTennis _ball;
    [ExportGroup("Rects")]
    [Export] private ColorRect _crossRect;
    [Export] private ColorRect _dividerRect;
    [ExportGroup("HUD Properties")]
    [Export] private Label _scoreP1Label;
    [Export] private Label _scoreP2Label;
    [Export] private Label _timerLabel;
    [Export] private Label _middleScreenLabel;
    // *-> Fields
    private int _timeInSeconds = 0;
    private int _maxTimeInSeconds = 9999;
    private byte _maxScore = 255;
    // *-> Godot Overrides
    public override void _Ready()
    {
        Score1 = new Score(_scoreP1Label);
        Score2 = new Score(_scoreP2Label);
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.OnGameStart += GameStart;
        _gameTimer.Timeout += TimerUpdate;
        _menu.Visible = true;
        AudioManager.PlayAudioClip(SfxGameStart);
    }
    protected override void GameReset()
    {
        _gameTimer.Stop();
        _paddleP1.ResetPosition();
        _paddleP2.ResetPosition();
        _ball.ResetBall();
        Score1.Reset();
        Score2.Reset();
        _timeInSeconds = 0;
        _gameTimer.WaitTime = 1.0;
        RequestGameState(GameState.Paused);
    }
    private void TriggerGameOver()
    {
        GameOverReason =
            Score1.CurrentScore > Score2.CurrentScore ? GameOverReason.Player1Won :
            Score2.CurrentScore > Score1.CurrentScore ? GameOverReason.Player1Lost :
            GameOverReason.EveryoneWon;
        RequestGameState(GameState.GameOver);
    }
    protected override async void StateGameOver(GameOverReason reason)
    {
        if (CurrentGameState != GameState.GameOver)
            return;
        _middleScreenLabel.Text = reason switch
        {
            GameOverReason.Player1Won => "Player 1 Wins!",
            GameOverReason.Player1Lost => "Player 2 Wins!",
            GameOverReason.EveryoneWon => "It's a Tie!",
            _ => "Game Over!"
        };
        SubmitScore();
        ToggleRainbowColorEffect();
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
        if (_gameTimer.IsStopped())
            _gameTimer.Start();
        _gameTimer.Paused = false;
        if (_ball != null && !_ball.IsEnabled)
            _ball.ToggleEnable();
        if (_menu != null)
            _menu.Visible = false;
    }
    /// <summary>
    /// Starts a new game with the given parameters sent from the Menu.
    /// </summary>
    /// <param name="player1Type"></param>
    /// <param name="player2Type"></param>
    /// <param name="ballSize"></param>
    /// <param name="paddle1Size"></param>
    /// <param name="paddle2Size"></param>
    /// <param name="paddle1Speed"></param>
    /// <param name="paddle2Speed"></param>
    /// <param name="paddle1Color"></param>
    /// <param name="paddle2Color"></param>
    /// <param name="gameTime"></param>
    /// <param name="maxScore"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void GameStart(PlayerType player1Type, PlayerType player2Type, int ballSize, int paddle1Size, int paddle2Size, int paddle1Speed, int paddle2Speed, Color paddle1Color, Color paddle2Color, Color ballColor, int gameTime, int maxScore)
    {
        if (PreviousGameState == GameState.GameOver)
            GameReset();
        Player1Controller?.Detach();
        Player2Controller?.Detach();
        Player1Controller = player1Type switch
        {
            PlayerType.Player1 => new PaddlePlayer(_paddleP1, _ball, Score1, true, true),
            PlayerType.Player2 => new PaddlePlayer(_paddleP1, _ball, Score1, true, false),
            PlayerType.AI => new PaddleAI(_paddleP1, _ball, Score1, true),
            _ => throw new ArgumentOutOfRangeException(nameof(player1Type), "Invalid player type")
        };
        GD.Print("Controller 1 created.");
        Player2Controller = player2Type switch
        {
            PlayerType.Player1 => new PaddlePlayer(_paddleP2, _ball, Score2, false, true),
            PlayerType.Player2 => new PaddlePlayer(_paddleP2, _ball, Score2, false, false),
            PlayerType.AI => new PaddleAI(_paddleP2, _ball, Score2, false),
            _ => throw new ArgumentOutOfRangeException(nameof(player2Type), "Invalid player type")
        };
        GD.Print("Controller 2 created.");
        Player1Controller.Attach();
        Player2Controller.Attach();
        GD.Print("Controllers attached.");
        _middleScreenLabel.Visible = false;
        _menu.Visible = false;
        _menu.ToggleButtons();
        _ball.AdjustSize((byte)ballSize);
        _ball.AdjustColor(ballColor);
        GD.Print("Ball adjusted.");
        _paddleP1.Resize((byte)paddle1Size);
        _paddleP2.Resize((byte)paddle2Size);
        GD.Print("Paddles resized.");
        _paddleP1.ChangeSpeed((uint)paddle1Speed);
        _paddleP2.ChangeSpeed((uint)paddle2Speed);
        GD.Print("Paddles speed changed.");
        _paddleP1.AdjustColor(paddle1Color);
        _paddleP2.AdjustColor(paddle2Color);
        GD.Print("Paddles color adjusted.");
        _maxTimeInSeconds = gameTime;
        GD.Print("Game time set.");
        _maxScore = (byte)maxScore;
        GD.Print("Max score set.");
        AudioManager.PlayAudioClip(MusicGameMain);
        if (!_ball.IsEnabled)
            _ball.ToggleEnable();
        RequestGameState(GameState.Playing);
    }
    /// <summary>
    /// Sends out the highest of the player scores.
    /// </summary>
    protected override void SubmitScore()
    {
        var score = Score1.CurrentScore >= Score2.CurrentScore ? Score1.CurrentScore : Score2.CurrentScore;
        OnScoreSubmission?.Invoke(1, score);
        OnScoreSubmission?.Invoke(2, score);
    }
    /// <summary>
    /// Updates the game timer each second. Calls GameOver if the max time (or score) is reached.
    /// </summary>
    private void TimerUpdate()
    {
        if (Score1.CurrentScore >= _maxScore || Score2.CurrentScore >= _maxScore)
            TriggerGameOver();
        if (_timeInSeconds < _maxTimeInSeconds)
        {
            _timeInSeconds++;
            int time = _maxTimeInSeconds - _timeInSeconds;
            _timerLabel.Text = time.ToString("D4");
        } else
            TriggerGameOver();
    }
}