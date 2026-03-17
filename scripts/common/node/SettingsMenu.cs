namespace Common;

using Godot;
using System;
/// <summary>
/// The menu which handles settings.
/// </summary>
public sealed partial class SettingsMenu : Control
{
    [Export] private HSlider _channel1Vol;
    [Export] private Label _channel1Label;
    [Export] private HSlider _channel2Vol;
    [Export] private Label _channel2Label;
    [Export] private HSlider _channel3Vol;
    [Export] private Label _channel3Label;
    [Export] private TextEdit _nameEnrty;
    [Export] private PopupMenu _resolutionDrop;
    [Export] private PopupMenu _screenSet;
    private SettingsManager _settingsManager;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _settingsManager = GameManagers.Instance.Settings;
        //?
        // TODO: SettingsMenu will send data into SettingsManager which raises an update settings event. This is because the settings being changed are spread throughout.
        //?


    }
}