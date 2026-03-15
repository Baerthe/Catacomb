namespace BlockGame;

using Common;
using Godot;
/// <summary>
/// AI controller for the paddle in the block_game.
/// Determines paddle movement based on ball Position.
/// </summary>
public class PaddleAI : IController
{
    public BallBlock Ball { get; private set; }
    public PaddleBlock Paddle { get; private set; }
    public Score Score { get; private set; }
    private Direction _lastDirection = Direction.None;
    public PaddleAI(PaddleBlock paddle, BallBlock ball, Score score)
    {
        Paddle = paddle;
        Score = score;
        Ball = ball;
        GD.Print($"PaddleAI created");
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
            if (Ball.Position.X < Paddle.Position.X - detectionZone)
                direction = Direction.Left;
            else if (Ball.Position.X > Paddle.Position.X + detectionZone)
                direction = Direction.Right;
        }
        switch (_lastDirection)
        {
            case Direction.Left:
                if (flip < 8)
                     break;
                direction = Direction.Left;
                break;
            case Direction.Right:
                if (flip < 8)
                    break;
                direction = Direction.Right;
                break;
            case Direction.None:
                break;
        }
        _lastDirection = direction;
        return direction;
    }
}