namespace Common;

using Godot;
/// <summary>
/// A singleton node responsible for managing core game managers (systems) such as audio, scoring, settings, and pause state. This node is intended to be a child of the AppShell and provides global access to these manager systems throughout the game. It holds the screentree connections to the nodes that the managers (which are not nodes) hold reference to.
/// </summary>
public sealed partial class GameManagers : Node
{
    public static GameManagers Instance { get; private set; }
    public AudioManager Audio { get; private set; }
    public ScoreManager Score { get; private set; }
    public SettingsManager Settings { get; private set; }
    public WindowManager Window { get; private set; }
    public GameManagers()
    {
        GD.Print("GameManagers: Initialized");
        if (Instance != null)
        {
            GD.PrintErr("GameManagers: Attempted to create a second instance of GameManagers.");
            return;
        }
        Instance = this;
        Audio = new AudioManager(
            this.AddNode<AudioStreamPlayer>("AudioChannel1"),
            this.AddNode<AudioStreamPlayer>("AudioChannel2"),
            this.AddNode<AudioStreamPlayer>("AudioChannelMusic"));
        Score = new ScoreManager();
        Settings = new SettingsManager();
        Window = new WindowManager(GetParent() as AspectRatioContainer);
        if (Audio == null || Score == null || Settings == null)
            GD.PrintErr("GameManagers: Failed to initialize one or more core systems. Check constructor for details.");
        else
        GD.Print("GameManagers: All systems initialized");
    }
}