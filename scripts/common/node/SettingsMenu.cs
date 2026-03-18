namespace Common;

using System;
using Godot;
/// <summary>
/// The menu which handles settings.
/// </summary>
public sealed partial class SettingsMenu : MenuBase
{
    //?
    // TODO: SettingsMenu will send data into SettingsManager which raises an update settings event. This is because the settings being changed are spread throughout.
    //?
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
    [Export] private PopupMenu _resolutionDrop;
    [Export] private PopupMenu _screenSet;
    [Export] private Button _applyButton;
    [Export] private Button _cancelButton;
    [ExportCategory("GUI Margin Options")]
    [Export] private HSlider _guiMarginSlider;
    [Export] private Label _guiMarginValueLabel;
    [ExportCategory("Window Options")]
    [Export] private OptionButton _baseSizeOptionButton;
    [Export] private OptionButton _stretchAspectOptionButton;
    [Export] private OptionButton _stretchModeOptionButton;
    [Export] private OptionButton _guiAspectRatioOptionButton;
    [Export] private HSlider _scaleFactorSlider;
    [Export] private Label _scaleFactorValueLabel;
    private float _guiAspectRatio = -1.0f;
    protected override void ConnectControlEvents()
    {
        _channel1Vol.ValueChanged += (double value) => _channel1Label.Text = value.ToString();
        _channel2Vol.ValueChanged += (double value) => _channel2Label.Text = value.ToString();
        _channel3Vol.ValueChanged += (double value) => _channel3Label.Text = value.ToString();
        _guiAspectRatioOptionButton.ItemSelected += HandleGuiAspectRatioItemSelected;
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
            if (userDict.TryGetValue("GuiAspectRatio", out var guiAspect))
                _guiAspectRatio = guiAspect.Item1.AsSingle();
            if (userDict.TryGetValue("Resolution", out var resolutionData))
            {
                var r = resolutionData.Item1.AsVector2();
                byte idx = 0;
                if (r == new Vector2(648, 648)) idx = 0;
                else if (r == new Vector2(640, 480)) idx = 1;
                else if (r == new Vector2(720, 480)) idx = 2;
                else if (r == new Vector2(800, 600)) idx = 3;
                else if (r == new Vector2(1152, 648)) idx = 4;
                else if (r == new Vector2(1280, 720)) idx = 5;
                else if (r == new Vector2(1280, 800)) idx = 6;
                else if (r == new Vector2(1680, 720)) idx = 7;
                _baseSizeOptionButton.Select(idx);
            }
            if (userDict.TryGetValue("StretchMode", out var modeData))
                _stretchModeOptionButton.Select(modeData.Item1.AsInt32());
            if (userDict.TryGetValue("StretchAspect", out var aspectData))
                _stretchAspectOptionButton.Select(aspectData.Item1.AsInt32());
            GameManagers.Instance.Settings.ApplyWindowSettings(GetWindow());
        } else
            {
                GD.PrintErr("Settings Menu: Failed to map user settings.");
                return Error.ParseError;
            }
        return Error.Ok;
    }
    private void UpdateSetting(string key, Variant value)
    {
        var settingsDict = GameManagers.Instance.Settings.UserSettings.Item2;
        settingsDict[key] = (value, settingsDict.ContainsKey(key) ? settingsDict[key].Item2 : true);
        GameManagers.Instance.Settings.SaveData(Sectional.User, settingsDict);
    }
    private void HandleGuiAspectRatioItemSelected(long index)
    {
        switch (index)
        {
            case 0: _guiAspectRatio = -1.0f; break;
            case 1: _guiAspectRatio = 5.0f / 4.0f; break;
            case 2: _guiAspectRatio = 4.0f / 3.0f; break;
            case 3: _guiAspectRatio = 3.0f / 2.0f; break;
            case 4: _guiAspectRatio = 16.0f / 10.0f; break;
            case 5: _guiAspectRatio = 16.0f / 9.0f; break;
            case 6: _guiAspectRatio = 21.0f / 9.0f; break;
        }
        UpdateSetting("GuiAspectRatio", _guiAspectRatio);
        Callable.From(GameManagers.Instance.Window.UpdateContainer).CallDeferred();
    }
    private void HandleResized()
    {
        Callable.From(GameManagers.Instance.Window.UpdateContainer).CallDeferred();
    }
    private void HandleWindowBaseSizeItemSelected(long index)
    {
        Vector2 baseWindowSize = new Vector2(1152, 648);
        switch (index)
        {
            case 0: baseWindowSize = new Vector2(648, 648); break;
            case 1: baseWindowSize = new Vector2(640, 480); break;
            case 2: baseWindowSize = new Vector2(720, 480); break;
            case 3: baseWindowSize = new Vector2(800, 600); break;
            case 4: baseWindowSize = new Vector2(1152, 648); break;
            case 5: baseWindowSize = new Vector2(1280, 720); break;
            case 6: baseWindowSize = new Vector2(1280, 800); break;
            case 7: baseWindowSize = new Vector2(1680, 720); break;
        }
        GetWindow().ContentScaleSize = (Vector2I)baseWindowSize;
        UpdateSetting("Resolution", baseWindowSize);
        Callable.From(GameManagers.Instance.Window.UpdateContainer).CallDeferred();
    }
    private void HandleWindowStretchModeItemSelected(long index)
    {
        var stretchMode = (Window.ContentScaleModeEnum)index;
        GetWindow().ContentScaleMode = stretchMode;
        if (_baseSizeOptionButton != null) _baseSizeOptionButton.Disabled = stretchMode == Window.ContentScaleModeEnum.Disabled;
        if (_stretchAspectOptionButton != null) _stretchAspectOptionButton.Disabled = stretchMode == Window.ContentScaleModeEnum.Disabled;
        UpdateSetting("StretchMode", (int)stretchMode);
    }

    private void HandleWindowStretchAspectItemSelected(long index)
    {
        var stretchAspect = (Window.ContentScaleAspectEnum)index;
        GetWindow().ContentScaleAspect = stretchAspect;
        UpdateSetting("StretchAspect", (int)stretchAspect);
    }
    private void HandleWindowScaleFactorDragEnded(bool valueChanged)
    {
        var scaleFactor = (float)_scaleFactorSlider.Value;
        if (_scaleFactorValueLabel != null)
            _scaleFactorValueLabel.Text = $"{(int)(scaleFactor * 100)}%";
        GetWindow().ContentScaleFactor = scaleFactor;
        UpdateSetting("ScaleFactor", scaleFactor);
    }
}