namespace Common;

using Godot;
public partial class CursorTracker : Node
{
    public Sprite2D CursorSprite { get; set; }
    public Control BoundsControl { get; set; }
    public override void _Process(double delta)
    {
        if (CursorSprite == null || !IsInstanceValid(CursorSprite)) return;
        Vector2 mousePos = CursorSprite.GetGlobalMousePosition();
        if (BoundsControl != null && IsInstanceValid(BoundsControl))
        {
            Rect2 rect = BoundsControl.GetGlobalRect();
            mousePos.X = Mathf.Clamp(mousePos.X, rect.Position.X, rect.End.X);
            mousePos.Y = Mathf.Clamp(mousePos.Y, rect.Position.Y, rect.End.Y);
        }
        CursorSprite.GlobalPosition = mousePos;
    }
}