namespace Common;

using Godot;
using Godot.Collections;
/// <summary>
/// Manages loading and saving scores for game packs. Scores are stored in 'scores' (godot cfg) files in the user directory, keyed by pack name derived from scene string name.
/// </summary>
public sealed class ScoreManager
{
    public Dictionary<string, uint> CurrentScores { get; set; }
    public string CurrentP1 { get; private set;} = "test";
    public string CurrentP2 { get; private set;} = "test";
    private readonly string _savePath = "user://saves";
    private readonly string _saveFileName = "high.scores";
    private readonly string _section = "High Scores";
    private readonly Dictionary<string, uint> _defaultScores = new()
    {
        { "Tim" , 1100},
        { "Kim" , 1000},
        { "Jim" , 900},
        { "Zim" , 800},
        { "Qim" , 700}
    };
    public ScoreManager() => GD.Print("ScoreManager: instantiated");
    /// <summary>
    /// Loads scores for the given game pack from a SCORES file. If no file exists, creates a new one with default scores.
    /// <returns>Dictionary of scores keyed by scorer name</returns>
    public void LoadScores(string pack)
    {
        string fullPath = $"{_savePath}/{pack}/";
        Dictionary<string, uint> save = [];
        ConfigFile config = new();
        if (config.Load(fullPath + _saveFileName) == Error.Ok)
        {
            foreach (string player in config.GetSectionKeys(_section))
            {
                var playerScore = (uint)config.GetValue(_section, player);
                save.Add(player, playerScore);
            }
            CurrentScores = save;
        } else
        {
            GD.Print($"ScoreManager: Score table for {pack} could not be loaded. Creating...");
            SaveData(_defaultScores, config, fullPath);
            CurrentScores = _defaultScores;
        }
    }
    /// <summary>
    /// Saves scores for the given game pack to a SCORES file. If no file exists, creates a new one. If a file exists, it is overwritten with the new scores.
    /// </summary>
    public void SaveScores(string pack)
    {
        string fullPath = $"{_savePath}/{pack}";
        ConfigFile config = new();
        if (config.Load(fullPath + _saveFileName) == Error.Ok)
            SaveData(CurrentScores, config, fullPath);
        else
            GD.PrintErr("ScoreManager: Could not save?");
    }
    /// <summary>
    /// Submits a new score for a given player. If the player already has a score, it is only updated if the new score is higher. Player parameter is a byte that should be 1 or 2, representing player 1 or player 2 respectively. The score manager uses the CurrentP1 and CurrentP2 properties as keys for the score dictionary, so make sure to update those with UpdateUserName before submitting scores.
     /// *Note: This method does not automatically save scores to file; call SaveScores to persist changes.
    /// </summary>
    /// <param name="player"></param>
    /// <param name="score"></param>
    public void SubmitScore(byte player, uint score)
    {
        if (score == 0)
            return;
        if (player > 2 || player < 1 )
        {
            GD.PrintErr("ScoreManager: Player byte must be 1 or 2!");
            return;
        }
        string playerName = player == 1 ? CurrentP1 : CurrentP2;
        if (CurrentScores.ContainsKey(playerName))
        {
            if (score > CurrentScores[playerName])
                CurrentScores[playerName] = score;
        } else
            CurrentScores.Add(playerName, score);
    }
    /// <summary>
    /// Updates the current username for desired player as an internal ref for the score manager; this does not save the username to the save data.
    /// </summary>
    /// <param name="name"></param>
    public void UpdateUserName (byte player, string name)
    {
        if (player > 2 || player < 1 )
        {
            GD.PrintErr("ScoreManager: Player byte must be 1 or 2!");
            return;
        } else if (player == 1)
            CurrentP1 = name;
        else
            CurrentP2 = name;
    }
    /// <summary>
    /// Helper method to save score data to a file.
    /// Casts the score dict to a list to perform easy sorting because godot dict do not support LINQ.
    /// </summary>
    private void SaveData(Dictionary<string, uint> scores, ConfigFile config, string fullPath)
    {
        var dir = DirAccess.Open(fullPath.GetBaseDir());
        if (dir == null)
            DirAccess.MakeDirAbsolute(fullPath);
        System.Collections.Generic.List<(string, uint)> sortedScores = [];
        foreach (var score in scores)
            sortedScores.Add((score.Key, score.Value));
        sortedScores.Sort((a, b) => b.Item2.CompareTo(a.Item2));
        if (sortedScores.Count > 5)
            sortedScores.RemoveRange(5, sortedScores.Count - 5);
        foreach (var (player, score) in sortedScores)
            config.SetValue(_section, player, score);
        config.Save(fullPath + _saveFileName);
        GD.Print($"ScoreManager: Saved {sortedScores.Count} scores to {fullPath}{_saveFileName}");
    }
}