namespace BlockGame;

using Godot;
/// <summary>
/// Resource representing level data for BlockGame.
/// It contains the grid layout of blocks as a string of 24 characters per line.
/// 0 is empty, 1 - 4 are blocks with corresponding hit points.
/// </summary>
[GlobalClass]
public sealed partial class LevelData : Resource
{
    [Export(PropertyHint.MultilineText)] public string LevelGrid { get; set; } = "000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n000000000000000000000000\n";
}