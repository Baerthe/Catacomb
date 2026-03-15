namespace Common;

using Godot;
using System;
/// <summary>
/// Handles the animations between loading game packs.
/// </summary>
public sealed partial class LoadingControl : Control
{
    [Export] private AnimationPlayer _animator;
}