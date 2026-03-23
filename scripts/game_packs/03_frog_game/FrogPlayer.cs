namespace FrogGame;

using Common;
using Godot;
public sealed class FrogPlayer(FrogCharacter _character) : IController
{
    public void Update()
    {
        Direction direction = GetInputDirection();
        if (direction == Direction.None)
            return;
        _character.Move(direction);
    }
    public Direction GetInputDirection()
    {
        Direction direction = Direction.None;
        if (Input.IsActionPressed("p1_move_up"))
            direction = Direction.Up;
        if (Input.IsActionPressed("p1_move_left"))
            direction = Direction.Left;
        if (Input.IsActionPressed("p1_move_down"))
            direction = Direction.Down;
        if (Input.IsActionPressed("p1_move_right"))
            direction = Direction.Right;
        return direction;
    }
}