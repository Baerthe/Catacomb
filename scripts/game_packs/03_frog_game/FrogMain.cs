namespace FrogGame;

using Godot;
using Common;
using System.Linq;
public sealed partial class FrogMain : PackBase
{
    [ExportGroup("References")]
    [Export] private FrogCharacter _character;
    [Export] private FrogMenu _menu;
    [Export] private Area2D _mapBounds;
    private FrogAI _frogAI;
    private FrogPlayer _controller;
    private FrogMovable[] _movables;
    // *-> Godot Overrides
    public override void _Ready()
    {
        _menu.OnGameStart += () => RequestGameState(GameState.Playing);
        _menu.OnGameCancel += InvokeUnpause;
        _menu.OnGameQuit += () => RequestGameState(GameState.GameQuit);
        _menu.Visible = true;
        _controller = new FrogPlayer(_character);
        _movables = GetTree().GetNodesInGroup("moveable").OfType<FrogMovable>().ToArray();
        _frogAI = new FrogAI(_movables, _mapBounds);
    }
    public override void Tick()
    {
        _controller.Update();
    }
    public override void InfrequentTick()
    {
        _frogAI.Update();
    }
    // *-> Game Methods
    protected override void GameReset()
    {
        // Reset game state to initial conditions here if needed.
    }
    // *-> Game State Functions
    protected override void StateGameOver(GameOverReason reason)
    {
        throw new System.NotImplementedException();
    }
    protected override void StatePaused()
    {
        _menu.Visible = true;
        _character.SetPhysicsProcess(false);
        _frogAI.Freeze();
    }
    protected override void StatePlaying()
    {
        _menu.Visible = false;
        _character.SetPhysicsProcess(true);
        _frogAI.Unfreeze();
    }
}