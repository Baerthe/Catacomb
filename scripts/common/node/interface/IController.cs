namespace Common;

using Godot;
/// <summary>
/// Interface for a controller that manages movement.
/// </summary>
public interface IController
{
    protected Direction GetInputDirection();
    void Update();
    void Attach(){}// default no-op
    void Detach(){}// default no-op
}