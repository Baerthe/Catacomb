namespace FrogGame;

using Godot;
using Common;
public sealed partial class FrogMain : PackBase
{
    [ExportGroup("References")]
    [Export] private FrogCharacter _character;
    [Export] private FrogMenu _menu;
    private FrogAI _frogAI;
    private FrogPlayer _controller;
    private FrogMovable[] _movables;
    private byte _gridSize = 16;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _menu.OnGameStart += GameStart;
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
        _character.GridSize = _gridSize;
        _controller = new FrogPlayer(_character);
        _frogAI = new FrogAI(_movables, this);
    }
    public override void Tick()
    {
        _controller.Update();
        _frogAI.Update();
    }
    // *-> Game Methods
    protected override void GameReset()
    {
        // Reset game state to initial conditions here if needed.
    }
    private void GameStart()
    {
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