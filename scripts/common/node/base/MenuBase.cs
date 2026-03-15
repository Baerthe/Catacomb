namespace Common;

using Godot;
using System;
public abstract partial class MenuBase : Control
{
    public abstract event Action OnGameCancel;
    public abstract event Action OnGameQuit;
    // *-> Exported Properties
    [ExportGroup("Sounds")]
    [Export] protected AudioEvent SfxButtonPress { get; private set;}
    [Export] protected AudioEvent SfxMenuOpen { get; private set;}
    [Export] protected AudioEvent SfxMenuClose { get; private set;}
    // *-> Managers
    protected AudioManager AudioManager { get; private set;} = GameManagers.Instance.Audio;
    protected ScoreManager ScoreManager { get; private set;} = GameManagers.Instance.Score;
    protected SettingsManager SettingsManager { get; private set;} = GameManagers.Instance.Settings;
    // *-> Godot Overrides
    public override void _EnterTree()
    {
        VisibilityChanged += () =>
        {
            if (Visible)
                AudioManager.PlayAudioClip(SfxMenuOpen);
            else
                AudioManager.PlayAudioClip(SfxMenuClose);
        };
        foreach (Node button in GetChildren(true))
            if (button is Button)
                ((Button)button).Pressed += () => OnAnyButtonPressed(button as Button);
        ConnectControlEvents();
    }
    // *-> Abstract Methods
    /// <summary>
    /// Method called on _EnterTree to connect signals for menu controls. Implemented by derived classes to handle their specific controls.
    /// </summary>
    protected abstract void ConnectControlEvents();
    // *-> Helper Methods
    /// <summary>
    /// Configures the appearance and behavior of a ColorPickerButton.
    /// </summary>
    protected static void ConfigureColorPicker(ColorPickerButton button)
    {
        ColorPicker picker = button.GetPicker();
        picker.AddThemeConstantOverride("sv_width", 100);
        picker.AddThemeConstantOverride("sv_height", 100);
        picker.PresetsVisible = false;
        picker.CanAddSwatches = false;
        picker.SamplerVisible = false;
        picker.ColorModesVisible = false;
        picker.HexVisible = false;
    }
    private void OnAnyButtonPressed(Button button)
    {
        GD.Print($"{Name}: Button {button.Name} pressed.");
        AudioManager.PlayAudioClip(SfxButtonPress);
    }
}