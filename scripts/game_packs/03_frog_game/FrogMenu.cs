namespace FrogGame;

using Common;
using Godot;
using System;

public sealed partial class FrogMenu : PackMenuBase
{
    public event Action<byte, Color> OnGameStart;
    public override event Action OnGameCancel;
    public override event Action OnGameQuit;
    [ExportGroup("Buttons")]
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonPlay;
    [Export] private Button _buttonQuit;
    [ExportGroup("Settings")]
    [Export] private HSlider _sliderLives;
    [Export] private Label _labelLives;
    [Export] private ColorPickerButton _colorPickerFrog;
    // *-> Godot Overrides
    public override void _Ready()
    {
        if (_colorPickerFrog != null) ConfigureColorPicker(_colorPickerFrog);
        if (_buttonCancel != null) _buttonCancel.Visible = false;
    }
    // *-> Menu Connections
    protected override void ConnectControlEvents()
    {
        if (_buttonPlay != null) _buttonPlay.Pressed += OnButtonPlayPressed;
        if (_buttonQuit != null) _buttonQuit.Pressed += () => OnGameQuit?.Invoke();
        if (_buttonCancel != null) _buttonCancel.Pressed += () => OnGameCancel?.Invoke();
        if (_sliderLives != null && _labelLives != null)
        {
            _sliderLives.ValueChanged += value => _labelLives.Text = ((int)value).ToString("D2");
            _labelLives.Text = ((int)_sliderLives.Value).ToString("D2");
        }
    }
    public void ToggleButtons()
    {
        if (_buttonCancel != null) _buttonCancel.Visible = !_buttonCancel.Visible;
    }
    // *-> Button Handlers
    private void OnButtonPlayPressed()
    {
        byte lives = _sliderLives != null ? (byte)_sliderLives.Value : (byte)3;
        Color color = _colorPickerFrog != null ? _colorPickerFrog.Color : Colors.White;
        OnGameStart?.Invoke(lives, color);
        Visible = false;
    }
}