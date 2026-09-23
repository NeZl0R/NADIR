using SkiaSharp;

namespace NADIR.Rendering;

internal sealed class SceneRenderer : IDisposable
{
    private readonly FpsOverlay _fpsOverlay = new();

    public void Render(SKCanvas canvas, double framesPerSecond)
    {
        canvas.Clear(SKColors.Black);
        _fpsOverlay.Draw(canvas, framesPerSecond);
    }

    public void Dispose()
    {
        _fpsOverlay.Dispose();
    }
}
