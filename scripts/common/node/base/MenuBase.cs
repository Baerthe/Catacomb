namespace Common;

using Godot;
public abstract partial class MenuBase : Control
{
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
        ConnectButtonsRecursive(this);
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
    /// <summary>
    /// Clicked a button? Play a sound!
    /// </summary>
    /// <param name="button"></param>
    private void OnAnyButtonPressed(Button button)
    {
        GD.Print($"{Name}: Button {button.Name} pressed.");
        AudioManager.PlayAudioClip(SfxButtonPress);
    }
    /// <summary>
    /// Go through the tree and find every button to wire with OnAnyButtonPressed to.
    /// </summary>
    /// <param name="node"></param>
    private void ConnectButtonsRecursive(Node node)
    {
        foreach (Node button in node.GetChildren(true))
            if (button is Button)
                ((Button)button).Pressed += () => OnAnyButtonPressed(button as Button);
            else if (button is Control)
                ConnectButtonsRecursive(button);
    }
}