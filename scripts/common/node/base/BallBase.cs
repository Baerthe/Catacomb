namespace Common;

using Godot;
/// <summary>
/// Base class for the ball in the tennis games.
/// Contains common properties and methods for ball behavior.
/// </summary>
public abstract partial class BallBase : CharacterBody2D
{
    [ExportGroup("Properties")]
    [Export(PropertyHint.Range, "1,100")] public byte Acceleration { get; private set; } = 25;
    [Export(PropertyHint.Range, "1,100")] public byte Size { get; private set; } = 8;
    [ExportGroup("Components")]
    [Export] public CollisionShape2D CollisionShape { get; private set; }
    [Export] public ColorRect ColorRect { get; private set; }
    [Export] public GpuParticles2D TrailParticles { get; private set; }
    [Export] public VisibleOnScreenNotifier2D VisibleNotifier { get; private set; }
    public readonly AudioManager AudioInstance = GameManagers.Instance.Audio;
    public Vector2 InitialPosition { get; private set; }
    public bool IsEnabled { get; private set; } = false;
    public float SpeedFactor = 0.05f;
    public override void _Ready()
    {
        AdjustSize(Size);
        VisibleNotifier.ScreenExited += ResetBall;
        InitialPosition = Position;
        AddToGroup("ball");
    }
    /// <summary>
    /// Adjusts the size of the ball and updates related components.
    /// </summary>
    /// <param name="size"></param>
    public void AdjustSize(byte size)
    {
        Size = (byte)Mathf.Clamp(size, 8, 32);
        CollisionShape.Shape = new CircleShape2D() { Radius = Size / 2 };
        ColorRect.Size = new Vector2(Size, Size);
        ColorRect.Position = new Vector2(-Size / 2, -Size / 2);
        TrailParticles.ProcessMaterial.Set("scale_max", Size);
        TrailParticles.ProcessMaterial.Set("scale_min", Size);
    }
    /// <summary>
    /// Adjusts the color of the ball and its trail effect.
    /// </summary>
    /// <param name="color"></param>
    public void AdjustColor(Color color)
    {
        ColorRect.Color = color;
        TrailParticles.ProcessMaterial.Set("color", color);
    }
    public abstract void ResetBall();
    /// <summary>
    /// Toggles whether the ball is enabled (moving) or not.
    /// </summary>
    public void ToggleEnable()
    {
        IsEnabled = !IsEnabled;
        TrailParticles.Emitting = IsEnabled;
    }
}