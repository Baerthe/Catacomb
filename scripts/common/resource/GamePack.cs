namespace Common;

using Godot;
using Godot.Collections;
/// <summary>
/// A resource representing a game pack with its associated metadata.
/// </summary>
[GlobalClass]
public sealed partial class GamePack : Resource
{
    [Export] public Texture2D GameIcon { get; private set; }
    [Export] public string GameName { get; private set; }
    [Export] public PackedScene GameScene { get; private set; }
    [Export] public AudioEvent GameMusic { get; private set; }
    [Export] public Color GameBackgroundColor { get; private set; }
    [Export(PropertyHint.MultilineText)] public string GameDescription { get; private set; }
}