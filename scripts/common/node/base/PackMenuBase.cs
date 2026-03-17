namespace Common;

using Godot;
using System;
/// <summary>
/// Additional abstractions for the pack relevant menus.
/// </summary>
public abstract partial class PackMenuBase : MenuBase
{
    public abstract event Action OnGameCancel;
    public abstract event Action OnGameQuit;
}