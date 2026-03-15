namespace TennisGame;

using Common;
using System;
using Godot;
/// <summary>
/// Represents the menu in the tennis_game.
/// Handles user input for game settings and starts/cancels games.
/// </summary>
public sealed partial class MenuTennis : MenuBase
{
    public event Action<PlayerType, PlayerType, int, int, int, int, int, Color, Color, Color,int, int> OnGameStart;
    public override event Action OnGameCancel;
    public override event Action OnGameQuit;
    [ExportGroup("Buttons")]
    [Export] private Button _buttonCancel;
    [Export] private Button _buttonPlay;
    [Export] private Button _buttonQuit;
    [Export] private OptionButton _optionPaddle1;
    [Export] private OptionButton _optionPaddle2;
    [ExportGroup("Settings Sliders")]
    [Export] private HSlider _sliderBall;
    [Export] private Label _labelBallSpeed;
    [Export] private HSlider _sliderPaddle1Size;
    [Export] private Label _labelPaddle1Size;
    [Export] private HSlider _sliderPaddle2Size;
    [Export] private Label _labelPaddle2Size;
    [Export] private HSlider _sliderPaddle1Speed;
    [Export] private Label _labelPaddle1Speed;
    [Export] private HSlider _sliderPaddle2Speed;
    [Export] private Label _labelPaddle2Speed;
    [Export] private ColorPickerButton _colorPickerPaddle1;
    [Export] private ColorPickerButton _colorPickerPaddle2;
    [Export] private ColorPickerButton _colorPickerBall;
    [Export] private HSlider _gameTimeSlider;
    [Export] private Label _labelGameTime;
    [Export] private HSlider _maxScoreSlider;
    [Export] private Label _labelMaxScore;
    // *-> Godot Overrides
    public override void _Ready()
    {
        ConfigureColorPicker(_colorPickerPaddle1);
        ConfigureColorPicker(_colorPickerPaddle2);
        ConfigureColorPicker(_colorPickerBall);
        _buttonCancel.Visible = false;
    }
    // *-> Menu Connections
    protected override void ConnectControlEvents()
    {
        // Connect button signals
        _buttonPlay.Pressed += OnButtonPlayPressed;
        _buttonQuit.Pressed += () => OnGameQuit?.Invoke();
        _buttonCancel.Pressed += () => OnGameCancel?.Invoke();
        // Connect slider signals to update labels
        _gameTimeSlider.ValueChanged += value => _labelGameTime.Text = ((int)value).ToString("D4");
        _sliderBall.ValueChanged += value => _labelBallSpeed.Text = ((int)value).ToString("D2");
        _sliderPaddle1Size.ValueChanged += value => _labelPaddle1Size.Text = ((int)value).ToString("D2");
        _sliderPaddle2Size.ValueChanged += value => _labelPaddle2Size.Text = ((int)value).ToString("D2");
        _sliderPaddle1Speed.ValueChanged += value => _labelPaddle1Speed.Text = ((int)value).ToString("D2");
        _sliderPaddle2Speed.ValueChanged += value => _labelPaddle2Speed.Text = ((int)value).ToString("D2");
        _maxScoreSlider.ValueChanged += value => _labelMaxScore.Text = ((int)value).ToString("D2");
        // Initialize labels
        _labelGameTime.Text = ((int)_gameTimeSlider.Value).ToString("D4");
        _labelBallSpeed.Text = ((int)_sliderBall.Value).ToString("D2");
        _labelPaddle1Size.Text = ((int)_sliderPaddle1Size.Value).ToString("D2");
        _labelPaddle2Size.Text = ((int)_sliderPaddle2Size.Value).ToString("D2");
        _labelPaddle1Speed.Text = ((int)_sliderPaddle1Speed.Value).ToString("D2");
        _labelPaddle2Speed.Text = ((int)_sliderPaddle2Speed.Value).ToString("D2");
        _labelMaxScore.Text = ((int)_maxScoreSlider.Value).ToString("D2");
    }
    // *-> Helper Methods
    /// <summary>
    /// Toggles the visibility of the Reset and Cancel buttons.
    /// </summary>
    public void ToggleButtons() => _buttonCancel.Visible = !_buttonCancel.Visible;
    /// <summary>
    /// Handles the Play button press event. Sends the menu settings to start a new game.
    /// </summary>
    private void OnButtonPlayPressed()
    {
        GD.Print("Play button pressed");
        Visible = false;
        OnGameStart?.Invoke(
            (PlayerType)_optionPaddle1.GetSelectedId(),
            (PlayerType)_optionPaddle2.GetSelectedId(),
            (int)_sliderBall.Value,
            (int)_sliderPaddle1Size.Value,
            (int)_sliderPaddle2Size.Value,
            (int)_sliderPaddle1Speed.Value,
            (int)_sliderPaddle2Speed.Value,
            _colorPickerPaddle1.Color,
            _colorPickerPaddle2.Color,
            _colorPickerBall.Color,
            (int)_gameTimeSlider.Value,
            (int)_maxScoreSlider.Value
        );
        GD.Print($"Controllers selected: P1 - {(PlayerType)_optionPaddle1.GetSelectedId()}, P2 - {(PlayerType)_optionPaddle2.GetSelectedId()}");
    }
}