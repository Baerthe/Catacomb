namespace TennisGame;

using Common;
using Godot;
/// <summary>
/// AI controller for the paddle in the tennis_game.
/// Determines paddle movement based on ball position.
/// </summary>
public class PaddlePlayer : IController
{
    public BallTennis BallTennis { get; private set; }
    public bool IsLeftSide { get; private set; }
    public Paddle Paddle { get; private set; }
    public Score Score { get; private set; }
    private readonly string _inputPrefix;
    public PaddlePlayer(Paddle paddle, BallTennis ball,Score score, bool isLeftSide, bool isPlayer1)
    {
        IsLeftSide = isLeftSide;
        _inputPrefix = isPlayer1 ? "p1_" : "p2_";
        Paddle = paddle;
        Score = score;
        BallTennis = ball;
        GD.Print($"PaddlePlayer created for {(IsLeftSide ? "Player 1" : "Player 2")}");
    }
    public void Attach() => BallTennis.OnOutOfBounds += OnPointScore;
    public void Detach() => BallTennis.OnOutOfBounds -= OnPointScore;
    public void OnPointScore(bool isLeftSide)
    {
        if (isLeftSide && IsLeftSide || !isLeftSide && !IsLeftSide)
            Score.AddPoint();
    }
    public void Update()
    {
        Direction direction = GetInputDirection();
        if (direction == Direction.None)
            return;
        Paddle.Move(direction);
    }
    public Direction GetInputDirection()
    {
        Direction direction = Direction.None;
        if (Input.IsActionPressed($"{_inputPrefix}move_up"))
            direction = Direction.Up;
        if (Input.IsActionPressed($"{_inputPrefix}move_down"))
            direction = Direction.Down;
        return direction;
    }
}