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
                    FrogSpeed.slow => 16f,
                    FrogSpeed.normal => 22.5f,
                    FrogSpeed.fast => 29.75f,
                    FrogSpeed.faster => 31.25f,
                    _ => throw new System.InvalidOperationException()
                };
                GD.Print($"{this}: moving {i}...");
                i.MovementVector = speed * direction;
            }
            else
            {
                i.Position = i.InitPosition;
                i.MovementVector = Vector2.Zero;
            }
        }
    }
    public void Freeze()
    {
        GD.Print($"{this}: Freeze Called.");
        foreach (FrogMovable i in _objList)
            if (i.IsPhysicsProcessing())
                i.SetPhysicsProcess(false);
    }
    public void Unfreeze()
    {
        GD.Print($"{this}: Unfreeze Called.");
        foreach (FrogMovable i in _objList)
            if (!i.IsPhysicsProcessing())
                i.SetPhysicsProcess(true);
    }
}
public enum FrogSpeed : byte { slow, normal, fast, faster }