namespace Common;

using Godot;
using System;
using System.Collections.Generic;
public sealed class WindowManager
{
    private readonly Window _window;
    public WindowManager(Window window)
    {
        _window = window;
    }
    /// <summary>
    /// Applies the provided window and display settings to the specified Window.
    /// </summary>
    public void ApplyWindowSettings(Dictionary<string, (Variant, bool)> dict)
    {
        if (dict.TryGetValue("Resolution", out var resolutionData))
            _window.ContentScaleSize = (Vector2I)resolutionData.Item1.AsVector2();
        if (dict.TryGetValue("StretchMode", out var stretchModeData))
            _window.ContentScaleMode = (Window.ContentScaleModeEnum)stretchModeData.Item1.AsByte();
        if (dict.TryGetValue("StretchAspect", out var stretchAspectData))
            _window.ContentScaleAspect = (Window.ContentScaleAspectEnum)stretchAspectData.Item1.AsSingle();
        if (dict.TryGetValue("ScaleFactor", out var scaleFactorData))
            _window.ContentScaleFactor = scaleFactorData.Item1.AsSingle();
        if (dict.TryGetValue("ScreenSet", out var screenSetData))
            _window.ContentScaleStretch = (Window.ContentScaleStretchEnum)screenSetData.Item1.AsByte();
        GD.Print($"Window Manager: Updated to {resolutionData}, {stretchModeData}, {stretchAspectData}, {scaleFactorData}, {screenSetData}");
    }
}