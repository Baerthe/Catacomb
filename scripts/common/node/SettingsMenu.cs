namespace Common;

using Godot;
/// <summary>
/// The menu which handles settings.
/// </summary>
public sealed partial class SettingsMenu : MenuBase
{
    //?
    // TODO: SettingsMenu will send data into SettingsManager which raises an update settings event. This is because the settings being changed are spread throughout.
    //?
    [Export] private HSlider _channel1Vol;
    [Export] private Label _channel1Label;
    [Export] private HSlider _channel2Vol;
    [Export] private Label _channel2Label;
    [Export] private HSlider _channel3Vol;
    [Export] private Label _channel3Label;
    [Export] private TextEdit _nameEnrty;
    [Export] private PopupMenu _resolutionDrop;
    [Export] private PopupMenu _screenSet;
    [Export] private Button _applyButton;
    [Export] private Button _cancelButton;
    protected override void ConnectControlEvents()
    {
        _channel1Vol.ValueChanged += (double value) => _channel1Label.Text = value.ToString();
        _channel2Vol.ValueChanged += (double value) => _channel2Label.Text = value.ToString();
        _channel3Vol.ValueChanged += (double value) => _channel3Label.Text = value.ToString();
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
            _channel1Vol.Value = (double)audioDict["Channel1"].Item1;
            _channel2Vol.Value = (double)audioDict["Channel2"].Item1;
            _channel3Vol.Value = (double)audioDict["ChannelMusic"].Item1;
        } else
            {
                GD.PrintErr("Settings Menu: Failed to map audio settings.");
                return Error.ParseError;
            }
        if (userDict != null)
        {
            
        } else
            {
                GD.PrintErr("Settings Menu: Failed to map user settings.");
                return Error.ParseError;
            }
        return Error.Ok;
    }
}