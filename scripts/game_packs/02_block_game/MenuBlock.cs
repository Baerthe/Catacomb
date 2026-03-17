namespace BlockGame;

using Common;
using Godot;
using System;
/// <summary>
/// Menu UI for BlockGame. Handles game settings configuration and start/cancel/quit actions.
/// </summary>
public sealed partial class MenuBlock : MenuBase
{
    public event Action<PlayerType, int, int, int, Color, Color, int, uint> OnGameStart;
    public override event Action OnGameCancel;
    public override event Action OnGameQuit;
    [ExportGroup("UI References")]
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonPlay;
    [Export] private Button _buttonQuit;
    [Export] private OptionButton _optionPaddle;
    [Export] private HSlider _sliderBall;
    [Export] private Label _labelBall;
    [Export] private HSlider _sliderPaddleSize;
    [Export] private Label _labelPaddleSize;
    [Export] private HSlider _sliderPaddleSpeed;
    [Export] private Label _labelPaddleSpeed;
    [Export] private ColorPickerButton _colorPickerPaddle;
    [Export] private ColorPickerButton _colorPickerBall;
    [Export] private HSlider _gameTimeSlider;
    [Export] private Label _labelGameTime;
    [Export] private HSlider _maxScoreSlider;
    [Export] private Label _labelMaxScore;
    // *-> Godot Overrides
    public override void _Ready()
    {
        ConfigureColorPicker(_colorPickerPaddle);
        ConfigureColorPicker(_colorPickerBall);
        _buttonCancel.Visible = false;
    }
    // *-> Menu Connections
    protected override void ConnectControlEvents()
    {
        _buttonPlay.Pressed += OnButtonPlayPressed;
        _buttonQuit.Pressed += () => OnGameQuit?.Invoke();
        _buttonCancel.Pressed += () => OnGameCancel?.Invoke();
        _sliderBall.ValueChanged += v => _labelBall.Text = ((int)v).ToString("D2");
        _sliderPaddleSize.ValueChanged += v => _labelPaddleSize.Text = ((int)v).ToString("D3");
        _sliderPaddleSpeed.ValueChanged += v => _labelPaddleSpeed.Text = ((int)v).ToString("D4");
        _gameTimeSlider.ValueChanged += v => _labelGameTime.Text = ((int)v).ToString("D4");
        _maxScoreSlider.ValueChanged += v => _labelMaxScore.Text = ((int)v).ToString("D2");
        _labelBall.Text = ((int)_sliderBall.Value).ToString("D2");
        _labelPaddleSize.Text = ((int)_sliderPaddleSize.Value).ToString("D3");
        _labelPaddleSpeed.Text = ((int)_sliderPaddleSpeed.Value).ToString("D4");
        _labelGameTime.Text = ((int)_gameTimeSlider.Value).ToString("D4");
        _labelMaxScore.Text = ((int)_maxScoreSlider.Value).ToString("D2");
    }
    // *-> Helper Methods
    /// <summary>
    /// Toggles the visibility of the Cancel button.
    /// </summary>
    public void ToggleButtons() => _buttonCancel.Visible = !_buttonCancel.Visible;
    /// <summary>
    /// Handles the Play button press. Sends game settings to start a new game.
    /// </summary>
    private void OnButtonPlayPressed()
    {
        GD.Print("BlockGame: Play button pressed");
        Visible = false;
        OnGameStart?.Invoke(
            (PlayerType)_optionPaddle.GetSelectedId(),
            (int)_sliderBall.Value,
            (int)_sliderPaddleSize.Value,
            (int)_sliderPaddleSpeed.Value,
            _colorPickerPaddle.Color,
            _colorPickerBall.Color,
            (int)_gameTimeSlider.Value,
            (uint)_maxScoreSlider.Value
        );
    }
}