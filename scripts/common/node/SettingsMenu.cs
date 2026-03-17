namespace Common;

using Godot;
using System;
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
    protected override void ConnectControlEvents()
    {
        
    }
}