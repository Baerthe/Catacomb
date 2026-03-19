namespace Common;

using Godot;
using System.Collections.Generic;
public sealed class WindowManager
{
    private readonly Window _window;
    private Sprite2D _cursorSprite;
    private CursorTracker _cursorTracker;
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
    /// <summary>
    /// Sets a custom AtlasTexture as the mouse cursor, hiding the OS cursor, and locking its position within the specified bounds.
    /// The sprite is injected just below the CRT overlay Z-index.
    /// </summary>
    public void SetCustomCursor(AtlasTexture texture, Control boundsControl)
    {
        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
        if (_cursorSprite == null)
        {
            _cursorSprite = new Sprite2D
            {
                Name = "CustomCursorSprite",
                ZIndex = 14 // Keeps it strictly under the CRT overlay which is Z-Index 15
            };
            boundsControl.AddChild(_cursorSprite);
        }
        _cursorSprite.Texture = texture;
        if (_cursorTracker == null)
        {
            _cursorTracker = new CursorTracker
            {
                Name = "CustomCursorTracker",
                CursorSprite = _cursorSprite,
                BoundsControl = boundsControl
            };
            boundsControl.AddChild(_cursorTracker);
        }
    }
}