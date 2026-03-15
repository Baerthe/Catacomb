namespace BlockGame;

using Common;
using Godot;
/// <summary>
/// AI controller for the paddle in the tennis_game.
/// Determines paddle movement based on ball position.
/// </summary>
public class PaddlePlayer : IController
{
    public BallBlock Ball { get; private set; }
    public PaddleBlock Paddle { get; private set; }
    public Score Score { get; private set; }
    public PaddlePlayer(PaddleBlock paddle, BallBlock ball,Score score)
    {
        Paddle = paddle;
        Score = score;
        Ball = ball;
        GD.Print($"PaddlePlayer created");
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
        if (Input.IsActionPressed("p1_move_up"))
            direction = Direction.Left;
        if (Input.IsActionPressed("p1_move_down"))
            direction = Direction.Right;
        return direction;
    }
}