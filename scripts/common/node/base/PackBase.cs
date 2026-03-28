namespace Common;

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Abstract base class for game packs, providing common functionality and properties that all game packs can utilize.
/// </summary>
public abstract partial class PackBase : Node2D
{
    public event Action OnRequestPackExit;
    public event Action OnRequestUnpause;
    public virtual event Action<byte, uint> OnScoreSubmission;
    public bool EnableStepTicking {get; set;} = false;
    public bool GameStarted {get; private set;} = false;
    // *-> Exported Properties
    [ExportGroup("Sounds")]
    [Export] protected AudioEvent SfxGameOver { get; private set;}
    [Export] protected AudioEvent SfxGameStart { get; private set;}
    [Export] protected AudioEvent MusicGameMain { get; private set;}
    // *-> Properties
    public GameState CurrentGameState { get; private set;} = GameState.Paused;
    public GameState PreviousGameState { get; private set;} = GameState.GameOver;
    // *-> Managers
    protected AudioManager AudioManager { get; private set;} = GameManagers.Instance.Audio;
    protected ScoreManager ScoreManager { get; private set;} = GameManagers.Instance.Score;
    protected SettingsManager SettingsManager { get; private set;} = GameManagers.Instance.Settings;
    // *-> Components
    protected IController Player1Controller { get; set;}
    protected IController Player2Controller { get; set;}
    protected Score Score1 { get; set;}
    protected Score Score2 { get; set;}
    protected GameOverReason GameOverReason { get; set;}
    // *-> Fields
    private bool _isRainbowEffectActive = false;
    private readonly List<Control> _colorableLabels = [];
    private readonly List<Color> _labelColors = [];
    private readonly List<Control> _colorableRects = [];
    private readonly List<Color> _rectColors = [];
    private byte _tickCount = 0;
    // *-> Godot Overrides
    public override void _EnterTree()
    {
        CollectColorableNodes(this);
    }
    public override void _Process(double delta)
    {
        if (CurrentGameState != GameState.Playing)
            return;
        Player1Controller?.Update();
        Player2Controller?.Update();
        if (!EnableStepTicking)
            {
                Tick();
                _tickCount++;
                if (_tickCount >= 10)
            {
                _tickCount = 0;
                InfrequentTick();
            }
            }
    }
    // *-> Virtual Debug
    public virtual void DebugPoints(uint points, byte player = 1)
    {
        if (player == 1)
            Score1.AddPoints(points);
        else
            Score2.AddPoints(points);
    }
    // *-> Virtual Methods
    /// <summary>
    /// Submits the current score to the score manager.
    /// </summary>
    protected virtual void SubmitScore(byte player, Score score) => OnScoreSubmission?.Invoke(player, score.CurrentScore);
    /// <summary>
    /// Toggles a rainbow color cycling effect on game elements.
    /// </summary>
    protected virtual async void ToggleRainbowColorEffect()
    {
        _isRainbowEffectActive = !_isRainbowEffectActive;
        while (_isRainbowEffectActive)
        {
            Color color = new(GD.Randf(), GD.Randf(), GD.Randf());
            foreach (Control label in _colorableLabels)
            {
                if (!IsInstanceValid(label))
                    continue;
                label.AddThemeColorOverride("font_color", color);
            }
            foreach (Control rect in _colorableRects)
            {
                if (!IsInstanceValid(rect))
                    continue;
                ((ColorRect)rect).Color = color;
            }
            await ToSignal(GetTree().CreateTimer(0.1f), "timeout");
        }
        foreach (Control label in _colorableLabels)
        {
            if (!IsInstanceValid(label))
                continue;
            label.AddThemeColorOverride("font_color", _labelColors[_colorableLabels.IndexOf(label)]);
        }
        foreach (Control rect in _colorableRects)
        {
            if (!IsInstanceValid(rect))
                continue;
            ((ColorRect)rect).Color = _rectColors[_colorableRects.IndexOf(rect)];
        }
    }
    // *-> Abstract Methods
    /// <summary>
    /// Reset the current game pack to its initial state.
    /// </summary>
    protected abstract void GameReset();
    // *-> Virtual Methods
    /// <summary>
    /// Game packs called update function. This is called inside of the Godot Process loop.
    /// This allows adding to _Process both without having to call the base and can be called one frame at a time.
    /// </summary>
    public virtual void Tick(){}
    /// <summary>
    /// Game packs called update function that is only called every tenth frame.
    /// </summary>
    public virtual void InfrequentTick(){}
    // *-> State Management
    /// <summary>
    /// Requests a change in the game state, which will trigger the appropriate logic for that state (e.g. pausing, game over screen, etc.).
    /// </summary>
    /// <param name="newState"></param>
    public void RequestGameState(GameState newState)
    {
        if (CurrentGameState == newState)
            return;
        GD.Print($"PackBase: Game state changed to {newState} from {CurrentGameState}.");
        PreviousGameState = CurrentGameState;
        CurrentGameState = newState;
        switch (CurrentGameState)
        {
            case GameState.GameOver:
                StateGameOver(GameOverReason);
                break;
            case GameState.Paused:
                StatePaused();
                GameManagers.Instance.Window.SetCRTShaderPaused();
                GameManagers.Instance.Window.SetCustomCursorVisible(true);
                break;
            case GameState.Playing:
                StatePlaying();
                if (GameStarted == false)
                    {
                    AudioManager.PlayMusicTrack(MusicGameMain);
                    GameStarted = true;
                    }
                GameManagers.Instance.Window.SetCRTShaderDefault();
                GameManagers.Instance.Window.SetCustomCursorVisible(false);
                break;
            case GameState.GameQuit:
                AudioManager.StopChannel(3);
                OnRequestPackExit?.Invoke();
                break;
        }
    }
    // *-> State Handling Functions
    ///<summary>
    /// Janky work around to make sure the ESC button also toggles the crt effect.
    /// </summary>
    protected virtual void InvokeUnpause() => OnRequestUnpause?.Invoke();
    /// <summary>
    /// Should not be called directly. Handle logic for when the game is over, such as displaying the game over screen, submitting scores, and resetting the game. The reason parameter can be used to determine why the game ended (e.g. player lost, time ran out, etc.) and display different messages or handle different logic accordingly.
    /// </summary>
    /// <param name="reason"></param> <summary>
    protected abstract void StateGameOver(GameOverReason reason);
    /// <summary>
    /// Should not be called directly. Pause the current loaded game pack.
    /// </summary>
    protected abstract void StatePaused();
    /// <summary>
    /// Should not be called directly. Handle logic for when the game is in the playing state. Basically what to do when we unpause.
    /// </summary>
    protected abstract void StatePlaying();
    // *-> Support Methods
    private void CollectColorableNodes(Node node)
    {
        for (int i = 0; i < node.GetChildCount(); i++)
        {
            var child = node.GetChild(i);
            if (child is Label label)
            {
                GD.Print($"{Name}: Adding control node {label.Name} to label list.");
                _colorableLabels.Add(label);
                _labelColors.Add(label.GetThemeColor("font_color"));
            }
            else if (child is ColorRect colorRect)
            {
                GD.Print($"{Name}: Adding control node {colorRect.Name} to colorRect list.");
                _colorableRects.Add(colorRect);
                _rectColors.Add(colorRect.Color);
            }
            // Recursively check children
            CollectColorableNodes(child);
        }
    }
}