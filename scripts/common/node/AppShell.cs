namespace Common;

using Godot;
using System;
using System.Collections.Generic;
/// <summary>
/// The core game manager responsible for handling game state and transitions. Global Root Node.
/// Normally we would want some sort of state orchestratior/scene loader, but we will be handling it inline for simplicity.
/// </summary>
public sealed partial class AppShell : Control
{
    [ExportCategory("References")]
    [ExportGroup("Scenes")]
    [Export] private PackedScene _mainMenuScene;
    [Export] private PackedScene _settingsMenuScene;
    [ExportGroup("Nodes")]
    [Export] private DebugMenu _debugMenu;
    [Export] private Control _gameScreen;
    [Export] private Control _loadingScreen;
    [Export] private Control _crtOverlay;
    [ExportGroup("Shaders")]
    [Export] private ShaderMaterial _defaultCrtMaterial;
    [Export] private ShaderMaterial _pausedCrtMaterial;
    [Export] private ShaderMaterial _bootCrtMaterial;
    [ExportGroup("Cursor")]
    [Export] private AtlasTexture _cursorTexture;
    // *-> State Fields
    private AppState _currentState;
    private AppState _priorState;
    // *-> System References
    private DebugWatcher _debugWatcher;
    private GameManagers _gameManagers;
    private PauseWatcher _pauseWatcher;
    // *-> Loaded References
    private GamePack _loadedPack;
    private PackBase _loadedScene;
    private MainMenu _mainMenu;
    private SettingsMenu _settingsMenu;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _gameManagers = this.AddNode<GameManagers>("game_managers");
        _debugWatcher = this.AddNode<DebugWatcher>("debug_watcher");
        _pauseWatcher = this.AddNode<PauseWatcher>("pause_watcher");
        if (_debugWatcher == null || _gameManagers == null ||  _pauseWatcher == null)
            GD.PrintErr("App: Failed to initialize System Refs Check _EnterTree method for details.");
        else
            GD.Print("App: Successfully initialized AppShell Systems.");
        _mainMenu = _gameScreen.InstanceScene(_mainMenuScene) as MainMenu;
        _settingsMenu = _gameScreen.InstanceScene(_settingsMenuScene) as SettingsMenu;
        _settingsMenu.Visible = false;
        _mainMenu.Scale = new Vector2(2f, 2f);
        _mainMenu.Position = _gameScreen.Position;
        _settingsMenu.Scale = new Vector2(2f, 2f);
        _settingsMenu.Position = _gameScreen.Position;
        _mainMenu.OnStartGame += HandleStartGame;
        _mainMenu.OnSettingsToggle += () => _settingsMenu.Visible = !_settingsMenu.Visible;
        _mainMenu.OnRequestScores += HandleRequestScores;
        _mainMenu.OnQuitGame += () => GetTree().Quit();
        _debugMenu.OnDebugPoints += HandleDebugMenuPoints;
        _debugWatcher.OnToggleDebug += HandleDebugMenu;
        _gameManagers.Settings.OnSettingsUpdated += HandleSettingsUpdated;
        _pauseWatcher.OnTogglePause += HandleTogglePause;
        _gameManagers.Window.SetCustomCursor(_cursorTexture, _gameScreen);
        RequestAppState(AppState.MainMenu);
        _gameManagers.Settings.LoadData();
    }
    // *-> Private Methods
    /// <summary>
    /// Handles transitioning between different application states (Main Menu, In-Game, Loading) by updating the visibility of UI elements, loading/unloading game scenes, and applying appropriate settings based on the new state. This function ensures that the correct game state is active and that resources are managed properly during transitions.
    /// </summary>
    /// <param name="newState">The new application state to transition to.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the new state is not recognized.</exception>
    private void RequestAppState(AppState newState)
    {
        if (_currentState == newState)
            {
                GD.PrintErr($"App: Attempted to change to the same state: {newState}. No state change will occur.");
                return;
            }
        _priorState = _currentState;
        _currentState = newState;
        GD.Print($"App: State has changed, updating... New State: {_currentState}, Prior State: {_priorState}");
        switch (_currentState)
        {
            case AppState.MainMenu:
                GD.Print("App: Switching to Main Menu.");
                if (IsInstanceValid(_loadedScene))
                {
                    _gameManagers.Score.SaveScores(_loadedPack.GameName);
                    _loadedScene.OnScoreSubmission -= _gameManagers.Score.SubmitScore;
                    _loadedScene.OnRequestPackExit -= () => RequestAppState(AppState.MainMenu);
                    _loadedScene.OnRequestUnpause -= HandleTogglePause;
                    _loadedScene.QueueFree();
                    GD.Print("App: Previous game scene freed from memory. Proceeding to main menu.");
                } else
                    GD.Print("App: No existing game scene to free. Proceeding to main menu.");
                _mainMenu.Visible = true;
                _loadingScreen.Visible = false;
                SetBackgroundColor(Colors.Red);
                break;
            case AppState.InPack:
                GD.Print("App: Switching to In-Pack.");
                _loadingScreen.Visible = false;
                _loadedScene.RequestGameState(GameState.Paused);
                _crtOverlay.Material = _defaultCrtMaterial;
                SetBackgroundColor(_loadedPack.GameBackgroundColor);
                break;
            case AppState.Loading:
                GD.Print("App: Switching to Loading Screen.");
                _loadingScreen.Visible = true;
                _mainMenu.Visible = false;
                _crtOverlay.Material = _defaultCrtMaterial;
                SetBackgroundColor(Colors.Blue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    // *-> Event Handlers
    private void HandleDebugMenu()
    {
        _debugMenu.Visible = !_debugMenu.Visible;
    }
    private void HandleDebugMenuPoints()
    {
        if(_loadedScene != null)
            _loadedScene.DebugPoints(1000);
    }
    /// <summary>
    /// Handles getting the current score-table.
    /// </summary>
    /// <param name="pack"></param>
    /// <returns></returns>
    private Godot.Collections.Dictionary<string, uint> HandleRequestScores(GamePack pack)
    {
        _gameManagers.Score.LoadScores(pack.GameName);
        if (_gameManagers.Score.CurrentScores != null)
            return _gameManagers.Score.CurrentScores;
        GD.PrintErr($"App: {pack.GameName}'s score table could not be loaded?");
        return null;
    }
    /// <summary>
    /// Handles actions to take when a game pack is loaded.
    /// </summary>
    /// <param name="pack">GamePack</param>
    private void HandleStartGame(GamePack pack)
    {
        GD.Print($"App: Starting game with pack: {pack.GameName}");
        if (pack == null)
        {
            GD.PrintErr("App: Attempted to load a null GamePack.");
            return;
        }
        if (pack.GameScene == null)
        {
            GD.PrintErr($"App: GamePack '{pack.GameName}' has no scene assigned.");
            return;
        }
        RequestAppState(AppState.Loading);
        _loadedPack = pack;
        _loadedScene = pack.GameScene.Instantiate<PackBase>();
        if (_loadedScene == null)
        {
            GD.PrintErr($"PackRegister: Failed to instantiate scene for GamePack '{pack.GameName}'.");
            return;
        }
        _gameScreen.AddChild(_loadedScene);
        _loadedScene.Position = _gameScreen.Position;
        _loadedScene.OnScoreSubmission += _gameManagers.Score.SubmitScore;
        _loadedScene.OnRequestPackExit += () => RequestAppState(AppState.MainMenu);
        _loadedScene.OnRequestUnpause += HandleTogglePause;
        _gameManagers.Score.LoadScores(pack.GameName);
        RequestAppState(AppState.InPack);
        GD.Print("App: Pack loaded and scene instantiated.");
    }
    /// <summary>
    /// Handles updates to settings when the SettingsManager invokes the OnSettingsUpdated event. The dataBundle parameter contains a tuple with the section of settings.
    /// </summary>
    /// <param name="dataBundle"></param>
    private void HandleSettingsUpdated((Sectional , Dictionary<string, (Variant, bool)>) dataBundle)
    {
        var section = dataBundle.Item1;
        var dict = dataBundle.Item2;
        Action<Dictionary<string, (Variant, bool)>> action = section switch
        {
            Sectional.Audio => UpdateAudioSettings,
            Sectional.User => UpdateUserSettings,
            _ => null
        };
        if (action == null)
        {
            GD.PrintErr($"App: Unrecognized settings section: {section}. No updates applied.");
            return;
        }
        action(dict);
    }
    /// <summary>
    /// Handles toggling the pause state of the game.
    /// </summary>
    private void HandleTogglePause()
    {
        if (_currentState == AppState.MainMenu || _currentState == AppState.Loading)
            return;
        bool isPaused = _crtOverlay.Material == _pausedCrtMaterial;
        if (IsInstanceValid(_loadedScene)
        && _loadedScene.GameStarted == true
        && (_loadedScene.CurrentGameState == GameState.Playing || _loadedScene.CurrentGameState == GameState.Paused))
            _loadedScene.RequestGameState(isPaused ? GameState.Playing : GameState.Paused);
        else
            return;
        _crtOverlay.Material = isPaused ? _defaultCrtMaterial : _pausedCrtMaterial;
        GD.Print($"App: Game paused state toggled. Current State: {_currentState}");
    }
    // *-> Support Functions
    /// <summary>
    /// Sets the background color of the game screen by updating shader parameters on the CRT overlay material. This allows for dynamic background color changes based on the loaded game pack's settings.
    /// </summary>
    /// <param name="color">The color to set as the background.</param>
    private void SetBackgroundColor(Color color)
    {
        _gameScreen.Material.Set("shader_parameter/main_color", color);
        _gameScreen.Material.Set("shader_parameter/second_color", color * 0.6f);
        _gameScreen.Material.Set("shader_paremeter/back_color", color * 0.3f);
    }
    // *-> Update Settings Functions
    /// <summary>
    /// Updates audio settings based on the provided data dictionary. The dictionary contains key-value pairs where the key is the setting name and the value is a tuple of (setting value, isEnabled). This function applies the new settings to the AudioManager instance, allowing for real-time updates to audio channels and volume levels.
    /// </summary>
    /// <param name="data"></param>
    private void UpdateAudioSettings(Dictionary<string, (Variant, bool)> data)
    {
        GD.Print("App: Updating audio settings.");
        var audioInstance = _gameManagers.Audio;
        if (data.TryGetValue("Channel1", out var channel1Data))
        {
            audioInstance.SetAudioAllowed(1, channel1Data.Item2);
            audioInstance.SetChannelVolume(1, (float)channel1Data.Item1);
            GD.Print($"App: Audio settings updated - Channel 1 volume set to {(float)channel1Data.Item1}, Allowed: {channel1Data.Item2}");
        }
        if (data.TryGetValue("Channel2", out var channel2Data))
        {
            audioInstance.SetAudioAllowed(2, channel2Data.Item2);
            audioInstance.SetChannelVolume(2, (float)channel2Data.Item1);
            GD.Print($"App: Audio settings updated - Channel 2 volume set to {(float)channel2Data.Item1}, Allowed: {channel2Data.Item2}");
        }
        if (data.TryGetValue("ChannelMusic", out var channelMusicData))
        {
            audioInstance.SetAudioAllowed(3, channelMusicData.Item2);
            if (channelMusicData.Item2 == false)
                audioInstance.StopChannel(3);
            audioInstance.SetChannelVolume(3, (float)channelMusicData.Item1);
            GD.Print($"App: Audio settings updated - Channel Music volume set to {(float)channelMusicData.Item1}, Allowed: {channelMusicData.Item2}");
        }
        GD.Print("App: Audio settings updated successfully.");
    }
    /// <summary>
    /// Updates user settings based on the provided data dictionary. The dictionary contains key-value pairs where the key is the setting name and the value is a tuple of (setting value, isEnabled). This function can be expanded to apply various user settings such as display options, control mappings, or other preferences as needed.
    /// </summary>
    /// <param name="data"></param>
    private void UpdateUserSettings(Dictionary<string, (Variant, bool)> data)
    {
        GD.Print("App: Updating user settings.");
        if (data.TryGetValue("Username", out var userName))
        {
            GameManagers.Instance.Score.CurrentP1 = userName.Item1.AsString();
            GD.Print($"App: User settings updated - Username set to {userName.Item1.AsString()}");
        }
        if (data.TryGetValue("Resolution", out var resolutionData))
        {
            var resolution = resolutionData.Item1.AsVector2();
            GameManagers.Instance.Window.ApplyWindowSettings(data);
            GD.Print($"App: User settings updated - Resolution set to {resolution.X}x{resolution.Y}");
        }
        if (data.TryGetValue("StretchMode", out var stretchModeData))
        {
            var stretchMode = (Window.ContentScaleModeEnum)stretchModeData.Item1.AsByte();
            GameManagers.Instance.Window.ApplyWindowSettings(data);
            GD.Print($"App: User settings updated - Stretch Mode set to {stretchMode}");
        }
        if (data.TryGetValue("StretchAspect", out var stretchAspectData))
        {
            var stretchAspect = (Window.ContentScaleAspectEnum)stretchAspectData.Item1.AsByte();
            GameManagers.Instance.Window.ApplyWindowSettings(data);
            GD.Print($"App: User settings updated - Stretch Aspect set to {stretchAspect}");
        }
        if (data.TryGetValue("ScaleFactor", out var scaleFactorData))
        {
            var scaleFactor = scaleFactorData.Item1.AsSingle();
            GameManagers.Instance.Window.ApplyWindowSettings(data);
            GD.Print($"App: User settings updated - Scale Factor set to {scaleFactor}");
        }
        GD.Print("App: User settings updated successfully,");
    }
}