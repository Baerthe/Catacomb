namespace Common;

using Godot;
/// <summary>
/// Our custom mouse cursor.
/// </summary>
public partial class CursorTracker : Node
{
    public Sprite2D CursorSprite { get; set; }
    public Control BoundsControl { get; set; }
    public override void _Process(double delta)
    {
        if (CursorSprite == null || !IsInstanceValid(CursorSprite)) return;
        Vector2 mousePos = CursorSprite.GetGlobalMousePosition();
        Vector2 mousePosReal = mousePos;
        if (IsInstanceValid(BoundsControl))
        {
            Rect2 rect = BoundsControl.GetGlobalRect();
            mousePos.X = Mathf.Clamp(mousePos.X, rect.Position.X, rect.End.X);
            mousePos.Y = Mathf.Clamp(mousePos.Y, rect.Position.Y, rect.End.Y);
        }
        CursorSprite.GlobalPosition = mousePos;
        if (mousePos != mousePosReal)
            Input.SetMouseMode(Input.MouseModeEnum.Visible);
        else
            Input.SetMouseMode(Input.MouseModeEnum.Hidden);
    }
}