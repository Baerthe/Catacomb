namespace FrogGame;

using Common;
using Godot;
public sealed class FrogAI
{
    private FrogMovable[] _objList;
    public FrogAI (FrogMovable[] objList, Area2D mapBounds)
    {
        _objList = objList;
        foreach (FrogMovable i in _objList)
            i.Init();
        mapBounds.BodyEntered += node =>
        {
            if (node is FrogMovable moveable)
                moveable.InBounds = true;
        };
        mapBounds.BodyExited += node =>
        {
            if (node is FrogMovable moveable)
                moveable.InBounds = false;
        };
    }
    public void Update()
    {
        foreach (FrogMovable i in _objList){
            if (i.InBounds)
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
                    FrogSpeed.normal => 32.5f,
                    FrogSpeed.fast => 39.75f,
                    FrogSpeed.faster => 51.25f,
                    _ => throw new System.InvalidOperationException()
                };
                i.Velocity = speed * direction;
            }
            else
                i.Position = i.InitPosition;
        }
    }
    public void Freeze()
    {
        foreach (FrogMovable i in _objList)
            if (i.IsPhysicsProcessing())
                i.SetPhysicsProcess(false);
    }
    public void Unfreeze()
    {
        foreach (FrogMovable i in _objList)
            if (!i.IsPhysicsProcessing())
                i.SetPhysicsProcess(true);
    }
}
public enum FrogSpeed : byte { slow, normal, fast, faster }