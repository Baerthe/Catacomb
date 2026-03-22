namespace FrogGame;

using Godot;
using Common;
public sealed partial class FrogMain : PackBase
{
    [ExportGroup("References")]
    [Export] private FrogCharacter _character;
    [Export] private FrogMenu _menu;
    private IController _controller;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _menu.OnGameStart += GameStart;
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
        _controller = new FrogPlayer(_character);
    }
    public override void _PhysicsProcess(double delta)
    {
        _controller.Update();
    }
    // *-> Game Methods
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