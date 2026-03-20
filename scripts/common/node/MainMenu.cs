namespace Common;

using Godot;
using System;
using Godot.Collections;
/// <summary>
/// Main menu controller.
/// Unlike the pack minigames which have their menu logic handled by their main class,
/// the main menu handles itself as to not clutter the game manager.
/// </summary>
public sealed partial class MainMenu : MenuBase
{
    public event Action<GamePack> OnStartGame;
    public event Func<GamePack, Dictionary<string, uint>> OnRequestScores;
    public event Action OnSettingsToggle;
    public event Action OnQuitGame;
    [ExportGroup("References")]
    [Export] private VBoxContainer _packButtonContainer;
    [Export] private RichTextLabel _packDesc;
    [Export] private Label _selectedPackLabel;
    [ExportGroup("Sounds")]
    [Export] private AudioEvent _menuTheme;
    [ExportGroup("Buttons")]
    [Export] private Button _playButton;
    [Export] private Button _settingsButton;
    [Export] private Button _quitButton;
    private GamePack _selectedGamePack;
    private string _selectedGamePackDesc;
    // *-> Singleton References
    private PackRegister _packRegister;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _packRegister = new PackRegister();
        if (_packRegister == null)
            GD.PrintErr("MainMenu: Failed to initialize PackRegister. Check _Ready method for details.");
        _playButton.Visible = false;
        _selectedPackLabel.Visible = false;
        AudioManager.PlayMusicTrack(_menuTheme);
        LoadPackButtons();
    }
    // *-> Base Overrides
    protected override void ConnectControlEvents()
    {
        _playButton.Pressed += HandlePlayButtonPressed;
        _quitButton.Pressed += () => OnQuitGame?.Invoke();
        _settingsButton.Pressed += () => OnSettingsToggle?.Invoke();
    }
    // *-> Private Methods
    /// <summary>
    /// Loads buttons for each available game pack into the main menu.
    /// </summary>
    private void LoadPackButtons()
    {
        foreach (var packEntry in _packRegister.GamePacks)
        {
            Button packButton = _packButtonContainer.AddNode<Button>();
            packButton.Text = packEntry.Value.GameName;
            packButton.Icon = packEntry.Value.GameIcon;
            packButton.ExpandIcon = true;
            packButton.Set("theme_override_font_sizes/font_size", 16);
            packButton.MouseEntered += () => _packDesc.Text = RequestGamePackScores(packEntry.Value);
            packButton.MouseExited += () => _packDesc.Text = _selectedGamePackDesc;
            packButton.Pressed += () => HandlePackSelected(packEntry.Value);
            packButton.Pressed += () => _playButton.Visible = true;
        }
    }
    /// <summary>
    /// Handles when a pack is selected from the menu.
    /// </summary>
    /// <param name="packName"></param>
    /// <returns></returns>
    private void HandlePackSelected(GamePack pack)
    {
        if (_selectedPackLabel.Visible == false)
            _selectedPackLabel.Visible = true;
            GD.Print($"MainMenu: Pack selected: {pack.GameName}");
            if (pack != null)
                _selectedGamePack = pack;
            _selectedPackLabel.Text = pack.GameName;
            _selectedGamePackDesc = RequestGamePackScores(pack);
            _packDesc.Text = _selectedGamePackDesc;
    }
    /// <summary>
    /// Handles the play button being pressed, starting the game with the selected pack.
    /// </summary>
    private void HandlePlayButtonPressed()
    {
        if (_selectedGamePack == null)
        {
            GD.PrintErr("MainMenu: No game pack selected, cannot start game.");
            return;
        }
        AudioManager.StopChannel(3);
        OnStartGame?.Invoke(_selectedGamePack);
    }
    /// <summary>
    /// Get selected or highlighted pack scores.
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    private string RequestGamePackScores(GamePack pack)
    {
        var text = pack.GameDescription;
        var scores = OnRequestScores?.Invoke(pack);
        text += "\n\n[font_size=24][b]-High Scores-[/b][/font_size]";
        foreach (var item in scores)
            text += $"\n{item.Key} - {item.Value}";
        return text;
    }
}