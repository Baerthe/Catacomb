namespace Common;

using Godot;
/// <summary>
/// A resource representing an audio event with an associated audio clip.
/// </summary>
[GlobalClass]
public sealed partial class AudioEvent : Resource
{
    [Export] public AudioStream Clip { get; private set; }
    [Export] public bool IsMusic { get; private set; } = false;
}