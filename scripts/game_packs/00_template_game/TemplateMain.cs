namespace TemplateGame;

using Godot;
using Common;
public sealed partial class TemplateMain : PackBase
{
    [ExportGroup("References")]
    [Export] private TemplateMenu _menu;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _menu.OnGameStart += GameStart;
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
    }
    // *-> Game Methods
    protected override void Tick()
    {
        // Implement per-frame logic here if needed.
    }
    protected override void GameReset()
    {
        // Reset game state to initial conditions here if needed.
    }
    private void GameStart(PlayerType playerType, int exampleValue1, int exampleValue2)
    {
        // Use the settings from the menu to configure the game here.
        // For example:
        GD.Print($"Game Starting with PlayerType: {playerType}, ExampleValue1: {exampleValue1}, ExampleValue2: {exampleValue2}");
        RequestGameState(GameState.Playing);
    }
    // *-> Game State Functions
    protected override void StateGameOver(GameOverReason reason)
    {
        throw new System.NotImplementedException();
    }
    protected override void StatePaused()
    {
        throw new System.NotImplementedException();
    }
    protected override void StatePlaying()
    {
        throw new System.NotImplementedException();
    }
}