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
    [Export] private Button _applyButton;
    [Export] private Button _cancelButton;
    [ExportCategory("Window Options")]
    [Export] private OptionButton _baseSizeOptionButton;
    [Export] private OptionButton _stretchAspectOptionButton;
    [Export] private OptionButton _stretchModeOptionButton;
    [Export] private HSlider _scaleFactorSlider;
    [Export] private Label _scaleFactorValueLabel;
    private Vector2[] _baseWindowSizes = [
            new(640, 480),      // 4:3
            new(800, 600),      // 4:3
            new(1024, 768),     // 4:3
            new(1280, 960),     // 4:3
            new(1280, 720),     // 16:9
            new(1366, 768),     // 16:9
            new(1600, 900),     // 16:9
            new(1920, 1080),    // 16:9
            new(2560, 1440),    // 16:9
            new(2560, 1080),    // 21:9
            new(3440, 1440),    // 21:9
    ];
    private Dictionary<string, (Variant, bool)> _localAudioSettings = [];
    private Dictionary<string, (Variant, bool)> _localUserSettings = [];
    protected override void ConnectControlEvents()
    {
        _channel1Vol.ValueChanged += value =>
        {
            if (_channel1Label != null) _channel1Label.Text = value.ToString();
            UpdateSetting(Sectional.Audio, "Channel1", (float)value);
        };
        _channel2Vol.ValueChanged += value =>
        {
            if (_channel2Label != null) _channel2Label.Text = value.ToString();
            UpdateSetting(Sectional.Audio, "Channel2", (float)value);
        };
        _channel3Vol.ValueChanged += value =>
        {
            if (_channel3Label != null) _channel3Label.Text = value.ToString();
            UpdateSetting(Sectional.Audio, "ChannelMusic", (float)value);
        };
        _baseSizeOptionButton.ItemSelected += HandleWindowBaseSizeItemSelected;
        _stretchModeOptionButton.ItemSelected += HandleWindowStretchModeItemSelected;
        _stretchAspectOptionButton.ItemSelected += HandleWindowStretchAspectItemSelected;
        _scaleFactorSlider.DragEnded += HandleWindowScaleFactorDragEnded;
        if (_applyButton != null) _applyButton.Pressed += ApplySettings;
        if (_cancelButton != null) _cancelButton.Pressed += () => Visible = false;
        Error err = SetupValues();
        if (err != Error.Ok)
            GD.PrintErr($"Settings Menu: Unable to set default values. {err}");
    }
    // *-> Support Functions
    /// <summary>
    /// Commits the local settings dicts to the Settings Manager.
    /// </summary>
    private void ApplySettings()
    {
        // Add username entry manual polling since it's a TextEdit and doesn't fire events implicitly like OptionButtons or Sliders
        if (_nameEnrty != null && !string.IsNullOrWhiteSpace(_nameEnrty.Text))
            UpdateSetting(Sectional.User, "Username", _nameEnrty.Text);
        // Setup control enabled states that might have updated
        if (_channel1Enabled != null && _localAudioSettings.TryGetValue("Channel1", out var ch1))
            UpdateSetting(Sectional.Audio, "Channel1", ch1.Item1, _channel1Enabled.ButtonPressed);

        if (_channel2Enabled != null && _localAudioSettings.TryGetValue("Channel2", out var ch2))
            UpdateSetting(Sectional.Audio, "Channel2", ch2.Item1, _channel2Enabled.ButtonPressed);

        if (_channel3Enabled != null && _localAudioSettings.TryGetValue("ChannelMusic", out var chMus))
            UpdateSetting(Sectional.Audio, "ChannelMusic", chMus.Item1, _channel3Enabled.ButtonPressed);

        GameManagers.Instance.Settings.SaveData(Sectional.Audio, _localAudioSettings);
        GameManagers.Instance.Settings.SaveData(Sectional.User, _localUserSettings);
    }
    /// <summary>
    /// Loads the settings from the Settings Manager.
    /// </summary>
    /// <returns>Error.OK on success.</returns>
    private Error SetupValues()
    {
        var audioDict = GameManagers.Instance.Settings.AudioSettings.Item2;
        var userDict = GameManagers.Instance.Settings.UserSettings.Item2;
        _localAudioSettings = new Dictionary<string, (Variant, bool)>(audioDict);
        _localUserSettings = new Dictionary<string, (Variant, bool)>(userDict);
        if (audioDict != null)
        {
            if (audioDict.TryGetValue("Channel1", out var channel1))
            {
                _channel1Vol.Value = (double)channel1.Item1;
                if (_channel1Enabled != null) _channel1Enabled.ButtonPressed = (bool)channel1.Item2;
                if (_channel1Label != null) _channel1Label.Text = _channel1Vol.Value.ToString();
            }
            if (audioDict.TryGetValue("Channel2", out var channel2))
            {
                _channel2Vol.Value = (double)channel2.Item1;
                if (_channel2Enabled != null) _channel2Enabled.ButtonPressed = (bool)channel2.Item2;
                if (_channel2Label != null) _channel2Label.Text = _channel2Vol.Value.ToString();
            }
            if (audioDict.TryGetValue("ChannelMusic", out var channel3))
            {
                _channel3Vol.Value = (double)channel3.Item1;
                if (_channel3Enabled != null) _channel3Enabled.ButtonPressed = (bool)channel3.Item2;
                if (_channel3Label != null) _channel3Label.Text = _channel3Vol.Value.ToString();
            }
        }
        else
        {
            GD.PrintErr("Settings Menu: Failed to map audio settings.");
            return Error.ParseError;
        }
        if (userDict != null)
        {
            if (userDict.TryGetValue("Username", out var userName))
            {
                if (_nameEnrty != null) _nameEnrty.Text = userName.Item1.AsString();
            }
            if (_baseSizeOptionButton != null && _baseSizeOptionButton.ItemCount == 0)
            {
                foreach (var size in _baseWindowSizes)
                    _baseSizeOptionButton.AddItem($"{(int)size.X}x{(int)size.Y}");
            }
            if (userDict.TryGetValue("Resolution", out var resolutionData))
            {
                var index = resolutionData.Item1.AsVector2();
                int selectedIndex = 0;
                for (int i = 0; i < _baseWindowSizes.Length; i++)
                {
                    if (_baseWindowSizes[i] == index)
                    {
                        selectedIndex = i;
                        break;
                    }
                }
                _baseSizeOptionButton?.Select(selectedIndex);
            }
            if (userDict.TryGetValue("StretchMode", out var modeData))
            {
                var stretchMode = modeData.Item1.AsInt32();
                _stretchModeOptionButton?.Select(stretchMode);
                if (_baseSizeOptionButton != null) _baseSizeOptionButton.Disabled = stretchMode == (int)Window.ContentScaleModeEnum.Disabled;
                if (_stretchAspectOptionButton != null) _stretchAspectOptionButton.Disabled = stretchMode == (int)Window.ContentScaleModeEnum.Disabled;
            }
            if (userDict.TryGetValue("StretchAspect", out var aspectData))
            {
                _stretchAspectOptionButton?.Select(aspectData.Item1.AsInt32());
            }
            if (userDict.TryGetValue("ScaleFactor", out var scaleFactorData))
            {
                if (_scaleFactorSlider != null) _scaleFactorSlider.Value = scaleFactorData.Item1.AsSingle();
                if (_scaleFactorValueLabel != null && _scaleFactorSlider != null)
                    _scaleFactorValueLabel.Text = $"{(int)(_scaleFactorSlider.Value * 100)}%";
            }
        }
        else
        {
            GD.PrintErr("Settings Menu: Failed to map user settings.");
            return Error.ParseError;
        }
        return Error.Ok;
    }
    /// <summary>
    /// Updates the relevant data dict within the local settings dicts.
    /// </summary>
    /// <param name="section"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="enabled"></param>
    private void UpdateSetting(Sectional section, string key, Variant value, bool? enabled = null)
    {
        Dictionary<string, (Variant, bool)> settingsDict = section switch
        {
            Sectional.Audio => _localAudioSettings,
            Sectional.User => _localUserSettings,
            _ => null
        };
        if (settingsDict == null)
        {
            GD.PrintErr($"Settings Menu: Cannot update settings,{section} dict not found.");
            return;
        }
        // Use provided enabled state, otherwise fallback to existing state if present, finally default to true
        bool finalEnabled = enabled ?? (!settingsDict.ContainsKey(key) || settingsDict[key].Item2);
        settingsDict[key] = (value, finalEnabled);
    }
    // *-> Event Handlers
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