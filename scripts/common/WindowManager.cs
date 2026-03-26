namespace Common;

using Godot;
using System.Collections.Generic;
public sealed class WindowManager
{
    private ColorRect _crtScreen;
    private ShaderMaterial _bootCRT;
    private ShaderMaterial _defaultCRT;
    private ShaderMaterial _pausedCRT;
    private Sprite2D _cursorSprite;
    private CursorTracker _cursorTracker;
    private Window _window;
    /// <summary>
    /// Sets the CRT screen shader. This is called in _Ready due to we need the control to exist.
    /// </summary>
    /// <param name="control"></param>
    public void AddScreen(ColorRect control) => _crtScreen = control;
    /// <summary>
    /// Sets the CRT shader materials.
    /// </summary>
    /// <param name="boot"></param>
    /// <param name="def"></param>
    /// <param name="paused"></param>
    public void AddScreenShaders(ShaderMaterial boot, ShaderMaterial def, ShaderMaterial paused)
    {
        GD.Print($"{this}: Adding shaders...");
        _bootCRT = boot;
        _defaultCRT = def;
        _pausedCRT = paused;
    }
    /// <summary>
    /// Sets the window the manager controls. This is called in _Ready due to we need the window to exist.
    /// </summary>
    /// <param name="window"></param>
    public void AddWindow(Window window) => _window = window;
    /// <summary>
    /// Applies the provided window and display settings to the specified Window.
    /// </summary>
    public void ApplyWindowSettings(Dictionary<string, (Variant, bool)> dict)
    {
        if (dict.TryGetValue("Resolution", out var resolutionData))
            _window.Size = (Vector2I)resolutionData.Item1.AsVector2();
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
    /// Set CRT screen to Boot Shader
    /// </summary>
    public void SetCRTShaderBoot() => SetCRTShader(_bootCRT);
    /// <summary>
    /// Set CRT screen to Default Shader
    /// </summary>
    public void SetCRTShaderDefault() => SetCRTShader(_defaultCRT);
    /// <summary>
    /// Set CRT Screen to Paused Shader
    /// </summary>
    public void SetCRTShaderPaused() => SetCRTShader(_pausedCRT);
    /// <summary>
    /// Sets a custom AtlasTexture as the mouse cursor, hiding the OS cursor, and locking its position within the specified bounds.
    /// The sprite is injected just below the CRT overlay Z-index.
    /// </summary>
    public void SetCustomCursor(AtlasTexture texture, Control boundsControl)
    {
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
    /// <summary>
    /// Sets if the custom cursor is visible.
    /// </summary>
    /// <param name="i"></param>
    public void SetCustomCursorVisible(bool i) => _cursorSprite.Visible = i;
    /// <summary>
    /// Helper function to set crt screen material.
    /// </summary>
    /// <param name="shader"></param>
    private void SetCRTShader(ShaderMaterial shader) => _crtScreen.Material = shader;
}