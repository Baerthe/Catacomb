namespace FrogGame;

using Common;
using Godot;
public sealed class FrogAI
{
    private FrogMovable[] _objList;
    private Timer _speedSlow;
    private bool _speedSlowCooldown = false;
    private Timer _speedNormal;
    private bool _speedNormalCooldown = false;
    private Timer _speedFast;
    private bool _speedFastCooldown = false;
    private Timer _speedFaster;
    private bool _speedFasterCooldown = false;
    public FrogAI (FrogMovable[] objList, Node Parent)
    {
        _objList = objList;
        _speedSlow = Parent.AddNode<Timer>();
        _speedNormal = Parent.AddNode<Timer>();
        _speedFast = Parent.AddNode<Timer>();
        _speedFaster = Parent.AddNode<Timer>();
        _speedSlow.Timeout += () => _speedSlowCooldown = !_speedFastCooldown;
        _speedNormal.Timeout += () => _speedNormalCooldown = !_speedNormalCooldown;
        _speedFast.Timeout += () => _speedFastCooldown = !_speedFastCooldown;
        _speedFaster.Timeout += () => _speedFasterCooldown = !_speedFasterCooldown;
        _speedSlow.OneShot = true;
        _speedNormal.OneShot = true;
        _speedFast.OneShot = true;
        _speedFaster.OneShot = true;
        _speedSlow.WaitTime = 1f;
        _speedNormal.WaitTime = 0.75f;
        _speedFast.WaitTime = 0.5f;
        _speedFaster.WaitTime = 0.25f;
    }
    public void Update()
    {
        foreach (FrogMovable i in _objList){
            if (i.OnScreen)
                i.Position = Vector2.Down; //temp
        }
    }
}