namespace TennisGame;

using Common;
using Godot;
using System;
/// <summary>
/// Represents the ball in the tennis_game.
/// Handles movement, collisions, scoring, and visual effects.
/// </summary>
[GlobalClass]
public sealed partial class BallTennis : BallBase
{
    public event Action<bool> OnOutOfBounds;
    [Export] public AudioEvent HitSound;
    [Export] public AudioEvent ScoreSound;
    public override void _PhysicsProcess(double delta)
    {
        if (!IsEnabled)
            return;
        Velocity = Velocity.Clamp(new Vector2(-12000,-12000), new Vector2(12000, 12000));
        var collision = MoveAndCollide(Velocity * (float)delta * SpeedFactor);
        if (collision != null)
        {
            AudioInstance.PlayAudioClip(HitSound);
            var normal = collision.GetNormal();
            Velocity = Velocity.Bounce(normal);
            SpeedFactor += SpeedFactor * (Acceleration / 200.0f);
            SpeedFactor = Mathf.Clamp(SpeedFactor, 0f, 0.8f);
            if (Velocity.Y > -256 && Velocity.Y < 256)
                Velocity = new Vector2(Velocity.X, GD.RandRange(-512, 512));
        }
    }
    /// <summary>
    /// Resets the ball Position and velocity.
    /// </summary>
    public override void ResetBall()
    {
        Velocity = Vector2.Zero;
        Position = InitialPosition;
        SpeedFactor = 0.05f;
        var flip = GD.Randf() < 0.5f ? -1 : 1;
        if (Position.X == 0)
        {
            if (flip < 0)
                Velocity = new Vector2( -8000, GD.RandRange(-512, 512));
            else
                Velocity = new Vector2( 8000, GD.RandRange(-512, 512));
            return;
        }
        if (Position.X != 0)
        {
            var winner = Position.X >= 0;
            AudioInstance.PlayAudioClip(ScoreSound);
            OnOutOfBounds?.Invoke(winner);
            if (winner)
                Velocity = new Vector2( -8000, GD.RandRange(-512, 512));
            else
                Velocity = new Vector2( 8000, GD.RandRange(-512, 512));
        }
    }
}