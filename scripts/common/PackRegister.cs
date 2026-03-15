namespace Common;

using Godot;
using System.Collections.Generic;
/// <summary>
/// Registry for game packs, keeps a dictionary of available packs.
/// </summary>
public sealed class PackRegister
{
    public static PackRegister Instance { get; private set; }
    public Dictionary<string, GamePack> GamePacks { get; private set; }
    public PackRegister()
    {
        if (Instance != null)
        {
            GD.PrintErr("PackRegister: Attempted to create a second instance of PackRegister.");
            return;
        }
        GD.Print("PackRegister: Initializing PackRegister and loading game packs.");
        GamePacks = LoadGamePacks();
        if (GamePacks == null || GamePacks.Count == 0)
            GD.PrintErr("PackRegister: No game packs found. Check LoadGamePacks method for details.");
        else
            GD.Print($"PackRegister: Successfully loaded {GamePacks.Count} game packs.");
        Instance = this;
    }
    /// <summary>
    /// Loads all game packs from the designated resource directory.
    /// </summary>
    /// <returns></returns>
    private static Dictionary<string, GamePack> LoadGamePacks()
    {
        string hardPath = "res://assets/resources/packs";
        var dir = DirAccess.Open(hardPath);
        var packs = new Dictionary<string, GamePack>();
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (fileName.EndsWith(".tres"))
                {
                    var packPath = $"{hardPath}/{fileName}";
                    GD.Print($"PackRegister: Found pack file at: {packPath}");
                    GamePack pack = GD.Load<GamePack>(packPath);
                    packs.Add(pack.GameName, pack);
                    GD.Print($"PackRegister: checked pack: {pack.GameName} from {packPath}");
                }
                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
        } else
            GD.PrintErr("PackRegister: Failed to open packs directory.");
        return packs;
    }
}