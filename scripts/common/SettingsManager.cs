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
    private readonly string _configPath = "user://config/user.save";
    public SettingsManager()
    {
        _configFile = new();
        GD.Print("SettingsManager instantiated");
        if (_configFile.Load(_configPath) != Error.Ok)
            {
                GD.Print("SettingsManager: Unable to load config file, using defaults, creating new file. Error: " + _configFile.Load(_configPath));
                _configFile.Save(_configPath);
                SaveData(Sectional.Audio, AudioSettings.Item2);
                SaveData(Sectional.User, UserSettings.Item2);
            }
    }
    // *-> Public Methods
    /// <summary>
    /// Loads settings from the config file and updates the relevant properties.
    /// </summary>
    public void LoadData()
    {
        var sections = Enum.GetValues<Sectional>();
        foreach (var section in sections)
        {
            var sectionDict = new Dictionary<string, (Variant, bool)>();
            var keys = _configFile.GetSectionKeys(section.ToString());
            foreach (var key in keys)
            {
                var value = _configFile.GetValue(section.ToString(), key, 0f);
                var allowed = _configFile.GetValue(section.ToString(), $"{key}_allowed?", true);
                sectionDict[key] = ((float)value, (bool)allowed);
            }
            switch (section)
            {
                case Sectional.Audio: AudioSettings = (section, sectionDict); break;
                case Sectional.User: UserSettings = (section, sectionDict); break;
            }
        }
        OnSettingsUpdated?.Invoke(AudioSettings);
        OnSettingsUpdated?.Invoke(UserSettings);
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
        _configFile.Save(_configPath);
        var dict = section switch
        {
            Sectional.Audio => AudioSettings,
            Sectional.User => UserSettings,
        };
        OnSettingsUpdated?.Invoke(dict);
    }
}
