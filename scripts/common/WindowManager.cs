namespace Common;

using Godot;
using System;
public sealed class WindowManager
{
    private readonly AspectRatioContainer _app;
    public WindowManager(AspectRatioContainer app)
    {
        _app = app;
        _app.Resized += HandleResized;
        Callable.From(UpdateContainer).CallDeferred();
    }
    public void UpdateContainer()
    {
        for (int i = 0; i < 2; i++)
        {
            if (Mathf.IsEqualApprox(_guiAspectRatio, -1.0f))
                _arc.Ratio = _panel.Size.Aspect();
            else
                _arc.Ratio = Mathf.Min(_panel.Size.Aspect(), _guiAspectRatio);
        }
    }
    private void HandleResized() => Callable.From(UpdateContainer).CallDeferred();
}