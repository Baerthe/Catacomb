namespace Common;

using Godot;
using System;
/// <summary>
/// Main menu controller.
/// Unlike the pack minigames which have their menu logic handled by their main class,
/// the main menu handles itself as to not clutter the game manager.
/// </summary>
public sealed partial class MainMenu : Control
{
    public event Action<GamePack> OnStartGame;
    public event Func<GamePack, string> OnRequestScores;
    public event Action OnQuitGame;
    [ExportGroup("References")]
    [Export] private VBoxContainer _packButtonContainer;
    [Export] private RichTextLabel _packDesc;
    [Export] private Label _selectedPackLabel;
    [Export] private Control _settingsMenu;
    [ExportGroup("Sounds")]
    [Export] private AudioEvent _menuBootUpSound;
    [Export] private AudioEvent _menuTheme;
    [Export] private AudioEvent _sfxButtonPress;
    [Export] private AudioEvent _sfxMenuOpen;
    [ExportGroup("Buttons")]
    [Export] private Button _playButton;
    [Export] private Button _settingsButton;
    [Export] private Button _quitButton;
    private GamePack _selectedGamePack;
    // *-> Singleton References
    private AudioManager _audioManager;
    private PackRegister _packRegister;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _audioManager = GameManagers.Instance.Audio;
        _packRegister = new PackRegister();
        if (_packRegister == null)
            GD.PrintErr("MainMenu: Failed to initialize PackRegister. Check _Ready method for details.");
        _playButton.Pressed += HandlePlayButtonPressed;
        _quitButton.Pressed += () => OnQuitGame?.Invoke();
        _settingsButton.Pressed += () => _settingsMenu.Visible = !_settingsMenu.Visible;
        _playButton.Visible = false;
        _selectedPackLabel.Visible = false;
        MenuLoad();
        LoadPackButtons();
    }
    /// <summary>
    /// Plays the menu theme and boot sound.
    /// </summary>
    public void MenuLoad()
    {
        _audioManager.PlayAudioClip(_sfxMenuOpen);
        _audioManager.PlayMusicTrack(_menuTheme);
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
            packButton.MouseEntered += () => HandlePackHightlighted(packEntry.Value);
            packButton.MouseExited += () => _packDesc.Text = "";
            packButton.Pressed += () => HandlePackSelected(packEntry.Key);
            packButton.Pressed += () => _playButton.Visible = true;
        }
    }
    /// <summary>
    /// Handles when a pack is highlighted from the menu.
    /// </summary>
    /// <param name="pack"></param>
    private void HandlePackHightlighted(GamePack pack)
    {
        var text = pack.GameDescription;
        _packDesc.Text = text;
    }
    private void HandlePackUnhighlighted()
    {
        _packDesc.Text = "";
    }
    /// <summary>
    /// Handles when a pack is selected from the menu.
    /// </summary>
    /// <param name="packName"></param>
    /// <returns></returns>
    private void HandlePackSelected(string packName)
    {
        if (_selectedPackLabel.Visible == false)
            _selectedPackLabel.Visible = true;
        if (_packRegister.GamePacks.TryGetValue(packName, out var pack))
        {
            GD.Print($"MainMenu: Pack selected: {packName}");
            if (pack != null)
                _selectedGamePack = pack;
            _selectedPackLabel.Text = pack.GameName;
        }
        else
        {
            GD.PrintErr($"MainMenu: Failed to select pack: {packName}");
            return;
        }
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
        _audioManager.PlayAudioClip(_sfxButtonPress);
        _audioManager.StopChannel(3);
        OnStartGame?.Invoke(_selectedGamePack);
    }
}