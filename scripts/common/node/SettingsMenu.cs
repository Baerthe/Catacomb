namespace Common;

using Godot;
using System.Collections.Generic;
/// <summary>
/// The menu which handles settings.
/// </summary>
public sealed partial class SettingsMenu : MenuBase
{
    [ExportCategory("Nodes")]
    [Export] private HSlider _channel1Vol;
    [Export] private CheckBox _channel1Enabled;
    [Export] private Label _channel1Label;
    [Export] private HSlider _channel2Vol;
    [Export] private CheckBox _channel2Enabled;
    [Export] private Label _channel2Label;
    [Export] private HSlider _channel3Vol;
    [Export] private CheckBox _channel3Enabled;
    [Export] private Label _channel3Label;
    [Export] private TextEdit _nameEnrty;
    [Export] private Button _cancelButton;
    [ExportCategory("Window Options")]
    [Export] private OptionButton _baseSizeOptionButton;
    [Export] private OptionButton _stretchAspectOptionButton;
    [Export] private OptionButton _stretchModeOptionButton;
    [Export] private HSlider _scaleFactorSlider;
    [Export] private Label _scaleFactorValueLabel;
    private Vector2[] _baseWindowSizes = {
            new Vector2(640, 480),      // 4:3
            new Vector2(800, 600),      // 4:3
            new Vector2(1024, 768),     // 4:3
            new Vector2(1280, 960),     // 4:3
            new Vector2(1280, 720),     // 16:9
            new Vector2(1366, 768),     // 16:9
            new Vector2(1600, 900),     // 16:9
            new Vector2(1920, 1080),    // 16:9
            new Vector2(2560, 1440),    // 16:9
            new Vector2(2560, 1080),    // 21:9
            new Vector2(3440, 1440),    // 21:9
    };
    protected override void ConnectControlEvents()
    {
        _channel1Vol.ValueChanged += (double value) => _channel1Label.Text = value.ToString();
        _channel2Vol.ValueChanged += (double value) => _channel2Label.Text = value.ToString();
        _channel3Vol.ValueChanged += (double value) => _channel3Label.Text = value.ToString();
        _baseSizeOptionButton.ItemSelected += HandleWindowBaseSizeItemSelected;
        _stretchModeOptionButton.ItemSelected += HandleWindowStretchModeItemSelected;
        _stretchAspectOptionButton.ItemSelected += HandleWindowStretchAspectItemSelected;
        _scaleFactorSlider.DragEnded += HandleWindowScaleFactorDragEnded;
        Error err = SetupValues();
        if (err != Error.Ok)
            GD.PrintErr($"Settings Menu: Unable to set default values. {err}");
    }
    /// <summary>
    /// Loads the settings from the Settings Manager.
    /// </summary>
    /// <returns>Error.OK on success.</returns>
    private Error SetupValues()
    {
        var audioDict = GameManagers.Instance.Settings.AudioSettings.Item2;
        var userDict = GameManagers.Instance.Settings.UserSettings.Item2;
        if (audioDict != null)
        {
            if (audioDict.TryGetValue("Channel1", out var channel1))
                _channel1Vol.Value = (double)channel1.Item1;
            if (audioDict.TryGetValue("Channel2", out var channel2))
                _channel2Vol.Value = (double)channel2.Item1;
            if (audioDict.TryGetValue("ChannelMusic", out var channel3))
                _channel3Vol.Value = (double)channel3.Item1;
            if (audioDict.TryGetValue("Channel1", out var channel1Enabled))
                _channel1Enabled.ToggleMode = (bool)channel1Enabled.Item2;
            if (audioDict.TryGetValue("Channel2", out var channel2Enabled))
                _channel2Enabled.ToggleMode = (bool)channel2Enabled.Item2;
            if (audioDict.TryGetValue("ChannelMusic", out var channel3Enabled))
                _channel3Enabled.ToggleMode = (bool)channel3Enabled.Item2;
        } else
            {
                GD.PrintErr("Settings Menu: Failed to map audio settings.");
                return Error.ParseError;
            }
        if (userDict != null)
        {
            if (userDict.TryGetValue("Username", out var userName))
                _nameEnrty.Text = userName.Item1.AsString();
            if (userDict.TryGetValue("Resolution", out var resolutionData))
            {
                foreach (var size in _baseWindowSizes)
                    _baseSizeOptionButton.AddItem($"{(int)size.X}x{(int)size.Y}");
                var index = resolutionData.Item1.AsVector2();
                int selectedIndex = 0;
                for (int i = 0; i < _baseWindowSizes.Length; i++)
                {
                    if (_baseWindowSizes[i] == index)
                        {selectedIndex = i; break; }
                }
                _baseSizeOptionButton.Select(selectedIndex);
            }
            if (userDict.TryGetValue("StretchMode", out var modeData))
                _stretchModeOptionButton.Select(modeData.Item1.AsInt32());
            if (userDict.TryGetValue("StretchAspect", out var aspectData))
                _stretchAspectOptionButton.Select(aspectData.Item1.AsInt32());
        } else
            {
                GD.PrintErr("Settings Menu: Failed to map user settings.");
                return Error.ParseError;
            }
        return Error.Ok;
    }
    /// <summary>
    /// Updates the relevant data dict within the settings manager and raises the event through the manager to update the settings globally.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="enabled"></param>
    private void UpdateSetting(Sectional section, string key, Variant value, bool enabled = true)
    {
        Dictionary<string, (Variant, bool)> settingsDict = section switch
        {
            Sectional.Audio => GameManagers.Instance.Settings.AudioSettings.Item2,
            Sectional.User => GameManagers.Instance.Settings.UserSettings.Item2,
            _ => null
        };
        if (settingsDict == null)
        {
            GD.PrintErr($"Settings Menu: Cannot update settings,{section} dict not found.");
            return;
        }
        settingsDict[key] = (value, settingsDict.ContainsKey(key) ? settingsDict[key].Item2 : enabled);
        GameManagers.Instance.Settings.SaveData(section, settingsDict);
    }
    /// <summary>
    /// Handles when a new base window size is selected from the options. Applies the new base window size to the Window and updates the setting in the Settings Manager.
    /// </summary>
    /// <param name="index"></param>
    private void HandleWindowBaseSizeItemSelected(long index)
    {
        Vector2 baseWindowSize = _baseWindowSizes[index];
        UpdateSetting(Sectional.User, "Resolution", baseWindowSize);
    }
    /// <summary>
    /// Handles when a new stretch mode is selected from the options. Applies the new stretch mode to the Window and updates the setting in the Settings Manager.
    /// </summary>
    /// <param name="index"></param>
    private void HandleWindowStretchModeItemSelected(long index)
    {
        var stretchMode = (Window.ContentScaleModeEnum)index;
        if (_baseSizeOptionButton != null) _baseSizeOptionButton.Disabled = stretchMode == Window.ContentScaleModeEnum.Disabled;
        if (_stretchAspectOptionButton != null) _stretchAspectOptionButton.Disabled = stretchMode == Window.ContentScaleModeEnum.Disabled;
        UpdateSetting(Sectional.User, "StretchMode", (int)stretchMode);
    }
    /// <summary>
    /// Handles when a new stretch aspect is selected from the options. Applies the new stretch aspect to the Window and updates the setting in the Settings Manager.
    /// </summary>
    /// <param name="index"></param>
    private void HandleWindowStretchAspectItemSelected(long index)
    {
        var stretchAspect = (Window.ContentScaleAspectEnum)index;
        UpdateSetting(Sectional.User, "StretchAspect", (int)stretchAspect);
    }
    /// <summary>
    /// Handles when the window scale factor drag ends. Updates the scale factor for the Window and saves the setting in the Settings Manager.
    /// </summary>
    /// <param name="valueChanged"></param>
    private void HandleWindowScaleFactorDragEnded(bool valueChanged)
    {
        var scaleFactor = (float)_scaleFactorSlider.Value;
        if (_scaleFactorValueLabel != null)
            _scaleFactorValueLabel.Text = $"{(int)(scaleFactor * 100)}%";
        UpdateSetting(Sectional.User, "ScaleFactor", scaleFactor);
    }
}