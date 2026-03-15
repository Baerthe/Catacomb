namespace TennisGame;

using Common;
using Godot;
/// <summary>
/// AI controller for the paddle in the tennis_game.
/// Determines paddle movement based on ball Position.
/// </summary>
public class PaddleAI : IController
{
    public BallTennis BallTennis { get; private set; }
    public bool IsLeftSide { get; private set; }
    public Paddle Paddle { get; private set; }
    public Score Score { get; private set; }
    private Direction _lastDirection = Direction.None;
    public PaddleAI(Paddle paddle, BallTennis ball, Score score, bool isLeftSide)
    {
        IsLeftSide = isLeftSide;
        Paddle = paddle;
        Score = score;
        BallTennis = ball;
        GD.Print($"PaddleAI created for {(IsLeftSide ? "Player 1" : "Player 2")}");
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
        int flip = GD.RandRange(0, 100);
        var detectionZone = (Paddle.Size + GD.RandRange(-18, 12)) / 4;
        Direction direction = Direction.None;
        if (flip < 20)
        {
            if (BallTennis.Position.Y < Paddle.Position.Y - detectionZone)
                direction = Direction.Up;
            else if (BallTennis.Position.Y > Paddle.Position.Y + detectionZone)
                direction = Direction.Down;
        }
        switch (_lastDirection)
        {
            case Direction.Up:
                if (flip < 8)
                     break;
                direction = Direction.Up;
                break;
            case Direction.Down:
                if (flip < 8)
                    break;
                direction = Direction.Down;
                break;
            case Direction.None:
                break;
        }
        _lastDirection = direction;
        return direction;
    }
}