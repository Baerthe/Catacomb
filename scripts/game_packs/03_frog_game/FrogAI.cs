namespace FrogGame;

using Common;
using Godot;
public sealed class FrogAI
{
    private FrogMovable[] _objList;
    public FrogAI (FrogMovable[] objList)
    {
        _objList = objList;
    }
    public void Update()
    {
        foreach (FrogMovable i in _objList){
            if (i.OnScreen)
            {
                Vector2 direction = i.MovingDirection switch
                {
                    Direction.Up => Vector2.Up,
                    Direction.Left => Vector2.Left,
                    Direction.Down => Vector2.Down,
                    Direction.Right => Vector2.Right,
                    Direction.None => Vector2.Zero,
                    _ => throw new System.InvalidOperationException()
                };
                float speed = i.Speed switch
                {
                    FrogSpeed.slow => 26f,
                    FrogSpeed.normal => 28.5f,
                    FrogSpeed.fast => 29.75f,
                    FrogSpeed.faster => 31.25f,
                    _ => throw new System.InvalidOperationException()
                };
                i.Velocity = speed * direction;
            }
            else
                i.Position = i.InitPosition;
        }
    }
}
public enum FrogSpeed : byte { slow, normal, fast, faster }