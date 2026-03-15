namespace BlockGame;

using Common;
using Godot;
using System;
/// <summary>
/// Ball node for BlockGame. Handles movement, wall/paddle/block collisions,
/// and out-of-bounds detection for the breakout-style game.
/// </summary>
[GlobalClass]
public sealed partial class BallBlock : BallBase
{
    public event Action<Block> OnBlockHit;
    public event Action OnOutOfBounds;
    [Export] public AudioEvent HitSound;
    [Export] public AudioEvent ScoreSound;
    public override void _Ready()
    {
        base._Ready();
        Velocity = Vector2.Zero;
        SpeedFactor = 0.015f;
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsEnabled)
            return;
        Velocity = Velocity.Clamp(new Vector2(-8000, -8000), new Vector2(8000, 8000));
        var collision = MoveAndCollide(Velocity * (float)delta * SpeedFactor);
        if (collision != null)
        {
            if (collision.GetCollider() is Block block)
                {
                    AudioInstance.PlayAudioClip(ScoreSound);
                    OnBlockHit?.Invoke(block);
                }
                else
                    AudioInstance.PlayAudioClip(HitSound);
            var normal = collision.GetNormal();
            Velocity = Velocity.Bounce(normal);
            SpeedFactor += SpeedFactor * (Acceleration / 400.0f);
            SpeedFactor = Mathf.Clamp(SpeedFactor, 0f, 0.8f);
            // Prevent ball from getting stuck in a near-horizontal path
            if (Velocity.X > -256 && Velocity.X < 256)
                Velocity = new Vector2(GD.RandRange(-512, 512), Velocity.Y);
        }
    }
    /// <summary>
    /// Called when the ball exits the screen (out of bounds). Fires the event and relaunches.
    /// </summary>
    public override void ResetBall()
    {
        OnOutOfBounds?.Invoke();
        Launch();
    }
    /// <summary>
    /// Launches the ball from its starting Position with a random upward trajectory.
    /// </summary>
    public void Launch()
    {
        Position = InitialPosition;
        Velocity = new Vector2(GD.RandRange(-4000, 4000), -12000);
        SpeedFactor = 0.015f;
    }
}