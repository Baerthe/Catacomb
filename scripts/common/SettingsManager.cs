namespace Common;

using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// Manages saving and loading of settings and game configuration within a 'save' files (godot cfg) in the user directory. We're running system dicts due to gd dict not supporting certain features.
/// </summary>
public sealed class SettingsManager
{
    public event Action<(Sectional , Dictionary<string, (Variant, bool)>)> OnSettingsUpdated;
    public (Sectional , Dictionary<string, (Variant, bool)>) AudioSettings { get; private set;}
    = (Sectional.Audio ,new()
    {
        { "Channel1", (0.9f, true)},
        { "Channel2", (0.7f, true)},
        { "ChannelMusic", (0.6f, true)},
    });
    public (Sectional , Dictionary<string, (Variant, bool)>) UserSettings { get; private set;}
    = (Sectional.User ,new()
    {
        { "Username", ("Player", true)},
    });
    private readonly ConfigFile _configFile;
    private readonly string _configPath = "user://settings/";
    private readonly string _configSave = "user.save";
    public SettingsManager()
    {
        GD.Print("SettingsManager instantiated");
        _configFile = new();
    }
    // *-> Public Methods
    /// <summary>
    /// Loads settings from the config file and updates the relevant properties.
    /// </summary>
    public void LoadData()
    {
        if (_configFile.Load(_configPath + _configSave) == Error.Ok)
        {
            var sections = Enum.GetValues<Sectional>();
            foreach (var section in sections)
            {
                var sectionDict = new Dictionary<string, (Variant, bool)>();
                var keys = _configFile.GetSectionKeys(section.ToString());
                foreach (var key in keys)
                {
                    if (key.EndsWith("_allowed?")) continue;
                    var value = _configFile.GetValue(section.ToString(), key, 0f);
                    var allowed = _configFile.GetValue(section.ToString(), $"{key}_allowed?", true);
                    sectionDict[key] = (value, (bool)allowed);
                }
                switch (section)
                {
                    case Sectional.Audio: AudioSettings = (section, sectionDict); break;
                    case Sectional.User: UserSettings = (section, sectionDict); break;
                }
            }
            OnSettingsUpdated?.Invoke(AudioSettings);
            OnSettingsUpdated?.Invoke(UserSettings);
        } else
        {
            GD.Print("Settings Manager: No settings file found, creating...");
            SaveData(AudioSettings.Item1, AudioSettings.Item2);
            SaveData(UserSettings.Item1, UserSettings.Item2);
        }
    }
    /// <summary>
    /// Saves settings to the config file and updates the relevant properties.
    /// </summary>
    public void SaveData(Sectional section, Dictionary<string, (Variant, bool)> data)
    {
        foreach (var (setting, (value,allowed)) in data)
        {
            _configFile.SetValue(section.ToString(), setting, value);
            _configFile.SetValue(section.ToString(), $"{setting}_allowed?", allowed);
        }
        switch (section)
        {
            case Sectional.Audio: AudioSettings = (section, data); break;
            case Sectional.User: UserSettings = (section, data); break;
        }
        CommitData();
        var dict = section switch
        {
            Sectional.Audio => AudioSettings,
            Sectional.User => UserSettings,
        };
        OnSettingsUpdated?.Invoke(dict);
    }
    /// <summary>
    /// Helper method to save settings data to file.
    /// </summary>
    private void CommitData()
    {
        GD.Print($"Settings Manager: Commiting data to path: {_configPath + _configSave}");
        if (!DirAccess.DirExistsAbsolute(_configPath))
        {
            Error dirErr = DirAccess.MakeDirRecursiveAbsolute(_configPath);
            if (dirErr != Error.Ok)
            {
                GD.PrintErr($"Settings Manager: Failed to create directory '{_configPath}'. Error: {dirErr}");
            }
        }
        var err = _configFile.Save(_configPath + _configSave);
        if (err != Error.Ok)
        {
            GD.PrintErr($"Settings Manager: Failed to save config. Error code: {err}");
        }
    }
}
