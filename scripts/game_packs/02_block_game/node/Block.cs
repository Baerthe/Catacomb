namespace BlockGame;

using System;
using Godot;
/// <summary>
/// Represents a block in the BlockGame that can be hit by the ball.
/// Handles hit points, color changes, and destruction effects. Blocks must be 24 x 18 px.
/// </summary>
[GlobalClass]
public sealed partial class Block : StaticBody2D
{
	public event Action<Block> BlockDestroyed;
	[Export] public CollisionShape2D CollisionShape { get; private set; }
	[Export] public ColorRect ColorRect { get; private set; }
	[Export] public GpuParticles2D HitParticles { get; private set; }
	public byte HitPoints { get; private set; } = 1;
	public byte XPOS { get; set; } //!!! This is an unsafe way of doing this; Debugging purposes only
	public byte YPOS { get; set; } //!!! This is an unsafe way of doing this; Debugging purposes only
	private bool _isDestroyed = false;
	private float _delta;
	private static readonly BlockColorMap _blockColorMap = BlockColorMap.Instance;
	public override void _Ready()
	{
		AddToGroup("block");
		HitParticles.Emitting = false;
	}
	public override void _Process(double delta)
	{
		if (!_isDestroyed)
			return;
		RemoveFromGroup("block");
		var duration = 0.05f;
		_delta += (float)delta;
		if (_delta >= duration)
		{
			_delta = 0;
			ColorRect.Color = new Color(ColorRect.Color, ColorRect.Color.A - 0.1f);
			if (ColorRect.Color.A <= 0)
				QueueFree();
		}
	}
	/// <summary>
	/// Handles the block being hit by the ball.
	/// </summary>
	public void OnBlockHit()
	{
		if (_isDestroyed)
			return;
		HitParticles.Emitting = true;
		SetHitPoints((byte)(HitPoints - 1));
		if (HitPoints <= 0)
		{
			_isDestroyed = true;
			CollisionShape.Disabled = true;
			BlockDestroyed?.Invoke(this);
		}
	}
	/// <summary>
	/// Sets the hit points of the block and updates its color.
	/// </summary>
	/// <param name="hitPoints"></param>
	public void SetHitPoints(byte hitPoints)
	{
		HitPoints = hitPoints;
		byte color = HitPoints > 0 ? HitPoints : (byte)1;
		GD.Print($"Setting block ({XPOS},{YPOS}) hit points to {HitPoints}, color {color}");
		ColorRect.Color = _blockColorMap.ColorMap[color];
		if (!_blockColorMap.ParticleColorMap.ContainsKey(color))
			_blockColorMap.AddParticleMaterial(color, HitParticles.ProcessMaterial as ParticleProcessMaterial);
		HitParticles.ProcessMaterial = _blockColorMap.ParticleColorMap[color];
	}
}
