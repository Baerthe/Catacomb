namespace FrogGame;

using Common;
using Godot;
using System;

public sealed partial class FrogMenu : PackMenuBase
{
    public event Action OnGameStart;
    public override event Action OnGameCancel;
    public override event Action OnGameQuit;

    [ExportGroup("Buttons")]
    [Export] private Button _buttonPlay;
    [Export] private Button _buttonQuit;

    // *-> Menu Connections
    protected override void ConnectControlEvents()
    {
        _buttonPlay.Pressed += OnButtonPlayPressed;
        _buttonQuit.Pressed += () => OnGameQuit?.Invoke();
    }

    // *-> Button Handlers
    private void OnButtonPlayPressed()
    {
        OnGameStart?.Invoke();
        Visible = false;
    }
}