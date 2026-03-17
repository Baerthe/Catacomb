namespace TemplateGame;

using Godot;
using Common;
using System;
public sealed partial class TemplateMenu : PackMenuBase
{
    public event Action<PlayerType, int, int> OnGameStart;
    public override event Action OnGameCancel;
    public override event Action OnGameQuit;
    [ExportGroup("Buttons")]
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonPlay;
    [Export] private Button _buttonQuit;
    [ExportGroup("Settings Options")]
    [Export] private OptionButton _optionPlayerType;
    [ExportGroup("Settings Sliders")]
    [Export] private HSlider _sliderExample;
    [Export] private Label _labelExample;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _buttonCancel.Visible = false;
        _labelExample.Text = ((int)_sliderExample.Value).ToString("D2");
        //...
    }
    // *-> Menu Connections
    protected override void ConnectControlEvents()
    {
        _buttonPlay.Pressed += OnButtonPlayPressed;
        _buttonQuit.Pressed += () => OnGameQuit?.Invoke();
        _buttonCancel.Pressed += () => OnGameCancel?.Invoke();
        _sliderExample.ValueChanged += value => _labelExample.Text = ((int)value).ToString("D2");
    }
    // *-> Button Handlers
    private void OnButtonPlayPressed()
    {
        // Gather settings from UI controls
        var playerType = (PlayerType)_optionPlayerType.Selected;
        var exampleValue = (int)_sliderExample.Value;
        // Raise the OnGameStart event with gathered settings
        OnGameStart?.Invoke(playerType, exampleValue, exampleValue);
        Visible = false;
    }
}