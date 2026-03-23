namespace FrogGame;

using Common;
using Godot;
public sealed class FrogAI
{
    private FrogMovable[] _objList;
    public FrogAI (FrogMovable[] objList, Node Parent)
    {
        _objList = objList;
    }
    public void Update()
    {
        foreach (FrogMovable i in _objList){
            if (i.OnScreen)
                i.Velocity += 16f * i.MovingDirection;
            else
                i.Position = i.InitPosition;
        }
    }
}
public enum FrogSpeed : byte { slow, normal, fast, faster }